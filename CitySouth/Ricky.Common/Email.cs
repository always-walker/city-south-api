using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**
 * ============================================================================
 * * 版权所有 2017-2030 we-ai中国，并保留所有权利。
 * 网站地址: http://www.we-ai5.com；
 * ----------------------------------------------------------------------------
 * 这不是一个自由软件！您只能在不用于商业目的的前提下对程序代码进行修改和
 * 使用；不允许对程序代码以任何形式任何目的的再发布。
 * ============================================================================
 * @author:     ricky.gz <zgz521@foxmail.com>
 * @version:    v1.0
 * ---------------------------------------------
 */
namespace Ricky
{
    public class Email
    {
        private string smtp;
        private int pop;
        private string email;
        private string password;
        public Encoding encoding { get; set; }
        public Email(string smtp, int pop, string email, string password)
        {
            this.smtp = smtp;
            this.pop = pop;
            this.email = email;
            this.password = password;
            this.encoding = Encoding.UTF8;
        }
        /// <summary>
        /// 群发邮件，收到者会显示所有收到者的邮箱
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="sendMails">接收邮箱</param>
        /// <param name="isHtml">是否显示html格式</param>
        public void Send(string title, string content, string[] sendMails, bool isHtml = false, bool ssl = true)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.IsBodyHtml = isHtml;
            msg.From = new System.Net.Mail.MailAddress(email);
            foreach (string sendMail in sendMails)
            {
                msg.To.Add(new System.Net.Mail.MailAddress(sendMail));
            }
            msg.Subject = title;
            msg.SubjectEncoding = encoding;
            msg.Body = content;
            msg.BodyEncoding = encoding;
            System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient(smtp, pop);
            sm.EnableSsl = ssl;
            sm.UseDefaultCredentials = false;
            sm.Credentials = new System.Net.NetworkCredential(email, password);
            sm.Send(msg);
        }
        /// <summary>
        /// 群发邮件，循环发送邮件，收到者不会显示所有收到者的邮箱
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="sendMails">接收邮箱</param>
        /// <param name="isHtml">是否显示html格式</param>
        public void SendSigle(string title, string content, string[] sendMails, bool isHtml = false, bool ssl = true)
        {
            System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient(smtp, pop);
            sm.Credentials = new System.Net.NetworkCredential(email, password);
            sm.EnableSsl = ssl;
            sm.UseDefaultCredentials = false;
            System.Net.Mail.MailMessage msg;
            foreach (string sendMail in sendMails)
            {
                msg = new System.Net.Mail.MailMessage();
                msg.IsBodyHtml = isHtml;
                msg.From = new System.Net.Mail.MailAddress(email);
                msg.To.Add(new System.Net.Mail.MailAddress(sendMail));
                msg.Subject = title;
                msg.SubjectEncoding = encoding;
                msg.Body = content;
                msg.BodyEncoding = encoding;
                sm.Send(msg);
            }
        }
    }
}
