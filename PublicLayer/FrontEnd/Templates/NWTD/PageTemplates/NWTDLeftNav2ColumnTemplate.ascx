<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NWTDLeftNav2ColumnTemplate.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.PageTemplates.NWTDLeftNav2ColumnTemplate" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchByPublisher.ascx" TagName="SearchBar" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/NWTDHeader.ascx" TagName="Header" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/FooterModule.ascx" TagName="Footer" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Menu/Breadcrumb.ascx" TagPrefix="NWTD" TagName="Breadcrumb" %>
<%@ Register src="../Menu/Default.ascx" tagname="Default" tagprefix="menu" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"
    TagPrefix="cms" %>
<%@ Register src="../SearchLink.ascx" tagname="SearchLink" tagprefix="NWTD" %>
<div id="nwtd-page" class="nwtd-defaultPage">
	<div id="nwtd-pageHeader">
		<NWTD:Header runat="server" ID="nwtdheader" />
		<div id="nwtd-submenu" style="clear:both;">
			<div class="nwtd-loginMenu">
				<cms:ThemedControlModule ID="ThemedControlModule1" EnableViewState="false" ThemePath="UserStatusModule.ascx" runat="server" />
			</div>
		</div>
	</div>
	

	<div id="nwtd-pageContent" class="nwtd-defaultPageContent">
	    <div id="print-logo"><img id="Img1" src="~/Images/print-logo.png" runat="server" /></div>
		<div id="nwtd-breadrumb">
			<NWTD:Breadcrumb ID="Breadcrumb1" runat="server" EnableViewState="false" />
		</div>
		<NWTD:SearchLink ID="SearchLink1" runat="server" />	
		<NWTD:SearchBar runat="server" ID="searchBar" ShowAdvancedSearch="false" />
		<asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/top-curve.png" cssclass="curves" />
		
		
		<div id="nwtd-leftNav">
			THIS IS WHERE THE LEFT NAV WILL BE
		</div>
		<div id="leftColumn">
			<cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
		</div>
		<div id="rightColumn">
			<cms:CmsPlaceHolder runat="server" ID="RightContentArea" />
		</div>

		<asp:Image ID="Image2" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/bottom-curve.png" cssclass="curves" />
	</div>
	
	<br style="clear:both;" />
	<NWTD:Footer runat="server" ID="nwtdfooter" />
</div>