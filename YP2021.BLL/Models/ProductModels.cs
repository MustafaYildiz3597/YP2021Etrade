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
    public class ProductModels
    {
    }

    public class ProductDTListDTO
    {
        public int? SecID { get; set; }
        public int? MCatID { get; set; }
        public int? CatID { get; set; }
        public string OEMNo { get; set; }
        public string BUKOD { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDir { get; set; }
        public string searchValue { get; set; }
        public  ProductDTListSearchItems searchitems { get; set; }
    }

    public class ProductDTListSearchItems
    {
        public int? HasImage { get; set; }
        public string BUKod { get; set; }
        public string SectionName { get; set; }
        public string MCatName { get; set; }
        public string CatName { get; set; }
        public string ProductName { get; set; }
        public string Added { get; set; }
        public string Enabled { get; set; }
    }

    public class ProductDetailDTO
    {
        public int ID { get; set; }
    }

    
    public class ProductDeleteDTO
    {
        public int ID { get; set; }
    }

    public class ProductSaveDTO
    {
        public int? PRODID { get; set; }
        public string BUKOD { get; set; }
        public string CATGROUP { get; set; }
        public int? CATID { get; set; }
        public string DESCC { get; set; }
        public string DESI { get; set; }
        public string EDITOR_TABLE { get; set; }
        public string EDITOR_TABLE_EN { get; set; }
        public string EDITOR_TABLE_FR { get; set; }
        public string EDITOR_TABLE_GR { get; set; }
        public bool? ENABLED { get; set; }
        public string MARKA { get; set; }
        public int? MCATID { get; set; }
        public string NAME { get; set; }
        public string NAME_DE { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_FR { get; set; }
        public string RESIM { get; set; }
        public string RESIMDATA { get; set; }
        public bool? DeletedResim { get; set; }
        public int? SECID { get; set; }
        public int? SIRALAMA { get; set; }
        public string TRESIM { get; set; }
        public string TRESIMDATA { get; set; }
        public bool? DeletedTResim { get; set; }
        public decimal? B2BBasePrice { get; set; }
        public int? B2BCurrencyID { get; set; }
        public decimal? B2BDiscountedPrice { get; set; }
        public bool? B2BIsNewProduct { get; set; }
        public bool? B2BIsOnSale { get; set; }
        public bool? B2BIsVisible { get; set; }
        public bool? B2BIsVisibleOnCategoryHomepage { get; set; }
        public bool? B2BIsVisibleOnHomepage { get; set; }
        public decimal? B2BStockAmount { get; set; }
        public decimal? CountInPalette { get; set; }
        public string EDITOR_B2B { get; set; }
        public string EDITOR_ManagerNote { get; set; }
        public bool? IsEuroPalette { get; set; }
        public bool? OpenForSale { get; set; }
        public string PackageDimensions { get; set; }
        public decimal? PackagePieceCount { get; set; }
        public bool? IsBundled { get; set; }
        public List<RelatedProductDTO> RelatedProducts { get; set; }
        public List<BundledProductDTO> BundledProducts { get; set; }
    }

    public class RelatedProductDTO
    {
        public int? ID { get; set; }
        public int? ProductID { get; set; }
        public int? RelatedID { get; set; }
        public int? RankNumber { get; set; }
        public int? Quantity { get; set; }
    }
    public class BundledProductDTO
    {
        public int? ID { get; set; }
        public int? ProductID { get; set; }
        public int? RelatedID { get; set; }
        public int? RankNumber { get; set; }
        public int? Quantity { get; set; }
    }

    public class ProductSaveImageDTO
    {
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public int? ProductID { get; set; }
        public int? ItemType { get; set; }
        public int? RankNumber { get; set; }
        public string ProductImageData { get; set; }
        public string FileName { get; set; }
    }

    public class ProductDeleteImageDTO
    {
        public int ID { get; set; }
    }

    public class ProductOEMListDTO
    {
        public int? PRODID { get; set; }
    }

    public class ProductOEMSaveDTO
    {
        public int? OEMID { get; set; }
        public string OEMNR { get; set; }
        public int? SUPID { get; set; }
        public int? PRODID { get; set; }
        public string BUKOD { get; set; }
    }

    public class ProductOEMDeleteDTO
    {
        public int? OEMID { get; set; }
    }


    public class ProductMUListDTO
    {
        public int? PRODID { get; set; }
    }

    public class ProductMUSaveDTO
    {
        public int MPID { get; set; }
        public int? MusteriID { get; set; }
        public int? ProductID { get; set; }
        public string BUKOD { get; set; }
        public string XPSUP { get; set; }
        public string NAME { get; set; }
        public string EDITOR_TABLE { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
        public string XPSNO { get; set; }
        public string BUESKI { get; set; }
    }

    public class ProductMUDeleteDTO
    {
        public int MPID { get; set; }
    }

    public class ProductTUListDTO
    {
        public int? PRODID { get; set; }
    }

    public class ProductTUSaveDTO
    {
        public int TPID { get; set; }
        public int? MusteriID { get; set; }
        public int? ProductID { get; set; }
        public string BUKOD { get; set; }
        public string XPSUP { get; set; }
        public string NAME { get; set; }
        public string EDITOR_TABLE { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public decimal? PRICE { get; set; }
        public int? CURRENCY { get; set; }
        public string OEM { get; set; }
        public string XPSNO { get; set; }
        public string BRAND { get; set; }
        public string CATNAME { get; set; }
    }

    public class ProductTUDeleteDTO
    {
        public int TPID { get; set; }
    }

    public class ImportExcelUploadDataDTO
    {
        public List<ImportExcelUploadingDataDTO> Data { get; set; }
        public ImportExcelUploadingCatsDTO Cats { get; set; }
    }

    public class ImportExcelUploadingDataDTO
    {
        public string BUKOD { get; set; }
        public string NAME { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public string NAME_FR { get; set; }
        public string EDITOR_TABLE { get; set; }
        public string EDITOR_TABLE_EN { get; set; }
        public string EDITOR_TABLE_GR { get; set; }
        public string EDITOR_TABLE_FR { get; set; }
        public string ENABLED { get; set; }
    }

    public class ImportExcelUploadingCatsDTO
    {
        public int SECID { get; set; }
        public int MCATID { get; set; }
        public int CATID { get; set; }
    }


}
