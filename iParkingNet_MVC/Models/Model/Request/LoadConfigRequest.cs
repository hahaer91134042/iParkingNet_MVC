using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LoadConfigRequest 的摘要描述
/// </summary>
public class LoadConfigRequest : RequestAbstractModel
{
    public int range { get; set; }
    public string unit { get; set; }//M KM
    public int page = 1;
    public List<int> charges { get; set; }
    public SearchTime searchTime { get; set; }

    public DistanceUnit unitEnum { get { return unit.toEnum<DistanceUnit>(); } }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(unit);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public class SearchTime : IRequestConvert<List<ReservedTime>>
    {
        //formate yyyy-MM-dd
        public List<string> date { get; set; }
        //formate hh:mm:ss 24h
        public string start { get; set; }
        public string end { get; set; }

        public List<ReservedTime> convertToDbModel()
        {
            //this.saveLog("Search time convertToDb");

            return new List<ReservedTime>().Also(list =>
            {
                if (date != null)
                    date.ForEach(d =>
                    {
                        try
                        {
                            list.Add(new ReservedTime()
                            {
                                //Date = d,
                                StartTime = $"{d} {start}".toDateTime(),
                                EndTime = $"{d} {end}".toDateTime()
                            });
                        }
                        catch (Exception e)
                        {
                            this.saveLog("data error",e);
                        }
                    });
            });
        }
    }

}