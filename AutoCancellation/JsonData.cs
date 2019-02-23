using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace AutoCancellation
{
    
    public class JsonGetCollectionNumber
    {
        public string collBusinessTypeCode { get; set; }
        public string certiNo { get; set; }
        public string certiNoTypeCode { get; set; }
        public string capitalverificationType { get; set; }
        public string systemCode { get; set; }
        public string loginComcode { get; set; }
        public string operatorCode { get; set; }
    }


    public class JsonPostBackCollectionNumber
    {
	public string createDate { get; set; }
    public string updateDate { get; set; }
    public string objectId { get; set; }
    public string prpallObjectId { get; set; }
    public string certiTypeCode { get; set; }
    public string certiNo { get; set; }
    public string policyNo { get; set; }
    public string serialNo { get; set; }
    public string payRefReason { get; set; }
    public string payRefTimes { get; set; }
    public string proposalNo { get; set; }
    public string classCode { get; set; }
    public string productCode { get; set; }
    public string appliCode { get; set; }
    public string appliName { get; set; }
    public string insuredCode { get; set; }
    public string insuredName { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public string validDate { get; set; }
    public string payNo { get; set; }
    public string totalPayNo { get; set; }
    public string recCurrency { get; set; }
    public string recAndStaExchRate { get; set; }
    public string planFee { get; set; }
    public string staPlanFee { get; set; }
    public string planFeeIncludingTax { get; set; }
    public string staPlanFeeIncludingTax { get; set; }
    public string endPayDate { get; set; }
    public string comCode { get; set; }
    public string makeCom { get; set; }
    public string branchCode { get; set; }
    public string centerCode { get; set; }
    public string agentCode { get; set; }
    public string agentName { get; set; }
    public string prpallOperatorCode { get; set; }
    public string prpallOperatorName { get; set; }
    public string mainSalesManCode { get; set; }
    public string mainSalesManName { get; set; }
    public string handlerCode { get; set; }
    public string underWriteDate { get; set; }
    public string inCoinsTypeCode { get; set; }
    public string operatorCode { get; set; }
    public string operateComCode { get; set; }
    public string payRefCurrency { get; set; }
    public string payRefStatus { get; set; }
    public string payAndRecExchRate { get; set; }
    public string payRefFee { get; set; }
    public string staPayRefFee { get; set; }
    public string payAndStaExchRate { get; set; }
    public string businessNatureCode { get; set; }
    public string channelTypeCode { get; set; }
    public string premiumSourceCode { get; set; }
    public string locationFlag { get; set; }
    public string seeFeeFlag { get; set; }
    public string accBookTypeCode { get; set; }
    public string accBookCode { get; set; }
    public string combineProductFlag { get; set; }
    public string writeOffStatus { get; set; }
    public string signDate { get; set; }
    public string collectionNumber { get; set; }
    public string commRate { get; set; }
    public string crossBorderPayRefFlag { get; set; }
    public string agentCertificateNo { get; set; }
    public string commTaxFlag { get; set; }
    public string carRelaCertiNo { get; set; }
    public string carModelCode { get; set; }
    public string frameNo { get; set; }
    public string licenseNo { get; set; }
    public string receiveTime { get; set; }
    public string policySort { get; set; }
    public string immediateFlag { get; set; }
    public string systemSource { get; set; }
    public string collStatus { get; set; }
    public string demandTime { get; set; }
    public string gdAndHkCarFlag { get; set; }
    public string susTransFlag { get; set; }
    public string linkedBusinessFlag { get; set; }
    public string vatrate { get; set; }
}

public class JsonPostBackCollectionDatas
    {
        
        [JsonProperty(PropertyName = "Data$")]
    public List<JsonPostBackCollectionNumber> PostBackCollectionNumbers { get; set; }
    [JsonProperty(PropertyName = "entityCount$")]
public string total  { get; set; }
[JsonProperty(PropertyName = "from")]
public string startNum { get; set; }
    [JsonProperty(PropertyName = "limit")]
public string limit { get; set; }
}

public class JsonWithdrawData
{
    public string collectionNumber { get; set; }
    public string exceptionContext { get; set; }
    public string systemCode { get; set; }
    public string loginComcode { get; set; }
    public string operatorCode { get; set; }
    public string operatorName { get; set; }
}
public class PromptMessage
{
}

public class PromptInfo
{
}

public class PromptListInfo
{
}

public class JsonPostBackWithdrawData
{
    public string businessInformation { get; set; }
    public string returnFlag { get; set; }
    public PromptMessage promptMessage { get; set; }
    public List<PromptInfo> promptInfo { get; set; }
    public PromptListInfo promptListInfo { get; set; }
}

}
