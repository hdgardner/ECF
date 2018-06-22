<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Default.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.Menu.Default" %>
<ul class="nwtd-menu" id="menu">
    <asp:Repeater runat="server" ID="SiteMenu" EnableViewState="false">
        <ItemTemplate>
			<li class="nwtd-menuItem">
                <a  <%#(Eval("Text").Equals("Publishers"))?"target='_blank'":"" %> class="top-menu-item" href='<%#Page.ResolveUrl(Eval("navigateurl").ToString())%>'><%# Eval("Text") %></a>
			</li>
		</ItemTemplate>
    </asp:Repeater>
    <li id="Div1" runat="server" class="top-menu-last-column">
		<a runat="server" class="top-menu-item" id="TopMenuLastColumn" href='' />
	</li>
</ul>
<asp:SiteMapDataSource runat="server" ID="MenuDataSource" SiteMapProvider="CmsSiteMapProvider" />
<asp:SiteMapDataSource runat="server" ID="CatalogDataSource" SiteMapProvider="CatalogSiteMapProvider" />