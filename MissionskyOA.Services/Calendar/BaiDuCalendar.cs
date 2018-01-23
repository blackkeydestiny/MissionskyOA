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
    public class BaiDuCalendar : CalendarBase, ICalendar
    {
        /// <summary>
        /// ApiKey = b31e81f21b72d4656ab3412fd56d2330
        /// </summary>
        private const string APP_KEY = "ApiKey";

        /// <summary>
        /// Url = http://apis.baidu.com/xiaogg/holiday/holiday
        /// </summary>
        private const string API_URL = "Url";

        private readonly NameValueCollection _apiSettings = null;

        public BaiDuCalendar(NameValueCollection apiSettings)
        {
            _apiSettings = apiSettings;
        }

        /// <summary>
        /// 验证日历类型：工作日，休息日，节假日
        /// </summary>
        /// <param name="day">日期，格式yyyyMMdd</param>
        /// <returns>日历类型</returns>
        public CalendarType CheckCalendar(string day)
        {
            if (_apiSettings == null || !_apiSettings.AllKeys.Contains(API_URL) || !_apiSettings.AllKeys.Contains(APP_KEY))
            {
                Log.Error("第三方请求参数异常。");
                throw new InvalidOperationException("第三方请求参数异常。");
            }

            string url = _apiSettings[API_URL];
            string appKey = _apiSettings[APP_KEY];

            try
            {
                //请求
                url = url.Contains("?") ? string.Format("{0}&d={1}", url, day) : string.Format("{0}?d={1}", url, day);
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                request.Headers.Add("apikey", appKey);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = false;
                request.Timeout = 5000;

                //响应
                CalendarType calendarType = CalendarType.Invalid;
                string result = string.Empty;
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                Stream stream = response.GetResponseStream();

                if (stream == null)
                {
                    throw new InvalidOperationException("第三方请求异常。");
                }

                //读取响应数据
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                result = reader.ReadToEnd();
                reader.Close();

                int tempType;
                Int32.TryParse(result, out tempType);
                calendarType = (CalendarType) calendarType;

                if (calendarType == CalendarType.Invalid)
                {
                    throw new InvalidOperationException("第三方验证无效。");
                }

                return calendarType;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }
    }
}
