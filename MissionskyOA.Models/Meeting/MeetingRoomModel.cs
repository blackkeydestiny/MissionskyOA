using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    public class MeetingRoomModel:BaseModel
    {
        [Description("会议室名")]
        public string MeetingRoomName { get; set; }
        [Description("可容纳人数")]
        public int Capacity { get; set; }
        [Description("会议室设备")]
        public string Equipment { get; set; }
        [Description("备注")]
        public string Remark { get; set; }
        [Description("会议室数据状态")]
        public int? Status { get; set; }
        [Description("创建者用户名")]
        public string CreateUserName { get; set; }
        [Description("创建日期")]
        public DateTime? CreateDate { get; set; }
    }
}
