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
    
    public partial class Workflow
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workflow()
        {
            this.WorkflowSteps = new HashSet<WorkflowStep>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int Type { get; set; }
        public bool Status { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string Condition { get; set; }
        public string ConditionDesc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; }
    }
}
