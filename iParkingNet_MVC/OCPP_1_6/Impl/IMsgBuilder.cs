using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCPP_1_6
{
    public interface IMsgBuilder<T>
    {
        T build();
    }
}