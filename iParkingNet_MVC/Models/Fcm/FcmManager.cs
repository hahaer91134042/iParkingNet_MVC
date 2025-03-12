using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.IO;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using DevLibs;

/// <summary>
/// FcmManager 需要使用FcmSharp
/// </summary>
public class FcmManager : IDisposable
{
    public static FcmManager New()
    {
        return new FcmManager();
    }

    private FirebaseMessaging client;
    private CancellationTokenSource cts = new CancellationTokenSource();
    FcmManager()
    {



        client = FirebaseMessaging.DefaultInstance;

        //var settings = StreamBasedFcmClientSettings.CreateFromStream(FcmConfig.project_id, new MemoryStream(iParkingNet_MVC.Properties.Resources.FcmApiFile));
        //var settings = FileBasedFcmClientSettings.CreateFromFile(HttpContext.Current.Server.MapPath(FcmConfig.jsonFilePath));
        //client = new FcmClient(settings);
    }

    public BatchResponse SendMultiClient(List<IFcmSet> fcmset, IFcmMsg broadCastMsg)
    {
        var result = client.SendAllAsync(
            from set in fcmset
            select GetFcmMsg(set.fcmToken(), set.device(), broadCastMsg),
            cts.Token).GetAwaiter().GetResult();

        return result;
    }
    public FcmResponse SendTo(IFcmSet set, IFcmMsg broadCastMsg)
    {
        return SendTo(set.fcmToken(), set.device(), broadCastMsg);
    }

    public FcmResponse SendTo(string token, MobilType type, IFcmMsg broadCastMsg)
    {

        try
        {
            Message message = GetFcmMsg(token, type, broadCastMsg);

            // Send the Message and wait synchronously:
            var result = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();
            //Log.d($"FcmManager Send result->{result} obj->{FcmResponse.Creat(result).toJsonString()}");
            return FcmResponse.Creat(result);
        }
        catch (Exception e)
        {
            Log.e("FcmManager Send Error", e);
        }
        return null;
    }

    private Message GetFcmMsg(string token, MobilType type, IFcmMsg broadCastMsg)
    {
        switch (type)
        {
            case MobilType.iOS:
                return iosFcmMsg_CustomData(token, broadCastMsg);
            case MobilType.Android:
                return androidFcmMsg(token, broadCastMsg);
            default:
                return webFcmMsg(token, broadCastMsg);
        }
    }

    private Message iosFcmMsg_CustomData(string token, IFcmMsg broadCastMsg)
    {
        ApsAlert alert = null;
        if (broadCastMsg.hasInterface<IiosPushNotify>())
            alert = iosPushAlert(broadCastMsg as IiosPushNotify);

        return new Message
        {
            Token = token,
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    MutableContent = true,
                    Alert = alert
                },
                CustomData = iosData(broadCastMsg)
            }
        };
    }
    private Message androidFcmMsg(string token, IFcmMsg broadCastMsg)
    {
        return new Message
        {
            Token = token,
            Android = new AndroidConfig
            {
                Data = androidData(broadCastMsg)
            }
        };
    }
    private Message webFcmMsg(string token, IFcmMsg broadCastMsg)
    {
        return new Message
        {
            Token = token,
            Webpush = new WebpushConfig
            {
                Data = androidData(broadCastMsg)
            }
        };
    }

    private ApsAlert iosPushAlert(IiosPushNotify iosPush)
    {
        return new ApsAlert()
        {
            Body = iosPush.body(),
            Title = iosPush.title()
        };
    }

    //private class FcmMsg : FcmMessage
    //{
    //    internal FcmMsg(Message msg)
    //    {
    //        ValidateOnly = false;
    //        Message = msg;
    //    }
    //}

    public class FcmResponse
    {
        public static FcmResponse Creat(string result) => new FcmResponse(result);

        //projects/myproject-b5ae1/messages/0:1500415314455276%31bd1c9631bd1c96
        public string msgId;
        public string projectId;
        private FcmResponse(string mid)
        {
            msgId = mid;
            var arr = mid.Split('/');
            projectId = arr[1];
        }


    }

    private Dictionary<string, object> iosData(IFcmMsg broadCastMsg) => new Dictionary<string, object>() {
            {FcmConfig.methodKey,broadCastMsg.fcmMethod() },
            {FcmConfig.contentKey,broadCastMsg.toJsonString() }
        };

    private Dictionary<string, string> androidData(IFcmMsg broadCastMsg) => new Dictionary<string, string>{
            { FcmConfig.methodKey, broadCastMsg.fcmMethod()},
            { FcmConfig.contentKey, broadCastMsg.toJsonString()}
        };

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }
}