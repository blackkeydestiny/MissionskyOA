using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;


namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 假单管理
    /// </summary>
    [RoutePrefix("api/orders")]
    public class OrderController : BaseController
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(OrderController));

        private IOvertimeService OvertimeService { get; set; }
        private IAskLeaveService AskLeaveService { get; set; }
        private IOrderService OrderService { get; set; }
        private IUserService UserService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="overtimeService">加班管理服务</param>
        /// <param name="askLeaveService">请假管理服务</param>
        /// <param name="userService">用户管理服务</param>
        public OrderController(IOvertimeService overtimeService, IOrderService orderService, IAskLeaveService askLeaveService, IUserService userService)
        {
            this.OvertimeService = overtimeService;
            this.AskLeaveService = askLeaveService;
            this.OrderService = orderService;
            this.UserService = userService;
        }

        /// <summary>
        /// 获取自己请假申请单列表
        /// </summary>
        /// <returns></returns>
        [Route("AskLeave/List")]
        [HttpGet]
        public ApiPagingListResponse<OrderModel> GetAskLeaveList(int pageIndex = 0, int pageSize = 15)
        {

            // 3. 获取所有请假单信息
            // 每页显示15条记录
            // 这里用var来定义一个list对象
            var query = this.AskLeaveService.MyAskLeaveList(this.Member, pageIndex, pageSize);

            var result = new PaginationModel<OrderModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<OrderModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据请假单具体id,获取单个假单(包括详情列表)
        /// </summary>
        /// <param name="askLeaveDetailID"></param>
        /// <returns></returns>
        [Route("AskLeave")]
        [HttpGet]
        public ApiResponse<OrderDetModel> GetAskLeaveDetail(int askLeaveDetailID)
        {
            // 1. 验证具体请假ID
            if (askLeaveDetailID < 1)
            {
                throw new Exception("BadRequest");
            }

            // 2. 根据具体请假ID获取请假的具体信息
            var askLeaveDetails = this.AskLeaveService.getAskLeaveDetailsByID(askLeaveDetailID);

            // 3. 构造 API Response 信息
            ApiResponse<OrderDetModel> response = new ApiResponse<OrderDetModel>()
            {
                Result = askLeaveDetails
            };

            return response;
        }

        /// <summary>
        /// 获取自己加班单列表
        /// </summary>
        /// <returns></returns>
        [Route("Overtime/List")]
        [HttpGet]
        public ApiPagingListResponse<OrderModel> GetOvertimeList(int pageIndex = 0, int pageSize = 15)
        {

            // 3. 获取所有请假单信息
            var query = this.OvertimeService.MyOvertimeList(this.Member, pageIndex, pageSize);

            var result = new PaginationModel<OrderModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<OrderModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据加班单具体id,获取单个加班单(包括详情列表)
        /// </summary>
        /// <param name="overtimeDetialID"></param>
        /// <returns></returns>
        [Route("Overtime")]
        [HttpGet]
        public ApiResponse<OrderDetModel> GetOvertimeDetail(int overtimeDetialID)
        {
            // 1. 验证具体加班单ID
            if (overtimeDetialID < 1)
            {
                throw new Exception("Invalid Overtime Detail ID");
            }

            // 2. 根据具体加班单ID获取加班的具体信息
            var overtimeDetails = this.OvertimeService.GetOvertimeDetailsByID(overtimeDetialID);

            // 3. 构造 API Response 信息
            ApiResponse<OrderDetModel> response = new ApiResponse<OrderDetModel>()
            {
                Result = overtimeDetails
            };

            return response;
        }

        /// <summary>
        /// 根据UserID查询加班记录
        /// </summary>
        /// <returns></returns>
        [Route("GetOvertimeHistoryByUserID/{userID}")]
        [HttpGet]
        public ApiPagingListResponse<OrderModel> GetOvertimeHistoryByUserID(int userID, int pageIndex = 0, int pageSize = 15)
        {
            // 1. 验证用户ID
            if (userID < 1)
            {
                throw new Exception("Invalid User ID");
            }
            // 2. 获取用户信息
            UserModel user = this.UserService.GetUserDetail(userID);
            
            if(User==null)
            {
                throw new Exception("Didn't find this user");
            }

            // 3. 获取用户加班历史记录
            var query = this.OvertimeService.GetOvertimeHistoryByUserID(user, pageIndex, pageSize);

            var result = new PaginationModel<OrderModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<OrderModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据UserID查询请假记录
        /// </summary>
        /// <returns></returns>
        [Route("GetAskLeaveHistoryByUserID/{userID}")]
        [HttpGet]
        public ApiPagingListResponse<OrderModel> GetAskLeaveHistoryByUserID(int userID, int pageIndex = 0, int pageSize = 15)
        {
            // 1. 验证用户ID
            if (userID < 1)
            {
                throw new Exception("Invalid User ID");
            }
            // 2. 获取用户信息
            UserModel user = this.UserService.GetUserDetail(userID);

            if (User == null)
            {
                throw new Exception("Didn't find this user");
            }

            // 3. 获取用户加班历史记录
            var query = this.AskLeaveService.GetAskLeaveHistoryByUserID(user, pageIndex, pageSize);

            var result = new PaginationModel<OrderModel>();
            result.Result = query.Result;
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<OrderModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 新增请假单或加班单
        /// </summary>
        /// <returns></returns>
        [Route("addOrder")]
        [HttpPost]
        [RequestBodyFilter]
        public ApiResponse<OrderModel> AddOrder(ApplyOrderModel model)
        {
            // 1. 检查输入参数
            if (model == null)
            {
                throw new ApiBadRequestException("无效的参数");
            }

            // 2. 验证申请单是否有效
            /*
             * 验证不需要返回值，不符合要求弹出提示信息
             * 
             * 
             * **/
            ValidApplyOrder(model);

            //新增假单 如果是请假或者提交加班申请需要启动工作申请流程
            // 3. Construct API Response
            var response = new ApiResponse<OrderModel>()
            {
                Result = AskLeaveService.AddOrder(model, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 根据申请单OrderId获取申请单详细信细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/order")]
        [HttpGet]
        public ApiResponse<OrderModel> GetOrderDetail(int id)
        {
            // 1. 验证具体请假ID
            if (id < 1)
            {
                throw new Exception("BadRequest");
            }

            // 2. 根据id,获取单据信息
            var order = this.AskLeaveService.GetOrderDetail(id);

            // 3. 构造 API Response 信息
            ApiResponse<OrderModel> response = new ApiResponse<OrderModel>()
            {
                Result = order
            };

            return response;
        }

        /// <summary>
        /// 根据申请单号OrderNo获取申请单详细信细
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [Route("getOrder/{orderNo}")]
        [HttpGet]
        public ApiResponse<OrderModel> GetOrderDetailByOrderNo(int orderNo)
        {
            // 1. 验证具体请假ID
            if (orderNo < 1)
            {
                throw new Exception("BadRequest");
            }

            // 2. 根据申请单号OrderNo获取申请单详细信细
            var order = this.AskLeaveService.GetOrderDetailByOrderNo(orderNo);

            // 3. 构造 API Response 信息
            ApiResponse<OrderModel> response = new ApiResponse<OrderModel>()
            {
                Result = order
            };

            return response;
        }

        /// <summary>
        /// 更新假单
        /// </summary>
        /// <param name="orderNo">申请单号</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{orderNo}/update")]
        [HttpPut]
        public ApiResponse<bool> UpdateOrder(int orderNo, UpdateOrderModel model)
        {
            if (orderNo < 0 || model == null)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = AskLeaveService.UpdateOrder(this.Member.Id, orderNo, model)
            };

            return response;
        }

        ///<summary>
        ///销请假单或者加班单
        ///</summary>
        ///<param name="orderNo"></param>
        ///<param name="model"></param>
        ///<returns></returns>
        [Route("{orderNo}/revoke")]
        [HttpPut]
        public ApiResponse<bool> Revoke(int orderNo, RevokeOrderModel model)
        {
            //销假 如果是请假或者提交加班申请需要启动工作申请流程
            if (orderNo < 0 || model == null)
            {
                throw new ApiBadRequestException("无效的参数");
            }

            if (!AskLeaveService.IsRevokeTimeAvailiable(orderNo, model))
            {
                throw new ApiBadRequestException("撤销开始时间和撤销结束时间必须在当前申请单开始时间和结束时间内");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = AskLeaveService.RevokeOrder(orderNo, model, this.Member)
            };
            return response;
        }

        ///<summary>
         ///取消请假或者加班单
         ///</summary>
        ///<param name="orderNo"></param>
        ///<returns></returns>
        [Route("{orderNo}/cancel")]
        [HttpPut]
        public ApiResponse<bool> Cancel(int orderNo)
        {
            //取消 如果是请假或者提交加班申请需要启动工作申请流程
            if (orderNo < 0)
            {
                throw new ApiBadRequestException("Bad request");
            }

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = AskLeaveService.CancelOrder(orderNo, this.Member)
            };
            return response;
        }
        
        /// <summary>
        /// 批准
        /// </summary>
        /// <param name="model">申请单</param>
        /// <returns>是否批准成功</returns>
        [Route("approve")]
        [HttpPut]
        public ApiResponse<int> ApproveOrder(OperateOrderModel model)
        {
            if (model == null || model.OrderNo < 1)
            {
                throw new Exception("Invalid order model.");
            }

            model.Operation = WorkflowOperation.Approve;

            ApiResponse<int> response = new ApiResponse<int>()
            {
                Result = this.OrderService.Approve(model, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="model">申请单</param>
        /// <returns>是否拒绝成功</returns>
        [Route("reject")]
        [HttpPut]
        public ApiResponse<bool> RejecteOrder(OperateOrderModel model)
        {
            if (model == null || model.OrderNo < 1)
            {
                throw new Exception("Invalid order model.");
            }

            model.Operation = WorkflowOperation.Rejecte;

            ApiResponse<bool> response = new ApiResponse<bool>()
            {
                Result = this.OrderService.Rejecte(model, this.Member)
            };

            return response;
        }

        /// <summary>
        /// 上传申请单附件
        /// </summary>
        /// <param name="orderNo">申请单Id</param>
        /// <returns>是否上传成功</returns>
        [Route("upload/{orderNo}")]
        [HttpPost]
        public ApiResponse<bool> Upload(int orderNo)
        {
            return AttachmentController.Upload(orderNo, Constant.ATTACHMENT_TYPE_ORDER_ATTACHMENT);
        }

        #region 获取用户审批的申请单
        /// <summary>
        /// 获取待处理的申请单，默认查询所有
        /// </summary>
        /// <returns>审批任务列表</returns>
        [Route("get")]
        [HttpGet]
        public ApiListResponse<OrderModel> GetPendingOrders()
        {
            if (this.Member == null || this.Member.Id < 1)
            {
                throw new Exception("Invalid user.");
            }

            ApiListResponse<OrderModel> response = new ApiListResponse<OrderModel>()
            {
                Result = this.OrderService.GetPendingOrdersByUserId(this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 获取待处理的申请单数目
        /// </summary>
        /// <returns>申请单数目</returns>
        [Route("count")]
        [HttpGet]
        public ApiResponse<PendingOrderCountModel> CountPendingOrders()
        {
            if (this.Member == null || this.Member.Id < 1)
            {
                throw new Exception("Invalid user.");
            }

            ApiResponse<PendingOrderCountModel> response = new ApiResponse<PendingOrderCountModel>()
            {
                Result = this.OrderService.CountPendingOrders(this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 统计审批人审批(待审批和已审批)申请单，默认查询所有
        /// </summary>
        /// <returns>审批(待审批和已审批)申请单</returns>
        [Route("summary")]
        [HttpGet]
        public ApiListResponse<ApproveOrderModel> SummaryApproveOrders()
        {
            ApiListResponse<ApproveOrderModel> response = new ApiListResponse<ApproveOrderModel>()
            {
                Result = this.OrderService.SummaryApproveOrders(this.Member.Id)
            };

            return response;
        }
        #endregion

        /// <summary>
        /// 根据待审批人英文名查询申请单
        /// </summary>
        /// <returns>审批(待审批和已审批)申请单</returns>
        [Route("audit/search")]
        [HttpGet]
        public ApiListResponse<ApproveOrderModel> SearchOrdersByEnglishName(string englishname)
        {
            ApiListResponse<ApproveOrderModel> response = new ApiListResponse<ApproveOrderModel>()
            {
                Result = this.OrderService.SearchApproveOrders(englishname, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 验证申请单是否有效
        /// </summary>
        /// <param name="model">申请单</param>
        private void ValidApplyOrder(ApplyOrderModel model)
        {
            if (model.UserIds.Count() < 0)
            {
                Log.Error("请选择申请人。");
                throw new ApiBadRequestException("请选择申请人。");
            }

            if (DateTime.Compare(model.StartDate, DateTime.Now.AddDays(-15)) < 0)
            {
                throw new ApiBadRequestException("开始必须大于" + string.Format("{0:F}", DateTime.Now.AddDays(-15)));
            }

            if (DateTime.Compare(model.StartDate, DateTime.Now.AddDays(15)) > 0)
            {
                throw new ApiBadRequestException("开始必须小于" + string.Format("{0:F}", DateTime.Now.AddDays(15)));
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new ApiBadRequestException("结束时间必须大于开始时间");
            }

            /*
             * 
             * 
             * **/
            if (model.OrderType != OrderType.Overtime && model.OrderType != 0)
            {
                TimeSpan dspWorkingDayAM = DateTime.Parse("08:30").TimeOfDay;
                TimeSpan dspWorkingDayPM = DateTime.Parse("18:30:59").TimeOfDay;
                if (model.StartDate.TimeOfDay < dspWorkingDayAM)
                {
                    throw new ApiBadRequestException("开始时间必须大于等于上午8点半");
                }
                if (model.EndDate.TimeOfDay > dspWorkingDayPM)
                {
                    throw new ApiBadRequestException("结束时间必须小于等于下午6点半");
                }
            }
            else
            {
                TimeSpan dspWorkingDayAM = DateTime.Parse("08:30").TimeOfDay;
                if (model.StartDate.TimeOfDay < dspWorkingDayAM)
                {
                    throw new ApiBadRequestException("开始时间必须大于上午8点半");
                }
            }

            /*
             * 验证用户申请的时间段是否被占用
             * 
             * **/
            var invalidUsers = AskLeaveService.IsOrderTimeAvailiable(model, 0);


            if (!string.IsNullOrEmpty(invalidUsers))
            {
                throw new ApiBadRequestException(string.Format("用户({0})此申请时段已经占用。", invalidUsers));
            }

        }
    }
}
