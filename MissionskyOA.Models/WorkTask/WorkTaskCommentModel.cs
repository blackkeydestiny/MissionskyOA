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
    /// 工作任务评论
    /// </summary>
    public class WorkTaskCommentModel : BaseModel
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        [Description("任务Id")]
        public int TaskId { get; set; }

        /// <summary>
        /// 评论人
        /// </summary>
        [Description("评论人")]
        public int UserId { get; set; }

        /// <summary>
        /// 评论人姓名
        /// </summary>
        [Description("评论人姓名")]
        public string UserName { get; set; }
        
        /// <summary>
        /// 评论
        /// </summary>
        [Description("评论")]
        public string Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }
    }
}
