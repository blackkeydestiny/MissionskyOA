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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDets = new HashSet<OrderDet>();
        }
    
        public int Id { get; set; }
        public Nullable<int> Status { get; set; }
        public int OrderType { get; set; }
        public int UserId { get; set; }
        public Nullable<int> ApplyUserId { get; set; }
        public Nullable<int> RefOrderId { get; set; }
        public Nullable<int> WorkflowId { get; set; }
        public Nullable<int> NextStep { get; set; }
        public Nullable<int> NextAudit { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public int OrderNo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDet> OrderDets { get; set; }
        public virtual User User { get; set; }
    }
}