using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// BaseManager 的摘要描述
/// </summary>
public abstract class BaseManager : IDisposable,IFormDataControl
{
    //protected bool SocketContainUser(string UniqueID)
    //{        
    //    return BroadcastSocket.Instance.ContainUser(UniqueID);
    //}
    //protected bool SendBroadCastTo(string UniqueID,string msg)
    //{
    //    var broadcast = BroadcastSocket.Instance;
    //    if (broadcast != null)
    //        if (SocketContainUser(UniqueID))
    //            return broadcast.SendTo(UniqueID, msg);
    //    return false;
    //}
    protected void SendBroadCastAll(string msg)
    {
        var broadcast = BroadcastSocket.Instance;
        if (broadcast != null)
            broadcast.SendAll(msg);
    }


    protected bool isValidSerialNum(string serialNum)
    {
        return SerialNumUtil.isLocationSerialNum(serialNum);  
    }
    //protected string generateOrderTimeSerNum()
    //{
    //    using (var sqlHelper = new SqlHelper())
    //    {
    //        do
    //        {
    //            var now = DateTime.Now;
    //            var serNum = SerialNumUtil.OrderTimeSerialNum(now);
    //            var cmd = SqlCmd.Get<EkiOrder>.RowCountCmd(QueryPair.getInstance().addQuery("SerialNumber", serNum));
    //            var count = TableParaser.CountNum(sqlHelper.query(cmd));
    //            if (count < 1)
    //                return serNum;
    //        } while (true);            
    //    }        
    //}

    //目前棄用
    //protected string generateOrderSerNum()
    //{
    //    using (var sqlHelper=new SqlHelper())
    //    {
    //        do
    //        {
    //            var serNum = SerialNumUtil.OrderSerialNum();
    //            var cmd = SqlCmd.Get<EkiOrder>.RowCountCmd(QueryPair.getInstance().addQuery("SerialNumber", serNum));
    //            var count = TableParaser.CountNum(sqlHelper.query(cmd));
    //            if (count < 1)
    //                return serNum;
    //        } while (true);
    //    }
    //}

    //protected string generateLocSerialNum()
    //{
    //    using (var sqlHelper=new SqlHelper())
    //    {
    //        do
    //        {
    //            var serNum = SerialNumUtil.LocationSerialNum();
    //            var cmd = SqlCmd.Get<Location>.RowCountCmd(QueryPair.getInstance().addQuery("SerNum", serNum));
    //            var count = TableParaser.CountNum(sqlHelper.query(cmd));
    //            if (count < 1)
    //                return serNum;
    //        } while (true);
    //    }
    //}

    protected string mapImg(Member member,IMapImg input)
    {
        return mapImg(member, input.imgName());
    }
    protected string mapImg(Member member ,string fileName)
    {
        return mapImg(member.UniqueID.toString(), fileName);
    }
    protected string mapImg(string uniqueId, string fileName)
    {
        return $"{WebUtil.getWebURL()}{DirPath.Member}/{uniqueId}/{fileName}";
    }

    protected string serverPath(string path)
    {
        return HttpContext.Current.Server.MapPath(path);
    }

    protected void deleteFile(string path)
    {
        new FileInfo(serverPath(path)).Delete();
    }

    protected void creatDir(String path)
    {
        if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
    }

    protected IEnumerable<M> GetTable<M>()where M : BaseDbDAO
    {
        return EkiSql.ppyp.table<M>();
    }


    protected List<M> CreatListByQuery<M>(QueryPair pair) where M : BaseDbDAO
    {
        return EkiSql.ppyp.tableByPair<M>(pair).toSafeList();
    }
    protected DbObjList<M> CreatDbListByQuery<M>(QueryPair pair) where M : BaseDbDAO
    {
        return new DbObjList<M>(CreatListByQuery<M>(pair));
    }


    //public virtual bool isManagerMember()
    //{
    //    return false;
    //}

    public virtual void Dispose()
    {
    }


}