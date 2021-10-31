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
    
    public partial class MusteriDocument
    {
        public int ID { get; set; }
        public Nullable<int> MusteriID { get; set; }
        public string DocumentType { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> ItemIndex { get; set; }
        public string FilePath { get; set; }
        public string ThumbnailPath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> RankNumber { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public string FileName { get; set; }
    
        public virtual Member Member { get; set; }
        public virtual Member Member1 { get; set; }
        public virtual MUSTERILER MUSTERILER { get; set; }
    }
}
