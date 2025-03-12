using Jose;
using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace DevLibs
{

    #region ---Log---
    public class Log
    {
        //private static string logDirPath = Environment.CurrentDirectory+"\\log";
        //private static string fileName = "log_{0}.txt";

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void d(string msg, Exception e = null)
        {
            if (e == null)
                logger.Debug(msg);
            else
                logger.Debug($"{msg}->{e}");
        }

        public static void i(string msg, Exception e = null)
        {
            if (e == null)
                logger.Info(msg);
            else
                logger.Info($"{msg}->{e}");
        }

        public static void e(string msg, Exception e = null)
        {
            if (e == null)
            {
                logger.Error(msg);
            }
            else
            {
                logger.Error($"{msg}->{e}");
            }
        }
    }
    #endregion

    #region ---ResponseBuildr---
    public class ResponseBuilder
    {
        private const string flag = "Hill";
        //private Dictionary<string, object> msgList = new Dictionary<string, object>();
        private List<KeyValuePair<String, object>> msgList = new List<KeyValuePair<String, object>>();
        public static ResponseBuilder getInstance()
        {
            return new ResponseBuilder();
        }
        public ResponseBuilder Append(string key, object msg)
        {
            msgList.Add(new KeyValuePair<string, object>(key, msg));
            //msgList.Add(key, msg);
            return this;
        }
        public ResponseBuilder Append(object msg)
        {
            msgList.Add(new KeyValuePair<string, object>(flag, msg));
            //msgList.Add(flag, msg);
            return this;
        }
        public ResponseBuilder PrintContent(object obj)
        {
            var type = obj.GetType();
            Append("--> Object---", type.Name + " <--");
            if (type.GetProperties().Length > 0)
                parseObjProperty(1, obj);
            if (type.GetFields().Length > 0)
                parseObjField(1, obj);
            return this;
        }

        private void parseObjField(int lv, object obj)
        {
            Append("-->Lv_" + lv + "  Fields Object  ", obj.GetType().Name + "<--");
            foreach (var field in obj.GetType().GetFields())
            {
                try
                {
                    var value = field.GetValue(obj);
                    Append("Lv_" + lv + " field - " + field.Name, value);
                    if (value.GetType().GetProperties().Length > 0)
                    {
                        if (value.GetType() != typeof(string))
                            parseObjField(lv + 1, value);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void parseObjProperty(int lv, object obj)
        {
            Append("-->Lv_" + lv + " Properties Object  ", obj.GetType().Name + "<--");
            foreach (var property in obj.GetType().GetProperties())
            {
                try
                {
                    var value = property.GetValue(obj, null);
                    Append("Lv_" + lv + " property - " + property.Name, value);
                    if (value.GetType().GetProperties().Length > 0)
                    {
                        if (value.GetType() != typeof(string))
                            parseObjProperty(lv + 1, value);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        public ResponseBuilder print()
        {
            foreach (var pair in msgList)
            {
                // null object and DBNull value can`t print "null"
                if (pair.Value == null)
                {
                    HttpContext.Current.Response.Write(pair.Key + " -> object null<br/>");
                }
                else if (pair.Value == DBNull.Value)
                {
                    HttpContext.Current.Response.Write(pair.Key + " -> DBNull<br/>");
                }
                else
                {
                    HttpContext.Current.Response.Write(pair.Key + " -> " + pair.Value + "<br/>");
                }
            }
            return this;
        }
        public void end()
        {
            HttpContext.Current.Response.End();
        }
    }
    #endregion

    #region ---EzScript---
    public class EzScript
    {
        public static EzScript New()
        {
            return new EzScript();
        }

        private StringBuilder script = new StringBuilder();

        public EzScript addDatePicker(string ID, string format = "yy-mm-dd")
        {
            script.AppendLine("$(\"#" + ID + "\").datepicker({ dateFormat: \"" + format + "\" });");
            return this;
        }

        public void Into(Literal literal)
        {
            literal.Text = "<script type=\"text/javascript\" language=\"javascript\">" +
                    "  $(function () {" +
                    script.ToString() +
                    "  });" +
                    "</script>";
        }
    }
    #endregion

    #region ---Utils---

    //#region ---Excel Util--- 需使用NOPI
    //public class ExcelUtil
    //{
    //    public static Stream RenderDataTableToExcel(DataTable SourceTable)
    //    {
    //        HSSFWorkbook workbook = new HSSFWorkbook();
    //        MemoryStream ms = new MemoryStream();
    //        HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;
    //        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;

    //        // handling header. 
    //        foreach (DataColumn column in SourceTable.Columns)
    //            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

    //        // handling value. 
    //        int rowIndex = 1;

    //        foreach (DataRow row in SourceTable.Rows)
    //        {
    //            HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;

    //            foreach (DataColumn column in SourceTable.Columns)
    //            {
    //                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
    //            }

    //            rowIndex++;
    //        }

    //        workbook.Write(ms);
    //        ms.Flush();
    //        ms.Position = 0;

    //        sheet = null;
    //        headerRow = null;
    //        workbook = null;

    //        return ms;
    //    }
    //    //直接輸出成一個檔案
    //    public static void RenderDataTableToExcel(DataTable SourceTable, string FileName)
    //    {
    //        MemoryStream ms = RenderDataTableToExcel(SourceTable) as MemoryStream;
    //        FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
    //        byte[] data = ms.ToArray();

    //        fs.Write(data, 0, data.Length);
    //        fs.Flush();
    //        fs.Close();

    //        data = null;
    //        ms = null;
    //        fs = null;
    //    }
    //    //將upload stream轉乘DataTable
    //    public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
    //    {
    //        HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
    //        HSSFSheet sheet = workbook.GetSheet(SheetName) as HSSFSheet;

    //        DataTable table = new DataTable();

    //        HSSFRow headerRow = sheet.GetRow(HeaderRowIndex) as HSSFRow;
    //        int cellCount = headerRow.LastCellNum;

    //        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
    //        {
    //            DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
    //            table.Columns.Add(column);
    //        }

    //        int rowCount = sheet.LastRowNum;

    //        for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
    //        {
    //            HSSFRow row = sheet.GetRow(i) as HSSFRow;
    //            DataRow dataRow = table.NewRow();

    //            for (int j = row.FirstCellNum; j < cellCount; j++)
    //                dataRow[j] = row.GetCell(j).ToString();
    //        }

    //        ExcelFileStream.Close();
    //        workbook = null;
    //        sheet = null;
    //        return table;
    //    }
    //    public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
    //    {
    //        HSSFWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
    //        HSSFSheet sheet = workbook.GetSheetAt(SheetIndex) as HSSFSheet;

    //        DataTable table = new DataTable();

    //        HSSFRow headerRow = sheet.GetRow(HeaderRowIndex) as HSSFRow;
    //        int cellCount = headerRow.LastCellNum;

    //        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
    //        {
    //            DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
    //            table.Columns.Add(column);
    //        }

    //        int rowCount = sheet.LastRowNum;

    //        for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
    //        {
    //            HSSFRow row = sheet.GetRow(i) as HSSFRow;
    //            DataRow dataRow = table.NewRow();

    //            for (int j = row.FirstCellNum; j < cellCount; j++)
    //            {
    //                if (row.GetCell(j) != null)
    //                    dataRow[j] = row.GetCell(j).ToString();
    //            }

    //            table.Rows.Add(dataRow);
    //        }

    //        ExcelFileStream.Close();
    //        workbook = null;
    //        sheet = null;
    //        return table;
    //    }
    //}
    //#endregion

    #endregion
}

//context.Response.AddHeader("content-disposition", "attachment;filename=" + ((context.Request.Browser.Browser == "InternetExplorer" || context.Request.Browser.Browser == "IE") ? context.Server.UrlEncode(vsDt.Rows[0][vsDescCol].ToString().Replace(" ", "_")) : vsDt.Rows[0][vsDescCol].ToString().Replace(" ", "_")) + vsFi.Extension);
//context.Response.ContentEncoding = System.Text.Encoding.UTF8;
//                        switch (vsFi.Extension.ToLower())
//                        {
//                            case ".zip":
//                                context.Response.ContentType = "application/x-zip-compressed";
//                                break;
//                            case ".pdf":
//                                context.Response.ContentType = "application/pdf";
//                                break;
//                            case ".csv":
//                                context.Response.ContentType = "application/csv";
//                                break;
//                            case ".doc":
//                                context.Response.ContentType = "application/doc";
//                                break;
//                            case ".docx":
//                                context.Response.ContentType = "application/docx";
//                                break;
//                            case ".xls":
//                                context.Response.ContentType = "application/xls";
//                                break;
//                            case ".xlsx":
//                                context.Response.ContentType = "application/xlsx";
//                                break;
//                            case ".png":
//                                context.Response.ContentType = "application/png";
//                                break;
//                            case ".gif":
//                                context.Response.ContentType = "application/gif";
//                                break;
//                            case ".jpg":
//                                context.Response.ContentType = "application/jpg";
//                                break;
//                            case ".txt":
//                                context.Response.ContentType = "application/txt";
//                                break;
//                            default:
//                                context.Response.ContentType = "text/plain";
//                                break;
//                        }