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
    public static class AssetTransactionExtentions
    {

        public static AssetTransaction ToEntity(this AssetTransactionModel model)
        {
            var entity = new AssetTransaction()
            {
                Id = model.Id,
                AssetId = model.AssetId,
                BusinessType = (int)model.BusinessType,
                InUserId = model.InUserId,
                InUserIsConfirm = model.InUserIsConfirm,
                OutUserId = model.OutUserId,
                OutUserIsConfirm = model.OutUserIsConfirm,
                Description = model.Description
            };
            return entity;
        }

        public static AssetTransactionModel ToModel(this AssetTransaction entity)
        {
            var model = new AssetTransactionModel()
            {
                Id = entity.Id,
                AssetId = entity.AssetId,
                InUserId = entity.InUserId,
                InUserIsConfirm = entity.InUserIsConfirm,
                OutUserId = entity.OutUserId,
                OutUserIsConfirm = entity.OutUserIsConfirm,
                Description = entity.Description
            };
            if (entity.BusinessType.HasValue)
            {
                model.BusinessType = (BusinessType)entity.BusinessType.Value;
            }
            if (entity.Status.HasValue)
            {
                model.Status = (AssetTransactionStatus)entity.Status.Value;
            }

            return model;
        }

    }
}
