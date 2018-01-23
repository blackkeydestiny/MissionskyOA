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
    public class ApplyOrderModel
    {
        [Description("单类型")]
        public OrderType OrderType { get; set; }

        [Description("开始日期")]
        public DateTime StartDate { get; set; }

        [Description("结束日期")]
        public DateTime EndDate { get; set; }

        [Description("申请时长")]
        public double IOHours { get; set; }

        [Description("申请理由")]
        public string Description { get; set; }

        /// <summary>
        /// 申请人Id可以有多个：表示可以或者可能有多个申请用户
        /// </summary>
        [Description("申请人")]
        public int[] UserIds { get; set; }

        [Description("是否告知领导")]
        public bool? InformLeader { get; set; }

        [Description("是否工作交接")]
        public bool? WorkTransfer { get; set; }

        [Description("移交人")]
        public int? Recipient { get; set; }
    }
}
