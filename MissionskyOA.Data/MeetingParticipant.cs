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
    
    public partial class MeetingParticipant
    {
        public int Id { get; set; }
        public int MeetingCalendarId { get; set; }
        public int UserId { get; set; }
        public string IsOptional { get; set; }
    
        public virtual MeetingCalendar MeetingCalendar { get; set; }
        public virtual User User { get; set; }
    }
}