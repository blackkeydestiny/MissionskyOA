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
    /// 报表参数配置
    /// </summary>
    public class ReportParameterModel : BaseModel
    {
        [Description("报表Id")]
        public int ReportId { get; set; }

        [Description("参数名称")]
        public string Name { get; set; }

        [Description("参数说明")]
        public string Desc { get; set; }

        [Description("参数类型")]
        public string Type { get; set; }

        [Description("是否可空")]
        public bool Nullable { get; set; }
        
        [Description("创建日期")]
        public DateTime CreatedTime { get; set; }

        [Description("参数取值表")]
        public string DataTable { get; set; }

        [Description("参数取值范围")]
        public Dictionary<string, string> DataSource { get; set; }
    }
}