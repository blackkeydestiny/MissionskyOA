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

namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/audit")]
    public class AuditMessageController : BaseController
    {
        private IAuditMessageService AuditMessageService { get; set; }

        public AuditMessageController(IAuditMessageService auditMessageService)
        {
            this.AuditMessageService = auditMessageService;
        }

        /// <summary>
        /// 获取用户审计信息
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>返回用户审计信息及分页信息</returns>
        [Route("getUserAuditMessages")]
        [HttpPost]
        public ApiPagingListResponse<AuditMessageModel> GetUserAuditMessages(SearchAuditMessageModel model, int pageIndex = 0, int pageSize = 25)
        {
            if (model == null)
            {
                throw new Exception("The request body cant't be null.");
            }

            //验证User Id
            if (model.UserId < 1)
            {
                throw new ApiBadRequestException("Invalid user id");
            }

            //查页查询审计信息
            var query = this.AuditMessageService.GetUserAuditMessages(model, pageIndex, pageSize);

            //查询结果
            var result = new PaginationModel<AuditMessageModel>();
            result.Result = query.Result;

            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<AuditMessageModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }
    }
}
