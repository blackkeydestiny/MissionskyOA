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
    /// 查询工作任务
    /// </summary>
    public class SearchWorkTaskModel
    {
        /// <summary>
        /// 发起人
        /// </summary>
        [Description("发起人")]
        public int? Sponsor { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        [Description("执行人")]
        public int? Executor { get; set; }

        /// <summary>
        /// 监督人(默认当前用户)
        /// </summary>
        [Description("监督人")]
        public int? Supervisor { get; set; }
        
        /// <summary>
        /// 状态(默认所有状态)
        /// </summary>
        [Description("状态")]
        public WorkTaskStatus? Status { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        [Description("项目Id")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// 会议Id
        /// </summary>
        [Description("会议Id")]
        public int? MeetingId { get; set; }
    }
}
