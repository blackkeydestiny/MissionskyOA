using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 申请单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 无效类型
        /// </summary>
        [Description("无效类型")]
        None = 0,

        /// <summary>
        /// 年假
        /// </summary>
        [Description("年假")]
        AnnualLeave = 1,

        /// <summary>
        /// 病假
        /// </summary>
        [Description("病假")]
        SickLeave = 2,

        /// <summary>
        /// 调休
        /// </summary>
        [Description("调休")]
        DaysOff = 3,
        
        /// <summary>
        /// 加班
        /// </summary>
        [Description("加班")]
        Overtime = 4,

        /// <summary>
        /// 婚假
        /// </summary>
        [Description("婚假")]
        MarriageLeave = 5,

        /// <summary>
        /// 产假
        /// </summary>
        [Description("产假")]
        MaternityLeave = 6,

        /// <summary>
        /// 丧假
        /// </summary>
        [Description("丧假")]
        FuneralLeave = 7,

        /// <summary>
        /// 事假
        /// </summary>
        [Description("事假")]
        AskLeave = 8,
        
        /// <summary>
        /// 陪产假
        /// </summary>
        [Description("陪产假")]
        PaternityLeave = 9,

        /// <summary>
        /// 哺乳假
        /// </summary>
        [Description("哺乳假")]
        BreastfeedingLeave = 10,

        /// <summary>
        /// 其他(出差/外访等)
        /// </summary>
        [Description("其他(出差/外访等)")]
        Other = 11
    }
}
