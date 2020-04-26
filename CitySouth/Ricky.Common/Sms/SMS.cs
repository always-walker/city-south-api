using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
namespace Ricky.Sms
{
    abstract public class SMS
    {
        public string appkey;
        public string appsecret;
        public string param;
        protected string url;
        protected string method = "GET";
        protected Encoding encoding = Encoding.UTF8;
        public string errmsg = "";
        public string signname = "";
        #region 构造函数
        public SMS()
        {
        }
        public SMS(string appkey, string secretkey)
        {
            this.appkey = appkey;
            this.appsecret = secretkey;
        }
        public SMS(string appkey, string secretkey, string param)
        {
            this.appkey = appkey;
            this.appsecret = secretkey;
            this.param = param;
        }
        public SMS(string signname, string appkey, string secretkey, string param)
        {
            this.signname = signname;
            this.appkey = appkey;
            this.appsecret = secretkey;
            this.param = param;
        }
        #endregion
        public abstract bool Send(string phone, string content);
        public abstract bool Send(string phone, string content, string template);
        #region 创建短信发送实例
        public static SMS Instance(string ClassName)
        {
            SMS sms = (SMS)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            return sms;
        }
        public static SMS Instance(string ClassName, string appkey, string secretkey)
        {
            SMS sms = (SMS)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            sms.appkey = appkey;
            sms.appsecret = secretkey;
            return sms;
        }
        public static SMS Instance(string ClassName, string appkey, string secretkey, string param)
        {
            SMS sms = (SMS)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            sms.appkey = appkey;
            sms.appsecret = secretkey;
            sms.param = param;
            return sms;
        }
        public static SMS Instance(string ClassName, string signname, string appkey, string secretkey, string param)
        {
            SMS sms = (SMS)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            sms.signname = signname;
            sms.appkey = appkey;
            sms.appsecret = secretkey;
            sms.param = param;
            return sms;
        }
        #endregion
        public string Request(string querys, string bodys, WebHeaderCollection headers)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }
            if (url.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers = headers;
            if (0 < bodys.Length)
            {
                byte[] data = encoding.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, encoding);
            return reader.ReadToEnd();
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
