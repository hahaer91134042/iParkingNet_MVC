using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocationInfoResponseModel 的摘要描述
/// </summary>
public class LocationInfoResponseModel:ApiAbstractModel
{

    public int Current = 0;
    public int Charge = 0;
    //public int Status = 0;
    public string SerialNumber = "";
    public string Content = "";
    //public string Img = "";
    public int Position = 0;
    public int Size = 0;
    public int Type = 0;
    public double Height = -1;
    public double Weight = -1;

    public void load(LocationInfo data)
    {
        Current = data.Current;
        Charge = data.Charge;
        //Status = data.Status;
        SerialNumber = data.SerialNumber;
        Content = data.InfoContent;
        //Img = data.Img;
        Position = data.Position;
        Size = data.Size;
        Type = data.Type;
        Height = data.Height;
        Weight = data.Weight;
    }
}