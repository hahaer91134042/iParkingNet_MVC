using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ChargeSocketRequest 的摘要描述
/// 這是專給v2使用的紀錄解析充電插頭array
/// </summary>
public class ChargeSocketRequest:RequestObjList<ChargeSocketRequest.Socket>,ApiFeature_v2.Request
{

    public bool isValid_v2()
    {
        if (this.isNotEmpty())
        {
            foreach (var s in this)
            {
                switch (s.current.toEnum<CurrentUnit>())
                {
                    case CurrentUnit.AC:
                        if (!ChargeAdapter.AC.Any(cs => cs == s.charge.toEnum<ChargeSocket>()))
                            return false;
                        break;
                    case CurrentUnit.DC:
                        if (!ChargeAdapter.DC.Any(cs => cs == s.charge.toEnum<ChargeSocket>()))
                            return false;
                        break;
                    default:
                        return false;//不接受其他的
                }
            }
        }
        return true;
    }

    public class Socket : RequestAbstractModel, IRequestConvert<LocSocket>
    {
        public int current { get; set; }
        public int charge { get; set; }

        public LocSocket convertToDbModel() => new LocSocket
        {
            Current = current,
            Charge = charge
        };
    }
}