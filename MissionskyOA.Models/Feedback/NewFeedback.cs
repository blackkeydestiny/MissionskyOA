using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models.Feedback
{
    /// <summary>
    /// 编辑新的意见反馈
    /// </summary>
    public class NewFeedback : BaseModel
    {
        /// <summary>
        /// 问题描述
        /// </summary>
        [Description("问题描述")]
        public string Description { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        public int PictureID { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        [Description("问题类型")]
        public ProblemType ProblemType { get; set; }


        /// <summary>
        /// 主题
        /// </summary>
        [Description("主题")]
        public string Theme { get; set; }

        /// <summary>
        /// 反馈意见的员工ID
        /// </summary>
        [Description("反馈意见的员工ID")]
        public int UserID { get; set; }

        /// <summary>
        /// 反馈的时间
        /// </summary>
        [Description("反馈的时间")]
        public DateTime Datetime { get; set; }
    }
}
