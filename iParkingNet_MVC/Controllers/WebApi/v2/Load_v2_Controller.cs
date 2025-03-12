using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

[RoutePrefix("api/v2/Load")]
public class Load_v2_Controller : BaseApiController
{

    #region ---Location---
    [HttpPost]
    [Route("Location")]
    public object Location(LoadLocationRequest request)
    {
        try
        {
            if (request.isEmpty())
                throw new ArgumentNullException();
            if (!request.isValid())
                throw new ArgumentException();

            //return new
            //{
            //    request
            //};
            //DateTime end;
            //DateTime start;

            //return new
            //{
            //    IsEnd= DateTime.TryParseExact(request.config.searchTime.end, ApiConfig.TimeFormat, null, System.Globalization.DateTimeStyles.None, out end),
            //    end,
            //    IsStart= DateTime.TryParseExact(request.config.searchTime.start, ApiConfig.TimeFormat, null, System.Globalization.DateTimeStyles.None, out start),
            //    start,
            //    Large=end>start
            //};


            var loadManager = new LoadManager();

            //var locInRange = request.address.isEmpty() ? loadManager.getLocationFrom(request.lat, request.lng, request.config,request.nowTime) : loadManager.getLocationFrom(request.address, request.config,request.nowTime);
            var result = loadManager.getLocationFrom(request,2);

            return new LoadResponse(true)
            {
                info = result
            };
        }
        catch (GoogleApiException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion


    public class LoadResponse : ResponseInfoModel<object>
    {
        public LoadResponse(bool successful) : base(successful)
        {
        }
    }
}

