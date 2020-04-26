using System;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
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
    public class Common
    {
        #region 默认数据
        /// <summary>
        /// 默认GUID
        /// </summary>
        public static Guid DefaultGuid = new Guid();
        /// <summary>
        /// 默认加解密密码
        /// </summary>
        public static string DefaultPasswordKey = "ricky*())(*";
        #endregion
        #region 数据验证
        /// <summary>
        /// 检查是否为精确度到4位的数字
        /// </summary>
        /// <param name="strUrl">要测试的字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string strDecimal)
        {
            if (string.IsNullOrEmpty(strDecimal))
                return false;
            return Regex.IsMatch(strDecimal, @"^\-?(0|[1-9]\d*)(\.\d{1,4})?$");
        }
        public static bool IsIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;
            return Regex.IsMatch(ipAddress, @"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)");
        }
        #endregion
        #region 加解密
        /// <summary>
        /// 3des加密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="encryptValue">加密字符</param>
        /// <param name="key">加密key(24字符)</param>
        /// <returns>加密后Base64字符</returns>
        public static string EncryptString(string encryptValue, string key = null)
        {
            if (string.IsNullOrEmpty(key))
                key = DefaultPasswordKey;
            string enstring = "加密出错!";
            ICryptoTransform ct; //需要此接口才能在任何服务提供程序上调用 CreateEncryptor 方法，服务提供程序将返回定义该接口的实际 encryptor 对象。
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateEncryptor();

            byt = Encoding.UTF8.GetBytes(encryptValue);//将原始字符串转换成字节数组。大多数 .NET 加密算法处理的是字节数组而不是字符串。

            //创建 CryptoStream 对象 cs 后，现在使用 CryptoStream 对象的 Write 方法将数据写入到内存数据流。这就是进行实际加密的方法，加密每个数据块时，数据将被写入 MemoryStream 对象。

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                enstring = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                enstring = ex.ToString();
            }
            finally
            {
                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();
                des3.Clear();
                ct.Dispose();
            }
            enstring = Convert.ToBase64String(ms.ToArray());
            return enstring;
        }
        /// <summary>
        /// 字符串截取补字符函数
        /// </summary>
        /// <param name="s">要处理的字符串</param>
        /// <param name="len">长度</param>
        /// <param name="b">补充的字符</param>
        /// <returns>处理后字符</returns>
        public static string splitStringLen(string s, int len, char b)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.Length >= len)
                return s.Substring(0, len);
            return s.PadRight(len, b);
        }
        /// <summary>
        /// 3des解密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="decryptString">解密字符</param>
        /// <param name="key">解密key(24字符)</param>
        /// <returns>解密后字符</returns>
        public static string DecryptString(string decryptString, string key = null)
        {
            if (string.IsNullOrEmpty(decryptString))
                return "";
            if (string.IsNullOrEmpty(key))
                key = DefaultPasswordKey;
            string destring = "解密字符失败!";
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateDecryptor();

            byt = Convert.FromBase64String(decryptString);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                destring = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                destring = ex.ToString();
            }
            finally
            {
                ms.Close();
                cs.Close();
                ms.Dispose();
                cs.Dispose();
                ct.Dispose();
                des3.Clear();
            }
            return destring;
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string md5(string str, int code)
        {
            byte[] result = Encoding.Default.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string md5String = BitConverter.ToString(output).Replace("-", "").ToLower();
            if (code == 16)
                md5String = md5String.Substring(8, 16);
            return md5String;
        }
        #endregion
        #region 时间戳
        public static long GetStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static long GetStamp(DateTime datetime)
        {
            TimeSpan ts = TimeZone.CurrentTimeZone.ToLocalTime(datetime) - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static DateTime GetTimeForStamp(long timestamp)
        {
            if (timestamp.ToString().Length == 10)
                return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(timestamp);
            else
                return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddMilliseconds(timestamp);
        }
        public static DateTime NowDate
        {
            get { return DateTime.UtcNow.AddHours(8); }
        }
        #endregion
        #region LINQ扩展
        /// <summary>  
        /// LINQ返回DataTable类型  
        /// </summary>  
        /// <typeparam name="T"> </typeparam>  
        /// <param name="varlist"> </param>  
        /// <returns> </returns>  
        public static DataTable ToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names  
            PropertyInfo[] oProps = null;

            if (varlist == null)
                return dtReturn;
            foreach (T rec in varlist)
            {
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;
                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
        #endregion
        #region 其它函数
        public static string GetRandom(int digit)
        {
            string RandomSn = "";
            Random r = new Random();
            for (int i = 0; i < digit; i++)
                RandomSn += r.Next(0, 9).ToString();
            return RandomSn;
        }
        #endregion
    }
}