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
    /// 流程处理引擎
    /// </summary>
    public class WorkflowProcessService : ServiceBase, IWorkflowProcessService
    {
        private readonly IWorkflowService _workflowService = new WorkflowService();
        private readonly IUserService _userService = new UserService();
        private readonly IAttendanceSummaryService _attendanceSummaryService = new AttendanceSummaryService();

        /// <summary>
        /// 添加流程处理记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="curUser"></param>
        /// <param name="operation"></param>
        public void AddWorkflowProcess(MissionskyOAEntities dbContext, UserModel curUser,
            OperateOrderModel operation)
        {
            var process = new WorkflowProcessModel()
            {
                OrderNo = operation.OrderNo,
                FlowId = 0,
                StepId = 0,
                Operator = curUser.Id,
                OperationDesc = string.Format("{0}取消了申请单。", curUser.EnglishName),
                Operation = operation.Operation,
                CreatedTime = DateTime.Now
            };

            dbContext.WorkflowProcesses.Add(process.ToEntity());
        }

        /// <summary>
        /// 流程处理
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="operation">流程操作</param>
        /// <param name="curUser">当前用户</param>
        /// <returns>流程处理对象</returns>
        /// <remarks>
        /// 流程逻辑：
        /// 1, 提交申请时，查找申请人的直接领导，
        /// 2, 如果申请人的直接领导可以审批通过，则转到行政审批
        /// 3, 如果申请人的直接领导没有权限审批， 则转到审批人的直接领导，重复2的验证
        /// 4, 如果申请人无直接领导，则转到行政审批
        /// </remarks>
        public WorkflowProcessModel Process(MissionskyOAEntities dbContext, OrderModel order, UserModel curUser, OperateOrderModel operation)
        {
            try
            {
                #region 1, 验证申请单数据及状态
                if (order == null || order.OrderDets == null || order.OrderDets.Count < 1)
                {
                    Log.Error("申请单数据无效。");
                    throw new InvalidOperationException("申请单数据无效。");
                }

                if (!order.IsApproveStatus() && !order.IsReviewStatus()) //无效的[审批或审阅]状态
                {
                    Log.Error("申请单状态无效。");
                    throw new KeyNotFoundException("申请单状态无效。");
                }

                if (operation.Operation == WorkflowOperation.Approve &&
                    (order.WorkflowId == null || order.WorkflowId.Value < 1 || order.NextStep == null ||
                     order.NextStep.Value < 1 || order.NextAudit == null || order.NextAudit.Value < 1))
                {
                    Log.Error("申请单流程无效。");
                    throw new KeyNotFoundException("申请单流程无效。");
                }
                
                var detail = order.OrderDets.FirstOrDefault(); //申请单详细

                if (detail == null || !detail.IOHours.HasValue)
                {
                    throw new KeyNotFoundException("申请单详细数据无效。");
                }
                #endregion
                
                //流程处理
                var workflow = GetWorkflow(dbContext, order, curUser, operation); //2, 选择流程
                var nextStep = GotoNextStep(order, curUser, workflow, operation); //3, 选择流程下一步
                var nextApprover = GotoNextApprover(dbContext, nextStep); //4, 转到流程下一步审批人
                var nextStatus = SwitchStatus(order, nextStep, operation); //5, 转换申请单流程状态

                UpdateAttendanceSummary(dbContext, order, workflow, nextStep, nextStatus); //6, 更新假期表
                
                var process = new WorkflowProcessModel()
                {
                    OrderNo = order.OrderNo,
                    FlowId = workflow.Id,
                    StepId = operation.Operation == WorkflowOperation.Apply || !order.NextStep.HasValue? 0 : order.NextStep.Value,
                    NextStep = nextStep,
                    NextApprover = nextApprover,
                    NextStatus = nextStatus,
                    Operator = curUser.Id,
                    OperationDesc =
                        operation.Operation == WorkflowOperation.Apply ? detail.Description : operation.Opinion,
                    Operation =
                        operation.Operation == WorkflowOperation.Apply
                            ? WorkflowOperation.Apply
                            : operation.Operation,
                    CreatedTime = DateTime.Now
                };

                dbContext.WorkflowProcesses.Add(process.ToEntity());

                return process;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 申请单流程状态转换
        /// </summary>
        /// <param name="order">申请单</param>
        /// <param name="nextStep">流程下一步</param>
        /// <param name="operation">流程操作</param>
        /// <returns>流程下一步状态</returns>
        private OrderStatus SwitchStatus(OrderModel order, WorkflowStepModel nextStep, OperateOrderModel operation)
        {
            var nextStatus = OrderStatus.Apply;

            //1, 申请
            if (operation.Operation == WorkflowOperation.Apply)
            {
                nextStatus = OrderStatus.Apply;
            }
            //2, 拒绝
            else if (operation.Operation == WorkflowOperation.Rejecte)
            {
                nextStatus = OrderStatus.Rejected;
            }
            //3, 流程未完成
            else if (nextStep != null && nextStep.Id > 0)
            {
                //3.1, 行政、财务审阅(领导已批准)
                if (nextStep.IsAdminReviewing() || nextStep.IsFinanceReviewing())
                {
                    //nextStatus = OrderStatus.Reviewing;
                    nextStatus = (order.RefOrderId == null || order.RefOrderId.Value < 1
                        ? OrderStatus.Approved
                        : OrderStatus.Revoked);
                }
                //3.2, 领导审批
                else if (nextStep.IsLeaderApproving())
                {
                    nextStatus = OrderStatus.Approving;
                }
            }
            //4, 流程结束
            else
            {
                nextStatus = (order.RefOrderId == null || order.RefOrderId.Value < 1
                    ? OrderStatus.Approved
                    : OrderStatus.Revoked);
            }

            return nextStatus;
        }

        /// <summary>
        /// 获取申请单流程
        /// </summary>
        /// <param name="dbContext">数据库上下文对象</param>
        /// <param name="order">申请单</param>
        /// <param name="curUser">当前用户</param>
        /// <param name="operation">操作</param>
        /// <returns>流程对象</returns>
        private WorkflowModel GetWorkflow(MissionskyOAEntities dbContext, OrderModel order, UserModel curUser, OperateOrderModel operation)
        {
            var searchWorkflow = new SearchWorkflowModel();

            if (operation.Operation == WorkflowOperation.Apply) //申请
            {
                searchWorkflow.WorkflowId = MatchWorkflow(dbContext, order, curUser); //匹配当前用户满足条件的申请流程
            }
            else
            {
                if (!order.WorkflowId.HasValue)
                {
                    Log.Error("无效的申请单流程。");
                    throw new InvalidOperationException("无效的申请单流程。");
                }

                searchWorkflow.WorkflowId = order.WorkflowId.Value;
            }

            var workflow = _workflowService.GetWorkflowDetail(dbContext, searchWorkflow.WorkflowId); //流程

            if (workflow == null)
            {
                Log.Error("找不到申请流程。");
                throw new KeyNotFoundException("找不到申请流程。");
            }

            return workflow;
        }

        /// <summary>
        /// 根据部门和申请单类型配置流程
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="curUser">当前用户</param>
        /// <returns>配置的流程ID</returns>
        private int MatchWorkflow(MissionskyOAEntities dbContext, OrderModel order, UserModel curUser)
        {
            var workflowId = 0; //配置的流程ID
            
            //匹配流程
            var flowType = order.IsOvertime() ? (int)WorkflowType.Overtime : (int)WorkflowType.AskLeave;
            var workflows = dbContext.Workflows.Where(it => it.Type == flowType && it.Status).ToList();

            workflows.ForEach(it =>
            {
                if (!string.IsNullOrEmpty(it.Condition) && workflowId == 0) //流程条件不为空 && 未匹配到相关流程
                {
                    //满足申请条件的用户
                    var conditionUser =
                        dbContext.Users.SqlQuery(string.Format("SELECT * FROM [USER] WHERE Id={0} AND {1}", curUser.Id,
                            it.Condition));

                    //用户满足申请条件
                    if (conditionUser != null && conditionUser.Any())
                    {
                        workflowId = it.Id;
                    }
                }
            });

            if (workflowId <= 0)
            {
                Log.Error("找不到申请流程。");
                throw new KeyNotFoundException("找不到申请流程。");
            }

            return workflowId;
        }

        #region 获取流程下一步
        /// <summary>
        /// 申请单流程状态转换
        /// </summary>
        /// <param name="order">申请单</param>
        /// <param name="curUser">当前用户</param>
        /// <param name="workflow">流程下一步</param>
        /// <param name="operation">流程操作</param>
        /// <returns>流程下一步状态</returns>
        private WorkflowStepModel GotoNextStep(OrderModel order, UserModel curUser, WorkflowModel workflow, OperateOrderModel operation)
        {
            if (order == null || order.OrderDets == null || order.OrderDets.Count < 1)
            {
                Log.Error("无效的申请单。");
                throw new InvalidOperationException("无效的申请单。");
            }

            if (workflow == null || workflow.WorkflowSteps == null || workflow.WorkflowSteps.Count < 1)
            {
                Log.Error("无效的流程。");
                throw new InvalidOperationException("无效的流程。");
            }
            
            WorkflowStepModel nextStep = null; //流程下一步

            switch (operation.Operation)
            {
                case WorkflowOperation.Apply: //申请
                case WorkflowOperation.Revoke: //撤销
                    //直接领导
                    var director = curUser.DirectlySupervisorId.HasValue
                        ? _userService.GetUserDetail(curUser.DirectlySupervisorId.Value)
                        : null;

                    nextStep = GetNextStepForUserApply(director, workflow);
                    break;
                case WorkflowOperation.Approve: //审批
                    var curStep = GetCurrentStep(order, workflow, operation); //获取流程当前步骤
                    curStep = GetApproveStepForApprover(curUser, curStep, workflow); //将[直接领导]审批步骤转换为有效审批步骤

                    if (curStep.IsLeaderApproving()) //当前是[领导审批]
                    {
                        nextStep = GetNextStepForLeaderApprove(order, curStep, workflow); //获取[领导审批]的下一步骤
                    }
                    else if (curStep.IsAdminReviewing()) //当前是[行政审批]
                    {
                        nextStep = workflow.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.FinanceReview); //需要[财务审批]
                    }
                    break;
                case WorkflowOperation.None: //无效
                case WorkflowOperation.Rejecte: //拒绝
                case WorkflowOperation.Cancel: //取消
                    break;
            }

            return nextStep;
        }

        /// <summary>
        /// 获取[领导审批]的下一步骤
        /// </summary>
        /// <param name="order"></param>
        /// <param name="curStep"></param>
        /// <param name="workflow"></param>
        /// <returns></returns>
        private WorkflowStepModel GetNextStepForLeaderApprove(OrderModel order, WorkflowStepModel curStep, WorkflowModel workflow)
        {
            WorkflowStepModel nextStep = null;
            var detail = order.OrderDets.FirstOrDefault(); //申请单详细

            if (detail == null || detail.IOHours == null || Math.Abs(detail.IOHours.Value) <= 0.0)
            {
                Log.Error("无效的申请单详细。");
                throw new KeyNotFoundException("无效的申请单详细。");
            }

            //在领导审批允许的范围内，则结束[领导审批]，转到[行政财务审批]
            if (Math.Abs(detail.IOHours.Value) <= curStep.MaxTimes)
            {
                //行政审批
                nextStep = workflow.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.AdminReview); 
                //财务审批
                nextStep = nextStep ?? workflow.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.FinanceReview);
            }
            else
            {
                curStep.NextStep = curStep.NextStep ?? Constant.WORKFLOW_INVALID_NODE;
                nextStep = workflow.WorkflowSteps.FirstOrDefault(it => it.Id == curStep.NextStep.Value);

                //领导审批未结束, 则提示异常
                if (nextStep == null || !nextStep.IsLeaderApproving())
                {
                    Log.Error("无效的领导审批流程。");
                    throw new KeyNotFoundException("无效的领导审批流程。");
                }
            }

            if (nextStep == null)
            {
                Log.Error("无效的审批流程。");
                throw new KeyNotFoundException("无效的审批流程。");
            }

            return nextStep;
        }

        /// <summary>
        /// 获取用户申请的下一审批步骤
        /// </summary>
        /// <param name="director"></param>
        /// <param name="workflow"></param>
        /// <returns></returns>
        private WorkflowStepModel GetNextStepForUserApply(UserModel director, WorkflowModel workflow)
        {
            WorkflowStepModel nextStep = null;

            //无直接领导人, 则到[行政财务]审批
            if (director == null)
            {
                //行政审批
                nextStep = workflow.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.AdminReview);
                //财务审批
                nextStep = nextStep ?? workflow.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.FinanceReview);
            }
            //[直接领导]审批 
            else
            {
                nextStep = workflow.GetDirectSupervisorApproveStep();
                nextStep.OperatorType = WorkflowOperator.User;
                nextStep.Operator = director.Id; //设置审批人
            }

            if (nextStep == null)
            {
                Log.Error("无效的申请流程。");
                throw new InvalidOperationException("无效的申请流程。");
            }

            return nextStep;
        }

        /// <summary>
        /// 获取审批步骤，将[直接领导]审批步骤转换为有效审批步骤
        /// </summary>
        /// <param name="approver"></param>
        /// <param name="curStep"></param>
        /// <param name="workflow"></param>
        /// <returns></returns>
        private WorkflowStepModel GetApproveStepForApprover(UserModel approver, WorkflowStepModel curStep,
            WorkflowModel workflow)
        {
            if (curStep.Id == Constant.WORKFLOW_DIRECT_SUPERVISOR_APPROVE_NODE) //当前步骤是[直接领导审批]
            {
                //按[角色]查找
                curStep =
                    workflow.WorkflowSteps.FirstOrDefault(
                        it =>
                            it.Type == WorkflowStepType.LeaderApprove &&
                            it.OperatorType == WorkflowOperator.Role && it.Operator == approver.Role);

                //按[用户]查找
                curStep = curStep ?? workflow.WorkflowSteps.FirstOrDefault(
                    it =>
                        it.Type == WorkflowStepType.LeaderApprove &&
                        it.OperatorType == WorkflowOperator.User && it.Operator == approver.Id);

                if (curStep == null)
                {
                    Log.Error("直接领导无法审批流程。");
                    throw new InvalidOperationException("直接领导无法审批流程。");
                }
            }

            return curStep;
        }

        /// <summary>
        /// 流程当前步骤
        /// </summary>
        /// <param name="order"></param>
        /// <param name="workflow"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private WorkflowStepModel GetCurrentStep(OrderModel order, WorkflowModel workflow, OperateOrderModel operation)
        {
            WorkflowStepModel curStep = null;

            if (operation.Operation == WorkflowOperation.Apply)
            {
                curStep = workflow.GetUserApplyStep(); //当前步骤为[申请]步骤
            }
            else
            {
                if (order.NextStep.HasValue)
                {
                    curStep = (order.NextStep.Value == Constant.WORKFLOW_DIRECT_SUPERVISOR_APPROVE_NODE
                        ? workflow.GetDirectSupervisorApproveStep() //当前步骤为[直接领导审批]步骤
                        : workflow.WorkflowSteps.FirstOrDefault(it => it.Id == order.NextStep.Value));

                }
            }

            if (curStep == null)
            {
                Log.Error("当前流程步骤无效。");
                throw new KeyNotFoundException("当前流程步骤无效。");
            }

            return curStep;
        }
        #endregion

        /// <summary>
        /// 转到下一个审批人
        /// </summary>
        /// <param name="dbContext">数据库上下文对象</param>
        /// <param name="nextStep">下一步</param>
        /// <returns></returns>
        private UserModel GotoNextApprover(MissionskyOAEntities dbContext, WorkflowStepModel nextStep)
        {
            UserModel nextApprover = null; //流程下一步审批人

            if (nextStep != null)
            {
                //[用户]审批
                if (nextStep.OperatorType == WorkflowOperator.User)
                {
                    nextApprover = dbContext.Users.FirstOrDefault(it => it.Id == nextStep.Operator).ToModel();
                }
                    //[角色]审批
                else
                {
                    var userRole = dbContext.UserRoles.FirstOrDefault(it => it.RoleId == nextStep.Operator);

                    if (userRole == null)
                    {
                        Log.Error("找不到流程的审批人角色。");
                        throw new KeyNotFoundException("找不到流程的审批人角色。");
                    }

                    var nextApproverEntity = dbContext.Users.FirstOrDefault(it => it.Id == userRole.UserId);

                    if (nextApproverEntity == null)
                    {
                        Log.Error("找不到流程审批人。");
                        throw new KeyNotFoundException("找不到流程审批人。");
                    }

                    nextApprover = nextApproverEntity.ToModel();
                }
            }

            return nextApprover;
        }

        /// <summary>
        /// 更新假期信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="order">申请单</param>
        /// <param name="workflow">申请单流程</param>
        /// <param name="nextStep">流程下一步</param>
        /// <param name="nextStatus">申请单状态</param>
        private void UpdateAttendanceSummary(MissionskyOAEntities dbContext, OrderModel order, WorkflowModel workflow,
            WorkflowStepModel nextStep, OrderStatus nextStatus)
        {
            var detail = order.OrderDets.FirstOrDefault(); //申请单详细

            if (detail == null || detail.IOHours == null || Math.Abs(detail.IOHours.Value) <= 0.0)
            {
                Log.Error("申请单无效");
                throw new InvalidOperationException("申请单无效。");
            }

            var curStep = order.NextStep == Constant.WORKFLOW_DIRECT_SUPERVISOR_APPROVE_NODE
                ? workflow.GetDirectSupervisorApproveStep()
                : workflow.WorkflowSteps.FirstOrDefault(it => it.Id == order.NextStep); //当前步骤

            var isUpdated = false;
            var times = detail.IOHours.Value;

            //1, 拒绝
            if (nextStatus == OrderStatus.Rejected)
            {
                if (curStep != null && curStep.IsFinanceReviewing()) //当前是[财务审阅]: [行政审批]完成，已经更新了假期，需要[财务审阅]时，取消扣除的假期
                {
                    isUpdated = true;
                    times = -times;
                }
            }
            //2, 流程结束(只能在没有财务审批节点才更新，避免重复更新(原始需求：完成行政审批就已经更新假期))
            else if (!workflow.NeedFinanceApprove() && nextStep == null)
            {
                isUpdated = true;
            }
            //3, 流程未结束, 转到[财务审阅](完成行政审批就更新)
            else if (nextStep != null && nextStep.IsFinanceReviewing())
            {
                isUpdated = true;
            }

            if (isUpdated)
            {
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
        }
    }
}
