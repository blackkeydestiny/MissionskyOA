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
    public class AssetTypeService : IAssetTypeService
    {
        public AssetTypeModel Add(AssetTypeModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var existItem = dbContext.AssetTypes.Where(it => it.Name == model.Name).FirstOrDefault();
                if (existItem != null)
                {
                    throw new Exception("此类别已存在.");
                }

                var entity = model.ToEntity();
                dbContext.AssetTypes.Add(entity);
                dbContext.SaveChanges();
                return entity.ToModel();
            }
        }

        public bool Remove(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTypes.Where(it => it.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    var assetCount = dbContext.Assets.Where(it => it.TypeId == entity.Id).Count();
                    if (assetCount > 0)
                    {
                        throw new Exception("该类别已经被资产使用,不能删除.");
                    }

                    if (entity.AssetTypeAttributes != null && entity.AssetTypeAttributes.Count > 0)
                    {
                        //删除类型关联的该属性数据
                        var assetTypeAttributeList = entity.AssetTypeAttributes.ToList();
                        for (int i = assetTypeAttributeList.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetTypeAttributes.Remove(assetTypeAttributeList[i]);
                        }
                    }
                    dbContext.AssetTypes.Remove(entity);
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool Update(AssetTypeModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var existItem = dbContext.AssetTypes.Where(it => it.Name == model.Name && it.Id != model.Id).FirstOrDefault();
                if (existItem != null)
                {
                    throw new Exception("此类别已存在.");
                }

                var entity = dbContext.AssetTypes.Where(it => it.Id == model.Id).FirstOrDefault();
                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.Sort = model.Sort;
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 设置资产类别的属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SetAttributes(AssetTypeModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTypes.Where(it => it.Id == model.Id).FirstOrDefault();
                if (entity != null)
                {
                    //处理区域
                    var sourceTypeAttributes = dbContext.AssetTypeAttributes.Where(it => it.TypeId == model.Id).ToList();
                    if (model.Attributes != null && model.Attributes.Count > 0)
                    {
                        List<int> sourceAttributeIds = sourceTypeAttributes.Select(it => it.AttributeId).ToList();
                        var modelAttributeIds = model.Attributes.Select(it => it.Id).ToList().Distinct();
                        foreach (var id in modelAttributeIds)
                        {
                            //添加新的
                            if (!sourceAttributeIds.Contains(id))
                            {
                                AssetTypeAttribute typeAttr = new AssetTypeAttribute();
                                typeAttr.TypeId = model.Id;
                                typeAttr.AttributeId = id;
                                dbContext.AssetTypeAttributes.Add(typeAttr);
                            }
                            sourceAttributeIds.Remove(id);
                        }
                        //删除
                        var needDeleteTypeAttributes = dbContext.AssetTypeAttributes.Where(it => sourceAttributeIds.Contains(it.AttributeId) && it.TypeId == model.Id);
                        foreach (var item in needDeleteTypeAttributes)
                        {
                            dbContext.AssetTypeAttributes.Remove(item);
                        }
                    }
                    else
                    {
                        //删除所有
                        foreach (var item in sourceTypeAttributes)
                        {
                            dbContext.AssetTypeAttributes.Remove(item);
                        }
                    }
                    dbContext.SaveChanges();
                }

                return true;
            }
        }

        public ListResult<AssetTypeModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.AssetTypes.AsEnumerable();
                var count = list.Count();
                list = list.OrderBy(it => it.Sort).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
                ListResult<AssetTypeModel> result = new ListResult<AssetTypeModel>();
                result.Data = new List<AssetTypeModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        public List<AssetTypeModel> GetAll()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<AssetTypeModel>();
                dbContext.AssetTypes.Include(it => it.AssetTypeAttributes).OrderBy(it => it.Sort).ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return result;
            }
        }
        public AssetTypeModel GetTypeById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTypes.Include(it => it.AssetTypeAttributes).Where(it => it.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    var model = entity.ToModel();
                    if (entity.AssetTypeAttributes != null && entity.AssetTypeAttributes.Count > 0)
                    {
                        var typeAttributes = entity.AssetTypeAttributes.Where(it => it.AssetType.Id == entity.Id).ToList();
                        if (typeAttributes != null && typeAttributes.Count > 0)
                        {
                            model.Attributes = new List<AssetAttributeModel>();
                            typeAttributes.ToList().ForEach(it =>
                                {
                                    model.Attributes.Add(it.AssetAttribute.ToModel());
                                });
                        }

                    }
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
