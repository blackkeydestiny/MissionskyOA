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
    public class AcceptProxyService : IAcceptProxyService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        /// <summary>
        /// 新增签收单
        /// </summary>
        /// <returns>签收信息</returns>
        public AcceptProxyModel AddAcceptProxy(AcceptProxyModel acceptProxy, bool isProduction = false)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                // 存入数据库
                var AcceptProxyEntity = acceptProxy.ToEntity();
                dbContext.AcceptProxies.Add(AcceptProxyEntity);
                dbContext.SaveChanges();

                // 根据AcceptUserId获取user，判断接收人是否存在
                var User = dbContext.Users.Where(it => it.Id == acceptProxy.AcceptUserId).FirstOrDefault();
                if (User == null)
                {
                    throw new Exception("This accept user not exist.");
                }

                // create user为当前登录用户
                var createUser = dbContext.Users.Where(it => it.Id == acceptProxy.CreateUserId).FirstOrDefault();

                // 构建通知信息：邮件发送
                NotificationModel model = new NotificationModel()
                {
                    Target = User.Email,
                    CreatedUserId = User.Id,
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email, // 邮件发送
                    BusinessType = BusinessType.ExpressMessage,
                    Title = "Missionsky OA Notification",
                    MessageContent = "你有新的快递," + createUser.EnglishName + "已经签收!",// 当前用户已经帮接收人签收
                    MessagePrams = "test",
                    Scope = NotificationScope.User,
                    TargetUserIds = new List<int> { User.Id }
                };

                try
                {
                    // 消息通知
                    this._notificationService.Add(model, isProduction);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return AcceptProxyEntity.ToModel();
            }
        }

        /// <summary>
        /// 更新签收单
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool UpdateAcceptProxy(UpdateAcceptProxyModel acceptProxy, int acceptProxyId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var AcceptProxyEntity = dbContext.AcceptProxies.Where(it => it.Id == acceptProxyId).FirstOrDefault();
                if (AcceptProxyEntity == null)
                {
                    throw new Exception("This express doesn't exist.");
                }

                AcceptProxyEntity.Description = acceptProxy.Description;
                AcceptProxyEntity.LastModifyTime = DateTime.Now;
                AcceptProxyEntity.AcceptUserId = acceptProxy.AcceptUserId;
                if (AcceptProxyEntity.Status == (int)ExpressStatus.Proxy)
                {
                    AcceptProxyEntity.Status = (int)acceptProxy.Status;
                }
                // 保存更改
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 删除签收单
        /// </summary>
        /// <returns>是否删除成功</returns>
        public bool deleteAcceptProxy(int acceptProxyId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var AcceptProxyEntity = dbContext.AcceptProxies.Where(it => it.Id == acceptProxyId).FirstOrDefault();
                if (AcceptProxyEntity == null)
                {
                    throw new Exception("This express doesn't exist.");
                }
                // 删除签收单
                dbContext.AcceptProxies.Remove(AcceptProxyEntity);
                dbContext.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// 查询当前用户的签收记录
        /// </summary>
        /// <returns>签收记录信息</returns>
        public IPagedList<AcceptProxyModel> MyAcceptProxyList(int userId, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<AcceptProxyModel> result = new List<AcceptProxyModel>();

                //
                var myAcceptProxyList = dbContext.AcceptProxies.Where(it => it.AcceptUserId == userId || it.CreateUserId == userId).OrderByDescending(item => item.LastModifyTime);

                if (myAcceptProxyList == null)
                {
                    return null;

                }
                myAcceptProxyList.ToList().ForEach(item =>
                {
                    //添加到返回结果
                    result.Add(item.ToModel());
                });


                return new PagedList<AcceptProxyModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 查询所有用户的签收记录
        /// </summary>
        /// <returns>签收记录信息</returns>
        public IPagedList<AcceptProxyModel> AcceptProxyHistoryList(int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<AcceptProxyModel> result = new List<AcceptProxyModel>();

                // 按日期排序
                var myAcceptProxyList = dbContext.AcceptProxies.OrderByDescending(item => item.Date);

                if (myAcceptProxyList == null)
                {
                    return null;

                }
                myAcceptProxyList.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                return new PagedList<AcceptProxyModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 查询签收信息
        /// </summary>
        /// <returns>签收记录信息</returns>
        public AcceptProxy GetAcceptProxyById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //查询申请信息是否存在
                var AcceptProxyEntity = dbContext.AcceptProxies.Where(it => it.Id == id).FirstOrDefault();

                if (AcceptProxyEntity == null)
                {
                    throw new Exception("This express doesn't exist.");
                }

                return AcceptProxyEntity;
            }
        }

        /// <summary>
        /// 列出所有签收单
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ListResult<AcceptProxyModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.AcceptProxies.AsEnumerable();

                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "LastModifyTime":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.LastModifyTime);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.LastModifyTime);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<AcceptProxyModel> result = new ListResult<AcceptProxyModel>();
                result.Data = new List<AcceptProxyModel>();
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
