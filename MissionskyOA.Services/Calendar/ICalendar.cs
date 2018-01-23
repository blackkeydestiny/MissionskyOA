using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionskyOA.Core.Enum;

namespace MissionskyOA.Services
{
    public interface ICalendar
    {
        /// <summary>
        /// 验证日历类型：工作日，休息日，节假日
        /// </summary>
        /// <param name="day">日期，格式yyyyMMdd</param>
        /// <returns>日历类型</returns>
        CalendarType CheckCalendar(string day);
    }
}
