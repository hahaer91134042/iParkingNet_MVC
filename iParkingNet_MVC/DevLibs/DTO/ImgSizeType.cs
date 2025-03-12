using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ImgSizeType 的摘要描述
/// </summary>
public class ImgSizeType
{
    public static ImgSizeType Icon = new ImgSizeType(720,720);


    public int width;
    public int height;
    public ImgSizeType(int w, int h)
    {
        this.width = w;
        this.height = h;
    }
}