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
    /// 申请数据量模型
    /// </summary>
    public class PendingOrderCountModel
    {
        [Description("请假待审批数量")]
        public int AskLeave { get; set; }

        [Description("加班待审批数量")]
        public int Overtime { get; set; }
    }
}
