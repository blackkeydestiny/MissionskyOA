using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    public enum ExpenseAuditStep
    {
        /// <summary>
        /// 申请
        /// </summary>
        [Description("申请")]
        Apply = 0,

        /// <summary>
        /// 审核中
        /// </summary>
        [Description("部门审批")]
        DepartmentAudit = 1,

        /// <summary>
        /// 已批准
        /// </summary>
        [Description("财务审批")]
        FinacialAudit = 2
    }
}

