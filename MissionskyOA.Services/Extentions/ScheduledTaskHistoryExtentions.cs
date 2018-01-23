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
    public static class ScheduledTaskHistoryExtentions
    {
        public static ScheduledTaskHistory ToEntity(this ScheduledTaskHistoryModel model)
        {
            var entity = new ScheduledTaskHistory()
            {
                Id = model.Id,
                Desc = model.Desc,
                Result = model.Result,
                TaskId = model.TaskId,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static ScheduledTaskHistoryModel ToModel(this ScheduledTaskHistory entity)
        {
            var model = new ScheduledTaskHistoryModel()
            {
                Id = entity.Id,
                TaskId = entity.TaskId,
                Desc = entity.Desc,
                Result = entity.Result,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }
    }
}
