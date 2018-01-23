using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 报表处理接口
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// 获取用户可以使用的报表
        /// </summary>
        /// <param name="token">用户Token</param>
        /// <returns>用户可以使用的报表</returns>
        IList<ReportModel> GetUserReports(string token);

        /// <summary>
        /// 获取报表服务配置
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表详细配置</returns>
        ReportModel GetReportDetail(int reportId);

        /// <summary>
        /// 获取报表服务配置
        /// </summary>
        /// <param name="reportNo">报表编号</param>
        /// <returns>报表详细配置</returns>
        ReportModel GetReportDetail(string reportNo);

        /// <summary>
        /// 获取参数详细
        /// </summary>
        /// <param name="paramId">参数Id</param>
        /// <returns>参数详细</returns>
        ReportParameterModel GetParameterDetail(int paramId);
        
        /// <summary>
        /// 添加报表
        /// </summary>
        /// <param name="report">报表</param>
        /// <param name="configs">报表配置</param>
        /// <returns>true or exception</returns>
        int AddReport(ReportModel report, NameValueCollection configs);
        
        /// <summary>
        /// 更新报表
        /// </summary>
        /// <param name="report">报表</param>
        /// <param name="configs">报表配置</param>
        /// <returns>true or exception</returns>
        bool UpdateReport(ReportModel report, NameValueCollection configs);

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="id">报表id</param>
        /// <returns>是否更新成功</returns>
        bool Delete(int id);

        /// <summary>
        /// 添加报表参数
        /// </summary>
        /// <param name="param">报表参数</param>
        /// <returns>report id</returns>
        int AddParameter(ReportParameterModel param);

        /// <summary>
        /// 更新报表参数
        /// </summary>
        /// <param name="param">报表参数</param>
        /// <returns>true or exception</returns>
        bool UpdateParameter(ReportParameterModel param);
        
        /// <summary>
        /// 删除报表参数
        /// </summary>
        /// <param name="id">报表参数id</param>
        /// <returns>是否更新成功</returns>
        bool DeleteParameter(int id);

        /// <summary>
        /// 获取报表配置
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表配置</returns>
        NameValueCollection GetReportConfigs(int reportId);

        /// <summary>
        /// 获取报表配置
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表配置</returns>
        IList<ReportParameterModel> GetReportParameters(int reportId);
        
        /// <summary>
        /// 获取报表输出格式
        /// </summary>
        /// <returns>报表输出格式</returns>
        List<string> GetReportFormats();

        /// <summary>
        /// 获取报表参数类型
        /// </summary>
        /// <returns>报表参数类型</returns>
        List<string> GetReportParameterTypes();

        /// <summary>
        /// 获取所有报表
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>所有报表</returns>
        ListResult<ReportModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);
    }
}
