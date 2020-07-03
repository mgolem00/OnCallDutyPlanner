<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="OnCallDutyPlanner.Projects" %>

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
                <a href="Projects.aspx" class="active">Projects</a>
                <a href="AccDistrConfig.aspx">Account Distribution Configuration</a>
                <a href="SLATeams.aspx">SLA Teams</a>
                <a href="Users.aspx">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:Button ID="OpenCreateProjectButton" runat="server" OnClick="OpenCreateProject_Click" Text="Create new project" />
            <p>
                <asp:Literal runat="server" ID="CreateProjectLiteral" />
            </p>
            <asp:Panel ID="CreateProjectPanel" runat="server" Visible="false">
                <asp:Label runat="server" AssociatedControlID="ProjectName">Project name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="ProjectName" />
                </div>
                <asp:Label runat="server" AssociatedControlID="ProjectUID">Project Unique ID</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="ProjectUID" />
                </div>
                <asp:Label runat="server" AssociatedControlID="ProjectDesc">Project description</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="ProjectDesc" />
                </div>
                <asp:Label runat="server" AssociatedControlID="TeamsListBox">Teams</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="TeamsListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>
                <div>
                    <asp:Button ID="CreateProjectButton" runat="server" OnClick="CreateProject_Click" Text="Create" />
                    <asp:Button ID="CancelCreateProjectButton" runat="server" OnClick="CancelCreateProject_Click" Text="Cancel" />
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
                <asp:GridView ID="ProjectsGridView" runat="server" AutoGenerateColumns="false" OnRowCommand="ProjectsGridView_RowCommand" OnRowDataBound="ProjectsGridView_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Project name">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ProjectName" runat="server" Text='<%#Eval("lbl_ProjectName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Project Uniqued ID">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ProjectUID" runat="server" Text='<%#Eval("lbl_ProjectUID") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Description">
                            <ItemTemplate>
                                <asp:Label ID="lbl_ProjectDesc" runat="server" Text='<%#Eval("lbl_ProjectDesc") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField HeaderText="Working teams">
                            <ItemTemplate>
                                <asp:Repeater ID="TeamsRepeater" runat="server" DataSource='<%# Eval("lbl_Teams") %>' >
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_Teams" runat="server" Text='<%# Container.DataItem %>'></asp:Label><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Past teams">
                            <ItemTemplate>
                                <asp:Repeater ID="TeamsRepeater" runat="server" DataSource='<%# Eval("lbl_PastTeams") %>' >
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_PastTeams" runat="server" Text='<%# Container.DataItem %>'></asp:Label><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Date created">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DateCreated" runat="server" Text='<%#Eval("lbl_DateCreated") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Date finished">
                            <ItemTemplate>
                                <asp:Label ID="lbl_DateFinished" runat="server" Text='<%#Eval("lbl_DateFinished") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:TemplateField ShowHeader="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="EditButton" Text="Edit" runat="server" CommandName="EditProject" CommandArgument="<%# Container.DataItemIndex %>" />
                                <asp:LinkButton ID="EndButton" Text="End" runat="server" CommandName="EndProject" CommandArgument="<%# Container.DataItemIndex %>" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ShowHeader="false" Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" Text="Project completed? "></asp:Label>
                                <asp:LinkButton ID="YesEndButton" Text="Yes" runat="server" CommandName="YesEnd" CommandArgument="<%# Container.DataItemIndex %>" />
                                <asp:LinkButton ID="NoEndButton" Text="No" runat="server" CommandName="NoEnd" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            
            <hr/>
            <asp:Panel ID="EditProjectPanel" runat="server" Visible="false">
                <asp:HiddenField ID="HiddenEditProjectName" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditProjectUID" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditProjectDesc" runat="server" Visible="false" />
                <asp:HiddenField ID="HiddenEditRowIndex" runat="server" Visible="false" />
                <p>
                    <asp:Literal runat="server" ID="EditWarningLiteral" />
                </p>
                <asp:Label runat="server" AssociatedControlID="NewProjectNameTextBox">Project name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="NewProjectNameTextBox" />                
                </div>
                <asp:Label runat="server" AssociatedControlID="NewProjectUIDTextBox">Project name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="NewProjectUIDTextBox" />                
                </div>
                <asp:Label runat="server" AssociatedControlID="NewProjectDescTextBox">Project name</asp:Label>
                <div>
                    <asp:TextBox runat="server" ID="NewProjectDescTextBox" />                
                </div>
                <asp:Label runat="server" AssociatedControlID="EditCurrentTeamsListBox">Current teams (select to remove from project)</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="EditCurrentTeamsListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>
                <asp:Label runat="server" AssociatedControlID="EditAvailableTeamsListBox">Available teams (select to add to project)</asp:Label>
                <div>
                    <asp:ListBox runat="server" ID="EditAvailableTeamsListBox" SelectionMode="Multiple"></asp:ListBox>
                </div>

                <div>
                    <asp:Button ID="SaveEditButton" runat="server" OnClick="SaveEditProject_Click" Text="Save" />
                    <asp:Button ID="ApplyEditButton" runat="server" OnClick="ApplyEditProject_Click" Text="Apply" />
                    <asp:Button ID="CancelEditButton" runat="server" OnClick="CancelEditProject_Click" Text="Cancel" />
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
