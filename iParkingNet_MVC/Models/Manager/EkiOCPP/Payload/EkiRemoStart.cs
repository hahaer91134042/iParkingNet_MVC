using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;
using Eki_OCPP;

/// <summary>
/// EkiRemoStart 的摘要描述
/// </summary>
public class EkiRemoStart:RemoteStartTransactionCall
{
    public EkiRemoStart() : base(EkiOCPP.Config.EkiAdminIdTag)
    {

    }

}