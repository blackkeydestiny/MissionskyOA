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
using MissionskyOA.Core.Common;


namespace MissionskyOA.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        private readonly IRoleService _roleService = new RoleService();
        private readonly IAssetInventoryService _assetInventoryService=new AssetInventoryService();
        
        /// <summary>
        /// 申请通告
        /// </summary>
        /// <param name="AnnoucementModel">通告信息</param>
        /// <returns></returns>
        public AnnouncementModel AddAnnouncement(AnnouncementModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //构建通知消息
                var notificationModel = new NotificationModel()
                {
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email,
                    CreatedUserId = model.CreateUserId,
                    Scope = NotificationScope.Public,
                    Title = model.Title,
                    MessageContent = model.Content,
                    BusinessType = BusinessType.AdministrationEventAnnounce
                };

                if (model.Type == AnnouncementType.AssetInventory)
                {
                    //资产盘点发通知后需要创建新的盘点任务
                    int inventoryId = this._assetInventoryService.AddAssetInventory(new AssetInventoryModel()
                    {
                        Title = model.Title,
                        Description = model.Content,
                        Status = AssetInventoryStatus.Open
                    });
                    model.RefAssetInventoryId = inventoryId;
                    notificationModel.BusinessType = BusinessType.AssetInventory;
                    // 消息通知
                    this._notificationService.Add(notificationModel, Global.IsProduction);

                }
                else
                {
                    if (model.Status == AnnouncementStatus.AllowPublish)
                    {
                        // 消息通知
                        this._notificationService.Add(notificationModel, Global.IsProduction);
                    }
                }
                
                var entity = dbContext.Announcements.Add(model.ToEntity());
                dbContext.SaveChanges();
                return entity.ToModel();
            }
        }

        /// <summary>
        /// 用户是否有操作此通告的权利
        /// </summary>
        /// <param name="AnnoucementModel"></param>
        /// <returns></returns>
        public bool isUserHaveAuthority2Announcement(AnnouncementType type, int roleId)
        {
            RoleModel role = _roleService.SearchRole(roleId);
            if (!role.RoleName.Contains("行政") && (type != AnnouncementType.EmployeeeNews || type != AnnouncementType.ActivityMessage))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取待处理的通告申请单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>通告申请单审批列表</returns>
        public List<AnnouncementModel> GetPendingAnnocument()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var pendingAnnocumentModel = new List<AnnouncementModel>();

                var pendingAnnocumentEntity = dbContext.Announcements.Where(it => it.Status == (int)AnnouncementStatus.Apply);

                pendingAnnocumentEntity.ToList().ForEach(item =>
                {
                    pendingAnnocumentModel.Add(item.ToModel());
                });
                return pendingAnnocumentModel;
            }
        }

        /// <summary>
        /// 审批通告
        /// </summary>
        /// <param name="model">审批model</param>
        /// <param name="userId">用户Id</param>
        /// <returns>审批通告成功或者失败</returns>
        public bool Audit(AuditAnnouncementModel model, int userId, string userName)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var annocumentEntity = dbContext.Announcements.Where(it => it.Id==model.announcementID&&it.Status==(int)AnnouncementStatus.Apply).FirstOrDefault();
                if(annocumentEntity==null)
                {
                    throw new Exception("此通告不存在");
                }

                annocumentEntity.AuditReason = model.AuditReason;
                annocumentEntity.CreatedTime = DateTime.Now;

                if (model.auditStatus)
                {
                    annocumentEntity.Status = (int)AnnouncementStatus.AllowPublish;
                    var notificationModel = new NotificationModel()
                    {
                        //MessageType = NotificationType.PushMessage,
                        MessageType = NotificationType.Email,
                        CreatedUserId = annocumentEntity.CreateUserId,
                        Scope = NotificationScope.Public,
                        Title = annocumentEntity.Title,
                        MessageContent = annocumentEntity.Content,
                        BusinessType = BusinessType.AdministrationEventAnnounce
                    };
                    this._notificationService.Add(notificationModel, Global.IsProduction);
                }
                else
                {
                    annocumentEntity.Status = (int)AnnouncementStatus.RejectPublish;
                    var applyUserEntity = dbContext.Users.Where(it => it.Id == annocumentEntity.CreateUserId).FirstOrDefault();
                    if(applyUserEntity==null)
                    {
                        throw new Exception("通告申请用户不存在");
                    }
                  
                    var notificationModel = new NotificationModel()
                    {
                        Target = applyUserEntity.Email,
                        //MessageType = NotificationType.PushMessage,
                        MessageType = NotificationType.Email,
                        CreatedUserId = annocumentEntity.CreateUserId,
                        Scope = NotificationScope.User,
                        Title = "通告审批消息",
                        MessageContent = "您的申请通告'" + annocumentEntity.Title + "',已经被" + userName+"拒绝，拒绝理由："+model.AuditReason,
                        BusinessType = BusinessType.AnnocumentAuditMessage,
                        TargetUserIds = new List<int> { applyUserEntity.Id }
                    };
                    this._notificationService.Add(notificationModel, Global.IsProduction);
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        /// </summary>
        /// 显示在有效期内的通告
        /// </summary>
        /// <returns>有效期内的通告信息</returns>
        public IPagedList<AnnouncementModel> AnnouncementList(int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                DateTime currentDate = DateTime.Now.Date;
                List<AnnouncementModel> result = new List<AnnouncementModel>();

                var annocumentList = dbContext.Announcements.Where(it => it.Status == (int)AnnouncementStatus.AllowPublish).OrderByDescending(item => item.CreatedTime);

                if (annocumentList == null)
                {
                    return null;

                }

                annocumentList.ToList().ForEach(item =>
                {
                    if(item.CreatedTime.Value.AddDays((double)item.EffectiveDays) > currentDate)
                    {
                        result.Add(item.ToModel());
                    }
                    
                });

                return new PagedList<AnnouncementModel>(result, pageIndex, pageSize);
            }
        }

        /// </summary>
        /// 显示所有的通告
        /// </summary>
        /// <returns>显示所有的通告</returns>
        public ListResult<AnnouncementModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                // 1、首先获取所有通告从数据库中
                var list = dbContext.Announcements.AsEnumerable();

                // 2、判断是否有排序
                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedTime":
                            // 
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

                // 
                var count = list.Count();
                // 分页
                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<AnnouncementModel> result = new ListResult<AnnouncementModel>();
                result.Data = new List<AnnouncementModel>();

                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        /// <summary>
        /// 根据ID获取通告
        /// </summary>
        /// <param name="Id">通告Id</param>
        /// <returns></returns>
        public AnnouncementModel GetAnnouncementByID(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var annocumentEntity = dbContext.Announcements.Where(it => it.Id==id).FirstOrDefault();
                return annocumentEntity.ToModel();
            }
        }
    }
}
