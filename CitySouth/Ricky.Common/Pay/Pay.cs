using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ricky.Pay
{
    abstract  public class Pay
    {
        protected string url;
        protected string method = "GET";
        protected Encoding encoding = Encoding.UTF8;
        #region
        private string _merchantNo;
        public string MerchantNo { get { return _merchantNo; } set { _merchantNo = value; } }
        private string _merchantKey;
        public string MerchantKey { get { return _merchantKey; } set { _merchantKey = value; } }
        private string _actionUrl;
        public string ActionUrl { get { return _actionUrl; } set { _actionUrl = value; } }
        public string OrderSn { get; set; }
        public string Message { get; set; }
        public string Param1 { get; set; }
        public string ChangeUrl { get; set; }
        #endregion
        #region 实例化
        public static Pay Instance(string ClassName)
        {
            Pay pay = (Pay)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            return pay;
        }
        public static Pay Instance(string ClassName, string merchantNo, string merchantKey, string actionUrl)
        {
            Pay pay = (Pay)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            pay.MerchantNo = merchantNo;
            pay.MerchantKey = merchantKey;
            pay.ActionUrl = actionUrl;
            return pay;
        }
        public static Pay Instance(string ClassName, string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password)
        {
            Pay pay = (Pay)Type.GetType(ClassName).Assembly.CreateInstance(ClassName);
            pay.MerchantNo = merchantNo;
            pay.MerchantKey = merchantKey;
            pay.ActionUrl = actionUrl;
            pay.SetCert(pfxFileName, password);
            return pay;
        }
        #endregion
        #region RSA签名
        private X509Certificate2 _certificate;
        private string _privateKey;
        private string _publicKey;
        #endregion
        #region 构造函数
        public Pay()
        {

        }
        public Pay(string merchantNo, string merchantKey, string actionUrl)
        {
            _merchantNo = merchantNo;
            _merchantKey = merchantKey;
            _actionUrl = actionUrl;
        }
        public Pay(string merchantNo, string merchantKey, string actionUrl, string pfxFileName, string password)
        {
            _merchantNo = merchantNo;
            _merchantKey = merchantKey;
            _actionUrl = actionUrl;
            this._certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory.ToString().TrimEnd('\\') + "\\" + pfxFileName, password, X509KeyStorageFlags.Exportable);
            this._privateKey = _certificate.PrivateKey.ToXmlString(true);
            this._publicKey = _certificate.PublicKey.Key.ToXmlString(false);
        }
        public void SetCert(string pfxFileName, string password)
        {
            this._certificate = new X509Certificate2(AppDomain.CurrentDomain.BaseDirectory.ToString().TrimEnd('\\') + "\\" + pfxFileName, password, X509KeyStorageFlags.Exportable);
            this._privateKey = _certificate.PrivateKey.ToXmlString(true);
            this._publicKey = _certificate.PublicKey.Key.ToXmlString(false);
        }
        #endregion
        public abstract string FormString(string OrderNo, decimal Amount, string frontUrl, string backUrl, string BankCode, string userId, string subject, string body);
        public abstract bool VerifyCallback(Dictionary<string, string> param);
        public abstract string WechatImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body);
        public abstract string AlipayImgUrl(string OrderNo, decimal Amount, string backUrl, string BankCode, string subject, string body);
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
        #region 签名和验证签名
        public string SignatureFormatter(string strEncryptString)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strEncryptString);
            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(this._privateKey);
            byte[] signatureBytes = key.SignData(bytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }
        public bool SignatureDeformatter(string sourceData, string strEncryptString)
        {
            try
            {
                byte[] rgbHash = Encoding.UTF8.GetBytes(sourceData);
                RSACryptoServiceProvider key = new RSACryptoServiceProvider();
                key.FromXmlString(this._publicKey);
                bool result = key.VerifyData(rgbHash, "SHA1", Convert.FromBase64String(strEncryptString));
                return result;
            }
            catch
            {
                return false;
            }
        }
        public string PublicKeyEncrypt(string strEncryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(this._publicKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(strEncryptString);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }
        public string PrivateKeyEncrypt(string strEncryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(this._privateKey);
            byte[] bytes = new UnicodeEncoding().GetBytes(strEncryptString);
            return Convert.ToBase64String(provider.Encrypt(bytes, false));
        }
        public string PrivateKeyDecrypt(string strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(this._privateKey);
            byte[] rgb = Convert.FromBase64String(strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }
        public string PublicKeyDecrypt(string strDecryptString)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(this._publicKey);
            byte[] rgb = Convert.FromBase64String(strDecryptString);
            byte[] bytes = provider.Decrypt(rgb, false);
            return new UnicodeEncoding().GetString(bytes);
        }
        public string md5(string str, Encoding encode)
        {
            byte[] b = encode.GetBytes(str);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion
        #region 组合
        public string GetSignStr(Dictionary<string, string> dic)
        {
            dic = SortDictionary(dic);
            string sigSource = "";
            foreach (var item in dic)
            {
                if (item.Value != null && item.Value.Trim().Length > 0)
                {
                    sigSource = sigSource + item.Key + "=" + item.Value + "&";
                }
            }
            if (sigSource.Length > 1)
            {
                sigSource = sigSource.Substring(0, sigSource.Length - 1);
            }
            return sigSource;
        }
        protected Dictionary<string, string> SortDictionary(Dictionary<string, string> dic)
        {
            List<KeyValuePair<string, string>> myList = new List<KeyValuePair<string, string>>(dic);
            myList.Sort(delegate(KeyValuePair<string, string> s1, KeyValuePair<string, string> s2)
            {
                return s1.Key.CompareTo(s2.Key);
            });
            dic.Clear();
            foreach (KeyValuePair<string, string> pair in myList)
            {
                dic.Add(pair.Key, pair.Value);
            }
            return dic;
        }
        #endregion
        public void logstr(string str)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory.ToString().TrimEnd('\\') + "\\paylog\\" + Common.NowDate.ToString("yyMMdd") + ".log", true);
                sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                sw.WriteLine(Common.NowDate.ToString() + "[" + str + "]");
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public Dictionary<string, string> Request2Dic(HttpRequestBase request)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (string name in request.Form.Keys)
            {
                if (!dic.ContainsKey(name))
                    dic.Add(name, request.Form[name]);
            }
            foreach (string name in request.QueryString.Keys)
            {
                if (!dic.ContainsKey(name))
                    dic.Add(name, request.Form[name]);
            }
            return dic;
        }
    }
}
