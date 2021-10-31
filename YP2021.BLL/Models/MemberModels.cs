using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero2021.BLL.Models
{
    public class MemberModels
    {
    }
     
    public class GetProfileModel
    {
        public int MemberID { get; set; }
    }

    public class GetMentionsModel
    {
        public string SearchKeyword { get; set; }
    }

    public class MemberDetailRQDTO
    {
        public string ID { get; set; }
    }

    public class MemberUpdateModel
    {
        public int MemberID { get; set; }
        public string ImagePath { get; set; }
    }

    public class MemberSaveModel
    {
        public string ID { get; set; }
        public string SLSMANCODE { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GSMNumber { get; set; }
        public string Email { get; set; }
        public string DepartmentID { get; set; }
        public string RoleID { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public string  PhotoPath { get; set; }
        public List<string> Permissions { get; set; }
    }

    public class MemberGetDTO
    {
        public string ID { get; set; }
    }

    public class MemberSaveDTO
    {
        public string ID { get; set; }
        public string RoleID { get; set; }
        public string JobTitleID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCIdentityNo { get; set; }
        public string GSMNumber { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public DateTime? StartDateF { get; set; }
        public DateTime? BirthDateF { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string FileName { get; set; }
        public string imgData { get; set; }
        public bool? DeletedResim { get; set; }
        public ChangePasswordDTO user { get; set; }
        public List<SaveMemberUserLevelDTO> UserLevels { get; set; }
        //public string newpassword { get; set; }
        //public string confirmpassword { get; set; }
    }

    public class SaveMemberUserLevelDTO
    {
        public Guid ID { get; set; }
        public string MemberID { get; set; }
        public int UserLevelID { get; set; }
        public bool? Checked { get; set; }
    }

    public class PermissionDTO
    {
        public string ID { get; set; }
    }

    public class RememberPass_OTPCheckPhoneNumberDTO
    {
        public string GSMNumber { get; set; }
    }

    public class RememberPassConfirmOTPDTO
    {
        public Guid VerifyID { get; set; }
        public string OTPCode { get; set; }
    }

    public class UploadProfileImageDTO
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string imgData { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string ID { get; set; }
        public string newpassword { get; set; }
        public string confirmpassword { get; set; }
    }

    

}
