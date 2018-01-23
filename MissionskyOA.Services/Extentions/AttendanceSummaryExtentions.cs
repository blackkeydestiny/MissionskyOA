using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using MissionskyOA.Core.Enum;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 考勤统计Model转换
    /// </summary>
    public static class AttendanceSummaryExtentions
    {
        public static AttendanceSummary ToEntity(this AttendanceSummaryModel model)
        {
            var entity = new AttendanceSummary()
            {
                Id = model.Id,
                UserId = model.UserId,
                Year = model.Year,
                Type = (int)model.Type,
                LastValue = model.LastValue,
                BaseValue = model.BaseValue,
                RemainValue = model.RemainValue
            };

            return entity;
        }

        public static AttendanceSummaryModel ToModel(this AttendanceSummary entity)
        {
            // 将entity 转为 Model
            var model = new AttendanceSummaryModel()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Year = entity.Year,
                Type = (OrderType)entity.Type,
                LastValue = entity.LastValue,
                BaseValue = entity.BaseValue,
                RemainValue = entity.RemainValue
            };

            //考勤(申请单)类型名称
            model.TypeName = EnumExtensions.GetDescriptionList(typeof(OrderType))[model.Type.ToString()];

            return model;
        }

        /// <summary>
        /// 获取上个月一号的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLastMouthFirstDay(int year, int mouth) 
        {       
            int beforeYear = 0;
            int beforeMouth = 0;

            if (mouth <= 1)//如果当前月是一月，那么年份就要减1 
            {
                beforeYear = year - 1;
                beforeMouth = 12;//上个月 
            }
            else
            {
                beforeYear = year;
                beforeMouth = mouth - 1;//上个月 
            }
            string beforeMouthOneDay = beforeYear.ToString("0000") +"-" + beforeMouth.ToString("00") + "-01";   //上个月第一天 
            //string beforeMouthLastDay = beforeYear + beforeMouth + DateTime.DaysInMonth(year, beforeMouth) + "";//上个月最后一天

            return Convert.ToDateTime(beforeMouthOneDay);
        }
    }
}
