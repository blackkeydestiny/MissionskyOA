//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MissionskyOA.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class AuditMessage
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
    }
}