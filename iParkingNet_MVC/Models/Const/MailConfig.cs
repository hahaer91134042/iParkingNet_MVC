using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MailConfig : SmtpConfig
{

    public static Smtp.Builder creatBuilder() => Smtp.GetBuilder(new MailConfig());
    private MailConfig() { }
    //public override string serverIp() => "192.168.11.198";
    public override string serverIp() => "192.168.11.202";//ppyp server

    public override int serverPort() => 25;

    public override bool serverSSL() => false;

    public override string userName() => "";

    public override string userPwd() => "";
}
