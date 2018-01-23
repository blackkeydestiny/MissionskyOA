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
    public class AnnouncementController : Controller
    {

         IAnnouncementService AnnouncementService;
        //
        // GET: /Area/

        public AnnouncementController(IAnnouncementService announcementService)
        {
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
            FilterModel filter = null;
            if (dRequest.Filters != null && dRequest.Filters.Count > 0)
            {
                filter = ((FilterDescriptor)dRequest.Filters[0]).ToSortModel();
            }
            var annocumentResult = AnnouncementService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = annocumentResult.Data,
                Total = annocumentResult.Total
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
            var businessTypeList = new List<SelectListItem>();
            businessTypeList.Add(new SelectListItem()
            {
                Text = "公告通知",
                Value = ((int)AnnouncementType.AdministrationEventAnnounce).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "资产盘点",
                Value = ((int)AnnouncementType.AssetInventory).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "活动消息",
                Value = ((int)AnnouncementType.ActivityMessage).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "公司消息",
                Value = ((int)AnnouncementType.CompanyNews).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "员工消息",
                Value = ((int)AnnouncementType.EmployeeeNews).ToString()
            });

            ViewData["businessTypeList"] = new SelectList(businessTypeList, "Value", "Text", "0");
            if (id.HasValue && id.Value > 0)
            {
                var model = this.AnnouncementService.GetAnnouncementByID(id.Value);
                if (model.Type != null)
                {
                    ViewData["businessTypeList"] = new SelectList(businessTypeList, "Value", "Text", (int)model.Type);

                }
                return View(model);
            }
            else
            {
                AnnouncementModel model = new AnnouncementModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AnnouncementModel model)
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

            if (string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("Content", "请输入通知内容.");
            }
            if (model.EffectiveDays==null ||model.EffectiveDays==0)
            {
                ModelState.AddModelError("EffectiveDays", "请输入有效天数.");
            }
            if (ModelState.IsValid)
            {
                try
                {                                   
                    var annocumentModel = new AnnouncementModel()
                    {
                        Type = model.Type,
                        Title = model.Title,
                        Content = model.Content,
                        ApplyUserId =CommonInstance.GetInstance().LoginUser.Id,
                        EffectiveDays = model.EffectiveDays,
                        CreateUserId = CommonInstance.GetInstance().LoginUser.Id,
                        CreatedTime = DateTime.Now,
                        Status = AnnouncementStatus.AllowPublish,
                        RefAssetInventoryId=0
                    };
                    this.AnnouncementService.AddAnnouncement(annocumentModel);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                 
                    ViewBag.Message = ex.Message;
                }
            }
            var businessTypeList = new List<SelectListItem>();
            businessTypeList.Add(new SelectListItem()
            {
                Text = "公告通知",
                Value = ((int)AnnouncementType.AdministrationEventAnnounce).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "资产盘点",
                Value = ((int)AnnouncementType.AssetInventory).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "活动消息",
                Value = ((int)AnnouncementType.ActivityMessage).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "公司消息",
                Value = ((int)AnnouncementType.CompanyNews).ToString()
            });
            businessTypeList.Add(new SelectListItem()
            {
                Text = "员工消息",
                Value = ((int)AnnouncementType.EmployeeeNews).ToString()
            });
            ViewData["businessTypeList"] = new SelectList(businessTypeList, "Value", "Text", (int)model.Type);
            return View(model);
        }

    }
}
