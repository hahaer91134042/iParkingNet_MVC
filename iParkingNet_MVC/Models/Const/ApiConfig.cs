using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ApiConfig 的摘要描述
/// </summary>
public class ApiConfig
{
    //Jwt Token Life Day
    public const int TokenLifeDay = 365;
    //生成序列碼驗算用
    public const string SerialKey = "EKI";
    public const string JwtSecret = "!qaz2WSX#edc";
    //最大可記錄的載具數量
    public const int MaxVehicleNum= 5;
    //papaya google api key
    public const string GoogleApiKey = "AIzaSyDzawQDDnl2psVF-v0L4w7Cn8wdH3x0hsA";

    //papaya Fcm message api key
    //public const string FcmSendUrl = "https://fcm.googleapis.com/fcm/send";
    //public const string FcmMessageApiKey = "AAAAHA1JIx8:APA91bGNbFehiYJtEDihl_CiIlult9ijOhivt45twNu8NE_PaXbIZGspjduINWHb0fo6l20cbSilvDuQIofBBYFGp5PJGN1jiSrcCzks-u9QOnmKTApHH5NWIYe91racAGoRbRelKolb";
    //public const string FcmMessageSenderId = "120481981215";

    //&sensor=false&language=zh-TW
    public const string GoogleAddressUrl = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}";
    public const int DataPageNum = 100;//先測試用 以後要改回來
    //public const int DataPageNum = 40;

    public const string DateFormat = "yyyy-MM-dd";
    public const string TimeFormat = "HH:mm:ss";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const int CountMinGap = 10;//超過10分鐘計費一次
    public const string ParkingOrderSerHead = "PAY";
    public const string EkiOrderSerHead = "EKI";
    public const int OrderExistDays = 180;//180天內建立的預約單
    public const int OrderCancelMin = 30;//分鐘 以上才能取消預約
    public const int OrderCancelFreeMin = 60;//分鐘以上 才能免費取消預約
    public const int OrderCancelLimit = 20;//一天內最多取消訂單比數
    public const int MaxReservaDay = 60;//最多預約幾天後
    public const int MaxCreditCard = 3;//最多紀錄3張卡
    public const decimal OrderClaimantRate = 3m;//違約金費率
    public const string SocketAuthKey = "User";
    public const int BillingMinOffsetMinute = 15;//每次計算的時間區間
    public const int MinBillingMinute = 30;//最低計算的停車時間
    public const int FreeCheckoutMinute = 10;//checkout 免計算時間
    public const int MinLocationOrderMonth = 1;//地主找尋車位訂單最小距離月份
    public const int MaxRatingNum = 100;

    public const int MaxLocationImgNum = 3;//最多紀錄幾張該地點的圖片

    public const string Url_iParkingNet = "http://iparkingnet.eki.com.tw"; 
    public const string Url_PpypApi = "https://api.ppyp.app";
    public const string Url_Ppyp = "https://www.ppyp.app";
    //測試
    public const string Firestore_Root = "eki_iparkingnet";
    //正式
    //public const string Firestore_Root = "eki_ppyp";

    public static class UrlMap
    {

        /// <summary>
        /// 現在使用url直接loading就好
        /// </summary>
        /// <param name="serNum"></param>
        /// <returns></returns>
        [Obsolete]
        public static string mapAd(string serNum)
        {
            //測試
            return Url_Ppyp + "/AD/Test/" + serNum;
            //正式
            //return PpypUrl + "/AD/" + serNum;
        }
    }
   
    public class MaxSearch
    {
        public const int Range = 2;
        public const DistanceUnit Unit = DistanceUnit.KM;
    }    

    public class ManagerMulctSet
    {
        public static List<IMulctRule> CancelRules = new List<IMulctRule>().Also(l =>
        {
            l.Add(new ManagerMulctRule_TwoWeek());
            l.Add(new ManagerMulctRule_OneWeek());
            l.Add(new ManagerMulctRule_ThreeDay());
            l.Add(new ManagerMulctRule_OneDay());
            l.Add(new ManagerMulctRule_In24Hour());
        });
    }
    public class Discount
    {
        public const string Secret = "PPYP";
        public const int CodeFullLength = 8;
        public const int CodeLength = 7;

        public const int MaxValidDay = 365;
    }
}