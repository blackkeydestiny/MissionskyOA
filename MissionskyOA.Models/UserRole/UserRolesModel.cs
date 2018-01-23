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
    /// 角色
    /// </summary>
    public class UserRolesModel : BaseModel
    {
        [Description("角色Id")]
        public int RoleId { get; set; }

        [Description("用户Id")]
        public int UserId { get; set; }

        [Description("中文名")]
        public string ChineseName { get; set; }
        [Description("英文名")]
        public string EnglishName { get; set; }
        /// <summary>
        /// 是否在职
        /// </summary>
        [Description("是否在职")]
        public bool Available { get; set; }
        [Description("部门名称")]
        public string DeptName { get; set; }
        [Description("项目名称")]
        public string ProjectName { get; set; }
        [Description("职位")]
        public string Position { get; set; }
    }
}