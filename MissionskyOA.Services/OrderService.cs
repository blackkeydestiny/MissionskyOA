using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 申请单处理
    /// </summary>
    public class OrderService : ServiceBase, IOrderService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        private readonly IWorkflowProcessService _workflowProcessService = new WorkflowProcessService();
        private readonly IUserService _userService = new UserService();
        //private readonly IAskLeaveService _askLeaveService = new AskLeaveService();

        #region SQL常量
        /// <summary>
        /// 获取当前申请单号
        /// 
        /// 获取当前Order表中OrderNo的最大值，然后再+1，就是新申请单的OrderNo
        /// 
        /// </summary>
        private const string GENERATE_ORDER_NO = @"SELECT ISNULL(MAX(OrderNo), 0) + 1 FROM [Order];";

        /// <summary>
        /// 统计审批人审批(待审批和已审批)申请单
        /// </summary>
        private const string SUMMARY_APPROVE_ORDERS = @"
                        SELECT * FROM [Order]
                        WHERE ISNULL(Status, 0) != 4 --过滤已取消的单
                                --AND ApplyUserId !={0}
                                AND OrderNo IN (
	                                SELECT DISTINCT OrderNo FROM WorkflowProcess WHERE Operator = {0} AND ISNULL(StepId, 0) != 0 --已审批 (StepId = 0, 为流程申请节点)
	                                UNION
	                                SELECT DISTINCT OrderNo AS OrderId FROM [Order] WHERE ISNULL(NextAudit, 0) = {0} AND ISNULL(NextStep, 0) != 0 --待审批
                                );
                    ";
        #endregion

        /// <summary>
        /// 获取当前申请单号
        /// 
        /// 生成申请单号
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns>申请单号</returns>
        public int GenerateOrderNo(MissionskyOAEntities dbContext)
        {
            // 1、
            int currentApplyOrderId = 0;

            /*
             * 
             *  涉及到EntityFramework.dll的DbContext的使用
             *  
             *  SqlQuery<TElement>(string sql，params object[] parameters) 返回的是一个 DbRawSqlQuery<TElement>类型对象
             * 
             * 
             * **/
            // 2、获取当前Order表中OrderNo的最大值，然后再+1，就是新申请单的OrderNo
            var data = dbContext.Database.SqlQuery<int>(GENERATE_ORDER_NO);

            var test = data.ToList();

            // 3、
            if (data != null)
            {

                currentApplyOrderId = Math.Abs(data.FirstOrDefault());
            }

            // 4、
            return currentApplyOrderId;
        }

        /// <summary>
        /// 根据申请单号和用户Id获取申请单Id
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="orderNo">申请单号</param>
        /// <param name="userId">申请用户</param>
        /// <returns></returns>
        public int GetOrderIdByOrderNoAndUserId(MissionskyOAEntities dbContext, int orderNo, int userId)
        {
            var order = dbContext.Orders.FirstOrDefault(it => it.OrderNo == orderNo && it.UserId == userId);

            if (order == null)
            {
                Log.Error("找不到申请单。");
                throw new InvalidOperationException("找不到申请单。");
            }

            return order.Id;
        }

        #region 申请单操作

        /// <summary>
        /// 流程申请
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <returns>流程处理对象</returns>
        public WorkflowProcessModel Apply(MissionskyOAEntities dbContext, OrderModel order)
        {
            #region 验证申请单数据
            if (order == null)
            {
                Log.Error("申请单无效。");
                throw new InvalidOperationException("申请单无效。");
            }

            if (order.OrderDets == null || order.OrderDets.Count < 1)
            {
                Log.Error("申请单详细数据无效。");
                throw new InvalidOperationException("申请单详细数据无效。");
            }

            //申请流程类型
            if (!order.IsAskLeave() && !order.IsOvertime()) //申请单类型是否有效
            {
                Log.Error("申请单类型无效。");
                throw new InvalidOperationException("申请单类型无效。");
            }

            //流程申请人
            var applicant = dbContext.Users.FirstOrDefault(it => it.Id == order.ApplyUserId);
            if (applicant == null) //流程申请人是否有效
            {
                Log.Error("流程申请人无效。");
                throw new KeyNotFoundException("流程申请人无效。");
            }
            #endregion

            return _workflowProcessService.Process(dbContext, order, applicant.ToModel(),
                new OperateOrderModel() { Operation = WorkflowOperation.Apply });
        }

        /// <summary>
        /// 批准请假加班申请(撤销)单
        /// </summary>
        /// <param name="model">申请单</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否审批成功</returns>
        public int Approve(OperateOrderModel model, UserModel approver)
        {
            if (model == null || model.OrderNo < 1 || approver == null)
            {
                Log.Error("无效的操作。");
                throw new InvalidOperationException("无效的操作。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                #region 1 申请单详细

                //申请单详细
                var orderEntities = dbContext.Orders.Where(it => it.OrderNo == model.OrderNo); //所有申请单
                var entity = orderEntities.FirstOrDefault();

                if (entity == null)
                {
                    Log.Error("无效的申请单。");
                    throw new KeyNotFoundException("无效的申请单。");
                }

                var order = entity.ToModel();
                if (order.OrderUsers == null || order.OrderUsers.Count < 1)
                {
                    order.OrderUsers = _userService.GetOrderUsers(dbContext, order.OrderNo);
                }

                //当前审批人是否为流程下一个审批人 || 当前审批人是否存在
                if (order.NextAudit == null && order.NextStep == null)
                {
                    Log.Error("申请流程已完成。");
                    throw new InvalidOperationException("申请流程已完成。");
                }

                if (approver.Id != order.NextAudit)
                {
                    Log.Error("当前用户不允许审批流程。");
                    throw new InvalidOperationException("当前用户不允许审批流程。");
                }

                //申请人详细
                var applicant = dbContext.Users.FirstOrDefault(it => it.Id == order.ApplyUserId);
                if (applicant == null)
                {
                    Log.Error("找不到申请人。");
                    throw new KeyNotFoundException("找不到申请人。");
                }

                #endregion

                #region 2 流程审批处理

                var process = _workflowProcessService.Process(dbContext, order, approver, model);

                if (process == null)
                {
                    Log.Error("找不到流程。");
                    throw new InvalidOperationException("找不到流程。");
                }

                orderEntities.ToList().ForEach(item =>
                {
                    item.Status = (int) process.NextStatus;

                    if (process.NextStep != null) //下一步骤不为null，则需要上级继续审批
                    {
                        item.NextStep = process.NextStep.Id;
                        item.NextAudit = process.NextApprover.Id;
                    }
                    else //下一步骤为null，则审批结束
                    {
                        item.NextAudit = null;
                        item.NextStep = null;
                        item.Status = order.IsRevokedOrder()
                            ? (int) OrderStatus.Revoked
                            : (int) OrderStatus.Approved; //批准撤销 || 批准申请
                    }
                });

                #endregion

                #region 3 添加审计信息

                //审计信息
                var message = new AuditMessageModel()
                {
                    Type = AuditMessageType.None,
                    Status = AuditMessageStatus.Unread,
                    CreatedTime = DateTime.Now,
                    Message = "你同意{0}的{1}申请"
                };

                //同意加班申请/撤销消息
                if (order.IsOvertime())
                {
                    message.Type = order.IsRevokedOrder()
                        ? AuditMessageType.Approve_OT_Cance_Application_Message
                        : AuditMessageType.Approve_OT_Application_Message;
                    message.Message = string.Format(message.Message, applicant.EnglishName, "加班");
                }

                //同意请假申请/撤销消息
                if (order.IsAskLeave())
                {
                    message.Type = order.IsRevokedOrder()
                        ? AuditMessageType.Approve_Leave_Cacnel_Application_Message
                        : AuditMessageType.Approve_Leave_Application_Message;
                    message.Message = string.Format(message.Message, applicant.EnglishName, "请假");
                }

                dbContext.AuditMessages.Add(message.ToEntity());

                #endregion

                #region 4 工作流处理消息推送

                //流程结束 && 批量申请，向流程申请人发推送消息
                if (process.IsCompletedApproved() && order.IsBatchApply)
                {
                    AddNotification(dbContext, order, approver.Id, process); //默认发送到申请人
                }

                order.OrderUsers.ToList().ForEach(item =>
                {
                    if (process.IsCompletedApproved()) //审批通过， 流程结束 || 过[行政审阅]到[财务审阅]
                    {
                        AddNotification(dbContext, order, approver.Id, process, item.Id);
                    }
                    else if (process.NeedContinueApprove()) //需要下一步审批审批
                    {
                        AddNotification(dbContext, order, approver.Id, process, process.NextApprover.Id);
                    }
                    else
                    {
                        Log.Error("处理流程异常。");
                        throw new InvalidOperationException("处理流程异常。");
                    }
                });

                #endregion

                // 5更新数据库
                dbContext.SaveChanges();

                return (int) order.Status;
            }
        }

        /// <summary>
        /// 流程撤销
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="owner">申请人</param>
        /// <param name="operation">流程操作</param>
        /// <returns>下一个审批步骤</returns>
        public WorkflowProcessModel Revoke(MissionskyOAEntities dbContext, OrderModel order, UserModel owner, OperateOrderModel operation)
        {
            if (order == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            // 流程审批
            WorkflowProcessModel process = _workflowProcessService.Process(dbContext, order, owner, operation);

            return process;
        }

        /// <summary>
        /// 拒绝请假加班申请(撤销)单
        /// </summary>
        /// <param name="model">申请单</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否拒绝成功</returns>
        public bool Rejecte(OperateOrderModel model, UserModel approver)
        {
            if (model == null || model.OrderNo < 1 || approver == null)
            {
                Log.Error("无效的参数。");
                throw new InvalidOperationException("无效的参数。");
            }
            
            if (string.IsNullOrEmpty(model.Opinion))
            {
                Log.Error("拒拒理由为空。");
                throw new InvalidOperationException("拒拒理由为空。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                #region 申请单详细
                //申请单详细
                var orderEntities = dbContext.Orders.Where(it => it.OrderNo == model.OrderNo);

                #region 验证申请是否有效
                var validateEntity = orderEntities.FirstOrDefault();

                //流程及步骤是否有效
                if (validateEntity == null || validateEntity.WorkflowId < 1 || validateEntity.NextStep < 1)
                {
                    Log.Error("申请单流程无效。");
                    throw new InvalidOperationException("申请单流程无效.");
                }

                var validateOrder = validateEntity.ToModel();

                //申请单为撤销单
                var isRevokedOrder = validateOrder.IsRevokedOrder();

                //领导审批中
                var isApproveStatus = validateOrder.IsApproveStatus();

                //行政财务审批中
                var isReviewStatus = validateOrder.IsReviewStatus();

                //申请状态不是审批状态
                if (!isApproveStatus && !isReviewStatus)
                {
                    Log.Error("申请单状态无效。");
                    throw new InvalidOperationException("申请单状态无效.");
                }

                //用户不是申请当前的审批人
                if (approver.Id != validateEntity.NextAudit)
                {
                    Log.Error("当前用户不能审批申请。");
                    throw new InvalidOperationException("当前用户不能审批申请.");
                }

                //申请人详细
                var applicant = dbContext.Users.FirstOrDefault(it => it.Id == validateEntity.ApplyUserId);
                if (applicant == null)
                {
                    Log.Error("无效的申请人。");
                    throw new KeyNotFoundException("无效的申请人。");
                }
                #endregion

                var hasSentToAapplicant = false;　//已经向申请人发送过拒绝申请的推送消息
                var operation = new OperateOrderModel() { Operation = WorkflowOperation.Rejecte, Opinion = model.Opinion };
                WorkflowProcessModel process = _workflowProcessService.Process(dbContext, validateOrder, approver, operation); //拒绝申请

                orderEntities.ToList().ForEach(o =>
                {
                    if (isRevokedOrder && !isApproveStatus) //拒绝撤销申请单，则更新原始申请单为通过状态
                    {
                        var oldOrder = dbContext.Orders.FirstOrDefault(it => it.Id == o.RefOrderId.Value);

                        if (oldOrder != null)
                        {
                            oldOrder.Status = (int)OrderStatus.Approved;
                        }
                    }

                    //更新申请单状态[拒绝]
                    o.Status = (int)OrderStatus.Rejected;
                    o.NextAudit = null; //不再需要审批
                    o.NextStep = null; //

                    //更新假期
                    //_askLeaveService.RecoverOrder(o.ToModel());

                    hasSentToAapplicant = (hasSentToAapplicant || o.UserId == applicant.Id);
                    AddNotification(dbContext, validateOrder, approver.Id, process, o.Id); //向用户推送拒绝消息
                });

                if (!hasSentToAapplicant)
                {
                    AddNotification(dbContext, validateOrder, approver.Id, process, applicant.Id); //向申请人推送拒绝消息 
                }
                #endregion

                #region 添加审计信息
                //审计信息
                var message = new AuditMessageModel()
                {
                    Type = AuditMessageType.None,
                    Status = AuditMessageStatus.Unread,
                    CreatedTime = DateTime.Now,
                    Message = "你拒绝{0}的{1}申请"
                };

                //同意加班申请/撤销消息
                if (validateOrder.IsOvertime())
                {
                    message.Type = AuditMessageType.Recject_OT_Application_Message;
                    message.Message = string.Format(message.Message, applicant.EnglishName,
                        (validateOrder.IsRevokedOrder() ? "加班撤销" : "加班"));
                }

                //同意请假申请/撤销消息
                if (validateOrder.IsAskLeave())
                {
                    message.Type = AuditMessageType.Recject_Leave_Application_Message;
                    message.Message = string.Format(message.Message, applicant.EnglishName,
                        (validateOrder.IsRevokedOrder() ? "请假撤销" : "请假"));
                }

                dbContext.AuditMessages.Add(message.ToEntity());
                #endregion

                //更新数据库
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 工作流处理消息推送
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order">申请单</param>
        /// <param name="operaterId">消息推送人Id</param>
        /// <param name="process">流程处理</param>
        /// <param name="receiverId">消息接收人Id</param>
        /// <remarks>receiverId = 0: 表示默认发送到代申请人</remarks>
        public void AddNotification(MissionskyOAEntities dbContext, OrderModel order, int operaterId,
            WorkflowProcessModel process, int receiverId = 0)
        {
            #region 1. 获取用户信息

            //代理申请人
            var agentUser = dbContext.Users.FirstOrDefault(it => it.Id == order.ApplyUserId);
            if (agentUser == null)
            {
                Log.Error(string.Format("找不到代申请人, Id: {0}", order.ApplyUserId));
                throw new KeyNotFoundException("找不到代申请人。");
            }

            //消息接收人
            var receiver = dbContext.Users.FirstOrDefault(it => it.Id == receiverId);
            receiver = receiver ?? agentUser;

            //消息推送人
            var operater = (operaterId == receiverId
                ? receiver
                : dbContext.Users.FirstOrDefault(it => it.Id == operaterId));

            if (operater == null)
            {
                Log.Error(string.Format("找不到流程操作人, Id: {0}", receiverId));
                throw new KeyNotFoundException("找不到流程操作人。");
            }
            #endregion

            #region 2. 验证申请单

            if (order == null || order.OrderDets == null || order.OrderDets.Count < 1)
            {
                Log.Error("无效的流程申请单。");
                throw new InvalidOperationException("无效的流程申请单。");
            }

            var detail = order.OrderDets.FirstOrDefault();
            if (detail == null || !detail.IOHours.HasValue)
            {
                Log.Error("无效的流程申请单详细。");
                throw new InvalidOperationException("无效的流程申请单详细。");
            }

            // 委托人
            var recipient = dbContext.Users.FirstOrDefault(it => it.Id == detail.Recipient);

            var checkType = order.OrderType;
            if (checkType == OrderType.DaysOff)
            {
                checkType = OrderType.Overtime;
            }

            #endregion

            #region 3. 批量申请，向代申请推送消息

            var model = new NotificationModel()
            {
                Target = receiver.Email,
                CreatedUserId = operater.Id,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.Approving,
                Title = "Missionsky OA Notification",
                MessagePrams = "申请单流程处理",
                Scope = NotificationScope.User,
                CreatedTime = DateTime.Now,
                TargetUserIds = new List<int> {receiver.Id}
            };

            //流程拒绝时，推送拒绝消息
            if (process.Operation == WorkflowOperation.Rejecte) 
            {
                model.MessageContent = string.Format("您的{0}单被拒绝。", order.IsOvertime() ? "加班" : "请假");
            }
            //审批流程结束，或者到财务审批时(需要更新用户假期信息)
            else if (process.Operation == WorkflowOperation.Approve &&
                     (process.NextStep == null || process.NextStep.IsFinanceReviewing())) 
            {
                if (receiverId == 0) //默认发送到代申请人
                {
                    model.MessageContent = string.Format("您的{0}单已批准。", order.IsOvertime() ? "加班" : "请假");
                }
                else if (receiver.Id == receiverId) //发送到请假或加班实际申请人
                {
                    //假期汇总信息
                    var summary =
                        dbContext.AttendanceSummaries.FirstOrDefault(
                            it =>
                                it.UserId == receiverId && it.Type == (int)checkType && it.Year == DateTime.Now.Year);

                    if (summary == null)
                    {
                        Log.Error("未找用户假期或加班信息。");
                        throw new KeyNotFoundException("未找用户假期或加班信息。");
                    }

                    //计算剩余天数
                    var days = Math.Round(detail.IOHours.Value/8, 1); //申请天数
                    var remainDays = summary.RemainValue.HasValue
                        ? Math.Round(summary.RemainValue.Value/8, 1) + days
                        : 0.0; //剩余天数

                    days = Math.Abs(days); //取绝对值

                    if (order.OrderType == OrderType.AnnualLeave) //年假
                    {
                        model.MessageContent = order.RefOrderId.HasValue
                            ? string.Format("{0}, 您的年假增加{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays)
                            : string.Format("{0}, 您的年假被扣除{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays);
                    }
                    else if (order.OrderType == OrderType.DaysOff) //调休
                    {
                        model.MessageContent = order.RefOrderId.HasValue
                            ? string.Format("{0}, 您的调休假增加{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays)
                            : string.Format("{0}, 您的调休假被扣除{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays);
                    }
                    else if (order.OrderType == OrderType.Overtime) //加班
                    {
                        model.MessageContent = order.RefOrderId.HasValue
                            ? string.Format("{0}, 您的调休假被扣除{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays)
                            : string.Format("{0}, 您的调休假增加{1}天，剩余{2}天。", receiver.EnglishName, days, remainDays);
                    }
                    else //其它假期
                    {
                        model.MessageContent = string.Format("您的{0}单已批准。", order.IsOvertime() ? "加班" : "请假");
                    }
                }
                else
                {
                    Log.Error("推送消息接收人不匹配。");
                    throw new KeyNotFoundException("推送消息接收人不匹配。");
                }
            }
            //申请或审批时，发送到下一步审批人
            else 
            {
                if (receiverId == 0) //默认发送到代申请人
                {
                    model.MessageContent = string.Format("请您及时审批{0}的{1}单, 时间从{2} {3}到{4} {5}, 总时长为{6}, {7}。", 
                        agentUser.EnglishName,
                        order.IsOvertime() ? "加班" : "请假",
                        detail.StartDate.ToString("yyyy-MM-dd"), 
                        OrderExtentions.FormatTimeSpan(detail.StartTime),
                        detail.EndDate.ToString("yyyy-MM-dd"), 
                        OrderExtentions.FormatTimeSpan(detail.EndTime),
                        Math.Abs((sbyte)detail.IOHours),
                        recipient == null ? "没有工作交接人。" : "把工作交接给了" + recipient.EnglishName
                    );

                    model.MessageContent = string.Format("您的{0}单已批准。", order.IsOvertime() ? "加班" : "请假");
                }
                else if (receiver.Id == receiverId) //发送到请假或加班实际申请人
                {
                    model.MessageContent = string.Format("请您及时审批{0}的{1}单，时间从{2} {3}到{4} {5}, 总时长为{6}, {7}。", 
                        agentUser.EnglishName,
                        order.IsOvertime() ? "加班" : "请假",
                        detail.StartDate.ToString("yyyy-MM-dd"),
                        OrderExtentions.FormatTimeSpan(detail.StartTime),
                        detail.EndDate.ToString("yyyy-MM-dd"),
                        OrderExtentions.FormatTimeSpan(detail.EndTime),
                        Math.Abs((sbyte)detail.IOHours),
                        recipient == null ? "没有工作交接人。" : "把工作交接给了" + recipient.EnglishName
                        );
                }
                else
                {
                    Log.Error("推送消息接收人不匹配。");
                    throw new KeyNotFoundException("推送消息接收人不匹配。");
                }
            }

            this._notificationService.Add(model, Global.IsProduction); //消息推送

            #endregion
        }

        #endregion

        #region 获取用户审批的申请单

        /// <summary>
        /// 获取用户待处理的申请单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="status">申请单状态</param>
        /// <returns>审批任务列表</returns>
        public List<OrderModel> GetPendingOrdersByUserId(int userId, OrderStatus? status = null)
        {
            if (status == null)
            {
                status = OrderStatus.Invalid;
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var pendingOrders = new List<OrderModel>();

                dbContext.Orders.Where(
                    it => status == OrderStatus.Invalid || (!it.Status.HasValue && status == OrderStatus.Apply) || (int)status == it.Status)
                    .ToList()
                    .ForEach(it =>
                    {
                        var order = it.ToModel();

                        //待处理申请单: 申请 || 领导审批中 || 行政财务审阅中 && 下一步审批人为当前登录用户
                        if (pendingOrders.Any(o => o.OrderNo == order.OrderNo) == false && //过滤批量申请单
                            (order.IsApproveStatus() || order.IsReviewStatus()) &&
                            it.NextAudit.HasValue && it.NextAudit.Value == userId)
                        {
                            DoFill(dbContext, order);
                            pendingOrders.Add(order);
                        }
                    });

                return pendingOrders;
            }
        }

        /// <summary>
        /// 获取用户待处理的申请单数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PendingOrderCountModel CountPendingOrders(int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var model = new PendingOrderCountModel();
                var orders = new List<int>();

                dbContext.Orders.ToList().ForEach(it =>
                {
                    var order = it.ToModel();

                    //待处理申请单: 申请 || 领导审批中 || 行政财务审阅中 && 下一步审批人为当前登录用户
                    if (orders.Any(o => o == it.OrderNo) == false && //过滤批量申请单
                        (order.IsApproveStatus() || order.IsReviewStatus()) && 
                        it.NextAudit.HasValue && it.NextAudit.Value == userId)
                    {
                        if (order.IsAskLeave()) //汇总请假
                        {
                            model.AskLeave++;
                        }

                        if (order.IsOvertime()) //汇总加班
                        {
                            model.Overtime++;
                        }
                    }

                    orders.Add(it.OrderNo);
                });

                return model;
            }
        }

        /// <summary>
        /// 统计审批人审批(待审批和已审批)申请单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <param name="status">申请单状态</param>
        /// <returns>审批人审批申请单</returns>
        public List<ApproveOrderModel> SummaryApproveOrders(int userId, OrderStatus? status= null)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("无效的审批人Id。");
            }
            
            if (status == null)
            {
                status = OrderStatus.Invalid;
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var entities = dbContext.Orders.SqlQuery(string.Format(SUMMARY_APPROVE_ORDERS, userId)).ToList();
                    //查询审批人所有审批单(已审批和待审批)
                entities =
                    entities.Where(
                        it => status == OrderStatus.Invalid || (!it.Status.HasValue && status == OrderStatus.Apply) || it.Status == (int) status)
                        .ToList();

                var approveOrders = new List<ApproveOrderModel>();

                var users = dbContext.Users; //所有用户
                var details = dbContext.OrderDets; //所有申请详细

                entities.ToList().ForEach(entity =>
                {
                    if (approveOrders.Any(o => o.OrderNo == entity.OrderNo) == false) //过滤批量申请单
                    {
                        var detail = details.FirstOrDefault(it => it.OrderId == entity.Id); //申请单详细
                        var user = users.FirstOrDefault(it => it.Id == entity.UserId); //申请人

                        //构造审批单列表
                        if (detail != null && entity.Status.HasValue && user != null)
                        {
                            var order = new ApproveOrderModel()
                            {
                                OrderNo = entity.OrderNo,
                                Status = (OrderStatus) entity.Status.Value,
                                OrderType = (OrderType) entity.OrderType,
                                OrderUsers = _userService.GetOrderUsers(dbContext, entity.OrderNo), //申请单用户
                                IsRevoked = entity.RefOrderId.HasValue,
                                //StartDate = Convert.ToDateTime(detail.StartDate.ToShortDateString() +" "+ detail.StartTime.ToString()),
                                //EndDate = Convert.ToDateTime(detail.EndDate.ToShortDateString() +" "+ detail.EndTime.ToString()),
                                AppliedTime = detail.StartDate,
                                IOHours = detail.IOHours,
                                IsApproved = true //已审批
                            };

                            if (entity.NextAudit.HasValue && entity.NextAudit.Value == userId) //是否当前用户未审批
                            {
                                order.IsApproved = false;
                            }

                            approveOrders.Add(order);
                        }
                    }
                });
                
                return approveOrders.OrderByDescending(it => it.AppliedTime).ToList();
            }
        }

        /// <summary>
        /// 查询关联人员的申请单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <returns>审批人审批申请单</returns>
        public List<ApproveOrderModel> SearchApproveOrders(string englishname, int userId)
        {

            using (var dbContext = new MissionskyOAEntities())
            {
                List<ApproveOrderModel> list = this.SummaryApproveOrders(userId);
                var approveOrders = new List<ApproveOrderModel>();
                foreach (ApproveOrderModel item in list)
                {
                    var listByEnglishName = item.OrderUsers.FirstOrDefault(it =>  it.UserName != null && it.UserName.ToLower().Contains(englishname.ToLower()));
                    if(listByEnglishName!=null)
                    {
                        approveOrders.Add(item);
                    }
                }
                return approveOrders;
            }

        }
        #endregion

        /// <summary>
        /// 加载申请相关信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order"></param>
        /// <param name="loadWorkFlow"></param>
        public static void DoFill(MissionskyOAEntities dbContext, OrderModel order, bool loadWorkFlow = false)
        {
            //获取所有当前用户的 申请单附件 ID
            var attachmentList = (new AttachmentService()).GetAttathcmentIds(dbContext, order.OrderNo, Constant.ATTACHMENT_TYPE_ORDER_ATTACHMENT);

            // 向申请用户中添加对应的附件
            if (order.Attachments == null || order.Attachments.Any() == false)
            {
                order.Attachments = attachmentList;
            }
            else
            {
                foreach (int attachment in attachmentList)
                {
                    order.Attachments.Add(attachment);
                }
            }

            //当前步骤是否到[行政审批]或[财务审批]
            order.IsReview = order.IsReviewStatus();

            //申请用户(批量申请时，可能用多个用户)
            order.OrderUsers = (new UserService()).GetOrderUsers(dbContext, order.OrderNo);

            //加载流程信息
            if (loadWorkFlow && order.WorkflowId.HasValue)
            {
                // 获取流程
                var workflow = (new WorkflowService()).GetWorkflowDetail(dbContext, order.WorkflowId.Value);

                if (workflow == null)
                {
                    Log.Error("找不到相关流程。");
                    throw new KeyNotFoundException("找不到相关流程。");
                }

                //获取流程经办详细
                order.WorkflowName = workflow.Name;
                order.ProcessHistory = new List<WorkflowProcessModel>();

                if (order.WorkflowId > 0)
                {
                    dbContext.WorkflowProcesses.Where(
                        it => it.OrderNo == order.OrderNo && it.FlowId == order.WorkflowId)
                        .ToList()
                        .ForEach(it =>
                        {
                            if (it.StepId != 0) //申请StepId = 0
                            {
                                var process = it.ToModel();
                                var step = workflow.WorkflowSteps.FirstOrDefault(s => s.Id == it.StepId);
                                var operater = dbContext.Users.FirstOrDefault(u => u.Id == it.Operator);

                                process.StepName = step == null ? string.Empty : step.Name;
                                process.OperatorName = operater == null
                                    ? string.Empty
                                    : operater.EnglishName;

                                order.ProcessHistory.Add(process);
                            }
                        });
                }
            }
        }

        /// <summary>
        /// 请假工作交接通知
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="order">申请单</param>
        public void AddNotification(MissionskyOAEntities dbContext, OrderModel order)
        {
            if (order == null || order.OrderType == OrderType.Overtime)
            {
                return;
            }

            //申请人
            var applicant = dbContext.Users.FirstOrDefault(it => it.Id == order.ApplyUserId);
            if (applicant == null)
            {
                Log.Error(string.Format("找不到代申请人, Id: {0}", order.ApplyUserId));
                throw new KeyNotFoundException("找不到代申请人。");
            }

            if (order.OrderDets == null || order.OrderDets.Count < 1)
            {
                Log.Error("无效的流程申请单。");
                throw new InvalidOperationException("无效的流程申请单。");
            }

            var detail = order.OrderDets.FirstOrDefault();
            if (detail == null)
            {
                Log.Error("无效的流程申请单详细。");
                throw new InvalidOperationException("无效的流程申请单详细。");
            }
            
            //消息接收人
            var receiver = dbContext.Users.FirstOrDefault(it => it.Id == detail.Recipient);

            if (receiver == null)
            {
                return;
            }

            #region 3. 批量申请，向代申请推送消息

            var model = new NotificationModel()
            {
                Target = receiver.Email,
                CreatedUserId = applicant.Id,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.Approving,
                Title = "Missionsky OA Notification",
                MessagePrams = "请假工作交接通知",
                Scope = NotificationScope.User,
                CreatedTime = DateTime.Now,
                TargetUserIds = new List<int> {receiver.Id},
                MessageContent = string.Format("{0}请假，时间从{1} {2}到{3} {4}, 总时长为{5}, 并把工作交接给你了, 谢谢。", 
                                                applicant.EnglishName,
                                                detail.StartDate.ToString("yyyy-MM-dd"),
                                                OrderExtentions.FormatTimeSpan(detail.StartTime),
                                                detail.EndDate.ToString("yyyy-MM-dd"),
                                                OrderExtentions.FormatTimeSpan(detail.EndTime),
                                                Math.Abs((sbyte)detail.IOHours)
                                            )
            };
            
            this._notificationService.Add(model, Global.IsProduction); //消息推送

            #endregion
        }
    }
}
