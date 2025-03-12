using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LoadLocationRequest 的摘要描述
/// </summary>
public class LoadLocationRequest:RequestAbstractModel
{
    public double lat { get; set; }
    public double lng { get; set; }
    public string nowTime { get; set; }
    public AddressRequest address { get; set; }
    public LoadConfigRequest config { get; set; }

    public override bool isEmpty()
    {
        if(address!=null)
            address.cleanXss();
        if(config!=null)
            config.cleanXss();
        //因為經緯度有可能是負值
        //return (lat <= 0 || lng <= 0) && address==null;
        return address==null;
    }
    public override bool isValid()
    {
        if (config != null)
        {
            if (config.searchTime != null)
                if(config.searchTime.date!=null)
                    if (config.searchTime.date.Count > 0)
                        try
                        {
                            var failDate = (from d in config.searchTime.date
                                            where !d.isDateTime(ApiConfig.DateFormat)
                                            select d).Count() > 0;


                            DateTime end = config.searchTime.end.toDateTime(ApiConfig.TimeFormat);
                            DateTime start= config.searchTime.start.toDateTime(ApiConfig.TimeFormat);
                            return end > start && !failDate;
                        }
                        catch (Exception)
                        {
                            return false;
                        }                        
        }       

        return true;
    }
    //&& address.isEmpty()
}