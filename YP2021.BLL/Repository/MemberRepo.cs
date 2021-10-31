using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nero2021.Data;
using System.ComponentModel;
using Nero2021.BLL.Models;

namespace Nero2021.BLL.Repository
{
    public class MemberRepo : RepositoryBase<Member, string>
    {
        public enum MemberStatus
        {
            [Description("All")]
            All = 0,
            //[Description("New")]
            //New = 1,
            //[Description("Completed")]
            //Completed = 5,
            //[Description("Cancelled")]
            //Cancelled = 9,
        }

        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable List()
        {
            var list = (from m in db.Member.Where(q => (q.IsDeleted ?? false) == false)
                        join u in db.AspNetUsers on m.ID equals u.Id into u1
                        from u in u1.DefaultIfEmpty()
                        join r in db.AspNetRoles on m.RoleID equals r.Id into r1
                        from r in r1.DefaultIfEmpty()
                        join d in db.Department on m.DepartmentID equals d.ID into d1
                        from d in d1.DefaultIfEmpty()
                        select new
                        {
                            m.ID,
                            m.PhotoPath,
                            u.UserName,
                            m.FirstName,
                            m.LastName,
                            Status = m.IsActive == true ? "Aktif" : "Pasif",
                            //m.SLSMANCODE,
                            Role = r == null ? "" : r.Name,
                            Department = d == null ? "" : d.Name,
                            m.CreatedDT
                        }).AsQueryable();

            return list;
        }

        public Object FindByID(string id)
        {
            try
            {
                //var permissions = (from p in db.Permission.Where(q => q.UserID == id)
                //                   select new { p.ActionID }
                //                               ).ToArray();

                return (from m in db.Member.Where(q => q.ID == id && (q.IsDeleted ?? false) == false)
                        join u in db.AspNetUsers on m.ID equals u.Id into u1
                        from u in u1.DefaultIfEmpty()
                        join r in db.AspNetRoles on m.RoleID equals r.Id into r1
                        from r in r1.DefaultIfEmpty()
                        join d in db.Department on m.DepartmentID equals d.ID into d1
                        from d in d1.DefaultIfEmpty()
                            //join jt in db.JobTitle on m.JobTitleID equals jt.ID into jt1
                            //from jt in jt1.DefaultIfEmpty()

                        select new
                        {
                            m.ID,
                            m.PhotoPath,
                            u.UserName,
                            m.FirstName,
                            m.LastName,
                            m.GSMNumber,
                            m.Email,
                            Status = m.IsActive == true ? "Aktif" : "Pasif",
                            //m.SLSMANCODE,
                            m.RoleID,
                            RoleName = m.AspNetRoles.Name ?? "",
                            m.JobTitleID,
                            JobTitleName = m.JobTitle.Name ?? "", 
                            m.TCIdentityNo,
                            m.StartDate,
                            m.BirthDate,
                            // Role = r == null ? "" : r.Name,
                            m.DepartmentID,
                            // Department = d == null ? "" : d.Name,
                            m.IsActive,
                            m.Skype,

                            ADDED = m.CreatedDT,
                            ADDEDBY = m.Member3.FirstName + " " + m.Member3.LastName,

                            UPDATED = m.UpdatedOn,
                            UPDATEDBY = m.Member2.FirstName + " " + m.Member2.LastName,
                            Permissions = m.Permission.Select(o => o.ActionID).ToList()
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                //TODO: add to log
                throw;
            }
        }

        //public void ChangePassword(ChangePasswordDTO model)
        //{

        //    try
        //    {
        //        string userId = HttpContext.Current.User.Identity.GetUserId();

        //        // string id = User.Identity.GetUserId();
        //        var user = userManager.FindById(model.ID);

        //        if (user == null)
        //            return BadRequest("Kullanıcı bilgisi hatalı. Lütfen tekrar deneyiniz.");

        //        if (!userManager.CheckPassword(user, model.newpassword))
        //        {
        //            return BadRequest("Şifreniz hatalı. Bilgilerinizi doğru girmelisiniz.");
        //            //ModelState.AddModelError("Password", "Incorrect password.");
        //        }

        //        PasswordStrength passwordStrength = PasswordCheck.GetPasswordStrength(model.newpassword);

        //        switch (passwordStrength)
        //        {
        //            case PasswordStrength.Blank:
        //                return BadRequest("Yeni Şifre girmelisiniz!");
        //                break;
        //            case PasswordStrength.VeryWeak:
        //                return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
        //                break;
        //            case PasswordStrength.Weak:
        //                return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
        //                break;
        //            case PasswordStrength.Medium:
        //                return BadRequest("Lütfen kurallara uygun bir şifre giriniz.");
        //                break;
        //            case PasswordStrength.Strong:
        //                break;
        //            case PasswordStrength.VeryStrong:
        //                break;
        //            default:
        //                break;
        //        }

        //        userManager.RemovePassword(userId);
        //        var passwordChangeResult = userManager.AddPassword(userId, model.newpassword);

        //        //string resetToken = userManager.GeneratePasswordResetToken(accountChangePassword.userID);
        //        //IdentityResult passwordChangeResult = userManager.ResetPassword(accountChangePassword.userID, resetToken, accountChangePassword.newpassword);
        //        // var result = userManager.ChangePassword(accountChangePassword.userID, accountChangePassword.newpassword, accountChangePassword.confirmpassword);

        //        if (!passwordChangeResult.Succeeded)
        //        {
        //            return BadRequest("Şifre değiştirilemedi.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}

        //private CozumBuldumDBEntities _db = new CozumBuldumDBEntities();

        //public RegisterUserResult RegisterUser(RegisterUserRequest model)
        //{
        //    RegisterUserResult retval;

        //    try
        //    {
        //        var member = this.GetByFilter(q => q.RegistrationNumber == model.RegisterNumber && q.PhoneNumber == model.PhoneNumber);
        //        if (member == null)
        //        {
        //            Member newMember = new Member()
        //            {
        //                UserID = Guid.NewGuid().ToString(),
        //                RegistrationNumber = model.RegisterNumber,
        //                PhoneNumber = model.PhoneNumber,
        //                RegistrationOTPCode = model.OtpCode,
        //                IsActive = true,
        //                IsDeleted = false
        //            };

        //            new MemberRepo().Insert(newMember);
        //        }
        //        else
        //        {
        //            // silinmiş kullanıcı aktif edilir. Zira Denizbank servisten aktif olduğu bilgisi gelmektedir.
        //            //if (member.IsDeleted == true)
        //            //{                        
        //            //}

        //            member.RegistrationNumber = model.RegisterNumber;
        //            member.PhoneNumber = model.PhoneNumber;
        //            member.RegistrationOTPCode = model.OtpCode;
        //            member.IsActive = true;
        //            member.IsDeleted = false;

        //            new MemberRepo().Update();
        //        }

        //        retval = new RegisterUserResult() { IsSuccess = true, Code = 0, Message = "Ok" };
        //    }
        //    catch (Exception ex)
        //    {
        //        retval = new RegisterUserResult() { IsSuccess = false, Code = 101, Message = "Bir hata oluştu." };
        //    }

        //    return retval;
        //}

        //public RegisterUserResult CheckRegistrationOTP(RegisterUserRequest model)
        //{
        //    RegisterUserResult retval;

        //    try
        //    {
        //        var member = this.GetByFilter(q => q.RegistrationNumber == model.RegisterNumber && q.PhoneNumber == model.PhoneNumber);
        //        if (member == null)
        //            retval = new RegisterUserResult() { IsSuccess = false, Code = 102, Message = "Kullanıcı kaydı bulunamadı." };
        //        else
        //        {
        //            if(member.RegistrationOTPCode == model.OtpCode)
        //                retval = new RegisterUserResult() { IsSuccess = true, Code = 0, Message = "Ok" };
        //            else
        //                retval = new RegisterUserResult() { IsSuccess = false, Code = 103, Message = "Şifre hatası." };
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        retval = new RegisterUserResult() { IsSuccess = false, Code = 101, Message = "Bir hata oluştu." };
        //    }

        //    return retval;
        //}

        //public RegisterUserResult SetUserName(RegisterUserRequest model)
        //{
        //    RegisterUserResult retval;

        //    try
        //    {
        //        if(string.IsNullOrWhiteSpace(model.RegisterNumber))
        //            return new RegisterUserResult() { IsSuccess = false, Code = 112, Message = "Sicil Numarası belirtmelisiniz." };

        //        if (string.IsNullOrWhiteSpace(model.UserName))
        //            return new RegisterUserResult() { IsSuccess = false, Code = 113, Message = "Kullanıcı adı belirtmelisiniz." };



        //        var usernameexists = this.GetByFilter(q => q.RegistrationNumber != model.RegisterNumber && q.UserName == model.UserName);
        //        if(usernameexists != null)
        //            return new RegisterUserResult() { IsSuccess = false, Code = 111, Message = "Kullanıcı adı başka bir kullanıcı tarafından kullanılmaktadır." };


        //        var member = new MemberRepo().GetByFilter(q => q.RegistrationNumber == model.RegisterNumber);
        //        if (member == null)
        //            return new RegisterUserResult() { IsSuccess = false, Code = 110, Message = "Kullanıcı bulunamadı." };

        //        member.UserName = model.UserName;
        //        member.IsActive = true;

        //        new MemberRepo().Update();

        //        retval = new RegisterUserResult() { IsSuccess = true, Code = 100, Message = "Ok" };
        //    }
        //    catch (Exception)
        //    {
        //        retval = new RegisterUserResult() { IsSuccess = false, Code = 101, Message = "Bir hata oluştu." };
        //    }

        //    return retval;
        //} 

        //public IQueryable GetProfile(GetProfileModel model)
        //{
        //    var date = DateTime.Now;
        //    var nextSunday = date.AddDays(7 - (int)date.DayOfWeek);
        //    string suresidolacakalkistarihi = nextSunday.ToShortDateString();

        //    int uyemesajadet = new MessageRepo().MemberMessageCount(model.MemberID);
        //    int uyebildirimadet = new NotificationRepo().MemberNotificationCount(model.MemberID);

        //    var list = new MemberRepo().GetAll(q => q.ID == model.MemberID);

        //    var query = (from m in list
        //                 select new
        //                 {
        //                     memberID = m.ID,
        //                     UserID = m.UserID,
        //                     avatarPath = m.AvatarPath,
        //                     firstName = m.FirstName,
        //                     lastName = m.LastName,
        //                     username = m.UserName,
        //                     alkislananadet = m.SolvingApplauseCount,
        //                     uyealkisadedi = m.ApplauseCount,
        //                     suresidolacakalkistarihi = suresidolacakalkistarihi,
        //                     suresidolacakalkisadedi = (m.ApplauseCount ?? 0) - (m.LastUploadedApplauseCount ?? 0) > 0 ? (m.ApplauseCount ?? 0) - (m.LastUploadedApplauseCount ?? 0) : 0,
        //                     uyemesajadet = uyemesajadet,
        //                     uyebildirimadet = uyebildirimadet
        //                 }).AsQueryable();

        //    return query;
        //}

        //public IQueryable GetMentions(GetMentionsModel model)
        //{
        //    var list = new MemberRepo().GetAll(q => q.IsActive == true && q.UserName.Contains(model.SearchKeyword)).OrderBy(o => o.UserName).Take(50);

        //    var query = (from m in list
        //                 select new
        //                 {
        //                     m.ID,
        //                     m.UserName,
        //                     m.FirstName,
        //                     m.LastName
        //                 }).AsQueryable();

        //    return query;
        //}

        //public class RegisterUserRequest
        //{
        //    public string RegisterNumber { get; set; }
        //    public string PhoneNumber { get; set; }
        //    public string OtpCode { get; set; }
        //    public string UserName { get; set; }
        //}

        //public class RegisterUserResult
        //{
        //    public bool IsSuccess { get; set; }
        //    public int Code { get; set; }
        //    public string Message { get; set; }
        //}


        //public MemberProfil Get(string userId)
        //{
        //    try
        //    {
        //        var data = (from m in _db.Member.Where(q => q.UserID == userId)
        //                    select new MemberProfil()
        //                    {
        //                        ID = m.ID,
        //                        //FullName = m.FullName,
        //                        //BootleCount =  m.BootleCount ?? 0,
        //                        //BalancedPoint = (m.EarnedPoint ?? 0) - (m.BlockedPoint ?? 0) - (m.SpentPoint ?? 0),
        //                        // m.ImagePath
        //                    }).FirstOrDefault();

        //        return data;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}

        //public class MemberProfil
        //{
        //    public int ID { get; set; }
        //    public string FullName { get; set; }
        //    public decimal BootleCount { get; set; }
        //    public decimal BalancedPoint { get; set; }
        //}

    }

}
