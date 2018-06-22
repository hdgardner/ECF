<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Profile.OrganizationList" Codebehind="OrganizationList.ascx.cs" %>
<%@ Register Src="~/Apps/Core/Controls/EcfListViewControl.ascx" TagName="EcfListViewControl" TagPrefix="core" %>
<core:EcfListViewControl id="MyListView" runat="server" DataSourceID="OrganizationsDataSource" DataKey="PrincipalId" AppId="Profile" ViewId="Organization-List" ShowTopToolbar="true"></core:EcfListViewControl>
<profile:OrganizationSearchDataSource runat="server" ID="OrganizationsDataSource"></profile:OrganizationSearchDataSource>