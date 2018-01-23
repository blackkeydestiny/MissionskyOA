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
    /// 项目
    /// </summary>
    public class ProjectModel : BaseModel
    {
        [Description("项目Id")]
        public int? ProjectId { get; set; }

        [Description("项目名称")]
        public string Name { get; set; }

        [Description("创建时间")]
        public DateTime? CreatedTime { get; set; }
        [Description("项目编号")]
        public string ProjectNo { get; set; }
        [Description("项目经理ID")]
        public int? ProjectManager { get; set; }
        [Description("项目经理名")]
        public string ProjectManagerName { get; set; }
        [Description("项目开始日期")]
        public DateTime? ProjectBegin { get; set; }
        
        [Description("项目结束日期")]
        public DateTime? ProjectEnd { get; set; }
        [Description("创建者用户名")]
        public string CreateUserName { get; set; }
        [Description("项目数据状态")]
        public int? Status { get; set; }
    }
}