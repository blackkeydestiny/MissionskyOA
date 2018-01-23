using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class AcceptProxyController : Controller
    {
        private IAcceptProxyService AcceptProxyService;

        public AcceptProxyController (IAcceptProxyService AcceptProxyService)
        {
            this.AcceptProxyService = AcceptProxyService;
        }
        //
        // GET: /AcceptProxy/
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
            // 排序
            SortModel sort = null;
            if (dRequest.Sorts != null && dRequest.Sorts.Count > 0)
            {
                sort = dRequest.Sorts[0].ToSortModel();
            }

            //过滤
            FilterModel filter = null;
            if (dRequest.Filters != null && dRequest.Filters.Count > 0)
            {
                filter = ((FilterDescriptor)dRequest.Filters[0]).ToSortModel();
            }

            // 列出所有的签收单列表
            var noticeResult = AcceptProxyService.List(dRequest.Page, dRequest.PageSize, sort, filter);

            DataSourceResult result = new DataSourceResult()
            {
                Data = noticeResult.Data,
                Total = noticeResult.Total
            };

            return Json(result);
        }
	}
}