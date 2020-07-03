﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="OnCallDutyPlanner.Users" %>

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
                <a href="AccDistr.aspx">Account Distribution</a>
                <a href="Projects.aspx">Projects</a>
                <a href="AccDistrConfig.aspx">Account Distribution Configuration</a>
                <a href="SLATeams.aspx">SLA Teams</a>
                <a href="Users.aspx" class="active">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:Button ID="OpenCreateUserButton" runat="server" OnClick="OpenCreateUser_Click" Text="Create new user" />
            <p>
                <asp:Literal runat="server" ID="CreateUserLiteral" />
            </p>
            <asp:Panel ID="CreateUserPanel" runat="server" Visible="false">
                
                <asp:Label runat="server" AssociatedControlID="UserName">User name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="UserName" />                
                </div>
        
                <div>
                    <asp:Label runat="server" AssociatedControlID="Password">Password</asp:Label>
                </div>
                <div>
                    <asp:TextBox runat="server" ID="Password" TextMode="Password" />                
                </div>

                <asp:DropDownList ID="RoleDropDown" runat="server">
                    <asp:ListItem Text="Worker" Value="Worker" />
                    <asp:ListItem Text="Project Manager" Value="ProjectManager" />
                    <asp:ListItem Text="Admin" Value="Admin" />
                </asp:DropDownList>

                <div>
                    <asp:Button ID="CreateUserButton" runat="server" OnClick="CreateUser_Click" Text="Register" />
                    <asp:Button ID="CancelCreateUserButton" runat="server" OnClick="CancelCreateUser_Click" Text="Cancel" />
                </div>
            </asp:Panel>

            <hr/>

            <p>
                <asp:Literal runat="server" ID="ErrorEditLiteral" />
            </p>
            <p>
                <asp:Literal runat="server" ID="EditLiteral" />
            </p>
            <asp:GridView ID="UsersGridView" runat="server" AutoGenerateColumns="false" OnRowCommand="UsersGridView_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Username">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Username" runat="server" Text='<%#Eval("lbl_Username") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Role" runat="server" Text='<%#Eval("lbl_Role") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="SLATeam">
                        <ItemTemplate>
                            <asp:Label ID="lbl_SLATeam" runat="server" Text='<%#Eval("lbl_SLATeam") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <asp:LinkButton ID="EditButton" Text="Edit" runat="server" CommandName="EditUser" CommandArgument="<%# Container.DataItemIndex %>" />
                            <asp:LinkButton ID="DeleteButton" Text="Delete" runat="server" CommandName="DeleteUser" CommandArgument="<%# Container.DataItemIndex %>" />
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
            <hr/>

            <asp:Panel ID="EditUserPanel" runat="server" Visible="false">
                <asp:HiddenField ID="HiddenEditUsername" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditRole" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditRowIndex" runat="server" Visible="false" />
                <asp:Label runat="server" AssociatedControlID="NewUserNameTextBox">User name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="NewUserNameTextBox" />                
                </div>
        
                <div>
                    <asp:Label runat="server" AssociatedControlID="NewPasswordTextBox">New Password</asp:Label>
                </div>
                <div>
                    <asp:TextBox runat="server" ID="NewPasswordTextBox" TextMode="Password" />                
                </div>

                <asp:DropDownList ID="EditRoleDropDown" runat="server">
                    <asp:ListItem Text="Worker" Value="Worker" />
                    <asp:ListItem Text="Project Manager" Value="ProjectManager" />
                    <asp:ListItem Text="Admin" Value="Admin" />
                </asp:DropDownList>

                <div>
                    <asp:Button ID="SaveEditButton" runat="server" OnClick="SaveEditUser_Click" Text="Save" />
                    <asp:Button ID="ApplyEditButton" runat="server" OnClick="ApplyEditUser_Click" Text="Apply" />
                    <asp:Button ID="CancelEditButton" runat="server" OnClick="CancelEditUser_Click" Text="Cancel" />
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>