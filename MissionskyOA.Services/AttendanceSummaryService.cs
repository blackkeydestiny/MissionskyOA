using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Data;
using MissionskyOA.Models;
using System.Threading;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 用户考勤汇总
    /// </summary>
    public class AttendanceSummaryService : ServiceBase, IAttendanceSummaryService
    {
        private readonly IUserService _userService = new UserService();

        /// <summary>
        /// 获取用户本年度指定类型考勤汇总信息
        /// 
        /// 获取用户指定类型的假期信息(不同的类型，假期的时间不同), 本年度
        /// 
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <param name="orderType">申请单类型</param>
        /// <returns>用户本年度指定类型考勤汇总信息</returns>
        public AttendanceSummaryModel GetUserAttendanceSummary(MissionskyOAEntities dbContext, int userId,
            OrderType orderType)
        {
            // 1、调休加班一起
            var checkType = orderType;
            if (checkType == OrderType.DaysOff) //调休，扣除加班
            {
                checkType = OrderType.Overtime;
            }

            #region 验证用户性别
            //var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == userId);

            //if (userEntity == null)
            //{
            //    throw new KeyNotFoundException("找不到相关用户。");
            //}

            //var user = userEntity.ToModel();

            ////女，没有陪产假
            //if (user.Gender == Gender.Female && orderType == OrderType.PaternityLeave)
            //{
            //    return null;
            //}

            ////男，没有哺乳假和产假
            //if (user.Gender == Gender.Male && (orderType == OrderType.MaternityLeave || orderType == OrderType.BreastfeedingLeave))
            //{
            //    return null;
            //}
            #endregion

            // 2、获取指定类型假期信息
            var summary =
                dbContext.AttendanceSummaries.FirstOrDefault(
                    it =>
                        it.Year == DateTime.Now.Year && it.UserId == userId &&
                        it.Type == (int)checkType);


            // 3、判断获取的假期信息是否为空
            if (summary == null)
            {
                // 重新初始化用户假期信息
                InitAttendanceSummary(dbContext, userId);

                summary =
                dbContext.AttendanceSummaries.FirstOrDefault(
                    it =>
                        it.Year == DateTime.Now.Year && it.UserId == userId &&
                        it.Type == (int)checkType);

                if (summary == null)
                {
                    throw new NullReferenceException("获取假期汇总信息异常。");
                }
            }

            return summary.ToModel();
        }

        /// <summary>
        /// 根据用户Id获取本年度考勤汇总信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>本年度考勤汇总信息</returns>
        public IList<AttendanceSummaryModel> GetAttendanceSummariesByUserId(int userId)
        {
            IList<AttendanceSummaryModel> attendanceSummaries = new List<AttendanceSummaryModel>();

            using (var dbContext = new MissionskyOAEntities())
            {
                attendanceSummaries = GetAttendanceSummariesByUserId(dbContext, userId);
            }

            return attendanceSummaries;
        }

        /// <summary>
        /// 根据用户Id获取本年度考勤汇总信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dbContext">数据库上下文</param>
        /// <returns>本年度考勤汇总信息</returns>
        public IList<AttendanceSummaryModel> GetAttendanceSummariesByUserId(MissionskyOAEntities dbContext, int userId)
        {
            if (dbContext == null)
            {
                throw new NullReferenceException("Failed to get MissionskyOAEntities object");
            }

            IList<AttendanceSummaryModel> attendanceSummaries = new List<AttendanceSummaryModel>();

            InitAttendanceSummary(dbContext, userId);  //初使化用户考勤汇总信息

            var entities =
                dbContext.AttendanceSummaries.Where(it => (it.UserId == userId && it.Year == DateTime.Now.Year))
                    .ToList();

            var userEntity = dbContext.Users.FirstOrDefault(it => it.Id == userId);

            if (userEntity == null)
            {
                throw new KeyNotFoundException("找不到相关用户。");
            }

            var user = userEntity.ToModel();

            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int) OrderType.AnnualLeave), OrderType.AnnualLeave, userId)); //年假
            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.Overtime), OrderType.Overtime, userId)); //调休假/加班
            attendanceSummaries.Add(ToModel(null, OrderType.DaysOff, userId)); //调休假
            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.AskLeave), OrderType.AskLeave, userId)); //事假
            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.SickLeave), OrderType.SickLeave, userId)); //病假
            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.MarriageLeave), OrderType.MarriageLeave, userId)); //婚假

            if (user.Gender == Gender.Female || user.Gender == Gender.Privacy) //女
            {
                attendanceSummaries.Add(ToModel(
                    entities.FirstOrDefault(it => it.Type == (int) OrderType.MaternityLeave), OrderType.MaternityLeave,
                    userId)); //产假

                attendanceSummaries.Add(
                    ToModel(entities.FirstOrDefault(it => it.Type == (int) OrderType.BreastfeedingLeave),
                        OrderType.BreastfeedingLeave, userId)); //哺乳假
            }

            if (user.Gender == Gender.Male || user.Gender == Gender.Privacy) //男
            {
                attendanceSummaries.Add(ToModel(
                    entities.FirstOrDefault(it => it.Type == (int) OrderType.PaternityLeave), OrderType.PaternityLeave,
                    userId)); //陪产假
            }

            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.FuneralLeave), OrderType.FuneralLeave, userId)); //丧假
            attendanceSummaries.Add(ToModel(entities.FirstOrDefault(it => it.Type == (int)OrderType.Other), OrderType.Other, userId)); //其他(出差/外访等)

            return attendanceSummaries;
        }
        
        public bool ModifyAttendanceSummary(AttendanceSummaryModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                //获取年假信息
                var holidays = dbContext.AttendanceSummaries.FirstOrDefault(it => it.Id == model.Id);

                if (model.LastValue != null)
                {
                    holidays.LastValue = model.LastValue;
                }

                if(model.BaseValue!=null)
                {
                    holidays.BaseValue = model.BaseValue;
                }
                if(model.RemainValue!=null)
                {
                    holidays.RemainValue = model.RemainValue;                
                }
                

                //更新到数据库
                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 更新用户考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="model">考勤汇总更新信息</param>
        /// <param name="beforeUpdated">更新前的旧值</param>
        /// <param name="afterUpdated">更新后的新值</param>
        /// <returns></returns>
        public bool UpdateAttendanceSummary(MissionskyOAEntities dbContext, UpdateAttendanceSummaryModel model, ref double beforeUpdated, ref double afterUpdated)
        {
            if (model == null || model.UserId < 1 || Math.Abs(model.Times) < 0.0)
            {
                throw new KeyNotFoundException("更新假期异常。");
            }

            //CheckVacationBalance(model.ApplicantId, model.Type, model.Times); //验证余额是否充足

            #region 更新假期汇总信息
            var checkType = model.Type;
            if (checkType == OrderType.DaysOff) //调休，扣除加班
            {
                checkType = OrderType.Overtime;
            }

            //用户本年度相关类型 汇总结果
            var summary =
                dbContext.AttendanceSummaries.FirstOrDefault(
                    it =>
                        it.Year == DateTime.Now.Year && it.UserId == model.UserId &&
                        it.Type == (int)checkType);

            if (summary == null)
            {
                throw new NullReferenceException("更新假期异常。");
            }

            summary.RemainValue = summary.RemainValue ?? 0.0;
            beforeUpdated = summary.RemainValue.Value; //更新前的值
            summary.RemainValue += model.Times; //计算已请多少事假
            afterUpdated = beforeUpdated + model.Times; //更新后的值

            beforeUpdated = Math.Abs(beforeUpdated); //更新前的值
            afterUpdated = Math.Abs(afterUpdated); //更新后的值
            #endregion

            return true;
        }
        
        /// <summary>
        /// 初使化用户考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public void InitAttendanceSummary(MissionskyOAEntities dbContext, int userId)
        {
            if (dbContext == null)
            {
                throw new NullReferenceException("Invalid db context when init user attendance summary.");
            }
            
            var user = dbContext.Users.FirstOrDefault(it => it.Id == userId);
            if (user == null || !user.Available)
            {
                Log.Error(string.Format("用户不存在或已离职。Id: {0}", userId));
                return;
            }

            //
            var baseVacations = _userService.ImportUserBaseVacation(userId);

            if (baseVacations == null)
            {
                baseVacations = new NameValueCollection();
            }

            Type type = typeof (OrderType);
            ArrayList vacations = EnumExtensions.GetEmnuValueList(type);
        
            foreach (string item in vacations)
            {
                var orderType = (OrderType) Enum.Parse(type, item);

                if (orderType == OrderType.None || orderType == OrderType.DaysOff) //不汇总此类假期
                {
                    continue;
                }

                //本年度汇总信息
                var thisSummary = dbContext.AttendanceSummaries.FirstOrDefault(it => it.UserId == userId && it.Year == DateTime.Now.Year && it.Type == (int)orderType);

                //获取用户入职时间和工龄类型
                DateTime joinTime = (DateTime)user.JoinDate;
                int serverYearType = user.ServerYearType.HasValue ? user.ServerYearType.Value : (int)UserServiceYearType.TenYears;

                if (thisSummary == null)
                {
                    thisSummary = new AttendanceSummary() //初使化数据本年度
                    {
                        UserId = userId,
                        Year = DateTime.Now.Year,
                        Type = (int)orderType,
                        BaseValue = 0.0,
                        LastValue = 0.0,
                        RemainValue = 0.0
                    };

                    //上一年的汇总信息
                    var lastSummary = dbContext.AttendanceSummaries.FirstOrDefault(
                        it => it.UserId == userId && it.Year == DateTime.Now.Year - 1 && it.Type == (int)orderType);

                    switch (orderType)
                    {
                        case OrderType.AnnualLeave: //年假
                            thisSummary.LastValue = lastSummary == null ? 0.0 : lastSummary.RemainValue;
                            //thisSummary.BaseValue = baseVacations.AllKeys.Contains(OrderType.AnnualLeave.ToString())
                            //    ? Double.Parse(baseVacations[OrderType.AnnualLeave.ToString()])
                            //    : 75.0;
                            thisSummary.BaseValue = CaculateBaseValueForTotalServiceYear(joinTime, serverYearType, DateTime.Now) + CaculateBaseValueForCompanyServiceYear(joinTime, DateTime.Now);
                            thisSummary.RemainValue = thisSummary.LastValue + thisSummary.BaseValue; //基准 + 上一年剩余
                            break;
                        case OrderType.Overtime: //加班
                            thisSummary.LastValue = lastSummary == null ? 0.0 : lastSummary.RemainValue;
                            thisSummary.RemainValue = thisSummary.LastValue; //无基准
                            break;

                        case OrderType.SickLeave: //病假
                            thisSummary.BaseValue = baseVacations.AllKeys.Contains(OrderType.SickLeave.ToString())
                                ? Double.Parse(baseVacations[OrderType.SickLeave.ToString()])
                                : 0.0;
                            thisSummary.RemainValue = thisSummary.BaseValue; //无剩余
                            break;
                        case OrderType.FuneralLeave: //丧假
                            thisSummary.BaseValue = baseVacations.AllKeys.Contains(OrderType.FuneralLeave.ToString())
                                ? Double.Parse(baseVacations[OrderType.FuneralLeave.ToString()])
                                : 0.0;
                            thisSummary.RemainValue = thisSummary.BaseValue; //无剩余
                            break;
                        case OrderType.MaternityLeave: //产假
                            if (lastSummary == null) //未汇总过产假
                            {
                                thisSummary.BaseValue = baseVacations.AllKeys.Contains(OrderType.MaternityLeave.ToString())
                                    ? Double.Parse(baseVacations[OrderType.MaternityLeave.ToString()])
                                    : 0.0;
                            }
                            else if (lastSummary.RemainValue == lastSummary.BaseValue) //未使用过产假
                            {
                                thisSummary.BaseValue = lastSummary.RemainValue;
                            }

                            thisSummary.RemainValue = thisSummary.BaseValue;
                            break;

                        case OrderType.MarriageLeave: //婚假
                            if (lastSummary == null) //未汇总过婚假
                            {
                                thisSummary.BaseValue = baseVacations.AllKeys.Contains(OrderType.MarriageLeave.ToString())
                                    ? Double.Parse(baseVacations[OrderType.MarriageLeave.ToString()])
                                    : 0.0;
                            }
                            else if (lastSummary.RemainValue == lastSummary.BaseValue) //未使用过婚假
                            {
                                thisSummary.BaseValue = lastSummary.RemainValue;
                            }

                            thisSummary.RemainValue = thisSummary.BaseValue;
                            break;
                        case OrderType.AskLeave: //事假
                            break;
                    }

                    dbContext.AttendanceSummaries.Add(thisSummary);
                }
                else 
                {
                    switch (orderType)
                    {                    
                        case OrderType.AnnualLeave: //年假
                            double baseValueIncrementThisMouth = 0.0;
                            // 在每月的1号重新计算这个月的baseValue
                            double baseValueTotalThisMouth = CaculateBaseValueForTotalServiceYear(joinTime, serverYearType, DateTime.Now) + CaculateBaseValueForCompanyServiceYear(joinTime, DateTime.Now);
                            if ("01".Equals(DateTime.Now.Day.ToString("00")) && baseValueTotalThisMouth != thisSummary.BaseValue)
                            {
                                if (thisSummary.BaseValue == 0 || thisSummary.BaseValue == 0.0)
                                {
                                    // 获取上个月一号的时间
                                    DateTime lastMouthFirstDay = AttendanceSummaryExtentions.GetLastMouthFirstDay(DateTime.Now.Year, DateTime.Now.Month);
                                    // 当baseValue为0时，计算上月的baseValue
                                    double baseValueTotalLastMouth = CaculateBaseValueForTotalServiceYear(joinTime, serverYearType, lastMouthFirstDay) + CaculateBaseValueForCompanyServiceYear(joinTime, lastMouthFirstDay);
                                    baseValueIncrementThisMouth = Math.Round((double)(baseValueTotalThisMouth - baseValueTotalLastMouth), 1);
                                    thisSummary.BaseValue = baseValueTotalThisMouth;
                                    //thisSummary.RemainValue += baseValueIncrementThisMouth;
                                }
                                else
                                {
                                    // 计算上月新增的baseValue
                                    baseValueIncrementThisMouth = Math.Round((double)(baseValueTotalThisMouth - thisSummary.BaseValue), 1);

                                    // 更新baseValue 和 RemainValue
                                    thisSummary.BaseValue = baseValueTotalThisMouth;
                                    //thisSummary.RemainValue += baseValueIncrementThisMouth;
                                }
                                thisSummary.RemainValue += baseValueIncrementThisMouth;
                            }
                            break;
                        case OrderType.Overtime: //加班
                            break;
                        case OrderType.SickLeave: //病假
                            break;
                        case OrderType.FuneralLeave: //丧假
                            break;
                        case OrderType.MaternityLeave: //产假
                            break;
                        case OrderType.MarriageLeave: //婚假
                            break;
                        case OrderType.AskLeave: //事假
                            break;
                    }
                    //dbContext.AttendanceSummaries.Add(thisSummary);
                }
            }

            dbContext.SaveChanges(); //保存到数据库
        }

        /// <summary>
        /// 验证余额是否充足
        /// </summary>
        /// <param name="users">申请单用户</param>
        /// <param name="orderType">假单类型</param>
        /// <param name="orderTime">申请时长</param>
        /// <returns></returns>
        public AttendanceSummaryModel CheckVacationBalance(IList<OrderUserModel> users, OrderType orderType, double orderTime)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                AttendanceSummaryModel summary = null;

                users.ToList().ForEach(user =>
                {
                    // 1、正常假期申请：扣假
                    if (orderTime < 0)
                    {
                        // 2、
                        summary = GetUserAttendanceSummary(dbContext, user.Id, orderType);

                        // 3、年假 || 加班，　验证剩余假期
                        if (orderType == OrderType.AnnualLeave || orderType == OrderType.DaysOff)
                        {
                            summary = summary ?? new AttendanceSummaryModel();
                            

                            double uncompleted = 0.0; //未完成的假期申请
                            double remainVacation = 0.0; //剩余假期

                            /*
                             * 
                             * 处理未完成的假期时间(直接处理数据库逻辑来获取)
                             * 
                             * 未完成的假期申请： 表明还有在假期还在申请中....
                             * 满足未完成假期申请的条件：
                             *    1、当前用户Id
                             *    2、申请的假期类型
                             *    3、为正常申请
                             *    4、申请状态不能是已取消和已拒绝
                             *    5、申请流程尚未完成
                             *    6、申请时间必须为本年度的
                             * 
                             * **/
                            var sql =
                                @"SELECT ISNULL(SUM(D.IOHours), 0) AS UncompletedValue FROM [Order] O INNER JOIN OrderDet D ON O.Id = D.OrderId 
                            WHERE O.UserId = {0}  --用户
                            AND ISNULL(O.OrderType, 0) = {1} --假期类型
							AND ISNULL(O.RefOrderId, 0) = 0 --正常申请
							AND ISNULL(O.Status, 0) NOT IN (4, 3) --状态不是已取消或已拒绝
                            AND (ISNULL(O.NextStep, 0) != 0 AND NOT EXISTS(SELECT 1 FROM WorkflowStep WHERE [Type] = 3 AND ID = O.NextStep)) --流程未完成且当前步骤不是财务审批(未扣除的假期申请)
                            AND YEAR(D.StartDate) = YEAR(GETDATE()) AND YEAR(D.EndDate) = YEAR(GETDATE()) --本年度的休假";

                            // 获取未完成假期时间
                            var data = dbContext.Database.SqlQuery<double>(string.Format(sql, user.Id, (int)orderType));

                            if (data != null)
                            {
                                uncompleted = Math.Abs(data.FirstOrDefault());
                            }

                            // 申请时间为： 当前有的假期时间 - 尚未完成的时间
                            remainVacation = summary.RemainValue.HasValue ? summary.RemainValue.Value - uncompleted : 0.0;

                            //无剩余假期 || 请假时间多于剩余假期
                            /*
                             * 1、剩余假期时间 < 0
                             * 2、申请时间 > 剩余时间
                             * 
                             * 
                             * **/
                            if (remainVacation <= 0 || Math.Abs(orderTime) > remainVacation)
                            {
                                throw new InvalidOperationException(string.Format("{0}剩余假期({1}小时)不足。{2}", user.UserName,
                                    summary.RemainValue, (uncompleted > 0 ? "尚有未完成的申请。" : string.Empty)));
                            }
                        }
                    }
                });

                return summary;
            }
        }
        
        /// <summary>
        /// 计算用户已休假期
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orderType">假期类型</param>
        /// <returns></returns>
        public double CountUsedVacation(int userId, OrderType orderType)
        {
            double vacation = 0.0;

            if (orderType == OrderType.Overtime) //统计调休假
            {
                orderType = OrderType.DaysOff;
            }

            using (var dbContext = new MissionskyOAEntities())
            {
                var sql = @"SELECT ISNULL(SUM(D.IOHours), 0) AS UsedValue FROM [Order] O INNER JOIN OrderDet D ON O.Id = D.OrderId 
                            WHERE O.UserId = {0}  --用户
                            AND ISNULL(O.OrderType, 0) = {1} --假期类型
                            AND (ISNULL(O.[Status], 0) = 2 OR ISNULL(O.[Status], 0) = 5) --批准 || 撤销
                            AND (NextStep IS NULL OR EXISTS(SELECT 1 FROM WorkflowStep WHERE [Type] = 3 AND ID = O.NextStep)) --流程结束或到当前步骤是财务审批
                            AND YEAR(D.StartDate) = YEAR(GETDATE()) AND YEAR(D.EndDate) = YEAR(GETDATE()) --本年度的休假";

                var data = dbContext.Database.SqlQuery<double>(string.Format(sql, userId, (int)orderType));

                if (data != null)
                {
                    vacation = data.FirstOrDefault();
                }
            }

            return vacation;
        }
        
        /// <summary>
        /// init AttendanceSummaryModel
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="type"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        private AttendanceSummaryModel ToModel(AttendanceSummary entity, OrderType type, int userid)
        {
            if (entity == null)
            {
                entity = new AttendanceSummary()
                {
                    Type = (int)type,
                    BaseValue = 0,
                    LastValue = 0,
                    RemainValue = 0,
                    Year = DateTime.Now.Year,
                    UserId = userid
                };
            }

            var model = entity.ToModel();
            model.UsedValue = CountUsedVacation(userid, type);

            return model;
        }

        public bool UpdateAttendanceSummaryForAll(DateTime updateTime)
        {
            return true;
        }

        public bool UpdateAttendanceSummaryForSingle(DateTime updateTime)
        {
            return true;
        }

        /// <summary>
        /// 计算相应时长根据在公司工作的时间
        /// </summary>
        /// <param name="joinDate"></param>
        /// <returns></returns>
        public double CaculateBaseValueForCompanyServiceYear(DateTime joinDate, DateTime currentTime)
        {
            if (joinDate > DateTime.Now)
            {
                throw new Exception("无效的入职日期");
            }

            double baseValueForCompanyServiceYear = 0.0;
            double curenntTotalDays = 0.0;

            //DateTime currentTime = DateTime.Now;        // 获取当前时间
            int days = (currentTime - joinDate).Days;   // 员工的入职天数    
            DateTime currentNewYearDateTime = Convert.ToDateTime(currentTime.Year.ToString() + "-01-01");        //本年度元旦日期 日期格式

            // 计算当前年份的总天数，如果是闰年，则加一天
            curenntTotalDays = currentTime.Year.IsLeap() ? 366.0 : 365.0;

            // 获取员工从入职到当前时间的工作年限
            int yearsForCompany = (int)Math.Floor((currentTime - joinDate).Days / 365.0);

            // 获取员工入职时的日期（月份和日份）在当前年份的时间
            DateTime cureentJoinTime = !currentTime.Year.IsLeap() && string.Format("{0}{1}", joinDate.Month, joinDate.Day) == "229" ?
                new DateTime(currentTime.Year, joinDate.Month, joinDate.Day - 1) :
                new DateTime(currentTime.Year, joinDate.Month, joinDate.Day);

            //入职不满一年，没有工龄年假
            if (yearsForCompany <= 0)
            {
                baseValueForCompanyServiceYear = 0;
            }
            // 比较 员工入职时间在当前年份的时间 与 当前时间
            else if (DateTime.Compare(cureentJoinTime, currentTime) <= 0)
            {
                //if (YearsForCompany == 0)
                //{
                //    BaseValueForCompanyServiceYear = 0;
                //}
                // 员工入职时间在当前年份的时间 在 当前时间 之前
                baseValueForCompanyServiceYear = (double)(cureentJoinTime - currentNewYearDateTime).Days / curenntTotalDays * (yearsForCompany - 1) * 8;
                baseValueForCompanyServiceYear += (double)(currentTime - cureentJoinTime).Days / curenntTotalDays * yearsForCompany * 8;
            }
            else
            {
                // 员工入职时间在当前年份的时间 在 当前时间 之后
                baseValueForCompanyServiceYear = (double)(currentTime - currentNewYearDateTime).Days / curenntTotalDays * (yearsForCompany - 1) * 8;
            }

            return Math.Round(baseValueForCompanyServiceYear, 1);
        }

        /// <summary>
        /// 根据员工总共的工作年限（工龄）计算相应的假期时长
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userServiceYearType"></param>
        /// <returns></returns>
        public double CaculateBaseValueForTotalServiceYear(DateTime joinDate, int userServiceYearType, DateTime currentTime)
        {
            if (joinDate > DateTime.Now)
            {
                throw new Exception("无效的入职日期");
            }

            double baseValueForTotalServiceYea = 0.0;
            double curenntTotalDays = DateTime.Now.Year.IsLeap() ? 366.0 : 365.0; // 计算当前年份的总天数，如果是闰年，则加一天

            switch (userServiceYearType)
            {
                case (int)UserServiceYearType.ThirtyYears:
                    baseValueForTotalServiceYea = CaculateBaseValue(120, joinDate, curenntTotalDays, currentTime);
                    if (baseValueForTotalServiceYea >= 160)
                    {
                        baseValueForTotalServiceYea = 160; //如果在公司累计工作超过20天，
                    }
                    break;
                case (int)UserServiceYearType.TwentyYears:
                    baseValueForTotalServiceYea = CaculateBaseValue(80, joinDate, curenntTotalDays, currentTime);
                    if (baseValueForTotalServiceYea >= 120)
                    {
                        baseValueForTotalServiceYea = 120; //如果在公司累计工作超过15天，
                    }
                    break;
                case (int)UserServiceYearType.TenYears:
                    baseValueForTotalServiceYea = CaculateBaseValue(40, joinDate, curenntTotalDays, currentTime);
                    if (baseValueForTotalServiceYea >= 80)
                    {
                        baseValueForTotalServiceYea = 80; //如果在公司累计工作超过10天，
                    }
                    break;
            }

            return Math.Round(baseValueForTotalServiceYea, 1);
        }

        /// <summary>
        /// 根据员工的入职时间计算时长
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="joinDate"></param>
        /// <returns></returns>
        private double CaculateBaseValue(int hours, DateTime joinDate, double curenntTotalDays, DateTime currentTime)
        {
            double totalServiceYearTime = 0.0;
            //DateTime currentTime = DateTime.Now;    //获取当前时间
            DateTime currentNewYearDateTime = Convert.ToDateTime(currentTime.Year.ToString() + "-01-01");        //本年度元旦日期 日期格式
            int currentYearDays = (currentTime - currentNewYearDateTime).Days;                  //本年度当前时间距离本年度元旦的天数
            int days = (currentTime - joinDate).Days;                                           //员工的入职天数

            // 在公司工作超过1年的
            if (days > 365)
            {
                totalServiceYearTime = (double)currentYearDays / curenntTotalDays * hours;
            }
            else
            {
                if (DateTime.Compare(joinDate, currentNewYearDateTime) <= 0)
                {
                    // 在公司工作未超过1年的，且入职时间在本年度元旦之前的
                    totalServiceYearTime = (double)currentYearDays / curenntTotalDays * hours;
                }
                else
                {
                    // 在公司工作未超过1年的，且入职时间在本年度元旦之后的
                    totalServiceYearTime = (double)days / curenntTotalDays * hours; 
                }          
            }

            return totalServiceYearTime;
        }     
    }
}
