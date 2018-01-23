using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;

namespace MissionskyOA.Models
{
    /// <summary>
    /// 处理工作任务
    /// </summary>
    public class OperateWorkTaskModel
    {
        /// <summary>
        /// 处理说明
        /// </summary>
        [Description("处理说明")]
        public string Comment { get; set; }
        
        /// <summary>
        /// 执行人
        /// </summary>
        [Description("执行人")]
        public int? Executor { get; set; }
    }
}
