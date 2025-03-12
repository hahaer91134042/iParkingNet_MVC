using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class JwtAuthObject
{
    public string sub { get; set; }
    public string iat { get; set; }
    public string exp { get; set; }//到期日
    public string user { get; set; }//使用者

   
}
