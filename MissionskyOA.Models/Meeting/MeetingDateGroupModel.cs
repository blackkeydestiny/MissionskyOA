using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
namespace MissionskyOA.Models
{
    public class MeetingDateGroupModel
    {
        [Description("日期")]
        public DateTime startDate { get; set; }
        [Description("会议")]
        public IList<MeetingCalendarModel> meetings { get; set; }
    }
}
