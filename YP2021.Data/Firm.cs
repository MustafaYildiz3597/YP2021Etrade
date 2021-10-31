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
    
    public partial class Firm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Firm()
        {
            this.ExcelUploadLog = new HashSet<ExcelUploadLog>();
            this.Member1 = new HashSet<Member>();
        }
    
        public System.Guid ID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactName { get; set; }
        public string ContactGSMNo { get; set; }
        public string ContactEmail { get; set; }
        public string KEPEmail { get; set; }
        public string PhoneNo { get; set; }
        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }
        public Nullable<int> KEPMemberMaxLimit { get; set; }
        public Nullable<int> KEPMemberCount { get; set; }
        public Nullable<System.DateTime> KEPExpirationDT { get; set; }
        public Nullable<bool> KEPStatus { get; set; }
        public string CreateUserID { get; set; }
        public Nullable<System.DateTime> CreateDT { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> DeleteDT { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExcelUploadLog> ExcelUploadLog { get; set; }
        public virtual Member Member { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Member> Member1 { get; set; }
    }
}