using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class EkiSms
{
    internal const string acc = "ekiwebdesign123";
    internal const string pwd = "qaz123wsx";
    private string templet = "http://api.twsms.com/json/sms_send.php?username={0}&password={1}&mobile={2}&message={3}";
    private string mobile = "";
    public string msg = "";
    private EkiSms(string phone) => mobile = phone;
    public static EkiSms create(string phone) => new EkiSms(phone);

    public EkiSms setMsg(string m)
    {
        msg = m;
        return this;
    }

    public Result send()
    {
        var url = string.Format(templet, acc, pwd, mobile, HttpUtility.UrlEncode(msg));

        return WebConnect.CreatGet(url).Connect<Result>();
    }

    //public Task<Result> sendAsync() => Task.Run(() => send());

    public class Result
    {
        public string code { get; set; }
        public string text { get; set; }
        public int msgid { get; set; }
    }
}
