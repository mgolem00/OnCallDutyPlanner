<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccDistrConfig.aspx.cs" Inherits="OnCallDutyPlanner.Management.AccDistrConfig" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
                <a href="AccDistrConfig.aspx" class="active">Account Distribution Configuration</a>
                <a href="SLATeams.aspx">SLA Teams</a>
                <a href="Users.aspx">Users</a>
                <a id="logout" runat="server" onserverclick="SignOut">Log out</a>
            </div>
        </div>
        <div class="main">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Button runat="server" ID="modalPlaceholderButton" style="display:none" />
                    <asp:Label ID="lbl_warningChooseTeamDDL" runat="server" Text="You must choose a team first!" ForeColor="Red" Visible="false"></asp:Label><br />
                    <asp:Label runat="server" Text="Create an Account Distribution for:"></asp:Label>
                    <asp:DropDownList runat="server" ID="ChooseTeamDDL"></asp:DropDownList>
                    <asp:Button runat="server" ID="OpenCreateAccDistrConfigButton" OnClick="OpenCreateAccDistrConfigButton_Click" Text="Create New Account Distribution" /><br />
                    <asp:Label ID="lbl_SuccessCreate" runat="server" Text="Account Distribution succesfully created!" ForeColor="Green" Visible="false"></asp:Label>
                    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" ClientIDMode="Static" TargetControlID="modalPlaceholderButton" PopupControlID="PopupCreateAccDistrConfigPanel" CancelControlID="CancelCreateAccDistrConfigButton" OkControlID="modalPlaceholderButton" BackgroundCssClass="modalPopupBackground"></asp:ModalPopupExtender>
                    <asp:Panel runat="server" ID="PopupCreateAccDistrConfigPanel" CssClass="popup">
                        <asp:Label runat="server" ID="SumWarningLabel" Text="The sum of all percentages needs to be 100!" ForeColor="Red" Visible="false"></asp:Label><br />
                        <asp:Label runat="server" ID="ErrorDDLTxtBoxLabel" ForeColor="Red" Visible="false"></asp:Label><br />
                        <asp:UpdatePanel runat="server" ID="AddAccDistrUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <asp:GridView ID="AccountsListGridView" runat="server" OnRowDataBound="AccountsListGridView_RowDataBound" OnRowCommand="AccountsListGridView_RowCommand" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Percentage">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="percentTxtBox" OnTextChanged="percentTxtBox_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Account">
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlAccount" OnSelectedIndexChanged="ddlAccount_IndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="RemoveRowBtn" Text="Remove" CommandName="RemoveRow" CommandArgument="<%# Container.DataItemIndex %>" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="AddAccountButton" EventName="Click"/>
                            </Triggers>
                        </asp:UpdatePanel><br />
                        <asp:Button runat="server" ID="AddAccountButton" OnClick="AddAccountButton_Click" Text="Add"/><br /><br />
                        <asp:Label runat="server" ID="AccountDistributionStartDateLabel"></asp:Label><br />
                        <asp:Label runat="server" Text="Caution! If there already is an Account Distribution for the next month it will be overwritten!" ForeColor="Red"></asp:Label><br />
                        <asp:Button ID="CreateAccDistrConfigButton" runat="server" Text="Create" OnClick="CreateAccDistrConfigButton_Click"/>
                        <asp:Button ID="CancelCreateAccDistrConfigButton" runat="server" Text="Cancel" />
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="OpenCreateAccDistrConfigButton" eventname="Click" />
                </Triggers>
            </asp:UpdatePanel> <br />

            <hr/><br />
            <asp:Panel ID="ChoosePanel" runat="server">
                Choose a Month:<asp:DropDownList ID="DropDownMonth" runat="server"></asp:DropDownList>
                Choose a Year:<asp:DropDownList ID="DropDownYear" runat="server"></asp:DropDownList>
                <asp:Button id="SearchButton" Text="Select" OnClick="SelectBtnClick" runat="server"/>
            </asp:Panel>

            <br />

            <asp:Panel runat="server" ID="GridViewsPanel"></asp:Panel>
        </div>
    </form>

    <%--
    <script type="text/javascript">
        function ShowModalPopup() {
            $find("ModalPopupExtender1").show();
        }
        function HideModalPopup() {
            $find("ModalPopupExtender1").hide();
        }
    </script>--%>
</body>
</html>
