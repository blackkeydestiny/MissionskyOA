using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using System.Threading.Tasks;

namespace MissionskyOA.Models
{
    public class RevokeOrderModel
    {
        [Description("撤销开始日期")]
        public DateTime StartDate { get; set; }

        [Description("撤销结束日期")]
        public DateTime EndDate { get; set; }

        [Description("撤销时长")]
        public double IOHours { get; set; }

        [Description("撤销理由")]
        public string RevokeReason { get; set; }

        [Description("申请人")]
        public int[] UserIds { get; set; }

        [Description("是否告知领导")]
        public bool InformLeader { get; set; }

        [Description("是否工作交接")]
        public bool WorkTransfer { get; set; }

        [Description("移交人")]
        public int Recipient { get; set; }
    }
}
