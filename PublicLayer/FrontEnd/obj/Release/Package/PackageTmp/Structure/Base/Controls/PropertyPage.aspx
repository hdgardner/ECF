<%@ Page Language="C#" AutoEventWireup="true" Theme="PopupEdit" Inherits="Controls_PropertyPage" Codebehind="PropertyPage.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Control Settings</title>
</head>
<body style="margin: 0px 0px 0px 0px;">
    <form id="form1" runat="server">
        <table class="stdTable" cellspacing="0" cellpadding="8">
            <colgroup>
                <col>
                <tbody>
                    <tr>
                        <td class="header">
                            <div class="header">
                                <b><asp:Label runat="server" ID="HeaderLabel"></asp:Label></b></div>
                            <div class="headerdesc"><asp:Label runat="server" ID="DescriptionLabel"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="main" height="100%">
                            <table height="100%" cellspacing="0" cellpadding="0" width="100%">
                                <tbody>
                                    <tr>
                                        <td height="100%">
                                            <div class="ItemList">
                                                <div class=main><asp:PlaceHolder runat="server" ID="PPage" /></div>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <TD class=buttons style="HEIGHT: 40px"><asp:Button CssClass="button" runat="server" ID="Save" Text="OK" />&nbsp;<asp:Button runat="server" ID="Cancel" CssClass="button" Text="Cancel" OnClientClick="window.close();" /> </TD>
                    </tr>
                </tbody>
        </table>
    </form>
</body>
</html>
