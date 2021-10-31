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
    public class B2BModels
    {
    }

    public class SaveDealerTypeListDTO
    {
        //public SaveDealerTypeListItem List { get; set; }
        public int? ID { get; set; }
        public string Name { get; set; }
        public decimal? SalesRate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class SaveDealerTypeListItem
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public decimal? SalesRate { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class B2BStockDetailDTO
    {
        public int? ID { get; set; }
    }

    public class B2BStoklarSaveDTO
    {
        public int? PRODID { get; set; }
        public string BUKOD { get; set; }
        public decimal? B2BBasePrice { get; set; }
        public int? B2BCurrencyID { get; set; }
        public decimal? B2BDiscountedPrice { get; set; }
        public bool? B2BIsNewProduct { get; set; }
        public bool? B2BIsOnSale { get; set; }
        public bool? B2BIsVisible { get; set; }
        public bool? B2BIsVisibleOnCategoryHomepage { get; set; }
        public bool? B2BIsVisibleOnHomepage { get; set; }
        public decimal? B2BStockAmount { get; set; }
    }

    public class B2BStoklarDeleteDTO
    {
        public int? ID { get; set; }
    }


}
