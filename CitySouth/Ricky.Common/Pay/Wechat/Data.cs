using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Ricky.Pay.Wechat
{
    /// <summary>
    /// 微信支付协议接口数据类，所有的API接口通信都依赖这个数据结构，
    /// 在调用接口之前先填充各个字段的值，然后进行接口通信，
    /// 这样设计的好处是可扩展性强，用户可随意对协议进行更改而不用重新设计数据结构，
    /// 还可以随意组合出不同的协议数据包，不用为每个协议设计一个数据包结构
    /// </summary>
    public class WxPayData
    {
        public string appid { get; set; }
        public string secret { get; set; }
        public string mch_id { get; set; }
        public string mch_key { get; set; }
        public WxPayData()
        {
        }
        public WxPayData(string appid, string secret, string mch_id, string mch_key)
        {
            this.appid = appid;
            this.secret = secret;
            this.mch_id = mch_id;
            this.mch_key = mch_key;
        }
        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        public SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return null != o;
        }
        /**
        * @将Dictionary转成xml
        * @return 经转换得到的xml串
        * @throws WxPayException
        **/
        public string ToXml()
        {
            //数据为空时不能转化为xml格式
            string xml = "<xml>";
            if (m_values.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in m_values)
                {
                    //字段值不能为null，会影响后续流程
                    if (pair.Value != null)
                    {
                        if (pair.Value.GetType() == typeof(int))
                        {
                            xml += "<" + pair.Key + ">" + pair.Value.ToString() + "</" + pair.Key + ">";
                        }
                        else if (pair.Value.GetType() == typeof(string))
                        {
                            xml += "<" + pair.Key + ">" + pair.Value.ToString() + "</" + pair.Key + ">";
                            //xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                        }
                    }
                }
            }
            xml += "</xml>";
            return xml;
        }
        /**
        * @将xml转为WxPayData对象并返回对象内部的数据
        * @param string 待转换的xml串
        * @return 经转换得到的Dictionary
        * @throws WxPayException
        */
        public SortedDictionary<string, object> FromXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }
            //2015-06-29 错误是没有签名
            if (m_values["return_code"].ToString() != "SUCCESS")
                m_values["CheckStatus"] = false;
            else
            {
                if (CheckSign())
                    m_values["CheckStatus"] = true;
                else
                    m_values["CheckStatus"] = false;
            }
            return m_values;
        }

        /**
        * @Dictionary格式转化成url参数格式
        * @ return url格式串, 该串不包含sign字段值
        */
        public string ToUrl()
        {
            string buff = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value != null && pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        /**
        * @Dictionary格式化成Json
         * @return json串数据
        */
        public string ToJson()
        {
            string jsonStr = "{";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value != null)
                {
                    if (typeof(int) == pair.Value.GetType())
                        jsonStr += "\"" + pair.Key + "\":" + pair.Value.ToString() + ",";
                    else if (typeof(string) == pair.Value.GetType())
                        jsonStr += "\"" + pair.Key + "\":\"" + pair.Value.ToString() + "\",";
                }
            }
            jsonStr = jsonStr.TrimEnd(',') + "}";
            return jsonStr;
        }
        /**
        * @values格式化成能在Web页面上显示的结果（因为web页面上不能直接输出xml格式的字符串）
        */
        public string ToPrintStr()
        {
            string str = "";
            foreach (KeyValuePair<string, object> pair in m_values)
            {
                if (pair.Value != null)
                {
                    str += string.Format("{0}={1}<br>", pair.Key, pair.Value.ToString());
                }
            }
            return str;
        }
        /**
        * @生成签名，详见签名生成算法
        * @return 签名, sign字段不参加签名
        */
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + mch_key;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
        /**
        * 
        * 检测签名是否正确
        * 正确返回true，错误抛异常
        */
        public bool CheckSign()
        {
            //如果没有设置签名，则跳过检测
            if (!IsSet("sign"))
                return false;
            //如果设置了签名但是签名为空，则抛异常
            else if(GetValue("sign") == null || GetValue("sign").ToString() == "")
                return false;
            //获取接收到的签名
            string return_sign = GetValue("sign").ToString();
            //在本地计算新的签名
            string cal_sign = MakeSign();
            if (cal_sign == return_sign)
                return true;
            else
                return false;
        }
        /**
        * @获取Dictionary
        */
        public SortedDictionary<string, object> GetValues()
        {
            return m_values;
        }

        public static WxPayData Sendredpack(WxPayData inputObj, string SSLCERT_PATH, string SSLCERT_PASSWORD, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
            //检测必填参数
            if (!inputObj.IsSet("mch_billno"))//商户订单号
                return null;
            else if (!inputObj.IsSet("send_name"))//商户名称
                return null;
            else if (!inputObj.IsSet("re_openid"))//用户openid
                return null;
            else if (!inputObj.IsSet("total_amount"))//付款金额
                return null;
            else if (!inputObj.IsSet("total_num"))//红包发放总人数
                return null;
            else if (!inputObj.IsSet("wishing"))//红包祝福语
                return null;
            else if (!inputObj.IsSet("act_name"))//活动名称
                return null;
            else if (!inputObj.IsSet("remark"))//备注
                return null;
            //设置商户信息
            inputObj.SetValue("wxappid", inputObj.appid);//公众账号ID
            inputObj.SetValue("mch_id", inputObj.mch_id);//商户号
            //获取本机IP
            string name = Dns.GetHostName();
            string ipAddress = "192.168.0.1";
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            foreach (IPAddress ip in ipadrlist)
            {
                if (Common.IsIpAddress(ip.ToString()))
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }
            //获取结束
            inputObj.SetValue("client_ip", ipAddress);//终端ip
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();
            string response = Post(xml, url, true, timeOut, SSLCERT_PATH, SSLCERT_PASSWORD);
            WxPayData result = new WxPayData(inputObj.appid, inputObj.secret, inputObj.mch_id, inputObj.mch_key);
            result.FromXml(response);
            return result;
        }

        public static WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
                return null;
            else if (!inputObj.IsSet("body"))
                return null;
            else if (!inputObj.IsSet("total_fee"))
                return null;
            else if (!inputObj.IsSet("trade_type"))
                return null;
            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
                return null;
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
                return null;
            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
                return null;
            inputObj.SetValue("appid", inputObj.appid);//公众账号ID
            inputObj.SetValue("mch_id", inputObj.mch_id);//商户号
            inputObj.SetValue("spbill_create_ip", HttpContext.Current.Request.UserHostAddress);//终端ip	  	    
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();
            string response = Post(xml, url, false, timeOut, null, null);
            //string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "paylog\\" + Common.NowDate.ToString("yyMMdd") + ".log";
            //File.AppendAllText(path, response, Encoding.UTF8);
            WxPayData result = new WxPayData(inputObj.appid, inputObj.secret, inputObj.mch_id, inputObj.mch_key);
            result.FromXml(response);
            return result;
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
        public static string Post(string xml, string url, bool isUseCert, int timeout, string SSLCERT_PATH, string SSLCERT_PASSWORD)
        {
            System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }
                /***************************************************************
                * 下面设置HttpWebRequest的相关属性
                * ************************************************************/
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = timeout * 1000;
                //设置代理服务器
                //WebProxy proxy = new WebProxy();                          //定义一个网关对象
                //proxy.Address = new Uri(WxPayConfig.PROXY_URL);              //网关服务器端口:端口
                //request.Proxy = proxy;

                //设置POST的数据类型和长度
                request.ContentType = "text/xml";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                request.ContentLength = data.Length;

                //是否使用证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + SSLCERT_PATH, SSLCERT_PASSWORD);
                    request.ClientCertificates.Add(cert);
                }
                //往服务器写入数据
                reqStream = request.GetRequestStream();
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                //获取服务端返回数据
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (System.Threading.ThreadAbortException e)
            {
                System.Threading.Thread.ResetAbort();
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }
        public static WxPayData GetNotifyData(string mch_key)
        {
            //接收从微信后台POST过来的数据
            System.IO.Stream s = HttpContext.Current.Request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();
            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            data.mch_key = mch_key;
            data.FromXml(builder.ToString());
            return data;
        }
    }
}