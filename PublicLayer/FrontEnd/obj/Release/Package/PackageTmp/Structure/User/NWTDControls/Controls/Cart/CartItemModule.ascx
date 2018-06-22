<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CartItemModule.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.CartItemModule" %>
<asp:HyperLink ID="EntryLink" runat="server">
	<%# DataBinder.Eval(LineItem,"DisplayName") %>
</asp:HyperLink>