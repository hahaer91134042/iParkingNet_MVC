using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservationResponseModel 的摘要描述
/// </summary>
public class ReservaConfigResponseModel:ApiAbstractModel
{
    public bool beEnable = true;
    public bool beRepeat = true;
    public string Text = "";
    public decimal Price = 0;
    public int Unit = 0;
    public int Method = 0;
    public List<OpenTimeResponseModel> OpenSet = new List<OpenTimeResponseModel>();

    public void load(ReservaConfig data)
    {
        beEnable = data.beEnable;
        beRepeat = data.beRepeat;
        Text = data.Text;
        Price = data.Price;
        Unit = data.Unit;
        Method = data.Method;
        data.OpenSet.ForEach(set =>
        {
            OpenSet.Add(new OpenTimeResponseModel(set));
        });
    }
}