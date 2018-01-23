using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 数据字典扩展处理
    /// </summary>
    public static class DataDictExtensions
    {
        public static DataDictModel ToModel(this DataDict entity)
        {
            var model = new DataDictModel()
            {
                Id = entity.Id,
                //Type = entity.Type,
                Value = entity.Value,
                Text = entity.Text,
                Parent = entity.Parent
            };

            return model;
        }

        public static DataDict ToEntity(this DataDictModel model)
        {
            var entity = new DataDict()
            {
                Id = model.Id,
                //Type = model.Type,
                Value = model.Value,
                Text = model.Text,
                Parent = model.Parent
            };

            return entity;
        }
    }
}
