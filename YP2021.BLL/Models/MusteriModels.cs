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
    public class MusteriModels
    {
    }

    public class CarilerPageDTListDTO
    {
        public int? FirmaTipiID { get; set; }
        public int? UlkeID { get; set; }
    }

    public class MusteriIDDTO
    {
        public int? ID { get; set; }
    }

    public class MusteriContactSaveDTO
    {
        public int? ID { get; set; }
        public int FIRMA_ID { get; set; }
        public int? ContactTitleID { get; set; }
        public string ADI { get; set; }
        public string SOYADI { get; set; }
        public string MAIL_ADRESI { get; set; }
        public string GSM { get; set; }
        public string TEL { get; set; }
        public DateTime? TARIH { get; set; }
        public bool? IsB2bUser { get; set; }
        public string B2bUsername { get; set; }
        public string B2bPassword { get; set; }
    }

    public class MusteriDeleteContactDTO
    {
        public int? ID { get; set; }
    }

    public class MusteriDocumentSaveDTO
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int? MusteriID { get; set; }
        public int? RankNumber { get; set; }
        public string MusteriDocumentData { get; set; }
        public string DocumentType { get; set; }
    }
    public class MusteriDocumentDeleteDTO
    {
        public int? ID { get; set; }
    }


    public class MusteriAddressSaveDTO
    {
        public int? ID { get; set; }
        public string ADRES { get; set; }
        public int FIRMA_ID { get; set; }
        public int? TIP { get; set; }
        public int? ULKE { get; set; }
        public int? SEHIR { get; set; }
        public string POSTA_KODU { get; set; }
        public DateTime? TARIH { get; set; }
    }

    public class MusteriDeleteAddressDTO
    {
        public int? ID { get; set; }
    }

    public class MusteriSaveDTO
    {
        public int? ID { get; set; }
        public int MyProperty { get; set; }
        public int? FIRMA_TIPI { get; set; }
        public string FIRMA_ADI { get; set; }
        //public string FATURA_ADRESI { get; set; }
        //public string TESLIMAT_ADRESI { get; set; }
        public string ADRES { get; set; }
        public string POSTA_KODU { get; set; }
        public string VERGI_DAIRESI { get; set; }
        public string VERGI_NUMARASI { get; set; }
        public string MARKA { get; set; }
        public string MAIL_ADRESI { get; set; }
        public string WEB_STESI { get; set; }
        public string GSM { get; set; }
        public string TEL_1 { get; set; }
        public string TEL_2 { get; set; }
        public string FAX { get; set; }
        public string NOT { get; set; }
        public int? ULKE { get; set; }
        public int? SEHIR { get; set; }
        public int? CariVadeAltRakamID { get; set; }
        public int? CariVadeID { get; set; }
        public int? CariOdemeSekliID { get; set; }
        public int? CariTeslimatSekliID { get; set; }
        public int? CariNakliyeOdemesiID { get; set; }
        public string FIRMNICK { get; set; }
        public bool? IsB2BDealer { get; set; }
        public int? B2BDealerTypeID { get; set; }
    }

}
