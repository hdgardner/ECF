<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NWTDLeftNavTemplate.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.PageTemplates.NWTDLeftNavTemplate" %>
<%@ Register Src="~/Templates/NWTD/NWTDHeader.ascx" TagName="Header" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/FooterModule.ascx" TagName="Footer" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Menu/Breadcrumb.ascx" TagPrefix="NWTD" TagName="Breadcrumb" %>
<%@ Register src="../Menu/Default.ascx" tagname="Default" tagprefix="menu" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"  TagPrefix="cms" %>
<%@ Register src="../Menu/SubMenu.ascx" tagname="SubMenu" tagprefix="NWTD" %>
<div id="center">
	<div id="nwtd-page" class="nwtd-defaultPage leftNav">
		<div id="nwtd-pageHeader">
			<NWTD:Header runat="server" ID="nwtdheader" />
		</div>
		
		<div id="nwtd-pageContent" class="nwtd-defaultPageContent">
			<div id="nwtd-breadrumb">
				<NWTD:Breadcrumb ID="Breadcrumb1" runat="server" EnableViewState="false" />
			</div>
			<cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
			
			<div class="nwtd-leftNav">
				<NWTD:SubMenu ID="SubPagesMenu" MenuName="TopMenu" runat="server" />
			</div>
			
			<div id="nwtd-leftNav">
			
	    
				<cms:CmsPlaceHolder runat="server" ID="LeftNavArea" />
		
			</div>
			<br style="clear:both;" />
		<NWTD:Footer runat="server" ID="nwtdfooter" />
		</div>
		
	</div>
</div>