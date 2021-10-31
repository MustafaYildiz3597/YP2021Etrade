using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
//using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
using System.Web;
using System.Web.Http;
using Nero2021;
//using static TurkBelge.Controllers.PersonelController;

namespace Nero2021.Web.api
{
    //[Authorize]
    public class MemberController : ApiController
    {

        //public HttpResponseMessage UploadFile(string id, FileUploadModel model)
        //{
        //    var content = new { Serdar = "" };
        //    var json = JsonConvert.SerializeObject(content);

        //    var response = Request.CreateResponse(HttpStatusCode.OK);
        //    response.Content = new StringContent(json, Encoding.UTF8, "application/json");
        //    return response;
        //}

        // public readonly UserManager userManager = UserManager.Create();
        //private   RoleManager<IdentityRole> roleManager;
        // private ApplicationRoleManager roleManager;

        public readonly UserManager userManager = UserManager.Create();

        [HttpPost]
        public IHttpActionResult PostChangePassword(AccountChangePasswordModel accountChangePassword)
        {
            try
            {
                string userId = HttpContext.Current.User.Identity.GetUserId();

                // string id = User.Identity.GetUserId();
                var user = userManager.FindById(userId);

                if (user == null)
                    return BadRequest("Kullanıcı bilgisi hatalı. Lütfen tekrar deneyiniz.");

                if (!userManager.CheckPassword(user, accountChangePassword.password))
                {
                    return BadRequest("Şifreniz hatalı. Bilgilerinizi doğru girmelisiniz.");
                    //ModelState.AddModelError("Password", "Incorrect password.");
                }

                PasswordStrength passwordStrength = PasswordCheck.GetPasswordStrength(accountChangePassword.newpassword);

                switch (passwordStrength)
                {
                    case PasswordStrength.Blank:
                        return BadRequest("Yeni Şifre girmelisiniz!");
                        break;
                    case PasswordStrength.VeryWeak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Weak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Medium:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Strong:
                        break;
                    case PasswordStrength.VeryStrong:
                        break;
                    default:
                        break;
                }




                userManager.RemovePassword(userId);
                var passwordChangeResult = userManager.AddPassword(userId, accountChangePassword.newpassword);

                //string resetToken = userManager.GeneratePasswordResetToken(accountChangePassword.userID);
                //IdentityResult passwordChangeResult = userManager.ResetPassword(accountChangePassword.userID, resetToken, accountChangePassword.newpassword);
                // var result = userManager.ChangePassword(accountChangePassword.userID, accountChangePassword.newpassword, accountChangePassword.confirmpassword);

                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest("Şifre değiştirilemedi.");
                }

                return Ok("Şifre değiştirildi.");
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public class AccountChangePasswordModel
        {
            public string userID { get; set; }
            public string password { get; set; }
            public string newpassword { get; set; }
            public string confirmpassword { get; set; }
        }


        public class RememberPassChangePassDTO
        {
            public string verifiedID { get; set; }
            public string newpassword { get; set; }
            public string confirmpassword { get; set; }
            public string GSMNumber { get; set; }
        }


        #region remember password

        [HttpPost]
        public IHttpActionResult RememberPassOTPCheckPhoneNumber(RememberPass_OTPCheckPhoneNumberDTO model)
        {
            try
            {
                var member = new MemberRepo().GetByFilter(q => q.GSMNumber == model.GSMNumber);

                if (member == null)
                    return BadRequest("Cep telefonuna ait kayıt bulunamadı.");

                string gsmnumber = model.GSMNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                var otplog = new OTPLogRepo().GetByFilter(q => q.EntityID.ToString() == member.ID && q.ProcessTypeID == (int)OTPLogTypes.RememberPassword);
                if (otplog != null)
                    if (otplog.ExpiringTime > DateTime.Now)
                        return BadRequest("Son 3 dakika içerisinde bir talepte bulunmuştunuz. Bekleyiniz ve sonra tekrar deneyiniz.");

                #region checkotp limit 
                DateTime today = DateTime.Now.Date;
                var otpcount = new OTPLogRepo().GetAll(q => q.GsmNumber == gsmnumber && q.RequestTime > today && q.ProcessTypeID == (int)OTPLogTypes.RememberPassword).Count();

                int dailyotplimit = 50;
                if (otpcount > dailyotplimit)
                    return Ok(new { Code = 107, IsSuccess = false, Text = "Gün içerisinde en fazla " + dailyotplimit.ToString() + " kez deneme hakkınız bulunuyor. Limiti geçtiğiniz için yarın tekrar deneyebilirsiniz." });
                #endregion

                string otpkey = Utility.GenerateOTP(4);

#if DEBUG
                otpkey = "1111";
#else                 
                var request = new SmsApi.Messenger("buakademi", "buile951..aka", 2);
                var getAccountResponse = request.Login();
                string smscontent = "İleti görüntülemek için şifreniz: " + otpkey;
                bool smssendingresult = Utility.SendSMS(gsmnumber, smscontent, request);

                if (!smssendingresult)
                    return BadRequest("Şifreniz gönderilemedi. Lütfen tekrar deneyiniz.");
#endif

                Guid newid = Guid.NewGuid();

                new OTPLogRepo().Insert(new OTPLog()
                {
                    ID = newid,
                    ProcessTypeID = (int)OTPLogTypes.RememberPassword,
                    RequestTime = DateTime.Now,
                    UserID = null,
                    GsmNumber = gsmnumber,
                    ExpiringTime = DateTime.Now.AddMinutes(3d),
                    OTPPassword = otpkey,
                    EntityID = Guid.Parse(member.ID)
                });

                return Ok(new { Code = 100, IsSuccess = true, Text = "Ok.", Otpkey = otpkey, OTPVerifyID = newid.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. Tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult RememberPassConfirmOTP(RememberPassConfirmOTPDTO model)
        {
            if (model == null)
                return BadRequest("Geçersiz işlem. Lütfen tekrar deneyiniz.");

            if (model.VerifyID == null || String.IsNullOrWhiteSpace(model.OTPCode))
                return BadRequest("Geçersiz işlem. Lütfen tekrar deneyiniz.");

            try
            {
                Guid otplogID = model.VerifyID;
                var otplog = new OTPLogRepo().GetByID(otplogID);

                if (otplog.OTPPassword != model.OTPCode)
                    return BadRequest("Doğrulama kodu hatalı!");

                if (otplog.ExpiringTime < DateTime.Now)
                    return BadRequest("Doğrulama süresi geçmiş bulunuyor. Yeniden deneyiniz.");

                //if (otplog.UserID != userID)
                //    return Ok(new { Code = 108, IsSuccess = false, Text = "Geçersiz işlem!" });

                Guid verifiedID = Guid.NewGuid();

                otplog.VerifiedID = verifiedID;
                otplog.IsVerified = true; // burada gereksiz
                new OTPLogRepo().Update();

                return Ok(new { VerifiedID = verifiedID });
            }
            catch (Exception ex)
            {
                return BadRequest("bir hata oluştu. Yeniden deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult RememberPassChangePass(RememberPassChangePassDTO accountChangePassword)
        {
            try
            {
                Guid verifiedID = Guid.Parse(accountChangePassword.verifiedID);

                var otplog = new OTPLogRepo().GetByFilter(q => q.VerifiedID == verifiedID);

                if (otplog == null)
                    return BadRequest("İşlem yapılamadı. Lütfen tekrar deneyiniz.");

                string gsmnumber = accountChangePassword.GSMNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

                if (gsmnumber != otplog.GsmNumber)
                    return BadRequest("Veri güvenliği problemi. Lütfen tekrar deneyiniz.");

                string memberID = otplog.EntityID.ToString();
                var member = new MemberRepo().GetByID(memberID.ToString());

                if (member == null)
                    return BadRequest("İşlem yapılamadı. Lütfen tekrar deneyiniz.");

                string userId = member.ID; //HttpContext.Current.User.Identity.GetUserId();

                // string id = User.Identity.GetUserId();
                var user = userManager.FindById(userId);

                if (user == null)
                    return BadRequest("Kullanıcı bilgisi hatalı. Lütfen tekrar deneyiniz.");


                //if (!userManager.CheckPassword(user, accountChangePassword.newpassword))
                //{
                //    return BadRequest("Şifreniz hatalı. Bilgilerinizi doğru girmelisiniz.");
                //    //ModelState.AddModelError("Password", "Incorrect password.");
                //}

                PasswordStrength passwordStrength = PasswordCheck.GetPasswordStrength(accountChangePassword.newpassword);

                switch (passwordStrength)
                {
                    case PasswordStrength.Blank:
                        return BadRequest("Yeni Şifre girmelisiniz!");
                        break;
                    case PasswordStrength.VeryWeak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Weak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Medium:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Strong:
                        break;
                    case PasswordStrength.VeryStrong:
                        break;
                    default:
                        break;
                }

                userManager.RemovePassword(userId);
                var passwordChangeResult = userManager.AddPassword(userId, accountChangePassword.newpassword);

                //string resetToken = userManager.GeneratePasswordResetToken(accountChangePassword.userID);
                //IdentityResult passwordChangeResult = userManager.ResetPassword(accountChangePassword.userID, resetToken, accountChangePassword.newpassword);
                // var result = userManager.ChangePassword(accountChangePassword.userID, accountChangePassword.newpassword, accountChangePassword.confirmpassword);

                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest("Şifre değiştirilemedi.");
                }

                return Ok("Şifreniz değiştirildi.");
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }


        #endregion


        //[HttpPost]
        //public IHttpActionResult Get(MemberGetDTO model)
        //{
        //    try
        //    {
        //        var member = new MemberRepo().FindByID(model.ID);
        //        return Ok(member);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("İşlem hatası. " + ex.Message);
        //    }

        //}

        //[HttpPost]
        //public IHttpActionResult List()
        //{
        //    try
        //    {
        //        var list = new MemberRepo().List();

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("İşlem hatası. " + ex.Message);
        //    }
        //}

        //[HttpPost]
        //public IHttpActionResult FillAllCmb()
        //{
        //    try
        //    {
        //        var departments = new DepartmentRepo().List();
        //        var roles = new MemberRoleRepo().List();
        //        var actions = new ActionRepo().List();

        //        return Ok(new { Departments = departments, Roles = roles, Actions = actions });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Bir hata oluştu. " + ex.Message);
        //    }
        //}

        //[HttpPost]
        //public IHttpActionResult Delete(MemberGetDTO model)
        //{
        //    try
        //    {
        //        string IdentityUserID = model.ID;

        //        var user = userManager.FindById(IdentityUserID);

        //        if(user == null)
        //            return BadRequest("Kullanıcı kaydı bulunamadı!");

        //        foreach (string item in userManager.GetRoles(IdentityUserID))
        //        {
        //            userManager.RemoveFromRoles(IdentityUserID, item);
        //        }

        //        userManager.Delete(user);


        //        var memberpermissions = new PermissionRepo().GetAll(q => q.UserID == IdentityUserID);
        //        #region remove permissions (modelde bulunmayan permission'lar)
        //        foreach (var item in memberpermissions)
        //        {
        //            new PermissionRepo().Delete(item);
        //        }
        //        #endregion

        //        var entity = new MemberRepo().GetByID(IdentityUserID); 
        //        new MemberRepo().Delete(entity);

        //        return Ok("Kullanıcı silindi");

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Bir hata oluştu. " + ex.Message);
        //    }
        //}


        //public void DeleteUser (string IdentityUserID)
        //{
        //    var user = userManager.FindById(IdentityUserID);

        //    if (user == null)
        //        return; 

        //    foreach (string item in userManager.GetRoles(IdentityUserID))
        //    {
        //        userManager.RemoveFromRoles(IdentityUserID, item);
        //    }

        //    userManager.Delete(user);


        //    var memberpermissions = new PermissionRepo().GetAll(q => q.UserID == IdentityUserID);
        //    #region remove permissions (modelde bulunmayan permission'lar)
        //    foreach (var item in memberpermissions)
        //    {
        //        new PermissionRepo().Delete(item);
        //    }
        //    #endregion
        //}


        //[HttpPost]
        //public IHttpActionResult Save(MemberSaveModel model)
        //{
        //    string username = model.Email;
        //    string IdentityUserID = String.Empty;
        //    string successMessage = String.Empty;

        //    try
        //    {
        //        if (String.IsNullOrWhiteSpace(model.ID))
        //        {
        //            #region new member       

        //            successMessage = "Kullanıcı eklendi.";

        //            var user = userManager.FindByName(username);
        //            if (user != null)
        //                return BadRequest("Kullanıcı zaten tanımlı.");

        //            var idUser = new IdentityUser()
        //            {
        //                Email = model.Email,
        //                UserName = username,
        //                PhoneNumber = model.GSMNumber
        //            };

        //            var result = userManager.Create(idUser, model.Password);
        //            if (!result.Succeeded)
        //                return BadRequest("Kullanıcı oluşturulamadı.");

        //            var identityUser = userManager.FindByName(username);

        //            if (model.IsActive == true)
        //            {
        //                userManager.SetLockoutEndDate(identityUser.Id, DateTime.Now.AddYears(1));
        //                userManager.SetLockoutEnabled(identityUser.Id, false);
        //            }
        //            else
        //            {
        //                var lockoutEndDate = new DateTime(2999, 01, 01);
        //                userManager.SetLockoutEnabled(identityUser.Id, true);
        //                userManager.SetLockoutEndDate(identityUser.Id, lockoutEndDate);
        //            }

        //            IdentityUserID = identityUser.Id;

        //            var member = new Member()
        //            {
        //                ID = IdentityUserID,
        //                CreatedBy = Guid.Parse(HttpContext.Current.User.Identity.GetUserId()),
        //                CreatedDT = DateTime.Now,
        //                DepartmentID = model.DepartmentID,
        //                Email = model.Email,
        //                FirstName = model.FirstName,
        //                GSMNumber = model.GSMNumber,
        //                IsActive = model.IsActive,
        //                IsDeleted = false,
        //                LastName = model.LastName,
        //                PhotoPath = String.IsNullOrWhiteSpace(model.PhotoPath) ? "" : "/fileupload/profile/" + IdentityUserID + "/" + model.PhotoPath,
        //                RoleID = model.RoleID,
        //                SLSMANCODE = model.SLSMANCODE
        //            };

        //            new MemberRepo().Insert(member);

        //            #endregion
        //        }
        //        else
        //        {
        //            #region update

        //            IdentityUserID = model.ID;

        //            successMessage = "Kullanıcı güncellendi.";

        //            var member = new MemberRepo().GetByID(IdentityUserID);
        //            if (member == null)
        //                return BadRequest("Kullanıcı bulunamadı");

        //            var validuser = userManager.FindById(model.ID);

        //            if (model.Email != member.Email) // mail değişmiş
        //            {
        //                //if (validuser != null)
        //                //    return BadRequest("Kullanıcı Adı/ Mail başka bir kullanıcı tarafından kullanılıyor.");

        //                //validuser.Email = model.Email;
        //                //validuser.UserName = model.Email;
        //                //userManager.Update(validuser);

        //                //#region create new user 
        //                //var idUser = new IdentityUser()
        //                //{
        //                //    Email = model.Email,
        //                //    UserName = username,
        //                //    PhoneNumber = model.GSMNumber
        //                //};

        //                //var resultNewUser4Update = userManager.Create(idUser, model.Password);
        //                //if (!resultNewUser4Update.Succeeded)
        //                //    return BadRequest("Kullanıcı kaydı oluşturulamadı.");

        //                //var identityUser = userManager.FindByName(username);
        //                //IdentityUserID = identityUser.Id;
        //                //#endregion

        //                //#region delete old user 
        //                //DeleteUser(model.ID);
        //                //#endregion

        //            } 
        //            else
        //            {
        //                if (validuser == null)
        //                    return BadRequest("Kullanıcı kaydına ulaşılamadı.");

        //                if (validuser.Id != IdentityUserID)
        //                    return BadRequest("KullanıcıAdı sistemde başka bir kullanıcı adına kayıtlı.");

        //            }

        //            //var user = userManager.FindById(IdentityUserID);

        //            // kullanıcı aspnetusers tablosunda aktifliği set edilir.
        //            if (model.IsActive == true)
        //            {
        //                userManager.SetLockoutEndDate(IdentityUserID, DateTime.Now.AddYears(1));
        //                userManager.SetLockoutEnabled(IdentityUserID, false);
        //            }
        //            else
        //            {
        //                var lockoutEndDate = new DateTime(2999, 01, 01);
        //                userManager.SetLockoutEnabled(IdentityUserID, true);
        //                userManager.SetLockoutEndDate(IdentityUserID, lockoutEndDate);
        //            }

        //            if (!string.IsNullOrEmpty(model.Password))
        //            {
        //                userManager.RemovePassword(IdentityUserID);
        //                var passwordChangeResult = userManager.AddPassword(IdentityUserID, model.Password);
        //                if (!passwordChangeResult.Succeeded)
        //                {
        //                    return BadRequest("Şifre değiştirilemedi.");
        //                }
        //            }

        //            validuser.UserName = username;
        //            validuser.Email = model.Email;
        //            validuser.PhoneNumber = model.GSMNumber;

        //            var result = userManager.Update(validuser);
        //            if (!result.Succeeded)
        //            {
        //                if (model.Email != member.Email)
        //                    return BadRequest("Kullanıcı/Email başka bir kullanıcı adına zaten tanımlı.");
        //                else
        //                    return BadRequest("Kullanıcı güncellenemedi.");
        //            }

        //            member.DepartmentID = model.DepartmentID;
        //            member.Email = model.Email;
        //            member.FirstName = model.FirstName;
        //            member.GSMNumber = model.GSMNumber;
        //            member.LastName = model.LastName;
        //            member.IsActive = model.IsActive;
        //            member.PhotoPath = String.IsNullOrWhiteSpace(model.PhotoPath) ? "" : "/fileupload/profile/" + IdentityUserID + "/" + model.PhotoPath;
        //            member.RoleID = model.RoleID;
        //            member.SLSMANCODE = model.SLSMANCODE;

        //            new MemberRepo().Update();
        //            #endregion
        //        }


        //        foreach (string item in userManager.GetRoles(IdentityUserID))
        //        {
        //            userManager.RemoveFromRoles(IdentityUserID, item);
        //        }

        //        var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();


        //        if (!string.IsNullOrEmpty(model.RoleID))
        //        {
        //            IdentityRole role = roleManager.FindById(model.RoleID);

        //            if (role != null)
        //                userManager.AddToRole(IdentityUserID, role.Name);
        //        }

        //        var memberpermissions = new PermissionRepo().GetAll(q => q.UserID == IdentityUserID);
        //        #region remove permissions (modelde bulunmayan permission'lar)
        //        foreach (var item in memberpermissions)
        //        {
        //            var haspermission = model.Permissions.Any(q => q == item.ActionID);
        //            if (!haspermission)
        //                new PermissionRepo().Delete(item);
        //        }
        //        #endregion
        //        #region add permission (modelde yeni eklenmiş permission'lar)
        //        foreach (var item in model.Permissions)
        //        {
        //            var haspermission = memberpermissions.Any(q => q.ActionID == item);
        //            if (!haspermission)
        //            {
        //                new PermissionRepo().Insert(new Permission()
        //                {
        //                    ActionID = item,
        //                    UserID = IdentityUserID
        //                });
        //            }
        //        }
        //        #endregion

        //        return Ok(new { ID= IdentityUserID, Message = successMessage} );
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Bir hata oluştu.");
        //    }
        //}



        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable DTList()
        {
            var list = (from m in db.Member.Where(q => (q.IsDeleted ?? false) == false)
                        join r in db.AspNetRoles on m.RoleID equals r.Id into rr
                        from r in rr.DefaultIfEmpty()
                        join jt in db.JobTitle on m.JobTitleID equals jt.ID into jtr
                        from jt in jtr.DefaultIfEmpty()

                        select new
                        {
                            m.ID,
                            PhotoPath = "<img style=\"border-radius: 50 % !important; \" width=\"70\" height=\"70\" src=\"" +
                                (m.PhotoPath.Trim() == null || m.PhotoPath.Trim() == "" ? "/images/bulogo.jpg" : m.PhotoPath) + "\" />",
                            Role = r == null ? "" : r.Name,
                            JobTitle = jt == null ? "" : jt.Name,
                            m.FirstName,
                            m.LastName,
                            // m.TCIdentityNo,
                            m.Email,
                            m.GSMNumber,
                            m.StartDate,

                            //m.ESignerSpentCount,
                            //m.ESignerUsageLimit,
                            //m.ESignerExpiringTime,
                            //ESignerUsageType = m.ESignerUsageType == 1 ? "Adet Kullanım" : "Süreli Kullanım",
                            //ESignerBalancedCount = (m.ESignerUsageLimit ?? 0) - (m.ESignerSpentCount ?? 0),

                            IsActive = m.IsActive == true ? "Aktif" : "Pasif",

                            m.CreatedDT,
                            CreateFullName = m.Member2.FirstName + " " + m.Member2.LastName
                        }).OrderBy(o => o.FirstName + " " + o.LastName).AsQueryable();

            return list;
        }

        [HttpPost]
        public IHttpActionResult Detail(MemberDetailRQDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var member = new MemberRepo().FindByID(model.ID);
                var uselevels = (from ul in db.UserLevels.Where(q => (q.IsDeleted ?? false) == false && (q.IsActive ?? true) == true)
                                 join mu in db.MemberUserLevel.Where(q => q.MemberID == model.ID) on ul.UserLevelID equals mu.UserLevelID into muj
                                 from mu in muj.DefaultIfEmpty()
                                 select new
                                 {
                                     ul.UserLevelID,
                                     ul.UserLevelName,
                                     Rank = ul.UserLevelID <= 0 ? -1 : 1,
                                     Checked = mu == null ? false : true
                                 }).OrderBy(o => o.Rank).ThenBy(o => o.UserLevelName).ToList();

                return Ok(new { Member = member, UserLevels = uselevels });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(MemberGetDTO model)
        {
            try
            {
                string IdentityUserID = model.ID;

                var user = userManager.FindById(IdentityUserID);

                if (user != null)
                    userManager.Delete(user);

                // return BadRequest("Kullanıcı kaydı bulunamadı!");

                //foreach (string item in userManager.GetRoles(IdentityUserID))
                //{
                //    userManager.RemoveFromRoles(IdentityUserID, item);
                //}

                var memberpermissions = new PermissionRepo().GetAll(q => q.UserID == IdentityUserID);
                #region remove permissions (modelde bulunmayan permission'lar)
                foreach (var item in memberpermissions)
                {
                    new PermissionRepo().Delete(item);
                }
                #endregion

                var member = new MemberRepo().GetByID(IdentityUserID);
                if (member != null)
                {
                    member.IsDeleted = true;
                    new MemberRepo().Update();
                    //new MemberRepo().Delete(member);
                }

                return Ok("Kullanıcı silindi");

            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        // adminin imzacı/müşteri eklemesi.
        public IHttpActionResult Save(MemberSaveDTO model)
        {
            string userId = HttpContext.Current.User.Identity.GetUserId();

            model.GSMNumber = model.GSMNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            string successMessage = String.Empty;
            string roleID = String.Empty;
            string IdentityUserID = String.Empty;
            //string username = model.Email;

            /*check change password criteria */
            if (!string.IsNullOrWhiteSpace(model.user.newpassword))
            {
                var user = userManager.FindById(model.ID);

                if (user == null)
                    return BadRequest("Kullanıcı bilgisi hatalı. Lütfen tekrar deneyiniz.");

                //if (!userManager.CheckPassword(user, model.user.newpassword))
                //{
                //    return BadRequest("Şifreniz hatalı. Bilgilerinizi doğru girmelisiniz.");
                //    //ModelState.AddModelError("Password", "Incorrect password.");
                //}

                PasswordStrength passwordStrength = PasswordCheck.GetPasswordStrength(model.user.newpassword);

                switch (passwordStrength)
                {
                    case PasswordStrength.Blank:
                        return BadRequest("Yeni Şifre girmelisiniz!");
                        break;
                    case PasswordStrength.VeryWeak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Weak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Medium:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Strong:
                        break;
                    case PasswordStrength.VeryStrong:
                        break;
                    default:
                        break;
                }
            }


            try
            {
                //if (!User.IsInRole("Admin"))
                //    return BadRequest("Bu işlem için yetkiniz bulunmuyor.");

                //roleID = new AspNetRolesRepo().GetByFilter(q => q.Name == "Imzacı")?.Id;

                //if (String.IsNullOrWhiteSpace(roleID))
                //    return BadRequest("İmzacı Role kaydına ulaşılamadı. Sistem yöneticinize haber veriniz.");

                if (String.IsNullOrWhiteSpace(model.ID))
                {
                    #region new item       

                    successMessage = "Kullanıcı eklendi.";

                    //var member = new MemberRepo().GetByFilter(q => q.TCIdentityNo == model.TCIdentityNo && q.IsESigner == true);
                    //if (member != null)
                    //    return BadRequest("TC kimlik No ya ait daha önceden kayıtlı kullanıcı bulunmaktadır.");

                    var user = userManager.FindByName(model.UserName);
                    if (user != null)
                        return BadRequest("Kullanıcı Adı zaten tanımlı.");

                    var idUser = new IdentityUser()
                    {
                        Email = model.Email,
                        UserName = model.UserName,
                        PhoneNumber = model.GSMNumber,
                        LockoutEndDateUtc = DateTime.Now.AddYears(20),
                        LockoutEnabled = !model.IsActive
                    };

                    //var appuser = new Models.ApplicationUser
                    //{
                    //    FirstName = model.FirstName,
                    //    LastName = model.LastName,
                    //    PhoneNumber = "123123123",
                    //    UserName = model.Email,
                    //    Email = model.Email,
                    //};
                    //var result = userManager.Create(appuser, model.Password);

                    

                    var userrole = idUser.Roles.FirstOrDefault();
                    if (userrole?.RoleId != model.RoleID || model.RoleID == "")
                        idUser.Roles.Remove(userrole);

                    if (model.RoleID != "")
                        idUser.Roles.Add(new IdentityUserRole() { RoleId = model.RoleID, UserId = idUser.Id });

                    var result = userManager.Create(idUser, model.Password);
                    if (!result.Succeeded)
                        return BadRequest("Personel kaydı oluşturulamadı.");


                    //IdentityUserID = identityUser.Id;

                    //var userrole = idUser.Roles.FirstOrDefault();
                    //if (userrole?.RoleId != model.RoleID || model.RoleID == "") 
                    //    idUser.Roles.Remove(userrole);

                    //if (model.RoleID != "")
                    //    idUser.Roles.Add(new IdentityUserRole() { RoleId = model.RoleID, UserId = idUser.Id });

                    // ### add to role
                    //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                    //IdentityRole role = roleManager.FindById(model.ID.ToString());
                    //if (role != null)
                    //    userManager.AddToRole(IdentityUserID, role.Name);
                    // add to role ###



                    var item = new Member()
                    {
                        ID = idUser.Id,
                        CreatedBy = userId,
                        CreatedDT = DateTime.Now,
                        TCIdentityNo = model.TCIdentityNo,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        GSMNumber = model.GSMNumber,
                        Skype = model.Skype,
                        JobTitleID = string.IsNullOrWhiteSpace(model.JobTitleID) ? Guid.Empty : Guid.Parse(model.JobTitleID),
                        RoleID = model.RoleID,
                        StartDate = model.StartDateF,
                        BirthDate = model.BirthDateF,
                        IsActive = model.IsActive,
                        PhotoPath = (!String.IsNullOrWhiteSpace(model.imgData) ? Path.Combine("/FileUpload/ProfileImg/" + idUser.Id + "/", model.FileName) : null)
                    };
                    new MemberRepo().Insert(item);

                    model.ID = item.ID.ToString();

                    #endregion
                }
                else
                {
                    #region update item

                    var itemID = model.ID;
                    IdentityUserID = model.ID;

                    if (String.IsNullOrWhiteSpace(IdentityUserID))
                        return BadRequest("Kullanıcı kaydı bulunamadı.");

                    var validuser = userManager.FindById(IdentityUserID);
                    if (validuser == null)
                        return BadRequest("Kullanıcı kaydı bulunamadı.");

                    #region check username

                    if (string.IsNullOrWhiteSpace(model.UserName))
                        return BadRequest("Kullanıcı Adı girmelisiniz.");


                    if (validuser.UserName != model.UserName)
                    {
                        var hasuser = userManager.FindByName(model.UserName);
                        if (hasuser != null)
                            return BadRequest("Kullanıcı Adı başka bir kullanıcı için tanımlı. Başka bir Kullanıcı Adı tanımlayınız.");
                    }

                    #endregion


                    successMessage = "Kullanıcı güncellendi.";

                    var member = new MemberRepo().GetByFilter(q => q.TCIdentityNo == model.TCIdentityNo && q.IsESigner == true && q.ID != model.ID);
                    if (member != null)
                        return BadRequest("TC kimlik No ya ait daha önceden kayıtlı kullanıcı bulunmaktadır.");

                    var item = new MemberRepo().GetByID(itemID);
                    if (item == null)
                        return BadRequest("Kullanıcı kaydı bulunamadı");


                    // birden fazla user aynı maile sahip olabilir. 
                    //if (model.Email != item.Email) // mail değişmiş
                    //{
                    //    var mailuser = userManager.FindByEmail(model.Email);

                    //    if (mailuser != null)
                    //        return BadRequest("Girdiğiniz Yetkili Eposta adresi ile kayıtlı başka bir kişi bulunuyor.");
                    //}


                    // kullanıcı aspnetusers tablosunda aktifliği set edilir.
                    //if (model.IsActive == true)
                    //{
                    //    userManager.SetLockoutEndDate(IdentityUserID, DateTime.Now.AddYears(1));
                    //    userManager.SetLockoutEnabled(IdentityUserID, false);
                    //}
                    //else
                    //{
                    //    var lockoutEndDate = new DateTime(2999, 01, 01);
                    //    userManager.SetLockoutEnabled(IdentityUserID, true);
                    //    userManager.SetLockoutEndDate(IdentityUserID, lockoutEndDate);
                    //}

                    #region password adding section ayrı in other methods 
                    //if (!string.IsNullOrEmpty(model.Password))
                    //{
                    //    userManager.RemovePassword(IdentityUserID);
                    //    var passwordChangeResult = userManager.AddPassword(IdentityUserID, model.Password);
                    //    if (!passwordChangeResult.Succeeded)
                    //    {
                    //        return BadRequest("Şifre değiştirilemedi.");
                    //    }
                    //}
                    #endregion

                    validuser.UserName = model.UserName;
                    validuser.Email = model.Email;
                    validuser.PhoneNumber = model.GSMNumber;
                    validuser.LockoutEnabled = !model.IsActive;

                    var userrole = validuser.Roles.FirstOrDefault();
                    if (userrole?.RoleId != model.RoleID || string.IsNullOrWhiteSpace(model.RoleID))
                    {
                        validuser.Roles.Clear();

                        if (!string.IsNullOrWhiteSpace(model.RoleID))
                            validuser.Roles.Add(new IdentityUserRole() { RoleId = model.RoleID, UserId = validuser.Id });
                    }


                    var result = userManager.Update(validuser);
                    if (!result.Succeeded)
                    {
                        //if (model.Email != item.Email)
                        //    return BadRequest("Eposta Adresi başka bir kullanıcı adına zaten tanımlı.");
                        //else
                        return BadRequest("Kullanıcı güncellenemedi.");
                    }

                    // ### add to role
                    //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

                    //IdentityRole role = roleManager.FindByName("Imzacı");
                    //if (role != null)
                    //    userManager.AddToRole(IdentityUserID, role.Name);
                    // add to role ###

                    item.TCIdentityNo = model.TCIdentityNo;
                    item.FirstName = model.FirstName;
                    item.LastName = model.LastName;
                    item.Email = model.Email;
                    item.GSMNumber = model.GSMNumber;
                    item.Skype = model.Skype;
                    item.JobTitleID = model.JobTitleID == null ? Guid.Empty : Guid.Parse(model.JobTitleID);
                    item.RoleID = model.RoleID;
                    item.StartDate = model.StartDateF;
                    item.BirthDate = model.BirthDateF;
                    item.IsActive = model.IsActive;
                    item.UpdatedBy = userId;
                    item.UpdatedOn = DateTime.Now;

                    if (model.DeletedResim == true)
                        item.PhotoPath = null;
                    else
                        item.PhotoPath = !String.IsNullOrWhiteSpace(model.imgData) ? Path.Combine("/FileUpload/ProfileImg/" + model.ID + "/", model.FileName) : item.PhotoPath;


                    new MemberRepo().Update();

                    #endregion
                }

                #region save memberuserlevels 
                if (model.UserLevels != null)
                {
                    foreach (var item in model.UserLevels)
                    {
                        var userlevel = new MemberUserLevelRepo().GetByFilter(q => q.UserLevelID == item.UserLevelID && q.MemberID == model.ID);

                        if (userlevel == null)
                        {
                            if (item.Checked == true)
                            {
                                var newitem = new MemberUserLevel()
                                {
                                    ID = Guid.NewGuid(),
                                    MemberID = model.ID,
                                    UserLevelID = item.UserLevelID,
                                    CreatedBy = userId,
                                    CreatedOn = DateTime.Now,
                                    IsDeleted = false
                                };
                                new MemberUserLevelRepo().Insert(newitem);
                            }
                        }
                        else
                        {
                            if (item.Checked == false)
                            {
                                new MemberUserLevelRepo().Delete(userlevel);
                            }
                        }
                    }
                }
                #endregion


                //save image
                if (!string.IsNullOrWhiteSpace(model.imgData))
                {
                    //var member = new MemberRepo().GetByID(model.ID);
                    //if (member == null)
                    //    return BadRequest("Personel kaydı bulunamadı.");

                    #region save image 

                    if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/")))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"));


                    string path = Path.Combine(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"), model.FileName);

                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(model.imgData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                    #endregion

                    //member.PhotoPath = Path.Combine("/FileUpload/ProfileImg/" + model.ID + "/", model.FileName);
                    //new MemberRepo().Update();
                }

                //change password
                if (!string.IsNullOrWhiteSpace(model.user.newpassword))
                {
                    userManager.RemovePassword(IdentityUserID);
                    var passwordChangeResult = userManager.AddPassword(IdentityUserID, model.user.newpassword);

                    if (!passwordChangeResult.Succeeded)
                    {
                        return BadRequest("Yeni Şifre kaydedilirken bir hata oluştu.");
                    }
                }

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                return BadRequest("İşleminiz yapılırken bir hata oluştu.");
                // throw;
            }


        }

        [HttpPost]
        public IHttpActionResult FillAllCmb()
        {
            try
            {
                //var departments = new DepartmentRepo().List();
                var roles = new MemberRoleRepo().List();
                var jobtitles = new JobTitleRepo().List();

                return Ok(new { Roles = roles, JobTitles = jobtitles });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }



        [HttpPost]
        public IHttpActionResult UploadProfileImage(UploadProfileImageDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası.");

                if (
                    string.IsNullOrWhiteSpace(model.ID) ||
                    string.IsNullOrWhiteSpace(model.FileName) ||
                    string.IsNullOrWhiteSpace(model.imgData)
                    )
                    return BadRequest("Parametre hatası.");


                var member = new MemberRepo().GetByID(model.ID);
                if (member == null)
                    return BadRequest("Personel kaydı bulunamadı.");


                #region save image 

                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/")))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"));


                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/FileUpload/ProfileImg/" + model.ID + "/"), model.FileName);

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(model.imgData);
                        bw.Write(data);
                        bw.Close();
                    }
                }
                #endregion

                member.PhotoPath = Path.Combine("/FileUpload/ProfileImg/" + model.ID + "/", model.FileName);
                new MemberRepo().Update();

                return Ok("Resim yükleme işlemi tamamlandı.");
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult ChangePassword(ChangePasswordDTO model)
        {
            try
            {
                string userId = HttpContext.Current.User.Identity.GetUserId();

                // string id = User.Identity.GetUserId();
                var user = userManager.FindById(model.ID);

                if (user == null)
                    return BadRequest("Kullanıcı bilgisi hatalı. Lütfen tekrar deneyiniz.");

                if (!userManager.CheckPassword(user, model.newpassword))
                {
                    return BadRequest("Şifreniz hatalı. Bilgilerinizi doğru girmelisiniz.");
                    //ModelState.AddModelError("Password", "Incorrect password.");
                }

                PasswordStrength passwordStrength = PasswordCheck.GetPasswordStrength(model.newpassword);

                switch (passwordStrength)
                {
                    case PasswordStrength.Blank:
                        return BadRequest("Yeni Şifre girmelisiniz!");
                        break;
                    case PasswordStrength.VeryWeak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Weak:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Medium:
                        return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
                        break;
                    case PasswordStrength.Strong:
                        break;
                    case PasswordStrength.VeryStrong:
                        break;
                    default:
                        break;
                }

                userManager.RemovePassword(userId);
                var passwordChangeResult = userManager.AddPassword(userId, model.newpassword);

                //string resetToken = userManager.GeneratePasswordResetToken(accountChangePassword.userID);
                //IdentityResult passwordChangeResult = userManager.ResetPassword(accountChangePassword.userID, resetToken, accountChangePassword.newpassword);
                // var result = userManager.ChangePassword(accountChangePassword.userID, accountChangePassword.newpassword, accountChangePassword.confirmpassword);

                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest("Şifre değiştirilemedi.");
                }

                return Ok("Şifre değiştirildi.");
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }



    }
}
