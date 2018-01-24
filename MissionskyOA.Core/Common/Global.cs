using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MissionskyOA.Core.Common
{
    /// <summary>
    /// 全局处理
    /// </summary>
    public class Global
    {
        /// <summary>
        /// App类型
        /// </summary>
        public static bool IsProduction
        {
            get
            {
                if (ConfigurationManager.AppSettings["IsProduction"] != null &&
                    ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 国内领导的邮件
        /// </summary>
        public static string InlandHeaderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["InlandHeaderEmail"].ToLower();
            }
        }

        /// <summary>
        /// 国外领导的邮件
        /// </summary>
        public static string OverseaHeaderEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["OverseaHeaderEmail"].ToLower();
            }
        }

        /// <summary>
        /// 财务邮件
        /// </summary>
        public static string FinancialEmail
        {
            get
            {
                return ConfigurationManager.AppSettings["FinancialEmail"].ToLower();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string SmartQQHost
        {
            get
            {
                return ConfigurationManager.AppSettings["SmartQQHost"].ToLower();
            }
        }
        /// <summary>
        /// 会议纪要连接参数配置
        /// </summary>
        public static string MeetingSummaryURL
        {
            get { return ConfigurationManager.AppSettings[Constant.MEETING_SUMMARY_URL]; }
        }

        /// <summary>
        /// 报销申请单模板编号
        /// </summary>
        public static string ExpenseOrderReportNo
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(Constant.CONFIG_KEY_EXPENSE_ORDER_REPORT_NO))
                {
                    return ConfigurationManager.AppSettings[Constant.CONFIG_KEY_EXPENSE_ORDER_REPORT_NO];
                }

                throw new KeyNotFoundException("报销申请单报表配置不存在。");
            }
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <typeparam name="T">数据</typeparam>
        /// <param name="t"></param>
        /// <returns>JSON</returns>
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }
    }
}
