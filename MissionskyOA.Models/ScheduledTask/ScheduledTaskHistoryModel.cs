using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 定时任务执行记录
    /// </summary>
    public class ScheduledTaskHistoryModel : BaseModel
    {
        /// <summary>
        /// 定时任务Id
        /// </summary>
        [Description("定时任务Id")]
        public int TaskId { get; set; }

        /// <summary>
        /// 执行结果
        /// </summary>
        [Description("执行结果")]
        public bool Result { get; set; }

        /// <summary>
        /// 执行结果备注
        /// </summary>
        [Description("执行结果备注")]
        public string Desc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }
    }
}
