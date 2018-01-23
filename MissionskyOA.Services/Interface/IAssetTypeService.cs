using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 资产类别接口
    /// </summary>
    public interface IAssetTypeService
    {
        /// <summary>
        /// 添加资产类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AssetTypeModel Add(AssetTypeModel model);

        /// <summary>
        /// 删除资产类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Remove(int id);

        /// <summary>
        /// 更新资产类别
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update(AssetTypeModel model);

        /// <summary>
        /// 设置分类属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SetAttributes(AssetTypeModel model);

        /// <summary>
        /// 分页获取资产类别列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        ListResult<AssetTypeModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns></returns>
        List<AssetTypeModel> GetAll();

        /// <summary>
        /// 获取单个资产类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetTypeModel GetTypeById(int id);
    }
}
