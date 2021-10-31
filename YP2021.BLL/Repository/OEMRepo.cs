using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nero2021.Data;
using System.ComponentModel;
using Nero2021.BLL.Models;
using System.Web;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Data;

namespace Nero2021.BLL.Repository
{
    public class OEMRepo : RepositoryBase<OEM, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public Result ImportExcelData(OEMImportExcelUploadDataDTO model)
        {
            int totalrowcount = model.Data.Count();

            Result retval = new Result()
            {
                IsSuccess = false,
                StrObj = new List<ResultRow>(),
                StrList = new List<string>(),
                TotalCount = totalrowcount,
                UnProccessedCount = 0,
                SuccededCount = 0,
                AddedCount = 0,
                UpdatedCount = 0
            };

            int? exceluploadID = null;

            try
            {
                var emptyfields = (from t in model.Data.Where(q => q.BUKOD == null || q.BUKOD == "" || q.SUPID == null || q.SUPID == "" || q.OEMNR == null || q.OEMNR == "")
                                   select t
                                   ).ToList();
                if (emptyfields.Count() > 0)
                {
                    retval.Message = "Boş bilgisi bulunan kayıtlar mevcut. Lütfen veriyi kontrol ediniz.";

                    return retval;
                }


                var nonexistsproducts = (from t in model.Data.Where(q => q.BUKOD != null && q.BUKOD != "")
                                         join p in db.PRODUCTS.ToList().Where(q => (q.Deleted ?? false) == false) on t.BUKOD equals p.BUKOD into p1
                                         from p in p1.DefaultIfEmpty()
                                         where p == null
                                         select new
                                         {
                                             t.BUKOD,
                                             t.OEMNR,
                                             pBUKOD = p == null ? "" : p.BUKOD
                                         }
                                         ).ToList();

                if (nonexistsproducts.Count() > 0)
                {
                    retval.Message = "Bulunmayan/Geçersiz BU Numaralı kayıtlar bulundu. Lütfen veriyi kontrol ediniz.";

                    foreach (var item in nonexistsproducts)
                    {
                        string s = item.BUKOD.ToString() + " Bu numaralı ürün kaydı bulunamadı.";
                        retval.StrList.Add(s);
                    }

                    return retval;
                }

                var nonexistssupids = (from t in model.Data
                                       join c in db.OEMSUPNAME.ToList() on t.SUPID equals c.SUPID.ToString() into c1
                                       from c in c1.DefaultIfEmpty()
                                       where c == null
                                       select t
                                        ).ToList();

                if (nonexistssupids.Count() > 0)
                {
                    retval.Message = "OEM tedarikçi firması bulunmayan ya da geçersiz kayıtlar mevcut. Lütfen veriyi kontrol ediniz.";

                    return retval;
                }

                string userID = HttpContext.Current.User.Identity.GetUserId();

                var exceluploadinsert = new ExcelUploadLog()
                {
                    CreateDT = DateTime.Now,
                    CreateUserID = userID,
                    TotalRowCount = totalrowcount,
                    ExcelUploadEntity = "OEM"
                };

                new ExcelUploadLogRepo().Insert(exceluploadinsert);

                exceluploadID = exceluploadinsert.ID;
                string query = String.Empty;
                int? newID = null;

                string sqlconnstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                using (SqlConnection openCon = new SqlConnection(sqlconnstr))
                {
                    openCon.Open();

                    foreach (var item in model.Data)
                    {
                        var oem = new OEMRepo().GetByFilter(q => q.BUKOD == item.BUKOD.Trim() && q.SUPID.ToString() == item.SUPID && q.OEMNR == item.OEMNR.Trim());

                        var productID = new ProductRepo().GetByFilter(q => q.BUKOD == item.BUKOD)?.PRODID;

                        if (oem == null)
                        {
                            query = "INSERT into OEM (PRODID, BUKOD, OEMNR, SUPID, SUPNAME, CreatedOn, CreatedBy, IsDeleted) " +
                                "VALUES (@PRODID, @BUKOD, @OEMNR, @SUPID, @SUPNAME, @CreatedOn, @CreatedBy, 0)";

                            using (SqlCommand queryOEM = new SqlCommand(query))
                            {
                                queryOEM.Connection = openCon;

                                queryOEM.Parameters.Add("@PRODID", SqlDbType.Int).Value = productID;
                                queryOEM.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD.Trim() ?? DBNull.Value;
                                queryOEM.Parameters.Add("@OEMNR", SqlDbType.NVarChar, 255).Value = (object)item.OEMNR.Trim() ?? DBNull.Value;
                                queryOEM.Parameters.Add("@SUPID", SqlDbType.Int).Value = Convert.ToInt32(item.SUPID);
                                queryOEM.Parameters.Add("@SUPNAME", SqlDbType.NVarChar, 255).Value = (object)item.SUPID ?? DBNull.Value;
                                queryOEM.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = DateTime.Now;
                                queryOEM.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 128).Value = userID;

                                queryOEM.ExecuteNonQuery();
                            }

                            retval.AddedCount++;
                            retval.SuccededCount++;
                        }
                        else
                        {
                            //query = "UPDATE t SET t.IsDeleted=0, t.DeletedOn = null, t.DeletedBy = null " +
                            //    " FROM OEM t " +
                            //        " WHERE t.SUPID = @SUPID ";

                            //if (String.IsNullOrWhiteSpace(item.BUKOD))
                            //    query += "AND coalesce(t.BUKOD, '') = '' ";
                            //else
                            //    query += " AND t.BUKOD = @BUKOD ";

                            if (oem.IsDeleted == true)
                            {
                                query = "UPDATE t SET t.IsDeleted=0, t.DeletedOn = null, t.DeletedBy = null " +
                                    " FROM OEM t " +
                                    " WHERE t.OEMID = " + oem.OEMID.ToString();

                                using (SqlCommand queryOEM = new SqlCommand(query))
                                {
                                    queryOEM.Connection = openCon;

                                    //queryOEM.Parameters.Add("@PRODID", SqlDbType.Int).Value = productID;
                                    //queryOEM.Parameters.Add("@BUKOD", SqlDbType.NVarChar, 50).Value = (object)item.BUKOD ?? DBNull.Value;
                                    //queryOEM.Parameters.Add("@OEMNR", SqlDbType.NVarChar, 255).Value = (object)item.OEMNR ?? DBNull.Value;
                                    //queryOEM.Parameters.Add("@SUPID", SqlDbType.Int).Value = Convert.ToInt32(item.SUPID);

                                    queryOEM.ExecuteNonQuery();
                                }

                                retval.UpdatedCount++;
                                retval.SuccededCount++;
                            }


                        }
                    }

                    if (openCon.State == System.Data.ConnectionState.Open) openCon.Close();
                }

                var exceluploadlog = new ExcelUploadLogRepo().GetByID(exceluploadID ?? 0);
                if (exceluploadlog != null)
                {
                    exceluploadlog.CompletionDT = DateTime.Now;
                    exceluploadlog.TransferedRowCount = retval.SuccededCount;

                    var errtext = string.Empty;
                    foreach (var item in retval.StrObj)
                    {
                        errtext += item.Text + " " + item.Description + " ";
                    }

                    exceluploadlog.ErrorText = errtext;

                    new ExcelUploadLogRepo().Update();
                }

                retval.IsSuccess = true;
                retval.Message = "İşlem başarılı!";
                return retval;
            }
            catch (Exception ex)
            {
                var exceluploadlog = new ExcelUploadLogRepo().GetByID(exceluploadID ?? 0);
                if (exceluploadlog != null)
                {
                    exceluploadlog.CompletionDT = DateTime.Now;
                    exceluploadlog.TransferedRowCount = retval.SuccededCount;
                    exceluploadlog.HasError = true;
                    exceluploadlog.ErrorText = ex.GetBaseException().Message;
                    new ExcelUploadLogRepo().Update();
                }

                throw;
            }

        }

    }

}
