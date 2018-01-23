using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 更新用户基本信息
    /// </summary>
    public class UpdateUserModel
    {
        [Description("工号")]
        public string No { get; set; }
        [Description("中文名")]
        public string ChineseName { get; set; }

        [Description("英文名")]
        public string EnglishName { get; set; }

        [Description("性别")]
        public Gender Gender { get; set; }

        [Description("邮箱")]
        public string Email { get; set; }
        
        [Description("手机")]
        public string Phone { get; set; }

        [Description("直属上级Id")]
        public int? DirectlySupervisorId { get; set; }

        [Description("所属项目组")]
        public int? ProjectId { get; set; }

        [Description("账号状态")]
        public AccountStatus? Status { get; set; }
        [Description("是否在职")]
        public virtual bool? Available { get; set; }

        [Description("今日状态")]
        public virtual UserTodayStatus? TodayStatus { get; set; }

        [Description("QQID")]
        public string QQID { get; set; }

        [Description("用户角色")]
        public int Role { get; set; }

        [Description("部门Id")]
        public int? DeptId { get; set; }

        [Description("职位")]
        public string Position { get; set; }

        [Description("入职日期")]
        public DateTime JoinDate { get; set; }
        /// <summary>
        /// 员工的总共的工作年限
        /// </summary>
        [Description("工龄")]
        public int ServerYear { get; set; }

        /// <summary>
        /// 员工的工龄类型：10年以下、10年到20年、20年以上
        /// </summary>
        [Description("工龄类型")]
        public UserServiceYearType ServerYearType { get; set; }
    }
}
