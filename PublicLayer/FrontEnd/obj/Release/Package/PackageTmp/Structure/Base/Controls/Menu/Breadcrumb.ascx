<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Menu_Breadcrumb"
    CodeBehind="Breadcrumb.ascx.cs" %>
<asp:HyperLink ID="HyperLink1" runat="Server" NavigateUrl='<%#ResolveUrl("~/default.aspx")%>'>Home</asp:HyperLink>
<asp:SiteMapPath ID="StoreSiteMap" EnableViewState="false" runat="server" RootNodeStyle-CssClass="hidden" RenderCurrentNodeAsLink="false">
    <NodeTemplate>»
        <asp:HyperLink runat="server" NavigateUrl='<%#Eval("Url") %>' Text='<%#Eval("Title") %>'></asp:HyperLink>
    </NodeTemplate>
    <PathSeparatorTemplate>
    </PathSeparatorTemplate>
</asp:SiteMapPath>
<asp:SiteMapPath SiteMapProvider="CatalogSiteMapProvider" PathDirection="RootToCurrent" ID="CatalogSiteMap" EnableViewState="false"
    runat="server" RenderCurrentNodeAsLink="false">
    <NodeTemplate>»
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#Eval("Url") %>' Text='<%#Eval("Title") %>'></asp:HyperLink>
    </NodeTemplate>
    <PathSeparatorTemplate>
    </PathSeparatorTemplate>
    <CurrentNodeTemplate>» <%#Eval("Title")%>
    </CurrentNodeTemplate>
</asp:SiteMapPath>