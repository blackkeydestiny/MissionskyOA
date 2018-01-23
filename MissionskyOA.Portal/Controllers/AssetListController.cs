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
    public class AssetListController : Controller
    {
        IUserService UserService;
        INotificationService NotificationService;
        //
        // GET: /Area/

        public AssetListController(IUserService userService, INotificationService notificationService)
        {
            this.UserService = userService;
            this.NotificationService = notificationService;
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

            if (action == "test")
            {
                model = new NotificationModel()
                {
                    Target = "lex.wu@missionsky.com",
                    CreatedUserId = 95,
                    //MessageType = NotificationType.PushMessage,
                    MessageType = NotificationType.Email,
                    BusinessType = BusinessType.ExpressMessage,
                    Title = "Missionsky OA Notification",
                    MessageContent = "你有新的快递,Test已经签收!",
                    MessagePrams = "test",
                    Scope = NotificationScope.User,
                    TargetUserIds = new List<int> { 95 }
                };


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
                    bool isProduction = false;
                    if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
                    {
                        isProduction = true;
                    }
                    this.NotificationService.Add(model, isProduction);
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