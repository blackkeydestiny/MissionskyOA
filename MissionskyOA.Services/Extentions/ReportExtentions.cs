using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public static class ReportExtentions
    {
        public static Report ToEntity(this ReportModel model)
        {
            var entity = new Report()
            {
                Id = model.Id,
                Name = model.Name,
                No = model.No,
                IsOpen = model.IsOpen,
                Desc = model.Desc,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static ReportModel ToModel(this Report entity)
        {
            var model = new ReportModel()
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Desc = entity.Desc ?? string.Empty,
                No = entity.No,
                IsOpen = entity.IsOpen,
                CreatedTime = entity.CreatedTime
            };

            //获取报表配置
            //if (entity.ReportConfigs != null && entity.ReportConfigs.Count > 0)
            //{
            //    model.Configs = new NameValueCollection();

            //    entity.ReportConfigs.ToList().ForEach(it => model.Configs.Add(it.Config, it.Value));

            //}

            //获取报表参数
            //if (entity.ReportParameters != null && entity.ReportParameters.Count > 0)
            //{
            //    model.Parameters = new List<ReportParameterModel>();
            //    entity.ReportParameters.ToList().ForEach(it => model.Parameters.Add(it.ToModel()));
            //}

            return model;
        }

        public static ReportConfig ToEntity(this ReportConfigModel model)
        {
            var entity = new ReportConfig()
            {
                Id = model.Id,
                ReportId = model.ReportId,
                Config = model.Config,
                Value = model.Value,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static ReportConfigModel ToModel(this ReportConfig entity)
        {
            var model = new ReportConfigModel()
            {
                Id = entity.Id,
                ReportId = entity.ReportId,
                Config = entity.Config ?? string.Empty,
                Value = entity.Value ?? string.Empty,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }

        public static ReportParameter ToEntity(this ReportParameterModel model)
        {
            var entity = new ReportParameter()
            {
                Id = model.Id,
                ReportId = model.ReportId,
                Name = model.Name,
                Desc = model.Desc,
                Type = model.Type,
                DataSource = model.DataTable,
                Nullable = model.Nullable,
                CreatedTime = model.CreatedTime
            };

            return entity;
        }

        public static ReportParameterModel ToModel(this ReportParameter entity)
        {
            var model = new ReportParameterModel()
            {
                Id = entity.Id,
                ReportId = entity.ReportId,
                Name = entity.Name ?? string.Empty,
                Desc = entity.Desc ?? string.Empty,
                Type = entity.Type ?? string.Empty,
                DataTable = entity.DataSource ?? string.Empty,
                Nullable = entity.Nullable.HasValue ? entity.Nullable.Value : true,
                CreatedTime = entity.CreatedTime
            };

            return model;
        }
    }
}
