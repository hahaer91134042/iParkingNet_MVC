using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ChargeAdapter 的摘要描述
/// </summary>
public class ChargeAdapter:List<ChargeSocket>
{
    public static ChargeAdapter AC = new ChargeAdapter(
            ChargeSocket.Tesla,
            ChargeSocket.SAEJ1772,
            ChargeSocket.IEC62196,
            ChargeSocket.GB_T_20234,
            ChargeSocket.e_Moving,
            ChargeSocket.Gogoro,
            ChargeSocket.ionex,
            ChargeSocket.Home
        );

    public static ChargeAdapter DC = new ChargeAdapter(
            ChargeSocket.Tesla,
            ChargeSocket.Type1_CCS,
            ChargeSocket.Type2_CCS,
            ChargeSocket.CHAdeMO,
            ChargeSocket.GB_T_20234
        );

    private ChargeAdapter(params ChargeSocket[] sockets)
    {
        AddRange(sockets);
    }
}