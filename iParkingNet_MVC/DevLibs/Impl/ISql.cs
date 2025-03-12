using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevLibs
{
    public interface ISql
    {
        int timeOut();
        IDbConfig dbConfig();
    }
}