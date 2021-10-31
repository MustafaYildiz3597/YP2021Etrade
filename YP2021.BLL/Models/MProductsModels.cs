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
    public class MProductsModels
    {
    }

    public class MPListDTDTO
    {
        public int? MusteriID { get; set; }
        public string BUKOD { get; set; }
        public string ProductSearchText { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string searchValue { get; set; }
        public MPDTListSearchItems searchitems { get; set; }
    }

    public class MPDTListSearchItems
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

    public class MPImportExcelUploadDataDTO
    {
        public List<MPImportExcelUploadingDataDTO> Data { get; set; }
        public int? MusteriID { get; set; }
    }

    public class MPBrowseList
    {
        public int? MusteriID { get; set; }
    }

    public class MPImportExcelUploadingDataDTO
    {
        public string BUKOD { get; set; }
        public string NAME { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public decimal? PRICE { get; set; }
        public string CURRENCY { get; set; } // excelden currency code geliyor.
        public string XPSNO { get; set; }
        public string OEM { get; set; }
    }

    public class SaveMUGridEditDTO
    {
        public List<SaveMUGridEditItemDTO> Items { get; set; }
    }

    public class SaveMUGridEditItemDTO
    {
        public int MPID { get; set; }
        public string NAME { get; set; }
        public string XPSNO { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
    }

    public class SaveMUGridAddDTO
    {
        public List<SaveMUGridAddProductItemDTO> Products { get; set; }
        public List<SaveMUGridAddMusteriItemDTO> Musteriler { get; set; }
    }

    public class SaveMUGridAddProductItemDTO
    {
        public int MPID { get; set; }
        public string BUKOD { get; set; }
        public int? ProductID { get; set; }
        public string NAME { get; set; }
        public string XPSNO { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
    }

    public class SaveMUGridAddMusteriItemDTO
    {
        public int? ID { get; set; }
    }

    public class MProductsDetailDTO
    {
        public int? ID { get; set; }
    }


    public class SaveMProductDTO
    {
        public int? MPID { get; set; }
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

    public class MProductDeleteDTO
    {
        public int? ID { get; set; }
    }
}
