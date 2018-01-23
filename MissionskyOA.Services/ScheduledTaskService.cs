using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;
using MissionskyOA.Services.Task;


namespace MissionskyOA.Services
{
    /// <summary>
    /// 定时任务处理
    /// </summary>
    public class ScheduledTaskService : ServiceBase, IScheduledTaskService
    {
        private readonly IBookService _bookService = new BookService();
        private readonly IAttendanceSummaryService _attendanceSummaryService = new AttendanceSummaryService();

        /// <summary>
        /// 线程池
        /// 私用、静态、只读
        /// </summary>
        private static readonly IList<Thread> MyThreadPool = new List<Thread>();

        #region 定时任务，线程处理

        /// <summary>
        /// 手动执行定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns></returns>
        public bool Execute(ScheduledTaskModel task)
        {

            if (task == null)
            {
                Log.Error("无效的定时任务。");
                throw new KeyNotFoundException("无效的定时任务。");
            }

            //1、实例化处理类对象
            var taskClass = Type.GetType(task.TaskClass);

            if (taskClass == null)
            {
                Log.Error(string.Format("不能初使化定时任务处理类: {0}。", task.TaskClass));
                throw new InvalidOperationException(string.Format("不能初使化定时任务处理类: {0}。", task.TaskClass));
            }

            //2、
            var taskService = (ITaskRunnable) Activator.CreateInstance(taskClass); //创建处理类对象

            //执行定时任务
            var executeDate = DateTime.Now; //执行时间
            //3、
            taskService.Execute(task, executeDate);
            task.LastExecTime = executeDate; //更新最后执行时间

            PrintMyThreads();

            return true;
        }

        /// <summary>
        /// 启动任务(默认启动所有任务)
        /// 
        /// </summary>
        /// <param name="tasks">定时任务I</param>
        public void Start(IList<ScheduledTaskModel> tasks = null)
        {
            // tasks有值，就直接执行任务。
            // tasks没有值，就去取出所有的值，再去循环执行任务

            // 1、判断传入的任务列表是否有任务，没有任务时，默认启动所有任务。
            // Count属性是ICollection的属性
            if (tasks == null || tasks.Count < 1) //默认启动所有任务
            {
                //从数据库中获取所有任务
                tasks = GetScheduledTasks();
            }

            //2、
            if (tasks != null)
            {
                // 启动线程去启动任务
                foreach (var task in tasks)
                {
                    StartThread(task);
                }
            }
        }

        /// <summary>
        /// 启用禁用定时任务
        /// </summary>
        /// <param name="id">定时任务Id</param>
        public void Enable(int id)
        {
            ScheduledTaskModel task = GetScheduledTaskDetail(id);

            if (task.Status == ScheduledTaskStatus.Started)
            {
                //禁用
                task.Status = ScheduledTaskStatus.Stopped;
                AbortThread(task);
            }
            else
            {
                //启用
                task.Status = ScheduledTaskStatus.Started;
                StartThread(task);
            }

            Update(task); //更新定时任务
        }

        /// <summary>
        /// 监听线程
        /// </summary>
        /// <returns>监听结果</returns>
        public NameValueCollection Monitor()
        {
            var tasks = GetScheduledTasks();
            var result = new NameValueCollection();
            tasks.ToList().ForEach(task =>
            {
                foreach (Thread thread in MyThreadPool)
                {
                    // 存在线程且集合中不存在
                    if (!result.AllKeys.Contains(task.Name))
                    {
                        if (thread.Name != null &&
                            thread.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            result.Add(task.Name, thread.ThreadState.ToString());
                        }
                        else
                        {
                            result.Add(task.Name, "Stopped");
                        }
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// 启动定时任务线程
        /// </summary>
        /// <param name="task">定时任务</param>
        private void StartThread(ScheduledTaskModel task)
        {
            if (task == null)
            {
                Log.Error("定时任务无效。");
                return;
            }

            // 检查此任务是否有任务执行处理类
            if (string.IsNullOrEmpty(task.TaskClass))
            {
                Log.Error(string.Format("{0}: 无任务执行处理类。", task.Name));
                return;
            }

            // 如果任务的状态不是启动状态，就终止任务的线程
            if (task.Status != ScheduledTaskStatus.Started) //未启动
            {
                // 终止线程
                AbortThread(task);
                return;
            }

            //终止已有线程
            if (AbortThread(task))
            {
                //重启创建线程
                var thread = new Thread(new ParameterizedThreadStart(ExecuteTask)) { Name = task.Name };
                
                //创建任务线程
                thread.Start(task);
                //将任务线程添加到线程池
                MyThreadPool.Add(thread);
                Log.Info(string.Format("启动定时任务: {0}。", task.Name));
            }
        }
        
        /// <summary>
        /// 终止定时任务线程
        /// </summary>
        /// <param name="task">定时任务</param>
        private bool AbortThread(ScheduledTaskModel task)
        {
            if (task == null)
            {
                Log.Error("定时任务无效。");
                return true;
            }

            //创建线程
            // 判断要执行任务线程是否在线程池中，返回一个线程对象
            var thread = HasExistedThread(task.Name);

            // 如果执行任务的线程存在在线程池中
            if (thread != null) //线程存在，终止线程
            {
                try
                {
                    // 从线程池中移除这个任务线程，并执行Abort操作
                    MyThreadPool.Remove(thread);
                    thread.Abort();
                    Log.Info(string.Format("终止定时任务: {0}。", task.Name));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            return true;
        }

        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="objTask">定时任务</param>
        private void ExecuteTask(object objTask)
        {
            if (!(objTask is ScheduledTaskModel))
            {
                Log.Error("无效的定时任务。");
                return;
            }

            var task = objTask as ScheduledTaskModel;
            task.LastExecTime = task.LastExecTime.HasValue ? task.LastExecTime.Value : DateTime.Now.AddDays(-1); //更新上一次执行时间

            do
            {
                if (!IsExecutedTask(task)) //未到执行时间
                {
                    continue;
                }

                Execute(task);
            } while (true);
        }

        /// <summary>
        /// 是否达到执行时间间隔
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns>true or false</returns>
        private bool IsExecutedTask(ScheduledTaskModel task)
        {
            if (!task.LastExecTime.HasValue)
            {
                return true; //马上执行
            }

            //计算执行时间间隔
            var interval = 0; //执行间隔(毫秒)
            switch (task.Unit)
            {
                case ScheduledTaskUnit.Second:
                    interval = task.Interval*1000;
                    break;
                case ScheduledTaskUnit.Minute:
                    interval = task.Interval*60*1000;
                    break;
                case ScheduledTaskUnit.Hour:
                    interval = task.Interval*60*60*1000;
                    break;
                case ScheduledTaskUnit.Day:
                    interval = task.Interval*24*60*60*1000;
                    break;
                default:
                    interval = 60*60*1000; //默认1小时执行一次
                    break;
            }

            //当前时间离上一次执行时间的时间差
            var timespan = new TimeSpan(DateTime.Now.Ticks).Subtract(new TimeSpan(task.LastExecTime.Value.Ticks));

            return (timespan.TotalMilliseconds > interval); //是否达到执行时间间隔
        }

        /// <summary>
        /// 是否存在执行定时任务的线程
        /// </summary>
        /// <param name="taskName">定时任务名称</param>
        /// <returns></returns>
        private Thread HasExistedThread(string taskName)
        {
            //
            if (MyThreadPool == null)
            {
                Log.Error("线程池异常。");
                throw new InvalidOperationException("线程池异常。");
            }

            //
            if (string.IsNullOrEmpty(taskName))
            {
                Log.Error("定时任务名称为空。");
                throw new InvalidOperationException("定时任务名称为空。");
            }

            Thread thread =
                MyThreadPool.FirstOrDefault(
                    it => it.Name != null && it.Name.Equals(taskName, StringComparison.CurrentCultureIgnoreCase));

            return thread;
        }

        /// <summary>
        /// log线程
        /// </summary>
        private void PrintMyThreads()
        {
            MyThreadPool.ToList()
                .ForEach(
                    it =>
                        Log.Debug(string.Format("线程: {0}, ID: {1}, 状态: {2}", it.Name, it.ManagedThreadId, it.ThreadState)));
        }
        #endregion

        /// <summary>
        /// 获取所有定时任务
        /// </summary>
        /// <returns>定时任务列表</returns>
        public IList<ScheduledTaskModel> GetScheduledTasks()
        {
            // 
            using (var dbContext = new MissionskyOAEntities())
            {
                //从数据库中获取tasks并按照createTime排序
                var entities = dbContext.ScheduledTasks.OrderBy(it => it.CreatedTime);

                //
                var tasks = new List<ScheduledTaskModel>();

                //首先ToList,然后循环遍历entities，并将记录ToModel后添加到Tasks中
                entities.ToList().ForEach(it => tasks.Add(it.ToModel()));

                return tasks;
            }
        }

        /// <summary>
        /// 获取所有定时任务Id
        /// </summary>
        /// <returns>定时任务Id列表</returns>
        public IList<int> GetScheduledTaskIds()
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entities = dbContext.ScheduledTasks.OrderBy(it => it.CreatedTime);
                var ids = new List<int>();

                entities.ToList().ForEach(it => ids.Add(it.Id));

                return ids;
            }
        }

        /// <summary>
        /// 根据定时任务Id获取定时任务详细
        /// </summary>
        /// <param name="taskId">定时任务Id</param>
        /// <returns>定时任务详细</returns>
        public ScheduledTaskModel GetScheduledTaskDetail(int taskId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.ScheduledTasks.FirstOrDefault(it => it.Id == taskId);

                if (entity == null)
                {
                    return null;
                }

                return entity.ToModel();
            }
        }

        /// <summary>
        /// 更新定时任务
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="taskId">定时任务Id</param>
        /// <param name="content">定时任务执行内容</param>
        /// <param name="executeDate">执行时间</param>
        /// <param name="isSavedChanges">是否存储数据库变化，默认不保存</param>
        /// <returns>true or false</returns>
        public void UpdateTask(MissionskyOAEntities dbContext, int taskId, DateTime executeDate, string content,
            bool isSavedChanges = false)
        {
            //更新定时任务执行时间
            var entity = dbContext.ScheduledTasks.FirstOrDefault(it => it.Id == taskId);
            if (entity == null)
            {
                throw new KeyNotFoundException("更新定时任务异常。");
            }

            //更新执行时间
            entity.LastExecTime = executeDate;

            //无详细内容
            if (!string.IsNullOrEmpty(content))
            {
                //更新定时任务
                var history = new ScheduledTaskHistoryModel()
                {
                    TaskId = taskId,
                    Result = true,
                    Desc = content,
                    CreatedTime = executeDate
                };

                //添加执行记录
                dbContext.ScheduledTaskHistories.Add(history.ToEntity());
            }

            if (isSavedChanges)
            {
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 查询定时任务
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <returns>实时定时任务</returns>
        public ListResult<ScheduledTaskModel> TaskList(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.ScheduledTasks.OrderBy(it => it.Id).ToList();

                //查询
                if (filter != null)
                {
                }

                //排序
                if (sort != null)
                {
                }

                //分页
                var count = list.Count();
                list = list.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();

                //转换
                var result = new ListResult<ScheduledTaskModel>();
                result.Data = new List<ScheduledTaskModel>();

                list.ToList().ForEach(item => result.Data.Add(item.ToModel()));

                result.Total = count;

                return result;
            }
        }

        /// <summary>
        /// 查询定时任务执行记录
        /// </summary>
        /// <param name="pageNo">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="sort">排序</param>
        /// <param name="filter">查找条件</param>
        /// <param name="taskId">定时任务Id</param>
        /// <returns>实时定时任务执行记录</returns>
        public ListResult<ScheduledTaskHistoryModel> TaskHistoryList(int pageNo, int pageSize, SortModel sort,
            FilterModel filter, int taskId)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                IList<ScheduledTaskHistory> list =
                    dbContext.ScheduledTaskHistories.Where(it => it.TaskId == taskId)
                        .OrderByDescending(it => it.CreatedTime)
                        .ToList();

                //查询
                if (filter != null)
                {
                }

                //排序
                if (sort != null)
                {
                }

                //分页
                var count = list.Count();
                list = list.Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();

                //转换
                var result = new ListResult<ScheduledTaskHistoryModel>();
                result.Data = new List<ScheduledTaskHistoryModel>();

                list.ToList().ForEach(item => result.Data.Add(item.ToModel()));

                result.Total = count;

                return result;
            }
        }

        /// <summary>
        /// 添加定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns>task id</returns>
        public int Add(ScheduledTaskModel task)
        {
            if (task == null)
            {
                Log.Error("添加定时任务异常。");
                throw new InvalidOperationException("添加定时任务异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                ValidTask(dbContext, task);

                var createdTime = DateTime.Now;

                //添加定时任务
                var entity = task.ToEntity();
                entity.CreatedTime = createdTime;
                dbContext.ScheduledTasks.Add(entity);
                dbContext.SaveChanges(); //更新数据库

                //启动线程
                if (task.Status == ScheduledTaskStatus.Started)
                {
                    StartThread(task);
                }

                return entity.Id;
            }
        }

        /// <summary>
        /// 更新定时任务
        /// </summary>
        /// <param name="task">定时任务</param>
        /// <returns>true or exception</returns>
        public bool Update(ScheduledTaskModel task)
        {
            if (task == null)
            {
                Log.Error("更新定时任务异常。");
                throw new InvalidOperationException("更新定时任务异常。");
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                //验证任务
                ValidTask(dbContext, task);

                var oldTask = dbContext.ScheduledTasks.FirstOrDefault(it => it.Id == task.Id);
                if (oldTask == null)
                {
                    Log.Error(string.Format("找不到定时任务，Id: {0}", task.Id));
                    throw new InvalidOperationException(string.Format("找不到定时任务，Id: {0}", task.Id));
                }
                
                //更新定时任务
                if (!oldTask.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldTask.Name = task.Name;
                }

                if (!oldTask.Desc.Equals(task.Desc, StringComparison.InvariantCultureIgnoreCase))
                {
                    oldTask.Desc = task.Desc;
                }

                var doThread = false; //是否重置线程

                if (!oldTask.TaskClass.Equals(task.TaskClass, StringComparison.InvariantCultureIgnoreCase))
                {
                    doThread = true;
                    oldTask.TaskClass = task.TaskClass;
                }

                if (oldTask.Unit != (int)task.Unit)
                {
                    doThread = true;
                    oldTask.Unit = (int) task.Unit;
                }

                if (oldTask.Interval != task.Interval)
                {
                    doThread = true;
                    oldTask.Interval = task.Interval;
                }

                if (oldTask.Status != (int)task.Status)
                {
                    doThread = true;
                    oldTask.Status = (int)task.Status;
                }
                
                dbContext.SaveChanges(); //更新数据库

                if (doThread) //启动定时任务
                {
                    if (task.Status == ScheduledTaskStatus.Started)
                    {
                        StartThread(task);
                    }
                    else
                    {
                        AbortThread(task);
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="id">定时任务id</param>
        /// <returns>是否删除成功</returns>
        public bool Delete(int id)
        {
            using (var dbcontext = new MissionskyOAEntities())
            {
                var entity = dbcontext.ScheduledTasks.FirstOrDefault(it => it.Id == id);
                if (entity == null)
                {
                    Log.Error("找不到定时任务。");
                    throw new KeyNotFoundException("找不到定时任务。");
                }

                var task = entity.ToModel();
                //移除
                dbcontext.ScheduledTasks.Remove(entity);
                dbcontext.SaveChanges();

                AbortThread(task); //终止线程

                return true;
            }
        }
        
        /// <summary>
        /// 验证定时任务
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="task"></param>
        private void ValidTask(MissionskyOAEntities dbContext, ScheduledTaskModel task)
        {
            var existedTask =
                dbContext.ScheduledTasks.FirstOrDefault(
                    it =>
                        !string.IsNullOrEmpty(it.Name) && task.Id != it.Id &&
                        (it.Name.Equals(task.Name, StringComparison.InvariantCultureIgnoreCase) ||
                         it.TaskClass.Equals(task.TaskClass, StringComparison.InvariantCultureIgnoreCase)));

            if (existedTask != null)
            {
                Log.Error(string.Format("定时任务已经存在，定时任务: {0}, 处理类: {1}", task.Name, task.TaskClass));
                throw new InvalidOperationException(string.Format("定时任务已经存在，定时任务: {0}, 处理类: {1}", task.Name,
                    task.TaskClass));
            }
        }
    }
}
