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
    public static class AssetAttributeExtentions
    {
        public static AssetAttribute ToEntity(this AssetAttributeModel model)
        {
            var entity = new AssetAttribute()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Sort = model.Sort,
                DataType = (int)model.DataType
            };
            return entity;
        }

        public static AssetAttributeModel ToModel(this AssetAttribute entity)
        {
            var model = new AssetAttributeModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Sort = entity.Sort
            };
            if (entity.DataType.HasValue)
            {
                model.DataType = (AssetDataType)entity.DataType;
            }

            return model;
        }

    }
}
