<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Licensing.aspx.cs" Inherits="Mediachase.Commerce.Manager.Licensing" Theme="Main" %>

<%@ Register Src="Apps/Core/License/LicensingControl.ascx" TagName="LicensingControl"
    TagPrefix="uc1" %>
<%@ Register Src="Apps/Core/ErrorModule.ascx" TagName="ErrorModule" TagPrefix="uc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>eCommerce Framework 5.0: Licensing</title>
    <style>
        H2
        {
            color: #666666;
        }
        H4
        {
            color: #808080;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true"
        EnableScriptGlobalization="true" LoadScriptsBeforeUI="true" ScriptMode="debug">
    </asp:ScriptManager>
    <div style="width: 100%; height: 50px; background-image: url(<%=ResolveClientUrl("~/App_Themes/Default/Images/Shell/up_bg.gif") %>);
        background-repeat: repeat-y; background-color: #B4CAF4;">
        <div style="float: left; padding: 15px;">
            <asp:Label ID="lblPageTitle" runat="server" CssClass="ibn-pagetitle" Text="Licensing and Activation Manager"></asp:Label>
        </div>
        <div style="float: right; padding: 7px" id="rightPart">
        </div>
    </div>
    <div class="LoginPanel">
        <div class="LoginTable">
            <asp:Panel ID="NoLicensePanel" runat="server">
                <h1>
                    It Looks Like You Haven&#39;t Installed Your Mediachase License Yet!</h1><br />
                <h3>
                    No Problem!&nbsp; If you have received the license from Mediachase, simply choose
                    from the installation approaches below to install your license.&nbsp; This is a
                    one time installation unless you change computers later.</h3><br />
                <h2>
                    Product/Installation Support Information</h2><br />
                <h4>
                    Exception:
                    <span style="color:Red">
                    <asp:Label runat="server" ID="ExceptionLabel"></asp:Label>
                    <%=Request.QueryString["m"]%>
                    </span>
                </h4>
                <br />
                <h2>
                    Installation Options</h2>
                <h4>
                    If you need assistance contact Mediachase by visiting our web site at <a href="http://www.mediachase.com">
                        http://www.mediachase.com</a> or send an e-mail to <a href="mailto:sales@mediachase.com">
                            sales@mediachase.com</a>, otherwise use either the LIC file or the product
                    Key supplied to you or your IT department at the time of purchase..</h4>
            </asp:Panel>
            <asp:Panel ID="LicensedPanel" runat="server">
                <h1>
                    <div style="color: green">
                        License is Installed Correctly</div>
                    <h1>
                    </h1>
                    <h3>
                        You can now proceed to the
                        <asp:HyperLink runat="server" NavigateUrl="~/">Commerce Manager</asp:HyperLink>
                        .
                    </h3>
                    <h4>
                        You can access the licensing information at any time in the Commerce Manager under
                        Administration-&gt;System Settings</h4>
                </h1>
            </asp:Panel>
            <div>
                <uc1:LicensingControl ID="LicensingControl1" runat="server" />
            </div>
            <uc2:ErrorModule ID="ErrorModule1" runat="server" />
        </div>
    </div>
    <div class="LoginFooter">
        <asp:HyperLink ID="HyperLink1" Target="_blank" runat="server" NavigateUrl="http://www.mediachase.com"><%=Mediachase.Commerce.FrameworkContext.ProductName%></asp:HyperLink>
        <br />
        Version:
        <%=Mediachase.Commerce.FrameworkContext.ProductVersionDesc%>
    </div>
    </form>
</body>
</html>
