using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;
using System.Collections.ObjectModel;
using MissionskyOA.Core.Common;

namespace MissionskyOA.Services
{
    public static class ExpenseMainExtentions
    {
        public static ExpenseMain ToEntity(this ExpenseMainModel model)
        {
            var entity = new ExpenseMain()
            {
                AuditId = model.AuditId,
                DeptNo = model.DeptNo,
                ProjNo = model.ProjNo,
                Amount = model.Amount,
                Reason = model.Reason,
                ApplyUserId = model.ApplyUserId,
                PrintForm = model.PrintForm,
                ConfirmForm = model.ConfirmForm,
                CreatedTime = DateTime.Now
            };
            Collection<ExpenseDetail> expenseDetailsResult = new Collection<ExpenseDetail>();

            if (model.ExpenseDetails != null && model.ExpenseDetails.Count > 0)
            {
                foreach (ExpenseDetailModel item in model.ExpenseDetails)
                {
                    expenseDetailsResult.Add(item.ToEntity());
                }

            }
            entity.ExpenseDetails = expenseDetailsResult;

            Collection<ExpenseAuditHistory> AuditHistoriesResult = new Collection<ExpenseAuditHistory>();

            if (model.ExpenseAuditHistories != null&&model.ExpenseAuditHistories.Count>0)
            {
                foreach (ExpenseAuditHistoryModel item in model.ExpenseAuditHistories)
                {
                    AuditHistoriesResult.Add(item.ToEntity());
                }       
            }
            entity.ExpenseAuditHistories = AuditHistoriesResult;
            return entity;
        }

        public static ExpenseMainModel ToModel(this ExpenseMain entity)
        {
            var model = new ExpenseMainModel()
            {
                Id=entity.Id,
                AuditId = entity.AuditId,
                DeptNo = entity.DeptNo,
                ProjNo = entity.ProjNo,
                Amount = entity.Amount,
                Reason = entity.Reason,
                PrintForm = entity.PrintForm.HasValue ? entity.PrintForm.Value : 0,
                ConfirmForm = entity.ConfirmForm.HasValue ? entity.ConfirmForm.Value : false,
                CreatedTime = entity.CreatedTime,
                ApplyUserId=entity.ApplyUserId,
            };
            if(entity.Department==null)
            {
                model.Department = null;
            }
            else
            {
                model.Department = entity.Department.ToModel();
            }

            if (entity.Project == null)
            {
                model.Project = null;
            }
            else
            {
                model.Project = entity.Project.ToModel();
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.ApplyUserId);
                if (userEntity != null)
                {
                    model.ApplyUserName = userEntity.EnglishName;
                }
            }

            return model;
        }

        public static ExpenseMainModel ToModelWithAuditHistory(this ExpenseMain entity)
        {
            var model = new ExpenseMainModel()
            {
                Id = entity.Id,
                AuditId = entity.AuditId,
                DeptNo = entity.DeptNo,
                ProjNo = entity.ProjNo,
                Amount = entity.Amount,
                Reason = entity.Reason,
                CreatedTime = entity.CreatedTime,
                ApplyUserId = entity.ApplyUserId,
                PrintForm = entity.PrintForm.HasValue ? entity.PrintForm.Value : 0,
                ConfirmForm = entity.ConfirmForm.HasValue ? entity.ConfirmForm.Value : false
            };

            if (entity.Department == null)
            {
                model.Department = null;
            }
            else
            {
                model.Department = entity.Department.ToModel();
            }

            if (entity.Project == null)
            {
                model.Project = null;
            }
            else
            {
                model.Project = entity.Project.ToModel();
            }
            using (var dbContext = new MissionskyOAEntities())
            {
                //Get english name
                var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == entity.ApplyUserId);
                if (userEntity != null)
                {
                    model.ApplyUserName = userEntity.EnglishName;
                }
            }

            //set current status
            using (var dbContext = new MissionskyOAEntities())
            {
                var financialUserEntity =
                    dbContext.Users.FirstOrDefault(
                        it => it.Email != null && it.Email.ToLower().Contains(Global.FinancialEmail));

                var expenseHistory = (entity.AuditId == (int)ExpenseAuditStep.FinacialAudit)
                    ? dbContext.ExpenseAuditHistories.FirstOrDefault(it => it.ExpenseId == entity.Id && it.CurrentAudit == financialUserEntity.Id)
                    : dbContext.ExpenseAuditHistories.FirstOrDefault(it => it.ExpenseId == entity.Id);


                model.currentAuditStatus = expenseHistory == null ? null : expenseHistory.ToModel();
            }

            List<ExpenseAuditHistoryModel> AuditHistoriesResult = new List<ExpenseAuditHistoryModel>();

            if (entity.ExpenseAuditHistories != null && entity.ExpenseAuditHistories.Count > 0)
            {
                foreach (ExpenseAuditHistory item in entity.ExpenseAuditHistories)
                {
                    if(item.Status!=(int)OrderStatus.Approving&&item.Status!=(int)OrderStatus.Apply)
                    {
                        AuditHistoriesResult.Add(item.ToModel());
                    }
                }
            }
            model.ExpenseAuditHistories = AuditHistoriesResult;

            return model;
        }
    }
}
