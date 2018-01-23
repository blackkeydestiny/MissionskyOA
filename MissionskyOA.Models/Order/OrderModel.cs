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
    /// 用户模型
    /// </summary>
    public class OrderModel
    {
        [Description("假单类型")]
        public OrderType OrderType { get; set; }

        [Description("状态")]
        public OrderStatus Status { get; set; }

        [Description("申请单Id")]
        public int Id { get; set; }

        [Description("用户Id")]
        public int UserId { get; set; }

        [Description("是否[行政或财务]审阅")]
        public bool? IsReview { get; set; }
        
        [Description("流程申请人Id")]
        public int ApplyUserId { get; set; }

        [Description("申请人名")]
        public string ApplyUserName { get; set; }

        [Description("批量申请Id")]
        public int OrderNo { get; set; }

        [Description("是否批量申请")]
        public bool IsBatchApply { get; set; }

        [Description("审批人Id")]
        public int? RefOrderId { get; set; }

        [Description("流程Id")]
        public int? WorkflowId { get; set; }

        [Description("流程名称")]
        public string WorkflowName { get; set; }

        [Description("下一个流程步骤")]
        public int? NextStep { get; set; }

        [Description("审批人姓名")]
        public int? NextAudit { get; set; }

        [Description("审批人Id")]
        public string NextAuditName { get; set; }

        [Description("当前审批意见")]
        public string AuditAdvice { get; set; }

        [Description("关联单据")]
        public OrderModel RefOrder { get; set; }

        [Description("创建时间")]
        public DateTime CreatedTime { get; set; }

        [Description("假单详情")]
        public ICollection<OrderDetModel> OrderDets { get; set; }

        [Description("流程处理历史")]
        public IList<WorkflowProcessModel> ProcessHistory { get; set; }

        [Description("用户名")]
        public string UserName { get; set; }

        [Description("部门名")]
        public string DeptName { get; set; }

        [Description("项目组名")]
        public string ProjectName { get; set; }
        
        /// <summary>
        /// 申请单附件
        /// </summary>
        [Description("申请单附件")]
        public IList<int> Attachments { get; set; }
        
        /// <summary>
        /// 申请单用户
        /// </summary>
        [Description("申请单用户")]
        public IList<OrderUserModel> OrderUsers { get; set; }
    }
}
