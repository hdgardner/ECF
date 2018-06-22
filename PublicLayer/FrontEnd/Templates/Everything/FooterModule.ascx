<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Templates.Everything.FooterModule"
    CodeBehind="FooterModule.ascx.cs" %>
<div id="navbar-bottom" class="clearfix">
    <div id="sectionlinks">
    </div>
    <div id="aboutus">
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl='<%#ResolveUrl("~/About.aspx") %>'>About Demo</asp:HyperLink>
        |
        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl='<%#ResolveUrl("~/GeneralQuestions.aspx") %>'><%=RM.GetString("FOOTER_GENERAL_QUESTIONS")%></asp:HyperLink>
        |
        <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl="javascript:CSCompareProducts.OpenCompareView();"><%=RM.GetString("FOOTER_COMPARE_PRODUCTS")%></asp:HyperLink>
    </div>
</div>
<div id="copyright">
    <a target="_blank" href="http://www.mediachase.com">Mediachase LTD</a> © 1997-2009
    <asp:HyperLink runat="server" NavigateUrl='<%#ResolveUrl("~/Privacy.aspx") %>'><%=RM.GetString("FOOTER_PRIVACY_POLICY")%></asp:HyperLink>
    |
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#ResolveUrl("~/Terms.aspx") %>'><%=RM.GetString("FOOTER_TERMS_OF_USE")%></asp:HyperLink>
</div>
