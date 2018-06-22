<%@ Page Language="C#" AutoEventWireup="true" Theme="Main" CodeBehind="PopupPage.aspx.cs" Inherits="Mediachase.Commerce.Manager.Core.Controls.PopupPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title><asp:Literal runat="server" Text="<%$ Resources:SharedStrings, Mediachase_Commerce_Manager_5_0 %>"/></title>
</head>
<body class="popupWindowBody">
    <form id="form1" runat="server">
    <div>
        <div class="popupWindowHeader">
            <asp:Label runat="server" ID="HeaderText" CssClass="popupWindowHeader"></asp:Label>
        </div>
        <asp:Label runat="server" ID="ErrorText" ForeColor="Red" Visible="false"></asp:Label>
        <asp:PlaceHolder ID="phMain" runat="server"></asp:PlaceHolder>
    </div>
    </form>
</body>
</html>