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
    public static class ScheduledTaskExtentions
    {
        public static ScheduledTask ToEntity(this ScheduledTaskModel model)
        {
            var entity = new ScheduledTask()
            {
                Id = model.Id,
                Name = model.Name,
                Desc = model.Desc,
                Interval = model.Interval,
                Status =
                    model.Status == ScheduledTaskStatus.None ? (int) ScheduledTaskStatus.Started : (int) model.Status,
                Unit = model.Unit == ScheduledTaskUnit.None ? (int) ScheduledTaskUnit.Second : (int) model.Unit,
                CreatedTime = model.CreatedTime,
                LastExecTime = model.LastExecTime,
                TaskClass = model.TaskClass
            };

            return entity;
        }

        public static ScheduledTaskModel ToModel(this ScheduledTask entity)
        {
            var model = new ScheduledTaskModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Desc = entity.Desc,
                Interval = entity.Interval,
                Status = (ScheduledTaskStatus)entity.Status,
                Unit = (ScheduledTaskUnit)entity.Unit,
                CreatedTime = entity.CreatedTime,
                LastExecTime = entity.LastExecTime,
                TaskClass = entity.TaskClass
            };

            return model;
        }
    }
}
