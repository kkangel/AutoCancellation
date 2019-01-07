using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace AutoCancellation
{
    class Program
    {
        

        static void Main(string[] args)
        {
            string ipaddress = "9.0.6.69";
            //string ipaddress = "218.29.135.230";

            //string name = "411421199207292103";
            //string pwd = "123456..";
            //string comcode = "4101726001";

            //未来 乔
            //string name = "412322197909194869";
            //string pwd = "789789++";
            //string comcode = "41011064";

            //互动二部
            string name = "411123199004234524";
            string pwd = "0.0.0.0.";
            string comcode = "4101943202";
            string qrcode = "909647";

            //东区
            //string name = "412728199110253126";
            //string pwd = "....0000";
            //string comcode = "41017000";

            //string name = "412724199309117951";
            //string pwd = "123456++";
            //string comcode = "41017200";

            string cookieAllTheTime = "";
            
            Console.WriteLine("欢迎使用自动撤单系统");
            Console.WriteLine("系统登陆中");
           // Console.Write("请输入工号：");
           // string name = Console.ReadLine();
           // Console.Write("请输入密码:");
           //string pwd= Console.ReadLine();
           // Console.Write("请输入登录机构代码:");
           // string comcode= Console.ReadLine();

            User userCancellation = new User(name, pwd, comcode,qrcode, ipaddress);
            LoginInsure login = new LoginInsure();
            if( login.login(userCancellation))
            {
                cookieAllTheTime = login.cookiealltheway;
                Console.WriteLine("用户{0}:登陆成功",name);
                cookieAllTheTime = login.cookiealltheway;
                Console.Write("请输入要撤单的起始日期（YYYY-MM-DD）：");
                string strDateStart=Console.ReadLine();
                DateTime dtDateStart = Convert.ToDateTime(strDateStart);
                Console.Write("请输入要撤单的截止日期（YYYY-MM-DD）：");
                string strDateEnd = Console.ReadLine();
                DateTime dtDateEnd = Convert.ToDateTime(strDateEnd);
                //Console.WriteLine(dtDateStart.ToString("yyyy-MM-dd"));
                //Console.WriteLine(dtDateEnd.ToString("yyyy-MM-dd"));
                //Console.ReadLine();
                Cancellation ck = new Cancellation(userCancellation);
                Queue dateTime = new Queue();
                for (DateTime i=dtDateStart;i<=dtDateEnd;i= i.AddDays(1))
                {

                    dateTime.Enqueue(i);
                    

                }
                ck.StartCancel(dateTime, cookieAllTheTime);
                Console.WriteLine("{0}至{1}撤单完成",strDateStart,strDateEnd);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("用户名或密码错误,请重新登录");
                Console.ReadLine();
            }
            
        }
    }
}
