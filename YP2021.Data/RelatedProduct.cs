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
    
    public partial class RelatedProduct
    {
        public int ID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> RelatedID { get; set; }
        public Nullable<int> RankNumber { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual PRODUCTS PRODUCTS { get; set; }
        public virtual PRODUCTS PRODUCTS1 { get; set; }
    }
}
