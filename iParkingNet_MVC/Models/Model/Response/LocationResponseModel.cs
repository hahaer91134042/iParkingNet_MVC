using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocationResponseModel 的摘要描述
/// </summary>
public class LocationResponseModel:ApiAbstractModel
{
    public int Id = 0;
    public double Lat = 0;
    public double Lng = 0;
    public AddressResponseModel Address = new AddressResponseModel();
    public LocationInfoResponseModel Info = new LocationInfoResponseModel();
    public ReservaConfigResponseModel Config = new ReservaConfigResponseModel();
    public List<LocImg> Img = new List<LocImg>();
    public List<ChargeSocket> Socket = new List<ChargeSocket>();
    public AvailableStatus Available = AvailableStatus.UnKnow;

    //現在直接loading該網頁就好
    //public string Ad { get; set; }
    public double RatingCount { get; set; }
    //debug use
    //public double Distance { get; set; }
    //public bool IsOk { get; set; }
    //public int Range { get; set; }
    public class ChargeSocket
    {
        public int Current { get; set; }
        public int Charge { get; set; }            
    }
    public class LocImg
    {
        public int Sort { get; set; }
        public string Url { get; set; }
    }
    public object convertToManagerLocation(bool deleable) => new
    {
        Id,
        Lat,
        Lng,
        Img,
        Address,
        Info,
        Config,
        RatingCount,
        //這邊計算該地點能不能進行刪除
        Deleteable = deleable
    };
    public object convertToManagerLocation_v2(bool deleable) => new
    {
        Id,
        Lat,
        Lng,
        Img,
        Address,
        Info=Info.removeObjAttr("Current","Charge"),
        Config,
        Socket,
        RatingCount,
        //這邊計算該地點能不能進行刪除
        Deleteable = deleable
    };
    public object convertToManagerLocation() => new
    {
        Id,
        Lat,
        Lng,
        Img,
        Address,
        Info,
        Config,
        //Available,
        RatingCount
    };
    public object convertToManagerLocation_v2() => new
    {
        Id,
        Lat,
        Lng,
        Img,
        Address,
        Info = Info.removeObjAttr("Current", "Charge"),
        Config,
        Socket,
        //Available,
        RatingCount
    };

    public object convertToLoadLocResponse_v2() => new
    {
        Id,
        Lat,
        Lng,
        Img,
        Address,
        Info=Info.removeObjAttr("Current","Charge"),
        Config = Config.removeObjAttr("OpenSet", "beEnable"),
        Socket,
        Available,
        RatingCount
    };
    public object convertToLoadLocResponse() =>
        new
        {
            Id,
            Lat,
            Lng,
            Img,
            Address,
            Info,
            Config = Config.removeObjAttr("OpenSet", "beEnable"),
            Available,
            RatingCount
        };
}