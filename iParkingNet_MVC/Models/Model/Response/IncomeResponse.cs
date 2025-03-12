using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IncomeResponse 的摘要描述
/// </summary>
public class IncomeResponse : ApiAbstractModel
{
    public string SerNum { get; set; } = "";
    public List<IncomeResult> Result = new List<IncomeResult>();


    public class IncomeResult
    {
        public string Start = "";
        public string End = "";
        public double Income = 0d;
        public double Claimant = 0d;
    }
}