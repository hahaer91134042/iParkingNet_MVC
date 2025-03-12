using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// Location 的摘要描述
/// </summary>
[DbTableSet("Location")]
public class Location : BaseDbDAO,
                                         IConvertResponse<LocationResponseModel>,
                                         IPriceSet<decimal>,
                                         ILinkId,IEdit<EditLocationRequest>,
                                         ApiFeature_v2.IConvertResponse<LocationResponseModel>
{
    [DbRowKey("MemberId",false)]
    public int MemberId { get; set; }
    [DbRowKey("Lat",DbAction.Update)]
    public double Lat { get; set; }
    [DbRowKey("Lng",DbAction.Update)]
    public double Lng { get; set; }
    [DbRowKey("AddressId",false)]
    public int AddressId { get; set; }
    [DbRowKey("InfoId",false)]
    public int InfoId { get; set; }
    [DbRowKey("ReservaConfigId",false)]
    public int ReservaConfigId { get; set; }
    [DbRowKey("SerNum", false)]
    public string SerNum { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime sDate { get; set; }
    [DbRowKey("eDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime eDate { get; set; }
    [DbRowKey("Ip", DbAction.Update, true)]
    public string Ip { get; set; }

    /// <summary>
    /// 表示該地點是否刪除
    /// </summary>
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;//初始預設值

    public Member Member
    {
        get
        {
            if (member == null)
                member = new Member().Also(m => m.CreatById(MemberId));
            return member;
        }
    }

    public Address Address
    {
        get 
        {
            if (addr == null)
                addr = new Address().Also(a => a.CreatById(AddressId));
            return addr; 
        }
        set { addr = value; }
    }
    public LocationInfo Info
    {
        get 
        {
            if (info == null)
                info = new LocationInfo().Also(i => i.CreatById(InfoId));
            return info; 
        }
        set { info = value; }
    }
    public ReservaConfig ReservaConfig
    {
        get 
        {
            if (config == null)
                config = new ReservaConfig().Also(c => c.CreatById(ReservaConfigId));
            return config; 
        }
        set { config = value; }
    }
    public List<LocImg> LocImg {
        get {
            if (imgList == null)
                imgList = (from i in EkiSql.ppyp.table<LocImg>()
                           where i.LocationId == Id
                           select i).toSafeList();
            return imgList;
        }
    }
    public DbObjList<LocSocket> SocketList
    {
        get
        {
            if (socketList == null)
                socketList = new DbObjList<LocSocket>((from s in EkiSql.ppyp.table<LocSocket>()
                                                       where s.LocationId == Id
                                                       select s).toSafeList());
            return socketList;
        }
        set { socketList = value; }
    }

    public OCPP_CP Cp
    {
        get
        {
            if (cp == null)
                cp = (from c in EkiSql.ppyp.table<OCPP_CP>()
                      where c.beEnable
                      where c.LocSerial==SerNum                       
                      select c).FirstOrDefault();
            return cp;
        }
        set { cp = value; }
    }

    public AvailableStatus Available = AvailableStatus.UnKnow;
    private Member member;
    private Address addr ;
    private LocationInfo info ;
    private ReservaConfig config ;
    private List<LocImg> imgList = null;
    private DbObjList<LocSocket> socketList = null;
    private DbObjList<LocSocket> editSocketList = new DbObjList<LocSocket>();
    private OCPP_CP cp;


    //public MemberInfo mapIconUrlInfo()
    //{
    //    if (!string.IsNullOrEmpty(Info.IconImg))
    //        Info.IconImg = $"{WebUtil.getWebURL()}{DirPath.Member}/{UniqueID}/{Info.IconImg}";
    //    return Info;
    //}

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }
    public override bool CreatByUniqueId(string uniqueId)
    {
        return EkiSql.ppyp.loadDataByQueryPair(
            QueryPair.New()
            .addQuery("UniqueID", uniqueId)
            .addQuery("beEnable", 1),
            this);
    }
    public bool CreatBySerNum(string serNum)
    {
        return EkiSql.ppyp.loadDataByQueryPair(
            QueryPair.New()
            .addQuery("SerNum", serNum)
            .addQuery("beEnable",1),
            this);
    }

    public override int Insert(bool isReturnId = false)
    {
        //addr.Id = addr.Insert(true);
        //info.Id = info.Insert(true);
        //reserva.Id = reserva.Insert(true);
        AddressId = Address.Insert(true);//這樣會導致insert完成之後再重新loading一次            
        InfoId = Info.Insert(true);
        ReservaConfigId = ReservaConfig.Insert(true);
        Id = EkiSql.ppyp.insert(this, true);
        SocketList.InsertByObj(s => s.LocationId = Id);
        return Id;
    }
    public override bool Update()
    {
        if (Info.Update()&&ReservaConfig.Update())
        {
            if (SocketList.DeleteInDb())
            {
                if (editSocketList.Count>0)
                {
                    SocketList.AddRange(editSocketList);
                    SocketList.InsertByObj(s => s.LocationId = Id);
                }
            }
            return EkiSql.ppyp.update(this);
        }
        return false;
    }

    public override bool Delete()
    {
        //這邊 Address 不用刪除 因為訂單也要用 刪除會錯誤
        //if ( info.Delete() && config.Delete())
        //{            
        //    return DeleteData(this);
        //}
        beEnable = false;
        sDate = DateTime.Now;

        var member = new Member().Also(m => m.CreatById(MemberId));
        //只要刪除圖片好了
        LocImg.ForEach(i =>
        {
            i.deleteImgWith(member);
            i.Delete();
        });

        return Update();
    }


    public LocationResponseModel convertToResponse() => toLocResponse(1);
    public LocationResponseModel convertToResponse_v2() => toLocResponse(2);

    private LocationResponseModel toLocResponse(int ver)
    {
        var member = new Member().Also(m => m.CreatById(MemberId));

        var response = new LocationResponseModel()
        {
            Id = Id,
            Lat = Lat,
            Lng = Lng
        };
        response.Address.load(Address);
        response.Info.load(Info);
        response.Config.load(ReservaConfig);
        response.Available = Available;
        response.RatingCount = this.getRatingCount();
        LocImg.ForEach(l =>
        {
            response.Img.Add(new LocationResponseModel.LocImg
            {
                Sort = l.Sort,
                Url = l.mapMemberImgUrl(member)
            });
        });

        //var hasAD = Sql.count("select count(*) from LocationAD " +
        //           "where LocationId=@locID and DATEDIFF(DAY, StartTime, CONVERT(datetime, GETDATE())) >= 0 and DATEDIFF(DAY, CONVERT(datetime, GETDATE()), EndTime) >= 0",
        //           new { locID = Id }) > 0;
        //response.Ad = hasAD ? ApiConfig.UrlMap.mapAd(SerNum) : "";

        switch (ver)
        {            
            case 2://v2
                SocketList.ForEach(socket =>
                {
                    response.Socket.Add(new LocationResponseModel.ChargeSocket
                    {
                        Current=socket.Current,
                        Charge=socket.Charge
                    });
                });
                return response;

            default://v1
                //v2兼容v1的作法 暫時的
                if (SocketList.Count() > 0)
                {
                    var socket = SocketList.First();
                    response.Info.Charge = socket.Charge;
                    response.Info.Current = socket.Current;
                }
                return response;
        }
    }

    public decimal price() => ReservaConfig.Price;
    public PriceMethodSet methodSet() => ReservaConfig.PriceMethod;
    public CurrencyUnit currencyUnit() => ReservaConfig.Currency;

    public int linkId() => Id;

    public void editBy(EditLocationRequest data,int ver=1)
    {
        Info.editBy(data.info,ver);
        ReservaConfig.editBy(data.config);

        switch (ver)
        {
            case 2:
                editSocketList.AddRange(data.socket.convertToDbObjList<LocSocket>());
                break;
        }
    }


}