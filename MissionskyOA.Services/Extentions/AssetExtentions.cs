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
    public static class AssetExtentions
    {

        public static Asset ToEntity(this AssetModel model)
        {
            var entity = new Asset()
            {
                Id = model.Id,
                TypeId = model.TypeId,
                UserId = model.UserId,
                Status = (int)model.Status
            };
            return entity;
        }

        public static AssetModel ToModel(this Asset entity, MissionskyOAEntities dbContext)
        {
            var model = new AssetModel()
            {
                Id = entity.Id,
                TypeId = entity.TypeId,
                TypeName = entity.AssetType.Name,
                UserId = entity.UserId,
                UserName = entity.User.EnglishName,
            };
            if (entity.Status.HasValue)
            {
                model.Status = (AssetStatus)entity.Status.Value;
                switch (model.Status)
                {
                    case AssetStatus.Normal:
                        model.StatusName = "正常";
                        break;
                    case AssetStatus.Lose:
                        model.StatusName = "丢失";
                        break;
                    case AssetStatus.Scrapped:
                        model.StatusName = "报废";
                        break;
                    case AssetStatus.WaitOut:
                        model.StatusName = "待转出";
                        break;
                    case AssetStatus.WaitIn:
                        model.StatusName = "待转入";
                        break;
                    case AssetStatus.Idle:
                        model.StatusName = "闲置";
                        break;
                    default:
                        break;
                }
            }
            if (entity.AssetInfoes != null && entity.AssetInfoes.Count > 0)
            {
                model.AssetInfoes = new List<AssetInfoModel>();
                foreach (var item in entity.AssetInfoes.OrderBy(it => it.AssetAttribute.Sort))
                {
                    model.AssetInfoes.Add(new AssetInfoModel()
                        {
                            AttributeId = item.AttributeId,
                            AttributeValue = item.AttributeValue,
                            AttributeName = item.AssetAttribute.Name
                        });
                }
            }
            if (entity.AssetTransactions != null && entity.AssetTransactions.Count > 0)
            {
                var latest = entity.AssetTransactions.OrderByDescending(it => it.CreatedTime).FirstOrDefault();
                if (latest != null)
                {
                    model.TransactionId = latest.Id;
                }
            }
            return model;
        }

    }
}
