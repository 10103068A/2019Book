<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Http_WebChat.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Timer ID="Timer1" runat="server" Interval="2000" OnTick="Timer1_Tick">
                    </asp:Timer>
                    <br />
                    線上名單<br /> 
                    <asp:ListBox ID="ListBox1" runat="server" Width="200px"></asp:ListBox>
                    <br />
                    聊天看板<br /> 
                    <asp:TextBox ID="TextBox1" runat="server" Rows="6" TextMode="MultiLine"></asp:TextBox>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            我是：<asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="上線" />
            <br />
            <br />
            想說：<asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
&nbsp;<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="發言" />
        </div>
    </form>
</body>
</html>
