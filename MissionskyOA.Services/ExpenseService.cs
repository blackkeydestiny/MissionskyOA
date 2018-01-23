using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Common;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using System.Data.Entity.Validation;

namespace MissionskyOA.Services
{
    public class ExpenseService : ServiceBase, IExpenseService
    {
        private readonly INotificationService _notificationService = new NotificationService();

        /// <summary>
        /// 添加报销申请单
        /// </summary>
        /// <param name="model">报销单</param>
        /// <returns>报销申请</returns>
        public ExpenseMainModel AddExpense(ExpenseMainModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                model = InitExpenseAudit(model);
                ExpenseMain expenseMainEntity = model.ToEntity();
                dbContext.ExpenseMains.Add(expenseMainEntity);
                dbContext.SaveChanges();
                return expenseMainEntity.ToModel();
            }

        }

        /// <summary>
        /// 初始化审批状态
        /// </summary>
        public ExpenseMainModel InitExpenseAudit(ExpenseMainModel model)
        {
            //2.1 构造审批历史
            model.ExpenseAuditHistories = new List<ExpenseAuditHistoryModel>();
            ExpenseAuditHistoryModel auditHistoryModel = new ExpenseAuditHistoryModel();
            using (var dbContext = new MissionskyOAEntities())
            {
                var financialUserEntity = dbContext.Users.FirstOrDefault(it => it.Email != null && it.Email.ToLower().Contains(Global.FinancialEmail));
                //国内
                if (model.DeptNo == 1)
                {
                    var inlandHeaderUserEntity = dbContext.Users.FirstOrDefault(it => it.Email != null && it.Email.ToLower().Contains(Global.InlandHeaderEmail));
                    if (inlandHeaderUserEntity != null)
                    {
                        auditHistoryModel.CurrentAudit = inlandHeaderUserEntity.Id;
                        auditHistoryModel.NextAudit = financialUserEntity.Id;
                        auditHistoryModel.Status = OrderStatus.Apply;
                        auditHistoryModel.CreatedTime = DateTime.Now;
                    }
                }
                else
                {
                    var overseaHeaderUserEntity = dbContext.Users.FirstOrDefault(it => it.Email != null && it.Email.ToLower().Contains(Global.OverseaHeaderEmail));
                    if (overseaHeaderUserEntity != null)
                    {
                        auditHistoryModel.CurrentAudit = overseaHeaderUserEntity.Id;
                        auditHistoryModel.NextAudit = financialUserEntity.Id;
                        auditHistoryModel.Status = OrderStatus.Apply;
                        auditHistoryModel.CreatedTime = DateTime.Now;
                    }
                }
                model.AuditId=(int)ExpenseAuditStep.DepartmentAudit;
                model.ExpenseAuditHistories.Add(auditHistoryModel);

                return model;
            }

        }


        /// <summary>
        /// 显示所有报销单
        /// </summary>
        /// <returns>报销单分页信息</returns>
        public IPagedList<ExpenseMainModel> MyExpenseList(UserModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<ExpenseMainModel>();

                var expenses =
                    dbContext.ExpenseMains.Where(
                        it =>
                            ((it.ApplyUserId == model.Id))
                        ).OrderByDescending(item => item.CreatedTime);

                expenses.ToList().ForEach(item =>
                {
                    var expense = item.ToModel();
                    expense.currentAuditStatus = GetExpenseCurrentStatus(expense.Id, expense.AuditId);
                    result.Add(expense);
                });

                return new PagedList<ExpenseMainModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 获取当前报销单状态
        /// </summary>
        public ExpenseAuditHistoryModel GetExpenseCurrentStatus(int expenseMainId, int auditId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var financialUserEntity =
                    dbContext.Users.FirstOrDefault(
                        it => it.Email != null && it.Email.ToLower().Contains(Global.FinancialEmail));

                var expenseHistory = (auditId == (int) ExpenseAuditStep.FinacialAudit)
                    ? dbContext.ExpenseAuditHistories.FirstOrDefault(it => it.ExpenseId == expenseMainId && it.CurrentAudit == financialUserEntity.Id)
                    : dbContext.ExpenseAuditHistories.FirstOrDefault(it => it.ExpenseId == expenseMainId);


                return expenseHistory == null ? null : expenseHistory.ToModel();
            }
        }

        /// <summary>
        /// 显示具体报销单
        /// </summary>
        /// <returns>显示具体报销单</returns>
        public List<ExpenseDetailModel> getExpenseDetailByID(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var expenseMainEntity = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == id);
                List<ExpenseDetailModel> listExpneseDetail = new List<ExpenseDetailModel>();
                if (expenseMainEntity != null)
                {
                    var expenseDetailEntity = dbContext.ExpenseDetails.Where(it => it.MId == expenseMainEntity.Id);

                    expenseDetailEntity.ToList().ForEach(item =>
                    {
                        listExpneseDetail.Add(item.ToModelWithParticant());
                    });
                    return listExpneseDetail;
                }
                else
                {
                    throw new Exception("无效的ID");
                }

            }

        }

        /// <summary>
        /// 获取用户待处理的报销单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>审批任务列表</returns>
        public List<ExpenseMainModel> getPendingAuditLists(int userId)
        {
            var pendingExpenseOrder = new List<ExpenseMainModel>();
            using (var dbContext = new MissionskyOAEntities())
            {
                var expenseOrders = dbContext.ExpenseAuditHistories.Where(it => it.CurrentAudit == userId && it.Status != (int)OrderStatus.Rejected && it.Status != (int)OrderStatus.Approved&&it.Status!=(int)OrderStatus.Canceled).OrderByDescending(it=>it.CreatedTime);
                expenseOrders.ToList().ForEach(item =>
                {
                    var expenseMain = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == item.ExpenseId);
                    pendingExpenseOrder.Add(expenseMain.ToModel());
                });

                return pendingExpenseOrder;
            }
        }

        /// <summary>
        /// 部门总监审批
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="expenseMainEntity">报销单</param>
        /// <param name="reason">报销人员</param>
        /// <param name="isApproveExpense"></param>
        /// <returns>报销申请</returns>
        public void AuditExpenseByDepartmentHeader(MissionskyOAEntities dbContext, ExpenseMain expenseMainEntity, string reason, bool isApproveExpense)
        {
            var financialUserEntity = new User();//财务专员
            var auditEntity = new ExpenseAuditHistory(); //当前审批节点
            var curAduitUser = new User(); //当前审批人
            var applicant = new User(); //申请人详细

            //获取报销单相关信息
            FillRelatedInfo(dbContext, expenseMainEntity, out financialUserEntity, out curAduitUser, out applicant, out auditEntity);

            //审计信息
            var message = new AuditMessageModel()
            {
                Type = AuditMessageType.None,
                Status = AuditMessageStatus.Unread,
                CreatedTime = DateTime.Now,
                Message = "{0}{1}{2}的报销申请。" //A同意或拒绝B的报销申请
            };
            //推送消息
            var notificationModel = new NotificationModel()
            {
                Target = applicant.Email,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.Approving,
                Title = "Missionsky OA Notification",
                MessagePrams = "报销流程处理",
                Scope = NotificationScope.User,
                CreatedTime = DateTime.Now,
                TargetUserIds = new List<int> {applicant.Id}
            };

            if (isApproveExpense)
            {
                //设置原审批人同意,生成下一步审批人
                expenseMainEntity.AuditId = (int) ExpenseAuditStep.FinacialAudit;
                auditEntity.Status = (int) OrderStatus.Approved;
                auditEntity.AuditMessage = reason;
                auditEntity.CreatedTime = DateTime.Now;

                #region 审批人转到财务专员
                var nextAudit = new ExpenseAuditHistory()
                {
                    CurrentAudit = financialUserEntity.Id,
                    Status = (int) OrderStatus.Approving,
                    ExpenseId = expenseMainEntity.Id,
                    CreatedTime = DateTime.Now
                };

                dbContext.ExpenseAuditHistories.Add(nextAudit);
                #endregion

                message.Type = AuditMessageType.Approve_Expense_Application_Message;
                message.Message = string.Format(message.Message, curAduitUser.EnglishName, "同意", applicant.EnglishName);
                notificationModel.MessageContent = string.Format("{0}同意了你的报销申请", curAduitUser.EnglishName);
            }
            else
            {
                //设置原审批人拒绝
                auditEntity.Status = (int) OrderStatus.Rejected;
                auditEntity.AuditMessage = reason;
                auditEntity.NextAudit = null;
                auditEntity.CreatedTime = DateTime.Now;
                message.Type = AuditMessageType.Recject_Expense_Application_Message;
                message.Message = string.Format(message.Message, curAduitUser.EnglishName, "拒绝", applicant.EnglishName);
                notificationModel.MessageContent = string.Format("{0}拒绝了你的报销申请，理由是:{1}", applicant.EnglishName, reason);
            }

            this._notificationService.Add(notificationModel, Global.IsProduction); //消息推送
            dbContext.AuditMessages.Add(message.ToEntity());

            dbContext.SaveChanges();
        }

        /// <summary>
        /// 财务审批
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="expenseMainEntity">报销单</param>
        /// <param name="reason">报销人员</param>
        /// <param name="isApproveExpense"></param>
        /// <returns>报销申请</returns>
        public void AuditExpenseByFinacertHeader(MissionskyOAEntities dbContext, ExpenseMain expenseMainEntity, string reason, bool isApproveExpense)
        {
            var financialUserEntity = new User();//财务专员
            var auditEntity = new ExpenseAuditHistory(); //当前审批节点
            var curAduitUser = new User(); //当前审批人
            var applicant = new User(); //申请人详细

            //获取报销单相关信息
            FillRelatedInfo(dbContext, expenseMainEntity, out financialUserEntity, out curAduitUser, out applicant, out auditEntity);

            //审计信息
            var message = new AuditMessageModel()
            {
                Type = AuditMessageType.None,
                Status = AuditMessageStatus.Unread,
                CreatedTime = DateTime.Now,
                Message = "{0}{1}{2}的报销申请。" //A同意或拒绝B的报销申请
            };

            //推送消息
            var notificationModel = new NotificationModel()
            {
                Target = applicant.Email,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.Approving,
                Title = "Missionsky OA Notification",
                MessagePrams = "报销流程处理",
                Scope = NotificationScope.User,
                CreatedTime = DateTime.Now,
                TargetUserIds = new List<int> { applicant.Id }
            };

            if (isApproveExpense)
            {
                //设置原审批人同意
                auditEntity.Status = (int)OrderStatus.Approved;
                auditEntity.AuditMessage = reason;
                auditEntity.NextAudit = null;
                auditEntity.CreatedTime = DateTime.Now;
                message.Type = AuditMessageType.Approve_Expense_Application_Message;
                message.Message = string.Format(message.Message, financialUserEntity.EnglishName, "同意", applicant.EnglishName);
                notificationModel.MessageContent = string.Format("{0}同意了你的报销申请，请及时提交纸质报销单", financialUserEntity.EnglishName);

                //发送报销申请单
                SendExpenseForm(expenseMainEntity.Id, applicant.ToModel(), financialUserEntity.ToModel());
                expenseMainEntity.PrintForm ++;
            }
            else
            {
                //设置原审批人拒绝
                auditEntity.Status = (int)OrderStatus.Rejected;
                auditEntity.AuditMessage = reason;
                auditEntity.CreatedTime = DateTime.Now;
                auditEntity.NextAudit = null;
                message.Type = AuditMessageType.Recject_Expense_Application_Message;
                message.Message = string.Format(message.Message, financialUserEntity.EnglishName, "拒绝", applicant.EnglishName);
                notificationModel.MessageContent = string.Format("{0}拒绝了你的报销申请，原因:{1}", financialUserEntity.EnglishName, reason);
            }

            this._notificationService.Add(notificationModel, Global.IsProduction); //消息推送
            dbContext.AuditMessages.Add(message.ToEntity());

            dbContext.SaveChanges();
        }


        /// <summary>
        /// 批准或者拒绝报销单
        /// </summary>
        /// <param name="expenseId">申请单号</param>
        /// <param name="reason">理由</param>
        /// <param name="approver">审批人</param>
        /// <param name="isApprove">是否同意报销申请</param>
        /// <returns>是否审批成功</returns>
        public bool ApproveOrRejectExpense(int expenseId, String reason, UserModel approver, bool isApprove)
        {
            if (expenseId < 1 || approver == null)
            {
                Log.Error("报销单无效。");
                throw new InvalidOperationException("报销单无效。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //报销单
                var expenseEntities = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseId);

                if (expenseEntities == null)
                {
                    Log.Error(string.Format("未找相关报销单, Id: {0}。", expenseId));
                    throw new KeyNotFoundException(string.Format("未找相关报销单, Id: {0}。", expenseId));
                }


                if (approver.Email.EqualsIgnoreCase(Global.FinancialEmail)) //财审专员审批
                {
                    AuditExpenseByFinacertHeader(dbContext, expenseEntities, reason, isApprove);
                }
                else if (approver.Email.EqualsIgnoreCase(Global.InlandHeaderEmail)) //国内部门主管审批
                {
                    AuditExpenseByDepartmentHeader(dbContext, expenseEntities, reason, isApprove);
                }
                else if (approver.Email.EqualsIgnoreCase(Global.OverseaHeaderEmail)) //国外部门主管审批
                {
                    AuditExpenseByDepartmentHeader(dbContext, expenseEntities, reason, isApprove);
                }
                else
                {
                    Log.Error("报销审批流程无效。");
                    throw new InvalidOperationException("报销审批流程无效");
                }
            }
            return true;
        }
        
        /// <summary>
        /// 发送报销单
        /// </summary>
        /// <param name="expenseId"></param>
        /// <returns></returns>
        public bool SendExpenseForm(int expenseId)
        {
            if (expenseId <= 0)
            {
                Log.Error(string.Format("无效的报销申请, Id: {0}", expenseId));
                throw new InvalidOperationException(string.Format("无效的报销申请, Id: {0}", expenseId));
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //报销单
                var expenseEntitiy = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseId);

                if (expenseEntitiy == null)
                {
                    Log.Error(string.Format("未找相关报销单, Id: {0}", expenseId));
                    throw new KeyNotFoundException(string.Format("未找相关报销单, Id: {0}", expenseId));
                }

                //申请人
                var applicant = dbContext.Users.FirstOrDefault(it => it.Id == expenseEntitiy.ApplyUserId);
                if (applicant == null)
                {
                    Log.Error("找不到报销申请人。");
                    throw new KeyNotFoundException("找不到报销申请人。");
                }

                SendExpenseForm(expenseId, applicant.ToModel(), null); //发送报销单
                expenseEntitiy.PrintForm ++;

                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 统计审批人审批(待审批和已审批)报销单
        /// </summary>
        /// <param name="userId">审批人</param>
        /// <returns>审批人审批报销单</returns>
        public List<ExpenseAuditSummaryModel> ExpenseSummary(int userId)
        {
            if (userId < 1)
            {
                throw new InvalidOperationException("无效的审批人Id。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var entities =
                    dbContext.ExpenseAuditHistories.Where(
                        it => it.CurrentAudit == userId && it.Status != (int) OrderStatus.Canceled);
                //查询审批人所有审批单(已审批和待审批)

                var expenseOrders = new List<ExpenseAuditSummaryModel>();

                entities.ToList().ForEach(entity =>
                {
                    var expenseMainEntity = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == entity.ExpenseId);
                    bool isCurrentUserApproved = true;

                    if (entity.Status == (int) OrderStatus.Approving || entity.Status == (int) OrderStatus.Apply)
                    {
                        isCurrentUserApproved = false;
                    }

                    //ExpenseAuditHistoryModel expenseMain=GetExpenseCurrentStatus(expenseMainEntity.Id,expenseMainEntity.AuditId);
                    var summaryModel = new ExpenseAuditSummaryModel()
                    {
                        model = expenseMainEntity.ToModelWithAuditHistory(),
                        IsAudited = isCurrentUserApproved,
                        Status = (OrderStatus) entity.Status
                    };

                    expenseOrders.Add(summaryModel);
                });
                expenseOrders = expenseOrders.OrderByDescending(it => it.model.CreatedTime).ToList();

                return expenseOrders;
            }
        }

        /// <summary>
        /// 取消报销单
        /// </summary>
        /// <returns>取消</returns>
        public bool CancelExpense(int expenseId, UserModel user)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var orders = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseId);
                var auditMessageType = (int)AuditMessageType.Apply_Expense_Cancel_Application_Message;
                if(orders==null)
                {
                    throw new Exception("报销单不存在");
                }
                
                if (orders.AuditId == (int)ExpenseAuditStep.DepartmentAudit)
                {
                    ExpenseAuditHistoryModel expenseCurrentStaus = GetExpenseCurrentStatus(orders.Id, orders.AuditId);
                    if (expenseCurrentStaus != null && expenseCurrentStaus.Status == OrderStatus.Apply)
                    {
                        var auditHistory =dbContext.ExpenseAuditHistories.FirstOrDefault(it => it.Id == expenseCurrentStaus.Id);
                        auditHistory.NextAudit = null;
                        auditHistory.CurrentAudit = 0;
                        auditHistory.Status = (int)OrderStatus.Canceled;
                        var auditMessageEntitry = new AuditMessage()
                        {
                            Type = auditMessageType,
                            UserId = user.Id,
                            Status = (int)AuditMessageStatus.Unread,
                            Message = auditMessageType.ToString(),
                            CreatedTime = DateTime.Now
                        };

                        dbContext.AuditMessages.Add(auditMessageEntitry);

                        //更新数据库
                        dbContext.SaveChanges();

                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("已经在审批中，不能直接取消");
                    }
                }
                else
                {
                    throw new InvalidOperationException("已经在审批中，不能直接取消");
                }

            }
        }

        /// <summary>
        /// 修改报销单信息（目前只可修改报销理由）
        /// </summary>
        /// <returns>报销单信息</returns>
        public bool UpdateExpenseOrder(int expenseId, UpdateExpenseModel model, UserModel user)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var expenseMain = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseId);
                if (expenseMain == null)
                {
                    throw new Exception("报销单不存在");
                }
                if(!user.Email.EqualsIgnoreCase(Global.FinancialEmail))
                {
                    ExpenseAuditHistoryModel expenseCurrentStaus = GetExpenseCurrentStatus(expenseMain.Id, expenseMain.AuditId);
                    if (expenseMain.AuditId != (int)ExpenseAuditStep.DepartmentAudit || expenseCurrentStaus.Status != OrderStatus.Apply)
                    {
                        Log.Error("已经处于审批流程，不可修改");
                        throw new InvalidOperationException("已经处于审批流程，不可修改");
                    }
                }

                expenseMain.Reason = model.Reason;
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 确认接收资料
        /// </summary>
        /// <param name="expenseId">申请单号</param>
        /// <param name="approver">审批人</param>
        /// <returns>是否成功</returns>
        public bool ReciveExpenseFile(int expenseId, UserModel approver)
        {
            if (expenseId < 1 || approver == null)
            {
                Log.Error("无效的操作");
                throw new InvalidOperationException("无效的操作。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //报销单
                var expenseMainEntity = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseId); //报销申请单

                if (expenseMainEntity == null)
                {
                    Log.Error("报销单无效");
                    throw new InvalidOperationException("报销单无效");
                }

                if (approver.Email.EqualsIgnoreCase(Global.FinancialEmail))
                {
                    var financialUserEntity = new User();//财务专员
                    var auditEntity = new ExpenseAuditHistory(); //当前审批节点
                    var curAduitUser = new User(); //当前审批人
                    var applicant = new User(); //申请人详细

                    //获取报销单相关信息
                    FillRelatedInfo(dbContext, expenseMainEntity, out financialUserEntity, out curAduitUser, out applicant, out auditEntity);

                    //审计信息
                    var message = new AuditMessageModel()
                    {
                        Type = AuditMessageType.None,
                        Status = AuditMessageStatus.Unread,
                        CreatedTime = DateTime.Now,
                        Message = string.Format("{0}已提交报销纸资申请资料", applicant.EnglishName)
                    };

                    //推送消息
                    var notificationModel = new NotificationModel()
                    {
                        Target = applicant.Email,
                        //MessageType = NotificationType.PushMessage,
                        MessageType = NotificationType.Email,
                        BusinessType = BusinessType.Approving,
                        Title = "Missionsky OA Notification",
                        MessagePrams = "报销流程处理",
                        Scope = NotificationScope.User,
                        CreatedTime = DateTime.Now,
                        TargetUserIds = new List<int> { applicant.Id }
                    };

                    //确认提交纸质材料
                    expenseMainEntity.ConfirmForm = true;

                    //设置原审批人同意
                    auditEntity.Status = (int)OrderStatus.Recieved;
                    auditEntity.NextAudit = null;
                    auditEntity.CreatedTime = DateTime.Now;
                    message.Type = AuditMessageType.Confirm_Expense_File_Message;
                    notificationModel.MessageContent = string.Format("{0}已收到你的报销申请材料", financialUserEntity.EnglishName);
                  
                    this._notificationService.Add(notificationModel, Global.IsProduction); //消息推送
                    dbContext.AuditMessages.Add(message.ToEntity());

                    dbContext.SaveChanges();
                }
                else
                {
                    Log.Error("无确认权限");
                    throw new InvalidOperationException("无确认权限");
                }
            }

            return true;
        }

        /// <summary>
        /// 修改报销详细信息
        /// </summary>
        /// <returns>报销详细信息</returns>
        public bool UpdateExpenseDetailsOrder(int id, ApplyExpenseDetailModel model,UserModel user)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请详细信息是否存在
                var expenseDetail = dbContext.ExpenseDetails.FirstOrDefault(it => it.Id == id);
                if (expenseDetail == null)
                {
                    throw new Exception("报销详细单id不存在");
                }

                if (!user.Email.EqualsIgnoreCase(Global.FinancialEmail))
                {
                    //报销单
                    var expenseMainEntity = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseDetail.MId);
                    ExpenseAuditHistoryModel expenseCurrentStaus = GetExpenseCurrentStatus(expenseMainEntity.Id, expenseMainEntity.AuditId);
                    if (expenseMainEntity.AuditId != (int)ExpenseAuditStep.DepartmentAudit || expenseCurrentStaus.Status != OrderStatus.Apply)
                    {
                        Log.Error("已经处于审批流程，不可修改");
                        throw new InvalidOperationException("已经处于审批流程，不可修改");
                    }
                }

                if (model.Amount > 0 && model.Amount != expenseDetail.Amount)
                {
                    CalculateExpenseSum(expenseDetail.MId, model.Amount - expenseDetail.Amount);
                }

                expenseDetail.ODate = model.ODate;
                expenseDetail.Remark = model.Remark;
                expenseDetail.Amount = model.Amount;

                if (model.EType != 0)
                {
                    expenseDetail.EType = (int) model.EType;
                }

                #region 更新成员
                if (model.participants != null && model.participants.Count() > 0)
                {
                    var expenseMems = dbContext.ExpenseMembers.Where(it => it.DId == expenseDetail.Id); //原来存在的
                    var maxDetId = dbContext.ExpenseMembers.Max(it => it.Id); //求最大Detail Id

                    //移除现在不存在的
                    expenseMems.ToList().ForEach(it =>
                    {
                        if (!model.participants.Contains(it.MemberId))
                        {
                            dbContext.ExpenseMembers.Remove(it);
                        }
                    });

                    //添加新增的
                    model.participants.ToList().ForEach(it =>
                    {
                        if (expenseMems.Any(m => m.MemberId == it) == false)
                        {
                            dbContext.ExpenseMembers.Add(new ExpenseMember() { MemberId = it, DId = expenseDetail.Id, Id = ++ maxDetId });
                        }
                    });
                }
                #endregion

                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 删除报销单详细信息
        /// </summary>
        /// <returns>是否成功</returns>
        public bool DeleteExpenseDetailsOrder(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请详细信息是否存在
                var expenseDetail = dbContext.ExpenseDetails.FirstOrDefault(it => it.Id == id);
                if (expenseDetail == null)
                {
                    throw new Exception("报销详细单id不存在");
                }

                CalculateExpenseSum(expenseDetail.MId, -expenseDetail.Amount);
        
                var expenseDetailMembers = dbContext.ExpenseMembers.Where(it => it.DId == expenseDetail.Id);

                
                
                if (expenseDetailMembers!=null)
                {                   
                    expenseDetailMembers.ToList().ForEach(entity =>
                    {
                        dbContext.ExpenseMembers.Remove(entity);
                    });
                }
                dbContext.ExpenseDetails.Remove(expenseDetail);
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 添加报销详细信息
        /// </summary>
        /// <returns>是否成功</returns>
        public bool AddExpenseDetailsOrder(int id, ApplyExpenseDetailModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询报销信息是否存在
                var expenseMain = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == id);
                if (expenseMain == null)
                {
                    throw new Exception("报销单id不存在");
                }

                var expenseDetailModel = new ExpenseDetailModel()
                    {
                        ODate = model.ODate,
                        EType = model.EType,
                        Remark = model.Remark,
                        PCount = 1,
                        Amount = model.Amount
                    };
                if (model.Amount > 0)
                {
                    CalculateExpenseSum(expenseMain.Id, model.Amount);
                }
                //3.1 构造参与人员
                List<ExpenseMemberModel> activyParticipants = new List<ExpenseMemberModel>();
                if (model.participants != null && (model.participants.Length > 0))
                {
                    foreach (int participantItem in model.participants)
                    {
                        var expenseMemberModel = new ExpenseMemberModel()
                        {
                            MemberId = participantItem
                        };
                        activyParticipants.Add(expenseMemberModel);
                    }
                }
                expenseDetailModel.ExpenseMembers = activyParticipants;
                expenseMain.ExpenseDetails.Add(expenseDetailModel.ToEntity());


                dbContext.SaveChanges();
                return true;
            }
        }

        public void CalculateExpenseSum(int expenseMainId, decimal money)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请详细信息是否存在
                var expenseMain = dbContext.ExpenseMains.FirstOrDefault(it => it.Id == expenseMainId);
                if (expenseMain == null)
                {
                    throw new Exception("报销单不存在");
                }
                expenseMain.Amount = expenseMain.Amount + money;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 显示所有报销单
        /// </summary>
        /// <returns>报销单</returns>
        public ListResult<ExpenseMainModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.ExpenseMains.AsEnumerable();

                if (filter != null && filter.Member == "ApplyUserName")
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
                    list = list.Where(it => userIds.Contains(it.ApplyUserId.Value));
                }
                if (filter != null && filter.Member == "CreatedTime")
                {
                    var dtFormat = new System.Globalization.DateTimeFormatInfo() { ShortDatePattern = "yyyy-MM-dd" };
                    var dt = Convert.ToDateTime(filter.Value, dtFormat);

                    switch (filter.Operator)
                    {
                        case "IsEqualTo":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date == dt);
                            break;
                        case "IsNotEqualTo":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date != dt);
                            break;
                        case "IsGreaterThan":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date > dt);
                            break;
                        case "IsLessThan":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date < dt);
                            break;
                        case "IsGreaterThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date >= dt);
                            break;
                        case "IsLessThanOrEqualTo":
                            list = list.Where(it => it.CreatedTime.GetValueOrDefault().Date >= dt);
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

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                var result = new ListResult<ExpenseMainModel>() { Data = new List<ExpenseMainModel>() };
                list.ToList().ForEach(item =>
                {
                    var expense = item.ToModelWithAuditHistory();
                    result.Data.Add(expense);
                });

                result.Total = count;

                return result;
            }
        }
        
        /// <summary>
        /// 发送报销单
        /// </summary>
        /// <param name="expenseId">报销单Id</param>
        /// <param name="receiver">报错单接收人</param>
        /// <param name="ccUser">抄送人</param>
        private void SendExpenseForm(int expenseId, UserModel receiver, UserModel ccUser)
        {
            var reportParams =new Dictionary<string,string>();
            reportParams.Add("RequestID", expenseId.ToString());
            ReportHelper4Service.SendReport(Global.ExpenseOrderReportNo, receiver, ccUser, "Excel", reportParams);
        }

        /// <summary>
        /// 获取报销申请相关信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="expenseMainEntity"></param>
        /// <param name="financialUserEntity"></param>
        /// <param name="curAduitUser"></param>
        /// <param name="applicant"></param>
        /// <param name="auditEntity"></param>
        private void FillRelatedInfo(MissionskyOAEntities dbContext, ExpenseMain expenseMainEntity,
            out User financialUserEntity, out User curAduitUser, out User applicant, out ExpenseAuditHistory auditEntity)
        {
            //申请人详细
            applicant = dbContext.Users.FirstOrDefault(it => it.Id == expenseMainEntity.ApplyUserId);
            if (applicant == null)
            {
                Log.Error("找不到报销申请人。");
                throw new KeyNotFoundException("找不到报销申请人。");
            }

            //财务专员
            financialUserEntity =
                dbContext.Users.FirstOrDefault(
                    it => it.Email != null && it.Email.ToLower().Contains(Global.FinancialEmail));
            if (financialUserEntity == null)
            {
                Log.Error("找不到财务专号。");
                throw new KeyNotFoundException("找不到财务专员。");
            }

            //当前审批节点
            auditEntity =
                dbContext.ExpenseAuditHistories.Where(it => it.ExpenseId == expenseMainEntity.Id)
                    .OrderByDescending(it => it.Id)
                    .FirstOrDefault();
            if (auditEntity == null)
            {
                Log.Error("无效的当前审批步骤。");
                throw new KeyNotFoundException("无效的当前审批步骤。");
            }

            //当前审批人
            var currentAuditId = auditEntity.CurrentAudit;
            curAduitUser = (auditEntity.CurrentAudit == financialUserEntity.Id
                ? financialUserEntity
                : dbContext.Users.FirstOrDefault(it => it.Id == currentAuditId));
            if (curAduitUser == null)
            {
                Log.Error("无效的当前审批人。");
                throw new KeyNotFoundException("无效的当前审批人。");
            }
        }
    }
}
