using DevLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

[RoutePrefix("api/SMS")]
public class SMSController : BaseApiController
{
    [HttpGet]
    [Route("TestToken")]
    public object TestToken()
    {
        var secret = SecurityBuilder.CreateSaltKey();
        var token = SecurityBuilder.CreateEkiSecretHash(secret);

        return new
        {
            Secret=secret,
            SecretLength=secret.Length,
            Token=token,
            TokenLength=token.Length
        };
    }

    
    [HttpPost]
    [Route("Confirm")]
    [EkiSecretFilter]
    public object Confirm(SendSmsRequest value)
    {
        try
        {
            if (value.isEmpty() || !value.isValid())
                throw new InputFormatException();


            //var member = (from m in GetTable<Member>()
            //              where m.PhoneNum == value.phone
            //              select m).FirstOrDefault();

            IPhoneMap phoneMap;

            if (value.isForget)
            {
                var member = (from m in GetTable<Member>()
                              where m.PhoneNum == value.phone
                              select m).FirstOrDefault();

                if (member == null)
                    throw new AccountNotExistException();
                if (!member.beEnable)
                    throw new AccountException();
                phoneMap = member;
            }
            else
            {
                if (EkiSql.ppyp.count<Member>(new QueryPair().addQuery("PhoneNum", value.phone)) > 0)
                    throw new PhoneExistException();
                phoneMap = value;
            }



            var temp = ResUtil.GetApiRes(value.Lan, "SmsConfirmMsg");

            var random = new Random().Next(0000, 9999).ToString().PadLeft(4, '0');

            var msg = string.Format(temp, random);

            var mobile = phoneMap.getSmsCode();
            //var mobile = $"{value.countryCode}{Convert.ToInt32(value.phone)}";

            var result = EkiSms.create(mobile).setMsg(msg).send();

            var log = SmsLog.creat(phoneMap);
            log.Lan = value.lan;
            log.Code = result.code;
            log.Text = result.text;
            log.Msgid = result.msgid;
            log.Descript = "Confirm";
            log.Ip = WebUtil.GetUserIP();
            log.Insert();


            if (result.code.Equals(SmsStatusCode.Success.value))
            {
                return new SmsResponse(true)
                {
                    checkCode = random
                };
            }
            else if (result.code.Equals(SmsStatusCode.PhoneError.value))
            {
                return ResponseError(EkiErrorCode.E015);
            }
            else
            {
                return ResponseError();
            }

            //return new
            //{
            //    code = random,
            //    Temp = temp,
            //    Phone = mobile,
            //    Msg = msg,
            //    Result = result
            //};
        }
        catch (PhoneExistException) { return ResponseError(EkiErrorCode.E009); }
        catch (AccountException)
        {
            return ResponseError(EkiErrorCode.E024);
        }
        catch (AccountNotExistException)
        {
            return ResponseError(EkiErrorCode.E005);
        }
        catch (InputFormatException)
        {
            //return value.isEmpty();
            return ResponseError(EkiErrorCode.E006);
        }
        catch (Exception e)
        {
            saveUnknowError(e, value);
            //return e;
        }
        return ResponseError();
    }

    public class SmsResponse : ResponseAbstractModel
    {
        public string checkCode;

        public SmsResponse(bool successful) : base(successful)
        {
        }
    }

    /*WebRequest request = WebRequest.Create("http://jsonplaceholder.typicode.com/posts");
        request.Method = "GET";
        //使用 GetResponse 方法將 request 送出，如果不是用 using 包覆，請記得手動 close WebResponse 物件，避免連線持續被佔用而無法送出新的 request
        using (var httpResponse = (HttpWebResponse)request.GetResponse())
        //使用 GetResponseStream 方法從 server 回應中取得資料，stream 必需被關閉
        //使用 stream.close 就可以直接關閉 WebResponse 及 stream，但同時使用 using 或是關閉兩者並不會造成錯誤，養成習慣遇到其他情境時就比較不會出錯
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            return new { Result = result };
        } */


}
