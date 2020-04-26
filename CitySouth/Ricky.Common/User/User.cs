using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Ricky.Third.User
{
    abstract  public class User
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
        protected string url;
        protected string method = "GET";
        protected Encoding encoding = Encoding.UTF8;
        public abstract void Login(string loginName, string password);
        #region URL请求
        public string Request(string querys, string bodys, WebHeaderCollection headers)
        {
            Stream responseStream = null;
            StreamReader reader = null;
            try
            {
                HttpWebRequest httpRequest = null;
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
                #region HEAD参数
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                #endregion
                if (headers != null)
                    httpRequest.Headers = headers;
                if (0 < bodys.Length)
                {
                    byte[] data = encoding.GetBytes(bodys);
                    Stream stream = httpRequest.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }
                responseStream = ((HttpWebResponse)httpRequest.GetResponse()).GetResponseStream();
                reader = new StreamReader(responseStream, encoding);
                return reader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (responseStream != null) responseStream.Close();
            }
        }
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        #endregion
    }
}
