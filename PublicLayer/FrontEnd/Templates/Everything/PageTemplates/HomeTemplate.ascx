<%@ Control Language="C#" AutoEventWireup="true" Inherits="Templates_Home" CodeBehind="HomeTemplate.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/PopupBase/PopUpEditor.ascx" TagPrefix="editor"
    TagName="PopUp" %>
<%@ Register Src="~/Structure/Base/Controls/InlineBase/InlineEditor.ascx" TagPrefix="editor"
    TagName="Inline" %>
<%@ Register Src="~/Structure/Base/Controls/Image/ImageView.ascx" TagPrefix="cms"
    TagName="Image" %>
<%@ Register Src="~/Structure/Base/Controls/Menu/Breadcrumb.ascx" TagPrefix="cms"
    TagName="Breadcrumb" %>
<%@ Register Src="~/Structure/Base/Controls/Common/ErrorModule.ascx" TagPrefix="cms"
    TagName="ErrorModule" %>
<%@ Register Src="~/Structure/Base/Controls/Menu/SiteMenu.ascx" TagPrefix="cms"
    TagName="SiteMenu" %>        
<%@ Register Src="~/Structure/Base/Controls/Common/ThemedControlModule.ascx" TagName="ThemedControlModule"
    TagPrefix="cms" %>
<%@ Register Src="../MainMenuControl.ascx" TagPrefix="cms" TagName="Menu" %>
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
            <div id="searchbar" class="clearfix">
                <!-- search module-->
                <cms:ThemedControlModule ID="ThemedControlModule3" ThemePath="SearchModule.ascx"
                    runat="server"></cms:ThemedControlModule>
            </div>
        </div>
        <!--MENU-->
        <div id="TopMenu">
            <!-- menu -->
            <cms:SiteMenu ID="menuSite" runat="server" />
        </div>
        <table border="0" cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <td style="width: 100%">
                    <div id="AjaxMessage" class="hint" style="top: 200px; visibility: hidden; color: black;
                        display: inline">
                    </div>
                    <!--CONTENT-->
                    <div id="contentwrapper">
                        <cms:ErrorModule ID="ErrorModule1" runat="server"></cms:ErrorModule>
                        <!-- START: Content -->

                        <script type="text/javascript">new fadeshow(fadeimages, 973, 252, 0, 5000, 1);</script><br />

                        <div class="home-content">
                            <div class="home-highlight">
                                <asp:Image runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-highlight1.png" /><asp:Image runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-highlight2.png" usemap="#BrandsMap"/>
                                
                            </div>
                            <div class="home-promos">
                                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#ResolveUrl("~/digital-cameras.aspx")%>'><asp:Image ID="Image1" runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-promo1.png" /></asp:HyperLink><asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#ResolveUrl("~/catalog/searchresults.aspx?filter=&search=canon+powershot+pro+series")%>'><asp:Image ID="Image2" runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-promo2.png" /></asp:HyperLink><asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/catalog/searchresults.aspx?filter=&search=blackberry")%>'><asp:Image ID="Image3" runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-promo3.png" /></asp:HyperLink><a href="http://www.mediachase.com/commerce" target="_blank"><asp:Image ID="Image4" runat="server" ImageUrl="~/App_Themes/Everything/images/home/main-promo4.png" /></a>
                            </div>
                        </div>
                        <cms:CmsPlaceHolder runat="server" ID="MainContentArea" />
                        <!-- END: Content -->
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <!-- footer -->
    <div id="footerwrapper">
        <cms:Footer ID="Footer1" runat="server" EnableViewState="false" />
        <a target="_blank" href="http://www.mediachase.com">Running on Mediachase eCommerce
            Framework 5.0</a>
    </div>
</div>
<map name="BrandsMap"><area shape="rect" coords="56,11,129,42" href="catalog/searchresults.aspx?filter=&search=nokia">
<area shape="rect" coords="139,10,216,43" href="Digital-Cameras.aspx?Brand=canon">
<area shape="rect" coords="229,7,299,46" href="catalog/searchresults.aspx?filter=&search=rim">
<area shape="rect" coords="310,7,379,48" href="catalog/searchresults.aspx?filter=&search=lg">
<area shape="rect" coords="392,9,465,46" href="Digital-Cameras.aspx?Brand=olympus">
</map>  
