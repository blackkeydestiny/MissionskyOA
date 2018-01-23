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
    
    public partial class Notification
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Notification()
        {
            this.NotificationTargetUsers = new HashSet<NotificationTargetUser>();
        }
    
        public int Id { get; set; }
        public Nullable<int> BusinessType { get; set; }
        public string Title { get; set; }
        public string MessageContent { get; set; }
        public string Target { get; set; }
        public int MessageType { get; set; }
        public int Scope { get; set; }
        public string MessagePrams { get; set; }
        public Nullable<int> CreatedUserId { get; set; }
        public System.DateTime CreatedTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationTargetUser> NotificationTargetUsers { get; set; }
    }
}