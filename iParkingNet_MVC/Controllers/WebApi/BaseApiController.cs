using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Log = DevLibs.Log;
using System.Drawing;
using DevLibs;

public abstract class BaseApiController : ApiController,IFormDataControl
{
    //protected SqlContext sqlHelper = new SqlContext();
    //protected Log log = Log.getInstance();//目前沒辦法用

    public BaseApiController()
    {

    }
    protected void saveUnknowError(Exception e, object input)
    {
        if (input.isNullOrEmpty())
        {
            saveUnknowError(e);
        }
        else
        {
            saveUnknowError(e, input.toJsonString());
        }
    }
    protected void saveUnknowError(Exception e,string input="")
    {
        new ErrorLog()
        {
            Api=this.GetType().FullName,
            Input=input,
            Exception = e.GetType().Name,
            Msg = e.StackTrace,
            Ip=WebUtil.GetUserIP()
        }.Insert();
    }


    protected IEnumerable<M> GetTable<M>() where M : BaseDbDAO
    {
        return EkiSql.ppyp.table<M>();
    }


    public string getMemberImg(string uniqueId,string fileName)
    {
        return $"{WebUtil.getWebURL()}{DirPath.Member}/{uniqueId}/{fileName}";
    }

    protected bool isObjOverNum<T>(int max,QueryPair queryPair) where T : DbOperationModel
    {
        //var num = TableParaser.CountNum(sqlHelper.query(SqlCmd.Get<T>.RowCountCmd(queryPair)));
        var num = EkiSql.ppyp.count<T>(queryPair);
        return num >= max;
    }

    //protected bool formDataContain(string key)
    //{
    //    return HttpContext.Current.Request.Form.AllKeys.Contains(key);
    //}
    //protected bool formFileContain(string key)
    //{
    //    return HttpContext.Current.Request.Files.AllKeys.Contains(key);
    //}

    //protected T getPostObj<T>(string flag)
    //{
    //    return HttpContext.Current.Request.Form[flag].toObj<T>();
    //    //return ConvertToObj<T>(HttpContext.Current.Request.Form[flag]);
    //}

    //protected EkiPostImg getPostImg(string flag)
    //{
    //    if(this.formFileContain(flag))
    //        return getPostImg(HttpContext.Current.Request.Files[flag]);
    //    return null;
    //}

    //protected EkiPostImg getPostImg(HttpPostedFile postedFile)
    //{
    //    var postImg = new EkiPostImg(postedFile);
    //    if (FileUtil.checkFileUploadFormate(postImg.exten, FileUtil.AllowFileOption.Img) != FileUtil.Result.OK)
    //        throw new InputFormatException();
    //    return postImg;
    //}

    //protected T ConvertToObj<T>(string jsonStr)
    //{
    //    return JsonConvert.DeserializeObject<T>(jsonStr);
    //}
    //protected string ObjToText(object obj)
    //{
    //    return JsonConvert.SerializeObject(obj);
    //}

    protected string serverPath(string path)
    {
        return HttpContext.Current.Server.MapPath(path);
    }

    protected void deleteOrderImg(string uniqueId,string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
            deleteFile($"~{string.Format(DirPath.Order,uniqueId)}/{fileName}");
    }
    protected void deleteMemberImg(string uniqueId,string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
            deleteFile($"~{DirPath.Member}/{uniqueId}/{fileName}");
    }

    protected void deleteFile(string path)
    {
        var fileInfo = new FileInfo(serverPath(path));
        if(fileInfo.Exists)
            fileInfo.Delete();
    }

    protected void creatDir(string path)
    {
        if (!Directory.Exists(HttpContext.Current.Server.MapPath(path)))
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
    }

    protected virtual SuccessResponse ResponseOK()
    {
        return new SuccessResponse();
    }
    protected virtual ErrorResponse ResponseError(EkiErrorCode code = EkiErrorCode.E004)
    {
        //ActionContext.Response.StatusCode = HttpStatusCode.InternalServerError;          
        return (ErrorResponse)new ErrorResponse().setErrorCode(code);
    }

    protected class ErrorResponse : ResponseAbstractModel
    {
        public ErrorResponse() : base(false)
        {
        }
    }
    protected class SuccessResponse : ResponseAbstractModel
    {
        public SuccessResponse() : base(true)
        {
        }
    }

    protected JwtAuthObject getAuthObj()
    {
        if (ActionContext.Request.Headers.Authorization.Parameter.isNullOrEmpty())
            return null;

        var auth= JwtBuilder.GetDecoder()
                            .setToken(ActionContext.Request.Headers.Authorization.Parameter)
                            .decode();
        creatDir($"~{DirPath.Member}/{auth.user}");
        return auth;
    }

    protected List<JObject> RemoveListAttr<T>(List<T> list ,params string[] attrs)
    {
        var cleanList = new List<JObject>();
        list.ForEach(data => {
            var obj = JObject.FromObject(data);
            foreach (var key in attrs)
                obj.Remove(key);

            cleanList.Add(obj);
        });
        return cleanList;
    }

    protected object RemoveObjAttr(object data, params string[] attrs)
    {
        //var obj = JObject.FromObject(data);
        //foreach (var key in attrs)
        //    obj.Remove(key);
        //return obj;
        return data.removeObjAttr(attrs);
    }
    protected object RemoveIdAttr(object data)
    {
        return RemoveObjAttr(data, "Id");
    }
    protected List<JObject> RemoveListIdAttr<T>(List<T> list)
    {
        return RemoveListAttr(list, "Id");
    }    

}
