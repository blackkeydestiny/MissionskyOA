using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;
using System.Collections.ObjectModel;


namespace MissionskyOA.Services
{
    public static class ExpenseDetailExtentions
    {
        public static ExpenseDetail ToEntity(this ExpenseDetailModel model)
        {
            var entity = new ExpenseDetail()
            {
                MId = model.MId,
                ODate = model.ODate,
                EType = (int)model.EType,
                Remark = model.Remark,
                PCount = model.PCount,
                Amount = model.Amount,
            };
            ICollection<ExpenseMember> result = new Collection<ExpenseMember>();

            if (model.ExpenseMembers != null && model.ExpenseMembers.Count > 0)
            {
                foreach (ExpenseMemberModel item in model.ExpenseMembers)
                {
                    result.Add(item.ToEntity());
                }
            }
            entity.ExpenseMembers = result;
            return entity;
        }

        public static ExpenseDetailModel ToModel(this ExpenseDetail entity)
        {
            var model = new ExpenseDetailModel()
            {
                Id=entity.Id,
                MId = entity.MId,
                ODate = entity.ODate,
                EType = (ExpenseType)entity.EType,
                Remark = entity.Remark,
                PCount = entity.PCount,
                Amount = entity.Amount,
                ExpenseMain = entity.ExpenseMain.ToModel(),
                ExpenseType=(ExpenseType)entity.EType
            };
            return model;
        }

        public static ExpenseDetailModel ToModelWithParticant(this ExpenseDetail entity)
        {
            var model = new ExpenseDetailModel()
            {
                Id = entity.Id,
                MId = entity.MId,
                ODate = entity.ODate,
                EType = (ExpenseType)entity.EType,
                Remark = entity.Remark,
                PCount = entity.PCount,
                Amount = entity.Amount,
                ExpenseMain = entity.ExpenseMain.ToModelWithAuditHistory(),
                ExpenseType = (ExpenseType)entity.EType
            };

            List<ExpenseMemberModel> result = new List<ExpenseMemberModel>();

            if (entity.ExpenseMembers != null && entity.ExpenseMembers.Count > 0)
            {
                foreach (ExpenseMember item in entity.ExpenseMembers)
                {
                    result.Add(item.ToModel());
                    model.ExpenseMemberNames = model.ExpenseMemberNames+ item.User.EnglishName + ",";
                }
                model.ExpenseMemberNames = model.ExpenseMemberNames.Remove(model.ExpenseMemberNames.Length - 1);
        
            }
            model.ExpenseMembers = result;
            return model;
        }
    }
}
