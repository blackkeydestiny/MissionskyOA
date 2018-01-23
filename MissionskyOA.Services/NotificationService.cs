using cn.jpush.api;
using cn.jpush.api.push.mode;
using MissionskyOA.Core.Email;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Services
{
    public class NotificationService : INotificationService
    {
        public bool Add(NotificationModel model, bool isProduction = false)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = model.ToEntity();
                entity.CreatedTime = DateTime.Now;
                dbContext.Notifications.Add(entity);
                dbContext.SaveChanges();
                if (model.TargetUserIds != null && model.TargetUserIds.Count > 0)
                {
                    foreach (var userId in model.TargetUserIds)
                    {
                        NotificationTargetUser nUser = new NotificationTargetUser()
                        {
                            Notification = entity,
                            NotificationId = entity.Id,
                            TargetUserId = userId
                        };
                        dbContext.NotificationTargetUsers.Add(nUser);
                    }
                    dbContext.SaveChanges();
                }
                try
                {
                    string title = "通知";
                    switch (model.BusinessType)
                    {
                        case BusinessType.Approving:
                            title = "审批通知";
                            break;
                        case BusinessType.BookMeetingRoomSuccessAnnounce:
                            title = "预定会议室成功";
                            break;
                        case BusinessType.CacnelMeetingRoomSuccessAnnounce:
                            title = "取消会议室预定成功";
                            break;
                        case BusinessType.ExpressMessage:
                            title = "快递消息";
                            break;
                        default:
                            break;
                    }

                    if (model.MessageType == NotificationType.Email)
                    {
                        EmailClient.Send(new List<string>() { model.Target }, null,
                            string.IsNullOrEmpty(model.Title) ? title : model.Title, model.MessageContent);
                    }
                    else if (model.MessageType == NotificationType.SMessage)
                    {
                        //短信服务暂时没有
                        return true;
                    }
                    else
                    {
                        JPushClient client = new JPushClient(JPushExtensions.app_key, JPushExtensions.master_secret);
                        string parameters = "{\"businessType\":" + ((int) model.BusinessType).ToString() +
                                            ",\"notificationId\":" + entity.Id + "}";
                        if (model.BusinessType == BusinessType.AdministrationEventAnnounce)
                        {
                            title = string.IsNullOrEmpty(model.Title) ? "公告" : model.Title;
                            PushPayload payload = JPushExtensions.PushBroadcast(null, title, model.MessageContent,
                                parameters, isProduction);
                            JPushExtensions.SendPush(payload, client);
                        }
                        else if (model.BusinessType == BusinessType.AssetInventory)
                        {
                            title = string.IsNullOrEmpty(model.Title) ? "资产盘点公告" : model.Title;
                            PushPayload payload = JPushExtensions.PushBroadcast(null, title, model.MessageContent,
                                parameters, isProduction);
                            JPushExtensions.SendPush(payload, client);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.Target))
                            {
                                string target = model.Target;
                                // 2. 检查邮箱的格式
                                if (!String.IsNullOrEmpty(model.Target) && model.Target.IsEmail())
                                {
                                    target = model.Target.ToLower().Split('@')[0].Replace(".", "");
                                }
                                if (model.Scope == NotificationScope.UserGroup)
                                {
                                    PushPayload payload = JPushExtensions.PushBroadcast(new string[] {target}, title,
                                        model.MessageContent, parameters, isProduction);
                                    JPushExtensions.SendPush(payload, client);
                                }
                                else
                                {
                                    PushPayload payload =
                                        JPushExtensions.PushObject_android_and_ios(new string[] {target}, title,
                                            model.MessageContent, parameters, isProduction);
                                    JPushExtensions.SendPush(payload, client);
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    return true;
                }
            }

            return true;
        }
        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var notification = dbContext.Notifications.FirstOrDefault(it => it.Id == id);
                if (notification == null)
                {
                    throw new Exception("此通知不存在!");
                }
                var notificationUser=dbContext.NotificationTargetUsers.Where(it=>it.NotificationId==notification.Id);
                if (notificationUser != null && notificationUser.Count() > 0)
                {
                    notificationUser.ToList().ForEach(item =>
                    {
                            dbContext.NotificationTargetUsers.Remove(item);
                    });
                }
                
                dbContext.Notifications.Remove(notification);
                dbContext.SaveChanges();
                return true;
            }
        }

        public NotificationModel GetNotificationById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var notification = dbContext.Notifications.FirstOrDefault(it => it.Id == id);
                if (notification == null)
                {
                    throw new Exception("此通知不存在!");
                }

                return notification.ToModel();
            }
        }

        public IPagedList<NotificationModel> GetMyNotifications(int currentUserId, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                List<NotificationModel> result = new List<NotificationModel>();

                var notificationUserIds = dbContext.NotificationTargetUsers.Where(it => it.TargetUserId == currentUserId).Select(it => it.NotificationId).Distinct().ToList();
                var notifications = dbContext.Notifications.Where(it => it.Scope == (int)NotificationScope.Public || notificationUserIds.Contains(it.Id)).ToList();
                notifications.ToList().ForEach(item =>
                {
                    result.Add(item.ToModel());
                });
                // TO-DO 需要考虑按照时间排序(倒序)
                result = result.OrderByDescending(it => it.CreatedTime).ToList<NotificationModel>();
                return new PagedList<NotificationModel>(result, pageIndex, pageSize);
            }
        }


        public ListResult<NotificationModel> List(int pageNo, int pageSize, SortModel sort)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Notifications.AsEnumerable();
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
                ListResult<NotificationModel> result = new ListResult<NotificationModel>();
                result.Data = new List<NotificationModel>();
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
