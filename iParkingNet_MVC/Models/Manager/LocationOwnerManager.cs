using Dapper;
using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;


/// <summary>
/// LocationManager 的摘要描述
/// </summary>
public class LocationOwnerManager : BaseManager, ISerialNumGenerator
{
    public Member member;
    public DbObjList<Location> locationList = new DbObjList<Location>();

    public LocationOwnerManager(string uniqueID) : this(new Member().Also(m => m.CreatByUniqueId(uniqueID)))
    {

        //member = new Member();
        //member.CreatByUniqueId(uniqueID);


        ////locationList = CreatDbListByQuery<Location>(QueryPair.getInstance()
        ////    .addQuery("MemberId", member.Id));
        //locationList.AddRange((from loc in GetTable<Location>()
        //                       where loc.beEnable
        //                       where loc.MemberId==member.Id
        //                       select loc));
    }

    public LocationOwnerManager(Member m)
    {
        member = m;
        if (!member.beManager)
            throw new PermissionException();

        locationList.AddRange((from loc in GetTable<Location>()
                               where loc.beEnable
                               where loc.MemberId == member.Id
                               select loc));
    }
    //public void mapImg()
    //{
    //    locationList.ForEach(loc =>
    //    {
    //        loc.Info.Img = loc.Info.mapMemberImgUrl(member);
    //    });
    //}
    public List<LocImg> deleteLocationImg(LocationImgRequest imgRequest)
    {
        var loc = locationList.Where(l => l.SerNum == imgRequest.serNum).FirstOrDefault();
        if (loc == null)
            throw new ArgumentNullException();

        var locImg = (from i in GetTable<LocImg>()
                      where i.LocationId == loc.Id
                      where i.Sort == imgRequest.sort
                      select i).FirstOrDefault();
        if (locImg == null)
            throw new ArgumentNullException();

        try
        {
            locImg.deleteImgWith(member);
            locImg.Delete();

            return (from i in GetTable<LocImg>()
                    where i.LocationId == loc.Id
                    select i).toSafeList();
        }
        catch (Exception)
        {

        }
        return null;
    }

    public List<LocImg> saveLocationImg()
    {

        if (!this.formFileContain(RequestFlag.Body.Img) || !this.formDataContain(RequestFlag.Body.Info))
            throw new ArgumentNullException();

        var info = this.getPostObj<LocationImgRequest>(RequestFlag.Body.Info);

        if (!info.isValid())
            throw new InputFormatException();

        var img = this.getPostImg(RequestFlag.Body.Img);
        var loc = (from l in locationList
                   where l.SerNum == info.serNum
                   select l).FirstOrDefault();
        if (loc == null)
            throw new ArgumentNullException();

        var locImgList = (from i in GetTable<LocImg>()
                          where i.LocationId == loc.Id
                          select i).toSafeList();
        //假如要修改的圖片編號有包含 就修改該圖片 沒有 就新增並檢查已經儲存的圖片有無超過限制的數量
        if (!locImgList.Any(i => i.Sort == info.sort))
            if (locImgList.Count >= ApiConfig.MaxLocationImgNum)
                throw new OutOfNumberException();

        try
        {
            //info.saveLog("Request LocImg Info");

            //如果圖片已經存在 就更新
            if (locImgList.Any(i => i.Sort == info.sort))
            {

                var locImg = locImgList.Where(i => i.Sort == info.sort).FirstOrDefault();
                //locImg.saveLog("Update locImg");

                locImg.deleteImgWith(member);

                img.saveBitmapWith(member);

                locImg.Img = img.fullName;
                locImg.Update();
            }
            else//圖片不存在 就新增
            {
                img.saveBitmapWith(member);

                //img.saveLog("Insert LocImg");

                info.convertToDbModel().Also(l =>
                {
                    l.LocationId = loc.Id;
                    l.Img = img.fullName;
                }).Insert();
            }


            return (from l in GetTable<LocImg>()
                    where l.LocationId == loc.Id
                    select l).toSafeList();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public object getLocMulctOrder(SearchRequest request)
    {
        var timeList = request.timesToOpenList(ApiConfig.DateFormat);
        var orderTable = GetTable<EkiOrder>();
        //這裡的地點查詢必須包含已刪除地點避免有遺漏的罰金
        var locations = (from loc in GetTable<Location>()
                         where loc.MemberId == member.Id
                         //沒有指定要搜尋的地點就全部丟出
                         where request.serNum == null ? true : request.serNum.Count() < 1 ? true : request.serNum.Any(num => num == loc.SerNum)
                         select loc).toSafeList();
        //後面有去join order 所以 不用處理時間(order已經做過了)
        var molctTable = (from m in GetTable<ManagerMulct>()
                          where m.MemberId == member.Id
                          where locations.Any(loc => loc.Id == m.LocationId)
                          select m).toSafeList();

        var list = new List<object>();

        locations.ForEach(loc =>
        {
            var locOrder = (from o in orderTable
                            where o.LocationId == loc.Id
                            where o.StatusEnum == OrderStatus.CancelByManager
                            where timeList.Any(t => t.IsBetweenDate(o.ReservaTime.getStartTime()))
                            select o).toSafeList();
            var molcts = (from m in molctTable
                          where m.LocationId == loc.Id
                          select m).toSafeList();

            list.Add(new
            {
                SerNum = loc.SerNum,
                beEnable = loc.beEnable,
                Result = (from m in molcts
                          join o in locOrder on m.OrderId equals o.Id
                          select new
                          {
                              //OrderSerial = o.SerialNumber,
                              Start = o.ReservaTime.StartTime,
                              End = o.ReservaTime.EndTime,
                              CancelTime = m.Time.toString(),
                              Amt = m.Amount
                          })
            });
        });

        return list;
    }

    public object getLocIncome(SearchRequest request)//計算月份收益
    {
        var timeList = request.timesToOpenList(ApiConfig.DateFormat);

        //timeList.saveLog("GetLocIncome timeList");

        var orderTable = GetTable<EkiOrder>().Where(o => o.beEnable);
        var percent = (from p in GetTable<LvPercent>()
                       where p.ManagerLv == member.ManagerLv
                       select p).FirstOrDefault();

        var locations = (from loc in locationList
                         where request.serNum.Any(num => num == loc.SerNum)
                         select loc).toSafeList();

        var list = new List<IncomeResponse>();

        locations.ForEach(loc =>
        {
            //找出該地點 在時間範圍內已經結帳的訂單
            var locOrder = (from o in orderTable
                            where o.LocationId == loc.Id
                            //where o.StatusEnum == OrderStatus.CheckOut
                            where timeList.Any(t => t.IsBetweenDate(o.ReservaTime.getStartTime()))
                            //where timeList.Any(t => o.ReservaTime.getEndTime() <= t.End)
                            select o).toSafeList();

            //(from o in locOrder
            // select new
            // {
            //     SerNum = o.SerialNumber,
            //     StartTime = o.ReservaTime.getStartTime().toString()
            // }).Also(l =>
            // {
            //     l.saveLog($"Income orders  count->{l.Count()}");
            // });

            var calculator = IncomeCalculator.init(loc, timeList);

            list.Add(calculator.calIncomeResult(percent, locOrder).Let(result =>
            {
                var response = new IncomeResponse()
                {
                    SerNum = loc.SerNum
                };
                result.ForEach(i =>
                {
                    response.Result.Add(new IncomeResponse.IncomeResult
                    {
                        Start = i.timeSpan.Start.ToString(ApiConfig.DateTimeFormat),
                        End = i.timeSpan.End.ToString(ApiConfig.DateTimeFormat),
                        Income = i.income,
                        Claimant = i.claimant
                    });
                });

                return response;
            }));
        });

        return list;
    }

    private class IncomeCalculator
    {
        public static IncomeCalculator init(Location l, List<OpenSet> o) => new IncomeCalculator(l, o);
        private Location loc;
        private List<IncomeResult> resultList = new List<IncomeResult>();

        private IncomeCalculator(Location l, List<OpenSet> o)
        {
            loc = l;
            o.ForEach(set =>
            {
                resultList.Add(new IncomeResult().Also(r =>
                {
                    r.LocSerNum = loc.SerNum;
                    r.timeSpan = set;
                }));
            });
        }
        //需要傳入該地點的訂單
        public List<IncomeResult> calIncomeResult(LvPercent percent, List<EkiOrder> orderList)
        {
            var checkoutTable = EkiSql.ppyp.table<EkiCheckOut>();
            var multTable = EkiSql.ppyp.table<ManagerMulct>();

            orderList.ForEach(order =>
            {
                resultList.FirstOrDefault(r => r.timeSpan.IsBetweenDate(order.ReservaTime.getStartTime()))
                .notNull(result =>
                {
                    switch (order.StatusEnum)
                    {
                        case OrderStatus.BeSettle:
                        case OrderStatus.CheckOut://計算地主獲得的收益
                            var income = order.Cost.toDouble();

                            (from c in checkoutTable
                             where c.OrderId == order.Id
                             select c).FirstOrDefault()
                             .notNull(checkout =>
                             {
                                 var orderNormalCost = OrderManager.OrderCalculater.calOrderCost(checkout.Date.standarCheckOutTime(order.ReservaTime) - order.ReservaTime.getStartTime(), order).toCurrency(order);
                                 income = orderNormalCost.toDouble();

                                 //去掉車主的罰金
                                 //income -= checkout.Claimant;
                             });
                            income = (income - income * (percent.AccountPercent.toDouble() / 100d)).toCurrency(order);

                            result.income += income > 0.0d ? income : 0d;
                            break;
                        case OrderStatus.CancelByManager: //計算地主取消單獲得的罰金
                            var claimant = 0d;
                            (from m in multTable
                             where m.OrderId == order.Id
                             select m).FirstOrDefault()
                             .notNull(mult =>
                             {
                                 claimant = mult.Amount;
                             });
                            result.claimant += claimant.toCurrency(order);
                            break;
                    }
                });
            });

            return resultList;
        }
    }

    public class IncomeResult
    {
        public string LocSerNum = "";
        public OpenSet timeSpan;
        public double income = 0d;//計算收益總額
        public double claimant = 0d;//計算罰金總額
    }

    public object getLvInfo()
    {
        try
        {
            var percent = (from p in GetTable<LvPercent>()
                           where p.ManagerLv == member.ManagerLv
                           select p).FirstOrDefault();
            return new
            {
                member.ManagerLv,
                Percent = percent.AccountPercent
            };
        }
        catch (Exception)
        {

        }
        return null;
    }

    public DbObjList<Location> filterOpenByMonth(DateTime time)
    {
        var filter = new FilterOpenSiteRule(time);
        //避免修改到原來的
        return new DbObjList<Location>(locationList).Also(list =>
        {
            list.ForEach(location =>
            {
                var openSet = (from open in location.ReservaConfig.OpenSet
                               where filter.isInRule(open)
                               select open);
                location.ReservaConfig.OpenSet = new DbObjList<OpenTime>(openSet);
            });
        });
    }

    //public Location convertToLocation_v2(AddLocationRequest locationRequest) =>
    //    locationRequest.convertToModel_v2(loc =>
    //    {
    //        loc.MemberId = member.Id;
    //    });

    public LocationResponseModel addLocation_v2(AddLocationRequest locRequest)
    {
        var loc = locRequest.convertToModel_v2(l => l.MemberId = member.Id);
        loc.Insert(true);
        return loc.convertToResponse_v2();
    }

    public LocationResponseModel addLocation(AddLocationRequest locRequest)
    {
        var loc = locRequest.convertToDbModel();
        loc.MemberId = member.Id;
        loc.Insert(true);
        return loc.convertToResponse();
    }

    //public Location convertToLocation(AddLocationRequest locationRequest)
    //{
    //    var loc = locationRequest.convertToDbModel();
    //    loc.MemberId = member.Id;

    //    return loc;
    //}

    public bool addReferrer(string code)
    {
        if ((from r in GetTable<SalesReferrer>()
             where r.MemberId == member.Id
             select r).Count() > 0)
            throw new InputFormatException();

        var sales = (from s in GetTable<SalesData>()
                     where s.ReferrerCode == code
                     select s).FirstOrDefault();
        if (sales == null)
            throw new InputFormatException();
        try
        {
            var referrer = new SalesReferrer().Also(r =>
            {
                r.MemberId = member.Id;
                r.ReferrerCode = code;
                r.SalesId = sales.Id;
            });
            referrer.Insert();

            return true;
        }
        catch (Exception)
        {

        }
        return false;
    }

    public bool editBank(BankRequest request)
    {
        try
        {
            var newInfo = request.convertToDbModel();

            var info = (from i in GetTable<BankInfo>()
                        where i.MemberId == member.Id
                        select i).First();
            var address = (from a in GetTable<Address>()
                           where a.Id == info.AddressId
                           select a).First();

            address.editBy(request.address.convertToDbModel());
            info.editBy(newInfo);

            return address.Update() && info.Update();
        }
        catch (Exception)
        {
        }
        return false;
    }

    public object cancelOrder(SearchRequest request)
    {
        var now = request.time.isNullOrEmpty() ? DateTime.Now : request.time.toDateTime();

        var validOrders = new DbObjList<EkiOrder>(from num in request.serNum
                                                  join order in GetTable<EkiOrder>() on num equals order.SerialNumber
                                                  where order.isReserva(now)
                                                  where order.isCancelable(now)
                                                  where locationList.Any(loc => loc.Id == order.LocationId)
                                                  select order);

        var mulctList = new List<ManagerMulct>();

        validOrders.UpdateByObj(order =>
        {
            var mulct = new ManagerMulct()
            {
                MemberId = member.Id,//地主的memberId
                OrderId = order.Id,
                LocationId = order.LocationId,
                Amount = order.calManagerMulctAmt(now),//計算地主罰金
                Unit = order.Unit,
                Time = now
            };

            mulct.Insert(true);
            mulctList.Add(mulct);

            order.cancelByManager();
        });

        new SendManagerCancelOrderProcess(validOrders, now).run();

        return (from num in request.serNum
                select new
                {
                    SerialNumber = num,
                    Success = validOrders.Any(o => o.SerialNumber == num),
                    Order = validOrders.FirstOrDefault(o => o.SerialNumber == num)?.convertToResponse(),
                    Mulct = mulctList.FirstOrDefault(m => m.OrderId == validOrders.FirstOrDefault(o => o.SerialNumber == num)?.Id)?.convertToResponse()
                });
    }

    public object creatLocationOrder(SearchRequest request)
    {
        try
        {
            var locList = request.id.Let(ids =>
            {
                List<Location> locs;
                if (ids != null && ids?.Count() > 0)
                {
                    locs = (from id in ids
                            join loc in locationList on id equals loc.Id
                            where loc.MemberId == member.Id
                            select loc).toSafeList();
                }
                else
                {
                    locs = (from num in request.serNum
                            join loc in locationList on num equals loc.SerNum
                            where loc.MemberId == member.Id
                            select loc).toSafeList();
                }
                return locs;
            });



            var now = request.time.Let(time => time.isNullOrEmpty() ? DateTime.Now.AddDays(-ApiConfig.MinLocationOrderMonth) : time.toDateTime());
            var span = request.timeSpan.isNullOrEmpty() ? null : request.timeSpan;

            var checkOuts = GetTable<EkiCheckOut>();
            var members = GetTable<Member>();

            var data = (from loc in locList
                        join o in GetTable<EkiOrder>() on loc.Id equals o.LocationId into ORDERS
                        select new
                        {
                            loc.Id,
                            loc.SerNum,
                            Order = (from order in ORDERS
                                     where order.beEnable
                                     where span == null ? order.ReservaTime.getStartTime() >= now :
                                     span.start.toDateTime() <= order.ReservaTime.getStartTime() && order.ReservaTime.getStartTime() <= span.end.toDateTime()

                                     join orderMember in members on order.MemberId equals orderMember.Id
                                     select order.convertToResponse()
                                     .setMember(orderMember)
                                     .setRating((from r in GetTable<ManagerRating>()
                                                 where r.OrderId == order.Id
                                                 where r.MemberId == member.Id
                                                 select r).Count() < 1)
                                     .setArgue((from a in GetTable<Argue>()
                                                where a.OrderId == order.Id
                                                where a.Source.toEnum<ArgueSource>() == ArgueSource.Manager
                                                where a.MemberId == member.Id
                                                select a).Count() < 1)
                                     .Also(o => o.ReservaTime.setIsUser(order.MemberId == member.Id)))
                                     .toManagerLocOrder(order =>
                                     {
                                         if (order.Status == OrderStatus.CheckOut.toInt() || order.Status == OrderStatus.BeSettle.toInt())
                                         {
                                             var rawOrder = (from o in ORDERS
                                                             where o.SerialNumber == order.SerialNumber
                                                             select o).FirstOrDefault();

                                             var checkOut = (from c in checkOuts
                                                             where c.OrderId == rawOrder.Id
                                                             select c).FirstOrDefault();

                                             if (checkOut == null)
                                                 return "";
                                             return checkOut.mapImgUrlWith(DirPath.Order, new Member().Also(m => m.CreatById(rawOrder.MemberId)));
                                         }
                                         return "";
                                     })
                        });

            //data.saveLog("LocationOrder Data");
            return data;
        }
        catch (Exception e)
        {
            //e.saveLog("LocationOrderError");
        }
        return null;
    }
    public object deleteOpenSet(EditOpenSetRequest request,int ver=1)
    {
        var now = request.time.isNullOrEmpty() ? DateTime.Now : request.time.toDateTime();
        var location = locationList.FirstOrDefault(loc => request.id > 0 ? loc.Id == request.id : loc.SerNum == request.serNum);
        var dList = request.openSet.convertToDbObjList<OpenTime>();

        var deleteOpenList = (from set in location.ReservaConfig.OpenSet
                              where dList.Any(x => x == set)
                              select set).toDbList();
        if (deleteOpenList.Count < 1)
            throw new ArgumentException();


        //當地主要刪除開放時間時候 取消所有該開放時間裡面的訂單 並且計算罰金
        var orderList = (from order in GetTable<EkiOrder>()
                         where order.LocationId == location.Id
                         where order.beEnable
                         where order.StatusEnum == OrderStatus.Reserved
                         select order);

        var mulctList = new Dictionary<OpenTime, List<ManagerMulct>>();

        var cancelOrders = (from order in orderList
                            where deleteOpenList.Any(open => open.between(order.ReservaTime.getStartTime()))
                            select order).toDbList();

        cancelOrders.UpdateByObj(order =>
        {
            order.cancelByManager();

            var mulct = new ManagerMulct()
            {
                MemberId = member.Id,
                OrderId = order.Id,
                LocationId = order.LocationId,
                Amount = order.calManagerMulctAmt(now),//之後要計算罰金
                Unit = order.Unit,
                Time = now
            };

            mulct.Id = mulct.Insert(true);

            var open = deleteOpenList.First(o => o.between(order.ReservaTime.getStartTime()));

            if (mulctList.Any(p => p.Key == open))
            {
                mulctList.First(p => p.Key == open).Value.Add(mulct);
            }
            else
            {
                mulctList.Add(open, new List<ManagerMulct> { mulct });
            }

        });

        //要去插入 刪除的時間跟罰金的資料
        mulctList.Foreach((open, mulcts) =>
        {
            //插入刪除的時間
            var cancelOpenId = ManagerCancelTime.initFrom(open).Let(c =>
            {
                c.MemberId = member.Id;
                c.CancelTime = now;
                return c.Insert(true);
            });
            //插入Map
            mulcts.ForEach(mulct =>
            {
                new Map_CancelTime_Mulct()
                {
                    MulctId = mulct.Id,
                    CancelTimeId = cancelOpenId
                }.Insert();
            });

        });

        new SendManagerCancelOrderProcess(cancelOrders, now).run();

        if (deleteOpenList.DeleteInDb())
            return new
            {
                Location = (from loc in GetTable<Location>()
                            where loc.Id == location.Id
                            select loc).FirstOrDefault().Let(loc =>
                            {
                                switch (ver)
                                {
                                    case 2:
                                        return loc.convertToResponse_v2().convertToManagerLocation_v2();
                                    default:
                                        return loc.convertToResponse().convertToManagerLocation();
                                }
                            }),
                Mulcts = (from p in mulctList
                          select p.Value into Mulcts
                          from m in Mulcts
                          select m.convertToResponse()).toSafeList()
            };

        return null;
        //return new
        //{
        //    Location = location.convertToResponse()
        //};
    }
    public Location addOpenSet(EditOpenSetRequest request)
    {
        var location = locationList.FirstOrDefault(loc => request.id > 0 ? loc.Id == request.id : loc.SerNum == request.serNum);
        var now = request.time.isNullOrEmpty() ? DateTime.Now.toLocationTime(location) : request.time.toDateTime();
        //去除加入時間內小於現在時間的部分
        var addList = new DbObjList<OpenTime>(
            from o in request.openSet.convertToDbObjList<OpenTime>()
            where o.getStartTime().Date >= now.Date && o.getEndTime().Date >= now.Date
            select o);
        if (addList.Count < 1)
            throw new ArgumentNullException();

        var openCheck = new OpenCheck(from o in location.ReservaConfig.OpenSet
                                          //過去的開放時間 就不要了(非週期性的)
                                      where o.weekEnum != WeekEnum.NONE ? true : o.getStartTime().Date >= now.Date
                                      select o);

        if ((from o in addList
             where openCheck.anyOverLap(o)
             select o).Count() > 0)
            throw new TimeOverlapException();

        var success = addList.InsertToDb(set =>
         {
             set.ParentId = location.ReservaConfig.Id;
         });
        if (success)
            return (from loc in GetTable<Location>()
                    where loc.Id == location.Id
                    select loc).FirstOrDefault();
        return location;
    }

    public Location updateOpenSet(EditOpenSetRequest request)
    {
        try
        {
            var location = locationList.FirstOrDefault(loc => request.id > 0 ? loc.Id == request.id : loc.SerNum == request.serNum);
            //var opesList = request.openSet.convertToDbObjList<OpenTime>();
            location.ReservaConfig.OpenSet.DeleteInDb();
            location.ReservaConfig.OpenSet = request.openSet.convertToDbObjList<OpenTime>();
            location.ReservaConfig.OpenSet.InsertToDb(set =>
            {
                set.ParentId = location.ReservaConfig.Id;
            });

            return location;
        }
        catch (Exception)
        {
        }
        return null;
    }
    public void deleteLocationAsync(List<Location> locs = null)
    {
        Task.Run(() =>
        {
            deleteLocation(locs==null?locationList:locs);
        });
    }
    public void deleteLocation(List<int> ids)
    {
        deleteLocation((from loc in locationList
                        where ids.Contains(loc.Id)
                        select loc).toSafeList());
    }
    public void deleteLocation(List<Location> deleteList)
    {     
        deleteList.ForEach(loc =>
        {
            loc.Delete();
            locationList.Remove(locationList.First(data => data.Id == loc.Id));
        });
    }

    public Location updateLocation(EditLocationRequest request, int ver = 1)
    {
        request.cleanXss();
        try
        {
            return (from loc in locationList
                    where loc.Id == request.id
                    select loc).FirstOrDefault().Let(location =>
                    {
                        location.editBy(request, ver);
                        return location;
                    });
        }
        catch (Exception ex)
        {
            //ex.saveLog("edit loc v2 error ", "loc v2", request);
            throw new InputFormatException();
        }
    }
}