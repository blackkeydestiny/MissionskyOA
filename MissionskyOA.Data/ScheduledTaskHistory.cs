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
    
    public partial class ScheduledTaskHistory
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public bool Result { get; set; }
        public string Desc { get; set; }
        public System.DateTime CreatedTime { get; set; }
    
        public virtual ScheduledTask ScheduledTask { get; set; }
    }
}