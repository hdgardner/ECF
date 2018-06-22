<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Catalog_SharedModules_PriceLineModule" Codebehind="PriceLineModule.ascx.cs" %>
<asp:PlaceHolder Visible="False" Runat="server" ID="ListPricePanel">
<asp:Label runat="server" ID="ListPriceLabel" Font-Bold="true">
	<%=RM.GetString("GENERAL_LISTPRICE_LABEL")%>:
</asp:Label>
<asp:Label Font-Bold="True" Cssclass="ecf-listprice" Runat="server" ID="ListPriceValue"></asp:Label>
</asp:PlaceHolder>
<asp:Label runat="server" ID="SalePriceLabel" Font-Bold="true">
	<%=RM.GetString("GENERAL_PRICE_LABEL")%>:
</asp:Label>
<asp:Label Font-Bold="True" Cssclass="ecf-price" Runat="server" ID="SalePriceValue"></asp:Label>
<asp:PlaceHolder Visible="False" Runat="server" ID="DiscountPricePanel"><br />
	<b>
		<%=RM.GetString("YOU_SAVE_LABEL")%>:
	</b>
	<asp:Label id="DiscountPrice" Runat="server" Cssclass="ecf-price"></asp:Label>
</asp:PlaceHolder>

