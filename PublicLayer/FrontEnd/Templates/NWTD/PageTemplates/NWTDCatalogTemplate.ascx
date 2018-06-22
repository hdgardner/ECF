<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NWTDCatalogTemplate.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.PageTemplates.NWTDCatalogTemplate" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchByPublisher.ascx" TagName="SearchBar" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/NWTDHeader.ascx" TagName="Header" TagPrefix="NWTD" %>
<%@ Register src="../Menu/Default.ascx" tagname="Default" tagprefix="menu" %>
<%@ Register Src="~/Templates/NWTD/FooterModule.ascx" TagName="Footer" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/Base/Controls/Menu/Breadcrumb.ascx" TagPrefix="cms" TagName="Breadcrumb" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"
    TagPrefix="cms" %>
<div id="nwtd-page" class="nwtd-catalogBrowsePage">
	<div id="nwtd-pageHeader">
		<NWTD:Header runat="server" ID="nwtdheader" />
		<div id="nwtd-Menu">
			<menu:Default ID="Default1" runat="server" />
		</div>
		<div id="nwtd-submenu" style="clear:both;">
			<div class="nwtd-loginMenu">
				<cms:ThemedControlModule ID="ThemedControlModule1" EnableViewState="false" ThemePath="UserStatusModule.ascx" runat="server" />
			</div>
		</div>
	</div>
	
	<div id="nwtd-pageContent" class="nwtd-defaultPageContent">
		<div id="nwtd-breadrumb">
			<cms:Breadcrumb ID="Breadcrumb1" runat="server" EnableViewState="false" />
		</div>
		<NWTD:SearchBar runat="server" ID="searchBar" ShowAdvancedSearch="false" />
		<%--<div id="nwtd-searchLink">
		    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/catalog/searchresults.aspx">Search our Catalog</asp:HyperLink>
		</div>--%>
		<div id="print-logo"><img id="Img1" src="~/Images/print-logo.png" runat="server" /></div>
		<asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/top-curve.png" cssclass="curves" />
		<cms:CmsPlaceHolder runat="server" ID="MainContentArea" CssClass="MainContentArea" />
		<asp:Image ID="Image2" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/bottom-curve.png" cssclass="curves" />
	    <br style="clear:both;" />
	    <NWTD:Footer runat="server" ID="nwtdfooter" />
	</div>
	

</div>
