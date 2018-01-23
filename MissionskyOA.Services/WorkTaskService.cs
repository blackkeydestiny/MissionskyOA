using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Services.Extentions;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 工作任务
    /// </summary>
    public class WorkTaskService: ServiceBase, IWorkTaskService
    {
        private readonly INotificationService _notificationService = new NotificationService();
        private readonly IMeetingService _meetingService = new MeetingService();
        private readonly IUserService _userService = new UserService();
        private readonly IWorkTaskCommentService _workTaskCommentService = new WorkTaskCommentService();
        private readonly IWorkTaskHistoryService _workTaskHistoryService = new WorkTaskHistoryService();
        private readonly IAttachmentService _attachmentService = new AttachmentService();

        /// <summary>
        /// 创建新工作任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="currentUser"></param>
        /// <returns>返回新的Task id</returns>
        public int Add(NewWorkTaskModel task, UserModel currentUser)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //创建工作任务
                var entity = task.ToEntity();
                dbContext.WorkTasks.Add(entity);

                //添加操作记录
                AddHistory(dbContext, currentUser.Id, entity.Id, WorkTaskStatus.Created, string.Format("{0}创建了一个工作任务: {1}。", currentUser.EnglishName, task.Outline));

                //添加消息推送
                AddNotification(dbContext, task.ToModel(), currentUser);

                dbContext.SaveChanges();

                return entity.Id;
            }
        }

        /// <summary>
        /// 修改工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="task"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public bool Edit(int taskId, NewWorkTaskModel task, UserModel currentUser)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.WorkTasks.FirstOrDefault(it => it.Id == taskId);
                if (entity == null)
                {
                    Log.Error(string.Format("任务无效, 任务ID: {0}", taskId));
                    throw new InvalidOperationException("任务无效");
                }

                int oldSupervisor = entity.Supervisor == null ? task.Sponsor : entity.Supervisor.Value;  //原来的监督人
                int newSupervisor = task.Supervisor == null ? task.Sponsor : task.Supervisor.Value;  //新的监督人
                int oldExecutor = entity.Executor == null ? task.Sponsor : entity.Executor.Value;  //原来的执行人
                int newExecutor = task.Executor == null ? task.Sponsor : task.Executor.Value;  //新的执行人

                entity.Outline = task.Outline;
                entity.Type = (int)task.Type;
                entity.Desc = task.Desc;
                entity.MeetingId = task.MeetingId;
                entity.ProjectId = task.ProjectId;
                entity.Source = task.Source == null ? entity.Source : (int)task.Source;
                entity.Priority = task.Priority == null ? entity.Priority : (int)task.Priority;
                entity.Urgency = task.Urgency == null ? entity.Urgency : (int)task.Urgency;
                entity.Importance = task.Importance == null ? entity.Importance : (int)task.Importance;
                entity.EndTime = task.EndTime;
                entity.StartTime = task.StartTime;
                if (oldSupervisor != newSupervisor)
                {
                    entity.Supervisor = task.Supervisor;
                }

                if (oldExecutor != newExecutor)
                {
                    entity.Executor = task.Executor;
                }

                //添加推送消息
                AddNotification(dbContext, entity.ToModel(), currentUser);

                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 取消工作任务
        /// </summary>
        /// <param name="taskId">工作任务Id</param>
        /// <param name="season">取消工作任务原因</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public bool Cancel(int taskId, string season, UserModel currentUser)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //var entity = dbContext.WorkTasks.FirstOrDefault(it => it.Id == taskId);
                //if (entity == null)
                //{
                //    Log.Error(string.Format("任务无效, 任务ID: {0}", taskId));
                //    throw new InvalidOperationException("任务无效");
                //}

                //if (entity.Status != (int) WorkTaskStatus.Created)
                //{
                //    Log.Error("任务已开始,不能取消。");
                //    throw new InvalidOperationException("任务已开始,不能取消。");
                //}

                //entity.Status = (int)WorkTaskStatus.Canceled;

                //AddHistory(dbContext, currentUser.Id, taskId, WorkTaskStatus.Canceled,
                //    string.Format("{0}取消了任务，原因: {1}。", currentUser.EnglishName, season)); //记录日志

                //AddNotification(dbContext, entity.ToModel(), currentUser, season); //推送消息

                //dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 转移(指派)工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="executor">执行人</param>
        /// <param name="currentUser"></param>
        /// <param name="comment">备注信息</param>
        /// <returns></returns>
        public bool Transfer(int taskId, int executor, UserModel currentUser, string comment = null)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //var entity = dbContext.WorkTasks.FirstOrDefault(it => it.Id == taskId);
                //if (entity == null)
                //{
                //    Log.Error(string.Format("任务无效, 任务ID: {0}", taskId));
                //    throw new InvalidOperationException("任务无效");
                //}
                
                //entity.Executor = executor; //更改执行人
                //entity.Status = (int) WorkTaskStatus.Transfered;

                //AddHistory(dbContext, currentUser.Id, taskId, WorkTaskStatus.Transfered,
                //    string.Format("{0}更改了执行人，备注: {1}。", currentUser.EnglishName, comment)); //记录日志

                ////添加推送消息
                //AddNotification(dbContext, entity.ToModel(), currentUser);

                //dbContext.SaveChanges();

                return true;
            }
        }
        
        /// <summary>
        /// 执行工作任务工作任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextStatus"></param>
        /// <param name="currentUser"></param>
        /// <param name="comment">备注信息</param>
        /// <returns></returns>
        public bool Do(int taskId, WorkTaskStatus nextStatus, UserModel currentUser, string comment = null)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.WorkTasks.FirstOrDefault(it => it.Id == taskId);
                if (entity == null)
                {
                    Log.Error(string.Format("任务无效, 任务ID: {0}", taskId));
                    throw new InvalidOperationException("任务无效");
                }

                entity.Status = (int)nextStatus; //状态流转

                #region 添加操作日志
                switch (nextStatus)
                {
                    case WorkTaskStatus.Started:
                        AddHistory(dbContext, currentUser.Id, taskId, nextStatus,
                            string.Format("{0}开始执行工作任务，备注: {1}。", currentUser.EnglishName, comment)); //记录日志
                        break;
                    case WorkTaskStatus.Completed:
                        AddHistory(dbContext, currentUser.Id, taskId, nextStatus,
                            string.Format("{0}完成了工作任务，备注: {1}。", currentUser.EnglishName, comment)); //记录日志
                        break;
                    case WorkTaskStatus.Closed:
                        AddHistory(dbContext, currentUser.Id, taskId, nextStatus,
                            string.Format("{0}关闭了工作任务，备注: {1}。", currentUser.EnglishName, comment)); //记录日志
                        break;
                    default:
                        Log.Error("无效的工作任务操作。");
                        throw new InvalidOperationException("无效的工作任务操作。");
                }
                #endregion

                //添加推送消息
                AddNotification(dbContext, entity.ToModel(), currentUser);

                dbContext.SaveChanges();

                return true;
            }
        }

        /// <summary>
        /// 添加操作记录
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="userId"></param>
        /// <param name="taskId"></param>
        /// <param name="status"></param>
        /// <param name="auditMsg"></param>
        public void AddHistory(MissionskyOAEntities dbContext, int userId, int taskId, WorkTaskStatus status, string auditMsg)
        {
            var history = new WorkTaskHistoryModel()
            {
                TaskId = taskId,
                Operator = userId,
                Status = status,
                Audit = auditMsg,
                CreatedTime = DateTime.Now
            };

            _workTaskHistoryService.AddHistory(dbContext, history);
        }

        /// <summary>
        /// 工作流处理消息推送
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="task"></param>
        /// <param name="sender">推送消息发送人</param>
        /// <param name="extParam">扩展参数</param>
        /// <remarks>receiverId = 0: 表示默认发送到代申请人</remarks>
        public void AddNotification(MissionskyOAEntities dbContext, WorkTaskModel task, UserModel sender,
            object extParam = null)
        {
            //获取工作任务发起人
            var sponsor = dbContext.Users.FirstOrDefault(it => it.Id == task.Sponsor);
            if (sponsor == null)
            {
                Log.Error(string.Format("工作任务发起人无效, 发起人Id: {0}", task.Sponsor));
                throw new InvalidOperationException("工作任务发起人无效");
            }

            //获取工作任务监督人
            var supervisor = task.Sponsor == task.Supervisor
                ? sponsor
                : dbContext.Users.FirstOrDefault(it => it.Id == task.Supervisor);
            if (supervisor == null)
            {
                Log.Error(string.Format("工作任务监督人无效, 监督人Id: {0}", task.Supervisor));
                throw new InvalidOperationException("工作任务监督人无效");
            }

            //获取工作任务监督人
            var executor = task.Sponsor == task.Executor
                ? sponsor
                : dbContext.Users.FirstOrDefault(it => it.Id == task.Executor);
            if (executor == null)
            {
                Log.Error(string.Format("工作任务执行人无效, 执行人Id: {0}", task.Supervisor));
                throw new InvalidOperationException("工作任务执行人无效");
            }

            switch (task.Status)
            {
                case WorkTaskStatus.Created: //新建
                //case WorkTaskStatus.Transfered: //转移(指派)
                //    //推送给执行人
                //    SendNotification(sender, executor.ToModel(),
                //        string.Format("{0}给指派了一个工作任务：{1}", sender.EnglishName, task.Outline));

                //    //推送给监督人
                //    if (supervisor != executor)
                //    {
                //        SendNotification(sender, supervisor.ToModel(),
                //            string.Format("{0}给{1}指派了一个工作任务：{2}", sender.EnglishName, executor.EnglishName, task.Outline));
                //    }
                //    break;
                case WorkTaskStatus.Started: //开始
                    //推送给发起人
                    SendNotification(sender, sponsor.ToModel(),
                        string.Format("{0}开始执行工作任务：{1}", executor.EnglishName, task.Outline));

                    //推送给监督人
                    if (supervisor != sponsor)
                    {
                        SendNotification(sender, supervisor.ToModel(),
                            string.Format("{0}开始执行工作任务：{1}", executor.EnglishName, task.Outline));
                    }
                    break;
                case WorkTaskStatus.Completed: //完成
                    //推送给发起人
                    SendNotification(sender, sponsor.ToModel(),
                        string.Format("{0}已完成工作任务：{1}", executor.EnglishName, task.Outline));

                    //推送给监督人
                    if (supervisor != sponsor)
                    {
                        SendNotification(sender, supervisor.ToModel(),
                            string.Format("{0}已完成工作任务：{1}", executor.EnglishName, task.Outline));
                    }
                    break;
                //case WorkTaskStatus.Canceled: //取消
                //    //推送给执行人
                //    SendNotification(sender, executor.ToModel(),
                //        string.Format("{0}取消任务: {1}，原因：{2}", sender.EnglishName, task.Outline, extParam));

                //    //推送给监督人
                //    if (supervisor != sponsor)
                //    {
                //        SendNotification(sender, supervisor.ToModel(),
                //            string.Format("{0}取消任务: {1}，原因：{2}", sender.EnglishName, task.Outline, extParam));
                //    }
                //    break;
                case WorkTaskStatus.Closed: //关闭任务
                    //推送给执行人
                    SendNotification(sender, executor.ToModel(),
                        string.Format("{0}取消任务: {1}，原因：{2}", sender.EnglishName, task.Outline, extParam));

                    //推送给监督人
                    if (supervisor != sponsor)
                    {
                        SendNotification(sender, supervisor.ToModel(),
                            string.Format("{0}取消任务: {1}，原因：{2}", sender.EnglishName, task.Outline, extParam));
                    }
                    break;
            }
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="notification"></param>
        private void SendNotification(UserModel sender, UserModel receiver, string notification)
        {
            var model = new NotificationModel()
            {
                Target = receiver.Email,
                CreatedUserId = sender.Id,
                //MessageType = NotificationType.PushMessage,
                MessageType = NotificationType.Email,
                BusinessType = BusinessType.Approving,
                Title = "Missionsky OA Notification",
                MessagePrams = "工作任务管理消息",
                Scope = NotificationScope.User,
                CreatedTime = DateTime.Now,
                TargetUserIds = new List<int> { receiver.Id },
                MessageContent = notification
            };

            this._notificationService.Add(model, Global.IsProduction); //消息推送
        }
        
        /// <summary>
        /// 分页条件查询工作任务
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>满足条件的工作任务</returns>
        public IPagedList<WorkTaskModel> SearchByCriteria(SearchWorkTaskModel search, int pageIndex, int pageSize)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var query = DoSearch(dbContext, search); //查询
                var result = new List<WorkTaskModel>();

                query.ToList().ForEach(entity => result.Add(DoFill(entity.ToModel())));

                return new PagedList<WorkTaskModel>(result, pageIndex, pageSize);
            }
        }

        /// <summary>
        /// 根据工作任务ID获取详细信息
        /// </summary>
        /// <param name="taskId">工作任务id</param>
        /// <returns>工作任务详细信息</returns>
        public WorkTaskModel GetWorkTaskDetail(int taskId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.WorkTasks.FirstOrDefault(it => it.Id == taskId);

                if (entity == null)
                {
                    Log.Error(string.Format("找不到指定ID的工作任务, Id: {0}", taskId));
                    throw new KeyNotFoundException("找不到指定ID的工作任务。");
                }
                return DoFill(entity.ToModel(), true);
            }
        }

        /// <summary>
        /// 查询工作任务
        /// </summary>
        /// <param name="dbContext">数据库上下文对象</param>
        /// <param name="search">查询条件</param>
        /// <returns>满足条件的工作任务</returns>
        private IEnumerable<WorkTask> DoSearch(MissionskyOAEntities dbContext, SearchWorkTaskModel search)
        {
            search = search ?? new SearchWorkTaskModel();

            //状态查询
            var query =  dbContext.WorkTasks.Where(
                    it =>
                        !search.Status.HasValue || search.Status.Value == WorkTaskStatus.Invalid || //查询所有
                        it.Status == (int) search.Status.Value);//查询指定状态

            query = query.Where(it => !search.Sponsor.HasValue || search.Sponsor == 0 || it.Sponsor == search.Sponsor.Value); //指定发起人
            query = query.Where(it => !search.Executor.HasValue || search.Executor == 0 || it.Sponsor == search.Executor.Value); //指定执行人
            query = query.Where(it => !search.Supervisor.HasValue || search.Supervisor == 0 || it.Supervisor == search.Supervisor.Value); //指定监督人
            query = query.Where(it => !search.ProjectId.HasValue || search.ProjectId == 0 || it.ProjectId == search.ProjectId.Value); //项目Id查找
            query = query.Where(it => !search.MeetingId.HasValue || search.MeetingId == 0 || it.MeetingId == search.MeetingId.Value); //会议Id查找

            return query;
        }
        
        /// <summary>
        /// 获取任务详细
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="isDetail">是否加载详细信息</param>
        /// <returns>图书</returns>
        public WorkTaskModel DoFill(WorkTaskModel task, bool isDetail = true)
        {
            if (task == null)
            {
                Log.Error("无效的工作任务。");
                throw new NullReferenceException("无效的工作任务。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //项目名
                if (task.ProjectId.HasValue)
                {
                    var project = dbContext.Projects.FirstOrDefault(it => it.Id == task.ProjectId.Value);
                    task.ProjectName = project == null ? string.Empty : project.Name;
                }

                //发起人姓名
                var sponsor = _userService.GetUserDetail(task.Sponsor);
                task.SponsorName = sponsor == null ? string.Empty : sponsor.EnglishName;

                //执行人
                if (task.Executor.HasValue)
                {
                    if (task.Executor.Value == task.Sponsor)
                    {
                        task.ExecutorName = task.SponsorName;
                    }
                    else
                    {
                        var executor = _userService.GetUserDetail(task.Executor.Value);
                        task.ExecutorName = executor == null ? string.Empty : executor.EnglishName;
                    }
                }
                
                //监督人
                if (task.Supervisor.HasValue)
                {
                    if (task.Supervisor.Value == task.Sponsor)
                    {
                        task.SupervisorName = task.SponsorName;
                    }
                    else
                    {
                        var supervisor = _userService.GetUserDetail(task.Supervisor.Value);
                        task.SupervisorName = supervisor == null ? string.Empty : supervisor.EnglishName;
                    }
                }

                //获取更多详细
                if (isDetail)
                {
                    //会议详细
                    task.Meeting = task.MeetingId.HasValue && task.MeetingId.Value > 0 ? _meetingService.GetMeetingDetailsById(task.MeetingId.Value) : null;

                    //工作任务评论
                    task.Comments = _workTaskCommentService.GetWorkTaskCommentList(task.Id);

                    //工作任务操作记录
                    task.Histories = _workTaskHistoryService.GetWorkTaskHistoryList(task.Id);

                    //附件
                    task.Attachments = _attachmentService.GetAttathcmentIds(dbContext, task.Id, Constant.ATTACHMENT_TYPE_WORK_TASK);
                }

                return task;
            }
        }

        /// <summary>
        /// 统计个人任务信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<WorkTaskSummaryModel> SummaryWorkTask(int userId)
        {
            if (userId < 0)
            {
                Log.Error("无效的用户id");
                throw new InvalidOperationException("无效的用户id");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var summaries = new List<WorkTaskSummaryModel>();
                var createdTask = new WorkTaskSummaryModel() { Type = "CreatedTask", Value = 0 };
                var executingTask = new WorkTaskSummaryModel() { Type = "ExecutingTask", Value = 0 };
                var compltedTask = new WorkTaskSummaryModel() { Type = "CompltedTask", Value = 0 };
                var uncompletedTask = new WorkTaskSummaryModel() { Type = "UncompletedTask", Value = 0 };
                summaries.Add(createdTask);
                summaries.Add(executingTask);
                summaries.Add(compltedTask);
                summaries.Add(uncompletedTask);

                var entities = dbContext.WorkTasks.Where(it => it.Sponsor == userId || it.Executor == userId);
                entities.ToList().ForEach(it =>
                {
                    var task = it.ToModel();

                    if (task.Sponsor == userId)
                    {
                        createdTask.Value++;
                    }

                    if (task.Executor == userId)
                    {
                        if (task.Status == WorkTaskStatus.Started)
                        {
                            executingTask.Value++;
                        }
                        else if (task.Status == WorkTaskStatus.Completed)
                        {
                            compltedTask.Value++;
                        }
                        else if (task.Status == WorkTaskStatus.Paused)
                        {
                            uncompletedTask.Value++;
                        }
                    }
                });

                return summaries;
            }
        }

        /// <summary>
        /// 分页获取工作任务信息
        /// </summary>
        /// <returns>获取会工作任务信息</returns>
        public ListResult<WorkTaskModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.WorkTasks.AsEnumerable();

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<WorkTaskModel> result = new ListResult<WorkTaskModel>();
                result.Data = new List<WorkTaskModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }
    }
}
