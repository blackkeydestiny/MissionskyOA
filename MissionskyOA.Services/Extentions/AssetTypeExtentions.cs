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
    public static class AssetTypeExtentions
    {

        public static AssetType ToEntity(this AssetTypeModel model)
        {
            var entity = new AssetType()
            {
                Id = model.Id,
                Name = model.Name,
                Sort = model.Sort
            };
            return entity;
        }

        public static AssetTypeModel ToModel(this AssetType entity)
        {
            var model = new AssetTypeModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Sort = entity.Sort
            };
            if (entity.AssetTypeAttributes != null && entity.AssetTypeAttributes.Count > 0)
            {
                model.Attributes = new List<AssetAttributeModel>();
                entity.AssetTypeAttributes.OrderBy(it => it.AssetAttribute.Sort).ToList().ForEach(it =>
                    {
                        model.Attributes.Add(it.AssetAttribute.ToModel());
                    });
            }
            return model;
        }


    }
}
