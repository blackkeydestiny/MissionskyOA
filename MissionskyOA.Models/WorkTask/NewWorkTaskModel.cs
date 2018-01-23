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
    /// 编辑新的工作任务
    /// </summary>
    public class NewWorkTaskModel
    {
        /// <summary>
        /// 任务要求内容
        /// </summary>
        [Description("任务要求内容")]
        public string Outline { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        [Description("任务类型")]
        public WorkTaskType Type { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        [Description("任务描述")]
        public string Desc { get; set; }

        /// <summary>
        /// 会议Id
        /// </summary>
        [Description("会议Id")]
        public int? MeetingId { get; set; }

        /// <summary>
        /// 项目组Id
        /// </summary>
        [Description("项目组Id")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        [Description("发起人")]
        public int Sponsor { get; set; }
        
        /// <summary>
        /// 监督人(默认为发起人)
        /// </summary>
        [Description("监督人(默认为发起人)")]
        public int? Supervisor { get; set; }

        /// <summary>
        /// 执行人(默认为发起人
        /// </summary>
        [Description("执行人(默认为发起人)")]
        public int? Executor { get; set; }

        /// <summary>
        /// 来源：会议或通讯录，为空则默认通讯录个人
        /// </summary>
        [Description("来源：会议或通讯录，为空则默认通讯录个人")]
        public WorkTaskSource? Source { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Description("优先级，为空则默认一般")]
        public WorkTaskPriority? Priority { get; set; }

        /// <summary>
        /// 紧急性: 高中低
        /// </summary>
        [Description("紧急性: 高中低")]
        public WorkTaskUrgency? Urgency { get; set; }

        /// <summary>
        /// 重要性: 高中低
        /// </summary>
        [Description("重要性: 高中低")]
        public WorkTaskImportance? Importance { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        [Description("截止时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        public DateTime? StartTime { get; set; }
    }
}
