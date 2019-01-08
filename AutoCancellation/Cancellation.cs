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



namespace AutoCancellation
{
    class Cancellation
    {

        Queue dateTime = new Queue();//时间队列
        Queue alBackAllFormsCompulsory = new Queue();//主动撤回交强队列
        Queue alBackAllFormsBusiness=new Queue();//主动撤回商业队列
        Queue alCheckRecallCompulsory = new Queue();//交强回撤队列
        Queue alCheckRecallBusiness=new Queue();//商业回撤队列
        //Queue alTmp = new Queue();//主动撤回失败队列
        Queue alBackAllFormsFailure = new Queue();//主动撤回失败队列
        Queue alCheckRecallFail = new Queue();//回撤失败队列
        //Queue alFinalFail = new Queue();//最终失败队列
        string cookie = "";
        public User user { get; set; }
        public DatePass datePass { get; set; }
        public Cancellation(User user,DatePass datePass)
        {
            this.user = user;
            this.datePass = datePass;
        }

        public void StartCancel(Queue datetime,string cookie)
        {
            this.dateTime = datetime;
            this.cookie = cookie;
            foreach(DateTime dt in datetime)
            {
                CheckData(dt);
            }
            CancellationAllBackForm(alBackAllFormsBusiness, alCheckRecallBusiness, alBackAllFormsFailure);
            CancellationAllBackForm(alBackAllFormsCompulsory, alCheckRecallCompulsory, alBackAllFormsFailure);
            
            CancellationCheckRecall(alCheckRecallBusiness, alCheckRecallFail);
            //CancellationAllBackForm(alTmp, alCheckRecall, alFailure);
            CancellationCheckRecall(alCheckRecallCompulsory, alCheckRecallFail);
            // CancellationCheckRecall(alCheckRecallFail, alFinalFail);
            CancellationCheckRecall(alBackAllFormsFailure, alCheckRecallFail);

            string strDirectory = Environment.CurrentDirectory;
            string strFileParthFull = strDirectory + "\\" + user._id +" "+datePass.dateStart+"至"+datePass.DateEnd  +".txt";
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
            string ck = cookie;
            ArrayList lstCancelList = new ArrayList();

            HttpResult hrResult = new HttpResult();
            HttpHelper hhHelper = new HttpHelper();
            HttpItem hiItem = new HttpItem();
            hiItem.URL = string.Format("http://{0}:7001/prpall/common/tb/UIProposalCrossIndexQueryList.jsp", user._ipAddress);
            hiItem.ContentType = "application/x-www-form-urlencoded";
            hiItem.Method = "post";
            hiItem.Cookie = ck;
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
                    hiBackAllForms.Cookie = ck;
                    hiBackAllForms.Postdata = string.Format("RiskCode=0511&BIZTYPE=PROPOSAL&EDITTYPE=REMOVE&CheckFlag=N&PageNum={0}&PageCount={1}&WherePart={2}&VipCheckFlag=0&Personal=1&CertifyNoSign=%3D", i.ToString(), intTotalPage, strWherePart);
                    hrBackAllForms = hhBackAllForms.GetHtml(hiBackAllForms);
                    Console.WriteLine("当前获取{0}第{1}页", strdt, i.ToString());
                    GetAllBackFormList(hrBackAllForms, alBackAllFormsCompulsory, alBackAllFormsBusiness);
                    GetCheckRecall(hrBackAllForms, alCheckRecallCompulsory, alCheckRecallBusiness);
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
        private static void GetAllBackFormList(HttpResult hrResult,  Queue alBackAllFormsCompulsory,Queue alBackAllFormsBusiness)
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
        private static void GetCheckRecall(HttpResult hrResult,Queue alCheckRecallCompulsory,Queue alCheckRecallBusiness)
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
        private   void CancellationAllBackForm(Queue alBackAllForms,Queue alCheckRecall,Queue alTmp)
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
                hhItem.Cookie = cookie;
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
                    alTmp.Enqueue(o.ToString());
                    Console.WriteLine("{0}主动撤回失败！", o.ToString());

                }
            } 
            //foreach(object o in altmp )
            //{

            //    hhItem.URL = string.Format("http://218.29.135.230:7001/prpall/common/tb/UIJFeeCardBackAllNew.jsp?BizNo={0}&EditType=T&searchType=proposalCrossIndexQuery", o.ToString());
            //    hhItem.ContentType = "application/x-www-form-urlencoded";
            //    hhItem.Method = "post";
            //    hhItem.Cookie = cookie;
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


        //撤单
        private  void CancellationCheckRecall(Queue quCheck,Queue quFail)
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
                hhItem.Cookie = cookie;
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
