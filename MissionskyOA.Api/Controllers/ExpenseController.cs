using MissionskyOA.Api.ApiException;
using MissionskyOA.Core.Security;
using MissionskyOA.Models;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using System.Configuration;
using MissionskyOA.Core.Common;


namespace MissionskyOA.Api.Controllers
{
    [RoutePrefix("api/expense")]
    public class ExpenseController : BaseController
    {
        private IExpenseService ExpenseService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expenseService">报销服务管理</param>
        public ExpenseController(IExpenseService expenseService)
        {
            this.ExpenseService = expenseService;
        }

        /// <summary>
        /// 新增报销申请
        /// </summary>
        /// <returns></returns>
        [Route("applyExpense")]
        [HttpPost]
        [RequestBodyFilter]
        public ApiResponse<ExpenseMainModel> AddExpense(ApplyExpenseModel model)
        {
            // 1. 检查输入参数
            if (model == null)
            {
                throw new ApiBadRequestException("无效的参数");
            }
            // 2. 构造报销申请模型记录
            var expenseMainDataModel = new ExpenseMainModel()
            {
                DeptNo = model.DeptNo,
                ProjNo = model.ProjNo,
                Amount = model.Amount,
                Reason = model.Reason,
                ApplyUserId=this.Member.Id,
                CreatedTime=DateTime.Now
            };

            //3 构造请假单详情
            expenseMainDataModel.ExpenseDetails= new List<ExpenseDetailModel>();
            if (model.ExpenseDetails != null && (model.ExpenseDetails.Count > 0))
            {
                foreach (ApplyExpenseDetailModel item in model.ExpenseDetails)
                {
                    var expenseDetailModel = new ExpenseDetailModel()
                    {
                        ODate = item.ODate,
                        EType = item.EType,
                        Remark = item.Remark,
                        PCount = 1,
                        Amount = item.Amount
                    };
                    //3.1 构造参与人员
                    List<ExpenseMemberModel> activyParticipants = new List<ExpenseMemberModel>();
                    if (item.participants != null && (item.participants.Length > 0))
                    {
                        foreach (int participantItem in item.participants)
                        {
                            var expenseMemberModel = new ExpenseMemberModel()
                            {
                                MemberId = participantItem
                            };
                            activyParticipants.Add(expenseMemberModel);
                        }
                    }
                    expenseDetailModel.ExpenseMembers = activyParticipants;
                    expenseMainDataModel.ExpenseDetails.Add(expenseDetailModel);
                }
            }

            //新增假单 如果是请假或者提交加班申请需要启动工作申请流程
            // 3. Construct API Response
            var response = new ApiResponse<ExpenseMainModel>()
            {
                Result = ExpenseService.AddExpense(expenseMainDataModel)
            };

            return response;
        }

        /// <summary>
        /// 获取自己报销申请单列表
        /// </summary>
        /// <returns></returns>
        [Route("list")]
        [HttpGet]
        public ApiPagingListResponse<ExpenseMainModel> GetExpenseList(int pageIndex = 0, int pageSize = 15)
        {

            // 3. 获取所我的报销单信息
            var query = this.ExpenseService.MyExpenseList(this.Member, pageIndex, pageSize);

            var result = new PaginationModel<ExpenseMainModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<ExpenseMainModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据报销单id,获取报销单详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/details")]
        [HttpGet]
        public ApiListResponse<ExpenseDetailModel> GetExpenseDetail(int Id)
        {
            // 1. 验证具体请假ID
            if (Id < 1)
            {
                throw new Exception("无效的请求");
            }

            // 2. 根据报销单ID获取报销的具体信息
            var expenseDetails = this.ExpenseService.getExpenseDetailByID(Id);

            // 3. 构造 API Response 信息
            ApiListResponse<ExpenseDetailModel> response = new ApiListResponse<ExpenseDetailModel>()
            {
                Result = expenseDetails
            };

            return response;
        }

        /// <summary>
        /// 发送报销申请单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/form")]
        [HttpGet]
        public ApiResponse<bool> SendExpenseForm(int id)
        {
            if (id < 1)
            {
                throw new Exception("无效的请求");
            }

            var response = new ApiResponse<bool>()
            {
                Result = this.ExpenseService.SendExpenseForm(id)
            };

            return response;
        }
        
        /// <summary>
        /// 获取待处理的报销单
        /// </summary>
        /// <returns>审批任务列表</returns>
        [Route("pendingAuditList")]
        [HttpGet]
        public ApiListResponse<ExpenseMainModel> GetPendingAuditLists()
        {
            if (this.Member == null || this.Member.Id < 1)
            {
                throw new Exception("Invalid user.");
            }

            ApiListResponse<ExpenseMainModel> response = new ApiListResponse<ExpenseMainModel>()
            {
                Result = this.ExpenseService.getPendingAuditLists(this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 批准
        /// </summary>
        /// <param name="expenseId">申请单id</param>
        /// <param name="reason">理由</param>
        /// <returns>是否批准成功</returns>
        [Route("approve")]
        [HttpPut]
        public ApiResponse<bool> ApproveExpense(int expenseId, string reason = "")
        {
            if (expenseId < 1)
            {
                Log.Error("无效的请求");
                throw new Exception("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = this.ExpenseService.ApproveOrRejectExpense(expenseId, reason, this.Member, true)
            };

            return response;
        }

        /// <summary>
        /// 确认接收纸质报销单
        /// </summary>
        /// <param name="expenseId">申请单id</param>
        [Route("Expensefile/comfirm")]
        [HttpPut]
        public ApiResponse<bool> ApproveExpense(int expenseId)
        {
            if (expenseId < 1)
            {
                throw new Exception("无效的请求");
            }
             if (!this.Member.Email.EqualsIgnoreCase(Global.FinancialEmail))
            {
                 throw new Exception("无审批权限");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = this.ExpenseService.ReciveExpenseFile(expenseId,this.Member)
            };

            return response;
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="expenseId">申请单id</param>
        /// <param name="reason">理由</param>
        /// <returns>是否拒绝成功</returns>
        [Route("reject")]
        [HttpPut]
        public ApiResponse<bool> RejectExpense(int expenseId, string reason)
        {
            if (expenseId < 1)
            {
                throw new Exception("无效的请求");
            }
            if(String.IsNullOrEmpty(reason))
            {
                throw new Exception("请输入拒绝理由");
            }
            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = this.ExpenseService.ApproveOrRejectExpense(expenseId, reason, this.Member, false)
            };

            return response;
        }

        /// <summary>
        /// 统计审批人审批(待审批和已审批)报销单
        /// </summary>
        /// <returns>审批(待审批和已审批)报销单</returns>
        [Route("summary")]
        [HttpGet]
        public ApiListResponse<ExpenseAuditSummaryModel> ExpenseSummary()
        {
            ApiListResponse<ExpenseAuditSummaryModel> response = new ApiListResponse<ExpenseAuditSummaryModel>()
            {
                Result = this.ExpenseService.ExpenseSummary(this.Member.Id)
            };

            return response;
        }

        ///<summary>
        ///取消报销单
        ///</summary>
        ///<param name="orderNo"></param>
        ///<returns></returns>
        [Route("{id}/cancel")]
        [HttpPut]
        public ApiResponse<bool> Cancel(int id)
        {
            if (id < 0)
            {
                throw new ApiBadRequestException("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ExpenseService.CancelExpense(id, this.Member)
            };
            return response;
        }

        /// <summary>
        /// 更新报销单(目前只能更新报销理由)
        /// </summary>
        /// <param name="id">报销单号</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}/update")]
        [HttpPut]
        public ApiResponse<bool> UpdateExpenseOrder(int id, UpdateExpenseModel model)
        {
            if (id < 0 || model == null)
            {
                throw new ApiBadRequestException("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ExpenseService.UpdateExpenseOrder(id, model,this.Member)
            };
            return response;
        }

        /// <summary>
        /// 更新报销单详细信息
        /// </summary>
        /// <param name="id">报销详细id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("expensedetails/{id}/update")]
        [HttpPut]
        public ApiResponse<bool> UpdateExpenseDetailsOrder(int id, ApplyExpenseDetailModel model)
        {
            if (id < 0 || model == null)
            {
                throw new ApiBadRequestException("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ExpenseService.UpdateExpenseDetailsOrder(id, model,this.Member)
            };
            return response;
        }

        /// <summary>
        /// 删除报销详细
        /// </summary>
        /// <param name="id">报销详细id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("expensedetails/{id}/delete")]
        [HttpPut]
        public ApiResponse<bool> DeleteExpenseDetailsOrder(int id)
        {
            if (id < 0)
            {
                throw new ApiBadRequestException("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ExpenseService.DeleteExpenseDetailsOrder(id)
            };
            return response;
        }

        /// <summary>
        /// 添加报销单详细信息
        /// </summary>
        /// <param name="id">报销单id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("expensedetails/{id}/add")]
        [HttpPost]
        public ApiResponse<bool> AddExpenseDetailsOrder(int id, ApplyExpenseDetailModel model)
        {
            if (id < 0 || model == null)
            {
                throw new ApiBadRequestException("无效的请求");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = ExpenseService.AddExpenseDetailsOrder(id, model)
            };
            return response;
        }

    }
}
