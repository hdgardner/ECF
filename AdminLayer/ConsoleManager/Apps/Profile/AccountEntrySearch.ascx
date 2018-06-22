<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountEntrySearch.ascx.cs" Inherits="Mediachase.Commerce.Manager.Apps.Profile.AccountEntrySearch" %>
<%@ Register Src="../Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl"
    TagPrefix="core" %>
<IbnWebControls:McDock ID="DockTop" runat="server" Anchor="Top" EnableSplitter="False"
    DefaultSize="132">
    <DockItems>
		<asp:Panel runat="server" ID="pnlMain" DefaultButton="btnSearch" Height="130px" BackColor="#F8F8F8"
            BorderColor="Gray" BorderWidth="0">
            <div id="DataForm">
                <table cellpadding="4" style="background-color: #F8F8F8;">
                    <tr>
                        <td class="FormLabelCell">
                            <b><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:ProfileStrings, Search_By_User_Name %>"/>:</b>
                        </td>
                        <td class="FormFieldCell">
                            <asp:TextBox ID="tbName" Width="240" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="upSearchButton" ChildrenAsTriggers="true" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="btnSearch" runat="server" Width="100" Text="<%$ Resources:SharedStrings, Search %>" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" class="FormSectionCell">
                            <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:ProfileStrings, Additional_Filters %>"/>
                        </td>
                    </tr>
                    <%--<tr>
                        <td class="FormLabelCell">
                            <asp:Literal runat="server" Text="<%$ Resources:ProfileStrings, Search_By_User_Name %>"/>:
                        </td>
                        <td class="FormFieldCell" colspan="2">
                            <asp:TextBox runat="server" ID="tbUserName" Width="240"></asp:TextBox>
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:ProfileStrings, Filter_By_Organization_s %>"/>:
                        </td>
                        <td class="FormFieldCell" colspan="2">
                            <asp:DropDownList runat="server" ID="ddlOrganizations" Width="240" DataValueField="OrganizationId"
                                DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabelCell">
                            <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:ProfileStrings, Search_By_Address %>"/>:
                        </td>
                        <td class="FormFieldCell" colspan="2">
                            <asp:TextBox ID="tbAddress" Width="240" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </DockItems>
</IbnWebControls:McDock>
<core:EcfListViewControl id="MyListView" runat="server" DataKey="PrincipalId" AppId="Profile" ViewId="Account-SearchList" ShowTopToolbar="true"  >
</core:EcfListViewControl>
<profile:ProfileSearchDataSource runat="server" ID="AccountsDataSource" SourceType="ExtendedProfileSystem">
</profile:ProfileSearchDataSource>