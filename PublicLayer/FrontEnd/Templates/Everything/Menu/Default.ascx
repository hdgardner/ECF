<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Default.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.Everything.Menu.Default" %>
<div class="top-menu" id="menu">
        <asp:Repeater runat="server" ID="SiteMenu" EnableViewState="false">
            <ItemTemplate><div class="top-menu-normal-column"><a class="top-menu-item" href='<%#Page.ResolveUrl(Eval("navigateurl").ToString())%>'><%# Eval("Text") %></a></div></ItemTemplate>
        </asp:Repeater>
        <div id="Div1" runat="server" class="top-menu-last-column"><a runat="server" class="top-menu-item" id="TopMenuLastColumn" href='' /></div>
</div>
<asp:SiteMapDataSource runat="server" ID="MenuDataSource" SiteMapProvider="CmsSiteMapProvider" />
<asp:SiteMapDataSource runat="server" ID="CatalogDataSource" SiteMapProvider="CatalogSiteMapProvider" />