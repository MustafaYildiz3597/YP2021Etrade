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
    
    public partial class UserLevelPermission
    {
        public System.Guid ID { get; set; }
        public Nullable<int> UserLevelID { get; set; }
        public Nullable<int> ActionID { get; set; }
        public Nullable<bool> ExecutePermission { get; set; }
        public Nullable<bool> ViewPermission { get; set; }
        public Nullable<bool> AddPermission { get; set; }
        public Nullable<bool> UpdatePermission { get; set; }
        public Nullable<bool> DeletePermission { get; set; }
        public Nullable<bool> SearchPermission { get; set; }
    
        public virtual Action Action { get; set; }
        public virtual UserLevels UserLevels { get; set; }
    }
}