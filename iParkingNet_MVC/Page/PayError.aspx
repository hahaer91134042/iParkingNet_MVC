<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayError.aspx.cs" Inherits="iParkingNet_MVC.Page.PayError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>付款過程出現錯誤</h3>
        </div>
        <div>很抱歉您的訂單</div>
        <div>編號:<asp:Label ID="NoLabel" runat="server" Text=""></asp:Label></div>
        <div>在支付過程中出現錯誤，請洽客服人員處理</div>
    </form>
</body>
</html>
