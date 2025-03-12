using DevLibs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayCreditReturn 的摘要描述
/// </summary>
public class NewebPayCreditReturn
{
    public string Status { get; set; }
    public string Message { get; set; }
    public CreditResult Result { get; set; }


    public class CreditResult
    {
        public string MerchantID { get; set; }
        public int Amt { get; set; }
        public string TokenLife { get; set; }
        public string MerchantOrderNo { get; set; }
        public string TradeNo { get; set; }
        public string CheckCode { get; set; }
        public string RespondCode { get; set; }
        public string Auth { get; set; }
        public string AuthDate { get; set; }
        public string AuthTime { get; set; }
        public string Card6No { get; set; }
        public string Card4No { get; set; }
        public string IP { get; set; }
        public string EscrowBank { get; set; }
        public string ECI { get; set; }
        public string Exp { get; set; }
        public string AuthBank { get; set; }
        public string PaymentMethod { get; set; }

        public DateTime authTime() 
        {
            var aTime = DateTime.ParseExact(
                $"{AuthDate}{AuthTime}",
                "yyyyMMddHHmmss", 
                CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            //Log.print($"AuthTime->{aTime.toString()}");
            return aTime;
        }
    }
}