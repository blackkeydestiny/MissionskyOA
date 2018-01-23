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
    public class AssetAttributeService : IAssetAttributeService
    {
        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AssetAttributeModel Add(AssetAttributeModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var existItem = dbContext.AssetAttributes.Where(it => it.Name == model.Name).FirstOrDefault();
                if (existItem != null)
                {
                    throw new Exception("此属性已存在.");
                }

                var entity = model.ToEntity();
                dbContext.AssetAttributes.Add(entity);
                dbContext.SaveChanges();
                return entity.ToModel();
            }
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Remove(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetAttributes.Where(it => it.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    var assetInfoCount = dbContext.AssetInfoes.Where(it => it.AttributeId == entity.Id).Count();
                    if (assetInfoCount > 0)
                    {
                        throw new Exception("该属性已经被资产使用,不能删除.");
                    }
                    if (entity.AssetTypeAttributes != null && entity.AssetTypeAttributes.Count > 0)
                    {
                        //删除类型关联的该属性数据
                        var asetTypeAttributeEntitys = entity.AssetTypeAttributes.ToList();
                        for (int i = asetTypeAttributeEntitys.Count - 1; i >= 0; i--)
                        {
                            dbContext.AssetTypeAttributes.Remove(asetTypeAttributeEntitys[i]);
                        }
                    }
                    dbContext.AssetAttributes.Remove(entity);
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(AssetAttributeModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var existItem = dbContext.AssetAttributes.Where(it => it.Name == model.Name && it.Id != model.Id).FirstOrDefault();
                if (existItem != null)
                {
                    throw new Exception("此属性已存在.");
                }

                var entity = dbContext.AssetAttributes.Where(it => it.Id == model.Id).FirstOrDefault();
                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.Sort = model.Sort;
                    entity.DataType = (int)model.DataType;
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 分页获取属性列表
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ListResult<AssetAttributeModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {

            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.AssetAttributes.AsEnumerable();
                var count = list.Count();
                list = list.OrderBy(it => it.Sort).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
                ListResult<AssetAttributeModel> result = new ListResult<AssetAttributeModel>();
                result.Data = new List<AssetAttributeModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        public List<AssetAttributeModel> GetAll()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var result = new List<AssetAttributeModel>();
                dbContext.AssetAttributes.OrderBy(it => it.Sort).ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return result;
            }
        }

        /// <summary>
        /// 获取单个属性
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssetAttributeModel GetAttributeById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetAttributes.Where(it => it.Id == id).FirstOrDefault();
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

    }
}
