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
            if (recipients == null || recipients.Count <= 0)
            {
                Log.Error("无效的收件人。");
                throw new InvalidOperationException("无效的收件人。");
            }

            //获取邮件配置
            EmailConfig config = new EmailConfig();

            //发送邮件
            using (SmtpClient client = new SmtpClient(config.Server))
            {
                //邮件服务
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(config.Email, config.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                //邮箱内容
                MailMessage message = new MailMessage()
                {
                    Body = body,
                    Subject = title
                };

                recipients.ToList().ForEach(it => message.To.Add(it)); //添加收件人
                message.From = new MailAddress(config.Email); //发件人

                //抄送人
                if (ccList != null && ccList.Count > 0)
                {
                    ccList.ToList().ForEach(it => message.CC.Add(it)); //添加抄送人
                }

                //添加附件
                if (files != null && files.Count > 0)
                {
                    files.ToList().ForEach(it =>
                    {
                        Attachment attachment = new Attachment(it);
                        message.Attachments.Add(attachment);
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
