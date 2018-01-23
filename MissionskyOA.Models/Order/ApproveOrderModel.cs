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
    /// <summary>
    /// 审批申请单
    /// </summary>
    public class ApproveOrderModel
    {
        [Description("申请单号")]
        public int OrderNo { get; set; }

        [Description("状态")]
        public OrderStatus Status { get; set; }

        [Description("申请单类型")]
        public OrderType OrderType { get; set; }

        [Description("是否是撤销单")]
        public bool IsRevoked { get; set; }

        [Description("当前用户是否是已审批")]
        public bool IsApproved { get; set; }

        [Description("申请单用户")]
        public IList<OrderUserModel> OrderUsers { get; set; }
        
        //[Description("开始日期")]
        //public DateTime StartDate { get; set; }

        //[Description("结束日期")]
        //public DateTime EndDate { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        [Description("申请日期")]
        public DateTime AppliedTime { get; set; }

        [Description("时长")]
        public double? IOHours { get; set; }
    }
}
