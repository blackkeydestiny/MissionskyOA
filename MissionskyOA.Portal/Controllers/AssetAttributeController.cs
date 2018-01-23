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
    public class AssetAttributeController : Controller
    {
        IAssetAttributeService AssetAttributeService;

        public AssetAttributeController(IAssetAttributeService assetAttributeService)
        {
            this.AssetAttributeService = assetAttributeService;
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
            var noticeResult = AssetAttributeService.List(dRequest.Page, dRequest.PageSize, sort, filter);
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
                var model = this.AssetAttributeService.GetAttributeById(id.Value);
                return View(model);
            }
            else
            {
                AssetAttributeModel model = new AssetAttributeModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AssetAttributeModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入名称.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {
                        this.AssetAttributeService.Update(model);
                    }
                    else
                    {
                        this.AssetAttributeService.Add(model);
                    }
                    
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.AssetAttributeService.Remove(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


    }
}