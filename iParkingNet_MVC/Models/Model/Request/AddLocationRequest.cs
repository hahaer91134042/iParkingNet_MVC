using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AddLocationRequest 的摘要描述
/// </summary>
public class AddLocationRequest:RequestAbstractModel,
                                                               IRequestConvert<Location>,
                                                               ISerialNumGenerator,
                                                               ApiFeature_v2.Request,
                                                               ApiFeature_v2.IRequestConvert<Location>
{

    public double lat { get; set; } = -1;
    public double lng { get; set; } = -1;

    public AddressRequest address { get; set; }
    public LocationInfoRequest info { get; set; }
    public ReservaConfigRequest config { get; set; }
    //v2版api才有使用
    public ChargeSocketRequest socket { get; set; } = new ChargeSocketRequest();

    //這是專給v2使用的紀錄解析充電插頭array
    //public class Socket : RequestAbstractModel,IRequestConvert<LocSocket>
    //{
    //    public int current { get; set; }
    //    public int charge { get; set; }


    //    public LocSocket convertToDbModel() => new LocSocket
    //    {
    //        Current = current,
    //        Charge = charge
    //    };
    //}

    public override bool cleanXss()
    {
        return address.cleanXss() && info.cleanXss() && config.cleanXss();
    }

    public Location convertToDbModel()
    {
        var loc = toLoc(1);
        return loc;
    }



    public Location convertToModel_v2(Action<Location> back = null)
    {

        var loc = toLoc(2);
        //var sockets = socket.convertToDbObjList<LocSocket>();
        loc.SocketList = socket.convertToDbObjList<LocSocket>();

        if (back!=null)
            back(loc);

        return loc;
    }

    private Location toLoc(int version)
    {
        cleanXss();
        var location = new Location();

        if (lat > 0 && lng > 0)
        {
            location.Lat = lat;
            location.Lng = lng;
        }
        else
        {
            if (address == null || address.isEmpty())
                throw new ArgumentNullException();


            var latlng = address.convertToLatLng();

            location.Lat = latlng.Lat;
            location.Lng = latlng.Lng;
        }
        location.SerNum = this.generateLocSerialNum();

        location.Address = address.convertToDbModel();

        switch (version)
        {
            case 1:
                location.Info = info.convertToDbModel();
                break;
            case 2:
                location.Info = info.convertToModel_v2();
                break;
        }
        location.Info.SerialNumber = location.SerNum;
        location.ReservaConfig = config.convertToDbModel();

        location.Ip = WebUtil.GetUserIP();
        return location;
    }

    public override bool isValid()
    {
        if (lat <= 0 && lng <= 0 && address == null)
            return false;

        return !address.isEmpty() && info.isValid() && config.isValid();
    }

    public bool isValid_v2()
    {
        if (lat <= 0 && lng <= 0 && address == null)
            return false;

        //if (socket.isNotEmpty())
        //{
        //    foreach (var s in socket)
        //    {
        //        switch (s.current.toEnum<CurrentUnit>())
        //        {
        //            case CurrentUnit.AC:
        //                if (!ChargeAdapter.AC.Any(cs => cs == s.charge.toEnum<ChargeSocket>()))
        //                    return false;
        //                break;
        //            case CurrentUnit.DC:
        //                if (!ChargeAdapter.DC.Any(cs => cs == s.charge.toEnum<ChargeSocket>()))
        //                    return false;
        //                break;
        //            default:
        //                return false;//不接受其他的
        //        }
        //    }
        //}

        return !address.isEmpty() && info.isValid_v2() && config.isValid() && socket.isValid_v2();
    }
}