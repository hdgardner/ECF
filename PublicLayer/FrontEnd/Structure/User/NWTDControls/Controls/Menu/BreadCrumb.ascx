<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BreadCrumb.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Menu.BreadCrumb" %>
<asp:HyperLink ID="HyperLink1" runat="Server" NavigateUrl='<%#ResolveUrl("~/default.aspx")%>'>Home</asp:HyperLink>
<asp:SiteMapPath Visible="true" ID="StoreSiteMap" EnableViewState="false" runat="server" RootNodeStyle-CssClass="hidden" RenderCurrentNodeAsLink="false">
    <NodeTemplate>»
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#Eval("Url") %>' Text='<%#Eval("Title") %>'></asp:HyperLink>
    </NodeTemplate>
    <PathSeparatorTemplate>
    </PathSeparatorTemplate>
</asp:SiteMapPath>
<asp:SiteMapPath Visible="true" SiteMapProvider="CatalogSiteMapProvider" PathDirection="RootToCurrent" ID="CatalogSiteMap" EnableViewState="false"
    runat="server"  RenderCurrentNodeAsLink="false">
    <NodeTemplate><%--»
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#Eval("Url") %>' Text='<%#Eval("Title") %>'></asp:HyperLink>
    --%></NodeTemplate>
    <PathSeparatorTemplate>
    </PathSeparatorTemplate>
    <CurrentNodeTemplate>» <%#Eval("Title")%>
    </CurrentNodeTemplate>
</asp:SiteMapPath>