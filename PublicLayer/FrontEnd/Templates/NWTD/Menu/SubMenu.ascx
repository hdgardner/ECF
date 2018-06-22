<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubMenu.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.NWTD.Menu.SubMenu" %>
<h2><asp:HyperLink runat="server" ID="hlSubMenuHeading" /></h2>
<ul class="nwtd-SubMenu">
    <asp:Repeater runat="server" ID="SiteMenu" EnableViewState="false"  OnItemDataBound="SubMenu_ItemDatabound">
		
        <ItemTemplate>
			<li class="nwtd-subMenuItem">
				<asp:HyperLink runat="server" CssClass="nwtd-subMenuLink" NavigateUrl='<%#Eval("navigateurl").ToString() %>' Text='<%# Eval("Text") %>'  ID="itemLink" />
				
				<%--<a href='<%#Page.ResolveUrl(Eval("navigateurl").ToString())%>'><%# Eval("Text") %></a>--%>
			</li>
		</ItemTemplate>
    </asp:Repeater>
<%--    <li id="Div1" runat="server" class="last">
		<a runat="server" class="top-menu-item" id="TopMenuLastColumn" href='' />
	</li>--%>
</ul>
<asp:SiteMapDataSource runat="server" ID="MenuDataSource" SiteMapProvider="CmsSiteMapProvider" />
