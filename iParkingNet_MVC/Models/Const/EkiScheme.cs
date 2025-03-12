using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiScheme 的摘要描述
/// </summary>
public abstract class EkiScheme:IScheme
{
    public static CheckOut.Finish CheckoutFinish = new CheckOut.Finish();
    public static CheckOut.Back CheckoutBack = new CheckOut.Back();

    //public EkiScheme(string v)
    //{
    //    Uri = new Uri($"eki://iparkingnet.ekiweb.com/action?action={v}");
    //}

    

    public override string host() => "ppyp.app";
    public override string scheme() => "eki";

    public static IScheme.QueryPair addPair(string k, string v) => new IScheme.QueryPair(k, v);

    public abstract class CheckOut : EkiScheme
    {
        public override string path() => "checkout";

        public class Finish : CheckOut
        {           

            public override string action() => "finish";

            public Uri Uri(string serial,double amt,string num,string tokenLife="")
            {
                var qList = new List<QueryPair>() {
                    addPair("serial",serial),
                    addPair("amt",amt.ToString()),
                    addPair("card4",num),
                    addPair("time",DateTime.Now.toStamp().ToString())
                };
                if (!tokenLife.isNullOrEmpty())
                    qList.Add(addPair("token_life", tokenLife));

                return Uri(qList.ToArray());
            }

        }
        public class Back : CheckOut
        {
            public override string action() => "back";
        }
    }

   

}