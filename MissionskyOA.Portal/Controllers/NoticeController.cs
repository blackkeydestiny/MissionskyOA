using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Portal.Common;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class NoticeController : Controller
    {
        IUserService UserService;
        INotificationService NotificationService;
        IAssetInventoryService AssetInventoryService { get; set; }
        IAnnouncementService AnnouncementService { get; set; }
        //
        // GET: /Area/

        public NoticeController(IUserService userService, INotificationService notificationService, IAssetInventoryService assetInventoryService, IAnnouncementService announcementService)
        {
            this.UserService = userService;
            this.NotificationService = notificationService;
            this.AssetInventoryService = assetInventoryService;
            this.AnnouncementService = announcementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="dRequest"></param>
        /// <returns></returns>
        public ActionResult Read([DataSourceRequest]DataSourceRequest dRequest)
        {
            SortModel sort = null;
            if (dRequest.Sorts != null && dRequest.Sorts.Count > 0)
            {
                sort = dRequest.Sorts[0].ToSortModel();
            }
            var noticeResult = NotificationService.List(dRequest.Page, dRequest.PageSize, sort);
            DataSourceResult result = new DataSourceResult()
            {
                Data = noticeResult.Data,
                Total = noticeResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var model = this.NotificationService.GetNotificationById(id.Value);
                return View(model);
            }
            else
            {
                NotificationModel model = new NotificationModel();
                return View(model);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.NotificationService.Delete(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(NotificationModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Title))
            {
                ModelState.AddModelError("Title", "请输入标题.");
            }

            if (string.IsNullOrEmpty(model.MessageContent))
            {
                ModelState.AddModelError("MessageContent", "请输入通知内容.");
            }
            //model.MessageType = NotificationType.PushMessage;
            model.MessageType = NotificationType.Email;
            model.CreatedUserId = CommonInstance.GetInstance().LoginUser.Id;
            model.Scope = NotificationScope.Public;
            model.Title = model.Title;
            if (action == "test")
            {
                //model.Target = "kevin.zhao@missionsky.com";
                //model.CreatedUserId = CommonInstance.GetInstance().LoginUser.Id;
                //model.MessageType = NotificationType.PushMessage;
                //model.BusinessType = BusinessType.Approving;
                //model.Title = "Missionsky OA Test";
                //model.MessageContent = "Kevin Test- to Kevin";
                //model.MessagePrams = "test";
                //model.Scope = NotificationScope.User;
                //model.TargetUserIds = new List<int> { CommonInstance.GetInstance().LoginUser.Id };
            }

            if (ModelState.IsValid)
            {
                try
                {
                    AnnouncementType type=AnnouncementType.AdministrationEventAnnounce;
                    //盘点任务
                    if(model.BusinessType==BusinessType.AssetInventory)
                    {
                        type=AnnouncementType.AssetInventory;
                    }
                    
                    var annocumentModel = new AnnouncementModel()
                    {
                        Type = type,
                        Title = model.Title,
                        Content = model.MessageContent,
                        ApplyUserId =CommonInstance.GetInstance().LoginUser.Id,
                        EffectiveDays = 30,
                        CreateUserId = CommonInstance.GetInstance().LoginUser.Id,
                        CreatedTime = DateTime.Now,
                        Status = AnnouncementStatus.AllowPublish,
                    };
                    this.AnnouncementService.AddAnnouncement(annocumentModel);
                    //if (model.BusinessType == BusinessType.AssetInventory)
                    //{
                    //    int inventoryId = this.AssetInventoryService.AddAssetInventory(new AssetInventoryModel()
                    //    {
                    //        Title = model.Title,
                    //        Description = model.MessageContent,
                    //        Status = AssetInventoryStatus.Open
                    //    });
                    //    model.MessagePrams = inventoryId.ToString();
                    //}
                    //bool isProduction = false;
                    //if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
                    //{
                    //    isProduction = true;
                    //}
                    //this.NotificationService.Add(model, isProduction);
                    ////资产盘点发通知后需要创建新的盘点任务

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            return View(model);
        }

    }
}