using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    public enum ExpenseType
    {
        /// <summary>
        /// 加班费用
        /// </summary>
        [Description("加班费用")]
         OvertimeExpense= 1,

        /// <summary>
        /// 加班车费
        /// </summary>
        [Description("加班车费")]
        TransportationExpense = 2,

        /// <summary>
        /// 团队建设
        /// </summary>
        [Description("团队建设")]
        TeambuildingExpense = 3,
        
        /// <summary>
        /// 招待费
        /// </summary>
        [Description("招待费")]
        hospitalityExpense = 4
    }
}
