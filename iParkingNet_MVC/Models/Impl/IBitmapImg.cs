using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

/// <summary>
/// IBitmapImg 的摘要描述
/// </summary>
public interface IBitmapImg:IMapImg
{
    Bitmap bitmap();
    string imgExten();
}