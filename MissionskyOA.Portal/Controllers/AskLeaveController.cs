﻿using Kendo.Mvc;
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
    public class AskLeaveController : Controller
    {
        //
        // GET: /AskLeave/
        public ActionResult Index()
        {
            return View();
        }

         private IAskLeaveService AskLeaveService;
         private IOvertimeService OvertimeService;
        //
        // GET: /Area/

         public AskLeaveController(IAskLeaveService AskLeaveService, IOvertimeService OvertimeService)
        {
            this.AskLeaveService = AskLeaveService;
            this.OvertimeService = OvertimeService;
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
            var areaResult = AskLeaveService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 查找具体请假单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewAskLeaveDetail([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                var orderDetailResult = OvertimeService.GetOvertimeDetailsByOrderID(Id);
                DataSourceResult result = new DataSourceResult()
                {
                    Data = orderDetailResult.Data,
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        /// <summary>
        /// 查找具体流程记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ViewWorkFlowDetail([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                var orderDetailResult = AskLeaveService.GetOrderDetail(Id);
                List<WorkflowProcessModel> workflowHistory=new List<WorkflowProcessModel>();

                foreach(WorkflowProcessModel item in orderDetailResult.ProcessHistory)
                {
                    workflowHistory.Add(item);
                }
                DataSourceResult result = new DataSourceResult()
                {
                    Data = workflowHistory
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        
	}
}