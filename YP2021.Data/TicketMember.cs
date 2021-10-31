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
    
    public partial class TicketMember
    {
        public int ID { get; set; }
        public Nullable<int> TicketID { get; set; }
        public string MemberID { get; set; }
        public Nullable<bool> IsEnrollment { get; set; }
        public Nullable<bool> IsAssignedTo { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<int> Direction { get; set; }
        public Nullable<bool> IsRead { get; set; }
        public Nullable<System.DateTime> LastReadDate { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<int> UnreadCount { get; set; }
        public Nullable<System.DateTime> LastArchivedOn { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Member Member1 { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
