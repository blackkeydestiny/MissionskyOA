using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class ReserveMeetingModel:BaseModel
    {
        [Description("会议室ID")]
        public int MeetingRoomId { get; set; }
        [Description("会议主题")]
        public string Title { get; set; }
        [Description("开始日期和时间,批量新增会议时请初始化任一开始日期")]
        public DateTime StartDate { get; set; }

        [Description("结束日期和时间,批量新增会议时请初始化任一结束日期")]
        public DateTime EndDate { get; set; }

        [Description("主持人")]
        public string Host { get; set; }
        [Description("会议内容")]
        public virtual string MeetingContext { get; set; }

        [Description("会议纪要")]
        public virtual string MeetingSummary { get; set; }

        [Description("会议循环星期顺序")]
        public virtual DayOfWeek[] MeetingDayInWeek { get; set; }

        [Description("必须参会人员")]
        public int[] participants { get; set; }
        [Description("可选参会人员")]
        public int[] optionalParticipants { get; set; }
    }
}
