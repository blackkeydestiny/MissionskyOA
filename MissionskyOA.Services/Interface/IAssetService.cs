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
    /// 资产接口
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// 添加资产
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        AssetModel Add(AssetModel model);

        /// <summary>
        /// 导入资产
        /// </summary>
        /// <param name="list"></param>
        /// <param name="codePrefix">编号固定字符</param>
        /// <returns></returns>
        string Import(List<AssetModel> list, string codePrefix);

        /// <summary>
        /// 删除资产
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Remove(int id);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool BatchRemove(int[] ids);

        /// <summary>
        /// 更新资产
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update(AssetModel model);

        /// <summary>
        /// 获取资产列表
        /// </summary>
        List<AssetModel> List(AssetSearchModel search);

        /// <summary>
        /// 获取我的资产列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<AssetModel> MyList(int currentUserId);

        /// <summary>
        /// 查找资产
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="unIncludeUserId">查找公司资产,不包括属于当前用户的资产(当前登录用户Id)</param>
        /// <returns></returns>
        IPagedList<AssetModel> SearchAssets(AssetSearchModel search, int pageIndex, int pageSize, int? unIncludeUserId);

        /// <summary>
        /// 获取单个资产
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetModel GetAssetById(int id);

        /// <summary>
        /// 根据编码获取资产信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        AssetModel GetAssetByBarcode(string barcode);
    }
}
