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
            string id = "412322197909194869";
            string pwd = "789789++";
            string comcode = "41011064";
            string qrcode = "925179";

            //互动二部  谢莹莹
            //string id = "411123199004234524";
            //string pwd = "0.0.0.0.";
            //string comcode = "4101943201";
            //string qrcode = "909647";

            //互动二部 范献捷 
            //string id = "410104197406083512";
            //string pwd = "0.0.0.0.";
            //string comcode = "4101943202";
            //string qrcode = "161049";


            //东区
            //string name = "412728199110253126";
            //string pwd = "....0000";
            //string comcode = "41017000";

            //string name = "412724199309117951";
            //string pwd = "123456++";
            //string comcode = "41017200";

            string loginInsureCookiealltheway = "";
            string loginPayDeskalltheway = "";
            
            Console.WriteLine("欢迎使用自动撤单系统");
            Console.WriteLine("系统登陆中");
           // Console.Write("请输入工号：");
           // string name = Console.ReadLine();
           // Console.Write("请输入密码:");
           //string pwd= Console.ReadLine();
           // Console.Write("请输入登录机构代码:");
           // string comcode= Console.ReadLine();

            User userCancellation = new User(id, pwd, comcode,qrcode, ipaddress);
            LoginInsure loginInsure = new LoginInsure();
            if( loginInsure.login(userCancellation))
            {
                userCancellation._userName = loginInsure.userName;
                loginInsureCookiealltheway = loginInsure.cookiealltheway;
                Console.WriteLine("用户{0}:登陆成功",id);
                LoginPayDesk loginpaydesk = new LoginPayDesk();
                loginpaydesk.LoginCookiealltheway = loginInsureCookiealltheway;
                if (loginpaydesk.login(userCancellation))
                {
                    loginPayDeskalltheway = loginpaydesk.PayDeskLoginCookiealltheway;
                    Console.WriteLine("用户{0}:收银台登陆成功", id);
                }

                //loginInsureCookiealltheway = loginpaydesk.LoginCookiealltheway;

                Console.Write("请输入要撤单的起始日期（YYYY-MM-DD）：");
                string strDateStart = Console.ReadLine();
                //测试数据
                //string strDateStart = "2019-01-01";
                //string strDateEnd = "2019-01-03";
                DateTime dtDateStart = Convert.ToDateTime(strDateStart);
                Console.Write("请输入要撤单的截止日期（YYYY-MM-DD）：");
                string strDateEnd = Console.ReadLine();
                DateTime dtDateEnd = Convert.ToDateTime(strDateEnd);
                //Console.WriteLine(dtDateStart.ToString("yyyy-MM-dd"));
                //Console.WriteLine(dtDateEnd.ToString("yyyy-MM-dd"));
                //Console.ReadLine();
                DatePass dtpass= new DatePass(strDateStart,strDateEnd);
                Cancellation ck = new Cancellation(userCancellation, dtpass);
                Queue dateTime = new Queue();
                for (DateTime i=dtDateStart;i<=dtDateEnd;i= i.AddDays(1))
                {

                    dateTime.Enqueue(i);
                    

                }
                //需要执行两遍
                ck.StartCancel(dateTime, loginInsureCookiealltheway,loginPayDeskalltheway);
                ck.StartCancel(dateTime, loginInsureCookiealltheway,loginPayDeskalltheway);
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
