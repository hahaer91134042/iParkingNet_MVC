using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocImg 的摘要描述
/// </summary>
[DbTableSet("LocationImg")]
public class LocImg : BaseDbDAO,IMapImg
{
    [DbRowKey("LocationId",false)]
    public int LocationId { get; set; }
    [DbRowKey("Sort",DbAction.Update,false)]
    public int Sort { get; set; }
    [DbRowKey("Img",DbAction.Update,false)]
    public string Img { get; set; }



    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }
    public string imgName() => Img;

}