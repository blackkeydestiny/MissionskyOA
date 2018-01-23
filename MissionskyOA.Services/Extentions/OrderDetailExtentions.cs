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
    public static class OrderDetailExtentions
    {
        public static OrderDet ToEntity(this OrderDetModel model)
        {
            var entity = new OrderDet()
            {
                Id = model.Id,
                OrderId=model.Id,
                StartDate=model.StartDate,
                StartTime=model.StartTime,
                EndDate=model.EndDate,
                EndTime=model.EndTime,
                IOHours=model.IOHours,
               
                Description=model.Description,
                Audit = model.Audit,
                InformLeader = model.InformLeader,
                WorkTransfer = model.WorkTransfer,
                Recipient = model.Recipient
            };
            return entity;
        }

        public static OrderDetModel ToModel(this OrderDet entity)
        {
            var model = new OrderDetModel()
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                StartDate = entity.StartDate,
                StartTime = entity.StartTime,
                EndDate = entity.EndDate,
                EndTime = entity.EndTime,
                IOHours = entity.IOHours,
           
                Description = entity.Description,
                Audit = entity.Audit ?? string.Empty,
                InformLeader = entity.InformLeader ?? false,
                WorkTransfer = entity.WorkTransfer ?? false,
                Recipient = entity.Recipient.HasValue ? entity.Recipient.Value : 0,
            };
            using (var dbContext = new MissionskyOAEntities())
            {
                UserService userServer = new UserService();
                if (entity.Recipient != null)
                    {
                        var RecipientUser = userServer.GetUserDetail(entity.Recipient.Value);
	                    model.RecipientName = RecipientUser == null ? string.Empty : RecipientUser.EnglishName;
                    }
            }
            
            model.StartDate = Convert.ToDateTime(entity.StartDate.ToShortDateString() +" "+ entity.StartTime.ToString());
            model.EndDate = Convert.ToDateTime(entity.EndDate.ToShortDateString() +" "+ entity.EndTime.ToString());
            return model;
        }
    }
}
