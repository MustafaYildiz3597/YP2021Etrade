//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nero2021.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TEKLIFLER_DETAY
    {
        public int TDID { get; set; }
        public Nullable<int> TID { get; set; }
        public Nullable<int> PRODID { get; set; }
        public string ProductID { get; set; }
        public string CustomerCode { get; set; }
        public string BuCode { get; set; }
        public string Oem { get; set; }
        public string Oem1 { get; set; }
        public string Name { get; set; }
        public string Detay { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> CURRENCY { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    
        public virtual Currency Currency1 { get; set; }
        public virtual PRODUCTS PRODUCTS { get; set; }
        public virtual TEKLIFLER TEKLIFLER { get; set; }
    }
}
