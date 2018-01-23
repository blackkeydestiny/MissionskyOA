using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Common;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 报表处理
    /// </summary>
    public class ReportService : ServiceBase, IReportService
    {
        /// <summary>
        /// 获取用户可以使用的报表
        /// </summary>
        /// <param name="token">用户Token</param>
        /// <returns>用户可以使用的报表</returns>
        public IList<ReportModel> GetUserReports(string token)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var isAssetAdmin = false; //是否资产管理员

                #region 用户权限
                var user =
                    dbContext.Users.FirstOrDefault(
                        it =>
                            !string.IsNullOrEmpty(it.Token) &&
                            it.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase));

                var userReports = new List<ReportModel>();
                var allReports = new List<ReportModel>(); 

                if (user == null) //用户
                {
                    Log.Error("找不到用户。");
                    throw new KeyNotFoundException("找不到用户。");
                }

                var sql = string.Format("SELECT R.* FROM [Role] R INNER JOIN [UserRole] UR ON R.Id = UR.RoleId INNER JOIN [User] U ON UR.UserId = U.Id WHERE U.Id = {0}", user.Id);
                var roles = dbContext.Roles.SqlQuery(sql).ToList();

                //是否资产管理员
                isAssetAdmin =
                    roles.Any(
                        it => !string.IsNullOrEmpty(it.Name) && it.Name.Equals(Constant.ROLE_ASSET_ADMIN, StringComparison.InvariantCultureIgnoreCase));

                #endregion

                dbContext.Reports.Where(it => it.IsOpen).ToList().ForEach(it => allReports.Add(it.ToModel()));

                #region 过滤报表
                foreach (var report in allReports)
                {
                    //无权使用资产管理报表
                    if (!string.IsNullOrEmpty(report.Name) &&
                        (report.Name.Contains("资产") || report.Name.ToLower().Contains("asset")) && !isAssetAdmin)
                    {
                        continue;
                    }

                    userReports.Add(report);
                }
                #endregion

                return userReports;
            }
        }

        /// <summary>
        /// 获取报表服务配置
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表详细配置</returns>
        public ReportModel GetReportDetail(int reportId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Reports.FirstOrDefault(it => it.Id == reportId);

                if (entity != null)
                {
                    var report = entity.ToModel();

                    #region 获取报表参数
                    if (report.Parameters == null || report.Parameters.Count < 1)
                    {
                        report.Parameters = report.Parameters ?? new List<ReportParameterModel>();

                        dbContext.ReportParameters.Where(it => it.ReportId == reportId)
                            .ToList()
                            .ForEach(it => report.Parameters.Add(it.ToModel()));
                    }

                    FillParameterDataSource(dbContext, report.Parameters); //填充报表参数数据源
                    #endregion

                    //获取报表附件可用格式
                    report.Formats = GetReportFormats();

                    return report;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取报表服务配置
        /// </summary>
        /// <param name="reportNo">报表编号</param>
        /// <returns>报表详细配置</returns>
        public ReportModel GetReportDetail(string reportNo)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Reports.FirstOrDefault(it => !string.IsNullOrEmpty(it.No) && it.No == reportNo);

                if (entity != null)
                {
                    var report = entity.ToModel();

                    #region 获取报表参数
                    if (report.Parameters == null || report.Parameters.Count < 1)
                    {
                        report.Parameters = report.Parameters ?? new List<ReportParameterModel>();

                        dbContext.ReportParameters.Where(it => it.ReportId == entity.Id)
                            .ToList()
                            .ForEach(it => report.Parameters.Add(it.ToModel()));
                    }

                    FillParameterDataSource(dbContext, report.Parameters); //填充报表参数数据源
                    #endregion

                    //获取报表附件可用格式
                    report.Formats = GetReportFormats();

                    return report;
                }

                return null;
            }
        }

        /// <summary>
        /// 添加报表
        /// </summary>
        /// <param name="report">报表</param>
        /// <param name="configs">报表配置</param>
        /// <returns>report id</returns>
        public int AddReport(ReportModel report, NameValueCollection configs)
        {
            if (report == null || configs == null)
            {
                Log.Error("添加报表异常。");
                throw new InvalidOperationException("添加报表异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidReport(dbContext, report);

                var createdTime = DateTime.Now;

                //添加报表
                var entity = report.ToEntity();
                entity.CreatedTime = createdTime;
                dbContext.Reports.Add(entity);

                foreach (var key in configs.AllKeys)
                {
                    var config = new ReportConfig()
                    {
                        ReportId = entity.Id,
                        Config = key,
                        Value = configs[key],
                        CreatedTime = createdTime
                    };

                    dbContext.ReportConfigs.Add(config);
                }

                dbContext.SaveChanges(); //更新数据库

                return entity.Id;
            }
        }

        /// <summary>
        /// 更新报表
        /// </summary>
        /// <param name="report">报表</param>
        /// <param name="configs">报表配置</param>
        /// <returns>true or exception</returns>
        public bool UpdateReport(ReportModel report, NameValueCollection configs)
        {
            if (report == null || configs == null)
            {
                Log.Error("更新报表异常。");
                throw new InvalidOperationException("更新报表异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidReport(dbContext, report);

                var oldReport = dbContext.Reports.FirstOrDefault(it => it.Id == report.Id);
                if (oldReport == null || oldReport.ReportConfigs == null)
                {
                    Log.Error(string.Format("找不到报表，报表Id: {0}", report.Id));
                    throw new InvalidOperationException(string.Format("找不到报表，报表Id: {0}", report.Id));
                }

                //更新报表
                if (oldReport.Name == null || !oldReport.Name.Equals(report.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldReport.Name = report.Name;
                }

                if (oldReport.Desc == null || !oldReport.Desc.Equals(report.Desc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldReport.Desc = report.Desc;
                }

                //更新配置
                foreach (var config in oldReport.ReportConfigs)
                {
                    if (configs.AllKeys.Contains(config.Config) &&
                        !config.Value.Equals(configs[config.Config], StringComparison.InvariantCultureIgnoreCase))
                    {
                        config.Value = configs[config.Config];
                    }
                }

                dbContext.SaveChanges(); //更新数据库

                return true;
            }
        }

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="id">报表id</param>
        /// <returns>是否更新成功</returns>
        public bool Delete(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.Reports.FirstOrDefault(it => it.Id == id);

                if (entity == null)
                {
                    Log.Error("找不到报表。");
                    throw new KeyNotFoundException("找不到报表。");
                }

                //删除配置
                if (entity.ReportConfigs != null)
                {
                    entity.ReportConfigs.ToList().ForEach(it => dbcontext.ReportConfigs.Remove(it));
                }

                //删除参数
                if (entity.ReportParameters != null)
                {
                    entity.ReportParameters.ToList().ForEach(it => dbcontext.ReportParameters.Remove(it));
                }

                dbcontext.Reports.Remove(entity);
                dbcontext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 添加报表参数
        /// </summary>
        /// <param name="param">报表参数</param>
        /// <returns>report id</returns>
        public int AddParameter(ReportParameterModel param)
        {
            if (param == null)
            {
                Log.Error("添加报表参数异常。");
                throw new InvalidOperationException("添加报表参数异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidReportParameter(dbContext, param);

                //添加报表参数
                var entity = param.ToEntity();
                entity.CreatedTime = DateTime.Now;
                dbContext.ReportParameters.Add(entity);
                dbContext.SaveChanges(); //更新数据库

                return entity.Id;
            }
        }

        /// <summary>
        /// 更新报表参数
        /// </summary>
        /// <param name="param">报表参数</param>
        /// <returns>true or exception</returns>
        public bool UpdateParameter(ReportParameterModel param)
        {
            if (param == null)
            {
                Log.Error("更新报表参数异常。");
                throw new InvalidOperationException("更新报表参数异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidReportParameter(dbContext, param);

                var oldParameter = dbContext.ReportParameters.FirstOrDefault(it => it.Id == param.Id);
                if (oldParameter == null)
                {
                    Log.Error(string.Format("找不到报表参数，报表参数Id: {0}", param.Id));
                    throw new InvalidOperationException(string.Format("找不到报表参数，报表参数Id: {0}", param.Id));
                }

                //更新报表参数
                if (!oldParameter.Name.Equals(param.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldParameter.Name = param.Name;
                }

                if (string.IsNullOrEmpty(oldParameter.Desc) || !oldParameter.Desc.Equals(param.Desc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldParameter.Desc = param.Desc;
                }

                if (!oldParameter.Type.Equals(param.Type, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldParameter.Type = param.Type;
                }

                if (string.IsNullOrEmpty(oldParameter.DataSource) || !oldParameter.DataSource.Equals(param.DataTable, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldParameter.DataSource = param.DataTable;
                }

                if (!oldParameter.Nullable.HasValue || oldParameter.Nullable != param.Nullable)
                {
                    oldParameter.Nullable = param.Nullable;
                }
                
                dbContext.SaveChanges(); //更新数据库

                return true;
            }
        }

        /// <summary>
        /// 删除报表参数
        /// </summary>
        /// <param name="id">报表参数id</param>
        /// <returns>是否更新成功</returns>
        public bool DeleteParameter(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.ReportParameters.FirstOrDefault(it => it.Id == id);

                if (entity == null)
                {
                    Log.Error("找不到报表参数。");
                    throw new KeyNotFoundException("找不到报表参数。");
                }
                dbcontext.ReportParameters.Remove(entity);
                dbcontext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 获取报表参数
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表参数</returns>
        public IList<ReportParameterModel> GetReportParameters(int reportId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var parameters = new List<ReportParameterModel>();

                dbContext.ReportParameters.Where(it => it.ReportId == reportId)
                    .ToList()
                    .ForEach(it => parameters.Add(it.ToModel()));

                return parameters;
            }
        }

        /// <summary>
        /// 获取参数详细
        /// </summary>
        /// <param name="paramId">参数Id</param>
        /// <returns>参数详细</returns>
        public ReportParameterModel GetParameterDetail(int paramId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.ReportParameters.FirstOrDefault(it => it.Id == paramId);

                if (entity != null)
                {
                    return entity.ToModel();
                }

                return null;
            }
        }

        /// <summary>
        /// 获取报表配置
        /// </summary>
        /// <param name="reportId">报表Id</param>
        /// <returns>报表配置</returns>
        public NameValueCollection GetReportConfigs(int reportId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var configs = new NameValueCollection();

                dbContext.ReportConfigs.Where(it => it.ReportId == reportId)
                    .ToList()
                    .ForEach(it => configs.Add(it.Config, it.Value));

                return configs;
            }
        }

        /// <summary>
        /// 获取报表输出格式
        /// </summary>
        /// <returns>报表输出格式</returns>
        public List<string> GetReportFormats()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var formats = new List<string>();
                dbContext.DataDicts.Where(
                    it => it.Type.Equals(Constant.DATADICT_INDEX_REPORT_FORMAT, StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
                    .ForEach(it => formats.Add(it.Value));

                return formats;
            }
        }

        /// <summary>
        /// 获取报表参数类型
        /// </summary>
        /// <returns>报表参数类型</returns>
        public List<string> GetReportParameterTypes()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var paramTypes = new List<string>();
                dbContext.DataDicts.Where(
                    it => it.Type.Equals(Constant.DATADICT_INDEX_REPORT_PARAM_TYPE, StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
                    .ForEach(it => paramTypes.Add(it.Value));

                return paramTypes;
            }
        }
        
        /// <summary>
        /// 获取所有报表
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>所有报表</returns>
        public ListResult<ReportModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Reports.OrderBy(it => it.Id).AsEnumerable();

                //查询
                if (filter != null)
                {
                }

                //排序
                if (sort != null)
                {
                }

                //分页
                var count = list.Count();
                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                //转换
                ListResult<ReportModel> result = new ListResult<ReportModel>();
                result.Data = new List<ReportModel>();

                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;

                return result;
            }
        }

        /// <summary>
        /// 验证报表
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="report"></param>
        private void ValidReport(MissionskyOAEntities dbContext, ReportModel report)
        {
            var existedReport =
                dbContext.Reports.FirstOrDefault(
                    it =>
                        !string.IsNullOrEmpty(it.Name) &&
                        it.Name.Equals(report.Name, StringComparison.InvariantCultureIgnoreCase) && report.Id != it.Id);

            if (existedReport != null)
            {
                Log.Error(string.Format("报表已经存在，报表: {0}", report.Name));
                throw new InvalidOperationException(string.Format("报表已经存在，报表: {0}", report.Name));
            }
        }

        /// <summary>
        /// 验证报表参数
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="parameter"></param>
        private void ValidReportParameter(MissionskyOAEntities dbContext, ReportParameterModel parameter)
        {
            var existedReport =
                dbContext.ReportParameters.FirstOrDefault(
                    it =>
                        !string.IsNullOrEmpty(it.Name) && it.ReportId == parameter.Id &&
                        it.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase) && parameter.Id != it.Id);

            if (existedReport != null)
            {
                Log.Error(string.Format("报表参数已经存在，报表: {0}, 报表参数: {1}", parameter.Id, parameter.Name));
                throw new InvalidOperationException(string.Format("报表参数已经存在，报表: {0}, 报表参数: {1}", parameter.Id, parameter.Name));
            }
        }

        /// <summary>
        /// 填充报表参数数据源
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="parameters"></param>
        private void FillParameterDataSource(MissionskyOAEntities dbContext, IList<ReportParameterModel> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                foreach (ReportParameterModel param in parameters)
                {
                    param.DataSource = param.DataSource ?? new Dictionary<string, string>(); //数据源

                    if (param.Type.Equals(Constant.REPORT_PARAM_TYPE_DROPDOWNLIST,
                        StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(param.DataTable))
                    {
                        var sources = param.DataTable.Split(new [] {Constant.REPORT_PARAM_TABLE_SPLIT, Constant.REPORT_PARAM_FIELD_SPLIT});
                        var table = sources.Length > 0 ? sources[0] : string.Empty;
                        var keyField = sources.Length > 1 ? sources[1] : string.Empty;
                        var valueField = sources.Length > 2 ? sources[2] : keyField;
                        var defaultQuery = string.IsNullOrEmpty(keyField) || string.IsNullOrEmpty(valueField); //查询默认字段

                        if (table.Equals(Constant.TABLE_ASSETINVENTORY,
                            StringComparison.InvariantCultureIgnoreCase)) //资产盘点
                        {
                            dbContext.AssetInventories.OrderBy(it => it.Id)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Title));
                        }
                        else if (table.Equals(Constant.TABLE_ASSETTYPE,
                            StringComparison.InvariantCultureIgnoreCase)) //资产类型
                        {
                            dbContext.AssetTypes.OrderBy(it => it.Id)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Name));
                        }
                        else if (table.Equals(Constant.TABLE_DATADICT,
                            StringComparison.InvariantCultureIgnoreCase)) //数据字典
                        {
                            dbContext.DataDicts.Where(
                                it =>
                                    it.Type == Constant.DATADICT_REPORT_PARAM_DATASOURCE &&
                                    it.Value == param.Id.ToString())
                                .OrderBy(it => it.Id)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Text));
                        }
                        else if (table.Equals(Constant.TABLE_PROJECT,
                            StringComparison.InvariantCultureIgnoreCase)) //项目
                        {
                            if (defaultQuery)
                            {
                                dbContext.Projects.OrderBy(it => it.Id)
                                    .ToList()
                                    .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Name));
                            }
                            else
                            {
                                dbContext.Projects.Select(it => it.Name)
                                    .ToList()
                                    .ForEach(it => param.DataSource.Add(it, it));
                            }

                            param.DataSource.Add("All", "所有项目组");
                        }
                        else if (table.Equals(Constant.TABLE_USER,
                            StringComparison.InvariantCultureIgnoreCase)) //员工
                        {
                            dbContext.Users.Where(it => it.Available)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.EnglishName));

                            //param.DataSource =
                            //    (from item in param.DataSource orderby item.Value select item).ToDictionary(
                            //        item => item.Key, item => item.Value);
                        }
                        else if (table.Equals(Constant.TABLE_DEPARTMENT,
                            StringComparison.InvariantCultureIgnoreCase)) //部门
                        {
                            dbContext.Departments.OrderBy(it => it.Id)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Name));
                        }
                        else if (table.Equals(Constant.TABLE_ROLE,
                            StringComparison.InvariantCultureIgnoreCase)) //角色
                        {
                            dbContext.Roles.OrderBy(it => it.Id)
                                .ToList()
                                .ForEach(it => param.DataSource.Add(it.Id.ToString(), it.Name));
                        }
                    }
                }
            }
        }
    }
}
