using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using log4net;

namespace MissionskyOA.Core.Email
{
    public class EmailClient
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(EmailClient));


        /*
         * 
         * 程序集 ： System.dll
         * 命名空间 ： System.Net.Mail
         * 主要的类： smtpClient 、MailAddress 、 MailMessage
         *           smtpClient :  三个构造方法：https://msdn.microsoft.com/zh-cn/library/system.net.mail.smtpclient(v=vs.110).aspx
         *                         smtpClient(), smtpClient(string host), smtpClient(string host, int port)
         *          
         *           MailAddress：
         *           
         *           MailMessage：https://msdn.microsoft.com/zh-cn/library/system.net.mail.mailmessage(VS.80).aspx
         *          
         * 问题：没有对send方法进行异常捕捉(try...catch)
         * 
         * 
         * **/

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="recipients">收件人</param>
        /// <param name="ccList">抄送人</param>
        /// <param name="title">邮箱主题</param>
        /// <param name="body">邮箱正文</param>
        /// <param name="files">附件文件路径</param>
        public static void Send(IList<string> recipients, IList<string> ccList, string title, string body, IList<string> files = null)
        {
            /*
             * 1、判断邮件接收人是否为空
             * 
             * **/
            if (recipients == null || recipients.Count <= 0)
            {
                Log.Error("无效的收件人。");
                throw new InvalidOperationException("无效的收件人。");
            }

            // 2、获取邮件配置
            EmailConfig config = new EmailConfig();

            // 3、发送邮件
            using (SmtpClient client = new SmtpClient(config.Server))
            {
                //邮件服务
                /*
                 * 1、UseDefaultCredentials属性：该值控制是否 DefaultCredentials 随请求一起发送。
                 * 2、Credentials属性：获取或设置用来对发件人进行身份验证的凭据。
                 * 3、DeliveryMethod属性： 指定如何发送的电子邮件将处理消息。
                 * 
                 * **/
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(config.Email, config.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                //邮箱内容
                MailMessage message = new MailMessage()
                {
                    Body = body,        // Body
                    Subject = title     // Subject
                };

                /*
                 * 1、message.To.Add()
                 * 2、message.From
                 * 3、message.CC
                 * 4、message.Attachments.Add
                 * 
                 * **/
                recipients.ToList().ForEach(it => message.To.Add(it)); //收件人(To)
                message.From = new MailAddress(config.Email); //发件人(From)

                //抄送人
                if (ccList != null && ccList.Count > 0)
                {
                    ccList.ToList().ForEach(it => message.CC.Add(it)); //抄送人(CC)
                }

                //添加附件
                if (files != null && files.Count > 0)
                {
                    files.ToList().ForEach(it =>
                    {
                        Attachment attachment = new Attachment(it);
                        message.Attachments.Add(attachment);// 附件(Attachments)
                    });
                }

                //发送邮件
                client.Send(message);

                //释放附件
                message.Attachments.ToList().ForEach(it => it.Dispose());

                client.Dispose();
            }
        }
    }
}
