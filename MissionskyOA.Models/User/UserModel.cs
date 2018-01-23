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
    /// 用户模型
    /// </summary>
    public class UserModel : BaseModel
    {
        [Description("中文名")]
        public string ChineseName { get; set; }

        [Description("英文名")]
        public string EnglishName { get; set; }

        [Description("性别")]
        public Gender Gender { get; set; }

        [Description("邮箱")]
        public string Email { get; set; }

        [Description("密码")]
        public string Password { get; set; }

        [Description("手机")]
        public string Phone { get; set; }

        [Description("直属上级Id")]
        public int? DirectlySupervisorId { get; set; }

        [Description("所属项目组")]
        public int? ProjectId { get; set; }

        [Description("项目名称")]
        public string ProjectName { get; set; }

        [Description("账号状态")]
        public AccountStatus Status { get; set; }

        [Description("QQID")]
        public string QQID { get; set; }

        [Description("Token")]
        public string Token { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }

        [Description("用户角色")]
        public int Role { get; set; }

        [Description("用户角色名称")]
        public string RoleName { get; set; }
        
        [Description("部门Id")]
        public int? DeptId { get; set; }

        [Description("部门名称")]
        public string DeptName { get; set; }

        [Description("职位")]
        public string Position { get; set; }

        [Description("入职日期")]
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [Description("工号")]
        public string No { get; set; }

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
        /// 是否是资产管理员
        /// </summary>
        [Description("是否是资产管理员")]
        public bool IsAssetManager { get; set; }

        /// <summary>
        /// 是否为行政专员
        /// </summary>
        [Description("是否为行政专员")]
        public bool IsAdminStaff { get; set; }
        
        /// <summary>
        /// 用户年度考勤(加班/请假)汇总
        /// </summary>
        [Description("用户年度考勤(加班/请假)汇总")]
        public IList<AttendanceSummaryModel> VacationSummary { get; set; }

        /// <summary>
        /// 用户消息推送授权
        /// </summary>
        [Description("用户消息推送授权")]
        public Dictionary<string, bool> AuthNotify { get; set; }

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

        //for deleting UserRoles
        //[Description("假期列表")]
        //public virtual ICollection<OrderModel> Orders { get; set; }

        //[Description("用户角色列表")]
        //public virtual ICollection<RoleModel> UserRoles { get; set; }
    }
}
