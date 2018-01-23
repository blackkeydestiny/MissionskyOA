using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Common
{
    /// <summary>
    /// 常量
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 用户消息推送默认授权
        /// </summary>
        public const string USER_NOTIFY_DEFAULT_AUTH = "LeaveApprove:1;OvertimeApprove:1;ExpenseApprove:1;Notice:1;WorkTask:1;";

        /// <summary>
        /// 附件类型:工作任务
        /// </summary>
        public const string ATTACHMENT_TYPE_WORK_TASK = "工作任务";

        /// <summary>
        /// 附件类型:图书封面
        /// </summary>
        public const string ATTACHMENT_TYPE_BOOK_COVER = "图书封面";

        /// <summary>
        /// 附件类型:申请单附件
        /// </summary>
        public const string ATTACHMENT_TYPE_ORDER_ATTACHMENT = "申请单附件";

        /// <summary>
        /// 会议纪要连接参数配置
        /// </summary>
        public const string MEETING_SUMMARY_URL = "MeetingSummaryURL";

        /// <summary>
        /// 资产管理员
        /// </summary>
        public const string ROLE_ASSET_ADMIN = "资产管理员";
        
        /// <summary>
        /// 行政专员
        /// </summary>
        public const string ROLE_ADMIN_STAFF = "行政专员";
        
        /// <summary>
        /// 行政人事部
        /// </summary>
        public const string DEPT_ADMIN = "行政人事部";

        #region 报表相关常量
        /// <summary>
        /// 报表格式，数据字典索引
        /// </summary>
        public const string DATADICT_INDEX_REPORT_FORMAT = "报表格式";

        /// <summary>
        /// 报表参数类型，数据字典索引
        /// </summary>
        public const string DATADICT_INDEX_REPORT_PARAM_TYPE = "报表参数类型";

        /// <summary>
        /// 报表参数取值范围，数据字典索引
        /// </summary>
        public const string DATADICT_REPORT_PARAM_DATASOURCE = "报表参数数据源";

        /// <summary>
        /// 报表参数：数据源Table分隔符
        /// </summary>
        public const char REPORT_PARAM_TABLE_SPLIT = '|';

        /// <summary>
        /// 报表参数：数据源字段分隔符
        /// </summary>
        public const char REPORT_PARAM_FIELD_SPLIT = ',';

        /// <summary>
        /// 报表服务地址
        /// </summary>
        public const string REPORT_CONFIG_SERVICE_URL = "ServiceUrl";

        /// <summary>
        /// 报表路径
        /// </summary>
        public const string REPORT_CONFIG_REPORT_PATH = "ReportPath";

        /// <summary>
        /// 报表默认格式
        /// </summary>
        public const string REPORT_CONFIG_DEFAULT_FORMAT = "DefaultFormat";

        /// <summary>
        /// 用户名
        /// </summary>
        public const string REPORT_CONFIG_USER_NAME = "UserName";

        /// <summary>
        /// 用户密码
        /// </summary>
        public const string REPORT_CONFIG_PASSWORD = "Password";

        /// <summary>
        /// 域
        /// </summary>
        public const string REPORT_CONFIG_DOMAIN = "Domain";

        /// <summary>
        /// 默认格式
        /// </summary>
        public const string DEFAULT_REPORT_FORMAT = "Excel";

        /// <summary>
        /// 报表存放临时目录
        /// </summary>
        public const string WEB_CONFIG_TEMP_FOLDER = "ReportTemp";

        /// <summary>
        /// 参数类型: DropdownList
        /// </summary>
        public const string REPORT_PARAM_TYPE_DROPDOWNLIST = "DropdownList";
        #endregion

        #region 数据表
        /// <summary>
        /// 资产类型
        /// </summary>
        public const string TABLE_ASSETTYPE = "AssetType";

        /// <summary>
        /// 资产……
        /// </summary>
        public const string TABLE_ASSETINVENTORY = "AssetInventory";

        /// <summary>
        /// 项目
        /// </summary>
        public const string TABLE_PROJECT = "Project";

        /// <summary>
        /// 部门
        /// </summary>
        public const string TABLE_DEPARTMENT = "Department";

        /// <summary>
        /// 角色
        /// </summary>
        public const string TABLE_ROLE = "Role";
        
        /// <summary>
        /// 数据字典
        /// </summary>
        public const string TABLE_DATADICT = "DataDict";

        /// <summary>
        /// 用户
        /// </summary>
        public const string TABLE_USER = "User";
        #endregion

        #region 流程常量
        /// <summary>
        /// 无效节点
        /// </summary>
        public const int WORKFLOW_INVALID_NODE = -100;

        /// <summary>
        /// 申请节点
        /// </summary>
        public const int WORKFLOW_APPLY_NODE = 0;

        /// <summary>
        /// 直接领导审批节点
        /// </summary>
        public const int WORKFLOW_DIRECT_SUPERVISOR_APPROVE_NODE = 100;
        #endregion

        #region 配置项
        /// <summary>
        /// 配置项：报销申请单模板编号
        /// </summary>
        public const string CONFIG_KEY_EXPENSE_ORDER_REPORT_NO = "ExpenseOrderReportNo";
        #endregion
    }
}
