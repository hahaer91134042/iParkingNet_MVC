using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// ImgUtil 的摘要描述
/// </summary>
public class ImgUtil
{

    public static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
                return codec;
        }
        return null;
    }

    public static Bitmap ScaleImage(Bitmap pBmp, int pWidth, int pHeight)
    {
        try
        {
            Bitmap tmpBmp = new Bitmap(pWidth, pHeight);
            Graphics tmpG = Graphics.FromImage(tmpBmp);

            //tmpG.InterpolationMode = InterpolationMode.HighQualityBicubic;

            tmpG.DrawImage(pBmp,
                                       new Rectangle(0, 0, pWidth, pHeight),
                                       new Rectangle(0, 0, pBmp.Width, pBmp.Height),
                                       GraphicsUnit.Pixel);
            tmpG.Dispose();
            return tmpBmp;
        }
        catch
        {
            return null;
        }
    }

    public static Bitmap ZoomImage(Bitmap bitmap, ImgSizeType imgSize)
    {
        return ZoomImage(bitmap, imgSize.height, imgSize.width);
    }

    public static Bitmap ZoomImage(Bitmap bitmap, int destHeight, int destWidth)
    {
        try
        {
            System.Drawing.Image sourImage = bitmap;
            int width = 0, height = 0;
            //按比例缩放           
            int sourWidth = sourImage.Width;
            int sourHeight = sourImage.Height;
            if (sourHeight > destHeight || sourWidth > destWidth)
            {
                if ((sourWidth * destHeight) > (sourHeight * destWidth))
                {
                    width = destWidth;
                    height = (destWidth * sourHeight) / sourWidth;
                }
                else
                {
                    height = destHeight;
                    width = (sourWidth * destHeight) / sourHeight;
                }
            }
            else
            {
                width = sourWidth;
                height = sourHeight;
            }
            Bitmap destBitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(destBitmap);
            g.Clear(System.Drawing.Color.Transparent);
            //设置画布的描绘质量         
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(sourImage,
                new Rectangle(0, 0, width, height),
                new Rectangle(0, 0, sourImage.Width, sourImage.Height),
                GraphicsUnit.Pixel);
            g.Dispose();
            //设置压缩质量     
            //System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
            //long[] quality = new long[1];
            //quality[0] = 100;
            //System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            //encoderParams.Param[0] = encoderParam;
            sourImage.Dispose();
            return destBitmap;
        }
        catch
        {
            return bitmap;
        }
    }

    /// <summary>
    /// 將圖片Image轉換成Byte[]
    /// </summary>
    /// <param name="Image">image物件</param>
    /// <param name="imageFormat">字尾名</param>
    /// <returns></returns>
    public static byte[] ImageToBytes(System.Drawing.Image Image, System.Drawing.Imaging.ImageFormat imageFormat)
    {

        if (Image == null) { return null; }

        byte[] data = null;

        using (MemoryStream ms = new MemoryStream())
        {

            using (Bitmap Bitmap = new Bitmap(Image))
            {

                Bitmap.Save(ms, imageFormat);

                ms.Position = 0;

                data = new byte[ms.Length];

                ms.Read(data, 0, Convert.ToInt32(ms.Length));

                ms.Flush();

            }
        }
        return data;
    }

    /// <summary>
    /// byte[]轉換成Image
    /// </summary>
    /// <param name="byteArrayIn">二進位制圖片流</param>
    /// <returns>Image</returns>
    public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
    {
        if (byteArrayIn == null)
            return null;
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArrayIn))
        {
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            ms.Flush();
            return returnImage;
        }
    }



    //byte[] 轉換 Bitmap
    public static Bitmap BytesToBitmap(byte[] Bytes)
    {
        MemoryStream stream = null;
        try
        {
            stream = new MemoryStream(Bytes);
            return new Bitmap((System.Drawing.Image)new Bitmap(stream));
        }
        catch (ArgumentNullException ex)
        {
            throw ex;
        }
        catch (ArgumentException ex)
        {
            throw ex;
        }
        finally
        {
            stream.Close();
        }
    }

    //Bitmap轉byte[]  
    public static byte[] BitmapToBytes(Bitmap Bitmap)
    {
        MemoryStream ms = null;
        try
        {
            ms = new MemoryStream();
            Bitmap.Save(ms, Bitmap.RawFormat);
            byte[] byteImage = new Byte[ms.Length];
            byteImage = ms.ToArray();
            return byteImage;
        }
        catch (ArgumentNullException ex)
        {
            throw ex;
        }
        finally
        {
            ms.Close();
        }
    }

    public static void SaveBitmap(Bitmap source, string filePath, string extension, long quality = 100L)
    {
        var eps = new EncoderParameters(1);
        eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

        var codecInfo = GetEncoder(FormatParser.Parse(extension));
        //var codecInfo = GetEncoder(source.RawFormat);
        source.Save(filePath, codecInfo, eps);
        source.Dispose();
    }

    internal class FormatParser
    {
        public static ImageFormat Parse(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Bmp;
            }
        }
    }
}