using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class DataListModel<DATA>
{
    public DataListModel()
    {
        List = new List<DATA>();
    }

    public List<DATA> List { get; set; }
}
