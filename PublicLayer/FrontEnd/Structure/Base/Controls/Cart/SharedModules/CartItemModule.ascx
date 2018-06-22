<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Cart_SharedModules_CartItemModule" Codebehind="CartItemModule.ascx.cs" %>
<asp:HyperLink ID="EntryLink" runat="server" CssClass="ecf-cart-product-title">
<%# DataBinder.Eval(LineItem, "DisplayName")%>
</asp:HyperLink>
<div class="ecf-cart-code">Item #: <%# DataBinder.Eval(LineItem, "CatalogEntryId")%></div>
<!-- discounts -->
<asp:Repeater ID="Repeater1" runat="server" DataSource='<%# DataBinder.Eval(LineItem, "Discounts")%>'>
    <HeaderTemplate>
    </HeaderTemplate>
    <SeparatorTemplate>
    </SeparatorTemplate>
    <ItemTemplate>
        <div class="ecf-cart-discount-title"><%# Eval("DisplayMessage")%></div>        
    </ItemTemplate>
</asp:Repeater>