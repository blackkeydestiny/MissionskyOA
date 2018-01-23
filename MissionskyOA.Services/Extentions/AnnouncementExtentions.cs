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
    public static class AnnouncementExtentions
    {
        public static Announcement ToEntity(this AnnouncementModel model)
        {
            var entity = new Announcement()
            {
                Id=model.Id,
                Type = (int)model.Type,
                Title = model.Title,
                Content = model.Content,
                ApplyUserId = model.ApplyUserId,
                EffectiveDays = model.EffectiveDays,
                CreateUserId = model.CreateUserId,
                CreatedTime = model.CreatedTime,
                Status=(int)model.Status,
                AuditReason=model.AuditReason,
                RefAssetInventoryId = model.RefAssetInventoryId
            };

            return entity;
        }

        public static AnnouncementModel ToModel(this Announcement entity)
        {
            var model = new AnnouncementModel()
            {
                Id = entity.Id,
                Type = (AnnouncementType)entity.Type,
                Title = entity.Title,
                Content = entity.Content,
                ApplyUserId = entity.ApplyUserId,
                EffectiveDays = entity.EffectiveDays,
                CreateUserId = entity.CreateUserId,
                CreatedTime = entity.CreatedTime,
                Status = (AnnouncementStatus)entity.Status,
                AuditReason = entity.AuditReason,
                RefAssetInventoryId=entity.RefAssetInventoryId
            };
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntityForApplyUser = dbContext.Users.FirstOrDefault(it => it.Id == entity.ApplyUserId);
                if (userEntityForApplyUser != null)
                {
                    model.ApplyUserName = userEntityForApplyUser.EnglishName;
                }

                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.CreateUserId);
                if (userEntity != null)
                {
                    model.CreateUserName = userEntity.EnglishName;
                }
            }
            return model;
        }
    }
}
