using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MissionskyOA.Core;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services.Extentions
{
    public static class WorkTaskExtensions
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(WorkTaskExtensions));

        public static WorkTaskModel ToModel(this WorkTask entity)
        {
            if (entity == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var model = new WorkTaskModel()
            {
                Id = entity.Id,
                Outline = entity.Outline,
                Type = (WorkTaskType)entity.Type,
                Status = (WorkTaskStatus)entity.Status,
                Desc = entity.Desc,
                MeetingId = entity.MeetingId,
                ProjectId = entity.ProjectId,
                Sponsor = entity.Sponsor,
                Supervisor = entity.Supervisor,
                Executor = entity.Executor,
                Source = (WorkTaskSource)entity.Source,
                Priority = (WorkTaskPriority)entity.Priority,
                Urgency = entity.Urgency.HasValue ? (WorkTaskUrgency)entity.Urgency.Value : WorkTaskUrgency.Low,
                Importance = entity.Importance.HasValue ? (WorkTaskImportance)entity.Importance.Value : WorkTaskImportance.Low,
                Workload = entity.Workload,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                CompleteTime = entity.CompleteTime,
                CloseTime = entity.CloseTime,
                CreatedTime = entity.CreatedTime
            };

            #region 生成任务处理列表
            model.Histories = new List<WorkTaskHistoryModel>();

            if (entity.WorkTaskHistories != null)
            {
                entity.WorkTaskHistories.ToList().ForEach(it => model.Histories.Add(it.ToModel()));
            }
            #endregion

            #region 生成评论列表
            model.Comments = new List<WorkTaskCommentModel>();

            if (entity.WorkTaskHistories != null)
            {
                entity.WorkTaskComments.ToList().ForEach(it => model.Comments.Add(it.ToModel()));
            }
            #endregion

            return model;
        }

        public static WorkTaskHistoryModel ToModel(this WorkTaskHistory entity)
        {
            if (entity == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var model = new WorkTaskHistoryModel()
            {
                Id = entity.Id,
                TaskId = entity.TaskId,
                Operator = entity.Operator,
                OperatorName = entity.OperatorName ?? string.Empty,
                Status = (WorkTaskStatus)entity.Status,
                Audit = entity.Audit,
                CreatedTime = entity.CreatedTime
            };
            
            return model;
        }

        public static WorkTaskCommentModel ToModel(this WorkTaskComment entity)
        {
            if (entity == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var model = new WorkTaskCommentModel()
            {
                Id = entity.Id,
                TaskId = entity.TaskId,
                UserId = entity.UserId,
                Comment = entity.Comment,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

        public static WorkTask ToEntity(this WorkTaskModel model)
        {
            if (model == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var entity = new WorkTask()
            {
                Id = model.Id,
                Outline = model.Outline,
                Type = (int) model.Type,
                Status = (int) model.Status,
                Desc = model.Desc,
                MeetingId = model.MeetingId,
                ProjectId = model.ProjectId,
                Sponsor = model.Sponsor,
                Supervisor = model.Supervisor,
                Executor = model.Executor,
                Source = (int) model.Source,
                Priority = (int)model.Priority,
                Urgency = (int)model.Urgency,
                Importance = (int)model.Importance,
                Workload = model.Workload,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                CompleteTime = model.CompleteTime,
                CloseTime = model.CloseTime,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static WorkTaskHistory ToEntity(this WorkTaskHistoryModel model)
        {
            if (model == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var enity = new WorkTaskHistory()
            {
                Id = model.Id,
                TaskId = model.TaskId,
                Operator = model.Operator,
                Status = (int)model.Status,
                Audit = model.Audit,
                CreatedTime = model.CreatedTime
            };

            return enity;
        }

        public static WorkTaskComment ToEntity(this WorkTaskCommentModel model)
        {
            if (model == null)
            {
                Log.Error("参数无效。");
                throw new InvalidOperationException("参数无效。");
            }

            var enity = new WorkTaskComment()
            {
                Id = model.Id,
                TaskId = model.TaskId,
                UserId = model.UserId,
                Comment = model.Comment,
                CreatedTime = model.CreatedTime
            };

            return enity;
        }

        public static WorkTask ToEntity(this NewWorkTaskModel model)
        {
            model.Valid(); //验证Task参数是否有效

            var entity = new WorkTask()
            {
                Outline = model.Outline,
                Type = (int)model.Type,
                Status = (int) WorkTaskStatus.Created,
                Desc = model.Desc,
                MeetingId = model.MeetingId,
                ProjectId = model.ProjectId,
                Sponsor = model.Sponsor,
                Supervisor = model.Supervisor,
                Executor = model.Executor,
                Source = model.Source.HasValue ? (int)model.Source.Value : (int)WorkTaskSource.Person,
                Priority = model.Priority.HasValue ? (int)model.Priority.Value : (int)WorkTaskPriority.Normal,
                Urgency = model.Urgency.HasValue ? (int)model.Urgency.Value : (int)WorkTaskUrgency.Low,
                Importance = model.Importance.HasValue ? (int)model.Importance.Value : (int)WorkTaskImportance.Low,
                EndTime = model.EndTime,
                StartTime=model.StartTime,
                CreatedTime = DateTime.Now
            };

            return entity;
        }

        public static WorkTaskModel ToModel(this NewWorkTaskModel model)
        {
            model.Valid(); //验证Task参数是否有效

            var task = new WorkTaskModel()
            {
                Outline = model.Outline,
                Type = model.Type,
                Status = WorkTaskStatus.Started,
                Desc = model.Desc,
                MeetingId = model.MeetingId,
                ProjectId = model.ProjectId,
                Sponsor = model.Sponsor,
                Supervisor = model.Supervisor,
                Executor = model.Executor,
                Source = model.Source ?? WorkTaskSource.Person,
                Priority = model.Priority ?? WorkTaskPriority.Normal,
                EndTime = model.EndTime.Value,
                StartTime=model.StartTime.Value,
                CreatedTime = DateTime.Now
            };

            return task;
        }
        
        /// <summary>
        /// 验证新的工作任务是否有效
        /// </summary>
        /// <param name="task">新的工作任务</param>
        public static void Valid(this NewWorkTaskModel task)
        {
            if (task == null)
            {
                Log.Error("任务无效");
                throw new InvalidOperationException("任务无效");
            }

            if (string.IsNullOrEmpty(task.Outline))
            {
                Log.Error("任务具体要求无效");
                throw new InvalidOperationException("任务具体要求无效");
            }

            if (task.EndTime.HasValue && task.EndTime.Value <= DateTime.Now)
            {
                Log.Error("截止日期无效");
                throw new InvalidOperationException("截止日期无效");
            }

            if (task.StartTime.HasValue && task.StartTime.Value < DateTime.Now)
            {
                Log.Error("开始日期无效");
                throw new InvalidOperationException("开始日期无效");
            }

            if (task.EndTime.HasValue && task.StartTime.HasValue && task.EndTime.Value <= task.StartTime.Value)
            {
                Log.Error("截止日期必须大于开始日期");
                throw new InvalidOperationException("截止日期必须大于开始日期");
            }

            if (task.Sponsor <= 0)
            {
                Log.Error("任务发起人无效");
                throw new InvalidOperationException("任务发起人无效");
            }

            //默认当前登录用户为监督人
            if (!task.Supervisor.HasValue || task.Supervisor.Value <= 0)
            {
                task.Supervisor = task.Sponsor;
            }
            if(task.EndTime<=DateTime.Now)
            {
                Log.Error("截止时间必须大于当前时间");
                throw new InvalidOperationException("截止时间必须大于当前时间");
            }

            //默认当前登录用户为执行人
            if (!task.Executor.HasValue || task.Executor.Value <= 0)
            {
                task.Executor = task.Sponsor;
            }

            //为空则默认通讯录个人
            if (!task.Source.HasValue || task.Source.Value == WorkTaskSource.Invalid)
            {
                task.Source = WorkTaskSource.Person;
            }

            //为空则默认一般
            if (!task.Priority.HasValue || task.Priority.Value == WorkTaskPriority.Invalid)
            {
                task.Priority = WorkTaskPriority.Normal;
            }
        }
    }
}
