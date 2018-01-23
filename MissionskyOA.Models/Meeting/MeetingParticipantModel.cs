using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class MeetingParticipantModel:BaseModel
    {
        [Description("会议ID")]
        public int MeetingCalendarId { get; set; }
        [Description("用户ID")]
        public int UserId { get; set; }
        [Description("是否必须参与会议:必须(true)不必要(否)")]
        public string IsOptional { get; set; }

        //public virtual MeetingCalendarModel MeetingCalendar { get; set; }
        public virtual UserModel User { get; set; }
    }
}
