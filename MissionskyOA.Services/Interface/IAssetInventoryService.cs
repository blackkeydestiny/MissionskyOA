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
    /// 资产盘点接口
    /// </summary>
    public interface IAssetInventoryService
    {
        /// <summary>
        /// 添加盘点任务
        /// </summary>
        /// <param name="model">返回保存后的实体Id</param>
        /// <returns></returns>
        int AddAssetInventory(AssetInventoryModel model);

        /// <summary>
        /// 更新资产盘点任务状态
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool UpdateAssetInventoryStatus(int inventoryId, AssetInventoryStatus status);

        /// <summary>
        /// 提交用户扫描的盘点数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="inventoryId">盘点任务Id</param>
        /// <param name="userId">扫描条码的当前用户</param>
        /// <returns></returns>
        bool SubmitInventoryInfo(List<AssetInventoryRecordModel> model, int inventoryId, int userId);

        /// <summary>
        /// 获取所有盘点任务
        /// </summary>
        /// <returns></returns>
        List<AssetInventoryModel> GetInventories();

        /// <summary>
        /// 获取某次盘点任务对象
        /// </summary>
        /// <param name="inventoryId">盘点任务Id</param>
        /// <returns></returns>
        AssetInventoryModel GetAssetInventory(int inventoryId);

        /// <summary>
        /// 获取某次盘点任务对象
        /// </summary>
        /// <param name="inventoryId">盘点任务Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        AssetInventoryModel GetAssetInventory(int inventoryId, int userId);
    }
}
