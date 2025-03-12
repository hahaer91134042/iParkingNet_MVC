using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ResponseContent 的摘要描述
/// </summary>
public class ResponseContent
{
    public class ActionPage
    {
        public string Html { get; set; }
        public string Url { get; set; }
    }
    public class Action
    {
        public string Name { get; set; }

        public object Page { get; set; }
    }

    public class Notify
    {
        public string Title { get; set; }
        public string Msg { get; set; }
        public string Html { get; set; }

    }
}