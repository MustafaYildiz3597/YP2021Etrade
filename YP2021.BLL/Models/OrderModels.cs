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
    public class OrderModels
    {
    }

    public class SiparislerPageDTListDTO
    {
        public string OrderNo { get; set; }
        public int? CustomerID { get; set; }
        public DateTime? CriBeginDate   { get; set; }
        public DateTime? CriEndDate { get; set; }
    }

    public class OrderDetailDTO
    {
        public int ID { get; set; }
    }

    public class OrderDeleteDTO
    {
        public int ID { get; set; }
    }

    public class OrderSaveDTO
    {
        public int? OrderID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? OrderDateF { get; set; }
        public string OrderNo { get; set; }
        public int? MusteriID { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
        public List<OrderDeletedItemsDTO> DeletedItems { get; set; }
    }

    public class OrderDeletedItemsDTO
    {
        public int? ORDID { get; set; }
    }

    public class OrderItemDTO
    {
        public int ORDID { get; set; }
        public int OrderID { get; set; }
        public int? ProductID { get; set; }
        public string CustomerCode { get; set; }
        public string BuCode { get; set; }
       
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }

        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? CurrencyID { get; set; }
    }

    public class OrderImportExcelUploadDataDTO
    {
        public string BatchID { get; set; }
        public int? MusteriID { get; set; }
        public List<OrderImportExcelUploadItem> Data { get; set; }
    }
    public class OrderImportExcelUploadItem
    {
        public DateTime? OrderDate { get; set; }
        public string OrderNo { get; set; }
        //public int CustomerID { get; set; }
        //public string CustomerTitle { get; set; }
        public string BUKOD { get; set; }
        public string ProductName { get; set; }
        public string PCustomerCode { get; set; }
        public int? Quantity { get; set; }
        public decimal? SalesPrice { get; set; }
    }

}
