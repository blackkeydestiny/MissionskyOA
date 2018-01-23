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
    public static class ExpenseAuditHistoryExtentions
    {
        public static ExpenseAuditHistory ToEntity(this ExpenseAuditHistoryModel model)
        {
            var entity = new ExpenseAuditHistory()
            {       
                CurrentAudit = model.CurrentAudit,
                NextAudit = model.NextAudit,
                ExpenseId = model.ExpenseId,
                Status =(int) model.Status,
                AuditMessage = model.AuditMessage,
                CreatedTime = model.CreatedTime,
            };
            return entity;
        }

        public static ExpenseAuditHistoryModel ToModel(this ExpenseAuditHistory entity)
        {
            var model = new ExpenseAuditHistoryModel()
            {
                Id=entity.Id,
                CurrentAudit = entity.CurrentAudit,
                NextAudit = entity.NextAudit,
                ExpenseId = entity.ExpenseId,
                Status = (OrderStatus)entity.Status,
                AuditMessage = entity.AuditMessage,
                CreatedTime = entity.CreatedTime,
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.CurrentAudit);
                if (userEntity != null)
                {
                    model.CurrentAuditName = userEntity.EnglishName;
                }
                userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.NextAudit);
                if (userEntity != null)
                {
                    model.NextAuditName = userEntity.EnglishName;
                }
            }
            return model;
        }

        public static ExpenseAuditHistoryModel ToModelWithExpenseMainInfo(this ExpenseAuditHistory entity)
        {
            var model = new ExpenseAuditHistoryModel()
            {
                Id = entity.Id,
                CurrentAudit = entity.CurrentAudit,
                NextAudit = entity.NextAudit,
                ExpenseId = entity.ExpenseId,
                Status = (OrderStatus)entity.Status,
                AuditMessage = entity.AuditMessage,
                ExpenseMain=entity.ExpenseMain.ToModel(),
                CreatedTime = entity.CreatedTime,
            };

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.CurrentAudit);
                if (userEntity != null)
                {
                    model.CurrentAuditName = userEntity.EnglishName;
                }
                userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.NextAudit);
                if (userEntity != null)
                {
                    model.NextAuditName = userEntity.EnglishName;
                }
            }
            return model;
        }
    }
}
