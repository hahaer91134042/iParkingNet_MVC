using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Eki_OCPP;
using Eki_NewebPay;
using System.Net;
using DevLibs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace iParkingNet_MVC
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.d("---Api Init---");


            //server 設定套用的 ssl
            ServicePointManager.SecurityProtocol =
            SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
            SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;


            NewebPay.LoadMPG(PayConfig.藍新.config());
            NewebPay.LoadInvoice(PayConfig.Invoice.config());
            NewebPay.LoadCreditCard(PayConfig.CreditCard.config());

            var socket = BroadcastSocket.Connect("ws://0.0.0.0:3000");//要指定router內部的  "ws://192.168.11.208:3000"
            //var socket = BroadcastSocket.Connect("ws://10.0.0.4:3000");//要指定router內部的
            //azura 10.0.0.4
            //eki demo  192.168.11.208:3000

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromStream(
                    new MemoryStream(Properties.Resources.FcmApiFile))
            });

            //var ocppSocket = OCPPsocket.Connect("ws://0.0.0.0:3001");
            EkiOCPP.start();
            Firestore_iParkingNet.start();
        }
    }
}
