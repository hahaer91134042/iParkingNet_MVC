using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;
using DevLibs;

[RoutePrefix("api/Member")]
public class MemberController : BaseApiController
{
    [HttpPost]
    [Route("AddCreditTest")]
    public object AddCreditTest(MemberCreditRequest request)
    {
        try
        {
            //if (!request.isValid())
            //    throw new ArgumentException();

            request.isValid();

            return new
            {
                Request = request
            };
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
        }
        return ResponseError();
    }

    #region CleanCreditAgree
    [HttpPost]
    [Route("CleanCreditAgree")]
    [JwtAuthActionFilter]
    public object CleanCreditAgree()
    {
        try
        {
            var memberManager = new MemberManager(getAuthObj().user);

            memberManager.cleanCreditAgree();

            return new MemberResponse(true);

        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E003);
        }
        catch (Exception)
        {
        }
        return ResponseError();
    }
    #endregion

    #region MemberDiscount
    [HttpPost]
    [Route("Discount")]
    [JwtAuthActionFilter]
    public object Discount()
    {
        try
        {
            var member = getAuthObj().getMember();
            var manager = new MemberManager(member);

            return new MemberResponse(true)
            {
                info = manager.getDiscount()
            };
        }
        catch (Exception) { }
        return ResponseError();
    }
    #endregion

    #region Delete
    [HttpPost]
    [Route("Delete")]
    [JwtAuthActionFilter]
    public object Delete()
    {
        try
        {
            var auth = getAuthObj();
            var memberManager = new MemberManager(auth.user);

            memberManager.deleteMember();

            return new MemberResponse(true);
        }
        catch(Exception e)
        {
            e.saveLog("member delete error");
        }
        return ResponseError();
    }

    #endregion

    #region BeManager
    [HttpPost]
    [Route("BeManager")]
    [JwtAuthActionFilter]
    public object BeManager(BankRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var auth = getAuthObj();
            var memberManager = new MemberManager(auth.user);

            var isSuccess = memberManager.memberBeManager(request.convertToDbModel());
            //isSuccess.saveLog("member be manager success->");
            if (isSuccess)
            {
                //更新地主的地址
                if (!request.address.isEmpty())
                    request.address.convertToDbModel().Also(address =>
                    {
                        address.Id = memberManager.member.Address.Id;
                        address.Update();
                    });

                return new MemberResponse(true)
                {
                    info = (from info in GetTable<BankInfo>()
                            where info.MemberId==memberManager.member.Id
                            select info).First().convertToResponse()
                };
            }
            else
            {
                return new MemberResponse(false).setErrorCode(EkiErrorCode.E022);
            }            
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch(Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion
    #region ---Credit---
    [HttpPost]
    [Route("Credit")]
    [JwtAuthActionFilter]
    public object Credit()
    {
        try
        {
            var auth = getAuthObj();
            var memberManager = new MemberManager(auth.user);

            return new MemberResponse(true)
            {
                info = memberManager.CreditCardList()
            };
        }        
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion
    #region ---AddCredit---
    [HttpPost]
    [Route("AddCredit")]
    [JwtAuthActionFilter]
    public object AddCredit(MemberCreditRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();
            //以後這邊要加入驗證過程
            var auth = getAuthObj();
            var memberManager = new MemberManager(auth.user);
            memberManager.AddCreditCard(request);

            return new MemberResponse(true)
            {
                info=memberManager.CreditCardList()
            };
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (OutOfNumberException)
        {
            return ResponseError(EkiErrorCode.E016);
        }
        catch (Exception e)
        {
            saveUnknowError(e,request.toJsonString());
        }
        return ResponseError();
    }
    #endregion
    #region ---DeleteCredit---
    [HttpPost]
    [Route("DeleteCredit")]
    [JwtAuthActionFilter]
    public object DeleteCredit(SearchRequest request)
    {
        try
        {
            if (request.isSerNumEmpty())
                throw new ArgumentNullException();

            var memberManager = new MemberManager(getAuthObj().user);

            return new MemberResponse(true)
            {
                info = memberManager.DeleteCredit(request.serNum)
            };
        }
        catch (ArgumentNullException e)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
        }
        return ResponseError();
    }
    #endregion
    #region ---DeleteVehicle---
    [HttpPost]
    [Route("DeleteVehicle")]
    [JwtAuthActionFilter]
    public object DeleteVehicle(DeleteRequest request)
    {
        try
        {
            var auth = getAuthObj();

            if (request.isIdEmpty())
                throw new ArgumentNullException();

            var memberManager = new MemberManager(auth.user);
            var result = memberManager.deleteVehicleInfo(request.id);

            //var cmd = SqlCmd.Get<VehicleInfo>.DataObjByIdCmd(request.id);
            //var vehicleList = TableParaser.ConvertToListByRowName<VehicleInfo>(sqlHelper.query(cmd));
            ////刪除圖片
            //vehicleList.ForEach(info =>
            //{
            //    deleteFile($"~{DirPath.Member}/{auth.user}/{info.Img}");
            //});
            ////整批刪除用
            //var delCmd = SqlCmd.Delete<VehicleInfo>.TableRowById(request.id);
            //sqlHelper.query(delCmd);

            return new MemberResponse(true)
            {
                info=result
            };
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---UpdateVehicle---
    [HttpPost]
    [Route("UpdateVehicle")]
    [JwtAuthActionFilter]
    public object UpdateVehicle()
    {
        try
        {
            var auth = getAuthObj();


            //var httpRequest = HttpContext.Current.Request;

            if (!this.formDataContain(RequestFlag.Body.Info))
                throw new ArgumentNullException();

            var vehicleRequest = this.getPostObj<VehicleRequest>(RequestFlag.Body.Info);
            vehicleRequest.cleanXss();

            if (vehicleRequest.isEmpty() || vehicleRequest.id<=0)
                throw new ArgumentNullException();


            var vehicleInfo = EkiSql.ppyp.dataById<VehicleInfo>(vehicleRequest.id);
            vehicleInfo.UpdateValue(vehicleRequest);


            if (this.formFileContain(RequestFlag.Body.Img))
            {
                using (var vehicleImg = this.getPostImg(RequestFlag.Body.Img))
                {
                    if (!string.IsNullOrEmpty(vehicleInfo.Img))
                    {
                        deleteFile($"~{DirPath.Member}/{auth.user}/{vehicleInfo.Img}");
                    }
                    vehicleInfo.Img = vehicleImg.fullName;
                    vehicleImg.saveBitmap($"~{DirPath.Member}/{auth.user}".toServerPath());
                    //vehicleImg.saveTo($"~{DirPath.Member}/{auth.user}");
                }
            }
            vehicleInfo.Update();//這邊一定要先記錄 不然mapImg之後會錯
            //修改其他的IsDefault
            vehicleInfo.updateVehicleDefault();
            return new MemberResponse(true)
            {
                info = RemoveObjAttr(vehicleInfo.mapImgPath(auth.user), "MemberId", "UniqueID")
            };
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E006);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            var vehicleRequest = this.getPostObj<VehicleRequest>(RequestFlag.Body.Info);
            //var img = HttpContext.Current.Request.Files[RequestFlag.Body.Img];
            saveUnknowError(e, vehicleRequest.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---LoadVehicle---
    [HttpPost]
    [Route("LoadVehicle")]
    [JwtAuthActionFilter]
    public object LoadVehicle()
    {
        try
        {
            var auth = getAuthObj();
            var memberManager = new MemberManager(auth.user);
            var vehicleList=memberManager.loadMemberVehicle();
            return new MemberResponse(true)
            {
                info=vehicleList.convertResponseList<VehicleInfoResponse>()
            };
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion

    #region ---AddVehicle---
    [HttpPost]
    [Route("AddVehicle")]
    [JwtAuthActionFilter]
    public object AddVehicle()
    {
        try
        {
            var auth = getAuthObj();


            //var httpRequest = HttpContext.Current.Request;

            if (!this.formDataContain(RequestFlag.Body.Info))
                throw new ArgumentNullException();

            var vehicleRequest = this.getPostObj<VehicleRequest>(RequestFlag.Body.Info);
            vehicleRequest.cleanXss();

            if (vehicleRequest.isEmpty())
                throw new ArgumentNullException();

            var member = new Member();
            member.CreatByUniqueId(auth.user);

            if (isObjOverNum<VehicleInfo>(ApiConfig.MaxVehicleNum,QueryPair.New().addQuery("MemberId", member.Id)))
                throw new OutOfNumberException();

            var memberManager = new MemberManager(member);
            VehicleInfo vehicleInfo = null;

            //vehicleInfo.MemberId = member.Id;
            if (this.formFileContain(RequestFlag.Body.Img))
            {
                using (var vehicleImg = this.getPostImg(RequestFlag.Body.Img))
                {
                    vehicleInfo = memberManager.creatVehicleInfo(vehicleRequest, vehicleImg);
                    //vehicleInfo.Img = vehicleImg.fullName;
                    //vehicleImg.saveTo($"~{DirPath.Member}/{auth.user}");
                }
            }
            else
            {
                vehicleInfo = memberManager.creatVehicleInfo(vehicleRequest);
            }
            //vehicleInfo.Id = vehicleInfo.Insert(true);//這邊一定要先記錄 不然mapImg之後會錯

            return new MemberResponse(true)
            {
                info = RemoveObjAttr(vehicleInfo.mapImgPath(auth.user), "MemberId", "UniqueID")
            };
        }
        catch (OutOfNumberException)
        {
            return ResponseError(EkiErrorCode.E016);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E006);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            var vehicleRequest = this.getPostObj<VehicleRequest>(RequestFlag.Body.Info);
            saveUnknowError(e, vehicleRequest.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---PostImg---
    [HttpPost]
    [Route("PostImg")]
    [JwtAuthActionFilter]
    public object PostImg()
    {
        try
        {
            var auth = getAuthObj();


            //var httpRequest = HttpContext.Current.Request;
            if (this.formFileContain(RequestFlag.Body.Icon))
            {
                //var postedFile = httpRequest.Files[RequestFlag.Body.Icon];

                //if (postedFile.ContentLength > 2 * (int)FileUtil.Unit.MB)
                //    throw new InputFormatException();

                //var tempInfo = new FileInfo(postedFile.FileName);


                //if (FileUtil.checkFileUploadFormate(tempInfo, FileUtil.AllowFileOption.Img) != FileUtil.Result.OK)
                //    throw new InputFormatException();


                var member = new Member();
                member.CreatByUniqueId(auth.user);

                using (var postImg = this.getPostImg(RequestFlag.Body.Icon))
                {
                    postImg.adjustImg(ImgSizeType.Icon);
                    //postImg.saveTo($"~{DirPath.Member}/{auth.user}");
                    postImg.saveBitmapWith(member,50L);


                    if (!string.IsNullOrEmpty(member.Info.IconImg))
                    {
                        deleteFile($"~{DirPath.Member}/{auth.user}/{member.Info.IconImg}");
                        //var lastIcon = new FileInfo(serverPath($"~{DirPath.Member}/{auth.user}/{member.Info.IconImg}"));
                        //lastIcon.Delete();
                    }
                    member.Info.IconImg = postImg.fullName;
                    member.Update();
                }

                return new MemberResponse(true)
                {
                    info = new
                    {
                        imgUrl = member.mapIconUrlInfo().IconImg
                    }
                };
            }
            else
            {
                return ResponseError(EkiErrorCode.E014);
            }
        }
        catch (InputFormatException) { return ResponseError(EkiErrorCode.E013); }
        catch (Exception e)
        {
            //return e;
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion

    #region ---EditPwd---
    [HttpPost]
    [Route("EditPwd")]
    [JwtAuthActionFilter]
    public object EditPwd(EditPwdRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new InputFormatException();

            var auth = getAuthObj();          

            var memberManager = new MemberManager(auth.user);

            if (memberManager.editPwd(request.decode))
            {
                return new MemberResponse(true);
            }
            return new MemberResponse(false).setErrorCode(EkiErrorCode.E012);
        }
        catch (InputFormatException)
        {
            //request.saveLog($"valid->{request.isValid()} decode->{request.decode.toJsonString()} oldV->{TextUtil.checkPwdVaild(request.decode.oldPwd)} newV->{TextUtil.checkPwdVaild(request.decode.newPwd)} ");
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
        }
        return ResponseError();
    }
    #endregion

    #region ---Edit---
    [HttpPost]
    [Route("Edit")]
    [JwtAuthActionFilter]
    public object Edit(MemberEditRequest request)
    {
        try
        {
            var auth = getAuthObj();
            request.cleanXss();
            if (!request.isOK())
                throw new InputFormatException();

            var memberManager = new MemberManager(auth.user);
            memberManager.editMember(request);

            return new MemberResponse(true);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
        }
        return ResponseError();
    }
    #endregion

    #region ---Login---
    [HttpPost]
    [Route("Login")]
    public object Login(LoginRequest value)
    {
        try
        {


            if (!value.isValid() && !value.isEmpty())
                throw new InputFormatException();

            //value.saveLog("Get login");

            var queryPair = Member.checkValidMemberQuery(value.isEmail ? "Mail" : "PhoneNum", value.acc);


            if (EkiSql.ppyp.count<Member>(queryPair) != 1)
                throw new AccountNotExistException();


            //memberCmd.saveLog("Member Cmd");
            var member = EkiSql.ppyp.data<Member>(queryPair);
            //member.saveLog("Login member");
            if (!member.beEnable)
                throw new AccountException();

            var memberManager = new MemberManager(member);
            memberManager.Update(value);

            var vList = EkiSql.ppyp.tableByPair<VehicleInfo>(QueryPair.New()
                .addQuery("MemberId", member.Id)).toSafeList();
            vList.ForEach(vehicle =>
            {
                vehicle.mapImgPath(member.UniqueID.toString());
            });

            if (memberManager.checkPwd(value.pwd))
                return new MemberResponse(true)
                {
                    info = new
                    {
                        token = memberManager.member.token(),
                        mail = member.Mail,
                        phoneNum = member.PhoneNum,
                        member.beManager,
                        member.Lv,
                        //能被介紹
                        referrer = (from r in GetTable<SalesReferrer>()
                                    where r.MemberId == member.Id
                                    select r).Count() < 1 ? "" : (from r in GetTable<SalesReferrer>()
                                                                  where r.MemberId == member.Id
                                                                  select r).FirstOrDefault()?.ReferrerCode,
                        member = RemoveIdAttr(member.mapIconUrlInfo()),
                        address = RemoveIdAttr(member.Address),
                        //bank = member.beManager ? (from b in GetTable<BankInfo>()
                        //                           where b.MemberId == member.Id
                        //                           select b).FirstOrDefault()?.convertToResponse() : null,
                        vehicleList = RemoveListAttr(vList, "MemberId", "UniqueID", "currentUnit", "chargeSocket"),
                        //約定付款的token 是否可以使用
                        payTokenLife = new
                        {
                            neweb = member.PayInfo == null ? "" : member.PayInfo.neweb.TokenLife
                        }
                    }
                };
            else
                throw new AccountNotExistException();

        }
        catch (InputFormatException) { return ResponseError(EkiErrorCode.E001); }
        catch (AccountNotExistException) { return ResponseError(EkiErrorCode.E012); }
        catch (AccountException) { return ResponseError(EkiErrorCode.E024); }
        catch (Exception e)
        {
            saveUnknowError(e, value.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---Register---
    /// <summary>
    /// AddressRequest 可不代
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Register")]
    public object Register(RegisterRequest value)
    {
        try
        {
            if (!value.isValid())
                throw new InputFormatException();


            if (EkiSql.ppyp.count<Member>(QueryPair.New().addQuery("Mail", value.mail)) > 0)
                throw new EmailExistException();


            if (EkiSql.ppyp.count<Member>(QueryPair.New().addQuery("PhoneNum", value.phone)) > 0)
                throw new PhoneExistException();

            //var memberManager = new MemberManager();
            //var member = memberManager.convertToMember(value);
            var member = value.convertToDbModel();

            //return new
            //{
            //    Salt = saltKey,
            //    HashPwd = hashPwd
            //};

            var saveId = member.Insert(true);
            var newMember = EkiSql.ppyp.dataById<Member>(saveId);
            creatDir($"~{DirPath.Member}/{newMember.UniqueID.toString()}");
            creatDir($"~{string.Format(DirPath.Order, newMember.UniqueID.toString())}");
            //var success = new MemberResponse(true);
            //success.info = new { token = memberManager.setMember(newMember).getToken() };

            //success.token = JwtBuilder.GetEncoder()
            //    .setUser(newMember.UniqueID)
            //    .setExpDate(DateTime.Now.AddDays(WebUtil.getTokenDay()))
            //    .encode();

            return new MemberResponse(true)
            {
                info = new
                {
                    token = newMember.token()
                }
            };
        }
        catch (InputFormatException) { return ResponseError(EkiErrorCode.E001); }
        catch (EmailExistException) { return ResponseError(EkiErrorCode.E008); }
        catch (PhoneExistException) { return ResponseError(EkiErrorCode.E009); }
        catch (Exception e)
        {
            saveUnknowError(e, value.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ForgetPwd
    [HttpPost]
    [Route("ForgetPwd")]
    [EkiSecretFilter]
    public object ForgetPwd(ForgetPwdRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var member = (from m in GetTable<Member>()
                          where m.PhoneNum == request.decode.phone
                          select m).FirstOrDefault();
            if (member == null)
                throw new ArgumentException();
            if (!member.beEnable)
                throw new AccountException();

            var manager = new MemberManager(member);
            manager.editPwdByForget(request.decode);

            return ResponseOK();
        }
        catch (AccountException) { return ResponseError(EkiErrorCode.E024); }
        catch (ArgumentException)
        {
            request.saveLog($"valid->{request.isValid()} decode->{request.decode.toJsonString()}");
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion

    public class MemberResponse : ResponseInfoModel<object>
    {
        public MemberResponse(bool successful) : base(successful)
        {
        }
    }

}
