using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Common;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using System.Data.Entity.Validation;

namespace MissionskyOA.Services
{
    public class AskLeaveService : ServiceBase, IAskLeaveService
    {
         /*
          * 依赖注入
          * 
          * **/
        private readonly IAttendanceSummaryService _attendanceSummaryService = new AttendanceSummaryService();
        private readonly IWorkflowProcessService _workflowProcessService = new WorkflowProcessService();
        private readonly IWorkflowService _workflowService = new WorkflowService();
        private readonly IOrderService _orderService = new OrderService();
        private readonly IAttachmentService _attachmentService = new AttachmentService();
        private readonly IUserService _userService = new UserService();

        /// <summary>
        /// 显示所有请假单
        /// 返回类型为List
        /// </summary>
        /// <returns>请假单分页信息</returns>
        public IPagedList<OrderModel> MyAskLeaveList(UserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                // 创建OrderModel列表对象
                var result = new List<OrderModel>();

                // 从数据库中获取符合条件的order model
                var orders =
                    dbContext.Orders.Where(
                        it =>
                            ((it.UserId == model.Id || it.ApplyUserId == model.Id) && //查询用户申请的或者用户自己的
                             it.OrderType != (int) OrderType.Overtime)
                        ).OrderByDescending(item => item.CreatedTime);
                // 
                orders.ToList().ForEach(item =>
                {
                    var order = item.ToModel();
                    if (result.Any(it => it.OrderNo == order.OrderNo) == false) //过滤批量申请，重复的申请单
                    {
                        // 
                        OrderService.DoFill(dbContext, order);
                        result.Add(order);
                    }
                });

                return new PagedList<OrderModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 显示具体请假信息
        /// </summary>
        /// <param name="orderDetailId">申请详细详细Id</param>
        /// <returns>具体请假信息</returns>
        public OrderDetModel getAskLeaveDetailsByID(int orderDetailId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var askLeaveDetails = dbContext.OrderDets.FirstOrDefault(item => item.Id == orderDetailId);
                if (askLeaveDetails == null)
                {
                    throw new Exception("Invalid Id");
                }
                return askLeaveDetails.ToModel();
            }
        }

        /// <summary>
        /// 根据userid,显示请假历史记录
        /// </summary>
        /// <returns>请假单分页信息</returns>
        public IPagedList<OrderModel> GetAskLeaveHistoryByUserID(UserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<OrderModel>();

                var askLeaveList =
                    dbContext.Orders.Where(
                        it =>
                            ((it.UserId == model.Id || it.ApplyUserId == model.Id) && //查询用户申请的或者用户自己的
                             it.OrderType != (int) OrderType.Overtime && it.Status == (int) OrderStatus.Approved)
                        ).OrderByDescending(item => item.CreatedTime);


                askLeaveList.ToList().ForEach(item =>
                {
                    var order = item.ToModel();
                    if (result.Any(it => it.OrderNo == order.OrderNo) == false) //过滤批量申请，重复的申请单
                    {
                        OrderService.DoFill(dbContext, order);
                        result.Add(order);
                    }
                });

                return new PagedList<OrderModel>(result, pageIndex, pageSize);
            }
        }

        #region 请假加班单申请
        /// <summary>
        /// 添加请假信息
        /// 
        /// 处理请假申请单逻辑
        /// 
        /// </summary>
        /// <param name="model">申请单详细: 对应APP中的申请单详细</param>
        /// <param name="applicant">流程申请人Id</param>
        /// <returns>请假单信息</returns>
        public OrderModel AddOrder(ApplyOrderModel model, UserModel applicant)
        {
            /*
             * 
             * 
             * 
             * **/

            #region 申请单详细

            // 1、新建申请单变量
            var order = new OrderModel()
            {
                OrderType = model.OrderType,
                Status = OrderStatus.Apply,
                ApplyUserId = applicant.Id,
                IsBatchApply = (model.UserIds.Count() > 1),
                CreatedTime = DateTime.Now,
            };

            // 2、新建申请单详情
            var orderDet = new OrderDetModel()
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                StartTime = model.StartDate.TimeOfDay,
                EndTime = model.EndDate.TimeOfDay,
                Description = model.Description,
                InformLeader = model.InformLeader ?? false,
                WorkTransfer = model.WorkTransfer ?? false,
                Recipient = model.Recipient ?? 0,
                IOHours = model.IOHours
            };

            // 3、将申请单详情变量 放入到 申请单变量中
            order.OrderDets = new List<OrderDetModel>() {orderDet};
            #endregion

            // 4、逻辑处理
            using (var dbContext = new MissionskyOAEntities())
            {
                // 1、生成申请单号 、申请单用户
                // OrderNo --> int
                order.OrderNo = _orderService.GenerateOrderNo(dbContext); //生成申请单号
                /*
                * OrderUsers -->  IList<OrderUserModel>
                * model.UserIds --> 定义为 int[]， 表示可以或者可能有多个申请用户
                * 
                * **/
                order.OrderUsers = _userService.GetOrderUsers(dbContext, model.UserIds);


                // 2、验证申请单是否有效，并转换IOHours正负值
                ValidOrder(order, WorkflowOperation.Apply);


                // 3、添加初始审批信息
                AddAuditMessages(dbContext, order, applicant);

                // 4、添加申请单到数据库
                var dbOrders = AddOrderToDb(dbContext, order, WorkflowOperation.Apply);

                // 5、流程申请
                WorkflowProcessModel process = _orderService.Apply(dbContext, order); 
                if (process == null)
                {
                    Log.Error("找不到流程。");
                    throw new InvalidOperationException("找不到流程。");
                }

                // 6、更新申请单工作流
                UpdateOrderProcess(dbContext, dbOrders, order, process, applicant);

                // 7、发送工作交接通知
                if (model.WorkTransfer.HasValue && model.WorkTransfer.Value && model.Recipient.HasValue && model.OrderType != OrderType.Overtime)
                {
                    _orderService.AddNotification(dbContext, order);
                }

                // 8、更新申请单其它信息
                dbContext.SaveChanges(); 

                // 9、
                return order;
            }
        }

        /// <summary>
        /// 销请假单或者加班单
        /// </summary>
        /// <returns>取消</returns>
        public bool RevokeOrder(int orderNo, RevokeOrderModel model, UserModel applicant)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                #region 申请单信息

                var revokeOrder = new OrderModel()
                {
                    Status = OrderStatus.Apply,
                    ApplyUserId = applicant.Id,
                    IsBatchApply = (model.UserIds.Count() > 1),
                    CreatedTime = DateTime.Now
                };

                var orderDet = new OrderDetModel()
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    StartTime = model.StartDate.TimeOfDay,
                    EndTime = model.EndDate.TimeOfDay,
                    IOHours = model.IOHours,
                    Description = model.RevokeReason,
                    InformLeader = model.InformLeader,
                    WorkTransfer = model.WorkTransfer,
                    Recipient = model.Recipient
                };

                revokeOrder.OrderNo = _orderService.GenerateOrderNo(dbContext); //生成申请单号
                revokeOrder.OrderUsers = _userService.GetOrderUsers(dbContext, model.UserIds); //相关申请单用户
                revokeOrder.OrderDets = new List<OrderDetModel>() {orderDet};

                //验证申请单
                Order oldEntity = null;
                revokeOrder.OrderUsers.ToList().ForEach(ou =>
                {
                    //申请单是否存在
                    oldEntity = dbContext.Orders.FirstOrDefault(o => o.UserId == ou.Id && o.OrderNo == orderNo);
                    if (oldEntity == null)
                    {
                        Log.Error("此订单不存在。");
                        throw new KeyNotFoundException("此订单不存在。");
                    }

                    ou.OrderId = oldEntity.Id; //撤销单Id

                    //是否已经撤销过
                    var revokedEntity =
                        dbContext.Orders.FirstOrDefault(o => o.RefOrderId.HasValue && o.RefOrderId == ou.OrderId);
                    if (revokedEntity != null && !revokedEntity.ToModel().CanRevokeOrder())
                    {
                        Log.Error("已经撤销一次，不能再撤销。");
                        throw new InvalidOperationException("已经撤销一次，不能再撤销。");
                    }

                });

                revokeOrder.OrderType = (OrderType) oldEntity.OrderType; //申请单类型

                var oldDetailEntity = dbContext.OrderDets.FirstOrDefault(it => it.OrderId == oldEntity.Id);
                if (oldDetailEntity != null && oldDetailEntity.IOHours.HasValue)
                {
                    if (Math.Abs(oldDetailEntity.IOHours.Value) < Math.Abs(model.IOHours))
                    {
                        Log.Error("撤销时长不正确。");
                        throw new InvalidOperationException("撤销时长不正确。");
                    }
                }

                //验证申请单是否有效，并转换IOHours正负值
                ValidOrder(revokeOrder, WorkflowOperation.Revoke);

                #endregion

                //添加初始审批信息
                AddAuditMessages(dbContext, revokeOrder, applicant);

                //对于同意的请假单或者加班单,自动填写新的撤销请假单
                if (oldEntity.Status == (int) OrderStatus.Approved)
                {
                    //添加申请单到数据库
                    var dbOrders = AddOrderToDb(dbContext, revokeOrder, WorkflowOperation.Revoke);

                    //流程申请
                    WorkflowProcessModel process = _orderService.Apply(dbContext, revokeOrder);
                    if (process == null)
                    {
                        Log.Error("找不到流程。");
                        throw new InvalidOperationException("找不到流程。");
                    }

                    //更新申请单工作流
                    UpdateOrderProcess(dbContext, dbOrders, revokeOrder, process, applicant);

                    try
                    {
                        dbContext.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        LogDatabaseError(dbEx);
                        throw dbEx;
                    }

                    return true;
                }
                else
                {
                    Log.Error("流程处理异常。");
                    throw new InvalidOperationException("流程处理异常。");
                }
            }
        }

        /// <summary>
        /// 更新申请单工作流
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="dbOrders"></param>
        /// <param name="order"></param>
        /// <param name="process"></param>
        /// <param name="currentUser"></param>
        private void UpdateOrderProcess(MissionskyOAEntities dbContext, IEnumerable<Order> dbOrders, OrderModel order,
            WorkflowProcessModel process, UserModel currentUser)
        {
            //流程结束 && 批量申请，向流程申请人发推送消息
            if (process.IsCompletedApproved() && order.IsBatchApply)
            {
                this._orderService.AddNotification(dbContext, order, currentUser.Id, process); //默认发送到申请人
            }


            dbOrders.ToList().ForEach(entity =>
            {
                entity.WorkflowId = process.FlowId;
                entity.Status = (int) process.NextStatus;

                if (process.IsCompletedApproved()) //申请通过 || 过[行政审阅]到[财务审阅]
                {
                    this._orderService.AddNotification(dbContext, order, currentUser.Id, process, entity.UserId);
                }
                else //需要下一步审批
                {
                    if (process.NextApprover == null)
                    {
                        Log.Error("流程下一步审批人异常。");
                        throw new InvalidOperationException("流程下一步审批人异常。");
                    }

                    entity.NextStep = process.NextStep.Id;
                    entity.NextAudit = process.NextApprover.Id;

                    this._orderService.AddNotification(dbContext, order, currentUser.Id, process,
                        process.NextApprover.Id);
                }
            });
        }

        /// <summary>
        /// 添加申请单到数据库
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private IEnumerable<Order> AddOrderToDb(MissionskyOAEntities dbContext, OrderModel order, WorkflowOperation operation)
        {
            /*
             * 1、判断操作是否为 撤销和申请
             * 
             * **/
            if (operation != WorkflowOperation.Revoke && operation != WorkflowOperation.Apply)
            {
                Log.Error("无效的操作。");
                throw new InvalidOperationException("无效的操作。");
            }

            //
            IList<Order> dbOrders = new List<Order>();

            //假ID, 解决EF批量插入Entity时，报"Multiple added entities may have the same primary key"异常
            //http://www.cnblogs.com/joeylee/p/3805901.html
            var idFeed = 0;

            order.OrderUsers.ToList().ForEach(user =>
            {
                #region 添加申请单

                var orderEntity = order.ToEntity();
                orderEntity.Id = idFeed ++;
                orderEntity.UserId = user.Id;

                //撤销，记录原始申请Id
                if (operation == WorkflowOperation.Revoke)
                {
                    orderEntity.RefOrderId = user.OrderId;
                }

                dbContext.Orders.Add(orderEntity); //添加数据库记录
                order.Id = orderEntity.Id;

                //user.OrderId = orderEntity.Id; //申请单Id

                if (order.OrderDets != null && order.OrderDets.Count > 0)
                {
                    foreach (var orderDetail in order.OrderDets)
                    {
                        var orderDetailsEntity = orderDetail.ToEntity();
                        orderDetailsEntity.Id = idFeed;
                        orderDetailsEntity.OrderId = orderEntity.Id;
                        dbContext.OrderDets.Add(orderDetailsEntity); //添加数据库记录
                        orderDetail.Id = orderDetailsEntity.Id;
                    }
                }

                dbOrders.Add(orderEntity); //申请单

                #endregion
            });

            return dbOrders;
        }

        /// <summary>
        /// 添加初始审批信息
        /// </summary>
        private void AddAuditMessages(MissionskyOAEntities dbContext, OrderModel order, UserModel currentUser)
        {
            //添加初始审批信息，请假为Apply_Leave_Application_Message，加班为Apply_OT_Application_Message
            var auditMessageType = (int) AuditMessageType.Apply_Leave_Application_Message;

            if (order.OrderType == OrderType.Overtime)
            {
                auditMessageType = (int) AuditMessageType.Apply_OT_Application_Message;
            }


            var detail = order.OrderDets.FirstOrDefault();
            

            var auditMessageEntitry = new AuditMessage()
            {
                Type = auditMessageType,
                Message = detail != null && !string.IsNullOrEmpty(detail.Description) ? detail.Description : string.Empty,
                UserId = currentUser.Id,
                Status = (int) AuditMessageStatus.Unread,
                CreatedTime = DateTime.Now
            };

            // 将审核消息保存到表[MissionskyOA_DEV].[dbo].[AuditMessage]
            dbContext.AuditMessages.Add(auditMessageEntitry);
        }
        #endregion

        /// <summary>
        /// 修改申请信息
        /// </summary>
        /// <returns>修改状态</returns>
        public bool UpdateOrder(int userId, int orderNo, UpdateOrderModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                #region 申请信息

                //查询申请信息是否存在
                var orderEntities = dbContext.Orders.Where(o => o.OrderNo == orderNo);
                var orderUsers = orderEntities.Select(o => o.UserId).ToArray();
                var validateEntity = orderEntities.FirstOrDefault();

                if (validateEntity == null)
                {
                    Log.Error("申请记录不存在。");
                    throw new KeyNotFoundException("申请记录不存在。");
                }

                var validateOrder = validateEntity.ToModel();

                //model转换
                var applyModel = new ApplyOrderModel()
                {
                    OrderType = validateOrder.OrderType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IOHours = model.IOHours,
                    UserIds = orderUsers,
                    InformLeader = model.InformLeader,
                    WorkTransfer = model.WorkTransfer,
                    Recipient = model.Recipient
                };

                //var orderDet = new OrderDet()
                //{
                //    StartDate = model.StartDate,
                //    EndDate = model.EndDate,
                //    StartTime = model.StartDate.TimeOfDay,
                //    EndTime = model.EndDate.TimeOfDay,
                //    Description = model.Description,
                //    InformLeader = model.InformLeader ?? false,
                //    WorkTransfer = model.WorkTransfer ?? false,
                //    Recipient = model.Recipient ?? 0,
                //    IOHours = model.IOHours
                //};

                //validateEntity.OrderDets = new List<OrderDet>() { orderDet };

                //下一步审批人
                var opprover =
                    dbContext.Users.FirstOrDefault(
                        it => validateEntity.NextAudit.HasValue && it.Id == validateEntity.NextAudit.Value);

                #endregion

                #region 验证

                var validateMsg = string.Empty;

                if (userId != validateOrder.ApplyUserId && (opprover != null && userId != opprover.Id))
                {
                    Log.Error("用户不能修改申请单。");
                    throw new InvalidOperationException("用户不能修改申请单。");
                }

                if (DateTime.Compare(model.StartDate, DateTime.Now.AddDays(-15)) < 0)
                {
                    validateMsg = "开始必须大于" + string.Format("{0:F}", DateTime.Now.AddDays(-15));
                    Log.Error(validateMsg);
                    throw new InvalidOperationException(validateMsg);
                }

                if (DateTime.Compare(model.StartDate, DateTime.Now.AddDays(15)) > 0)
                {
                    validateMsg = "开始必须小于" + string.Format("{0:F}", DateTime.Now.AddDays(15));
                    Log.Error(validateMsg);
                    throw new InvalidOperationException(validateMsg);
                }

                if (model.StartDate >= model.EndDate)
                {
                    validateMsg = "结束时间必须大于开始时间";
                    Log.Error(validateMsg);
                    throw new InvalidOperationException(validateMsg);
                }

                if (validateOrder.OrderType != OrderType.Overtime && validateOrder.OrderType != 0)
                {
                    TimeSpan dspWorkingDayAm = DateTime.Parse("08:30").TimeOfDay;
                    TimeSpan dspWorkingDayPm = DateTime.Parse("18:30:59").TimeOfDay;
                    if (model.StartDate.TimeOfDay < dspWorkingDayAm)
                    {
                        throw new InvalidOperationException("开始时间必须大于等于上午8点半");
                    }
                    if (model.EndDate.TimeOfDay > dspWorkingDayPm)
                    {
                        throw new InvalidOperationException("结束时间必须小于等于下午6点半");
                    }
                }
                else
                {
                    TimeSpan dspWorkingDayAm = DateTime.Parse("08:30").TimeOfDay;
                    if (model.StartDate.TimeOfDay < dspWorkingDayAm)
                    {
                        throw new InvalidOperationException("开始时间必须大于上午8点半");
                    }
                }

                var invalidUsers = IsOrderTimeAvailiable(applyModel, orderNo);
                if (!string.IsNullOrEmpty(invalidUsers))
                {
                    throw new InvalidOperationException(string.Format("用户({0})此申请时段已经占用。", invalidUsers));
                }

                #endregion

                #region 更新信息

                var existedUser = new List<int>(); //申请单原本已经有的用户
                if ((int) OrderStatus.Apply == validateEntity.Status ||
                    (opprover != null && opprover.ToModel().IsAdminStaff))
                {
                    #region 更新已存在的用户申请
                    orderEntities.ToList().ForEach(o =>
                    {
                        if (model.UserIds.Contains(o.UserId)) //更新用户申请单
                        {
                            OrderDet detailEntity = null;
                            if (o.OrderDets == null || o.OrderDets.Any() == false)
                            {
                                detailEntity = dbContext.OrderDets.FirstOrDefault(od => od.OrderId == o.Id);
                            }
                            else
                            {
                                detailEntity = o.OrderDets.FirstOrDefault();
                            }

                            //o.OrderType = (int)model.OrderType; 不能更新申请单类型

                            if (detailEntity != null)
                            {
                                detailEntity.StartDate = model.StartDate.Date;
                                detailEntity.EndDate = model.EndDate.Date;
                                detailEntity.IOHours = model.IOHours;
                                detailEntity.StartTime = model.StartDate.TimeOfDay;
                                detailEntity.EndTime = model.EndDate.TimeOfDay;
                                detailEntity.Description = model.Description;
                                detailEntity.InformLeader = model.InformLeader ?? detailEntity.InformLeader;
                                detailEntity.WorkTransfer = model.WorkTransfer ?? detailEntity.WorkTransfer;
                                detailEntity.Recipient = model.Recipient ?? detailEntity.Recipient;
                            }

                            existedUser.Add(o.UserId); //申请单原本已经有的用户
                        }
                        else //移除用户申请单
                        {
                            o.OrderDets.ToList().ForEach(d => dbContext.OrderDets.Remove(d)); //移出申请单详细
                            dbContext.Orders.Remove(o); //移出申请单
                            //o.Status = (int) OrderStatus.Canceled;
                            //o.NextAudit = 0;
                            //o.NextStep = 0;
                        }
                    });
                    #endregion

                    #region 添加新增用户申请
                    model.UserIds.ToList().ForEach(it =>
                    {
                        //validateOrder
                        if (!existedUser.Contains(it))
                        {
                            var newOrder = validateEntity.Copyto();
                            newOrder.UserId = it;

                            //如果是修改撤销单
                            if (validateEntity.RefOrderId.HasValue)
                            {
                                var refOrder = GetOrderDetail(dbContext, validateEntity.RefOrderId.Value, false);

                                if (refOrder != null)
                                {
                                    newOrder.RefOrderId = _orderService.GetOrderIdByOrderNoAndUserId(dbContext,
                                        refOrder.OrderNo, it);
                                }
                            }

                            dbContext.Orders.Add(newOrder);
                        }
                    });
                    #endregion

                    //发送工作交接通知
                    if (model.WorkTransfer.HasValue && model.WorkTransfer.Value && model.Recipient.HasValue && validateEntity.OrderType != (int)OrderType.Overtime)
                    {
                        _orderService.AddNotification(dbContext, validateEntity.ToModel());
                    }

                    dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    Log.Error("已处于审批流程，不能更新。");
                    throw new InvalidOperationException("已处于审批流程，不能更新");
                }

                #endregion
            }
        }

        /// <summary>
        /// 取消请假或者加班单
        /// </summary>
        /// <returns>取消</returns>
        public bool CancelOrder(int orderNo, UserModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var orders = dbContext.Orders.Where(it => it.OrderNo == orderNo);
                var auditMessageType = (int)AuditMessageType.Apply_Leave_Cancel_Application_Message;

                orders.ToList().ForEach(order =>
                {
                    var detail = dbContext.OrderDets.FirstOrDefault(item => item.OrderId == order.Id);

                    if (detail == null)
                    {
                        Log.Error("找不到订单详细。");
                        throw new KeyNotFoundException("找不到订单详细。");
                    }

                    //审阅信息类型
                    if (order.OrderType == (int)OrderType.Overtime)
                    {
                        auditMessageType = (int)AuditMessageType.Apply_OT_Cancel_Application_Message;
                    }

                    //对于未批准的订单直接取消
                    if (order.Status != (int)OrderStatus.Approved)
                    {
                        order.Status = (int)OrderStatus.Canceled;
                        order.NextAudit = null;
                        order.NextStep = null;
                        order.RefOrderId = null;
                    }
                    else
                    {
                        Log.Error("已经在审批中，不能直接取消");
                        throw new InvalidOperationException("已经在审批中，不能直接取消");
                    }
                    
                    //更新假期
                    //RecoverOrder(order.ToModel());
                });

                //添加请假撤销信息，请假为Apply_Leave_Cancel_Application_Message，加班为Apply_OT_Cancel_Application_Message
                var auditMessageEntitry = new AuditMessage()
                {
                    Type = auditMessageType,
                    UserId = model.Id,
                    Status = (int)AuditMessageStatus.Unread,
                    Message = auditMessageType.ToString(),
                    CreatedTime = DateTime.Now
                };

                dbContext.AuditMessages.Add(auditMessageEntitry);

                //添加流程处理记录
                var operate = new OperateOrderModel()
                {
                    Operation = WorkflowOperation.Cancel,
                    OrderNo = orderNo,
                    Opinion = string.Format("{0}取消了申请单。", model.EnglishName)
                };

                _workflowProcessService.AddWorkflowProcess(dbContext, model, operate);

                //更新数据库
                dbContext.SaveChanges(); 

                return true;
            }
        }

        /// <summary>
        /// 显示所有请假记录
        /// </summary>
        /// <returns>请假单信息</returns>
        public ListResult<OrderModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Orders.Where(it => ((it.OrderType != (int) OrderType.Overtime))).AsEnumerable();

                if (filter != null && filter.Member == "UserName")
                {
                    var userList = dbContext.Users.Where(it => it.EnglishName == filter.Value);
                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            userList = dbContext.Users.Where(it => it.EnglishName == filter.Value);
                            break;
                        case "IsNotEqualTo":
                            userList = dbContext.Users.Where(it => it.EnglishName != filter.Value);
                            break;
                        case "StartsWith":
                            userList =
                                dbContext.Users.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().StartsWith(filter.Value.ToLower()));
                            break;
                        case "Contains":
                            userList =
                                dbContext.Users.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "DoesNotContain":
                            userList =
                                dbContext.Users.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        !it.EnglishName.ToLower().Contains(filter.Value.ToLower()));
                            break;
                        case "EndsWith":
                            userList =
                                dbContext.Users.Where(
                                    it =>
                                        it.EnglishName != null &&
                                        it.EnglishName.ToLower().EndsWith(filter.Value.ToLower()));
                            break;
                    }
                    List<int> userIds = new List<int>();
                    userList.ToList().ForEach(item =>
                    {
                        userIds.Add(item.Id);
                    });
                    list = list.Where(it => userIds.Contains(it.UserId));
                }
                if (filter != null && filter.Member == "CreatedTime")
                {
                    var dtFormat = new System.Globalization.DateTimeFormatInfo() {ShortDatePattern = "yyyy-MM-dd"};
                    var dt = Convert.ToDateTime(filter.Value, dtFormat);

                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date == dt);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date != dt);
                            break;
                        case "IsGreaterThan":
                            list = list.Where(it => it.CreatedTime.Value.Date > dt);
                            break;
                        case "IsLessThan":
                            list = list.Where(it => it.CreatedTime.Value.Date < dt);
                            break;
                        case "IsGreaterThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date >= dt);
                            break;
                        case "IsLessThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.Value.Date >= dt);
                            break;
                    }
                }
                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedTime":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.CreatedTime);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.CreatedTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();

                var result = new ListResult<OrderModel>() {Data = new List<OrderModel>()};
                list.ToList().ForEach(item =>
                {
                    var order = item.ToModel();

                    if (result.Data.Any(it => it.OrderNo == order.OrderNo) == false) //过滤批量申请，重复的申请单
                    {
                        OrderService.DoFill(dbContext, order);
                        result.Data.Add(order);
                    }
                });

                result.Total = count;

                return result;
            }

        }

        /// <summary>
        /// 根据请假单id显示所有请假单详细
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>具体加班信息</returns>
        public ListResult<OrderDetModel> GetAskLeaveDetailsByOrderID(int orderId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var askLeaveDetails = dbContext.OrderDets.Where(item => item.OrderId == orderId);

                var result = new ListResult<OrderDetModel>() {Data = new List<OrderDetModel>()};
                askLeaveDetails.ToList().ForEach(item => result.Data.Add(item.ToModel()));
                result.Total = askLeaveDetails.Count();
                return result;
            }
        }

        /// <summary>
        /// 获取申请单详细信息
        /// </summary>
        /// <param name="id">申请单Id</param>
        /// <returns></returns>
        public OrderModel GetOrderDetail(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                return GetOrderDetail(dbContext, id);
            }
        }

        /// <summary>
        /// 获取申请单详细信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="id">申请单Id</param>
        /// <param name="hasDetail"></param>
        /// <returns></returns>
        public OrderModel GetOrderDetail(MissionskyOAEntities dbContext, int id, bool hasDetail = true)
        {
            var order = dbContext.Orders.FirstOrDefault(item => item.Id == id);

            if (order == null)
            {
                Log.Error("找不到申请单。");
                throw new KeyNotFoundException("找不到申请单。");
            }

            var model = order.ToModel();

            if (hasDetail)
            {
                OrderService.DoFill(dbContext, model, true);
            }

            return model;
        }

        /// <summary>
        /// 根据申请单号OrderNo获取申请单详细信细
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public OrderModel GetOrderDetailByOrderNo(int orderNo)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var order = dbContext.Orders.FirstOrDefault(item => item.OrderNo == orderNo);

                if (order == null)
                {
                    Log.Error("找不到申请单。");
                    throw new KeyNotFoundException("找不到申请单。");
                }

                var model = order.ToModel();
                OrderService.DoFill(dbContext, model, true);

                return model;
            }
        }

        /// <summary>
        /// 验证申请单是否有效(转换IOHours正负值)
        /// </summary>
        /// <param name="order">申请单</param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private bool ValidOrder(OrderModel order, WorkflowOperation operation)
        {
            #region 验证参数

            /*
             * 1、判断申请单是否有效
             * 
             * **/

            if (order == null || order.OrderDets == null || order.OrderDets.Count < 1)
            {
                throw new NullReferenceException("无效的申请单。");
            }

            var detail = order.OrderDets.FirstOrDefault(); //申请单详情

            /*
             * 2、验证申请单详情是否有效
             * 
             * **/
            if (detail == null || !detail.IOHours.HasValue)
            {
                throw new NullReferenceException("无效的申请单。");
            }

            #endregion

            #region 转换正负值

            //撤销申请
            /*
             * 
             * 撤销：
             *          加班申请单： 因为撤销申请单，所以撤销之前的调休假的基础上，相应减少调休假： 意思是我撤销了加班单，所以等于没有加班，那么申请的加班时长减少
             *      其他类型申请单： 因为撤销申请单，其他类型因为是假期，所以撤销了假期类型的申请单，所以的增加假期
             *      
             * 正常申请：
             * 
             * 
             * **/
            if (operation == WorkflowOperation.Revoke) 
            {
                detail.IOHours = order.OrderType == OrderType.Overtime
                    ? Math.Abs(detail.IOHours.Value)*-1.0 //加班，减少调休假
                    : Math.Abs(detail.IOHours.Value); //请假，增加假期
            }
            else //正常申请
            {
                detail.IOHours = order.OrderType == OrderType.Overtime
                    ? Math.Abs(detail.IOHours.Value) //加班，增加调休假
                    : Math.Abs(detail.IOHours.Value)*-1.0; //请假，减少假期
            }

            #endregion

            #region 验证开始/结束时间
            /*
             * 验证申请的时间是否有效
             * 
             * **/
            try
            {
                if (detail.StartDate.CompareTo(detail.EndDate) >= 0) //开始时间等于大于结束时间
                {
                    throw new InvalidOperationException("开始时间必须大于结束时间。");
                }

                if (detail.StartDate.Year != detail.EndDate.Year) //开始时间和结束时间不能跨年
                {
                    throw new InvalidOperationException("开始时间和结束时间不能跨年。");
                }
            }
            catch
            {
                throw;
            }

            #endregion

            #region 验证用户性别, 性别不同，能申请假期类型不同

            var invalidUsers = string.Empty;
            order.OrderUsers.ToList().ForEach(user =>
            {
                //女，没有陪产假
                if (user.Gender == Gender.Female && order.OrderType == OrderType.PaternityLeave)
                {
                    invalidUsers += string.Format(",用户({0})不能申请陪产假", user.UserName);
                }

                //男，没有哺乳假和产假
                if (user.Gender == Gender.Male &&
                    (order.OrderType == OrderType.MaternityLeave || order.OrderType == OrderType.BreastfeedingLeave))
                {
                    invalidUsers += string.Format(",用户({0})不能申请产假或哺乳假", user.UserName);
                }
            });

            if (!string.IsNullOrEmpty(invalidUsers))
            {
                Log.Error(invalidUsers.Substring(1));
                throw new InvalidOperationException(invalidUsers.Substring(1));
            }

            #endregion

            //验证余额是否充足
            _attendanceSummaryService.CheckVacationBalance(order.OrderUsers, order.OrderType, detail.IOHours.Value);
            
            return true;
        }

        /// <summary>
        /// 是否时间段重复申请
        /// 
        /// 验证用户申请的时间段是否被占用
        /// 
        /// </summary>
        /// <returns>是否时间段重复申请</returns>
        public string IsOrderTimeAvailiable(ApplyOrderModel model, int updateOrderNo)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var startDate = model.StartDate;
                var endDate = model.EndDate;
                var invalidUsers = new List<int>(); //无效的用户

                // RefOrderId 为 审批人Id
                // 根据申请单号获取审批人Id
                var refOrderIds = dbContext.Orders.Where(it => it.OrderNo == updateOrderNo).Select(it => it.RefOrderId);

                model.UserIds.ToList().ForEach(userId =>
                {
                    //
                    var isTimeAvailabel = true;

                    /*
                     * 获取当前用户是否有正在审批或者正在执行的申请单
                     *      1、当前申请用户
                     *      2、不是现在正在申请的，也不是修改的
                     *      3、不是取消状态的
                     *      4、不是拒绝状态的
                     *      5、不是撤销状态的
                     * 
                     * **/
                    var entityOrder =
                        dbContext.Orders.Where(
                            it =>
                                it.UserId == userId && //查询用户的申请记录 
                                it.OrderNo != updateOrderNo && //不是当前待添加或修改的申请记录
                                it.Status != (int) OrderStatus.Canceled && //过滤 取消状态的申请记录
                                it.Status != (int) OrderStatus.Rejected && //过滤 拒绝状态的申请记录
                                it.Status != (int) OrderStatus.Revoked); //过滤 撤销状态的申请记录

                    var orderList = entityOrder.ToList();

                    foreach (Order item in orderList)
                    {
                        if (refOrderIds.Contains(item.Id)) //过滤撤销单
                        {
                            continue;
                        }

                        /*
                         * 
                         * 
                         * **/

                        //startdate>currentEndate or endate<currentStartdate
                        var avaliableStartDateTimeExist =
                            dbContext.OrderDets.FirstOrDefault(
                                it =>
                                    it.OrderId == item.Id &&
                                    (it.StartDate > endDate.Date || it.EndDate < startDate.Date) ||
                                    (it.StartDate == startDate.Date && it.EndDate == endDate.Date));

                        if (avaliableStartDateTimeExist == null)
                        {
                            isTimeAvailabel = false;
                            break;
                        }

                        var avaliableDateInTheDbdate =
                            dbContext.OrderDets.FirstOrDefault(
                                it =>
                                    it.OrderId == item.Id && (it.StartDate < startDate.Date) &&
                                    it.EndDate > endDate.Date);
                        if (avaliableDateInTheDbdate != null)
                        {
                            isTimeAvailabel = false;
                            break;
                        }

                        var entityStarTimeExist =
                            dbContext.OrderDets.FirstOrDefault(
                                it =>
                                    it.OrderId == item.Id && it.StartDate == startDate.Date &&
                                    it.EndDate == endDate.Date && startDate.TimeOfDay >= it.StartTime &&
                                    startDate.TimeOfDay < it.EndTime);
                        var entityEndTimeExist =
                            dbContext.OrderDets.FirstOrDefault(
                                it =>
                                    it.OrderId == item.Id && it.StartDate == startDate.Date &&
                                    it.EndDate == endDate.Date && endDate.TimeOfDay > it.StartTime &&
                                    endDate.TimeOfDay <= it.EndTime);
                        var DBTimeBetweenCurrentApplyTime =
                            dbContext.OrderDets.FirstOrDefault(
                                it =>
                                it.OrderId == item.Id && it.StartDate == startDate.Date &&
                                it.EndDate == endDate.Date && startDate.TimeOfDay <= it.StartTime &&
                                endDate.TimeOfDay >= it.EndTime);
                        if ((entityStarTimeExist != null) || (entityEndTimeExist != null) || (DBTimeBetweenCurrentApplyTime!=null))
                        {
                            isTimeAvailabel = false;
                            break;
                        }
                    }

                    if (!isTimeAvailabel)
                    {
                        invalidUsers.Add(userId);
                    }
                });

                /*
                 * 如果申请的时间段有被占用的话，那么返回占用时间段的申请人英文名
                 * 
                 * **/
                return (invalidUsers.Count > 0
                    ? _userService.GetUsersName(dbContext, invalidUsers.ToArray())
                    : string.Empty);
            }
        }

        /// <summary>
        /// 是否撤销的时间的在当前请假时间内
        /// </summary>
        /// <returns>是否撤销的时间的在当前请假时间内</returns>
        public bool IsRevokeTimeAvailiable(int orderNo, RevokeOrderModel model)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var orderEntity = dbcontext.Orders.FirstOrDefault(it => it.OrderNo == orderNo);
                if (orderEntity == null)
                {
                    Log.Error(string.Format("无效的撤销申请单, No: {0}", orderNo));
                    throw new NullReferenceException("无效的撤销申请单。");
                }

                var detailEntity = dbcontext.OrderDets.FirstOrDefault(it => it.OrderId == orderEntity.Id);
                OrderDetModel detail = detailEntity.ToModel();

                return (model.StartDate >= detail.StartDate && model.EndDate <= detail.EndDate);
            }
        }

        /// <summary>
        /// 当撤销的假期被奇效或者拒绝后，恢复年假（还有调休）.
        /// </summary>
        /// <returns>是否恢复成功</returns>
        public bool RecoverOrder(OrderModel order)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //如果有reforder，说明是撤销的假单.所以对于这种取消的时候应该恢复假期.
                if (order.RefOrder == null)
                {
                    return false;
                }

                var detail = order.OrderDets.FirstOrDefault(); //申请单详细
                var times = detail.IOHours.Value + order.RefOrder.OrderDets.FirstOrDefault().IOHours.Value;

                //update
                order.OrderUsers.ToList().ForEach(user =>
                {
                    var summary = new UpdateAttendanceSummaryModel()
                    {
                        UserId = user.Id,
                        SummaryYear = DateTime.Now.Year,
                        Times = times,
                        Type = order.OrderType
                    };

                    double beforeUpdated = 0.0;
                    double afterUpdated = 0.0;
                    string auditMsg = string.Empty;

                    this._attendanceSummaryService.UpdateAttendanceSummary(dbContext, summary, ref beforeUpdated,
                        ref afterUpdated); //更新假期

                    #region 更新剩余结果

                    if (order.OrderType == OrderType.AnnualLeave)
                    {
                        auditMsg = string.Format("剩余年假: 申请前{0}小时，申请后{1}小时", beforeUpdated, afterUpdated);
                    }

                    else if (order.OrderType == OrderType.Overtime)
                    {
                        auditMsg = string.Format("累计加班: 申请前{0}小时，申请后{1}小时", beforeUpdated, afterUpdated);
                    }
                    else if (order.OrderType == OrderType.DaysOff)
                    {
                        auditMsg = string.Format("剩余调休: 申请前{0}小时，申请后{1}小时", beforeUpdated, afterUpdated);
                    }
                    else
                    {
                        auditMsg = string.Format("累计休假: 申请前{0}小时，申请后{1}小时", beforeUpdated, afterUpdated);
                    }

                    if (!string.IsNullOrEmpty(auditMsg))
                    {
                        detail.Audit = auditMsg;
                    }
                    #endregion
                });
            }
            return true;
        }
    }
}
