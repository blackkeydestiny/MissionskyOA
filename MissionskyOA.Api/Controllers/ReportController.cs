using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.Common.Report;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 报表管理
    /// </summary>
    [RoutePrefix("api/reports")]
    public class ReportController : BaseController
    {
        private IReportService ReportService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportService"></param>
        public ReportController(IReportService reportService)
        {
            this.ReportService = reportService;
        }
        
        /// <summary>
        /// 向当前用户发送报表
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <param name="reportInfo">报表附件信息: 报表格式(Excel, Word, PDF), 报表参数(键值对)</param>
        /// <returns>true or false</returns>
        [Route("send/{reportId}")]
        [HttpPost]
        public ApiResponse<bool> SendReport(int reportId, SendReportModel reportInfo)
        {
            if (reportInfo == null)
            {
                Log.Error("报表附件信息无效。");
                throw new InvalidOperationException("报表附件信息无效。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = ReportHelper.SendReport(reportId, this.Token, reportInfo.Format, reportInfo.ReportParams)
            };

            return response;
        }

        /// <summary>
        /// 获取用户可以使用的报表
        /// </summary>
        /// <returns></returns>
        [Route("user")]
        [HttpGet]
        public ApiListResponse<ReportModel> GetUserReports()
        {
            var response = new ApiListResponse<ReportModel>()
            {
                Result = ReportService.GetUserReports(this.Token).ToList()
            };

            return response;
        }

        /// <summary>
        /// 获取报表详细，包括参数、可用附件格式
        /// </summary>
        /// <returns></returns>
        [Route("detail/{reportId}")]
        [HttpGet]
        public ApiResponse<ReportModel> GetReportDetail(int reportId)
        {
            var response = new ApiResponse<ReportModel>()
            {
                Result = ReportService.GetReportDetail(reportId)
            };

            return response;
        }

        /// <summary>
        /// 获取报表参数
        /// </summary>
        /// <returns></returns>
        [Route("parameter/{reportId}")]
        [HttpGet]
        public ApiListResponse<ReportParameterModel> GetReportParameters(int reportId)
        {
            var response = new ApiListResponse<ReportParameterModel>()
            {
                Result = ReportService.GetReportParameters(reportId).ToList()
            };

            return response;
        }

        /// <summary>
        /// 获取报表附件可用格式
        /// </summary>
        /// <returns></returns>
        [Route("formats")]
        [HttpGet]
        public ApiListResponse<string> GetReportFormats()
        {
            var response = new ApiListResponse<string>()
            {
                Result = ReportService.GetReportFormats()
            };

            return response;
        }

        /// <summary>
        /// 获取报表参数类型
        /// </summary>
        /// <returns></returns>
        [Route("parameter/types")]
        [HttpGet]
        public ApiListResponse<string> GetReportParamTypes()
        {
            var response = new ApiListResponse<string>()
            {
                Result = ReportService.GetReportParameterTypes()
            };

            return response;
        }
    }
}