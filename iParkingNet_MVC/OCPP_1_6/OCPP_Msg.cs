using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Msg 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class OCPP_Msg : List<object>
    {

        public Call msgToCall() => new Call(this);
        public Result msgToResult() => new Result(this);

        public MsgType getMsgType() => this[OCPP_Config.FieldPosition.Type].toInt().toEnum<MsgType>();
        public void setMsgType(MsgType r) => Insert(OCPP_Config.FieldPosition.Type, r.toInt());
        public string getMsgID() => this[OCPP_Config.FieldPosition.Uid].ToString();
        public void setMsgID(string id) => Insert(OCPP_Config.FieldPosition.Uid, id);

        public virtual void setPayload<T>(T input) 
        {
            if(this is IMsgPayloadPosition)
                Insert((this as IMsgPayloadPosition).dataPosition(), input);
            else
                throw new NotImplementedException($"Must implement interface {typeof(IMsgPayloadPosition).Name}");
        }
        public virtual T getPayload<T>()
        {
            if(this is IMsgPayloadPosition)
                return this[(this as IMsgPayloadPosition).dataPosition()].toObj<T>();

            throw new NotImplementedException($"Must implement interface {typeof(IMsgPayloadPosition).Name}");
        }

        public class Call : OCPP_Msg,IMsgPayloadPosition
        {            
            public Call() { }
            public Call(IEnumerable<object> datas)
            {
                AddRange(datas);
            }

            #region 靜態方法
            public static Call creatByPayload<T>(IOCPP_SendPayload<T> payload)where T:IOCPP_Payload
            {
                //必須依照順序
                var call = new Call();
                call.setMsgType(MsgType.Call);
                call.setMsgID(OCPP_Util.creatUid());
                call.setAction(payload.ocppAction());
                call.setPayload(payload.ocppPayload());

                return call;
            }
            #endregion

            public Result callToResult() => new Result(this);

            public OCPP_Action getAction() => this[OCPP_Config.FieldPosition.CallAction].ToString().toEnum<OCPP_Action>();
            public void setAction(OCPP_Action action) => Insert(OCPP_Config.FieldPosition.CallAction, action.ToString());
            int IMsgPayloadPosition.dataPosition() => OCPP_Config.FieldPosition.CallPayload;
        }

        public class Result : OCPP_Msg,IMsgPayloadPosition
        {
            public Result() { }
            public Result(IEnumerable<object> datas)
            {
                AddRange(datas);
            }
            public Result(Call msg)
            {
                setMsgType(MsgType.CallResult);
                setMsgID(msg.getMsgID());
            }

            int IMsgPayloadPosition.dataPosition() => OCPP_Config.FieldPosition.ResultPayload;
        }

        public interface IMsgPayloadPosition
        {
            int dataPosition();
        }
        
        public static CallBuilder<T> CreatCall<T>(IOCPP_SendPayload<T> payload) where T : IOCPP_Payload
        {
            return new CallBuilder<T>(payload);
        }

        public static ResultBuilder CreatResult(Call call)
        {
            
            return new ResultBuilder(call.callToResult());
        }

        public class CallBuilder <T>: IMsgBuilder<OCPP_Msg> where T : IOCPP_Payload
        {
            private IOCPP_SendPayload<T> payload;

            internal CallBuilder(IOCPP_SendPayload<T> p)
            {
                payload = p;
            }

            public OCPP_Msg build()
            {
                return Call.creatByPayload(payload);
            }
        }
        public class ResultBuilder : IMsgBuilder<OCPP_Msg>
        {
            private Result result;
            private object payload;
            internal ResultBuilder(Result r)
            {
                result = r;
            }

            public ResultBuilder addPayload(object p)
            {
                payload = p;
                return this;
            }
            public OCPP_Msg build()
            {
                if (payload == null)
                    payload = new { };
                result.setPayload(payload);
                return result;
            }
        }
    }
}
