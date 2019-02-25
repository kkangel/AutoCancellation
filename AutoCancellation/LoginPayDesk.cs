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
            string strloginid = user._id;
            string strloginpwd = user._pwd;
            string strlogincomcode = user._comcode;
            string ipLoginAddress = user._ipAddress;
            string ipPayDeskAddress = "9.0.9.48";
            string strqrcode = user._qrCode;
            string strRedirectUrl = "";
            HttpItem hiloginItem = new HttpItem();
            HttpItem hiPayDeskLoginItem= new HttpItem();

            HttpItem hitest=new HttpItem();
            HttpResult hrtest= new HttpResult();
            HttpHelper hhtestHelper= new HttpHelper();




            //跨域跳转第一步 取登收银台登录状态  /prpall/system/checkusergrade/UserGradePowerCheck.jsp?UserCode=411123199004234524&ComCode=4101943202&TaskCode=prpall.Planfee.newPayment.pay
            hiloginItem.URL = string.Format("http://{0}:7001/prpall/system/checkusergrade/UserGradePowerCheck.jsp?UserCode={1}&ComCode={2}&TaskCode=prpall.Planfee.newPayment.pay", ipLoginAddress,strloginid,strlogincomcode); //URL这里都是测试     必需项
            
            hiloginItem.Referer = String.Format("http://{0}:7001/prpall/processUtiMenu.do?actionType=showMenu&taskCode=showMenu&menuStyle=null",ipLoginAddress);
            hiloginItem.Cookie = LoginCookiealltheway;
            hiloginItem.ContentType = "text/xml";
            hiloginItem.Method = "Post"; //URL     可选项 默认为Get

            this._hhloginresult = httphelper.GetHtml(hiloginItem);
            //LoginCookiealltheway = this._hhloginresult.Cookie;







            if (_hhloginresult.Html.Contains("true"))
            {
                //跨域跳转第二步 /prpall/SGJreceipts.do?actionType=deskPay&TaskCode=prpall.Planfee.newPayment.pay&usercode=411123199004234524&comcode=4101943202
                // hiloginItem.URL ="/prpall/SGJreceipts.do?actionType=deskPay&TaskCode=prpall.Planfee.newPayment.pay&usercode=411123199004234524&comcode=4101943202";

                //这个地方不能加地址 我艹是为啥
                //2019-02-25 临时注销掉
               // hiloginItem.URL=String.Format("/prpall/SGJreceipts.do?actionType=deskPay&TaskCode=prpall.Planfee.newPayment.pay&usercode={0}&comcode={1}", strloginid,strlogincomcode);
               // hiloginItem.Method = "GET";
               // hiloginItem.Accept ="image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*";
               // hiloginItem.UserAgent =
               //     "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)";
               // hiloginItem.Host = "9.0.6.69:7001";
               // hiloginItem.Cookie = LoginCookiealltheway;
               //// hiloginItem.Referer ="http://9.0.6.69:7001/prpall/processUtiMenu.do?actionType=showMenu&taskCode=showMenu&menuStyle=null";
               // hhloginresult = httphelper.GetHtml(hiloginItem);

              
                
                ////测试一下系统登录状态好使不
                //hitest.URL =
                //    "http://9.0.6.69:7001/prpall/common/tb/UIProposalCrossIndexQueryInput.jsp?TaskCode=prpall.a01003002011&usercode=411123199004234524&comcode=4101943202";
                //hitest.Method = "get";
                //hitest.Cookie = LoginCookiealltheway;
                //hrtest = hhtestHelper.GetHtml(hitest);




                if (hhloginresult.StatusCode == HttpStatusCode.Found)
                {
                    //跨域跳转第三步 
                    string urlhead = string.Format("http://{0}:7001/prpall/", ipLoginAddress);
                    hiloginItem.URL = urlhead+  hhloginresult.Header["Location"];
                    hiloginItem.Cookie = LoginCookiealltheway;
                    hiloginItem.Method = "Get";
                    hiloginItem.Accept =
                        "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*";
                    hiloginItem.Referer =
                        "http://9.0.6.69:7001/prpall/processUtiMenu.do?actionType=showMenu&taskCode=showMenu&menuStyle=null";
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
                //跨域跳转过来第一步
                hiPayDeskLoginItem.URL = string.Format("http://9.0.9.48:7001/bpc/controller/bpcbasecode/codeDetail/findCodeDetails?baseCodeIds=CertiType,PrePayCode,ChequeType,IdentityType,DraftType,SeeFeeStatus,SurplusFeeDealMode,FzjCertiTypeCode,CertiTypeCode,FzjCertiNoTypeCode,CertiNoTypeCode,CurrencyCode,PayRefReason,ProductCode,ChannelType,CollStatus,AgriAllowanceType,CommissionFlag");
                hiPayDeskLoginItem.Referer = strRedirectUrl;
                hiPayDeskLoginItem.Method = "Get";
                hiPayDeskLoginItem.Accept = "application/javascript, */*;q=0.8";
                hiPayDeskLoginItem.Cookie = "";
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

                //if (HhPayDeskResult.StatusCode == HttpStatusCode.OK)
                //{
                //    islogin = true;
                //}

            //跨域跳转过来第二步
            hiPayDeskLoginItem.URL = strRedirectUrl;
            hiPayDeskLoginItem.Accept =
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            hiPayDeskLoginItem.Header["Upgrade-Insecure-Requests"] = "1";
            hiPayDeskLoginItem.ContentType = "";
            hiPayDeskLoginItem.Cookie = PayDeskLoginCookiealltheway;
            HhPayDeskResult = httphelper.GetHtml(hiPayDeskLoginItem);


            }

            //跨域跳转过来第三步
            hiPayDeskLoginItem.URL = "http://9.0.9.48:7001/bpc/controller/bpc/invoice/conRuleConfig/findConRuleConfig";
            hiPayDeskLoginItem.Cookie = PayDeskLoginCookiealltheway;
            hiPayDeskLoginItem.Accept = "*/*";
            hiPayDeskLoginItem.Method = "post";
            hiPayDeskLoginItem.ContentType = "application/json;charset=UTF-8";
            hiPayDeskLoginItem.Postdata = "{\"conRuleConfigList\":[{\"paramCode\":\"ExchangeRateTolerance\",\"serialNo\":\"1\"},{\"paramCode\":\"VoucherSwitch\",\"serialNo\":\"1\"},{\"paramCode\":\"SpecialCurrency_ZeroPrecision_Config\",\"serialNo\":\"1\"},{\"paramCode\":\"CancelPaymentTiming\",\"serialNo\":\"1\"},{\"paramCode\":\"PaymentTimeControl\",\"serialNo\":\"1\"}]}";

            HhPayDeskResult = httphelper.GetHtml(hiPayDeskLoginItem);
            //跨域跳转过来第四步

            hiPayDeskLoginItem.URL =
                "http://9.0.9.48:7001/bpc/controller/bpc/common/querycase/payQueryCase/findAllQueryCase";
            hiPayDeskLoginItem.Cookie = PayDeskLoginCookiealltheway;
            hiPayDeskLoginItem.Header["Origin"] = "http://9.0.9.48:7001";
            hiPayDeskLoginItem.Postdata =
                "{\"ownedPageId\":\"bpc.Collection.PolicyCollection\",\"systemCode\":\"Prpall\",\"loginStructureId\":\"4101943202\",\"userId\":\"411123199004234524\"}";
            hiPayDeskLoginItem.ContentType = "application/json;charset=UTF-8";
            HhPayDeskResult = httphelper.GetHtml(hiPayDeskLoginItem);

            if (HhPayDeskResult.StatusCode == HttpStatusCode.OK)
            {
                islogin = true;
            }


            //测试一下系统登录状态好使不


            hiloginItem.URL =
                "http://9.0.6.69:7001/prpall/common/tb/UIProposalCrossIndexQueryInput.jsp?TaskCode=prpall.a01003002011&usercode=411123199004234524&comcode=4101943202";
            hiloginItem.Method = "get";
            hiloginItem.Cookie = LoginCookiealltheway;
            hhloginresult = httphelper.GetHtml(hiloginItem);


            return islogin;
        }
    }
}
