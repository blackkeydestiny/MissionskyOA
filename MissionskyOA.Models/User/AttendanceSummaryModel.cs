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
    /// 用户年度考勤(加班请假)汇总
    /// </summary>
    public class AttendanceSummaryModel : BaseModel
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
        public int Year { get; set; }

        /// <summary>
        /// 考勤(申请单)类型
        /// </summary>
        [Description("考勤(申请单)类型")]
        public OrderType Type { get; set; }

        /// <summary>
        /// 考勤(申请单)类型名称
        /// </summary>
        [Description("考勤(申请单)类型名称")]
        public string TypeName { get; set; }

        /// <summary>
        /// 上一年度剩余
        /// </summary>
        [Description("上一年度剩余")]
        public double? LastValue { get; set; }

        /// <summary>
        /// 本年度可使用
        /// </summary>
        [Description("本年度基准额度")]
        public double? BaseValue { get; set; }

        /// <summary>
        /// 本年度剩余
        /// </summary>
        [Description("本年度剩余")]
        public double? RemainValue { get; set; }

        /// <summary>
        /// 本年度已使用假期
        /// </summary>
        [Description("本年度已使用假期")]
        public double? UsedValue { get; set; }
    }
}
