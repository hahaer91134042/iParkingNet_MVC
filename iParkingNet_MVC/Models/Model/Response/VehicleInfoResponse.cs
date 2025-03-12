using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// VehicleInfoResponse 的摘要描述
/// </summary>
public class VehicleInfoResponse:ApiAbstractModel
{
    public string Label { get; set; }
    public string Type { get; set; }
    public int Current { get; set; }
    public int Charge { get; set; }
    public string Number { get; set; }
    public string Name { get; set; }
    public string Img { get; set; }
    public int Id { get; set; }
    public bool IsDefault { get; set; }
}