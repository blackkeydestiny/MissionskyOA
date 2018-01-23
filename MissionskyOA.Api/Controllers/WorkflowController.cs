using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 工作流
    /// </summary>
    [RoutePrefix("api/workflow")]
    public class WorkflowController : BaseController
    {
        private IWorkflowService WorkflowService { get; set; }

        public WorkflowController(IWorkflowService workflowService)
        {
            this.WorkflowService = workflowService;
        }

        /// <summary>
        /// 获取用户当前的流程任务
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public ApiResponse<List<object>> GetAllTask()
        {
            return new ApiResponse<List<object>>
            {
                Result = new List<object>()
            };
        }

        /// <summary>
        /// 获取单个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public ApiResponse<object> GetTask(int id)
        {
            return new ApiResponse<object>
            {
                Result = new object()
            };
        }

        /// <summary>
        /// 更新Task,处理当前任务,控制下一步任务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpPut]
        public ApiResponse<object> UpdateRole(int id, object model)
        {
            return new ApiResponse<object>
            {
                Result = new object()
            };
        }

        /// <summary>
        /// 获取单个可用流程
        /// </summary>
        /// <param name="model">流程查询模型</param>
        /// <returns>流程</returns>
        [Route("single")]
        [HttpPost]
        public ApiResponse<WorkflowModel> GetSingleWorkflow(SearchWorkflowModel model)
        {
            if (model == null || (model.Type == WorkflowType.None && model.WorkflowId < 1 && string.IsNullOrEmpty(model.Name)))
            {
                throw new KeyNotFoundException("Invalid query parameter");
            }

            return new ApiResponse<WorkflowModel>
            {
                Result = this.WorkflowService.GetSingleWorkflow(model)
            };
        }
    }
}
