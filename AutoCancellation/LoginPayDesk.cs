using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsharpHttpHelper;

namespace AutoCancellation
{
    class LoginPayDesk
    {
        private bool _islogin; //登录状态
        private string _loginCookiealltheway; //登录系统的cookie
        private string _payDeskLoginCookiealltheway;
        private HttpResult _hhloginresult;
        private HttpResult _hhPayDeskResult;
        public HttpResult hhloginresult
        {
            get { return this._hhloginresult; }
            set { this._hhloginresult = value; }
        }

        public HttpResult HhPayDeskResult
        {
            get { return this._hhPayDeskResult; }
            set { this._hhPayDeskResult = value; }
        }

        public bool islogin
        {
            get { return this._islogin; }
            set { this._islogin = value; }
        }

        public string LoginCookiealltheway
        {
            get { return this._loginCookiealltheway; }
            set { this._loginCookiealltheway = value; }
        }

        public string PayDeskLoginCookiealltheway
        {
            get { return this._payDeskLoginCookiealltheway; }
            set { this._payDeskLoginCookiealltheway = value; }
        }

        public bool login(User user)
        {
            this._hhloginresult = new HttpResult();
            this._hhPayDeskResult=new HttpResult();
            islogin = false;
            HttpHelper httphelper = new HttpHelper();
            string strloginname = user._id;
            string strloginpwd = user._pwd;
            string strlogincomcode = user._comcode;
            string ipLoginAddress = user._ipAddress;
            string ipPayDeskAddress = "9.0.9.48";
            string strqrcode = user._qrCode;
            string strRedirectUrl = "";
            HttpItem hiloginItem = new HttpItem();
            HttpItem hiPayDeskLoginItem= new HttpItem();
            
            //取登收银台登录状态  /prpall/system/checkusergrade/UserGradePowerCheck.jsp?UserCode=411123199004234524&ComCode=4101943202&TaskCode=prpall.Planfee.newPayment.pay
            hiloginItem.URL = string.Format("http://{0}:7001/prpall/system/checkusergrade/UserGradePowerCheck.jsp?UserCode={1}&ComCode={2}&TaskCode=prpall.Planfee.newPayment.pay", ipLoginAddress,strloginname,strlogincomcode); //URL这里都是测试     必需项
            //hiloginItem.URL = "http://9.0.6.69:7001/prpall/index.jsp";//URL这里都是测试     必需项
            hiloginItem.Referer = String.Format("http://{0}:7001/prpall/processUtiMenu.do?actionType=showMenu&taskCode=showMenu&menuStyle=null",ipLoginAddress);
            hiloginItem.Cookie = LoginCookiealltheway;
            hiloginItem.ContentType = "text/xml";
            //hiloginItem.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/x-silverlight, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/x-ms-application, application/x-ms-xbap, application/vnd.ms-xpsdocument, application/xaml+xml, application/x-silverlight-2-b1, *";
            //hiloginItem.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
            //hiloginItem.Encoding = null;//编码格式（utf-8,gb2312,gbk）     可选项 默认类会自动识别
            //Encoding = Encoding.Default,
            hiloginItem.Method = "Post"; //URL     可选项 默认为Get
            //hiloginItem.Postdata = string.Format("loginMessage=&alertText=&prpDuserUserCode={0}&prpDuserPassword={1}&prpDuserComCode=0000000000&imageField.x=44&imageField.y=19", loginname, loginpwd);
            //hiloginItem.Postdata = string.Format("alertText=&prpDuserUserCode={0}&prpDuserPassword={1}&prpDuserComCode=0000000000&loginSyncRTXFlag=0&imageField.x=38&imageField.y=13", strloginname, strloginpwd);
            hiloginItem.Cookie =
                LoginCookiealltheway;
            this._hhloginresult = httphelper.GetHtml(hiloginItem);
            //LoginCookiealltheway = this._hhloginresult.Cookie;
            if (_hhloginresult.Html.Contains("true"))
            {
                //跨域跳转第二步 /prpall/SGJreceipts.do?actionType=deskPay&TaskCode=prpall.Planfee.newPayment.pay&usercode=411123199004234524&comcode=4101943202
                hiloginItem.URL=String.Format("http://{0}:7001/prpall/SGJreceipts.do?actionType=deskPay&TaskCode=prpall.Planfee.newPayment.pay&usercode={1}&comcode={2}",ipLoginAddress,strloginname,strlogincomcode);
                hhloginresult = httphelper.GetHtml(hiloginItem);
                if (hhloginresult.StatusCode == HttpStatusCode.Found)
                {
                    //跨域跳转第三步 
                    string urlhead = string.Format("http://{0}:7001/prpall/", ipLoginAddress);
                    hiloginItem.URL = urlhead+  hhloginresult.Header["Location"];
                    hhloginresult = httphelper.GetHtml(hiloginItem);
                    if (hhloginresult.StatusCode == HttpStatusCode.OK)
                    {
                        if (hhloginresult.Html.Contains("window.open"))
                        {
                            string strstart = @"window.open(""";
                            string strend = @"""";
                             strRedirectUrl= ContentHelper.GetMidString(hhloginresult.Html, strstart, strend);
                            
                        }
                    }
                }

            }

            if (strRedirectUrl != "")
            {
                hiPayDeskLoginItem.URL = string.Format("http://9.0.9.48:7001/bpc/controller/bpcbasecode/codeDetail/findCodeDetails?baseCodeIds=CertiType,PrePayCode,ChequeType,IdentityType,DraftType,SeeFeeStatus,SurplusFeeDealMode,FzjCertiTypeCode,CertiTypeCode,FzjCertiNoTypeCode,CertiNoTypeCode,CurrencyCode,PayRefReason,ProductCode,ChannelType,CollStatus,AgriAllowanceType,CommissionFlag");
                hiPayDeskLoginItem.Referer = strRedirectUrl;
                //hiPayDeskLoginItem.Accept = "application/javascript, */*;q=0.8";
                //hiPayDeskLoginItem.Method = "Get";
                //hiPayDeskLoginItem.ContentType = "";
                //hiPayDeskLoginItem.Header["Accept-Encoding"] = "gzip, deflate";
                //hiPayDeskLoginItem.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                //hiPayDeskLoginItem.Header["Accept-Language"] = "zh-Hans-CN,zh-Hans;q=0.5";
                HhPayDeskResult = httphelper.GetHtml(hiPayDeskLoginItem);
                PayDeskLoginCookiealltheway = HhPayDeskResult.Cookie;
                PayDeskLoginCookiealltheway = Regex.Replace(PayDeskLoginCookiealltheway, "path=/[,;]*", "", RegexOptions.IgnoreCase);
                PayDeskLoginCookiealltheway = Regex.Replace(PayDeskLoginCookiealltheway, "httponly[,;]*", "", RegexOptions.IgnoreCase);
                PayDeskLoginCookiealltheway = Regex.Replace(PayDeskLoginCookiealltheway, @"domain=.gpic.com.cn[,;]*", "", RegexOptions.IgnoreCase);
                if (HhPayDeskResult.StatusCode == HttpStatusCode.OK)
                {
                    islogin = true;
                }
            }
            
            return islogin;
        }
    }
}
