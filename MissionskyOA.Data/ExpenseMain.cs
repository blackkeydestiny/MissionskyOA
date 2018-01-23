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
    
    public partial class ExpenseMain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpenseMain()
        {
            this.ExpenseAuditHistories = new HashSet<ExpenseAuditHistory>();
            this.ExpenseDetails = new HashSet<ExpenseDetail>();
        }
    
        public int Id { get; set; }
        public int AuditId { get; set; }
        public int DeptNo { get; set; }
        public int ProjNo { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public Nullable<int> ApplyUserId { get; set; }
        public Nullable<int> PrintForm { get; set; }
        public Nullable<bool> ConfirmForm { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpenseAuditHistory> ExpenseAuditHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpenseDetail> ExpenseDetails { get; set; }
        public virtual Project Project { get; set; }
    }
}