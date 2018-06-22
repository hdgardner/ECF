<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Profile.AccountList" Codebehind="AccountList.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<core:EcfListViewControl id="MyListView" runat="server" DataSourceID="AccountsDataSource" AppId="Profile" ViewId="Account-List" ShowTopToolbar="true"></core:EcfListViewControl>
<profile:ProfileSearchDataSource runat="server" ID="AccountsDataSource" SourceType="MembershipProvider">
</profile:ProfileSearchDataSource>