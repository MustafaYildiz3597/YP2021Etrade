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
    public enum SignerStatus
    {
        [Description("İşlem Yok")]
        NotStarted = 0,
        [Description("İmzada")]
        OnSign = 1,
        [Description("İmzalandı")]
        Signed = 2,
        [Description("İmza Hatalı")]
        SignFailed = 3,
        [Description("İmza Red")]
        SignDeclined = 9,
    }

    public enum FileStatus
    {
        [Description("İşlem Yok")]
        NotStarted = 1,
        [Description("Aktif")]
        Activated = 2,
        [Description("Tamamlandı")]
        Completed = 3,
        [Description("Durduruldu")]
        Paused = 9,
    }

    public enum SignType
    {
        [Description("BES")]
        BES = 1,
        [Description("ZD")]
        ZD = 2
    }

    public enum SigningType
    {
        [Description("Tek")]
        Tek = 1,
        [Description("Müşterek")]
        Müşterek = 2
    }


    public class ImzaciModels
    {
    }

    public class GetSignersDTO
    {
        public string ID { get; set; }
    }

    public class ImzaciDetailRQDTO
    {
        public string ID { get; set; }
    }

    public class DeleteDocumentDTO
    {
        public string ID { get; set; }
    }

    public class MovedownSignerDTO
    {
        public string ID { get; set; }
        public string ImzaciFileLogID { get; set; }
    }
    
    public class MoveupSignerDTO
    {
        public string ID { get; set; }
        public string ImzaciFileLogID { get; set; }
    }

    public class ImzaciSaveDTO
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCIdentityNo { get; set; }
        public string GSMNumber { get; set; }
        public string Email { get; set; }
        public DateTime? ESignerExpiringTime { get; set; }
        public int ESignerUsageType { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
    }

    public class ImzaciStartProcessDTO
    {
        public string ImzaciFileLogID { get; set; }
    }

    public class GetImzaciFileSignerIDDTO
    {
        public string ImzaciFileSignerID { get; set; }
        public string ImzaciFileLogID { get; set; }
    }

    public class SignDocDTO
    {
        //public string pin { get; set; }
        //public string crt { get; set; }
        public string sid { get; set; }
    }
    public class GetFileDTO
    {
        public string sid { get; set; }
        public string tokenid { get; set; }
    }
   



    public class AddCountDTO
    {
        public string MemberID { get; set; }
        public int Count { get; set; }
    }

    public class RemoveCountDTO
    {
        public string MemberID { get; set; }
        public int Count { get; set; }
    }

    public class UsageLogsDTO
    {
        public string MemberID { get; set; }
    }

    public class DeleteSignerDTO
    {
        public string ID { get; set; }
        public string ImzaciFileLogID { get; set; }
    }

    public class SaveSignerDTO
    {
        public string ID { get; set; }
        public string ImzaciFileLogID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class GetTokenDTO
    {
        public string sid { get; set; }
    }

}
