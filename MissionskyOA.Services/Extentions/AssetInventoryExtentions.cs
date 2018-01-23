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
    public static class AssetInventoryExtentions
    {

        public static AssetInventory ToEntity(this AssetInventoryModel model)
        {
            var entity = new AssetInventory()
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Status = (int)model.Status
            };
            return entity;
        }

        public static AssetInventoryModel ToModel(this AssetInventory entity)
        {
            var model = new AssetInventoryModel()
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                CreatedTime = entity.CreatedTime
            };
            if (entity.Status.HasValue)
            {
                model.Status = (AssetInventoryStatus)entity.Status.Value;
            }
            if (entity.AssetInventoryRecords != null && entity.AssetInventoryRecords.Count > 0)
            {
                model.AssetInventoryRecords = new List<AssetInventoryRecordModel>();
                foreach (var item in entity.AssetInventoryRecords)
                {
                    model.AssetInventoryRecords.Add(new AssetInventoryRecordModel()
                        {
                            Id = item.Id,
                            AssetId = item.AssetId,
                            InventoryId = item.InventoryId,
                            ScanCode = item.ScanCode,
                            UserId = item.UserId
                        });
                }
            }
            return model;
        }

    }
}
