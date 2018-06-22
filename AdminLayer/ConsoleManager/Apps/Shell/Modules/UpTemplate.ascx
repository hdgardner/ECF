<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Commerce.Manager.Apps.Shell.Modules.UpTemplate" Codebehind="UpTemplate.ascx.cs" %>
<%@ Import Namespace="Mediachase.Commerce" %>
<div style="width:100%;height:50px; background-image:url(<%=ResolveClientUrl("~/App_Themes/Default/Images/Shell/up_bg.gif") %>); BACKGROUND-REPEAT: repeat-y; BACKGROUND-COLOR: #B4CAF4;">
	<div style="float:left;width:350px;padding:5px;">
	<asp:HyperLink runat="server" NavigateUrl="~/"><img alt="Mediachase eCommerce Framework 5.0: Commerce Manager" align='absmiddle' src='<%=ResolveClientUrl("~/App_Themes/Default/Images/Shell/Logo.png") %>' width="300" height="40" /></asp:HyperLink></div>
	<div style="float:left;padding:15px;">
        <asp:Label ID="lblPageTitle" runat="server" CssClass="ibn-pagetitle" Text="<%$ Resources:ShellStrings, Page_Title %>"></asp:Label>
	</div>
	<div style="float:right;padding:7px" id="rightPart">
	    <asp:label runat="server" id="lblUser" cssclass="ibn-propertysheet"></asp:label>
	    <br />
    	<asp:label runat="server" id="lblVersion" cssclass="ibn-propertysheet" Text='<%$ Resources:CommerceManager, UPTEMPLATE_VERSION %>'></asp:label>: <%=FrameworkContext.ProductVersionDesc%>
    	<br />
    	<asp:label runat="server" id="Label1" cssclass="ibn-propertysheet" Text='<%$ Resources:CommerceManager, UPTEMPLATE_LICENSE %>'></asp:label>:
    	<asp:LinkButton id="lblLicenseInfo" runat="server" cssclass="ibn-propertysheet" OnClientClick="CSManagementClient.ChangeView('Core', 'Licensing');return false;"></asp:LinkButton>
	</div>
	<div style="clear:both;"></div>
</div>
