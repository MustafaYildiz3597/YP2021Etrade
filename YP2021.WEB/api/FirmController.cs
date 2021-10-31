using Nero2021.BLL.Repository;
using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.IO;
using Nero2021.BLL.Models;
using Nero2021;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Nero2021.BLL.Utilities;

namespace Nero2021.api
{
    //[Authorize]
    public class FirmController : ApiController
    {
        public static readonly UserManager userManager = UserManager.Create();

        [HttpPost]
        public IHttpActionResult DTList()
        {
            try
            {
                return Ok(new FirmRepo().List());
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult CmbList()
        {
            try
            {
                return Ok(new FirmRepo().GetAll(q=> q.Deleted == false || q.Deleted == null).Select( s=> new { s.ID, s.Title }).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult Detail(DetailRQDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                    //    return Content(HttpStatusCode.Unauthorized, "My error message");

                return Ok(new FirmRepo().GetRow(Guid.Parse(model.ID)));

                //return Ok(new FirmRepo().GetByID(Guid.Parse(model.ID)));
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(SaveDTO model)
        {
            
            string username = model.ContactEmail;
            string IdentityUserID = String.Empty;
            string successMessage = String.Empty;
            Guid firmID = Guid.NewGuid();

            try
            {
                if (String.IsNullOrWhiteSpace(model.ID))
                {
                    model.Password = Utility.GeneratePassword(8); 

                    #region new item       

                    successMessage = "Firma eklendi.";

                    var user = userManager.FindByName(model.ContactEmail);
                    if (user != null)
                        return BadRequest("Yetkili E-Posta adresi zaten tanımlı.");

                    var idUser = new IdentityUser()
                    {
                        Email = model.ContactEmail,
                        UserName = model.ContactEmail,
                        PhoneNumber = model.ContactGSMNo
                    };

                    var result = userManager.Create(idUser, model.Password);
                    if (!result.Succeeded)
                        return BadRequest("Firma kaydı oluşturulamadı.");

                    var identityUser = userManager.FindByName(username);

                    if (model.KEPStatus == true)
                    {
                        userManager.SetLockoutEndDate(identityUser.Id, DateTime.Now.AddYears(1));
                        userManager.SetLockoutEnabled(identityUser.Id, false);
                    }
                    else
                    {
                        var lockoutEndDate = new DateTime(2999, 01, 01);
                        userManager.SetLockoutEnabled(identityUser.Id, true);
                        userManager.SetLockoutEndDate(identityUser.Id, lockoutEndDate);
                    }

                    IdentityUserID = identityUser.Id;

                    // ### add to role
                    var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                    IdentityRole role = roleManager.FindByName("İK Yöneticisi");
                    if (role != null)
                        userManager.AddToRole(IdentityUserID, role.Name);
                    // add to role ###

                    
                    var item = new Firm()
                    {
                        ID = firmID, //Guid.Parse(IdentityUserID),
                        CreateUserID = HttpContext.Current.User.Identity.GetUserId(),
                        CreateDT = DateTime.Now,
                        Address = model.Address,
                        City = model.City,
                        ContactEmail = model.ContactEmail,
                        ContactGSMNo = model.ContactGSMNo,
                        ContactName = model.ContactName,
                        Deleted = false,
                        KEPEmail = model.KEPEmail,
                        KEPExpirationDT = model.KEPExpirationDT2,
                        KEPMemberMaxLimit = model.KEPMemberMaxLimit,
                        KEPStatus = model.KEPStatus,
                        PhoneNo = model.PhoneNo,
                        TaxNumber = model.TaxNumber,
                        TaxOffice = model.TaxOffice,
                        Title = model.Title
                    };

                    new FirmRepo().Insert(item);


                    var memberitem = new MemberRepo().Insert(
                      new Member()
                      {
                          ID = IdentityUserID,
                          FirmID = firmID,
                          FirstName = model.ContactName,
                          GSMNumber = model.ContactGSMNo,
                          Email = model.ContactEmail,
                          RoleID = role.Id
                      });


                    #region şifre mail gönder
                    string http = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"] == "0" ? "http://" : "https://";
                    string host = http + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];

                    string emailbody = string.Format(Utility.CreateEmailBody("firma/yeni.html"),
                            model.ContactName,
                            "TürkBelge",
                            //"TürkBelge - Yeni Firma oluşturuldu.", //item.IletiPDF + " " + item.Subject,
                            host,
                            model.ContactEmail,
                            model.Password
                            );

                    // yeni kayıt olduğu için firma yöneticisine mail gönderilir.
                    bool mailsendingresult = Utility.SendMail(model.ContactEmail, "", "TürkBelge - Yeni Firma", emailbody);

                    #endregion //şifre mail gönder 

                    #endregion  //new item
                }
                else
                {
                    #region update item

                    firmID = Guid.Parse(model.ID);

                    successMessage = "Firma güncellendi.";

                    var firm = new FirmRepo().GetByID(firmID);
                    if (firm == null)
                        return BadRequest("Firma kaydı bulunamadı");

                    IdentityUserID = firm.Member1.FirstOrDefault()?.ID;
                    if (String.IsNullOrWhiteSpace(IdentityUserID))
                        return BadRequest("Err:151 - Firmaya ait kullanıcı kaydı bulunamadı. Sistem yöneticinize haber veriniz.");

                    var validuser = userManager.FindById(IdentityUserID);
                    if (validuser == null)
                        return BadRequest("Err:152 - Firmaya ait kullanıcı kaydı bulunamadı. Sistem yöneticinize haber veriniz.");

                    if (model.ContactEmail != firm.ContactEmail) // mail değişmiş
                    {
                        var mailuser = userManager.FindByEmail(model.ContactEmail);

                        if (mailuser != null)
                            return BadRequest("Girdiğiniz Yetkili Eposta adresi ile kayıtlı başka bir firma bulunuyor.");
                    }
                    //else
                    //{
                    //    if (validuser == null)
                    //        return BadRequest("Firma kaydına ulaşılamadı.");

                    //    if (validuser.Id.ToLower() != IdentityUserID.ToLower())
                    //        return BadRequest("Eposta sistemde başka bir firmaya kayıtlı.");
                    //}

                    #region firm update
                    firm.Address = model.Address;
                    firm.City = model.City;
                    firm.ContactEmail = model.ContactEmail;
                    firm.ContactGSMNo = model.ContactGSMNo;
                    firm.ContactName = model.ContactName;
                    firm.KEPEmail = model.KEPEmail;
                    firm.KEPExpirationDT = model.KEPExpirationDT2;
                    firm.KEPMemberMaxLimit = model.KEPMemberMaxLimit;
                    firm.KEPStatus = model.KEPStatus;
                    firm.PhoneNo = model.PhoneNo;
                    firm.TaxNumber = model.TaxNumber;
                    firm.TaxOffice = model.TaxOffice;
                    firm.Title = model.Title;

                    new FirmRepo().Update();
                    #endregion

                    //var user = userManager.FindById(IdentityUserID);

                    // kullanıcı aspnetusers tablosunda aktifliği set edilir.
                    if (model.KEPStatus == true)
                    {
                        userManager.SetLockoutEndDate(IdentityUserID, DateTime.Now.AddYears(1));
                        userManager.SetLockoutEnabled(IdentityUserID, false);
                    }
                    else
                    {
                        var lockoutEndDate = new DateTime(2999, 01, 01);
                        userManager.SetLockoutEnabled(IdentityUserID, true);
                        userManager.SetLockoutEndDate(IdentityUserID, lockoutEndDate);
                    }

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        userManager.RemovePassword(IdentityUserID);
                        var passwordChangeResult = userManager.AddPassword(IdentityUserID, model.Password);
                        if (!passwordChangeResult.Succeeded)
                        {
                            return BadRequest("Şifre değiştirilemedi.");
                        }
                    }

                    validuser.UserName = username;
                    validuser.Email = model.ContactEmail;
                    validuser.PhoneNumber = model.ContactGSMNo;

                    var result = userManager.Update(validuser);
                    if (!result.Succeeded)
                    {
                        if (model.ContactEmail != firm.ContactEmail)
                            return BadRequest("Firma Yetkili Eposta Adresi başka bir kullanıcı adına zaten tanımlı.");
                        else
                            return BadRequest("Firma güncellenemedi.");
                    }

                    // ### add to role
                    var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                    IdentityRole role = roleManager.FindByName("İK Yöneticisi");
                    if (role != null)
                        userManager.AddToRole(IdentityUserID, role.Name);
                    // add to role ###

                    var member = new MemberRepo().GetByID(IdentityUserID);
                    if(member == null)
                        return BadRequest("Firma kaydedildi ancak firma kullanıcı kaydına ulaşılamadı.");

                    member.FirstName = model.ContactName;
                    member.GSMNumber = model.ContactGSMNo;
                    member.Email = model.ContactEmail;
                    member.RoleID = role.Id;
                    new MemberRepo().Update();

                    #endregion
                }


                #region add to role

                //  ***********************
                // ***** BLOKLARA ALINDI bu bölüm ****
                //  **********************

                //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                //IdentityRole role = roleManager.FindByName("İK Yöneticisi");
                //if (role != null)
                //    userManager.AddToRole(IdentityUserID, role.Name);

                // no need.
                //foreach (string item in userManager.GetRoles(IdentityUserID))
                //{
                //    userManager.RemoveFromRoles(IdentityUserID, item);
                //}


                //if (!string.IsNullOrEmpty(model.RoleID))
                //{
                //    IdentityRole role = roleManager.FindById(model.RoleID);

                //    if (role != null)
                //        userManager.AddToRole(IdentityUserID, role.Name);
                //}

                //var memberpermissions = new PermissionRepo().GetAll(q => q.UserID == IdentityUserID);
                //#region remove permissions (modelde bulunmayan permission'lar)
                //foreach (var item in memberpermissions)
                //{
                //    var haspermission = model.Permissions.Any(q => q == item.ActionID);
                //    if (!haspermission)
                //        new PermissionRepo().Delete(item);
                //}
                #endregion

                return Ok(new { ID = firmID, Message = successMessage });
            }
            catch (Exception ex)
            {
                throw;
            }


        }

    }
}
