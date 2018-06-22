<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Cart_SharedModules_PriceLineModule" Codebehind="PriceLineModule.ascx.cs" %>
<asp:Label Font-Bold="True" Cssclass="ecf-price" Runat="server" ID="PriceLabel"></asp:Label>
<asp:PlaceHolder Visible="False" Runat="server" ID="ListPricePanel"><br /><asp:Label Font-Bold="True" Cssclass="ecf-listprice" Runat="server" ID="ListPriceLabel"></asp:Label></asp:PlaceHolder>
<asp:PlaceHolder Visible="False" Runat="server" ID="DiscountPricePanel">
	<%=RM.GetString("YOU_SAVE_LABEL")%>:
	<asp:Label id="DiscountPrice" Runat="server" Cssclass="ecf-discountprice"></asp:Label>
</asp:PlaceHolder>

