<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NWTDItemDetailTemplate.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.PageTemplates.NWTDItemDetailTemplate" %>
<%@ Register Src="~/Templates/NWTD/NWTDHeader.ascx" TagName="Header" TagPrefix="NWTD" %>
<%@ Register Src="~/Templates/NWTD/FooterModule.ascx" TagName="Footer" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Menu/Breadcrumb.ascx" TagPrefix="NWTD" TagName="Breadcrumb" %>

<%@ Register src="../SearchLink.ascx" tagname="SearchLink" tagprefix="NWTD" %>
<div id="center">
<div id="nwtd-page" class="nwtd-detailsPage">
	<div id="nwtd-pageHeader">
		<NWTD:Header runat="server" ID="nwtdheader" />
		
	</div>
	<div id="nwtd-pageContent" class="nwtd-defaultPageContent">
	    <div id="print-logo"><img id="Img1" src="~/Images/print-logo.png" runat="server" /></div>
		<div id="nwtd-breadrumb">
			<NWTD:Breadcrumb ID="Breadcrumb1" runat="server" EnableViewState="false" />
		</div>
		<asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/top-curve.png" cssclass="curves" />
		<cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
		<asp:Image ID="Image2" runat="server" ImageUrl="~/App_Themes/NWTD/images-template/bottom-curve.png" cssclass="curves" />
	    <br style="clear:both;" />
	    <NWTD:Footer runat="server" ID="nwtdfooter" />
	</div>
</div>
</div>