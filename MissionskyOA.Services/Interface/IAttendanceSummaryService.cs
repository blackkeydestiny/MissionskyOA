using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 用户考勤汇总
    /// </summary>
    public partial interface IAttendanceSummaryService
    {
        /// <summary>
        /// 获取用户本年度指定类型考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <param name="orderType">申请单类型</param>
        /// <returns>用户本年度指定类型考勤汇总信息</returns>
        AttendanceSummaryModel GetUserAttendanceSummary(MissionskyOAEntities dbContext, int userId,
            OrderType orderType);

        /// <summary>
        /// 根据用户Id获取本年度考勤汇总信息
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>本年度考勤汇总信息</returns>
        IList<AttendanceSummaryModel> GetAttendanceSummariesByUserId(int id);

        /// <summary>
        /// 根据用户Id获取本年度考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="userId">用户ID</param>
        /// <returns>本年度考勤汇总信息</returns>
        IList<AttendanceSummaryModel> GetAttendanceSummariesByUserId(MissionskyOAEntities dbContext, int userId);
        
        /// <summary>
        /// 更新用户考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="model">考勤汇总更新信息</param>
        /// <param name="beforeUpdated">更新前的旧值</param>
        /// <param name="afterUpdated">更新后的新值</param>
        /// <returns></returns>
        bool UpdateAttendanceSummary(MissionskyOAEntities dbContext, UpdateAttendanceSummaryModel model, ref double beforeUpdated, ref double afterUpdated);

        /// <summary>
        /// 更新假期信息
        /// </summary>
        /// <param name="model">假期信息</param>
        /// <returns></returns>
        bool ModifyAttendanceSummary(AttendanceSummaryModel model);

        /// <summary>
        /// 更新用户考勤汇总信息
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        void InitAttendanceSummary(MissionskyOAEntities dbContext, int userId);

        /// <summary>
        /// 验证余额是否充足
        /// </summary>
        /// <param name="users">申请单用户</param>
        /// <param name="orderType">假单类型</param>
        /// <param name="orderTime">申请时长</param>
        /// <returns></returns>
        AttendanceSummaryModel CheckVacationBalance(IList<OrderUserModel> users, OrderType orderType, double orderTime);

        /// <summary>
        /// 计算相应时长根据在公司工作的时间
        /// </summary>
        /// <param name="joinDate"></param>
        /// <returns></returns>
        double CaculateBaseValueForCompanyServiceYear(DateTime joinDate, DateTime currentTime);

        /// <summary>
        /// 根据员工总共的工作年限（工龄）计算相应的假期时长
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userServiceYearType"></param>
        /// <returns></returns>
        double CaculateBaseValueForTotalServiceYear(DateTime joinDate, int userServiceYearType, DateTime currentTime);
    }
}
