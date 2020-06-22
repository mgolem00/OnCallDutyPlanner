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
            <a id="logo" href="#">On-Call Duty Planner</a>
            <a id="welcome"><asp:Label ID="Welcome" runat="server"></asp:Label></a>

            <div class="topnav-right">
                <a href="/Scheduler.aspx">Scheduler</a>
                <a href="Projects.aspx">Projects</a>
                <a href="AccDistr.aspx" class="active">Account Distribution</a>
                <a href="AccDistrConfig.aspx">Account Distribution Configuration</a>
                <a href="SLATeams.aspx">SLA Teams</a>
                <a href="Users.aspx">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:Panel runat="server" ID="GridViewsPanel"></asp:Panel>
        </div>
    </form>
</body>
</html>
