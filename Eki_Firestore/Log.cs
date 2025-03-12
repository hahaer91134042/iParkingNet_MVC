using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore_PPYP.Log
{
    public class Log
    {
        private const bool isLogPrint = true;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void d(string msg, Exception e = null)
        {
            if (isLogPrint)
                if (e == null)
                    logger.Debug(msg);
                else
                    logger.Debug($"{msg}->{e}");
        }

        public static void i(string msg, Exception e = null)
        {
            if (isLogPrint)
                if (e == null)
                    logger.Info(msg);
                else
                    logger.Info($"{msg}->{e}");
        }

        public static void e(string msg, Exception e = null)
        {
            if (isLogPrint)
                if (e == null)
                    logger.Error(msg);
                else
                    logger.Error($"{msg}->{e}");
        }
    }
}
