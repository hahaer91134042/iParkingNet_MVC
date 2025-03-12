using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

/// <summary>
/// FileUtil 的摘要描述
/// </summary>
public class FileUtil
{
    public enum Result
    {
        OK,
        寬度錯誤,
        長度錯誤,
        檔案大小錯誤,
        上傳檔案格式錯誤
    }
    public enum AllowFileOption
    {
        ALL,
        Img,
        Exel,
        Word
    }
    public enum AllowFileFormat
    {
        png,
        jpg,
        jpeg,
        gif,
        xls,
        xlsx,
        pdf,
        csv,
        doc,
        docx,
        txt
    }
    public enum Unit
    {
        Byte = 1,
        KB = 1024,
        MB = 1024 * 1024
    }

    public class ContentType
    {
        public const string Image = "image/";
        public const string Excel = "application/vnd.ms-excel";
        public const string Pdf = "application/pdf";
        public const string Word = "application/msword";
        public const string Txt = " text/plain";
    }

    public static string creatRngFileName()
    {
        return DateTime.Now.ToString("yyyyMMddhhmmssfff") + new Random().Next(0000, 9999).ToString().PadLeft(4, '0');
    }
    public static Result checkImgSize(FileUpload input, int width, int height, bool isDis = true)
    {
        return checkImgSize(input.PostedFile.InputStream, width, height, isDis);
    }
    public static Result checkImgSize(Stream input, int width, int height, bool isDis = true)
    {
        return checkImgSize(new Bitmap(input), width, height, isDis);
    }
    public static Result checkImgSize(Bitmap imgFile, int width, int height, bool isDis = true)
    {
        int fileWidth = imgFile.Width;
        int fileHeight = imgFile.Height;
        if (fileWidth > width)
        {
            return Result.寬度錯誤;
        }
        if (fileHeight > height)
        {
            return Result.長度錯誤;
        }
        if (isDis)
            imgFile.Dispose();
        return Result.OK;
    }
    public static Result checkFileSize(FileUpload info, long size, Unit unit = Unit.Byte)
    {
        return checkFileSize(info.FileBytes, size, unit);
    }

    public static Result checkFileSize(byte[] bytes, long size, Unit unit = Unit.Byte)
    {
        try
        {
            if (bytes.LongLength <= size * (int)unit)
                return Result.OK;
        }
        catch (Exception) { }
        return Result.檔案大小錯誤;
    }

    public static Result checkFileUploadFormate(FileUpload info, params AllowFileFormat[] allows)
    {   //使用FileInfo只是為了解析副檔名 其他並沒有用 因為檔名抓不到真正的檔案(轉換檔案名稱了)
        return checkFileUploadFormate(new FileInfo(info.FileName), allows);
    }

    public static Result checkFileUploadFormate(FileInfo info, params AllowFileFormat[] allows)
    {
        return checkFileUploadFormate(info.Extension, allows);
    }

    public static Result checkFileUploadFormate(string exten, params AllowFileFormat[] allows)
    {
        try
        {
            foreach (var formate in allows)
            {
                if (exten.ToLower().EndsWith(formate.ToString()))
                    return Result.OK;
            }
            //string fileFormate = "," + info.Extension.ToLower() + ",";//副檔名 通通用小寫
            //StringBuilder formateStr = new StringBuilder(",");
            //foreach (var formate in allows)
            //{
            //    formateStr.Append("." + formate.ToString() + ",");
            //}
            //if (formateStr.ToString().IndexOf(fileFormate) >= 0)//有找到
            //{
            //    return Result.OK;
            //}
        }
        catch (Exception) { }
        return Result.上傳檔案格式錯誤;
    }

    public static Result checkFileUploadFormate(FileUpload info, AllowFileOption option)
    {
        return checkFileUploadFormate(new FileInfo(info.FileName), option);
    }

    public static Result checkFileUploadFormate(FileInfo info, AllowFileOption option)
    {
        return checkFileUploadFormate(info.Extension, option);
    }

    public static Result checkFileUploadFormate(string ext, AllowFileOption option)
    {
        switch (option)
        {
            case AllowFileOption.ALL://這個以後自行再增加確認的檔案型態 目前只有這些
                return checkFileUploadFormate(ext,
                    AllowFileFormat.png,
                    AllowFileFormat.jpg,
                    AllowFileFormat.jpeg,
                    AllowFileFormat.gif,
                    AllowFileFormat.xlsx,
                    AllowFileFormat.xlsx,
                    AllowFileFormat.pdf,
                    AllowFileFormat.csv,
                    AllowFileFormat.doc,
                    AllowFileFormat.docx,
                    AllowFileFormat.txt
                    );
            case AllowFileOption.Img://只確認圖片檔
                return checkFileUploadFormate(ext,
                    AllowFileFormat.png,
                    AllowFileFormat.jpg,
                    AllowFileFormat.jpeg,
                    AllowFileFormat.gif
                    );
            case AllowFileOption.Exel:
                return checkFileUploadFormate(ext,
                    AllowFileFormat.xls,
                    AllowFileFormat.xlsx
                    );
            case AllowFileOption.Word:
                return checkFileUploadFormate(ext,
                    AllowFileFormat.doc,
                    AllowFileFormat.docx
                    );
            default://只確認一個附檔名而已
                return Result.上傳檔案格式錯誤;
        }
    }

    public static void outPutFileForDownLoad(FileInfo info, string fileName, string fileExt = "")
    {
        var format = (AllowFileFormat)Enum.Parse(typeof(AllowFileFormat), String.IsNullOrEmpty(fileExt) ? info.Extension.Replace(".", "") : fileExt.Replace(".", ""), false);
        outPutFileForDownLoad(info, fileName, format);
    }
    public static void outPutFileForDownLoad(FileInfo info, string fileName, AllowFileFormat fileFormat)
    {
        MemoryStream stream = new MemoryStream();
        info.OpenRead().CopyTo(stream);
        outPutFileForDownLoad(stream.ToArray(), fileName, fileFormat);
    }
    public static void outPutFileForDownLoad(MemoryStream stream, string fileName, AllowFileFormat fileFormat)
    {
        outPutFileForDownLoad(stream.ToArray(), fileName, fileFormat);
        stream.Close();
        stream.Dispose();
        stream = null;
    }
    public static void outPutFileForDownLoad(string path, string fileName, AllowFileFormat fileFormat)
    {
        outPutFileForDownLoad(File.ReadAllBytes(path), fileName, fileFormat);
    }
    public static void outPutFileForDownLoad(byte[] source, string fileName, AllowFileFormat fileFormat)
    {
        var context = HttpContext.Current;

        context.Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.{1}", fileName, fileFormat.ToString()));
        context.Response.AddHeader("Content-Length", source.Length.ToString());
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        switch (fileFormat)
        {
            case AllowFileFormat.png:
            case AllowFileFormat.jpg:
            case AllowFileFormat.jpeg:
            case AllowFileFormat.gif:
                context.Response.ContentType = ContentType.Image + fileName.ToString();
                break;
            case AllowFileFormat.xls:
            case AllowFileFormat.xlsx:
            case AllowFileFormat.csv:
                context.Response.ContentType = ContentType.Excel;
                break;
            case AllowFileFormat.doc:
            case AllowFileFormat.docx:
                context.Response.ContentType = ContentType.Word;
                break;
            case AllowFileFormat.pdf:
                context.Response.ContentType = ContentType.Pdf;
                break;
            default:
                context.Response.ContentType = ContentType.Txt;
                break;
        }

        HttpContext.Current.Response.BinaryWrite(source);
    }

    public static void SaveFileStream(String path, Stream stream)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
        }
    }
}