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
    
    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            this.ODetay = new HashSet<ODetay>();
        }
    
        public int OrderID { get; set; }
        public Nullable<int> MusteriID { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string OrderNo { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<System.DateTime> Updates { get; set; }
        public string UpdatesUser { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<int> InvoiceAddressID { get; set; }
        public string InvoiceAddress { get; set; }
        public Nullable<int> DeliveryAddressID { get; set; }
        public string DeliveryAddress { get; set; }
        public Nullable<bool> IsB2b { get; set; }
        public Nullable<double> TotalAmount { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Member Member1 { get; set; }
        public virtual MUSTERILER MUSTERILER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ODetay> ODetay { get; set; }
        public virtual YETKILI_KISILER YETKILI_KISILER { get; set; }
    }
}
