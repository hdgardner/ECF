<%@ Control 
	Language="C#" 
	AutoEventWireup="true" 
	Inherits="Mediachase.Cms.Website.Templates.NWTD.FooterModule"
    CodeBehind="FooterModule.ascx.cs" %>
<div id="navbar-bottom">
    <%--On 08/02/17, Heath Gardner changed the copyright to 2017--%>
    <div class="copyright">&#0169; 2017 Northwest Textbook Depository.  All Rights Reserved.</div>  
	<div class="footer-links">
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#ResolveUrl("~/") %>'>Home</asp:HyperLink>
		|
		<asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl='<%#ResolveUrl("~/who-we-are/who-we-are.aspx") %>'>Who We Are</asp:HyperLink>
        <%--On 11/12/13, Heath Gardner commented out the the Publishers hyperlink per Shawn's request--%>
        <%--  |
		 <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl='<%#ResolveUrl("~/Publishers/OurPublishers.aspx") %>'>Publishers</asp:HyperLink> --%>
        |
		 <asp:HyperLink ID="HyperLink5" runat="server" NavigateUrl='<%#ResolveUrl("~/Support/support.aspx") %>'>Support</asp:HyperLink>
        |
		 <asp:HyperLink ID="HyperLink6" runat="server" NavigateUrl='<%#ResolveUrl("~/contact/contact.aspx") %>'>Contact Us</asp:HyperLink>
        |
		 <asp:HyperLink ID="HyperLink7" runat="server" NavigateUrl='<%#ResolveUrl("~/privacy-statement.aspx") %>'>Privacy Statement</asp:HyperLink>
	</div>
</div>


