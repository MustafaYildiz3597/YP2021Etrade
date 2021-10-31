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
    public class TProductsModels
    {
   
    }

    public class TPListDTDTO
    {
        public int? TedarikciID { get; set; }
        public string BUKOD { get; set; }
        public string ProductSearchText { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string searchValue { get; set; }
        public TPDTListSearchItems searchitems { get; set; }
    }
    public class TPDTListSearchItems
    {
        public string BUKod { get; set; }
        public string FIRMA_ADI { get; set; }
        public string XPSNO { get; set; }
        public string NAME { get; set; }
        public string CreatedOn { get; set; }
        public string UPDATED { get; set; }
        public string PRICE { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class TPImportExcelUploadDataDTO
    {
        public List<TPImportExcelUploadingDataDTO> Data { get; set; }
        public int? MusteriID { get; set; }
    }

    public class TPImportExcelUploadingDataDTO
    {
        public string BUKOD { get; set; }
        public string NAME { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public decimal? PRICE { get; set; }
        public string CURRENCY { get; set; }
        public string XPSNO { get; set; }
        public string OEM { get; set; }
    }

    public class SaveTUGridEditDTO
    {
        public List<SaveTUGridEditItemDTO> Items { get; set; }
    }

    public class SaveTUGridEditItemDTO
    {
        public int TPID { get; set; }
        public string NAME { get; set; }
        public string XPSNO { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
    }

    public class SaveTUGridAddDTO
    {
        public List<SaveTUGridAddProductItemDTO> Products { get; set; }
        public List<SaveTUGridAddTedarikciItemDTO> Tedarikciler { get; set; }
    }

    public class SaveTUGridAddProductItemDTO
    {
        public int TPID { get; set; }
        public string BUKOD { get; set; }
        public int? ProductID { get; set; }
        public string NAME { get; set; }
        public string XPSNO { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
    }

    public class SaveTUGridAddTedarikciItemDTO
    {
        public int? ID { get; set; }
    }

    public class TProductsDetailDTO
    {
        public int? ID { get; set; }
    }

    public class SaveTProductDTO
    {
        public int? TPID { get; set; }
        public string BUKOD { get; set; }
        public int? ProductID { get; set; }
        public string NAME { get; set; }
        public string NAME_DE { get; set; }
        public string NAME_EN { get; set; }
        public string OEM { get; set; }

        public string EDITOR_TABLE { get; set; }
        public int MusteriID { get; set; }

        public string XPSNO { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
    }

    public class TProductDeleteDTO
    {
        public int? ID { get; set; }
    }


}
