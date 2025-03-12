using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SampleValue 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    /// <summary>
    /// 基本上 內部的Context會一致 對應數值action
    /// </summary>
    public class SampleValue : List<SampleValue.Item>
    {
        public SampleValue() { }
        public SampleValue(IEnumerable<Item> collection):base(collection)
        {

        }

        #region Enum
        public class Unit:IOCPP_Enum<string>
        {
            public static Unit Wh = new Unit("Wh");//Watt-hours (energy). Default.
            public static Unit kWh = new Unit("kWh");//kiloWatt-hours (energy).
            public static Unit varh = new Unit("varh");//Var-hours (reactive energy).
            public static Unit kvarh = new Unit("kvarh");//kilovar-hours (reactive energy).
            public static Unit W = new Unit("W");//Watts (power).
            public static Unit kW = new Unit("kW");//kilowatts (power).
            public static Unit VA = new Unit("VA");//VoltAmpere (apparent power).
            public static Unit kVA = new Unit("kVA");//kiloVolt Ampere (apparent power).
            public static Unit var = new Unit("var");//Vars (reactive power).
            public static Unit kvar = new Unit("kvar");//kilovars (reactive power).
            public static Unit A = new Unit("A");//Amperes (current).
            public static Unit V = new Unit("V");//Voltage (r.m.s. AC).
            public static Unit K = new Unit("K");//Degrees Kelvin (temperature).
            public static Unit Celcius = new Unit("Celcius");
            public static Unit Celsius = new Unit("Celsius");//Degrees (temperature).
            public static Unit Fahrenheit = new Unit("Fahrenheit");//Degrees (temperature).
            public static Unit Percent = new Unit("Percent");//Percentage.

            public string symbol;
            internal Unit(string s) => symbol = s;

            public static bool operator ==(string p1, Unit p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Unit p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Unit p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Unit p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Unit p1, Unit p2)
            {
                //Log.d($"unit p1=>{p1} p2=>{p2} equal->{p1.Equals(p2)}");
                return p1.Equals(p2);
            }
            public static bool operator !=(Unit p1, Unit p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Unit)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;
        }
        public class Format:IOCPP_Enum<string>
        {
            public static Format Raw = new Format("Raw");
            public static Format SignedData = new Format("SignedData");

            public string symbol;
            internal Format(string s) => symbol = s;
            public static bool operator ==(string p1, Format p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Format p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Format p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Format p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Format p1, Format p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Format p1, Format p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Format)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;
        }
        public class Measurand:IOCPP_Enum<string>
        {
            public static Measurand Energy_Active_Export_Register = new Measurand("Energy.Active.Export.Register");
            public static Measurand Energy_Active_Import_Register = new Measurand("Energy.Active.Import.Register");
            public static Measurand Energy_Reactive_Export_Register = new Measurand("Energy.Reactive.Export.Register");
            public static Measurand Energy_Reactive_Import_Register = new Measurand("Energy.Reactive.Import.Register");
            public static Measurand Energy_Active_Export_Interval = new Measurand("Energy.Active.Export.Interval");
            public static Measurand Energy_Active_Import_Interval = new Measurand("Energy.Active.Import.Interval");
            public static Measurand Energy_Reactive_Export_Interval = new Measurand("Energy.Reactive.Export.Interval");
            public static Measurand Energy_Reactive_Import_Interval = new Measurand("Energy.Reactive.Import.Interval");
            public static Measurand Power_Active_Export = new Measurand("Power.Active.Export");
            public static Measurand Power_Active_Import = new Measurand("Power.Active.Import");
            public static Measurand Power_Offered = new Measurand("Power.Offered");
            public static Measurand Power_Reactive_Export = new Measurand("Power.Reactive.Export");
            public static Measurand Power_Reactive_Import = new Measurand("Power.Reactive.Import");
            public static Measurand Power_Factor = new Measurand("Power.Factor");
            public static Measurand Current_Import = new Measurand("Current.Import");//流向 EV 的瞬時電流
            public static Measurand Current_Export = new Measurand("Current.Export");//來自 EV 的瞬時電流
            public static Measurand Current_Offered = new Measurand("Current.Offered");//提供給 EV 的最大電流
            public static Measurand Voltage = new Measurand("Voltage");
            public static Measurand Frequency = new Measurand("Frequency");
            public static Measurand Temperature = new Measurand("Temperature");
            public static Measurand SoC = new Measurand("SoC");
            public static Measurand RPM = new Measurand("RPM");

            public string symbol;
            internal Measurand(string s)
            {
                symbol = s;
            }
            public static bool operator ==(string p1, Measurand p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Measurand p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Measurand p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Measurand p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Measurand p1, Measurand p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Measurand p1, Measurand p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Measurand)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;
        }
        public class Phase:IOCPP_Enum<string>
        {
            public static Phase L1 = new Phase("L1");//Measured on L1
            public static Phase L2 = new Phase("L2");//Measured on L2
            public static Phase L3 = new Phase("L3");//Measured on L3
            public static Phase N = new Phase("N");//Measured on Neutral
            public static Phase L1_N = new Phase("L1-N");//Measured on L1 with respect to Neutral conductor
            public static Phase L2_N = new Phase("L2-N");//Measured on L2 with respect to Neutral conductor
            public static Phase L3_N = new Phase("L3-N");//Measured on L3 with respect to Neutral conductor
            public static Phase L1_L2 = new Phase("L1-L2");//Measured between L1 and L2
            public static Phase L2_L3 = new Phase("L2-L3");//Measured between L2 and L3
            public static Phase L3_L1 = new Phase("L3-L1");//Measured between L3 and L1

            public string symbol;
            internal Phase(string s)
            {
                symbol = s;
            }
            public static bool operator ==(string p1, Phase p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Phase p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Phase p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Phase p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Phase p1, Phase p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Phase p1, Phase p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Phase)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;
        }
        public class Location:IOCPP_Enum<string>
        {
            public static Location Cable = new Location("Cable");//Measurement taken from cable between EV and Charge Point
            public static Location EV = new Location("EV");//EV 進行的測量
            public static Location Body = new Location("Body");//Measurement inside body of Charge Point (e.g. Temperature)
            public static Location Outlet = new Location("Outlet");//在連接器處測量。 默認值 Measurement at a Connector. Default value
            public static Location Inlet = new Location("Inlet");//在網絡（“網格”）入口連接處測量 Measurement at network (“grid”) inlet connection
            public string symbol;
            internal Location(string s)
            {
                symbol = s;
            }
            public static bool operator ==(string p1, Location p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Location p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Location p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Location p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Location p1, Location p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Location p1, Location p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Location)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;
        }
        public class Context:IOCPP_Enum<string>
        {
            public static Context Interruption_Begin = new Context("Interruption.Begin");//Value taken at start of interruption.
            public static Context Interruption_End = new Context("Interruption.End");//Value taken when resuming after interruption.
            public static Context Transaction_Begin = new Context("Transaction.Begin");//Value taken at start of transaction.
            public static Context Transaction_End = new Context("Transaction.End");//Value taken at end of transaction.
            public static Context Trigger = new Context("Trigger");//用trigger msg觸發的 Value taken in response to a TriggerMessage.req
            public static Context Other = new Context("Other");//Value for any other situations.
            public static Context Clock = new Context("Sample.Clock");//Value taken at clock aligned interval.
            public static Context Periodic = new Context("Sample.Periodic");//充電中自動觸發 Value taken as periodic sample relative to start time of transaction.


            public string symbol;
            internal Context(string s)
            {
                symbol = s;
            }

            public static bool operator ==(string p1, Context p2)
            {
                return p2.Equals(p1);
            }
            public static bool operator !=(string p1, Context p2)
            {
                return !p2.Equals(p1);
            }
            public static bool operator ==(Context p1, string p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Context p1, string p2)
            {
                return !p1.Equals(p2);
            }
            public static bool operator ==(Context p1, Context p2)
            {
                return p1.Equals(p2);
            }
            public static bool operator !=(Context p1, Context p2)
            {
                return !p1.Equals(p2);
            }
            public override bool Equals(object obj)
            {
                if (obj.GetType() == GetType())
                    return symbol == ((Context)obj).symbol;

                return symbol == obj.ToString();
            }
            public override string ToString()
            {
                return symbol;
            }

            public string enumValue() => symbol;

        }
        #endregion

        public Context findContext()
        {
            if (Count < 1)
                return null;
            return new Context(this[0].context);
        }


        public T valueBy<T>(params IOCPP_Enum<string>[] args)
        {
            if (args.Length < 1)
                return default(T);

            var context = args.OfType<Context>();
            var format = args.OfType<Format>();
            var measurand = args.OfType<Measurand>();
            var location = args.OfType<Location>();
            var phase = args.OfType<Phase>();
            var unit = args.OfType<Unit>();

            var data = (from value in this
                        where context.Count() > 0 ? value.check(context.ToArray()) : true
                        where format.Count() > 0 ? value.check(format.ToArray()) : true
                        where measurand.Count() > 0 ? value.check(measurand.ToArray()) : true
                        where location.Count() > 0 ? value.check(location.ToArray()) : true
                        where phase.Count() > 0 ? value.check(phase.ToArray()) : true
                        where unit.Count() > 0 ? value.check(unit.ToArray()) : true
                        select value).FirstOrDefault();

            if (data == null)
                return default(T);

            return (T)Convert.ChangeType(data.value, typeof(T));
        }

        public SampleValue find(params IOCPP_Enum<string>[] args)
        {

            var context = args.OfType<Context>();
            var format = args.OfType<Format>();
            var measurand = args.OfType<Measurand>();
            var location = args.OfType<Location>();
            var phase = args.OfType<Phase>();
            var unit = args.OfType<Unit>();

            //Log.d($"Sample find context->{context.toJsonString()} unit->{unit.toJsonString()}  unit count->{unit.Count()}");

            var data = (from value in this
                        where context.Count() > 0 ? value.check(context.ToArray()) : true
                        where format.Count() > 0 ? value.check(format.ToArray()) : true
                        where measurand.Count() > 0 ? value.check(measurand.ToArray()) : true
                        where location.Count() > 0 ? value.check(location.ToArray()) : true
                        where phase.Count() > 0 ? value.check(phase.ToArray()) : true
                        where unit.Count() > 0 ? value.check(unit.ToArray()) : true
                        select value);
            //Log.d($"find result->{data.toJsonString()}");
            //if (data == null)
            //    data = new List<Item>();
            return new SampleValue(data);
        }

        /*
       {
           "value":"2230",
           "context":"Transaction.End",
           "format":"Raw",
           "measurand":"Voltage",
           "phase":"L1-N",
           "location":"Body",
           "unit":"V"
        }
         */
        public class Item
        {
            public string value { get; set; }
            public string context { get; set; }
            public string format { get; set; }
            public string measurand { get; set; }//被測量/報告的值的類型。
            public string phase { get; set; }
            public string location { get; set; }//Inlet/Outlet 應該是流入/流出
            public string unit { get; set; }

            public T getValue<T>() => (T)Convert.ChangeType(value, typeof(T));

            public bool check<T>(object v) where T : IOCPP_Enum<string>
            {
                if (typeof(Context) == typeof(T))
                    return check((Context)v);
                if (typeof(Format) == typeof(T))
                    return check((Format)v);
                if (typeof(Measurand) == typeof(T))
                    return check((Measurand)v);
                if (typeof(Location) == typeof(T))
                    return check((Location)v);
                if (typeof(Phase) == typeof(T))
                    return check((Phase)v);
                if (typeof(Unit) == typeof(T))
                    return check((Unit)v);

                return false;
            }

            public bool check(params Unit[] args) => args.Any(u => u == unit);
            public bool check(params Context[] args) => args.Any(c => c == context);
            public bool check(params Measurand[] args) => args.Any(m => m == measurand);
            public bool check(params Format[] args) => args.Any(f => f == format);
            public bool check(params Phase[] args) => args.Any(p => p == phase);
            public bool check(params Location[] args) => args.Any(l => l == location);
        }
    }
}
