<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Return.aspx.cs" Inherits="iParkingNet_MVC.Page.Return" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            wellcome back!!
            <br/>
            <h1><asp:Label ID="PayStatusText" runat="server" ></asp:Label></h1>
            <br/>
            <h2><asp:Label ID="ErrorMsg" runat="server"></asp:Label></h2>            
            <br/>
            <asp:LinkButton   ID="CancelBtn"  runat="server">>請點此返回<</asp:LinkButton>
        </div>
    </form>
</body>
</html>
