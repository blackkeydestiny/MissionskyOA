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
    [AuthorizeFilter]
    public class MeetingController : Controller
    {
        IMeetingService MeetingService;
       //
        // GET: /Area/

        public MeetingController(IMeetingService meetingService)
        {
            this.MeetingService = meetingService;
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
            var areaResult = MeetingService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
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
                ViewBag.Title = "编辑会议室信息";
                var model = this.MeetingService.GetMeetingRoomById(id.Value);
                return View(model);
            }  
            else
            {
                ViewBag.Title = "添加会议室";
                MeetingRoomModel model = new MeetingRoomModel();
                return View(model);
            }
        }

        
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(MeetingRoomModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.MeetingRoomName))
            {
                ModelState.AddModelError("MeetingRoomName", "请输入会议室名.");
            }

            if (string.IsNullOrEmpty(model.Equipment))
            {
                ModelState.AddModelError("Equipment", "请输入设备.");
            }

            if ((model.Capacity)==0)
            {
                ModelState.AddModelError("Capacity", "请输入可容纳人数.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {

                        this.MeetingService.UpdateMeetingRoom(model);
                          
                    }
                    else
                    {
                        model.CreateDate = DateTime.Now;
                        model.CreateUserName = CommonInstance.GetInstance().LoginUser.EnglishName;
                        this.MeetingService.AddMeetingRoom(model);
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
                this.MeetingService.Remove(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult GetMeetingCalendar([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {

                var meetingList = MeetingService.GetMeetingListByMeetingRoomId(Id);
                DataSourceResult result = new DataSourceResult()
                {
                    Data = meetingList
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult GetMeetingParticipant([DataSourceRequest]DataSourceRequest dRequest, int Id)
        {
            try
            {
                var meeting = MeetingService.GetMeetingDetailsById(Id);
                List<MeetingParticipantModel> participantList =new List<MeetingParticipantModel>();
                foreach(MeetingParticipantModel item in meeting.MeetingParticipants )
                {
                    participantList.Add(item);
                }

                DataSourceResult result = new DataSourceResult()
                {
                    Data = participantList
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
