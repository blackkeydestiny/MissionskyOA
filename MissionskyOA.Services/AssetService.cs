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
using System.Data.Entity.SqlServer;

namespace MissionskyOA.Services
{
    public class AssetService : IAssetService
    {
        public AssetModel Add(AssetModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                if (CheckBarCodeExists(dbContext, model, null))
                {
                    throw new Exception("编号已存在.");
                }
                var entity = model.ToEntity();
                dbContext.Assets.Add(entity);
                dbContext.SaveChanges();
                ///添加属性
                if (model.AssetInfoes != null && model.AssetInfoes.Count > 0)
                {
                    foreach (var item in model.AssetInfoes)
                    {
                        dbContext.AssetInfoes.Add(new AssetInfo()
                        {
                            Asset = entity,
                            AssetId = entity.Id,
                            AttributeId = item.AttributeId,
                            AttributeValue = item.AttributeValue
                        });
                    }
                    dbContext.SaveChanges();
                }

                return GetAssetById(entity.Id);
            }
        }

        /// <summary>
        /// 导入资产
        /// </summary>
        /// <param name="list"></param>
        /// <param name="codePrefix">编号固定字符</param>
        /// <returns></returns>
        public string Import(List<AssetModel> list, string codePrefix)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                string result = "";
                var barcodeAttribute = dbContext.AssetAttributes.Where(it => it.Name == "编号").FirstOrDefault();
                if (barcodeAttribute == null)
                {
                    throw new Exception("编号属性不存在,请先添加.");
                }

                var orderAreaAttr = dbContext.AssetAttributes.Where(it => it.Name == "入账区域").FirstOrDefault();
                if (barcodeAttribute == null)
                {
                    throw new Exception("入账区域属性不存在,请先添加.");
                }

                var buyDateAttr = dbContext.AssetAttributes.Where(it => it.Name == "采购日期").FirstOrDefault();
                if (buyDateAttr == null)
                {
                    throw new Exception("采购日期属性不存在,请先添加.");
                }

                int row = 0;
                bool haserror = false;
                foreach (var model in list)
                {
                    var entity = model.ToEntity();
                    dbContext.Assets.Add(entity);
                    dbContext.SaveChanges();
                    ///添加属性
                    if (model.AssetInfoes != null && model.AssetInfoes.Count > 0)
                    {
                        try
                        {
                            foreach (var item in model.AssetInfoes)
                            {
                                dbContext.AssetInfoes.Add(new AssetInfo()
                                {
                                    Asset = entity,
                                    AssetId = entity.Id,
                                    AttributeId = item.AttributeId,
                                    AttributeValue = item.AttributeValue
                                });
                            }
                            var buyDate = model.AssetInfoes.Where(it => it.AttributeId == buyDateAttr.Id).First();
                            var orderArea = model.AssetInfoes.Where(it => it.AttributeId == orderAreaAttr.Id).First();
                            string prefixCodeStartWith = "S" + codePrefix;
                            if (orderArea.AttributeValue == "HK")
                            {
                                prefixCodeStartWith = "H" + codePrefix;
                            }
                            //添加条码字段
                            string barcode = GenerateBarcode(dbContext, prefixCodeStartWith, barcodeAttribute.Id, DateTime.Parse(buyDate.AttributeValue));
                            dbContext.AssetInfoes.Add(new AssetInfo()
                            {
                                Asset = entity,
                                AssetId = entity.Id,
                                AttributeId = barcodeAttribute.Id,
                                AttributeValue = barcode
                            });
                            dbContext.SaveChanges();
                            row += 1;
                        }
                        catch (Exception ex)
                        {
                            dbContext.Assets.Remove(entity);
                            dbContext.SaveChanges();
                            haserror = true;
                            result += "第" + row + "行数据导入异常," + ex.Message + "<br>\n";
                        }
                    }
                }
                if (haserror)
                {
                    result = "成功导入" + row + "条数据,异常信息如下：<br>\n" + result;
                }
                else
                {
                    result = "成功导入" + row + "条数据";
                }
                return result;
            }
        }

        /// <summary>
        /// //生成条码
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="codePrefix">条码前缀(HK/SZ)</param>
        /// <param name="barcodeAttrId">条码属性Id</param>
        /// <param name="buyDate">购买日期</param>
        /// <returns></returns>
        private string GenerateBarcode(MissionskyOAEntities dbContext, string codePrefix, int barcodeAttrId, DateTime buyDate)
        {
            string result = "";
            //编码格式为SHC1307001或HHC1307001
            var prefixCode = codePrefix + buyDate.Date.ToString("yyMM");
            //获取购买月份已经保存的最大条码
            var existsMaxBarcode = dbContext.AssetInfoes.Where(it => it.AttributeId == barcodeAttrId
                  && (it.AttributeValue.StartsWith(prefixCode)))
                  .OrderByDescending(it => it.AttributeValue)
                  .FirstOrDefault();
            if (existsMaxBarcode != null)
            {
                int seqNumber = int.Parse(existsMaxBarcode.AttributeValue.Substring(existsMaxBarcode.AttributeValue.Length - 3, 3));
                if ((seqNumber + 1) < 10)
                {
                    result = prefixCode + "00" + (seqNumber + 1);
                }
                else if ((seqNumber + 1) >= 10 && (seqNumber + 1) < 100)
                {
                    result = prefixCode + "0" + (seqNumber + 1);
                }
                else
                {
                    result = prefixCode + (seqNumber + 1);
                }
            }
            else
            {
                result = prefixCode + "001";
            }
            return result;
        }

        public bool Remove(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Assets.Where(it => it.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    //删除属性
                    if (entity.AssetInfoes != null && entity.AssetInfoes.Count > 0)
                    {
                        var assetInfoList = entity.AssetInfoes.ToList();
                        for (int i = assetInfoList.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetInfoes.Remove(assetInfoList[i]);
                        }
                    }
                    //删除转移记录
                    if (entity.AssetTransactions != null && entity.AssetTransactions.Count > 0)
                    {
                        var transactionList = entity.AssetTransactions.ToList();
                        for (int i = transactionList.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetTransactions.Remove(transactionList[i]);
                        }
                    }
                    //删除盘点记录
                    var inventoryRecords = dbContext.AssetInventoryRecords.Where(it => it.AssetId == id).ToList();
                    if (inventoryRecords != null && inventoryRecords.Count > 0)
                    {
                        for (int i = inventoryRecords.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetInventoryRecords.Remove(inventoryRecords[i]);
                        }
                    }
                    dbContext.Assets.Remove(entity);
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool BatchRemove(int[] ids)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var distinctIds = ids.Distinct();
                var assets = dbContext.Assets.Where(it => distinctIds.Contains(it.Id)).ToList();
                foreach (var asset in assets)
                {
                    //删除属性
                    if (asset.AssetInfoes != null && asset.AssetInfoes.Count > 0)
                    {
                        var assetInfoList = asset.AssetInfoes.ToList();
                        for (int i = assetInfoList.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetInfoes.Remove(assetInfoList[i]);
                        }
                    }
                    //删除转移记录
                    if (asset.AssetTransactions != null && asset.AssetTransactions.Count > 0)
                    {
                        var transactionList = asset.AssetTransactions.ToList();
                        for (int i = transactionList.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetTransactions.Remove(transactionList[i]);
                        }
                    }
                    //删除盘点记录
                    var inventoryRecords = dbContext.AssetInventoryRecords.Where(it => it.AssetId == asset.Id).ToList();
                    if (inventoryRecords != null && inventoryRecords.Count > 0)
                    {
                        for (int i = inventoryRecords.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetInventoryRecords.Remove(inventoryRecords[i]);
                        }
                    }
                    dbContext.Assets.Remove(asset);
                }

                dbContext.SaveChanges();
                return true;
            }
        }

        public bool Update(AssetModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                if (CheckBarCodeExists(dbContext, model, model.Id))
                {
                    throw new Exception("编号已存在.");
                }

                var entity = dbContext.Assets.Where(it => it.Id == model.Id).FirstOrDefault();
                if (entity != null)
                {
                    entity.TypeId = model.TypeId;
                    entity.UserId = model.UserId;
                    entity.Status = (int)model.Status;

                    ///添加属性
                    if (model.AssetInfoes != null && model.AssetInfoes.Count > 0)
                    {
                        //删除旧数据
                        if (entity.AssetInfoes != null && entity.AssetInfoes.Count > 0)
                        {
                            var entityAssetInfoes = entity.AssetInfoes.ToArray();
                            for (int i = entityAssetInfoes.Length - 1; i >= 0; i--)
                            {
                                dbContext.AssetInfoes.Remove(entityAssetInfoes[i]);
                            }
                        }

                        //添加新数据
                        foreach (var item in model.AssetInfoes)
                        {
                            dbContext.AssetInfoes.Add(new AssetInfo()
                            {
                                Asset = entity,
                                AssetId = entity.Id,
                                AttributeId = item.AttributeId,
                                AttributeValue = item.AttributeValue
                            });
                        }
                    }
                    dbContext.SaveChanges();
                }

                return true;
            }
        }

        public List<AssetModel> List(AssetSearchModel search)
        {
            return QueryExecute(search, null);
        }

        public List<AssetModel> MyList(int currentUserId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<AssetModel> result = new List<AssetModel>();
                //获取转出者已经确认转出并且没有取消,当前用户待确认接收的资产Ids
                var myTransacionAssetIds = dbContext.AssetTransactions.Where(it => it.InUserId == currentUserId
                    && !(it.Status.HasValue && it.Status.Value == (int)AssetTransactionStatus.Canceled)
                    && it.OutUserIsConfirm.HasValue && it.OutUserIsConfirm.Value
                    && !(it.InUserIsConfirm.HasValue && it.InUserIsConfirm.Value))
                    .Select(it => it.AssetId)
                    .Distinct()
                    .ToList();
                //获取我自己的资产Ids
                var myAssetIds = dbContext.Assets.Where(it => it.UserId == currentUserId).Select(it => it.Id).ToList();
                //并集
                myAssetIds = myAssetIds.Union(myTransacionAssetIds).ToList();
                var myAssets = dbContext.Assets
                    .Include(it => it.User)
                    .Include(it => it.AssetType)
                    .Include(it => it.AssetInfoes)
                    .Include(it => it.AssetTransactions)
                    .Where(it => myAssetIds.Contains(it.Id))
                    .ToList();
                myAssets.ForEach(item =>
                {
                    result.Add(item.ToModel(dbContext));
                });
                return result;
            }
        }

        public IPagedList<AssetModel> SearchAssets(AssetSearchModel search, int pageIndex, int pageSize, int? unIncludeUserId)
        {
            return new PagedList<AssetModel>(QueryExecute(search, unIncludeUserId), pageIndex, pageSize);
        }

        //执行查询
        private List<AssetModel> QueryExecute(AssetSearchModel search, int? unIncludeUserId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.Assets.AsQueryable();
                if (search != null)
                {
                    if (!string.IsNullOrEmpty(search.BarCode))
                    {
                        var barcodeAttribute = dbContext.AssetAttributes.Where(it => it.Name == "编号").FirstOrDefault();
                        if (barcodeAttribute != null)
                        {
                            var assetIds = dbContext.AssetInfoes.Where(it => it.AttributeId == barcodeAttribute.Id
                                && it.AttributeValue.Contains(search.BarCode))
                                .Select(it => it.AssetId)
                                .Distinct()
                                .ToArray();
                            query = query.Where(it => assetIds.Contains(it.Id));
                        }
                    }
                    if (search.BuyDate.HasValue)
                    {
                        var buyDateAttribute = dbContext.AssetAttributes.Where(it => it.Name == "采购日期").FirstOrDefault();
                        if (buyDateAttribute != null)
                        {
                            var assetIdWithBuyDates = dbContext.AssetInfoes.Where(it => it.AttributeId == buyDateAttribute.Id
                                && !string.IsNullOrEmpty(it.AttributeValue))
                                .Select(it => new
                                {
                                    Id = it.AssetId,
                                    BuyDate = it.AttributeValue
                                })
                                .Distinct()
                                .ToArray();
                            var dt = DateTime.Now;
                            var assetIds = assetIdWithBuyDates.Where(it => DateTime.TryParse(it.BuyDate, out dt)
                                && DateTime.Parse(it.BuyDate).Year == search.BuyDate.Value.Year
                                && DateTime.Parse(it.BuyDate).Month == search.BuyDate.Value.Month)
                                .Select(it => it.Id)
                                .ToArray();
                            query = query.Where(it => assetIds.Contains(it.Id));
                        }
                    }
                    if (search.Status.HasValue)
                    {
                        query = query.Where(it => it.Status.HasValue && it.Status.Value == (int)search.Status.Value);
                    }
                    if (search.TypeId.HasValue)
                    {
                        query = query.Where(it => it.TypeId == search.TypeId.Value);
                    }
                    if (unIncludeUserId.HasValue)
                    {
                        query = query.Where(it => it.UserId != unIncludeUserId.Value);
                    }
                    if (!string.IsNullOrEmpty(search.UserName))
                    {
                        query = query.Where(it => it.User != null && it.User.EnglishName.ToLower().Contains(search.UserName.ToLower()));
                    }
                }


                var result = new List<AssetModel>();
                query.Include(it => it.User)
                    .Include(it => it.AssetType)
                    .Include(it => it.AssetInfoes).ToList().ForEach(it =>
                    {
                        result.Add(it.ToModel(dbContext));
                    });
                return result;
            }
        }

        /// <summary>
        /// 检查条码是否已经存在
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckBarCodeExists(MissionskyOAEntities dbContext, AssetModel model, int? id)
        {
            bool isExists = false;
            if (model.AssetInfoes != null && model.AssetInfoes.Count > 0)
            {
                var barcodeAttribute = dbContext.AssetAttributes.Where(it => it.Name == "编号").FirstOrDefault();
                var modelBarCode = model.AssetInfoes.Where(it => it.AttributeId == barcodeAttribute.Id).FirstOrDefault();
                if (modelBarCode != null && barcodeAttribute != null)
                {
                    //编辑时
                    if (id.HasValue && id.Value > 0)
                    {
                        if (dbContext.AssetInfoes.Count(it => it.AttributeId == barcodeAttribute.Id && it.AttributeValue == modelBarCode.AttributeValue && it.AssetId != id.Value) > 0)
                        {
                            isExists = true;
                        }
                    }
                    else
                    {
                        //新增时
                        if (dbContext.AssetInfoes.Count(it => it.AttributeId == barcodeAttribute.Id && it.AttributeValue == modelBarCode.AttributeValue) > 0)
                        {
                            isExists = true;
                        }
                    }
                }
            }

            return isExists;
        }

        /// <summary>
        /// 获取资产详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssetModel GetAssetById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Assets
                    .Include(it => it.User)
                    .Include(it => it.AssetType)
                    .Include(it => it.AssetInfoes)
                    .Where(it => it.Id == id)
                    .FirstOrDefault();
                if (entity != null)
                {
                    return entity.ToModel(dbContext);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 根据编码获取资产信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public AssetModel GetAssetByBarcode(string barcode)
        {
            using (var dbContext = new MissionskyOAEntities())
            {

                if (!string.IsNullOrEmpty(barcode))
                {
                    var barcodeAttribute = dbContext.AssetAttributes.Where(it => it.Name == "编号").FirstOrDefault();
                    if (barcodeAttribute != null)
                    {
                        var assetId = dbContext.AssetInfoes.Where(it => it.AttributeId == barcodeAttribute.Id
                            && barcode == it.AttributeValue)
                            .Select(it => it.AssetId)
                            .FirstOrDefault();
                        if (assetId > 0)
                        {
                            var entity = dbContext.Assets
                                   .Include(it => it.User)
                                   .Include(it => it.AssetType)
                                   .Include(it => it.AssetInfoes)
                                   .Where(it => assetId == it.Id)
                                   .FirstOrDefault();
                            if (entity != null)
                            {
                                return entity.ToModel(dbContext);
                            }
                            else
                            {
                                return null;
                            }

                        }
                    }
                }
                return null;

            }
        }

    }
}
