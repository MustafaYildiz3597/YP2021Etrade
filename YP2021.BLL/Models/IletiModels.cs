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
    public enum ItemStatus
    {
        [Description("Başlamadı")]
        Baslamadi = 0,
        [Description("İşlemde")]
        Islemde = 1,
        [Description("Tamamlandı")]
        Tamamlandı = 2,
        [Description("Hatalı İşlem")]
        Hatalıİşlem = 3,
    }

    public enum SendingStatus
    {
        [Description("İşlem Yok")]
        Baslamadi = 0,
        [Description("İşlemde")]
        Islemde = 1,
        [Description("Gönderildi")]
        Gonderildi = 2,
        [Description("Gönderim Başarısız")]
        GonderimBasariz = 3,
    }

    public enum SigningStatus
    {
        [Description("İşlem Yok")]
        Baslamadi = 0,
        [Description("İmzada")]
        Imzada = 1,
        [Description("İmzalandı")]
        Imzalandi = 2,
        [Description("İmza Hatalı")]
        ImzaHatali = 3,
        [Description("İmza Red")]
        ImzaRed = 9,
    }

    public class IletiDetailRQDTO
    {
        public string ID { get; set; }
    }

    public class SendToEmployeesDTO
    {
        public string MessageText { get; set; }
        public string Subject { get; set; }
        public List<SelectedEmployeesDTO> SelectedEmployees { get; set; }
    }

    public class SelectedEmployeesDTO
    {
        public Guid? ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SelectedEmployeeListDTO
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string KEPAddress { get; set; }
        public string GSMNumber { get; set; }
    }

    public class StartIletiSendingsDTO
    {
        public string sid { get; set; }
        public string tokenid { get; set; }
    }

    public class OTPCheckPhoneNumberDTO
    {
        public Guid IletiEmployeeID { get; set; }
        public string GSMNumber { get; set; }
    }

    public class BordroOTPCheckPhoneNumberDTO
    {
        public Guid BordroEmployeeID { get; set; }
        public string GSMNumber { get; set; }
    }

    public class VerifyShowingOTPDTO
    {
        public Guid VerifyID { get; set; }
        public string OTPCode { get; set; }
    }

    public class GosterApiDTO
    {
        public string filename { get; set; }
    }
}
