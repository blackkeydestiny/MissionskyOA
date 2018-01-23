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
    /// 报表附件
    /// </summary>
    public class SendReportModel
    {
        [Description("报表附件格式")]
        public string Format { get; set; }

        [Description("参数")]
        public Dictionary<string, string> ReportParams { get; set; }
    }
}