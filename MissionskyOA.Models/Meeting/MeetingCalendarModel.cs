using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class MeetingCalendarModel:BaseModel
    {
        [Description("会议室ID")]
        public int MeetingRoomId { get; set; }
        [Description("会议主题")]
        public string Title { get; set; }
        [Description("开始日期")]
        public DateTime StartDate { get; set; }

        [Description("开始时间")]
        public TimeSpan? StartTime { get; set; }

        [Description("结束日期")]
        public DateTime EndDate { get; set; }

        [Description("结束时间")]
        public TimeSpan? EndTime { get; set; }
        [Description("主持人")]
        public string Host { get; set; }
        [Description("会议内容")]
        public string MeetingContext { get; set; }

        [Description("会议预定状态")]
        public int Status { get; set; }
        [Description("会议申请人")]
        public int ApplyUserId { get; set; }

        [Description("会议申请人英文名")]
        public string ApplyUserName { get; set; }

        [Description("会议申请人英文名")]
        public string MeetingSummary { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
        public IList<MeetingParticipantModel> MeetingParticipants { get; set; }
        public virtual MeetingRoomModel MeetingRoom { get; set; }
    }
}
