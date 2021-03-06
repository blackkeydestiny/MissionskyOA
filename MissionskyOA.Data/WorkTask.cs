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
    
    public partial class WorkTask
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkTask()
        {
            this.WorkTaskComments = new HashSet<WorkTaskComment>();
            this.WorkTaskHistories = new HashSet<WorkTaskHistory>();
        }
    
        public int Id { get; set; }
        public string Outline { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Desc { get; set; }
        public Nullable<int> MeetingId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public int Sponsor { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public Nullable<int> Executor { get; set; }
        public int Source { get; set; }
        public int Priority { get; set; }
        public int Workload { get; set; }
        public Nullable<int> Urgency { get; set; }
        public Nullable<int> Importance { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<System.DateTime> CompleteTime { get; set; }
        public Nullable<System.DateTime> CloseTime { get; set; }
        public System.DateTime CreatedTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkTaskComment> WorkTaskComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkTaskHistory> WorkTaskHistories { get; set; }
    }
}
