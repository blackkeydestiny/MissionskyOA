using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Portal.Controllers
{
    /// <summary>
    /// 实时任务
    /// </summary>
    public class TaskController : Controller
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(TaskController));

        private IScheduledTaskService ScheduledTaskService;

        public TaskController(IScheduledTaskService scheduledTaskService)
        {
            this.ScheduledTaskService = scheduledTaskService;
        }

        //
        // GET: /Task/
        public ActionResult Index()
        {
            return View();
        }
        
        //
        // GET: /Task/History/5
        public ActionResult History(int taskId)
        {
            var task = ScheduledTaskService.GetScheduledTaskDetail(taskId);

            if (task == null)
            {
                Log.Error("找不到定时任务。");
                throw new KeyNotFoundException("找不到定时任务。");
            }

            ViewData["Task"] = task;
            return View();
        }

        /// <summary>
        /// 手动执行定时任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ActionResult Execute(int taskId)
        {
            try
            {
                var task = ScheduledTaskService.GetScheduledTaskDetail(taskId);
                if (task == null)
                {
                    throw new KeyNotFoundException(string.Format("找不到定时任务，Id: {0}。", taskId));
                }

                ScheduledTaskService.Execute(task);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ViewBag.Message = ex.Message;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 重启所有定时任务
        /// </summary>
        /// <returns></returns>
        public ActionResult Restart()
        {
            try
            {
                ScheduledTaskService.Start(); //开始定时任务
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ViewBag.Message = ex.Message;
            }

            return RedirectToAction("Index");
        }
        
        //
        // GET: /Task/Edit/1
        public ActionResult Edit(int? id)
        {
            var selectedStatus = string.Empty; //选择状态
            var selectedUnit = string.Empty; //选择单位
            ScheduledTaskModel task = new ScheduledTaskModel();

            if (id.HasValue && id.Value > 0) //修改
            {
                task = ScheduledTaskService.GetScheduledTaskDetail(id.Value);

                //选择单位
                selectedUnit = task.Unit.ToString();

                //选择状态
                selectedStatus = task.Status.ToString();
            }

            InitScheduledTaskStatus(selectedStatus);
            InitScheduledTaskUnits(selectedUnit);
            return View(task);
        }

        [HttpPost]
        public ActionResult Edit(ScheduledTaskModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", "请输入定时任务名称.");
            }

            if (model.Interval <= 0)
            {
                ModelState.AddModelError("Interval", "请输入执行时间间隔.");
            }

            if (model.Unit== ScheduledTaskUnit.None)
            {
                ModelState.AddModelError("Unit", "请选择执行时间间隔单位.");
            }
            
            if (string.IsNullOrEmpty(model.TaskClass))
            {
                ModelState.AddModelError("TaskClass", "请输入定时任务处理类.");
            }

            // 判断页面更新是否有效，是就去更新数据库
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id <= 0)
                    {
                        // 添加任务
                        ScheduledTaskService.Add(model);
                    }
                    else
                    {
                        //更新任务
                        ScheduledTaskService.Update(model);
                    }

                    //
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            // 不是，留在编辑页面
            InitScheduledTaskStatus(((int)model.Status).ToString());
            InitScheduledTaskUnits(((int)model.Unit).ToString());
            return View(model);
        }

        /// <summary>
        /// 启用禁用定时任务
        /// </summary>
        /// <param name="id"></param>
        public ActionResult Enable(int id)
        {
            if (id > 0)
            {
                ScheduledTaskService.Enable(id);
            }

            return View("Index");
        }

        /// <summary>
        /// 监听线程
        /// </summary>
        /// <returns></returns>
        public ActionResult Monitor()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 异步加载定时任务
        /// </summary>
        /// <param name="dRequest">数据请求</param>
        /// <returns></returns>
        public ActionResult Read([DataSourceRequest]DataSourceRequest dRequest)
        {
            var areaResult = ScheduledTaskService.TaskList(dRequest.Page, dRequest.PageSize, null, null);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }
        /// <summary>
        /// 异步加载定时任务执行记录
        /// </summary>
        /// <param name="dRequest">数据请求</param>
        /// <param name="taskId">定时任务Id</param>
        /// <returns></returns>
        public ActionResult ReadHistory([DataSourceRequest]DataSourceRequest dRequest, int taskId)
        {
            var areaResult = ScheduledTaskService.TaskHistoryList(dRequest.Page, dRequest.PageSize, null, null, taskId);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
            };

            return Json(result);
        }

        /// <summary>
        /// 设置时间间隔单位
        /// </summary>
        /// <param name="selectedUnit"></param>
        private void InitScheduledTaskUnits(string selectedUnit)
        {
            ArrayList unitLists = EnumExtensions.GetEmnuValueList(typeof (ScheduledTaskUnit));

            var units = new List<SelectListItem>();
            units.Add(new SelectListItem() {Value = string.Empty, Text = "请选择时间间隔单位"});

            if (unitLists != null)
            {
                foreach (var unit in unitLists)
                {
                    if (unit.ToString()
                        .Equals(ScheduledTaskUnit.None.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    units.Add(new SelectListItem()
                    {
                        Value = ((int) Enum.Parse(typeof (ScheduledTaskUnit), unit.ToString())).ToString(),
                        Text = unit.ToString()
                    });
                }
            }

            ViewData["Units"] = new SelectList(units, "Value", "Text", selectedUnit);
        }

        /// <summary>
        /// 设置定时任务状态
        /// </summary>
        /// <param name="selectedStatus"></param>
        private void InitScheduledTaskStatus(string selectedStatus)
        {
            ArrayList statusLists = EnumExtensions.GetEmnuValueList(typeof(ScheduledTaskStatus));

            var statuses = new List<SelectListItem>();
            statuses.Add(new SelectListItem() { Value = string.Empty, Text = "请选择定时任务状态" });

            if (statusLists != null)
            {
                foreach (var status in statusLists)
                {
                    if (status.ToString()
                        .Equals(ScheduledTaskStatus.None.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    statuses.Add(new SelectListItem()
                    {
                        Value = ((int)Enum.Parse(typeof(ScheduledTaskStatus), status.ToString())).ToString(),
                        Text = status.ToString()
                    });
                }
            }

            ViewData["TaskStatus"] = new SelectList(statuses, "Value", "Text", selectedStatus);
        }
    }
}