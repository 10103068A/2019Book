<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Http_WebDraw.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript">
        var draw = false;
        var G;
        var Q;
        var S;
        function init() {
            S = new XMLHttpRequest();
            G = m.getContext("2d");
            Listen();
        }
        function Listen() {
            if (document.getElementById("H").value != "") {
                var z = document.getElementById("H").value.split("/");
                document.getElementById("H").value = "";
                var p = z[0].split(",");
                G.moveTo(p[0], p[1]);
                for (var i = 1; i < z.length - 1; i++) {
                    var q = z[i].split(",");
                    G.lineTo(q[0], q[1]);
                }
                G.stroke();
            }
            setTimeout("Listen()", 200);
        }
        function md() {
            G.moveTo(event.offsetX, event.offsetY);
            draw = true;
            Q = event.offsetX + "," + event.offsetY + "/";
        }
        function mv() {
            if (draw) {
                G.lineTo(event.offsetX, event.offsetY);
                G.stroke();
                Q += event.offsetX + "," + event.offsetY + "/";
            }
        }
        function mup() {
            draw = false;
            var url = "WebForm1.aspx?A=" + Q;
            S.open("GET", url, true);
            S.send();
        }
    </script>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body onload="init()">
    <form id="form1" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Timer ID="Timer1" runat="server" Interval="500" OnTick="Timer1_Tick">
                    </asp:Timer>
                    <asp:HiddenField ID="H" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
        <p>
            我是：<asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            &nbsp; 畫給：<asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
        &nbsp;看</p>
    </form>
    <canvas id="m" width="400" height="300" onmousedown="md()" onmousemove="mv()" onmouseup="mup()" style="border: thin solid #000000"></canvas>
</body>
</html>
