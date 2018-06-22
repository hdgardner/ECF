<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MSSDHeader.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.MSSDHeader" %>
<%@ Register src="Menu/Default.ascx" tagname="Default" tagprefix="menu" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"
    TagPrefix="cms" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/SearchByPublisher.ascx" TagName="SearchBar" TagPrefix="NWTD" %>

<div class="nwtd-header">
   <a href="/" style="float: left;"><img id="Img1" src="~/App_Themes/MSSD/images-template/nwtd-logo.png" runat="server" alt="Northwest Textbook Depository" /></a>
</div>
    <div class="need-help">
        <div id="nwtd-submenu" style="clear:both;">
		    <div class="nwtd-loginMenu">
				<cms:ThemedControlModule ID="ThemedControlModule1"  EnableViewState="false" ThemePath="UserStatusModule.ascx"  runat="server" />
			</div>
		</div>
	</div>

<div id="nwtd-Menu">
	<menu:Default MenuName="TopMenu" ID="Default1" runat="server" /><NWTD:SearchBar runat="server" ID="searchBar" ShowAdvancedSearch="false" />
</div>

