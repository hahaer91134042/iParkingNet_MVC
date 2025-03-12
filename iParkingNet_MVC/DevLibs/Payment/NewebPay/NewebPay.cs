using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPay 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public static class NewebPay
    {
        private static INewebPayConfig config_mpg { get; set; }
        private static INewebPayConfig config_invoice { get; set; }
        private static INewebPayConfig config_credit { get; set; }
        public static void LoadMPG(INewebPayConfig c) => config_mpg = c;
        public static void LoadInvoice(INewebPayConfig c) => config_invoice = c;
        public static void LoadCreditCard(INewebPayConfig c) => config_credit = c;
        public static NewebPayMPG MPG()
        {
            checkMpg();
            return new NewebPayMPG(config_mpg);
        }
        public static NewebPayInvoice Invo()
        {
            checkInvoice();
            return new NewebPayInvoice(config_invoice);
        }
        public static NewebPayCreditCard CreditCard()
        {
            checkCredit();
            return new NewebPayCreditCard(config_credit);
        }


        public static bool Parse(this NewebPayMPGReturn model)
        {
            checkMpg();
            try
            {
                var checkSha = config_mpg.getHashSha256(model.TradeInfo);
                if (checkSha != model.TradeSha)
                    return false;

                var jsonStr = config_mpg.DecryptAES256(model.TradeInfo);
                //ResultStr = jsonStr;
                //Log.print($"Neweb from->{from} return data->{jsonStr}");
                model.MPG = jsonStr.toObj<NewebPayMPGReturn.Result_MPG>();
                return true;
            }
            catch (Exception e)
            {
                Log.e("NewebPayMPGReturn parse error", e);
            }
            return false;
        }

        public static void checkMpg()
        {
            if (config_mpg == null)
                throw new ArgumentNullException("Need init MPG config First");
        }
        public static void checkInvoice()
        {
            if (config_invoice == null)
                throw new ArgumentNullException("Need init Invoice config First");
        }
        public static void checkCredit()
        {
            if (config_credit == null)
                throw new ArgumentNullException("Need init Credit Card config First");
        }

    }
}
