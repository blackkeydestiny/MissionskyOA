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
    public static class AcceptProxyExtensions
    {
        public static AcceptProxy ToEntity(this AcceptProxyModel model)
        {
            var entity = new AcceptProxy()
            {
                Id=model.Id,
                Date=model.Date,
                Description=model.Description,
                FileName = model.FileName,
                Content = model.Content,
                AcceptUserId=model.AcceptUserId,
                Status=(int)model.Status,
                Courier=model.Courier,
                LeaveMessage=model.LeaveMessage,
                CourierPhone= model.CourierPhone,
                CreateUserId=model.CreateUserId,
                LastModifyUserId=model.LastModifyUserId,
                LastModifyTime = model.LastModifyTime
            };

            return entity;
        }

        public static AcceptProxyModel ToModel(this AcceptProxy entity)
        {
            var model = new AcceptProxyModel()
            {
                Id = entity.Id,
                Date = entity.Date,
                Description = entity.Description,
                FileName = entity.FileName,
                Content = null,
                AcceptUserId = entity.AcceptUserId,
                Status = (ExpressStatus)entity.Status,
                Courier = entity.Courier,
                LeaveMessage = entity.LeaveMessage,
                CourierPhone = entity.CourierPhone,
                CreateUserId = entity.CreateUserId,
                LastModifyUserId = entity.LastModifyUserId,
                LastModifyTime = entity.LastModifyTime
            };
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.AcceptUserId);
                if (userEntity != null)
                {
                    model.AcceptUserName = userEntity.EnglishName;
                }

                userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.CreateUserId);
                if (userEntity != null)
                {
                    model.CreateUserName = userEntity.EnglishName;
                }
            }

            return model;
        }
    }
}
