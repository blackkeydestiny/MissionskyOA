using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 报表
    /// </summary>
    public class ReportModel : BaseModel
    {
        [Description("报表名称")]
        public string Name { get; set; }

        [Description("报表编号")]
        public string No { get; set; }

        [Description("描述")]
        public string Desc { get; set; }

        [Description("开放给终端用户使用")]
        public bool IsOpen { get; set; }
        
        [Description("创建日期")]
        public DateTime CreatedTime { get; set; }
        
        [Description("报表参数")]
        public IList<ReportParameterModel> Parameters { get; set; }

        [Description("报表附件可用格式")]
        public IList<string> Formats { get; set; }
    }
}