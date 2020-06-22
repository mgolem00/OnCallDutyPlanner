<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SLATeams.aspx.cs" Inherits="OnCallDutyPlanner.SLATeams" %>

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
                <a href="/Scheduler.aspx">Scheduler</a>
                <a href="Projects.aspx">Projects</a>
                <a href="AccDistr.aspx">Account Distribution</a>
                <a href="AccDistrConfig.aspx">Account Distribution Configuration</a>
                <a href="SLATeams.aspx" class="active">SLA Teams</a>
                <a href="Users.aspx">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:Button ID="OpenCreateTeamButton" runat="server" OnClick="OpenCreateTeam_Click" Text="Create new team" />
            <p>
                <asp:Literal runat="server" ID="CreateTeamLiteral" />
            </p>
            <asp:Panel ID="CreateTeamPanel" runat="server" Visible="false">
                <asp:Label runat="server" AssociatedControlID="TeamName">Team name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="TeamName" />
                </div>
                <asp:Label runat="server" AssociatedControlID="UserListBox">Users</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="UserListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>
                <div>
                    <asp:Button ID="CreateTeamButton" runat="server" OnClick="CreateTeam_Click" Text="Create" />
                    <asp:Button ID="CancelCreateTeamButton" runat="server" OnClick="CancelCreateTeam_Click" Text="Cancel" />
                </div>
            </asp:Panel>
            
            <hr/>

            <p>
                <asp:Literal runat="server" ID="ErrorEditLiteral" />
            </p>
            <p>
                <asp:Literal runat="server" ID="EditLiteral" />
            </p>
            <div>
                <asp:GridView ID="TeamsGridView" runat="server" AutoGenerateColumns="false" OnRowCommand="TeamsGridView_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Team name">
                            <ItemTemplate>
                                <asp:Label ID="lbl_TeamName" runat="server" Text='<%#Eval("lbl_TeamName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        
                        <asp:TemplateField HeaderText="Members">
                            <ItemTemplate>
                                <asp:Repeater ID="MembersRepeater" runat="server" DataSource='<%# Eval("lbl_Members") %>' >
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_Members" runat="server" Text='<%# Container.DataItem %>'></asp:Label><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ShowHeader="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="EditButton" Text="Edit" runat="server" CommandName="EditTeam" CommandArgument="<%# Container.DataItemIndex %>" />
                                <asp:LinkButton ID="DeleteButton" Text="Delete" runat="server" CommandName="DeleteTeam" CommandArgument="<%# Container.DataItemIndex %>" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ShowHeader="false" Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" Text="Delete? "></asp:Label>
                                <asp:LinkButton ID="YesDeleteButton" Text="Yes" runat="server" CommandName="YesDelete" CommandArgument="<%# Container.DataItemIndex %>" />
                                <asp:LinkButton ID="NoDeleteButton" Text="No" runat="server" CommandName="NoDelete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            
            <hr/>

            <asp:Panel ID="EditTeamPanel" runat="server" Visible="false">
                <asp:HiddenField ID="HiddenEditTeamName" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditRowIndex" runat="server" Visible="false" />
                <p>
                    <asp:Literal runat="server" ID="EditWarningLiteral" />
                </p>
                <asp:Label runat="server" AssociatedControlID="NewTeamNameTextBox">Team name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="NewTeamNameTextBox" />                
                </div>
                <asp:Label runat="server" AssociatedControlID="EditCurrentMembersListBox">Current Members (select to remove from team)</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="EditCurrentMembersListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>
                <asp:Label runat="server" AssociatedControlID="EditNonMembersListBox">Non Members (select to add to team)</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="EditNonMembersListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>

                <div>
                    <asp:Button ID="SaveEditButton" runat="server" OnClick="SaveEditTeam_Click" Text="Save" />
                    <asp:Button ID="ApplyEditButton" runat="server" OnClick="ApplyEditTeam_Click" Text="Apply" />
                    <asp:Button ID="CancelEditButton" runat="server" OnClick="CancelEditTeam_Click" Text="Cancel" />
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>