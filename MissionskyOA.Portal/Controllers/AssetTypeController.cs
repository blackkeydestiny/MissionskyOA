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
    public class AssetTypeController : Controller
    {
        IAssetTypeService AssetTypeService;
        IAssetAttributeService AssetAttributeService;

        public AssetTypeController(IAssetTypeService assetTypeService, IAssetAttributeService assetAttributeService)
        {
            this.AssetTypeService = assetTypeService;
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
            var noticeResult = AssetTypeService.List(dRequest.Page, dRequest.PageSize, sort, filter);
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
                var model = this.AssetTypeService.GetTypeById(id.Value);
                return View(model);
            }
            else
            {
                AssetTypeModel model = new AssetTypeModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AssetTypeModel model)
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
                        this.AssetTypeService.Update(model);
                    }
                    else
                    {
                        this.AssetTypeService.Add(model);
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

        //设置属性
        public ActionResult SetAttributes(int id)
        {
            var model = this.AssetTypeService.GetTypeById(id);

            var sourceAttributes = new List<SelectListItem>();
            var attributes = new List<SelectListItem>();
            var assetAttributes = this.AssetAttributeService.GetAll();
            //已关联的属性
            if (model.Attributes != null && model.Attributes.Count > 0)
            {
                model.Attributes.ForEach(it =>
                {
                    attributes.Add(new SelectListItem() { Text = it.Name, Value = it.Id.ToString() });
                    var toBeDeletedItem = assetAttributes.Where(item => item.Id == it.Id).FirstOrDefault();
                    if (toBeDeletedItem != null)
                    {
                        assetAttributes.Remove(toBeDeletedItem);
                    }
                });
            }

            //待选的属性
            assetAttributes.ForEach(it =>
            {
                sourceAttributes.Add(new SelectListItem() { Text = it.Name, Value = it.Id.ToString() });
            });

            ViewBag.SourceAttributes = sourceAttributes;
            ViewBag.Attributes = attributes;
            return View(model);
        }

        [HttpPost]
        public ActionResult SetAttributes(AssetTypeModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            try
            {
                this.AssetTypeService.SetAttributes(model);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.AssetTypeService.Remove(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

    }
}