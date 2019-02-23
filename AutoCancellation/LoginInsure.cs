using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using CsharpHttpHelper;
//using System.Web.HttpUtility;

namespace AutoCancellation
{
    class LoginInsure
    {
        private bool _islogin;//登录状态
        private string _cookiealltheway;//cookie
        private HttpResult _hhloginresult;
        public string userName { get; set; }
        public HttpResult hhloginresult
        {
            get
            {
                return this._hhloginresult;
            }
            set
            {
                this._hhloginresult = value;
            }
        }
        public bool islogin
        {
            get
            {
                return this._islogin;
            }
            set
            {
                this._islogin = value;
            }
        }
        public string cookiealltheway
        {
            get
            {
                return this._cookiealltheway;
            }
            set
            {
                this._cookiealltheway = value;
            }
        }

        public bool login(User user)
        {
            this._hhloginresult = new HttpResult();
            HttpHelper httphelper = new HttpHelper();
            string strloginname = user._id;
            string strloginpwd = user._pwd;
            
            string strlongincomcode = user._comcode;
            string ipAddress = user._ipAddress;
            string strqrcode = user._qrCode;
            HttpItem hiloginItem = new HttpItem();
            //取登录页面的cookie
            hiloginItem.URL = string.Format("http://{0}:7001/prpall/index.jsp",ipAddress);//URL这里都是测试     必需项
            //hiloginItem.URL = "http://9.0.6.69:7001/prpall/index.jsp";//URL这里都是测试     必需项
            //hiloginItem.Referer = "http://9.0.6.69:7031/claimCar/logonin.do";

            hiloginItem.ContentType = "application/x-www-form-urlencoded";
            //hiloginItem.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, *";
            //hiloginItem.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            //hiloginItem.Encoding = null;//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
            //Encoding = Encoding.Default,
            hiloginItem.Method = "Post";//URL     可选项 默认为Get
                                        //hiloginItem.Postdata = string.Format("loginMessage=&alertText=&prpDuserUserCode={0}&prpDuserPassword={1}&prpDuserComCode=0000000000&imageField.x=44&imageField.y=19", loginname, loginpwd);
                                        //hiloginItem.Postdata = string.Format("alertText=&prpDuserUserCode={0}&prpDuserPassword={1}&prpDuserComCode=0000000000&loginSyncRTXFlag=0&imageField.x=38&imageField.y=13", strloginname, strloginpwd);
            hiloginItem.Cookie = "userLoginInfo_prpall1479551080272=-117X-40X22X10X62X-22X-124X124X-62X-106X9X10X45X-9X3X4X117X51X-103X-41X-83X-22X79X54X76X-87X-91X-84X126X67X-64X88X21X-110X-119X52X124X-90X-59X-104; WMONID=gmulm43ACaG";
            this._hhloginresult = httphelper.GetHtml(hiloginItem);
            this._cookiealltheway = this._hhloginresult.Cookie;
            //用户名密码登录
            hiloginItem.URL = string.Format("http://{0}:7001/prpall/UICentralControl?SelfPage=/common/pub/UILogonInput.jsp",ipAddress);
            //hiloginItem.URL = string.Format("http://9.0.6.69:7001/prpall/UICentralControl?SelfPage=/common/pub/UILogonInput.jsp");
            strloginpwd = System.Web.HttpUtility.UrlEncode(strloginpwd);
            //sessionUserCode=&sessionComCode=&sessionUserName=&QRCodeSwitch=1&UserCode=411123199004234524&Password=0.0.0.0.&ComCode=4101943202&qrCode=909647&RiskCode=0511&ClassCode=&ClassCodeSelect=05&RiskCodeSelect=0511&USE0509COM=%2C12%2C&CILIFESPECIALCITY=%2C2102%2C3302%2C3502%2C3702%2C4402%2C&image.x=41&image.y=12
            hiloginItem.Postdata = string.Format("sessionUserCode=&sessionComCode=&sessionUserName=&QRCodeSwitch=1&UserCode={0}&Password={1}&ComCode={2}&qrCode={3}&RiskCode=0511&ClassCode=&ClassCodeSelect=05&RiskCodeSelect=0511&USE0509COM=%2C12%2C&CILIFESPECIALCITY=%2C2102%2C3302%2C3502%2C3702%2C4402%2C&image.x=102&image.y=13", strloginname, strloginpwd, strlongincomcode,strqrcode);


            this._hhloginresult = httphelper.GetHtml(hiloginItem);
            this._cookiealltheway = this._hhloginresult.Cookie;
            string strname= @"parent.document.frames(""fraTitle"").setTitleInfo(""";
            string strnameend = @"";
            userName= ContentHelper.GetMidString(hhloginresult.Html, strname, strnameend);
            return this._islogin = !this._hhloginresult.Html.Contains("302 Moved Temporarily");
        }
    }
}
