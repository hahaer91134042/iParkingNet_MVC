using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// MemberManager 的摘要描述
/// </summary>
public class MemberManager : BaseManager
{
    public Member member;

    public MemberManager()
    {
        member = new Member();
    }
    public MemberManager(string uniqueID)
    {
        member = new Member();
        member.CreatByUniqueId(uniqueID);
    }

    public MemberManager(Member member)
    {
        this.member = member;
    }

    public MemberManager setMember(Member member)
    {
        this.member = member;
        return this;
    }

    public bool isManager()
    {
        return member.beManager;
    }

    public List<DiscountResponse> getDiscount()
    {
        try
        {
            var now = DateTime.Now;
            var list = (from d in GetTable<MemberDiscount>()
                        where d.MemberId == member.Id
                        where d.beEnable
                        where d.IsRange ? now <= d.EndTime : true
                        select d).toDbList();
            return list.convertResponseList<DiscountResponse>();
        }
        catch (Exception) { }
        return new List<DiscountResponse>();
    }

    public void cleanCreditAgree()
    {
        if (member.PayInfo == null)
            throw new ArgumentNullException();

        member.PayInfo.Delete();
    }

    public bool memberBeManager(BankInfo info)
    {
        //member.saveLog("be member");

        if (member.beManager)
            return false;

        if ((from b in GetTable<BankInfo>()
             where b.MemberId == member.Id
             select b).Count() > 0)
        {
            (from b in GetTable<BankInfo>()
             where b.MemberId == member.Id
             select b).First().Delete();
        }

        info.AddressId = member.AddressID;
        info.MemberId = member.Id;
        info.Insert();

        member.beManager = true;
        member.Update();
        return true;
    }
    public void deleteMember()
    {
        var now = DateTime.Now;
        if (member.beManager)
        {
            var locOwnerManager = new LocationOwnerManager(member);
            //1.先把地點的訂單全部取消
            (from loc in locOwnerManager.locationList
             join o in (from order in GetTable<EkiOrder>()
                        where order.beEnable
                        where order.StatusEnum == OrderStatus.Reserved
                        select order) on loc.Id equals o.LocationId into LocOrder
             select LocOrder).Foreach(orders =>
             {
                 var locOrders = orders.toDbList();

                 locOrders.UpdateByObj(order =>
                 {
                     order.cancelByManager();
                 });

                 new SendManagerCancelOrderProcess(locOrders, now).run();

             });

            //2.把開放時間刪掉(先暫時不用好了)

            //3.把地點刪除
            locOwnerManager.deleteLocation(locOwnerManager.locationList);

        }

            //4.取消所有的預約單
            (from o in EkiSql.ppyp.table<EkiOrder>()
             where o.beEnable
             where o.MemberId == member.Id
             where o.StatusEnum == OrderStatus.Reserved
             join loc in EkiSql.ppyp.table<Location>() on o.LocationId equals loc.Id
             join locManager in EkiSql.ppyp.table<Member>() on loc.MemberId equals locManager.Id
             select new
             {
                 Manager = locManager,
                 Order = o,
                 Location = loc
             }).Foreach(data =>
             {
                 var mOrder = data.Order;

                 new OrderCancel()
                 {
                     OrderId = mOrder.Id,
                     Paid = true,
                     Time = now,
                     Text = "該成員已刪除"
                 }.Insert();

                 mOrder.toCancel();
                 mOrder.Update();

                 data.Manager.sendCancelOrderMsg(mOrder, data.Location);
             });


        //5.刪除成員
        member.Delete();
    }

    public object deleteVehicleInfo(List<int> ids)
    {

        var vehicleList = CreatListByQuery<VehicleInfo>(QueryPair.New().addQuery("MemberId", member.Id)).AsQueryable();

        //var result = (from deletId in ids.AsQueryable()
        //              select new
        //              {
        //                  Id = deletId,
        //                  Success = deleteVehicleInfo((from v in vehicleList where deletId == v.Id select v).FirstOrDefault())
        //              }).toSafeList();
        ids.ForEach(deletId =>
        {
            deleteVehicleInfo((from v in vehicleList where deletId == v.Id select v).FirstOrDefault());
        });

        return loadMemberVehicle().convertResponseList<VehicleInfoResponse>();

    }

    private bool deleteVehicleInfo(VehicleInfo vehicleInfo)
    {
        if (vehicleInfo != null)
        {
            if (!string.IsNullOrEmpty(vehicleInfo.Img))
                deleteFile($"~{DirPath.Member}/{member.UniqueID.toString()}/{vehicleInfo.Img}");
            var delCmd = SqlCmd.Delete<VehicleInfo>.TableRowById(vehicleInfo.Id);

            using (var sqlHelper = new SqlContext(EkiSql.ppyp))
            {
                sqlHelper.query(delCmd);

                //假如刪除的是預設載具
                if (vehicleInfo.IsDefault)
                {
                    var vehicleList = CreatListByQuery<VehicleInfo>(QueryPair.New()
                        .addQuery("MemberId", member.Id)).AsQueryable();

                    var first = (from v in vehicleList
                                 where v.IsDefault == false
                                 orderby v.Id ascending
                                 select v).FirstOrDefault();
                    try
                    {
                        first.IsDefault = true;
                        sqlHelper.query(SqlCmd.Update<VehicleInfo>.ObjTableById(first));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return true;
        }
        return false;
    }

    public object CreditCardList()
    {
        var cards = (from card in GetTable<MemberCredit>()
                     where card.MemberId == member.Id && card.beEnable
                     select new { card.EndNum }).ToList();
        return cards;
    }

    public void AddCreditCard(MemberCreditRequest request)
    {
        var cards = (from card in GetTable<MemberCredit>()
                     where card.MemberId == member.Id
                     select card);
        if (cards.Count() >= ApiConfig.MaxCreditCard)
            throw new OutOfNumberException();

        var memberCredit = request.convertToDbModel();

        if ((from card in cards
             where card.DecodeCreditInfo().CreditInfoDecode.CardNum == memberCredit.CreditInfoDecode.CardNum
             select card).Count() > 0)
            throw new ArgumentException();

        memberCredit.MemberId = member.Id;
        memberCredit.Insert();
    }

    public object DeleteCredit(List<string> serNum)
    {
        var cards = (from card in GetTable<MemberCredit>()
                     where card.MemberId == member.Id && card.beEnable
                     select card);
        return (from num in serNum.AsQueryable()
                join card in cards on num equals card.EndNum into CARD
                select new
                {

                    EndNum = num,
                    Success = (from c in CARD
                               where c.EndNum == num
                               select c).Count() > 0 ? (from c in CARD where c.EndNum == num select c.Delete()).First() : false
                }).ToList();
    }

    public void Update(LoginRequest value)
    {
        member.Lan = string.IsNullOrEmpty(value.lan) ? "TC" : value.lan;
        member.MobileType = value.mobileType;
        member.PushToken = value.pushToken;
        //member.Ip = WebUtil.GetUserIP();

        member.Update();
    }

    public VehicleInfo creatVehicleInfo(VehicleRequest request, EkiPostImg vehicleImg = null)
    {
        var info = new VehicleInfo();
        info.Type = request.type.isNullOrEmpty() ? "" : request.type;
        info.Label = request.label.isNullOrEmpty() ? "" : request.label;
        info.Current = request.current;
        info.Charge = request.charge;
        info.Number = request.number;
        info.Name = request.name;
        info.IsDefault = request.isDefault;

        info.MemberId = member.Id;

        if (vehicleImg != null)
        {
            info.Img = vehicleImg.fullName;
            vehicleImg.saveBitmapWith(member);
            //vehicleImg.saveTo($"~{DirPath.Member}/{member.UniqueID}");
        }

        info.Id = info.Insert(true);//這邊一定要先記錄 不然mapImg之後會錯
        //改變IsDefault
        info.updateVehicleDefault();

        return info;
    }

    public bool checkPwd(string pwd)
    {
        //var hashPwd = SecurityBuilder.CreatePasswordHash(member.SecuritySalt, pwd, EncryptFormat.SHA256);
        return member.checkCipher(pwd);
    }

    public string getToken()
    {
        return JwtBuilder.GetEncoder()
                .setUser(member.UniqueID.toString())
                .setExpDate(DateTime.Now.AddDays(ApiConfig.TokenLifeDay))
                .encode();
    }

    public void editMember(MemberEditRequest request)
    {
        var allMember = GetTable<Member>();
        request.mail.notNullOrEmpty(mail =>
        {
            //檢查是不是唯一
            var existMail = allMember.Any(m => m.Mail == mail);
            if (existMail)
            {
                throw new InputFormatException();
            }
            else
            {
                member.Mail = mail;
            }
        });
        request.phone.notNullOrEmpty(phone =>
        {
            //檢查是不是唯一
            var existPhone = allMember.Any(m => m.PhoneNum == phone || m.Info.PhoneNum == phone);
            if (existPhone)
            {
                throw new InputFormatException();
            }
            else
            {
                member.PhoneNum = phone;
                member.Info.PhoneNum = phone;
            }
        });

        //request.pwd.notNullOrEmpty(pwd =>
        //{
        //    if (!TextUtil.checkPwdVaild(pwd))
        //        throw new InputFormatException();
        //    //var hashPwd = SecurityBuilder.CreatePasswordHash(member.SecuritySalt,pwd, EncryptFormat.SHA256);
        //    //member.Pwd = hashPwd;
        //    member.creatCipher(pwd);
        //});


        //member.Ip = WebUtil.GetUserIP();
        // member.beEnable = true;
        //成為地主分開成為另外的api處理
        //成為地主之後就不能再取消了
        //if(!member.beManager)
        //     member.beManager = request.beManager;

        if (request.address != null)
        {
            member.Address.Country = request.address.country;
            member.Address.City = request.address.city;
            member.Address.State = request.address.state;
            member.Address.ZipCode = request.address.zip;
            member.Address.Detail = request.address.detail;
        }

        request.info.notNullOrEmpty(info =>
        {
            member.Info.FirstName = info.firstName;
            member.Info.LastName = info.lastName;
            member.Info.CountryCode = info.countryCode;
            member.Info.NickName = info.nickName;
        });


        member.Update();
    }

    public DbObjList<VehicleInfo> loadMemberVehicle()
    {
        var vehicles = (from v in GetTable<VehicleInfo>()
                        where v.MemberId == member.Id
                        select v.mapImgPath(member.UniqueID.toString())).toSafeList();
        return new DbObjList<VehicleInfo>(vehicles);
    }

    public bool editPwdByForget(PwdDecode decode)
    {
        try
        {
            member.creatCipher(decode.pwd);
            member.Update();
            return true;
        }
        catch (Exception) { }
        return false;
    }

    public bool editPwd(EditPwdDecode decode)
    {
        try
        {
            if (member.checkCipher(decode.oldPwd))
            {
                member.creatCipher(decode.newPwd);
                member.Update();
                return true;
            }
        }
        catch (Exception) { }
        return false;
    }
}