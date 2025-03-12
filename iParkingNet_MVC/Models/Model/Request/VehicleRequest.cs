using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// VehicleRequest 的摘要描述
/// </summary>
public class VehicleRequest:RequestAbstractModel
{
    public int id { get; set; }
    public string label { get; set; }
    public string type { get; set; }
    public int current { get; set; }
    public int charge { get; set; }
    public string name { get; set; }
    public string number { get; set; }
    public bool isDefault { get { return def; } set { def = value; } }

    private bool def = false;

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(label);
            cleanXssStr(type);
            cleanXssStr(name);
            cleanXssStr(number);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool isEmpty()
    {
        //string.IsNullOrEmpty(label) || string.IsNullOrEmpty(type) ||
        return string.IsNullOrEmpty(name) || string.IsNullOrEmpty(number);
    }
}