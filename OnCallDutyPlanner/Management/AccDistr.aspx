<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccDistr.aspx.cs" Inherits="OnCallDutyPlanner.AccDistr" %>

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
            <a id="welcome"><asp:Label ID="Welcome" runat="server"></asp:Label></a>

            <div class="topnav-right">
                <a href="/Scheduler.aspx">Time Planner</a>
                <a href="AccDistr.aspx" class="active" id="AccDistrLink" runat="server">Account Distribution</a>
                <a href="Projects.aspx" id="ProjectsLink" runat="server">Projects</a>
                <a href="AccDistrConfig.aspx" id="AccDistrConfigLink" runat="server">Account Distribution Configuration</a>
                <a href="SLATeams.aspx" id="SLATeamsLink" runat="server">SLA Teams</a>
                <a href="Users.aspx" id="UsersLink" runat="server">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:Panel ID="ChoosePanel" runat="server">
                Choose a Month:<asp:DropDownList ID="DropDownMonth" runat="server"></asp:DropDownList>
                Choose a Year:<asp:DropDownList ID="DropDownYear" runat="server"></asp:DropDownList>
                <asp:Button id="SearchButton" Text="Select" OnClick="SelectBtnClick" runat="server"/>
            </asp:Panel>
            <br />
            <asp:Panel runat="server" ID="GridViewsPanel"></asp:Panel>
        </div>

        <div class="footer">
            <span style="float:left; text-align:left;">How to use: <a href="#" target="_blank" style="color:gray;">Manual</a></span>
            <span style="float:right; text-align:right;">Made by: <a href="https://github.com/mgolem00" target="_blank" style="color:gray;">Marin Golem</a></span>
        </div>
    </form>
</body>
</html>
