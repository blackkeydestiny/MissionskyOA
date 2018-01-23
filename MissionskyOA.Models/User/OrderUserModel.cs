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
    /// 申请单用户模型
    /// </summary>
    public class OrderUserModel
    {
        [Description("申请单Id")]
        public int OrderId { get; set; }

        [Description("用户Id")]
        public int Id { get; set; }

        [Description("用户")]
        public string UserName { get; set; }

        [Description("部门")]
        public string DeptName { get; set; }

        [Description("项目组")]
        public string ProjectName { get; set; }
        
        [Description("用户性别")]
        public Gender Gender { get; set; }
        
    }
}
