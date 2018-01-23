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
    /// 报表配置
    /// </summary>
    public class ReportConfigModel : BaseModel
    {
        [Description("报表Id")]
        public int ReportId { get; set; }

        [Description("配置key")]
        public string Config { get; set; }

        [Description("配置value")]
        public string Value { get; set; }
        
        [Description("创建日期")]
        public DateTime CreatedTime { get; set; }
    }
}