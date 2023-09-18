using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Matt.Rp插件基础环境.Methods
{
    public class EmailStampServer
    {
        private MailMessage msg;
        private SmtpClient client;
        private Task task;
        /// <summary>
        /// 初始化stamp服务
        /// </summary>
        /// <param name="serverAddress">邮箱服务地址</param>
        /// <param name="serverPassword">邮箱服务密码</param>
        /// <param name="smtpIp">该类型邮箱smtp的服务Ip</param>
        /// <param name="sendAddress">发送邮件地址</param>
        /// <param name="sendName">发送者名字</param>
        public EmailStampServer(string serverAddress, string serverPassword, string smtpIp, string sendAddress, string sendName)
        {
            msg = new MailMessage();
            client = new SmtpClient();
            msg.From = new MailAddress(sendAddress, sendName, Encoding.UTF8);
            /* 上面3个参数分别是发件人地址（可以随便写），发件人姓名，编码*/
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(serverAddress, serverPassword);
            client.Host = smtpIp;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="receiveAddress">接受邮件的地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        public void sendTo(string receiveAddress, string title, string content)
        {
            try
            {
                if (task != null) task.Wait();
            }
            catch { }
            finally
            {
                task = new Task(() => sendEmail(receiveAddress, title, content));
                task.Start();
            }
        }
        private void sendEmail(string receiveAddress, string title, string content)
        {
            msg.To.Add(new MailAddress(receiveAddress));
            msg.Subject = title;//邮件标题    
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码    
            msg.Body = content;//邮件内容    
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码    
            msg.IsBodyHtml = false;//是否是HTML邮件    
            msg.Priority = MailPriority.High;//邮件优先级    
            try
            {
                client.Send(msg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine($"{ex}\r\n发送邮件出错");
            }
        }
    }
}
