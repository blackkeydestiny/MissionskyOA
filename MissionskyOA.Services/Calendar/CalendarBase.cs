using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MissionskyOA.Core;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Enum;
using MissionskyOA.Services.Task;

namespace MissionskyOA.Services
{
    public struct CalendarDay
    {
        /// <summary>
        /// yyymmdd
        /// </summary>
        public string Date;
        public string Desc;
    }

    public class CalendarBase
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(CalendarBase));

        /// <summary>
        /// 节假日
        /// </summary>
        protected static IList<CalendarDay> Holidays;

        /// <summary>
        /// 工作日
        /// </summary>
        protected static IList<CalendarDay> Workdays;
        
        private const string BAIDU = "BaiDu";

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static CalendarBase()
        {
            #region 节假日
            Holidays = new List<CalendarDay>()
            {
                new CalendarDay() {Date = "20160101", Desc = "元旦节"},
                new CalendarDay() {Date = "20160102", Desc = "元旦节"},
                new CalendarDay() {Date = "20160103", Desc = "元旦节"},
                new CalendarDay() {Date = "20160207", Desc = "除夕"},
                new CalendarDay() {Date = "20160208", Desc = "春节"},
                new CalendarDay() {Date = "20160208", Desc = "春节"},
                new CalendarDay() {Date = "20160209", Desc = "春节"},
                new CalendarDay() {Date = "20160210", Desc = "春节"},
                new CalendarDay() {Date = "20160211", Desc = "春节"},
                new CalendarDay() {Date = "20160212", Desc = "春节"},
                new CalendarDay() {Date = "20160213", Desc = "春节"},
                new CalendarDay() {Date = "20160402", Desc = "清明节"},
                new CalendarDay() {Date = "20160403", Desc = "清明节"},
                new CalendarDay() {Date = "20160404", Desc = "清明节"},
                new CalendarDay() {Date = "20160430", Desc = "劳动节"},
                new CalendarDay() {Date = "20160501", Desc = "劳动节"},
                new CalendarDay() {Date = "20160502", Desc = "劳动节"},
                new CalendarDay() {Date = "20160609", Desc = "端午节"},
                new CalendarDay() {Date = "20160610", Desc = "端午节"},
                new CalendarDay() {Date = "20160611", Desc = "端午节"},
                new CalendarDay() {Date = "20160915", Desc = "中秋节"},
                new CalendarDay() {Date = "20160916", Desc = "中秋节"},
                new CalendarDay() {Date = "20160917", Desc = "中秋节"},
                new CalendarDay() {Date = "20161001", Desc = "国庆节"},
                new CalendarDay() {Date = "20161002", Desc = "国庆节"},
                new CalendarDay() {Date = "20161003", Desc = "国庆节"},
                new CalendarDay() {Date = "20161004", Desc = "国庆节"},
                new CalendarDay() {Date = "20161005", Desc = "国庆节"},
                new CalendarDay() {Date = "20161006", Desc = "国庆节"},
                new CalendarDay() {Date = "20161007", Desc = "国庆节"}
            };
            #endregion

            #region 周末补班
            Workdays = new List<CalendarDay>()
            {
                new CalendarDay(){Date = "20160206", Desc = "春节补班"},
                new CalendarDay(){Date = "20160214", Desc = "春节补班"},
                new CalendarDay(){Date = "20160612", Desc = "端午节补班"},
                new CalendarDay(){Date = "20160918", Desc = "中秋节补班"},
                new CalendarDay(){Date = "20161008", Desc = "国庆节补班"},
                new CalendarDay(){Date = "20161009", Desc = "国庆节补班"}
            };
            #endregion
        }
        
        /// <summary>
        /// 初使化第三方请求处理类
        /// </summary>
        /// <param name="apiSettings">第三方配置</param>
        /// <returns></returns>
        public static ICalendar GetInstance(NameValueCollection apiSettings)
        {
            if (apiSettings== null || !apiSettings.AllKeys.Contains(Constant.CALENDAR_API_PROVIDER))
            {
                Log.Error("第三方请求参数异常。");
                throw new InvalidOperationException("第三方请求参数异常。");
            }

            string provider = apiSettings[Constant.CALENDAR_API_PROVIDER];

            if (provider.Equals(BAIDU, StringComparison.OrdinalIgnoreCase)) //百度API
            {
                return new BaiDuCalendar(apiSettings);
            }
            else
            {
                return new UserCalendar();
            }
        }
    }
}
