using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Services
{
    public class UserCalendar : CalendarBase, ICalendar
    {
        /// <summary>
        /// 验证日历类型：工作日，休息日，节假日
        /// </summary>
        /// <param name="day">日期，格式yyyyMMdd</param>
        /// <returns>日历类型</returns>
        public CalendarType CheckCalendar(string day)
        {
            if (string.IsNullOrEmpty(day))
            {
                Log.Error("日期参数不正确。");
                throw new InvalidOperationException("日期参数不正确。");
            }

            //节假日
            if (Holidays.Any(it => it.Date == day))
            {
                return CalendarType.Holiday;
            }

            try
            {
                DateTime date = DateTime.ParseExact(day, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);

                return ((date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) &&
                        Workdays.Any(it => it.Date == day) == false)
                    ? CalendarType.Weekend
                    : CalendarType.Workday;

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }
    }
}
