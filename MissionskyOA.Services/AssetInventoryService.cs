using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using System.Data.Entity.Validation;

namespace MissionskyOA.Services
{
    public class AssetInventoryService : IAssetInventoryService
    {
        /// <summary>
        /// 添加盘点任务
        /// </summary>
        /// <param name="model">返回保存后的实体Id</param>
        /// <returns></returns>
        public int AddAssetInventory(AssetInventoryModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var existOpenTask = dbContext.AssetInventories.Where(it => it.Status.HasValue && it.Status.Value == (int)AssetInventoryStatus.Open).FirstOrDefault();
                if (existOpenTask != null)
                {
                    throw new Exception("当前还有未关闭的盘点任务,不能启动新任务");
                }
                var entity = model.ToEntity();
                entity.CreatedTime = DateTime.Now;
                dbContext.AssetInventories.Add(entity);
                dbContext.SaveChanges();
                return entity.Id;
            }
        }

        /// <summary>
        /// 更新盘点任务状态
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateAssetInventoryStatus(int inventoryId, AssetInventoryStatus status)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetInventories.Where(it => it.Id == inventoryId).FirstOrDefault();
                if (entity != null)
                {
                    entity.Status = (int)status;
                    dbContext.SaveChanges();
                }
                return true;
            }
        }

        /// <summary>
        /// 提交用户扫描的盘点数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SubmitInventoryInfo(List<AssetInventoryRecordModel> model, int inventoryId, int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var assetInventory = dbContext.AssetInventories.Where(it => it.Id == inventoryId).FirstOrDefault();
                if (assetInventory == null)
                {
                    throw new Exception("未找到有效的盘点任务");
                }
                if (assetInventory.Status.HasValue && assetInventory.Status.Value == (int)AssetInventoryStatus.Closed)
                {
                    throw new Exception("该盘点任务已关闭,不能再提交记录");
                }
                var savedRecords = dbContext.AssetInventoryRecords.Where(it => it.UserId == userId && it.InventoryId == inventoryId).ToList();
                if (model != null && model.Count > 0)
                {
                    List<string> sourceScanCodes = savedRecords.Select(it => it.ScanCode).ToList();
                    foreach (var item in model)
                    {
                        if (!sourceScanCodes.Contains(item.ScanCode))
                        {
                            //新增的
                            dbContext.AssetInventoryRecords.Add(new AssetInventoryRecord()
                                {
                                    InventoryId = inventoryId,
                                    AssetId = item.AssetId,
                                    UserId = userId,
                                    ScanCode = item.ScanCode
                                });
                        }
                        sourceScanCodes.Remove(item.ScanCode);
                    }
                    //待删除
                    foreach (var code in sourceScanCodes)
                    {
                        var removeRecord = dbContext.AssetInventoryRecords.Where(it => it.UserId == userId
                            && it.InventoryId == inventoryId
                            && code == it.ScanCode)
                            .FirstOrDefault();
                        if (removeRecord != null)
                        {
                            dbContext.AssetInventoryRecords.Remove(removeRecord);
                        }
                    }
                }
                else
                {
                    //删除之前已经保存的
                    foreach (var item in savedRecords)
                    {
                        dbContext.AssetInventoryRecords.Remove(item);
                    }
                }
                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 获取所有盘点任务
        /// </summary>
        /// <returns></returns>
        public List<AssetInventoryModel> GetInventories()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<AssetInventoryModel>();
                dbContext.AssetInventories.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return result;
            }
        }
        /// <summary>
        /// 获取某次盘点任务对象
        /// </summary>
        /// <param name="inventoryId">盘点任务Id</param>
        /// <returns></returns>
        public AssetInventoryModel GetAssetInventory(int inventoryId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetInventories.Include(it => it.AssetInventoryRecords)
                    .Where(it => it.Id == inventoryId)
                    .FirstOrDefault();
                if (entity != null)
                {
                    return entity.ToModel();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取某次盘点任务对象
        /// </summary>
        /// <param name="inventoryId">盘点任务Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public AssetInventoryModel GetAssetInventory(int inventoryId, int userId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetInventories.Where(it => it.Id == inventoryId)
                    .FirstOrDefault();
                if (entity != null)
                {
                    if (entity.AssetInventoryRecords != null && entity.AssetInventoryRecords.Count > 0)
                    {
                        var inventoryRecords = entity.AssetInventoryRecords.Where(it => it.UserId == userId && it.InventoryId == inventoryId).ToList();
                        entity.AssetInventoryRecords = inventoryRecords;
                    }
                    return entity.ToModel();
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
