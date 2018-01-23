using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class WorkflowExtentions
    {
        public static Workflow ToEntity(this WorkflowModel model)
        {
            var entity = new Workflow()
            {
                Id = model.Id,
                Name = model.Name,
                Desc = model.Desc,
                Type = model.Type == WorkflowType.None ? (int) WorkflowType.AskLeave : (int) model.Type,
                Condition = model.Condition,
                ConditionDesc = model.ConditionDesc,
                CreatedTime = model.CreatedTime
            };

            entity.Status = true;
            if (model.Status == WorkflowStatus.Disabled)
            {
                entity.Status = false;
            }

            return entity;
        }

        public static WorkflowModel ToModel(this Workflow entity)
        {
            var model = new WorkflowModel()
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Desc = entity.Desc ?? string.Empty,
                CreatedTime = entity.CreatedTime,
                Type = (WorkflowType) entity.Type,
                Status = entity.Status ? WorkflowStatus.Enabled : WorkflowStatus.Disabled,
                Condition = entity.Condition ?? string.Empty,
                ConditionDesc = entity.ConditionDesc ?? string.Empty,
                WorkflowSteps = new List<WorkflowStepModel>()
            };

            //获取流程步骤
            if (entity.WorkflowSteps != null && entity.WorkflowSteps.Count > 0)
            {
                foreach (var step in entity.WorkflowSteps)
                {
                    WorkflowStepModel stepModel = new WorkflowStepModel()
                    {
                        Id = step.Id,
                        FlowId = step.FlowId,
                        Name = step.Name,
                        MinTimes = step.MinTimes,
                        MaxTimes = step.MaxTimes,
                        NextStep = step.NextStep.HasValue ? step.NextStep.Value : 0,
                        Desc = step.Desc,
                        CreatedTime = step.CreatedTime,
                        Operator = step.Operator,
                        OperatorType = (WorkflowOperator)step.OperatorType,
                        Type = step.Type.HasValue ? (WorkflowStepType) step.Type.Value : WorkflowStepType.LeaderApprove
                    };

                    if (stepModel.MinTimes == 0) stepModel.MinTimes = Int16.MinValue;
                    if (stepModel.MaxTimes == 0) stepModel.MaxTimes = Int16.MaxValue;

                    model.WorkflowSteps.Add(stepModel);
                }
            }

            return model;
        }

        #region 默认节点
        /// <summary>
        /// 直接领导审批节点(默认节点)
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public static WorkflowStepModel GetDirectSupervisorApproveStep(this WorkflowModel workflow)
        {
            return new WorkflowStepModel()
            {
                Id = Constant.WORKFLOW_DIRECT_SUPERVISOR_APPROVE_NODE,
                FlowId = workflow.Id,
                Name = "直接领导审批(默认节点)",
                Desc = "流程申请后第一步，直接领导审批",
                OperatorType = WorkflowOperator.Role,
                Type = WorkflowStepType.LeaderApprove,
                PrevStep = Constant.WORKFLOW_APPLY_NODE,
                CreatedTime = DateTime.Now
            };
        }

        /// <summary>
        /// 用户申请节点(默认节点)
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns></returns>
        public static WorkflowStepModel GetUserApplyStep(this WorkflowModel workflow)
        {
            return new WorkflowStepModel()
            {
                Id = Constant.WORKFLOW_APPLY_NODE,
                FlowId = workflow.Id,
                Name = "用户申请节点(默认节点)",
                Desc = "用户申请节点",
                OperatorType = WorkflowOperator.User,
                Type = WorkflowStepType.UserApply,
                CreatedTime = DateTime.Now
            };
        }
        #endregion

        /// <summary>
        /// 流程是否需要行政审批
        /// </summary>
        /// <param name="model">流程</param>
        /// <returns>true or false</returns>
        public static bool NeedAdminApprove(this WorkflowModel model)
        {
            if (model == null || model.WorkflowSteps == null || model.WorkflowSteps.Count < 1)
            {
                throw new KeyNotFoundException("Invalid order workflow.");
            }

            var step = model.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.AdminReview);

            return step != null;
        }

        /// <summary>
        /// 流程是否需要财务审批
        /// </summary>
        /// <param name="model">流程</param>
        /// <returns>true or false</returns>
        public static bool NeedFinanceApprove(this WorkflowModel model)
        {
            if (model == null || model.WorkflowSteps == null || model.WorkflowSteps.Count < 1)
            {
                throw new KeyNotFoundException("Invalid order workflow.");
            }

            var step = model.WorkflowSteps.FirstOrDefault(it => it.Type == WorkflowStepType.FinanceReview);

            return step != null;
        }
    }

}
