using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class MeetingSearchModel
    {
        [Description("会议室ID")]
        public virtual int MeetingRoomId { get; set; }

        [Description("用户ID")]
        public virtual int UserId { get; set; }

        [Description("开始日期时间")]
        public DateTime StartDate { get; set; }

        [Description("结束日期时间")]
        public DateTime EndDate { get; set; }
    }
}
