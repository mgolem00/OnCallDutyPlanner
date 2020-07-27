<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OnCallDutyPlanner.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Media/layout.css" rel="stylesheet" type="text/css" />
</head>

<body>
    <form id="form1" runat="server">
        <div class="topnav">
            <a id="logo">On-Call Duty Planner</a>
        </div>
        <div class="main">
            <div>
                <h4 style="font-size: medium">Log In</h4>
                <hr />
                <asp:PlaceHolder runat="server" ID="LoginStatus" Visible="false">
                    <p>
                        <asp:Literal runat="server" ID="StatusText" />
                    </p>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="LoginForm" Visible="false">
                    <div style="margin-bottom: 10px">
                        <asp:Label runat="server" AssociatedControlID="UserName">User name</asp:Label>
                        <div>
                            <asp:TextBox runat="server" ID="UserName" />
                        </div>
                    </div>
                    <div style="margin-bottom: 10px">
                        <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                        <div>
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" />
                        </div>
                    </div>
                    <div style="margin-bottom: 10px">
                        <div>
                            <asp:Button runat="server" OnClick="SignIn" Text="Log in" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="LogoutButton" Visible="false">
                    <div>
                        <div>
                            <asp:Button runat="server" OnClick="SignOut" Text="Log out" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="footer">
            <span style="float:right; text-align:right;">Made by: <a href="https://github.com/mgolem00" target="_blank" style="color:gray;">Marin Golem</a></span>
        </div>
    </form>
</body>
</html>
