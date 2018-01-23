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
    public class RoleModel : BaseModel
    {
        [Description("角色名称")]
        public string RoleName { get; set; }

        [Description("可以批假的天数")]
        public int? ApprovedDays { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }

        [Description("英文简称")]
        public string Abbreviation { get; set; }

        [Description("是否为数据库初始数据,是：1 否：0")]
        public int? IsInitRole { get; set; }
        [Description("创建用户")]
        public string CreateUser { get; set; }
        [Description("状态")]
        public int? Status { get; set; }
    
    }
}