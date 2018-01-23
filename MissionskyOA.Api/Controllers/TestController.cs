using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MissionskyOA.Api.Filter;
using System.Text;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [RoutePrefix("api/test")]
    public class TestController : BaseController
    {
        private IAttendanceSummaryService AttendanceSummaryService { get; set; }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="attendanceSummaryService"></param>
        public TestController(IAttendanceSummaryService attendanceSummaryService)
        {
            this.AttendanceSummaryService = attendanceSummaryService;
        }

        /// <summary>
        /// 计算工龄年假
        /// </summary>
        /// <param name="joinDate">入职日期: yyyy-MM-dd</param>
        /// <param name="count">测试次数</param>
        /// <returns>入职日期的工龄年假</returns>
        [Route("companyannual")]
        [HttpGet]
        public ApiResponse<String> CaculateBaseValueForCompanyServiceYear(string joinDate, int count)
        {
            StringBuilder strAnnual = new StringBuilder();
            count = count == 0 ? 100 : count;

            if (!String.IsNullOrEmpty(joinDate))
            {
                int days = 0;
                DateTime startTime = DateTime.Parse(joinDate);
                DateTime dtJoinDate;
                double annual = 0;

                do
                {
                    dtJoinDate = startTime.AddDays(days);
                    annual = AttendanceSummaryService.CaculateBaseValueForCompanyServiceYear(dtJoinDate, DateTime.Now);

                    strAnnual.Append(String.Format("JoinDate: {0}, Annual: {1} {2}", dtJoinDate, annual, Environment.NewLine));
                    Log.Debug(strAnnual.ToString());
                    days++;
                } while (count-- <= 0);
            }

            return new ApiResponse<String>
            {
                Result = strAnnual.ToString()
            };
        }

        /// <summary>
        /// 计算法定基础年假
        /// </summary>
        /// <param name="joinDate">入职日期: yyyy-MM-dd</param>
        /// <param name="type">社保类型</param>
        /// <param name="count">测试次数</param>
        /// <returns>入职日期的工龄年假</returns>
        [Route("baseannual")]
        [HttpGet]
        public ApiResponse<String> CaculateBaseValueForTotalServiceYear(string joinDate, int type, int count)
        {
            StringBuilder strAnnual = new StringBuilder();
            count = count == 0 ? 100 : count;

            if (!String.IsNullOrEmpty(joinDate))
            {
                int days = 0;
                DateTime startTime = DateTime.Parse(joinDate);
                DateTime dtJoinDate;
                double annual = 0;

                do
                {
                    dtJoinDate = startTime.AddDays(days);
                    annual = AttendanceSummaryService.CaculateBaseValueForTotalServiceYear(dtJoinDate, type, DateTime.Now);

                    strAnnual.Append(String.Format("JoinDate: {0}, Type: {1}, Annual: {2} {3}", dtJoinDate, type, annual, Environment.NewLine));
                    Log.Debug(strAnnual.ToString());
                    days++;
                } while (count-- <= 0);
            }

            return new ApiResponse<String>
            {
                Result = strAnnual.ToString()
            };
        }

        /// <summary>
        /// 计算年假
        /// </summary>
        /// <param name="joinDate">入职日期: yyyy-MM-dd</param>
        /// <param name="type">社保类型</param>
        /// <returns>入职日期的工龄年假</returns>
        [Route("caculateannual")]
        [HttpGet]
        public ApiResponse<String> CaculateAnnual(string joinDate, int type)
        {
            DateTime dtJoinDate = DateTime.Parse(joinDate);

            return new ApiResponse<String>
            {
                Result = String.Format(
                    "JoinDate: {0}, Type:{1}, Base: {2}, Company: {3}",
                    dtJoinDate,
                    type,
                    AttendanceSummaryService.CaculateBaseValueForTotalServiceYear(dtJoinDate, type, DateTime.Now),
                    AttendanceSummaryService.CaculateBaseValueForCompanyServiceYear(dtJoinDate, DateTime.Now)
                )
            };
        }
    }
}
