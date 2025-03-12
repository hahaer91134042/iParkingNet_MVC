using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StatusNotifySort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class StatusNotifySort : BaseCallMsgSort<StatusNotifyCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.StatusNotification;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {          

            try
            {
                cp.status = payload.status<OCPP_Status.CP>();
                cp.connectorId = payload.connectorId;

                Log.d($"StatusNotify  status->{payload.toJsonString()}  cp->{cp}");

                //payload.setStatus(OCPP_Status.CP.Faulted);
                //Log.print($"Test set status->{payload.toJsonString()}");

                //var statusResult = call.creatResult();
                switch (cp.status)
                {
                    //case OCPP_Status.CP.SuspendedEVSE:                
                    //case OCPP_Status.CP.SuspendedEV:
                    case OCPP_Status.CP.Faulted://改成重啟
                        EkiOCPP.sendCallAsync(cp.serial, TriggerMessageCall.Boot);
                        break;
                    case OCPP_Status.CP.Finishing:

                        //cp.auth = OCPP_Status.Authorize.Invalid;
                        //EkiOCPP.stopTranscation(cp.serial);
                        //EkiOCPP.sendCallAsync(cp.serial, new RemoteStopTransactionCall());

                        break;

                    case OCPP_Status.CP.Charging:
                        //準備充電中(要傳輸資料)                        
                        //要先去詢問裝置現在認證狀態
                        //這樣startTranscation才不會有問題

                        //這樣是避免伺服器重開 但CP正在充電中 所導致的資料問題
                        cp.auth = OCPP_Status.Authorize.Accepted;



                        break;
                    case OCPP_Status.CP.Preparing:
                        //目前要車主手動觸發
                        //checkCharge(cp);

                        break;


                    case OCPP_Status.CP.Unavailable:
                    case OCPP_Status.CP.Available://待機狀態(拔掉充電槍會觸發)
                                                  //每次充完電 須把認證狀態變成非認證 以便之後辨別是否非地主
                        cp.auth = OCPP_Status.Authorize.Invalid;
                        break;
                    default:

                        break;
                }
                //var result = call.callToResult().Also(r => r.setPayload(new StatusNotifyResult()));
                var result = new StatusNotifyResult();
                //Log.d($"StatusNotify result->{result.toJsonString()}");

                cp.socket.SendOCPP(OCPP_Msg.CreatResult(call).addPayload(result));
            }
            catch (Exception e)
            {
                Log.e("StatusNotifySort error", e);
            }

        }

        private void checkCharge(ChargePoint cp)
        {
            //表示沒有認證(非地主 無IDcard)過的使用者 要去查詢該地點能否充電
            if (cp.auth != OCPP_Status.Authorize.Accepted)
            {
                var model = OCPP_CP.CreatByCpSerial(cp.serial);

                var location = model.location();
                var now = DateTime.Now;

                var open = (from o in EkiSql.ppyp.table<EkiOrder>()
                            where o.LocationId == location.Id
                            where o.beEnable
                            where o.StatusEnum == OrderStatus.Reserved
                            where o.getStartTime().Date == now.Date
                            where o.between(now)
                            select new
                            {
                                Start = o.getStartTime(),
                                End = o.getEndTime()
                            }).FirstOrDefault();

                Log.d($"preparing loc serial->{location.SerNum}  order open->{open.toJsonString()}");

                if (open != null)
                {
                    cp.auth = OCPP_Status.Authorize.Accepted;

                    EkiOCPP.sendCallAsync(cp.serial, new RemoteStartTransactionCall
                    {
                        connectorId = cp.connectorId
                    });

                }


            }
        }

    }
}