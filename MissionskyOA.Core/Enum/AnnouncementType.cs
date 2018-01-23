using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MissionskyOA.Core.Enum
{
    /// <summary>
    /// 业务类型
    /// </summary>
    [Description("业务类型")]
    public enum AnnouncementType
    {
        /// <summary>
        /// 资产盘点
        /// </summary>
        [Description("资产盘点")]
        AssetInventory=1,

        /// <summary>
        /// 行政部门公告通知
        /// </summary>
        [Description("行政部门公告通知，仅限于行政")]
        AdministrationEventAnnounce = 2,

        /// <summary>
        /// 公司新闻
        /// </summary>
        [Description("公司新闻,仅限于行政")]
        CompanyNews=3,

        /// <summary>
        /// 员工消息
        /// </summary>
        [Description("员工消息，仅限于普通员工")]
        EmployeeeNews = 4,

        /// <summary>
        /// 活动消息
        /// </summary>
        [Description("活动消息,仅限于普通员工")]
        ActivityMessage = 5,

    }
}
