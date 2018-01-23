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
    /// 工作任务
    /// </summary>
    public class WorkTaskModel : BaseModel
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
        /// 任务状态
        /// </summary>
        [Description("任务状态")]
        public WorkTaskStatus Status { get; set; }

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
        /// 会议
        /// </summary>
        [Description("会议")]
        public MeetingCalendarModel Meeting { get; set; }

        /// <summary>
        /// 项目组Id
        /// </summary>
        [Description("项目组Id")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// 项目组
        /// </summary>
        [Description("项目组")]
        public string ProjectName { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        [Description("发起人")]
        public int Sponsor { get; set; }

        /// <summary>
        /// 发起人姓名
        /// </summary>
        [Description("发起人姓名")]
        public string SponsorName { get; set; }

        /// <summary>
        /// 监督人(默认为发起人)
        /// </summary>
        [Description("监督人(默认为发起人)")]
        public int? Supervisor { get; set; }

        /// <summary>
        /// 监督人姓名
        /// </summary>
        [Description("监督人姓名")]
        public string SupervisorName { get; set; }

        /// <summary>
        /// 执行人(默认为发起人
        /// </summary>
        [Description("执行人(默认为发起人)")]
        public int? Executor { get; set; }

        /// <summary>
        /// 执行人姓名
        /// </summary>
        [Description("执行人姓名")]
        public string ExecutorName { get; set; }

        /// <summary>
        /// 来源：会议或通讯录
        /// </summary>
        [Description("来源：会议或通讯录")]
        public WorkTaskSource Source { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Description("优先级")]
        public WorkTaskPriority Priority { get; set; }

        /// <summary>
        /// 工作量(小时)
        /// </summary>
        [Description("工作量(小时)")]
        public int Workload { get; set; }

        /// <summary>
        /// 紧急性: 高中低
        /// </summary>
        [Description("紧急性: 高中低")]
        public WorkTaskUrgency Urgency { get; set; }

        /// <summary>
        /// 重要性: 高中低
        /// </summary>
        [Description("重要性: 高中低")]
        public WorkTaskImportance Importance { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Description("开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 截止时间
        /// </summary>
        [Description("截止时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [Description("完成时间")]
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        [Description("关闭时间")]
        public DateTime? CloseTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public System.DateTime CreatedTime { get; set; }

        /// <summary>
        /// 操作记录
        /// </summary>
        [Description("操作记录")]
        public IList<WorkTaskHistoryModel> Histories { get; set; }

        /// <summary>
        /// 评论
        /// </summary>
        [Description("评论")]
        public IList<WorkTaskCommentModel> Comments { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [Description("附件")]
        public IList<int> Attachments { get; set; }
    }
}
