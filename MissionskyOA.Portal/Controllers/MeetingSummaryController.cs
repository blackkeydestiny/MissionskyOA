using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using MissionskyOA.Portal.Common;

namespace MissionskyOA.Portal.Controllers
{
    public class MeetingSummaryController : Controller
    {
        IMeetingService MeetingService;
       //
        // GET: /Area/

        public MeetingSummaryController(IMeetingService meetingService)
        {
            this.MeetingService = meetingService;
        }

        //
        // GET: /MeetingSummary/Details/5
        public ActionResult Details(int id)
        {
            MeetingCalendarModel model = new MeetingCalendarModel();
            if(id==0)
            {

            }
            model = this.MeetingService.GetMeetingDetailsById(id);
            return View(model);
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Details(MeetingCalendarModel model)
        {
            string action = Request["Submit"];

            if (string.IsNullOrEmpty(model.MeetingSummary))
            {
                ModelState.AddModelError("MeetingSummary", "请输入会议纪要.");
            }
          
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {

                        this.MeetingService.UpdateMeetingSummary(model);
                        ViewBag.Message = "更新成功";
                    }
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
