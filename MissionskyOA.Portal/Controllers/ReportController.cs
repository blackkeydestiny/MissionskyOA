using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using MissionskyOA.Core.Common;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Portal.Controllers
{
    /// <summary>
    /// 报表
    /// </summary>
    public class ReportController : Controller
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ReportController));
        
        private IReportService ReportService;

        public ReportController(IReportService reportService)
        {
            this.ReportService = reportService;
        }

        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }
        
        //
        // GET: /Report/Edit/1
        public ActionResult Edit(int? id)
        {
            var selectedFormat = string.Empty; //选择格式
            ReportModel report = new ReportModel();
            NameValueCollection reportConfigs = new NameValueCollection();

            if (id.HasValue && id.Value > 0) //修改
            {
                report = ReportService.GetReportDetail(id.Value);
                reportConfigs = ReportService.GetReportConfigs(id.Value);

                //流程类型
                selectedFormat = reportConfigs[Constant.REPORT_CONFIG_DEFAULT_FORMAT];
            }
            
            InitReportFormats(selectedFormat);
            ViewData["ReportId"] = id;
            ViewData["ReportConfigs"] = reportConfigs;
            ViewData["ReportParameters"] = report.Parameters ?? new List<ReportParameterModel>();

            return View(report);
        }

        [HttpPost]
        public ActionResult Edit(ReportModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            var defaultFormat = Request[Constant.REPORT_CONFIG_DEFAULT_FORMAT];
            var serviceUrl = Request[Constant.REPORT_CONFIG_SERVICE_URL];
            var reportPath = Request[Constant.REPORT_CONFIG_REPORT_PATH];
            var userName = Request[Constant.REPORT_CONFIG_USER_NAME];
            var password = Request[Constant.REPORT_CONFIG_PASSWORD];
            var domain = Request[Constant.REPORT_CONFIG_DOMAIN];
            
            #region 数据验证

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入报表名称.");
            }

            if (string.IsNullOrEmpty(defaultFormat))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_DEFAULT_FORMAT, "请选择报表格式.");
            }

            if (string.IsNullOrEmpty(serviceUrl))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_SERVICE_URL, "请输入报表服务地址.");
            }

            if (string.IsNullOrEmpty(reportPath))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_REPORT_PATH, "请输入报表路径.");
            }

            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_USER_NAME, "请输入用户名.");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_PASSWORD, "请输入用户密码.");
            }

            if (string.IsNullOrEmpty(domain))
            {
                ModelState.AddModelError(Constant.REPORT_CONFIG_DOMAIN, "请输入用户域.");
            }

            #endregion

            var configs = new NameValueCollection();
            configs.Add(Constant.REPORT_CONFIG_DEFAULT_FORMAT, defaultFormat);
            configs.Add(Constant.REPORT_CONFIG_SERVICE_URL, serviceUrl);
            configs.Add(Constant.REPORT_CONFIG_REPORT_PATH, reportPath);
            configs.Add(Constant.REPORT_CONFIG_USER_NAME, userName);
            configs.Add(Constant.REPORT_CONFIG_PASSWORD, password);
            configs.Add(Constant.REPORT_CONFIG_DOMAIN, domain);

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id <= 0)
                    {
                        var reportId = ReportService.AddReport(model, configs);
                        return RedirectToAction("Edit", new {id = reportId});
                    }
                    else
                    {
                        ReportService.UpdateReport(model, configs);
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            ViewData["ReportConfigs"] = configs;
            InitReportFormats(defaultFormat);
            return View(model);
        }

        /// <summary>
        /// 删除报表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                ReportService.Delete(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { error = ex.Message });
            }
        }

        //
        // GET: /Report/EditParameter/1
        public ActionResult EditParameter(int rId, int? pId)
        {
            if (rId < 1)
            {
                return RedirectToAction("Edit");
            }

            var selectedType = string.Empty;
            ReportModel report = ReportService.GetReportDetail(rId);
            if (report == null)
            {
                Log.Error("找不到报表。");
                throw new KeyNotFoundException("找不到报表。");
            }

            ReportParameterModel parameter = new ReportParameterModel();

            if (pId.HasValue && pId.Value > 0) //修改
            {
                parameter = ReportService.GetParameterDetail(pId.Value);

                if (parameter == null || parameter.ReportId != rId)
                {
                    Log.Error("找不到参数。");
                    throw new KeyNotFoundException("找不到参数。");
                }

                //参数类型
                selectedType = parameter.Type;
            }

            InitParameterTypes(selectedType);
            ViewData["Report"] = report;
            parameter.ReportId = rId;

            return View(parameter);
        }

        [HttpPost]
        public ActionResult EditParameter(ReportParameterModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入报表参数名称.");
            }

            if (string.IsNullOrEmpty(model.Type))
            {
                ModelState.AddModelError("Type", "请选择报表参数类型.");
            }

            if (model.Type.Equals(Constant.REPORT_PARAM_TYPE_DROPDOWNLIST, StringComparison.InvariantCultureIgnoreCase) &&
                string.IsNullOrEmpty(model.DataTable))
            {
                ModelState.AddModelError("DataTable", "请输入参数数据源.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id <= 0)
                    {
                        ReportService.AddParameter(model);
                    }
                    else
                    {
                        ReportService.UpdateParameter(model);
                    }

                    return RedirectToAction("Edit", new { id = model.ReportId });
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ViewBag.Message = ex.Message;
                }
            }

            EditParameter(model.ReportId, model.Id);
            return View(model);
        }

        /// <summary>
        /// 删除报表参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteParameter(int id)
        {
            try
            {
                ReportService.DeleteParameter(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="dRequest">数据请求</param>
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

            var areaResult = ReportService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 设置报表格式
        /// </summary>
        /// <param name="selectedFormat">选择报表格式</param>
        private void InitReportFormats(string selectedFormat)
        {
            var formatList = ReportService.GetReportFormats();

            var formsts = new List<SelectListItem>();
            formsts.Add(new SelectListItem() { Value = string.Empty, Text = "请选择报表格式" });

            if (formatList != null)
            {
                formatList.ToList().ForEach(it => formsts.Add(new SelectListItem() { Value = it, Text = it }));
            }

            ViewData["ReportFormats"] = new SelectList(formsts, "Value", "Text", selectedFormat);
        }
        
        /// <summary>
        /// 设置报表参数类型
        /// </summary>
        /// <param name="selectedType">选择报表类开</param>
        private void InitParameterTypes(string selectedType)
        {
            var paramTypes = ReportService.GetReportParameterTypes();

            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Value = string.Empty, Text = "请选择参数类型" });

            if (paramTypes != null)
            {
                paramTypes.ToList().ForEach(it => types.Add(new SelectListItem() { Value = it, Text = it }));
            }

            ViewData["ParameterTypes"] = new SelectList(types, "Value", "Text", selectedType);
        }
	}
}