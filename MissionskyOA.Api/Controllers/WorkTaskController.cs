using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;
using MissionskyOA.Services.Extentions;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 工作任务
    /// </summary>
    [RoutePrefix("api/worktask")]
    public class WorkTaskController : BaseController
    {
        private IWorkTaskService WorkTaskService { get; set; }
        private IAttachmentService AttachmentService { get; set; }
        private IWorkTaskCommentService WorkTaskCommentService { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkTaskController(IWorkTaskService workTaskService, IWorkTaskCommentService workTaskCommentService, IAttachmentService attachmentService)
        {
            this.WorkTaskService = workTaskService;
            this.WorkTaskCommentService = workTaskCommentService;
            this.AttachmentService = attachmentService;
        }

        /// <summary>
        /// 创建工作任务
        /// </summary>
        /// <param name="task">工作任务详细</param>
        /// <returns>返回新的Task id</returns>
        [Route("add")]
        [HttpPost]
        public ApiResponse<int> AddWorkTask(NewWorkTaskModel task)
        {
            task.Valid(); //验证Task参数是否有效

            var response = new ApiResponse<int>()
            {
                Result = WorkTaskService.Add(task, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 修改工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="task">工作任务详细变更</param>
        /// <returns></returns>
        [Route("edit/{taskId}")]
        [HttpPut]
        public ApiResponse<bool> EditWorkTask(int taskId, NewWorkTaskModel task)
        {
            task.Valid(); //验证Task参数是否有效

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Edit(taskId, task, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 取消工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">取消工作任务原因</param>
        /// <returns></returns>
        public ApiResponse<bool> CancelWorkTask(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            string season = model == null || string.IsNullOrEmpty(model.Comment) ? "任务取消" : model.Comment;

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Cancel(taskId, season, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 转移(指派)工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">备注</param>
        /// <returns></returns>
        public ApiResponse<bool> TransferWorkTask(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            if (model == null || !model.Executor.HasValue)
            {
                Log.Error("无效的执行人。");
                throw new InvalidOperationException("无效的执行人。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Transfer(taskId, model.Executor.Value, this.Member, model.Comment)
            };

            return response;
        }

        /// <summary>
        /// 开始工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">备注</param>
        /// <returns></returns>
        [Route("start/{taskId}")]
        [HttpPut]
        public ApiResponse<bool> StartWorkTask(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            model = model ?? new OperateWorkTaskModel();

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Do(taskId, WorkTaskStatus.Started, this.Member, model.Comment)
            };

            return response;
        }

        /// <summary>
        /// 完成工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">备注</param>
        /// <returns></returns>
        [Route("complete/{taskId}")]
        [HttpPut]
        public ApiResponse<bool> CompleteWorkTask(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            model = model ?? new OperateWorkTaskModel();

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Do(taskId, WorkTaskStatus.Completed, this.Member, model.Comment)
            };

            return response;
        }

        /// <summary>
        /// 关闭工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">备注</param>
        /// <returns></returns>
        [Route("close/{taskId}")]
        [HttpPut]
        public ApiResponse<bool> CloseWorkTask(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            model = model ?? new OperateWorkTaskModel();

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskService.Do(taskId, WorkTaskStatus.Closed, this.Member, model.Comment)
            };

            return response;
        }
        
        /// <summary>
        /// 添加工作任务评论
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="model">评论</param>
        /// <returns></returns>
        [Route("comment/add/{taskId}")]
        [HttpPost]
        public ApiResponse<bool> AddWorkTaskComment(int taskId, OperateWorkTaskModel model)
        {
            if (taskId < 0)
            {
                Log.Error("无效的工作任务。");
                throw new InvalidOperationException("无效的工作任务。");
            }

            model = model ?? new OperateWorkTaskModel();

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskCommentService.AddComment(this.Member, taskId, model.Comment)
            };

            return response;
        }

        /// <summary>
        /// 修改工作任务评论
        /// </summary>
        /// <param name="commentId">工作任务评论Id</param>
        /// <param name="model">评论</param>
        /// <returns></returns>
        [Route("comment/modify/{commentId}")]
        [HttpPut]
        public ApiResponse<bool> ModifyWorkTaskComment(int commentId, OperateWorkTaskModel model)
        {
            if (commentId < 0)
            {
                Log.Error("无效的工作任务评论。");
                throw new InvalidOperationException("无效的工作任务评论。");
            }

            model = model ?? new OperateWorkTaskModel();

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskCommentService.ModifyComment(this.Member, commentId, model.Comment)
            };

            return response;
        }
        
        /// <summary>
        /// 删除工作任务评论
        /// </summary>
        /// <param name="commentId">工作任务评论Id</param>
        /// <returns></returns>
        [Route("comment/delete/{commentId}")]
        [HttpGet]
        public ApiResponse<bool> DeleteWorkTaskComment(int commentId)
        {
            if (commentId < 0)
            {
                Log.Error("无效的工作任务评论。");
                throw new InvalidOperationException("无效的工作任务评论。");
            }

            var response = new ApiResponse<bool>()
            {
                Result = WorkTaskCommentService.DeleteComment(commentId)
            };

            return response;
        }

        /// <summary>
        /// 上传工作任务附件
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <returns>是否上传成功</returns>
        [Route("upload/{taskId}")]
        [HttpPost]
        public ApiResponse<bool> Upload(int taskId)
        {
            return AttachmentController.Upload(taskId, Constant.ATTACHMENT_TYPE_WORK_TASK);
        }

        /// <summary>
        /// 下载工作任务附件
        /// </summary>
        /// <param name="attaId">附件Id</param>
        /// <returns>工作任务附件文件流</returns>
        [Route("download/{attaId}")]
        [HttpGet]
        public HttpResponseMessage Download(int attaId)
        {
            var model = this.AttachmentService.GetAttathcmentDetail(attaId);
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            if (model != null)
            {
                result.Content = new ByteArrayContent(model.Content);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            }
            else
            {
                result.StatusCode = HttpStatusCode.NotFound;
            }

            return result;
        }

        /// <summary>
        /// 分页条件查询工作任务
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>工作任务分页列表</returns>
        [Route("search")]
        [HttpPost]
        public ApiPagingListResponse<WorkTaskModel> SearcWorkTasks(SearchWorkTaskModel search, int pageIndex = 0, int pageSize = 25)
        {
            //查询满足条件的图书
            var query = this.WorkTaskService.SearchByCriteria(search, pageIndex, pageSize);

            //查询结果
            var result = new PaginationModel<WorkTaskModel>();
            result.Result = query.Result;

            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<WorkTaskModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据工作任务ID获取详细信息
        /// </summary>
        /// <param name="taskId">工作任务id</param>
        /// <returns>工作任务详细信息</returns>
        [Route("{taskId}")]
        [HttpGet]
        public ApiResponse<WorkTaskModel> GetWorkTaskDetail(int taskId)
        {
            if (taskId < 1)
            {
                throw new Exception("无效的工作任务ID。");
            }

            var task = this.WorkTaskService.GetWorkTaskDetail(taskId);

            var response = new ApiResponse<WorkTaskModel>()
            {
                Result = task
            };

            return response;
        }

        /// <summary>
        /// 统计个人任务信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("summary/{userId}")]
        [HttpGet]
        public ApiResponse<List<WorkTaskSummaryModel>> SummaryWorkTask(int userId)
        {
            return new ApiResponse<List<WorkTaskSummaryModel>>
            {
                Result = this.WorkTaskService.SummaryWorkTask(userId)
            };
        }
    }
}
