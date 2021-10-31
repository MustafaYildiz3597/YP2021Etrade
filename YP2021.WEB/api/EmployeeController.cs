using Microsoft.AspNet.Identity;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.Data;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Nero2021.api
{
    //[Authorize]
    public class EmployeeController : ApiController
    {
        private NeroDBEntities db = new NeroDBEntities();


        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                string currentUserID = HttpContext.Current.User.Identity.GetUserId();

                string firmID = "";

                if (User.IsInRole("Admin"))
                {
                    firmID = "";
                }
                else
                {
                    firmID = new MemberRepo().GetByID(currentUserID).FirmID.ToString();
                    firmID = String.IsNullOrWhiteSpace(firmID) ? "**" : firmID;
                }

                var list = (from e in db.Employee.Where(q => q.FirmID.ToString() == firmID || firmID == "")
                            select new
                            {
                                e.ID,
                                e.FirstName,
                                e.LastName,
                                e.TCIdentityNo,
                                e.KEPAddress,
                                KEPStatus = e.KEPStatus == true ? "Aktif" : "Pasif",
                                ApprovalStatus = e.ApprovalStatus == true ? "Onaylı" : "Onay Bekliyor",
                                e.SentPayrollsCount,
                                e.CreateDT,
                                CreateFullName = e.Member.FirstName + " " + e.Member.LastName,
                                e.GSMNumber,
                                e.SEPAddress,
                                EmployeeGroupName = e.EmployeeGroup.Name
                            }).OrderBy(o => o.FirstName + " " + o.LastName).ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Detail(EmployeeDetailRQDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                return Ok(new EmployeeRepo().GetRow(Guid.Parse(model.ID)));

                //return Ok(new FirmRepo().GetByID(Guid.Parse(model.ID)));
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult TransferFromExcel(TransferEmployeeFromExcelDTO model)
        {
            string currentUserID = HttpContext.Current.User.Identity.GetUserId();
            bool approvalStatusDefaultVal = true;
            string firmID = "";
            string rolename = "İK Yöneticisi";
            int? excelUploadLogID = null;
            ExcelUploadLog excelUploadLog = null;
            int rownum = 0;
            int succededrownum = 0;
            int totalrowcount = 0;

            try
            {
                if (User.IsInRole("Admin"))
                {
                    approvalStatusDefaultVal = true;
                    rolename = "Admin";
                    firmID = model.FirmID;
                }
                else
                {
                    firmID = new MemberRepo().GetByID(currentUserID).FirmID.ToString();
                }

                if (String.IsNullOrWhiteSpace(firmID))
                    return BadRequest("Geçersiz Firma bilgisi.");

                if (String.IsNullOrWhiteSpace(currentUserID))
                    return BadRequest("Kullanıcı bilgileriniz doğrulanmadı. Lütfen çıkış yaparak ve tekrar üye girişi yapınız.");

                // excel upload log a yaz.
                excelUploadLog = new ExcelUploadLog()
                {
                    FirmID = Guid.Parse(firmID),
                    ExcelUploadEntity = "Employee",
                    HasError = false,
                    CreateDT = DateTime.Now,
                    CreateUserID = currentUserID,
                };

                new ExcelUploadLogRepo().Insert(excelUploadLog);

                excelUploadLogID = excelUploadLog.ID;

                if (model.ExcelRows == null)
                {
                    return BadRequest("Aktarılacak kayıt bulunamadı.");
                }

                totalrowcount = model.ExcelRows.Count();
                if (totalrowcount == 0)
                    return BadRequest("Aktarılacak kayıt bulunamadı.");


                var employeegroups = new EmployeeGroupRepo().GetAll(q => q.FirmID.ToString() == firmID);

                foreach (var item in model.ExcelRows)
                {
                    if (string.IsNullOrWhiteSpace(item.TCIdentityNo) || string.IsNullOrWhiteSpace(item.GSMNumber))
                        continue;

                    Guid? employeeGroupID = null;

                    if (!String.IsNullOrWhiteSpace(item.Birim.Trim()))
                    {
                        var employeegroup = employeegroups.Where(q => q.Name == item.Birim.Trim() && q.FirmID.ToString() == firmID).FirstOrDefault();

                        if (employeegroup == null)
                        {
                            employeeGroupID = Guid.NewGuid();

                            employeegroups.Insert(employeegroups.Count(),
                                new EmployeeGroup()
                            {
                                ID = employeeGroupID ?? Guid.Empty,
                                Name = item.Birim.Trim(),
                                FirmID = Guid.Parse(firmID)
                            });

                            new EmployeeGroupRepo().Insert(
                                new EmployeeGroup()
                                {
                                    ID = employeeGroupID ?? Guid.Empty,
                                    Name = item.Birim.Trim(),
                                    FirmID = Guid.Parse(firmID)
                                });
                        }
                        else
                        {
                            employeeGroupID = employeegroup.ID;
                        }
                    }

                    DateTime? beginDate = null;

                    DateTime dateTime = DateTime.Now;
                    if (DateTime.TryParse(item.BeginDate, out dateTime))
                    {
                        beginDate = dateTime;
                    }

                    var employeeRow = new EmployeeRepo().GetByFilter(q => q.TCIdentityNo == item.TCIdentityNo && q.FirmID.ToString() == firmID);
                    if (employeeRow == null)
                    {
                        Guid employeeID = Guid.NewGuid();
                        employeeRow = new Employee()
                        {
                            ID = employeeID,
                            FirmID = Guid.Parse(firmID),
                            CreateDT = DateTime.Now,
                            CreateUserID = currentUserID,
                            FirstName = item.FirstName,
                            SEPAddress = item.EMailAddress, // item.KEPAddress,
                            KEPAddress = item.EMailAddress, // item.KEPAddress,
                            KEPStatus = true,
                            ApprovalStatus = approvalStatusDefaultVal,
                            LastName = item.LastName,
                            TCIdentityNo = item.TCIdentityNo,
                            GSMNumber = item.GSMNumber,
                            Deleted = false,
                            GroupID = employeeGroupID,
                            StartDate = beginDate
                        };

                        new EmployeeRepo().Insert(employeeRow);

                        // loga yaz
                        new EmployeeLogRepo().Insert(new EmployeeLog()
                        {
                            ActionName = "TransferFromExcel",
                            ApprovalStatus = approvalStatusDefaultVal,
                            ActionType = "Insert",
                            ControllerName = "Employee",
                            CreatedAtRole = rolename,
                            CreateDT = DateTime.Now,
                            CreateUserID = currentUserID,
                            EmployeeID = employeeID,
                            FirmID = Guid.Parse(firmID),
                            HasError = false,
                            LogText = "İşlem Başarılı.",
                            RowJsonStr = new JavaScriptSerializer().Serialize(employeeRow).ToString(),
                            TCIdentityNo = item.TCIdentityNo
                        });

                    }
                    else
                    {
                        employeeRow.FirstName = item.FirstName;
                        employeeRow.LastName = item.LastName;
                        employeeRow.KEPAddress = item.EMailAddress; // item.KEPAddress;
                        employeeRow.KEPStatus = true;
                        employeeRow.SEPAddress = item.EMailAddress;
                        employeeRow.ApprovalStatus = approvalStatusDefaultVal;
                        employeeRow.StartDate = beginDate;
                        employeeRow.GSMNumber = item.GSMNumber;
                        employeeRow.GroupID = employeeGroupID;

                        string rowJsonStr = new JavaScriptSerializer().Serialize(item).ToString();

                        new EmployeeRepo().Update();

                        // loga yaz
                        new EmployeeLogRepo().Insert(new EmployeeLog()
                        {
                            ActionName = "TransferFromExcel",
                            ApprovalStatus = approvalStatusDefaultVal,
                            ActionType = "Update",
                            ControllerName = "Employee",
                            CreatedAtRole = rolename,
                            EmployeeID = employeeRow.ID,
                            FirmID = Guid.Parse(firmID),
                            HasError = false,
                            LogText = "İşlem Başarılı.",
                            RowJsonStr = rowJsonStr,
                            TCIdentityNo = item.TCIdentityNo
                        });

                    }

                    succededrownum++;
                    rownum++;
                }

                var excelUploadLogRow = new ExcelUploadLogRepo().GetByID(excelUploadLogID ?? 0);
                excelUploadLogRow.TotalRowCount = totalrowcount;
                excelUploadLogRow.TransferedRowCount = rownum + 1;
                //excelUploadLogRow.ErrorText = "";
                excelUploadLogRow.HasError = false;
                new ExcelUploadLogRepo().Update();


                return Ok("Excelden kullanıcı dataları başarılı bir şekilde aktarıldı." + "Toplam: " + model.ExcelRows.Count().ToString() + "; Aktarılan: " + succededrownum.ToString());
            }
            catch (Exception ex)
            {
                if (excelUploadLogID != null)
                {
                    var excelUploadLogRow = new ExcelUploadLogRepo().GetByID(excelUploadLogID ?? 0);
                    excelUploadLogRow.TotalRowCount = totalrowcount;
                    excelUploadLogRow.TransferedRowCount = rownum + 1;
                    excelUploadLogRow.ErrorText = "At Err Rownum: " + (rownum + 1).ToString() + " " + ex.GetBaseException().Message;
                    excelUploadLogRow.HasError = true;
                    new ExcelUploadLogRepo().Update();

                    return BadRequest("Aktarım işlemi tamamlanamadı. Hata oluşan satır numarası: " + (rownum + 1).ToString());
                }

                // baseden 
                // admine mail at...
                return BadRequest("Sistem hatası. Sistem yöneticinize iletiniz.");
            }

        }

        [HttpPost]
        public IHttpActionResult Save(EmployeeSaveDTO model)
        {
            string successMessage = String.Empty;
            model.GSMNumber = model.GSMNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            try
            {
                if (String.IsNullOrWhiteSpace(model.ID))
                {
                    #region new item       

                    successMessage = "Personel eklendi.";

                    Guid employeeID = Guid.NewGuid();
                    var item = new Employee()
                    {
                        ID = employeeID,
                        CreateUserID = HttpContext.Current.User.Identity.GetUserId(),
                        CreateDT = DateTime.Now,
                        FirmID = Guid.Parse(model.FirmID),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        KEPAddress = model.KEPAddress,
                        GSMNumber = model.GSMNumber,
                        SEPAddress = model.SEPAddress,
                        SentPayrollsCount = 0,
                        TCIdentityNo = model.TCIdentityNo,
                        KEPStatus = model.KEPStatus,
                        ApprovalStatus = User.IsInRole("Admin") == true ? model.ApprovalStatus : false,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate
                    };
                    new EmployeeRepo().Insert(item);

                    model.ID = item.ID.ToString();
                    #endregion
                }
                else
                {
                    #region update item

                    var itemID = model.ID;

                    successMessage = "Personel güncellendi.";

                    var item = new EmployeeRepo().GetByID(Guid.Parse(itemID));
                    if (item == null)
                        return BadRequest("Personel kaydı bulunamadı");

                    item.FirstName = model.FirstName;
                    item.LastName = model.LastName;
                    item.KEPAddress = model.KEPAddress;
                    item.SEPAddress = model.SEPAddress;
                    item.GSMNumber = model.GSMNumber;
                    item.TCIdentityNo = model.TCIdentityNo;
                    item.KEPStatus = model.KEPStatus;
                    item.StartDate = model.StartDate;
                    item.EndDate = model.EndDate;

                    if (User.IsInRole("Admin") == true)
                        item.ApprovalStatus = model.ApprovalStatus;

                    new EmployeeRepo().Update();

                    #endregion
                }

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                throw;
            }


        }

    }
}
