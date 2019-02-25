using CsharpHttpHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;


namespace AutoCancellation
{/// <summary>
/// 2019年1月30日 增加收款平台存在缴费记录情况下撤回功能
///  "通过收款平台缴费，不可做主动回撤" 
/// </summary>
    class Cancellation
    {

        Queue dateTime = new Queue();//时间队列
        Queue<string> alBackAllFormsCompulsory = new Queue<string>();//主动撤回交强队列
        Queue<string> alBackAllFormsBusiness =new Queue<string>();//主动撤回商业队列
        Queue<string> alPayCompulsory=new Queue<string>();//缴费记录交强队列
        Queue<string> alPayBusiness =new Queue<string>();//缴费记录商业队列
        Queue<string> alPayTemp= new Queue<string>();//缴费记录临时队列
        Queue<string> alCollectionNumber = new Queue<string>();//缴费号队列
        Queue<string> alCheckRecallCompulsory = new Queue<string>();//交强回撤队列
        Queue<string> alCheckRecallBusiness =new Queue<string>();//商业回撤队列
        //Queue alTmp = new Queue();//主动撤回失败队列
        Queue<string> alBackAllFormsFailure = new Queue<string>();//主动撤回失败队列
        Queue<string> alCheckRecallFail = new Queue<string>();//回撤失败队列
        //Queue alFinalFail = new Queue();//最终失败队列
        string loginCookie = "";
        private string payDeskCookie = "";
        public User user { get; set; }
        public DatePass datePass { get; set; }
        public Cancellation(User user,DatePass datePass)
        {
            this.user = user;
            this.datePass = datePass;
        }

        public void StartCancel(Queue datetime,string loginCookie,string payDeskCookie)
        {
            this.dateTime = datetime;
            this.loginCookie = loginCookie;
            this.payDeskCookie = payDeskCookie;
            foreach(DateTime dt in datetime)
            {
                CheckData(dt);
            }
            //主动撤回
            CancellationAllBackForm(alBackAllFormsBusiness, alCheckRecallBusiness,alPayBusiness, alBackAllFormsFailure);
            CancellationAllBackForm(alBackAllFormsCompulsory, alCheckRecallCompulsory, alPayCompulsory,alBackAllFormsFailure);
            //查询缴费记录
            GetPaymentRecords(alPayCompulsory, alCollectionNumber);
            GetPaymentRecords(alPayBusiness,alCollectionNumber);
            //去掉重复缴费记录数
            List<string> listCollectionList = alCollectionNumber.ToList();
            HashSet<string> hs = new HashSet<string>(listCollectionList);
            alCollectionNumber.Clear();
            hs.ToList().ForEach(item => alCollectionNumber.Enqueue(item));
            //撤消缴费记录
            WithdrawPaymentRecords(alCollectionNumber);

            //主动撤回撤消缴费记录后的数据
            CancellationAllBackForm(alPayBusiness, alCheckRecallBusiness, alPayTemp, alBackAllFormsFailure);
            CancellationAllBackForm(alPayCompulsory, alCheckRecallCompulsory, alPayTemp, alBackAllFormsFailure);

            //撤单
            CancellationCheckRecall(alCheckRecallBusiness, alCheckRecallFail);
            //CancellationAllBackForm(alTmp, alCheckRecall, alFailure);
            CancellationCheckRecall(alCheckRecallCompulsory, alCheckRecallFail);
            // CancellationCheckRecall(alCheckRecallFail, alFinalFail);
            CancellationCheckRecall(alBackAllFormsFailure, alCheckRecallFail);

            string strDirectory = Environment.CurrentDirectory;
            string strFileParthFull = strDirectory + "\\" + user._id +" "+datePass.DateStart+"至"+datePass.DateEnd  +".txt";
            FileStream fs = new FileStream(strFileParthFull, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string strFail in alBackAllFormsFailure)
            {
                sw.WriteLine(strFail);
            }
            foreach (string strFail in alCheckRecallFail)
            {
                sw.WriteLine(strFail);
            }
            sw.Flush();
            sw.Close();
            fs.Close();

        }
        #region 获取具体日期主动回撤和撤回的清单
        public void CheckData(DateTime date)
        {
            DateTime dt = date;
            string strdt = dt.ToString("yyyy-MM-dd");
            
            
            ArrayList lstCancelList = new ArrayList();

            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hiItem = new HttpItem();

            //获取登录权限
            hiItem.URL =
                string.Format(
                    "http://9.0.6.69:7001/prpall/system/checkusergrade/UserGradePowerCheck.jsp?UserCode={0}&ComCode={1}&TaskCode=prpall.a01003002011",
                    user._id, user._comcode);
            hiItem.Referer = "http://9.0.6.69:7001/prpall/processUtiMenu.do?actionType=showMenu&taskCode=showMenu&menuStyle=null";
            hiItem.ContentType = "text/xml";
            hiItem.Cookie = loginCookie;
            hrResult = hhHelper.GetHtml(hiItem);
            if (hrResult.Html.Contains("true"))
            {
                //增加一些验证措施
                //hiItem.URL =
                //    "http://9.0.6.69:7001/prpall/common/tb/UIProposalCrossIndexQueryInput.jsp?TaskCode=prpall.a01003002011&usercode=411123199004234524&comcode=4101943202";
                //hiItem.Method = "get";
                //hiItem.Cookie = loginCookie;
                //hrResult = hhHelper.GetHtml(hiItem);



                hiItem.URL = string.Format("http://{0}:7001/prpall/common/tb/UIProposalCrossIndexQueryList.jsp", user._ipAddress);
                hiItem.ContentType = "application/x-www-form-urlencoded";
                hiItem.Cookie = loginCookie;
                
                //hiItem.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)";
               // hiItem.Header["Accept-Encoding"] = "gzip, deflate";
               // hiItem.Header["Accept-Language"] = "zh-Hans-CN,zh-Hans;q=0.5";
               // hiItem.Referer = String.Format("http://9.0.6.69:7001/prpall/common/tb/UIProposalCrossIndexQueryInput.jsp?TaskCode=prpall.a01003002011&usercode={0}&comcode={1}", user._id, user._comcode);
               // hiItem.Accept ="image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*";
                hiItem.Method = "post";
                Encoding ecode = System.Text.Encoding.GetEncoding("GB2312");
                //hiItem.Postdata = string.Format("InputDate={0}&RiskCode=0511&OthFlag4=0&VipCheckFlag=0", dt.ToString("yyyy-MM-dd"));
                hiItem.Postdata = string.Format("ProposalNoSign=%3D&ProposalNo=&QuickLicenseNoSign=%3D&QuickLicenseNo=&EngineNoSign=%3D&EngineNo=&ContractNoSign=%3D&ContractNo=&LicenseNoSign=%3D&LicenseNo=&FrameNoSign=%3D&FrameNo=&BrandNameSign=%3D&BrandName=&ClassCodeSign=%3D&ClassCode=&VINNoSign=%3D&VINNo=&LicenseColorCodeSign=%3D&LicenseColorCode=&InsuredNameSign=%3D&InsuredName=&AppliNameSign=%3D&AppliName=&InsuredCodeSign=%3D&InsuredCode=&AppliCodeSign=%3D&AppliCode=&StartDateSign=%3D&StartDate=&EndDateSign=%3D&EndDate=&CurrencySign=%3D&Currency=&SumAmountSign=%3D&SumAmount=&SumPremiumSign=%3D&SumPremium=&MakeComSign=%3D&MakeCom=&HandlerCodeSign=%3D&HandlerCode=&OperatorCodeSign=%3D&OperatorCode=&OperateDateSign=%3D&OperateDate=&InputDateSign=%3D&InputDate={0}&BusinessID1Sign=%3D&BusinessID1=&UnderwriteEndDateSign=%3D&UnderwriteEndDate=&RiskCodeSign=%3D&RiskCode=0511&OthFlag4=0&VipCheckFlag=0", strdt);
                //string postdata = string.Format("InputDate={0}&RiskCode=0511&OthFlag4=0&VipCheckFlag=0",dt.ToString("yyyy-MM-dd"));
                //hiItem.Postdata = System.Web.HttpUtility.UrlEncode(postdata,ecode);
                hrResult = hhHelper.GetHtml(hiItem);
                //第 2 页 / 共 7 页
                string regexPage = @"第\s*(\w+)\s*页\s*/\s*共\s*(\w+)\s*页";
                Match mc = Regex.Match(hrResult.Html.ToString(), regexPage);
                string strNowPage = mc.Groups[1].Value;
                string strTotalPage = mc.Groups[2].Value;
                int intTotalPage = int.Parse(strTotalPage);
                string regexWherePart = @"name.*=.*WherePart.*""(.*)""";
                Match mcWherepart = Regex.Match(hrResult.Html.ToString(), regexWherePart);
                string strWhere = mcWherepart.Groups[1].Value;
                string strWherePart = System.Web.HttpUtility.UrlEncode(strWhere, System.Text.Encoding.GetEncoding("GB2312"));
                Console.WriteLine("{0}共有{1}页", strdt, strTotalPage);


                if (strNowPage != "0")
                {
                    // GetAllBackFormList(hrResult, alBackAllForms);
                    for (int i = 1; i <= intTotalPage; i++)
                    {
                        HttpResult hrBackAllForms = new HttpResult();
                        HttpHelper hhBackAllForms = new HttpHelper();
                        HttpItem hiBackAllForms = new HttpItem();
                        hiBackAllForms.URL = string.Format("http://{0}:7001/prpall/common/tb/UIProposalCrossIndexQueryList.jsp", user._ipAddress);
                        hiBackAllForms.ContentType = "application/x-www-form-urlencoded";
                        hiBackAllForms.Method = "post";
                        hiBackAllForms.Cookie = loginCookie;
                        hiBackAllForms.Postdata = string.Format("RiskCode=0511&BIZTYPE=PROPOSAL&EDITTYPE=REMOVE&CheckFlag=N&PageNum={0}&PageCount={1}&WherePart={2}&VipCheckFlag=0&Personal=1&CertifyNoSign=%3D", i.ToString(), intTotalPage, strWherePart);
                        hrBackAllForms = hhBackAllForms.GetHtml(hiBackAllForms);
                        Console.WriteLine("当前获取{0}第{1}页", strdt, i.ToString());
                        GetAllBackFormList(hrBackAllForms, alBackAllFormsCompulsory, alBackAllFormsBusiness);
                        GetCheckRecall(hrBackAllForms, alCheckRecallCompulsory, alCheckRecallBusiness);
                    }



                }

            }



            

            // return lstCancelList;
        }
        #endregion


        /// <summary>
        /// 获取主动回撤清单并放入交强,商业的队列
        /// </summary>
        /// <param name="hrResult"></param>
        /// <param name="alBackAllForms"></param>
        private static void GetAllBackFormList(HttpResult hrResult,  Queue<string> alBackAllFormsCompulsory,Queue<string> alBackAllFormsBusiness)
        {
            //主动回撤 backAllForm('905072018410172002548')
            string regexBackAllForm = @"backAllForm\S*\(\S*(\d{21})\S*\)";
            MatchCollection mcBackAllForms = Regex.Matches(hrResult.Html.ToString(), regexBackAllForm);
            //Match mcBackAllForm = Regex.Match(hrResult.Html.ToString(), regexBackAllForm);

            foreach (Match mctmp in mcBackAllForms)
            {
                string strtmp = mctmp.Groups[1].Value;
                if (strtmp.Substring(0, 5) == "90507")
                {
                    alBackAllFormsCompulsory.Enqueue((strtmp));
                }
                else
                {
                    alBackAllFormsBusiness.Enqueue((strtmp));
                }
                
            }
        }

        /// <summary>
        /// 获取撤回清单并放入交强,商业的队列
        /// </summary>
        /// <param name="hrResult"></param>
        /// <param name="alCheckRecall"></param>
        private static void GetCheckRecall(HttpResult hrResult,Queue<string> alCheckRecallCompulsory,Queue<string> alCheckRecallBusiness)
        {
            //撤单列表
            string regexCheckRecall = @"checkRecall.*BizNo=(\d{21})'";
            MatchCollection mcCheckRecall = Regex.Matches(hrResult.Html.ToString(), regexCheckRecall);
            foreach(Match mctmp in mcCheckRecall)
            {
                string strtmp = mctmp.Groups[1].Value;
                if (strtmp.Substring(0, 5) == "90507")
                {
                    alCheckRecallCompulsory.Enqueue(strtmp);
                }
                else
                {
                    alCheckRecallBusiness.Enqueue(strtmp);
                }
                   
                
            }
        }


        //主动回撤
        private   void CancellationAllBackForm(Queue<string> alBackAllForms,Queue<string> alCheckRecall,Queue<string> alPaymentQueue,Queue<string> alTmp)
        {
            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hhItem = new HttpItem();
            //ArrayList altmp = new ArrayList();
            while (alBackAllForms.Count > 0)
                
            {
                object o = alBackAllForms.Dequeue();
                hhItem.URL = string.Format("http://{1}:7001/prpall/common/tb/UIJFeeCardBackAllNew.jsp?BizNo={0}&EditType=T&searchType=proposalCrossIndexQuery", o.ToString(), user._ipAddress);
                hhItem.ContentType = "application/x-www-form-urlencoded";
                hhItem.Method = "post";
                hhItem.Cookie = loginCookie;
                hhItem.Postdata = "RiskCode=0511&BIZTYPE=PROPOSAL&EDITTYPE=REMOVE&CheckFlag=N&VipCheckFlag=0&Personal=1&CertifyNoSign=%3D";
                hrResult = hhHelper.GetHtml(hhItem);
                if (hrResult.Html.Contains("success.gif"))
                {
                    //alBackAllForms.Remove(o.ToString());
                    alCheckRecall.Enqueue(o.ToString());


                    Console.WriteLine("{0}主动撤回成功！", o.ToString());
                }
                else
                {
                    if (hrResult.Html.Contains("正在通过收款平台缴费"))
                    {
                        alPaymentQueue.Enqueue(o.ToString());
                    }
                    else
                    {
                        alTmp.Enqueue(o.ToString());
                    }
                    
                    Console.WriteLine("{0}主动撤回失败！", o.ToString());

                }
            } 
            //foreach(object o in altmp )
            //{

            //    hhItem.URL = string.Format("http://218.29.135.230:7001/prpall/common/tb/UIJFeeCardBackAllNew.jsp?BizNo={0}&EditType=T&searchType=proposalCrossIndexQuery", o.ToString());
            //    hhItem.ContentType = "application/x-www-form-urlencoded";
            //    hhItem.Method = "post";
            //    hhItem.Cookie = loginCookie;
            //    hhItem.Postdata = "RiskCode=0511&BIZTYPE=PROPOSAL&EDITTYPE=REMOVE&CheckFlag=N&VipCheckFlag=0&Personal=1&CertifyNoSign=%3D";
            //    hrResult = hhHelper.GetHtml(hhItem);
            //    if (hrResult.Html.Contains("success.gif"))
            //    {
            //        Console.WriteLine("{0}主动撤回成功！", o.ToString());
            //    }
            //    else
            //    {
                    
            //        Console.WriteLine("{0}主动撤回失败！", o.ToString());

            //    }
            //}
            
        }

        //查询缴费记录
        private void GetPaymentRecords(Queue<string> quOriginQueue, Queue<string> quCollectionQueue)
        {
            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hhItem = new HttpItem();
           // List<string> list = quOriginQueue.ToList();

            while (quOriginQueue.Count>0)
            {
                object o= quOriginQueue.Dequeue();
                JsonGetCollectionNumber jsonPostData= new JsonGetCollectionNumber();
                jsonPostData.collBusinessTypeCode = "1";
                jsonPostData.certiNo = o.ToString();
                jsonPostData.certiNoTypeCode = "T";
                jsonPostData.capitalverificationType = "1";
                jsonPostData.systemCode = "Prpall";
                jsonPostData.loginComcode = user._comcode;
                jsonPostData.operatorCode = user._id;
                hhItem.URL =
                    "http://9.0.9.48:7001/bpc/controller/bpc/collection/colltransact/payCollTransactMain/collPaymentInfoQuery?from=0&limit=10";
                hhItem.Cookie = payDeskCookie;
                hhItem.ContentType=  "application/json";
                hhItem.Method = "post";
                hhItem.Header.Add("Origin", "http://9.0.9.11");
                hhItem.Postdata = JsonConvert.SerializeObject(jsonPostData);
                hrResult = hhHelper.GetHtml(hhItem);
                JsonPostBackCollectionDatas jsonPostBack = new JsonPostBackCollectionDatas();
               jsonPostBack= JsonConvert.DeserializeObject<JsonPostBackCollectionDatas>(hrResult.Html);
               if (jsonPostBack.PostBackCollectionNumbers.Count > 0)
               {
                   foreach (var collectionNumber in jsonPostBack.PostBackCollectionNumbers)
                   {
                       quCollectionQueue.Enqueue(collectionNumber.collectionNumber);
                       Console.WriteLine("{0}获取缴费记录成功", collectionNumber.collectionNumber);
                    }

                   
               }

            }
            //list.ForEach(item=>quOriginQueue.Enqueue(item));
            

        }

        //取消缴费记录
        private void WithdrawPaymentRecords(Queue<string> quOriginQueue)
        {
            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hhItem = new HttpItem();
            while ( quOriginQueue.Count>0)
            {
                string o = quOriginQueue.Dequeue();
                JsonWithdrawData jsonPostData = new JsonWithdrawData();
                jsonPostData.collectionNumber = o;
                jsonPostData.exceptionContext = System.Web.HttpUtility.UrlEncode("自动撤消缴费记录", System.Text.Encoding.UTF8)
                    ;
                jsonPostData.loginComcode = user._comcode;
                jsonPostData.operatorCode = user._id;
                jsonPostData.operatorName = System.Web.HttpUtility.UrlEncode(user._userName, System.Text.Encoding.UTF8);
                jsonPostData.systemCode = "Prpall";
                hhItem.URL =
                    "http://9.0.9.48:7001/bpc/controller/bpc/collection/colltransact/payCollTransactMain/cancelPayAndRegist";
                hhItem.Cookie = payDeskCookie;
                hhItem.ContentType = "application/json";
                hhItem.Method = "post";
                hhItem.Header.Add("Origin", "http://9.0.9.11");
                hhItem.Postdata = JsonConvert.SerializeObject(jsonPostData);
                hrResult = hhHelper.GetHtml(hhItem);
                JsonPostBackWithdrawData jsonPostBack= new JsonPostBackWithdrawData();
                jsonPostBack = JsonConvert.DeserializeObject<JsonPostBackWithdrawData>(hrResult.Html);
                if (jsonPostBack.returnFlag== "SUCCESS")
                {
                    Console.WriteLine("{0}撤消缴费记录成功",jsonPostBack.businessInformation);
                }
                else
                {
                    Console.WriteLine("{0}撤消缴费记录失败", jsonPostBack.businessInformation);
                }


            }

           

        }

        //撤单
        private  void CancellationCheckRecall(Queue<string> quCheck,Queue<string> quFail)
        {
            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hhItem = new HttpItem();
            while (quCheck.Count > 0)
            {
                object o = quCheck.Dequeue();
                hhItem.URL = string.Format("http://{1}:7001/prpall/commonship/tbcb/UIPrpslPoliWithdrawSubmit.jsp?BIZTYPE=PROPOSAL&BizNo={0}&reason=AutoCancellation", o.ToString(), user._ipAddress);
                hhItem.ContentType = "application/x-www-form-urlencoded";
                hhItem.Method = "post";
                hhItem.Cookie = loginCookie;
                hhItem.Postdata = "RiskCode=0511&BIZTYPE=PROPOSAL&EDITTYPE=REMOVE&CheckFlag=N&VipCheckFlag=0&Personal=1&CertifyNoSign=%3D";
                hrResult = hhHelper.GetHtml(hhItem);
                if (hrResult.Html.Contains("下列投保单撤单成功"))
                {
                    //alCheckRecall.Remove(o.ToString());
                    Console.WriteLine("{0}撤单成功！", o.ToString());
                }
                else
                {
                    quFail.Enqueue(o.ToString());
                    Console.WriteLine("{0}撤单失败！", o.ToString());
                }
            } 
        }
    }
}
