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
    
    public partial class MeetingRoom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MeetingRoom()
        {
            this.MeetingCalendars = new HashSet<MeetingCalendar>();
        }
    
        public int Id { get; set; }
        public string MeetingRoomName { get; set; }
        public int Capacity { get; set; }
        public string Equipment { get; set; }
        public string Remark { get; set; }
        public Nullable<int> Status { get; set; }
        public string CreateUserName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MeetingCalendar> MeetingCalendars { get; set; }
    }
}
