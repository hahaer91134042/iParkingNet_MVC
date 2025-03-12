using DevLibs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// EkiPostImg 的摘要描述
/// </summary>
public class EkiPostImg : IDisposable,IBitmapImg
{
    public string exten;
    public Bitmap imgBitmap;
    public string fileName;
    public string fullName { get { return fileName + exten; } }
    public bool isEmpty = false;

    public EkiPostImg(HttpPostedFile postedFile)
    {
        if (postedFile != null)
        {
            exten = new FileInfo(postedFile.FileName).Extension.ToLower();
            fileName = FileUtil.creatRngFileName();
            imgBitmap = new Bitmap(postedFile.InputStream);
        }
        else
        {
            isEmpty = true;
        }        
    }

    public void adjustImg(ImgSizeType imgSize)
    {
        if(!isEmpty)
            imgBitmap = ImgUtil.ZoomImage(imgBitmap, imgSize);
    }

    public void saveTo(string path)
    {
        var dirPath = path.toServerPath();
        dirPath.creatDir();//保險起見
        if(!isEmpty)
            ImgUtil.SaveBitmap(imgBitmap, $"{dirPath}/{fullName}", exten);
    }

    public void Dispose()
    {
        if(!isEmpty)
            imgBitmap.Dispose();
    }

    public Bitmap bitmap() => imgBitmap;
    public string imgName() => fullName;
    public string imgExten() => exten;
}