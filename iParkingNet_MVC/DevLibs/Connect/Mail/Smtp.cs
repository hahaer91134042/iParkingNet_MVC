using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Smtp 的摘要描述
/// </summary>
public class Smtp:IDisposable
{
    public class Builder
    {
        public string serverIp = "";
        public int serverPort = 25;
        public bool serverSSL = false;
        public string userName = "";
        public string pwd = "";
        public Encoding encoding = Encoding.UTF8;
        public string mailFrom = "";
        public string senderName = "";
        public MailMsg mailMsg = MailMsg.New();
        public bool isHtml = true;
        //public List<string> receiverList = new List<string>();
        internal Builder(SmtpConfig c)
        {
            serverIp = c.serverIp();
            serverPort = c.serverPort();
            serverSSL = c.serverSSL();
            userName = c.userName();
            pwd = c.userPwd();
        }
        public Builder from(string from)
        {
            mailFrom = from;
            return this;
        }
        public Builder useHtmlBody(bool isHtml)
        {
            this.isHtml = isHtml;
            return this;
        }
        public Builder setSenderName(string name)
        {
            senderName = name;
            return this;
        }
        //public Builder Receiver(params string[] receivers)
        //{
        //    foreach (var receiver in receivers)
        //        receiverList.Add(receiver);
        //    return this;
        //}
        //public Builder Receiver(List<string> list)
        //{
        //    receiverList.AddRange(list);
        //    return this;
        //}

        public Builder setMsg(MailMsg msg)
        {
            mailMsg.copy(msg);
            return this;
        }

        public Smtp build() => new Smtp(this);

    }

    public static Builder GetBuilder(SmtpConfig c)
    {
        return new Builder(c);
    }
    
    public class Result
    {
        public bool isOK = true;
        public string Message = "Send Mail Success";
        internal Result() { }
        internal Result(string m) { Message = m; }
        internal Result(Exception e):this()
        {
            isOK = false;
            Message = e.Message;
        }
    }



    private Builder smtpBuilder;
    private SmtpClient smtpClient;
    private MailMessage mail = new MailMessage();

    Smtp(Builder b)
    {
        smtpBuilder = b;
        smtpClient = new SmtpClient(b.serverIp, b.serverPort);
        smtpClient.EnableSsl = b.serverSSL;
        smtpClient.Credentials = new System.Net.NetworkCredential(b.userName, b.pwd);

        mail.HeadersEncoding = smtpBuilder.encoding;
        mail.SubjectEncoding = smtpBuilder.encoding;
        mail.BodyEncoding = smtpBuilder.encoding;
        mail.IsBodyHtml = smtpBuilder.isHtml;

        mail.From = new MailAddress(smtpBuilder.mailFrom, smtpBuilder.senderName);

        mail.Subject = smtpBuilder.mailMsg.title;
        mail.Body = smtpBuilder.mailMsg.getMsg();
    }
    //public MailBuilder(string ip, int port)
    //{
    //    serverIp = ip;
    //    serverPort = port;
    //}
    //public MailBuilder ServerIp(string ip)
    //{
    //    serverIp = ip;
    //    return this;
    //}
    //public MailBuilder Port(int port)
    //{
    //    serverPort = port;
    //    return this;
    //}

    public void SendTo(List<string> list,Action<string,Result> back=null)
    {
        list.ForEach(to =>
        {
            var result=SendTo(to);
            if (back != null)
                back(to, result);
        });
    }


    public Result SendTo(string to,MailMsg m=null)
    {
        try
        {
            mail.To.Clear();
            mail.To.Add(to);
            if (m != null)
            {
                mail.Subject = m.title;
                mail.Body = m.getMsg();
            }                
            smtpClient.Send(mail);
        }
        catch(Exception e)
        {
            return new Result(e);
        }
        return new Result();        
    }

    public void Dispose()
    {
        mail.Dispose();
        smtpClient.Dispose();
    }
}