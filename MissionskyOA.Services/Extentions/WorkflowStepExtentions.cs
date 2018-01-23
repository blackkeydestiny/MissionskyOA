using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class WorkflowStepExtentions
    {
        public static WorkflowStep ToEntity(this WorkflowStepModel model)
        {
            var entity = new WorkflowStep()
            {
                Id = model.Id,
                FlowId = model.FlowId,
                Name = model.Name,
                MinTimes = model.MinTimes,
                MaxTimes = model.MaxTimes,
                NextStep = model.NextStep,
                Operator = model.Operator,
                OperatorType = model.OperatorType == WorkflowOperator.None ? (int)WorkflowOperator.User : (int)model.OperatorType,
                Desc = model.Desc,
                Type = model.Type == WorkflowStepType.None ? (int)WorkflowStepType.LeaderApprove : (int)model.Type,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static WorkflowStepModel ToModel(this WorkflowStep entity)
        {
            var model = new WorkflowStepModel()
            {
                Id = entity.Id,
                FlowId = entity.FlowId,
                Name = entity.Name,
                MinTimes = entity.MinTimes,
                MaxTimes = entity.MaxTimes,
                NextStep = entity.NextStep,
                Desc = entity.Desc,
                Type = entity.Type.HasValue ? (WorkflowStepType)entity.Type.Value : WorkflowStepType.LeaderApprove,
                Operator = entity.Operator,
                OperatorType = (WorkflowOperator)entity.OperatorType,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

        /// <summary>
        /// 是否是行政审阅步骤节点
        /// </summary>
        /// <param name="model">步骤</param>
        /// <returns>trur or false</returns>
        public static bool IsAdminReviewing(this WorkflowStepModel model)
        {
            if (model == null)
            {
                throw new KeyNotFoundException("Invalid order workflow.");
            }

            return model.Type == WorkflowStepType.AdminReview;
        }


        /// <summary>
        /// 是否是财务审阅步骤节点
        /// </summary>
        /// <param name="model">步骤</param>
        /// <returns>trur or false</returns>
        public static bool IsFinanceReviewing(this WorkflowStepModel model)
        {
            if (model == null)
            {
                throw new KeyNotFoundException("Invalid order workflow.");
            }

            return model.Type == WorkflowStepType.FinanceReview;
        }

        /// <summary>
        /// 是否由领导审批流程步骤
        /// </summary>
        /// <param name="model">步骤</param>
        /// <returns>trur or false</returns>
        public static bool IsLeaderApproving(this WorkflowStepModel model)
        {
            if (model == null)
            {
                throw new KeyNotFoundException("Invalid order workflow.");
            }

            return model.Type == WorkflowStepType.LeaderApprove || model.Type == WorkflowStepType.None;
        }
    }
}
