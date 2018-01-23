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
    public class AssetTransactionService : IAssetTransactionService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(AssetTransactionModel model, bool isProduction = false)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var asset = dbContext.Assets.Where(it => it.Id == model.AssetId).FirstOrDefault();
                if (asset == null)
                {
                    throw new Exception("指定的资产Id不存在.");
                }
                if (!asset.Status.HasValue || (asset.Status.HasValue && (AssetStatus)asset.Status.Value != AssetStatus.Normal))
                {
                    throw new Exception("此状态的资产不能进行转移操作.");
                }

                var entity = model.ToEntity();
                entity.InUserIsConfirm = false;
                entity.OutUserIsConfirm = false;
                entity.CreatedTime = DateTime.Now;
                dbContext.AssetTransactions.Add(entity);

                asset.Status = (int)AssetStatus.WaitOut;
                dbContext.SaveChanges();
                SendNotificationsToOutAndInUsers(dbContext, model.OutUserId, model.InUserId, false, isProduction);
                return true;
            }
        }

        /// <summary>
        /// 发送消息给转出和转出者
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="outUserId">转出者Id</param>
        /// <param name="inUserId">转入者Id</param>
        /// <param name="isCancelOperate">是否是取消操作</param>
        /// <param name="isProduction">是否是生成环境</param>
        private void SendNotificationsToOutAndInUsers(MissionskyOAEntities dbContext, int outUserId, int inUserId, bool isCancelOperate, bool isProduction)
        {
            var users = dbContext.Users.Where(it => it.Id == outUserId || it.Id == inUserId).ToList();
            if (users.Count != 2)
            {
                throw new Exception("指定的转出者或接收者不存在.");
            }
            var outUser = users.Where(it => it.Id == outUserId).First();
            var inUser = users.Where(it => it.Id == inUserId).First();

            NotificationModel outNotice = new NotificationModel()
            {
                Target = outUser.Email,
                CreatedUserId = outUser.Id,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.AssetTransfer,
                Title = isCancelOperate ? "资产转移-取消通知" : "资产转移-确认通知",
                MessageContent = isCancelOperate ? "你有一项将要转移给" + inUser.EnglishName + "的资产记录已取消,请知悉!" : "你有一项资产将要转移给" + inUser.EnglishName + ",请确认!",
                MessagePrams = "",
                Scope = NotificationScope.User,
                TargetUserIds = new List<int> { outUser.Id }
            };

            NotificationModel inNotice = new NotificationModel()
            {
                Target = inUser.Email,
                CreatedUserId = inUser.Id,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.AssetTransfer,
                Title = isCancelOperate ? "资产转移-取消通知" : "资产转移-确认通知",
                MessageContent = isCancelOperate ? outUser.EnglishName + "将要转移给你的资产记录已取消,请知悉!" : outUser.EnglishName + "将有一项资产将要转移给你,请确认!",
                MessagePrams = "",
                Scope = NotificationScope.User,
                TargetUserIds = new List<int> { inUser.Id }
            };

            try
            {
                this._notificationService.Add(outNotice, isProduction);
                this._notificationService.Add(inNotice, isProduction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Cancel(int transactionId, bool isProduction = false)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTransactions.Where(it => it.Id == transactionId).First();
                if (entity.Status.HasValue && entity.Status.Value == (int)AssetTransactionStatus.Canceled)
                {
                    throw new Exception("此转移记录已经处于取消状态,不能再次取消.");
                }
                entity.Status = (int)AssetTransactionStatus.Canceled;
                entity.Asset.Status = (int)AssetStatus.Normal;
                dbContext.SaveChanges();
                //取消后发通知
                SendNotificationsToOutAndInUsers(dbContext, entity.OutUserId, entity.InUserId, true, isProduction);
                return true;
            }
        }

        public bool Confirm(int transactionId, int outOrIn, bool isProduction = false)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTransactions.Where(it => it.Id == transactionId).First();
                if (entity.Status.HasValue && entity.Status.Value == (int)AssetTransactionStatus.Canceled)
                {
                    throw new Exception("此转移记录已经处于取消状态,不允许操作.");
                }
                if (outOrIn == 0)
                {
                    entity.OutUserIsConfirm = true;
                    entity.Asset.Status = (int)AssetStatus.WaitIn;
                }
                if (outOrIn == 1)
                {
                    entity.InUserIsConfirm = true;
                    entity.Asset.UserId = entity.InUserId;
                    entity.Asset.Status = (int)AssetStatus.Normal;
                }
                dbContext.SaveChanges();
                //确认转入后发通知给James
                if (outOrIn == 1)
                {
                    var users = dbContext.Users.Where(it => it.Id == entity.OutUserId || it.Id == entity.InUserId || (it.Email != null && it.Email.ToLower() == "james.xu@missionsky.com")).ToList();
                    var outUser = users.Where(it => it.Id == entity.OutUserId).First();
                    var inUser = users.Where(it => it.Id == entity.InUserId).First();
                    var assetAdmin = users.Where(it => it.Email != null && it.Email.ToLower() == "james.xu@missionsky.com").FirstOrDefault();
                    NotificationModel notice = new NotificationModel()
                    {
                        Target = "james.xu@missionsky.com",
                        CreatedUserId = entity.InUserId,
                        //MessageType = NotificationType.PushMessage,
                        MessageType = NotificationType.Email,
                        BusinessType = BusinessType.AssetTransfer,
                        Title = "资产转移通知",
                        MessageContent = "有一项资产从" + outUser.EnglishName + "转移给" + inUser.EnglishName + ",请知悉(点击可以查看详情)!",
                        MessagePrams = "{\"transactionId\":" + entity.Id + "}",
                        Scope = NotificationScope.User
                    };
                    if (assetAdmin != null)
                    {
                        notice.TargetUserIds = new List<int> { assetAdmin.Id };
                    }

                    try
                    {
                        this._notificationService.Add(notice, isProduction);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return true;
            }

        }

        /// <summary>
        /// 获取单个转移记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssetTransactionModel GetAssetTransactionById(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.AssetTransactions.Where(it => it.Id == id).FirstOrDefault();
                if (entity != null)
                {
                    var model = entity.ToModel();
                    var outInUsers = dbContext.Users.Where(it => it.Id == model.OutUserId || it.Id == model.InUserId).ToList();
                    if (outInUsers.Count != 2)
                    {
                        throw new Exception("指定的转出者或接收者不存在.");
                    }
                    model.OutUserName = outInUsers.Where(it => it.Id == model.OutUserId).First().EnglishName;
                    model.InUserName = outInUsers.Where(it => it.Id == model.InUserId).First().EnglishName;
                    model.Asset = entity.Asset.ToModel(dbContext);
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
