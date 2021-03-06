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
    
    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {
            this.TicketMember = new HashSet<TicketMember>();
            this.TicketReply = new HashSet<TicketReply>();
        }
    
        public int ID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<int> MusteriID { get; set; }
        public string Title { get; set; }
        public string DetailText { get; set; }
        public Nullable<int> PriorityID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<System.DateTime> LastRepliedOn { get; set; }
        public string LastRepliedBy { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Member Member1 { get; set; }
        public virtual MUSTERILER MUSTERILER { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketMember> TicketMember { get; set; }
        public virtual Member Member2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketReply> TicketReply { get; set; }
    }
}
