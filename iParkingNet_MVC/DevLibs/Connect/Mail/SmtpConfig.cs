using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class SmtpConfig
{
    public abstract string serverIp();
    public abstract int serverPort();
    public abstract bool serverSSL();
    public abstract string userName();
    public abstract string userPwd();
}
