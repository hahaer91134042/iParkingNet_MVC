using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservaRequest 的摘要描述
/// </summary>
public class ReservaRequest : RequestAbstractModel
{
    public RequestObjList<UserReservaTime> times { get; set; }


    public override bool isValid()
    {
        var now = DateTime.Now;
        foreach(var time in times)
        {
            if (!time.isValid())
                return false;

            //預約時間超過60天以上的話
            if ((time.start.toDateTime() - now).TotalDays > ApiConfig.MaxReservaDay)
                return false;
        }
        return true;
    }

    public class UserReservaTime : RequestAbstractModel, IRequestConvert<ReservedTime>
    {
        public int loc { get; set; }
        public string serNum { get; set; }
        public int week = -1;
        //public string date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string carNum { get; set; }
        public string remark { get; set; }

        public override bool isValid()
        {
            try
            {
                cleanXss();

                //if (date.isNullOrEmpty() && !date.isDateTime(ApiConfig.DateFormat)
                //    && !TextUtil.checkTimeFormat(start) && !TextUtil.checkTimeFormat(end))
                //    return false;

                var startStamp = start.toDateTime();
                var endStamp = end.toDateTime();

                if (endStamp <= startStamp)
                    return false;
                if (string.IsNullOrEmpty(carNum))
                    return false;

                if (loc > 0)
                    return true;
                else
                    return SerialNumUtil.isLocationSerialNum(serNum);

            }
            catch (Exception)
            {

            }
            return false;
        }

        public override bool cleanXss()
        {
            try
            {
                cleanXssStr(start);
                cleanXssStr(end);
                cleanXssStr(carNum);
                cleanXssStr(remark);
            }
            catch (Exception)
            {
            }
            return false;
        }

        public ReservedTime convertToDbModel()
        {
            if (loc < 1)
                loc = new Location().Also(l => l.CreatBySerNum(serNum)).Id;

            return new ReservedTime()
            {
                LocationId = loc,
                Week = week,
                //Date = date,
                StartTime = start.toDateTime(),
                EndTime = end.toDateTime(),
                CarNum=carNum,
                Remark=remark
            };
        }
    }
}