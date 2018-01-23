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
    public static class AuditMessageExtentions
    {
        public static AuditMessage ToEntity(this AuditMessageModel model)
        {
            var entity = new AuditMessage()
            {
                Id = model.Id,
                Type = (int)model.Type,
                Message = model.Message,
                UserId = model.UserId,
                Status = (int)model.Status,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static AuditMessageModel ToModel(this AuditMessage entity)
        {
            var model = new AuditMessageModel()
            {
                Id = entity.Id,
                Type = (AuditMessageType) entity.Type,
                Message = entity.Message,
                UserId = entity.UserId,
                Status = !entity.Status.HasValue ? AuditMessageStatus.Unread : (AuditMessageStatus) entity.Status.Value,
                CreatedTime = entity.CreatedTime.HasValue ? entity.CreatedTime.Value : DateTime.MinValue
            };
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.UserId);
                if (userEntity != null)
                {
                    model.UserEnglishName = userEntity.EnglishName;
                }
            }
            

            return model;
        }
    }
}
