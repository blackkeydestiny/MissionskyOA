using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Models
{
    public class SearchUserModel
    {
        /// <summary>
        /// 关键字(中文名,英文名
        /// </summary>
        [Description("关键字(中文名,英文名)")]
        public string Keyword { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        [Description("项目Id")]
        public int? ProjectId { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Description("邮箱")]
        public string Email { get; set; }
        
        /// <summary>
        /// 电话号码
        /// </summary>
        [Description("电话号码")]
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Description("QQID")]
        public string QQID { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        [Description("账号状态")]
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 是否在职
        /// </summary>
        [Description("是否在职")]
        public bool Available { get; set; }

        /// <summary>
        /// 用户今日状态
        /// </summary>
        [Description("用户今日状态")]
        public UserTodayStatus TodayStatus { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Description("工号")]
        public string No { get; set; }
    }
}
