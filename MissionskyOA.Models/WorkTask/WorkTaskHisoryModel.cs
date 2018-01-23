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
    /// 工作任务操作记录
    /// </summary>
    public class WorkTaskHistoryModel : BaseModel
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        [Description("任务Id")]
        public int TaskId { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [Description("操作人")]
        public int Operator { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        [Description("操作人姓名")]
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [Description("操作状态")]
        public WorkTaskStatus Status { get; set; }

        /// <summary>
        /// 审计信息
        /// </summary>
        [Description("审计信息")]
        public string Audit { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }
    }
}
