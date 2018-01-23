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
    /// 更新用户年度考勤(加班请假)汇总
    /// </summary>
    public class UpdateAttendanceSummaryModel 
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [Description("用户Id")]
        public int UserId { get; set; }

        /// <summary>
        /// 年份
        /// </summary>
        [Description("年份")]
        public int SummaryYear { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public OrderType Type { get; set; }

        /// <summary>
        /// 申请时长
        /// </summary>
        [Description("申请时长")]
        public double Times { get; set; }
    }
}
