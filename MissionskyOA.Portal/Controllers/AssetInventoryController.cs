using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Portal.Common;
using MissionskyOA.Portal.Extionsions;
using MissionskyOA.Portal.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class AssetInventoryController : Controller
    {
        IUserService UserService;
        IAssetInventoryService AssetInventoryService;
        IAssetService AssetService;
        //
        // GET: /Area/

        public AssetInventoryController(IUserService userService, IAssetInventoryService assetInventoryService, IAssetService assetService)
        {
            this.UserService = userService;
            this.AssetInventoryService = assetInventoryService;
            this.AssetService = assetService;

            var inventories = new List<SelectListItem>();
            this.AssetInventoryService.GetInventories().ForEach(it =>
            {
                inventories.Add(new SelectListItem() { Text = it.Title + "-------" + it.Status.ToString(), Value = it.Id.ToString() });
            });
            ViewBag.Inventories = inventories;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(AssetInventoryViewModel model)
        {
            string action = Request["Submit"];
            if (action == "close")
            {
                if (model.InventoryId > 0)
                {
                    this.AssetInventoryService.UpdateAssetInventoryStatus(model.InventoryId, AssetInventoryStatus.Closed);
                }
            }
            else if (action == "open")
            {
                if (model.InventoryId > 0)
                {
                    this.AssetInventoryService.UpdateAssetInventoryStatus(model.InventoryId, AssetInventoryStatus.Open);
                }
            }
            else if (action == "exportNoScanInfo")
            {
                //导出某次盘点未扫描的资产信息
                try
                {
                    var assetInventory = this.AssetInventoryService.GetAssetInventory(model.InventoryId);
                    var scanedAssetIds = new List<int>();
                    if (assetInventory != null && assetInventory.AssetInventoryRecords != null)
                    {
                        scanedAssetIds = assetInventory.AssetInventoryRecords.Where(it => it.AssetId.HasValue).Select(it => it.AssetId.Value).Distinct().ToList();
                    }
                    var workbook = new Aspose.Cells.Workbook();
                    var descriptor = new ExportExcel<AssetModel>();

                    AssetSearchModel search = new AssetSearchModel()
                    {
                        BarCode = null,
                        TypeId = null,
                        BuyDate = null,
                        UserName = null
                    };
                    var assets = this.AssetService.List(search).Where(it => !scanedAssetIds.Contains(it.Id)).ToList();
                    descriptor.FillAssetData(assets, workbook, 0);

                    var ms = new MemoryStream();
                    string fileName = assetInventory.Title + "(暂未扫描的资产信息)-" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                    workbook.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                    return File(ms.ToArray(), "application/vnd.ms-excel", fileName);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 查看盘点任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult View(int id)
        {
            var model = this.AssetInventoryService.GetAssetInventory(id);
            return View(model);
        }

    }
}