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
    public static class WorkflowProcessExtentions
    {
        public static WorkflowProcess ToEntity(this WorkflowProcessModel model)
        {
            var entity = new WorkflowProcess()
            {
                Id = model.Id,
                OrderNo = model.OrderNo,
                FlowId = model.FlowId,
                StepId = model.StepId,
                Operator = model.Operator,
                Operation = model.Operation == WorkflowOperation.None ? (int) WorkflowOperation.Apply : (int) model.Operation,
                OperationDesc = model.OperationDesc,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static WorkflowProcessModel ToModel(this WorkflowProcess entity)
        {
            var model = new WorkflowProcessModel()
            {
                Id = entity.Id,
                OrderNo = entity.OrderNo,
                FlowId = entity.FlowId,
                StepId = entity.StepId,
                Operator = entity.Operator,
                Operation = (WorkflowOperation)entity.Operation,
                OperationDesc = entity.OperationDesc,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

        /// <summary>
        /// 流程审批结束
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool IsCompletedApproved(this WorkflowProcessModel process)
        {
            return (process.NextStep == null || process.NextStep.IsFinanceReviewing());
        }

        /// <summary>
        /// 流程需要继续审批
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool NeedContinueApprove(this WorkflowProcessModel process)
        {
            return (process.NextStep != null && process.NextApprover != null);
        }
    }
}
