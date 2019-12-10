<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="Http_Gobang.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script type="text/javascript" src="chk5.js"></script>
    <script>
        var G;
        var S;
        var A;
        var Q;
        function init() {
            S = new XMLHttpRequest();
            G = C.getContext("2d");
            reset();
            setInterval("Listen()", 250);
        }
        function Listen() {
            if (document.getElementById("H").value == "") return;
            var q = document.getElementById("H").value;
            document.getElementById("H").value = "";
            if (q == "X") {
                reset();
            } else {
                var p = q.split(",");
                var x = parseInt(p[0]);
                var y = parseInt(p[1]);
                chess(x, y, -1);
            }
        }
        function reset() {
            G.clearRect(0, 0, 570, 570);
            A = new Array(19);
            for (var i = 0; i < 19; i++) {
                A[i] = new Array(19);
                for (var j = 0; j < 19; j++) {
                    A[i][j] = 0;
                }
            }
            Msg.innerHTML = "下棋嘍！"
            Q = true;
        }
        function plt(i, j, k) {
            var x = i * 30 + 15;
            var y = j * 30 + 15;
            G.beginPath();
            G.arc(x, y, 13, 0, Math.PI * 2, true);
            G.closePath();
            G.fillStyle = k;
            G.fill();
            G.stroke();
        }
        function md() {
            if (Q == false) return;
            var x = Math.round((event.offsetX - 15) / 30);
            var y = Math.round((event.offsetY - 15) / 30);
            if (A[x][y] != 0) return;
            url = "WebForm1.aspx?A=" + x + "," + y;
            S.open("GET", url, true);
            S.send();
            chess(x, y, 1);
        }
        function chess(x, y, st) {
            A[x][y] = st;
            switch (st) {
                case 1:
                    plt(x, y, "black");
                    Q = false;
                    if (chk5(x, y, 1)) {
                        Msg.innerHTML = "你贏了！！"
                    } else {
                        Msg.innerHTML = "換對手下...";
                    }
                    break;
                case -1:
                    plt(x, y, "white");
                    if (chk5(x, y, -1)) {
                        Msg.innerHTML = "你輸了！！"
                    } else {
                        Msg.innerHTML = "到你了...";
                        Q = true;
                    }
                    break;
            }
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
                <asp:Timer ID="Timer1" runat="server" Interval="250" OnTick="Timer1_Tick">
                </asp:Timer>
                <asp:HiddenField ID="H" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="Msg" style="width: 570px; text-align: center">
        </div>
        <div id="BD" style="background-image: url(&quot;bg.gif&quot;); width: 570px; height: 570px">
            <canvas id="C" width="570" height="570" onmousedown="md()"></canvas>
        </div>
        <p>
            我是：<asp:TextBox ID="TextBox1" runat="server" OnTextChanged="TextBox1_TextChanged"></asp:TextBox>
            &nbsp;在跟：<asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
            &nbsp;玩&nbsp; 
            <asp:Button ID="Button1" runat="server" Text="重玩" OnClick="Button1_Click" />
        </p>
    </form>
</body>
</html>
