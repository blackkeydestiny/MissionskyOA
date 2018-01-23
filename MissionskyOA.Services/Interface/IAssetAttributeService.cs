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
    /// 资产属性接口
    /// </summary>
    public interface IAssetAttributeService
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AssetAttributeModel Add(AssetAttributeModel model);

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Remove(int id);

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update(AssetAttributeModel model);

        /// <summary>
        /// 分页获取属性列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        ListResult<AssetAttributeModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 获取所有属性
        /// </summary>
        /// <returns></returns>
        List<AssetAttributeModel> GetAll();

        /// <summary>
        /// 获取单个属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetAttributeModel GetAttributeById(int id);
    }
}
