<%@ Control Language="C#" AutoEventWireup="true"  CodeBehind="SearchByPublisher.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.SearchByPublisher" %>
<asp:Panel runat="server" ID="pnlSearchByPublisher" CssClass="nwtd-SearchByPublisher" DefaultButton="btnSubmitSearch">
	<asp:Label runat="server" ID="lblearch" AssociatedControlID="tbKeyWord" Text="Search:" />
	<asp:DropDownList  ID="ddlPublisher" runat="server" DataTextField="Name" DataValueField="Key" />
	<asp:TextBox ID="tbKeyWord" runat="server" CssClass="nwtd-search-keyword" />
	<asp:Button ID="btnSubmitSearch" runat="server" onclick="btnSubmitSearch_Click" CssClass="search-btn" Text="Go"  />
	<asp:HyperLink runat="server" Visible="false" Text="Advanced Search"  ID="hlAdvancedSearch" NavigateUrl="~/catalog/advancedsearch.aspx"  />
</asp:Panel>