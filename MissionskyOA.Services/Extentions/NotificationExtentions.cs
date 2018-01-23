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
    public static class NotificationExtentions
    {
        public static Notification ToEntity(this NotificationModel model)
        {
            var entity = new Notification()
            {
                Id = model.Id,
                BusinessType = (int)model.BusinessType,
                Title = model.Title,
                MessageContent = model.MessageContent,
                MessageType = (int)model.MessageType,
                MessagePrams = model.MessagePrams,
                Target = model.Target,
                Scope = (int)model.Scope,
                CreatedUserId = model.CreatedUserId,
                CreatedTime = model.CreatedTime
            };
            return entity;
        }

        public static NotificationModel ToModel(this Notification entity)
        {
            var model = new NotificationModel()
            {
                Id = entity.Id,
                BusinessType = (BusinessType)entity.BusinessType,
                Title = entity.Title,
                MessageContent = entity.MessageContent,
                MessageType = (NotificationType)entity.MessageType,
                MessagePrams = entity.MessagePrams,
                Target = entity.Target,
                Scope = (NotificationScope)entity.Scope,
                CreatedUserId = entity.CreatedUserId,
                CreatedTime = entity.CreatedTime
            };
            //旧的数据库没有Title字段
            if (model.Title == null)
            {
                model.Title = entity.MessagePrams;
            }

            return model;
        }

    }
}
