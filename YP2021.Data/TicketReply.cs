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
    
    public partial class TicketReply
    {
        public int ID { get; set; }
        public Nullable<int> TicketID { get; set; }
        public string MemberID { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
    
        public virtual Ticket Ticket { get; set; }
        public virtual Member Member { get; set; }
    }
}