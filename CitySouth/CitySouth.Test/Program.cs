using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LumiSoft.Net.POP3.Client;
using LumiSoft.Net.Mail;
using System.IO;
using System.Data;

namespace CitySouth.Test
{
    public class LogItem
    {
        public LogItem(string name, object[] value)
        {
            this.name = name;
            this.value = value;
        }
        public string name { get; set; }
        public object[] value { get; set; }
    }
    class Program
    {
        public static void GetEmails()
        {
            using (POP3_Client c = new POP3_Client())
            {
                c.Connect("pop.qq.com", 995, true);
                c.Login("252697949@qq.com", "evyacsgisnylbiac");
                Console.WriteLine(c.Messages.Count.ToString());
                List<string> successList = new List<string>();
                List<string> failedList = new List<string>();
                if (c.Messages.Count > 0)
                {
                    for (var i = 0; i < c.Messages.Count; i++)
                    {
                        try
                        {
                            var t = Mail_Message.ParseFromByte(c.Messages[i].MessageToByte());
                            var from = t.From;
                            var to = t.To;
                            var date = t.Date;
                            var subject = t.Subject;
                            var bodyText = t.BodyText;
                            if (subject == "圕哥邮件通知")
                            {
                                Console.WriteLine(bodyText);
                                if (bodyText.StartsWith("参赛人信息"))
                                    successList.Add(bodyText);
                                else
                                    failedList.Add(bodyText);
                            }
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message);
                        }
                    }
                    string folder = AppDomain.CurrentDomain.BaseDirectory.ToString();
                    if(successList.Count > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("姓名");
                        dt.Columns.Add("学院");
                        dt.Columns.Add("学号");
                        dt.Columns.Add("电话");
                        dt.Columns.Add("QQ");
                        foreach (string line in successList)
                        {
                            string[] item = line.Replace("参赛人信息：", "").Split('，');
                            DataRow dr = dt.NewRow();
                            dr["姓名"] = item[0].Replace("姓名：", "");
                            dr["学院"] = item[1].Replace("学院：", "");
                            dr["学号"] = item[2].Replace("学号：", "");
                            dr["电话"] = item[3].Replace("电话号码：", "");
                            dr["QQ"] = item[4].Replace("QQ：", "");
                            dt.Rows.Add(dr);
                        }
                        Console.WriteLine("导出愿意参赛.xlsx");
                        Excel succExcle = new Excel(dt);
                        succExcle.Save(folder + "愿意参赛.xlsx");
                    }
                    if (failedList.Count > 0)
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("姓名");
                        dt.Columns.Add("学号");
                        foreach (string line in failedList)
                        {
                            string[] item = line.Replace("拒绝参赛：", "").Split('，');
                            DataRow dr = dt.NewRow();
                            dr["姓名"] = item[0].Replace("姓名：", "");
                            dr["学号"] = item[1].Replace("学号：", "");
                            dt.Rows.Add(dr);
                        }
                        Console.WriteLine("导出拒绝参赛.xlsx");
                        Excel succExcle = new Excel(dt);
                        succExcle.Save(folder + "拒绝参赛.xlsx");
                    }
                    Console.WriteLine("同意参加" + successList.Count.ToString() + "人");
                    Console.WriteLine("拒绝参赛" + failedList.Count.ToString() + "人");
                    Console.WriteLine("高完");
                }
            }
        }
        static void Main(string[] args)
        {
            //LogItem v1 = new LogItem("a1", new object[] { "1", "2" });
            //LogItem v2 = new LogItem("a1", new object[] { "1", "2" });
            //Console.WriteLine(v1.value[0].Equals(v2.value[0]));
            //List<LogItem> items = new List<LogItem>();
            //items.Add(new LogItem("a1", new object[] { "1", "2" }));
            //items.Add(new LogItem("a2", new object[] { 1, 2 }));
            //items.Add(new LogItem("a3", new object[] { DateTime.UtcNow, DateTime.Now }));
            //string ss = JsonConvert.SerializeObject(items);
            //Console.WriteLine(ss);
            //GetEmails();
            DataTable dt = new DataTable();
            dt.Columns.Add("x");
            DataRow dr = dt.NewRow();
            dr[0] = DateTime.Now;
            dt.Rows.Add(dr);
            string xx = dt.Rows[0][0].ToString();
            Console.WriteLine(xx);
            Console.ReadLine();
        }
    }
}
