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
    
    public partial class OEM
    {
        public int OEMID { get; set; }
        public Nullable<int> PRODID { get; set; }
        public string BUKOD { get; set; }
        public string OEMNR { get; set; }
        public Nullable<int> SUPID { get; set; }
        public string SUPNAME { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public bool LEFTT { get; set; }
        public bool RIGHTT { get; set; }
        public string OEMNR2 { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual OEMSUPNAME OEMSUPNAME { get; set; }
        public virtual PRODUCTS PRODUCTS { get; set; }
    }
}
