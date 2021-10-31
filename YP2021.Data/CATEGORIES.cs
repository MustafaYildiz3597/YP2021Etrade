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
    
    public partial class CATEGORIES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CATEGORIES()
        {
            this.PRODUCTS = new HashSet<PRODUCTS>();
        }
    
        public int CATID { get; set; }
        public int SECID { get; set; }
        public Nullable<int> MCATID { get; set; }
        public string NAME { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public string NAME_FR { get; set; }
        public Nullable<System.DateTime> ADDED { get; set; }
        public Nullable<System.DateTime> UPDATED { get; set; }
        public Nullable<bool> ENABLED { get; set; }
        public Nullable<int> SORT { get; set; }
        public string BARKOD { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUCTS> PRODUCTS { get; set; }
        public virtual MAINCATEGORIES MAINCATEGORIES { get; set; }
        public virtual SECTIONS SECTIONS { get; set; }
    }
}
