using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using log4net;
using Microsoft.Reporting.WebForms;
using MissionskyOA.Core.Common;
using MissionskyOA.Core.Email;
using MissionskyOA.Models;
using MissionskyOA.Services;

namespace MissionskyOA.Services
{
    public class ReportHelper4Service
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReportHelper4Service));
        private readonly static ReportService ReportService = new ReportService();

        /// <summary>
        /// 向当前用户发送报表
        /// </summary>
        /// <param name="reportNo">报表编号</param>
        /// <param name="receiver">接收人</param>
        /// <param name="ccUser">邮件抄送人</param>
        /// <param name="reportFormat">报表格式</param>
        /// <param name="reprotParams">报表参数</param>
        /// <param name="isDetele">是否删除临时的报表文件</param>
        /// <returns>true or false</returns>
        public static bool SendReport(string reportNo, UserModel receiver, UserModel ccUser, string reportFormat, Dictionary<string, string> reprotParams, bool isDetele = true)
        {
            //获取报表详细
            ReportModel report = ReportService.GetReportDetail(reportNo) ?? new ReportModel();

            //报表配置
            NameValueCollection reportConfigs = ReportService.GetReportConfigs(report.Id);

            if (report.Id <= 0 || reportConfigs == null || reportConfigs.Count < 1)
            {
                Log.Error("找不到报表。");
                throw new KeyNotFoundException("找不到报表。");
            }

            //创建表报控件
            ReportViewer reportViewer = CreateReportViewer(reportConfigs, reprotParams);

            //报表默认输入格式
            string defaultFormat = reportConfigs[Constant.REPORT_CONFIG_DEFAULT_FORMAT];
            reportFormat = string.IsNullOrEmpty(reportFormat) ? defaultFormat : reportFormat;
            reportFormat = string.IsNullOrEmpty(reportFormat) ? Constant.DEFAULT_REPORT_FORMAT : reportFormat;
            Log.Info(string.Format("输出格式: {0}", reportFormat));

            //读取报表字节数据
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = reportViewer.ServerReport.Render(reportFormat, null, out mimeType, out encoding,
                out extension,
                out streamids, out warnings);

            //生成报表文件
            var reportPath = GenerateFile(report, bytes, reportFormat);

            //发送报表邮件
            SendEmail(report, receiver, ccUser, reportPath);

            //删除临时文件
            if (isDetele && File.Exists(reportPath))
            {
                try
                {
                    File.Delete(reportPath);
                }
                catch (IOException ex)
                {
                    Log.Error(string.Format("删除报表临时文件失败: {0}。", reportPath));
                    throw new KeyNotFoundException(string.Format("删除报表临时文件失败: {0}。", reportPath));
                }
            }

            return true;
        }

        /// <summary>
        /// 生成报表文件
        /// </summary>
        /// <param name="report"></param>
        /// <param name="bytes"></param>
        /// <param name="reportFormat"></param>
        /// <returns></returns>
        private static string GenerateFile(ReportModel report, byte[] bytes, string reportFormat)
        {
            string extName = string.Empty; //文件扩展名
            switch (reportFormat.ToLower())
            {
                case "excel":
                    extName = "xls";
                    break;
                case "word":
                    extName = "doc";
                    break;
                case "pdf":
                    extName = "pdf";
                    break;
            }

            string tempFolder = ConfigurationManager.AppSettings[Constant.WEB_CONFIG_TEMP_FOLDER];
            tempFolder = string.IsNullOrEmpty(tempFolder) ? "temp" : tempFolder;
            tempFolder = string.Format(@"{0}{1}", HttpContext.Current.Request.PhysicalApplicationPath, tempFolder); //报表文件存放路径

            if (!File.Exists(tempFolder)) //创建文件目录
            {
                Directory.CreateDirectory(tempFolder);
            }

            string filePath = string.Format(@"{0}\{1}_{2}.{3}", tempFolder, report.Name, Guid.NewGuid().ToString().Replace("-", ""), extName);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            return filePath;
        }

        /// <summary>
        /// 创建报表处理控件
        /// </summary>
        /// <param name="reportConfigs"></param>
        /// <param name="reprotParams"></param>
        /// <returns></returns>
        private static ReportViewer CreateReportViewer(NameValueCollection reportConfigs, Dictionary<string, string> reprotParams)
        {
            //获取报表配置信息
            string reportUrl = reportConfigs[Constant.REPORT_CONFIG_SERVICE_URL];
            string reportPath = reportConfigs[Constant.REPORT_CONFIG_REPORT_PATH];
            string userName = reportConfigs[Constant.REPORT_CONFIG_USER_NAME];
            string password = reportConfigs[Constant.REPORT_CONFIG_PASSWORD];
            string domain = reportConfigs[Constant.REPORT_CONFIG_DOMAIN];

            Log.Info("*****************报表详细********************");
            Log.Info(string.Format("报表服务: {0}", reportUrl));
            Log.Info(string.Format("报表路径: {0}", reportPath));
            Log.Info(string.Format("报表用户: {0}", userName));
            Log.Info(string.Format("用户密码: {0}", password));
            Log.Info(string.Format("用户域: {0}\n\r", domain));

            try
            {
                //创建报表控件
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.PromptAreaCollapsed = true;
                reportViewer.ProcessingMode = ProcessingMode.Remote; //远程报表
                reportViewer.ServerReport.ReportServerUrl = new Uri(reportUrl);
                reportViewer.ServerReport.ReportPath = (reportPath.StartsWith("/") ? reportPath : string.Format("/{0}", reportPath));
                reportViewer.ServerReport.ReportServerCredentials = new SSRSReportServerCredentials(userName, password,
                    domain); //报表用户信证

                //生成报表参数
                IList<ReportParameter> reportParams = new List<ReportParameter>();
                ReportParameterInfoCollection paramsInfo = default(ReportParameterInfoCollection);
                paramsInfo = reportViewer.ServerReport.GetParameters();
                paramsInfo.ToList().ForEach(it =>
                {
                    string paramValue = string.Empty;

                    #region 参数转换
                    switch (it.DataType)
                    {
                        case ParameterDataType.DateTime:
                            var dateTime = DateTime.Now;

                            if (reprotParams[it.Name] != null)
                            {
                                DateTime.TryParse(reprotParams[it.Name], out dateTime);
                            }

                            paramValue = dateTime.ToLongDateString();
                            break;
                        case ParameterDataType.Boolean:
                            var boolean = true;

                            if (reprotParams[it.Name] != null)
                            {
                                Boolean.TryParse(reprotParams[it.Name], out boolean);
                            }

                            paramValue = boolean ? "1" : "0";
                            break;
                        case ParameterDataType.Integer:
                            var int32 = 0;

                            if (reprotParams[it.Name] != null)
                            {
                                Int32.TryParse(reprotParams[it.Name], out int32);
                            }

                            paramValue = int32.ToString();
                            break;
                        case ParameterDataType.Float:
                            var vFloat = 0.0;

                            if (reprotParams[it.Name] != null)
                            {
                                Double.TryParse(reprotParams[it.Name], out vFloat);
                            }

                            paramValue = vFloat.ToString();
                            break;
                        case ParameterDataType.String:
                        default:
                            if (reprotParams[it.Name] != null)
                            {
                                paramValue = reprotParams[it.Name].ToString();
                            }
                            break;
                    }
                    #endregion

                    reportParams.Add(new ReportParameter(it.Name, paramValue, true));
                });

                reportViewer.ServerReport.SetParameters(reportParams);

                return reportViewer;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new InvalidOperationException("匹配报表参数异常。");
            }
        }

        private static void SendEmail(ReportModel report, UserModel receiver, UserModel ccUser, string reportPath)
        {
            if (receiver == null)
            {
                Log.Error("邮件接收人无效。");
                throw new KeyNotFoundException("邮件接收人无效。");
            }

            //发送邮件及附件
            var attachments = new List<string>();
            attachments.Add(reportPath);
            EmailClient.Send(new List<string> { receiver.Email }, (ccUser == null ? null : new List<string> { ccUser.Email }), report.Name, string.Format("生成报表{0}成功。请查看附件。", report.Name), attachments);
        }
    }
}
