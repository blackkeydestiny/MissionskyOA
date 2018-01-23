using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 工作任务汇总
    /// </summary>
    public class WorkTaskSummaryModel 
    {
        /// <summary>
        /// 汇总类型
        /// </summary>
        [Description("汇总类型")]
        public string Type { get; set; }

        /// <summary>
        /// 汇总结果
        /// </summary>
        [Description("汇总结果")]
        public int Value { get; set; }
    }
}
