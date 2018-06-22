<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_SideMenu" CodeBehind="SideMenuTemplate.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Menu/Breadcrumb.ascx" TagPrefix="cms" TagName="Breadcrumb" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ErrorModule.ascx" TagPrefix="cms" TagName="ErrorModule" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule" TagPrefix="cms" %>
<%@ Register Src="~/Structure/Base/Controls/Image/ImageView.ascx" TagPrefix="cms" TagName="Image" %>   
<%@ Register Src="~/Structure/Base/Controls/Menu/SiteMenu.ascx" TagPrefix="cms" TagName="SiteMenu" %>
<%@ Register Src="../MainMenuControl.ascx" TagPrefix="cms" TagName="Menu" %>
<%@ Register Src="../Modules/SideMenuControl.ascx" TagPrefix="cms" TagName="SideMenu" %>
<%@ Register Src="../FooterModule.ascx" TagPrefix="cms" TagName="Footer" %>
<%@ Register TagPrefix="cms" TagName="LanguagePicker" Src="~/Structure/Base/Controls/Common/LanguagePicker.ascx" %>
<%@ Register TagPrefix="cms" TagName="SitePicker" Src="~/Structure/Base/Controls/Common/SitePicker.ascx" %>
<div id="bodywrapper">
    <div id="navbar-top">
        <div id="language" class="clearfix">            
            <cms:LanguagePicker runat="server" ID="LanguagePick" EnableViewState="false" />
        </div>
        <div id="sites" class="clearfix">
            <cms:SitePicker runat="server" ID="SitePicker1" EnableViewState="false" />
        </div>
        <div id="login" class="clearfix">
            <!-- user status -->
            <cms:ThemedControlModule ID="ThemedControlModule1" ThemePath="UserStatusModule.ascx"
                runat="server" />
        </div>
    </div>
    <div class="pagewrapper clearfix">
        <div id="headwrapper">
            <div id="sitetitle"><asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl='<%#ResolveUrl("~/default.aspx")%>'><cms:Image runat="server" ID="SiteLogo" ImageUrl="~/images/title.gif" />&nbsp;</asp:HyperLink></div>
            <div id="searchbar">
                <!-- search module-->
                <cms:ThemedControlModule ID="ThemedControlModule3" ThemePath="SearchModule.ascx"
                    runat="server"></cms:ThemedControlModule>
            </div>
        </div>
        <!--MENU-->
        <div id="menu-top">
            <cms:SiteMenu ID="menuSite" runat="server" />
        </div>
        <div id="AjaxMessage" class="hint" style="top: 200px; visibility: hidden; color: black;
            display: inline">
        </div>
        <!--CONTENT-->
        <div id="contentwrapper">
            <!-- bredcrumb -->
            <div id="breadcrumb">
                <cms:Breadcrumb runat="server" />
            </div>
            <div id="maincontent">
                <cms:ErrorModule ID="ErrorModule1" runat="server"></cms:ErrorModule>
                <!-- START: Content -->
                <cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
                <!-- END: Content -->
            </div>
        </div>
    </div>
    <!-- footer -->
    <div id="footerwrapper">
        <cms:Footer runat="server" />
        <a target="_blank" href="http://www.mediachase.com">Running on Mediachase eCommerce Framework 5.0</a>
    </div>
</div>
