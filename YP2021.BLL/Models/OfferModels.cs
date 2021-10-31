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
    public class OfferModels
    {
    }

    public class OfferDTListDTO
    {
        public int? TeklifTipiID { get; set; }
        public int? FirmID { get; set; }
    }

    public class OfferDetailDTO
    {
        public int ID { get; set; }
    }

    
    public class OfferDeleteDTO
    {
        public int TID { get; set; }
    }

    public class OfferSaveDTO
    {
        public int? TID { get; set; }
        public int? TTIPI { get; set; }
        public int? MusteriID { get; set; }
        public int? YetkiliKisiID { get; set; }
        public string TITLE { get; set; }
        public string ICERIK { get; set; }
        public int? TDURUMU { get; set; }
        public List<OfferItemDTO> TeklifItems { get; set; }
        public List<OfferDeletedItemsDTO> DeletedItems { get; set; }
    }

    public class OfferDeletedItemsDTO
    {
        public int? TDID { get; set; }
    }

    public class OfferItemDTO
    {
        public int TDID { get; set; }
        public int TID { get; set; }
        public int? ProductID { get; set; }
        public string CustomerCode { get; set; }
        public string BuCode { get; set; }
        public string Oem { get; set; }
        public string Oem1 { get; set; }
        public string Name { get; set; }
        public string Detay { get; set; }
        public int? Quantity  { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? CURRENCY { get; set; }
    }


}
