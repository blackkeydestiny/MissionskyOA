using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 用户模型
    /// </summary>
    public class OrderDetModel : BaseModel
    {
        [Description("假单Id")]
        public int OrderId { get; set; }

        [Description("开始日期")]
        public DateTime StartDate { get; set; }

        [Description("开始时间")]
        public TimeSpan? StartTime { get; set; }

        [Description("结束日期")]
        public DateTime EndDate { get; set; }

        [Description("结束时间")]
        public TimeSpan? EndTime { get; set; }

        [Description("增加或减少假期小时数,负数为请假,正数为增加假期")]
        public double? IOHours { get; set; }

        [Description("引用Id,用于销假对单")]
        public int? RefOrderId { get; set; }

        [Description("申请理由")]
        public string Description { get; set; }
        
        [Description("假期变化结果")]
        public string Audit { get; set; }

        [Description("是否告知领导")]
        public bool InformLeader { get; set; }

        [Description("是否工作交接")]
        public bool WorkTransfer { get; set; }

        [Description("移交人")]
        public int Recipient { get; set; }

        [Description("移交人name")]
        public string RecipientName { get; set; }

    }
}
