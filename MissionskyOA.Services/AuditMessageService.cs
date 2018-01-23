using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 审记信息处理类
    /// </summary>
    public class AuditMessageService : IAuditMessageService
    {
        /// <summary>
        /// 添加审计信息
        /// </summary>
        /// <param name="model">审计信息</param>
        /// <returns>更新后的审计信息Model</returns>
        public AuditMessageModel AddMessage(AuditMessageModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = model.ToEntity();

                if (model.Status == AuditMessageStatus.None)
                {
                    entity.Status = (int) AuditMessageStatus.Unread;
                }

                dbContext.AuditMessages.Add(entity);
                dbContext.SaveChanges();
                model = entity.ToModel();

                return model;
            }
        }

        /// <summary>
        /// 添加审计信息
        /// </summary>
        /// <param name="dbContext">上下文</param>
        /// <param name="model">审计信息</param>
        /// <returns>是否更新成功</returns>
        public bool AddMessage(MissionskyOAEntities dbContext, AuditMessageModel model)
        {
                var entity = model.ToEntity();

                if (model.Status == AuditMessageStatus.None)
                {
                    entity.Status = (int)AuditMessageStatus.Unread;
                }

                dbContext.AuditMessages.Add(entity);

                return true;
        }
        
        /// <summary>
        /// 获取用户审计信息
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>返回用户审计信息及分页信息</returns>
        public IPagedList<AuditMessageModel> GetUserAuditMessages(SearchAuditMessageModel model, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = dbContext.AuditMessages.AsEnumerable();


                //查户指定类型信息
                if (model != null && model.Type != AuditMessageType.None)
                {
                    query = query.Where(it => it.Type == (int)model.Type);
                }

                //查户指定状态信息
                if (model != null && model.Status != AuditMessageStatus.None)
                {
                    query = query.Where(it => it.Status == (int)model.Status);
                }

                //查户指定用户信息
                if (model != null && model.UserId > 0)
                {
                    query = query.Where(it => it.UserId == model.UserId);
                }

                List<AuditMessageModel> result = new List<AuditMessageModel>();
                query.ToList().ForEach(it =>
                {
                    result.Add(it.ToModel());
                });

                return new PagedList<AuditMessageModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 审计日志列表
        /// </summary>
        /// <returns>审计日志信息列表</returns>
        public ListResult<AuditMessageModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.AuditMessages.AsEnumerable();

                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedTime":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.CreatedTime);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.CreatedTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<AuditMessageModel> result = new ListResult<AuditMessageModel>();
                result.Data = new List<AuditMessageModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }
    }
}
