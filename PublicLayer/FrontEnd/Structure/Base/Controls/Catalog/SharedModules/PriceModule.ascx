<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Catalog_SharedModules_PriceModule" Codebehind="PriceModule.ascx.cs" %>
    <table cellpadding="0" cellspacing="0" id="prodPrices" align="center">
        <tr runat="server" id="ListPricePanel">
            <td><%=RM.GetString("GENERAL_LISTPRICE_LABEL")%>:</td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="ListPriceLabel"></asp:Label></td>
        </tr>
        <tr runat="server" id="DiscountPricePanel">
            <td>
                <strong><%=RM.GetString("GENERAL_DISCOUNT_LABEL")%>:</strong>
            </td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="DiscountPrice"></asp:Label>
            </td>
        </tr>
        <tr class="yourPrice">
            <td><%=RM.GetString("GENERAL_YOUR_PRICE_LABEL")%>:</td>
            <td class="ecf-price">
                <asp:Label runat="server" ID="PriceLabel"></asp:Label></td>
        </tr>
        <tr class="shipping">
            <td colspan="2">
                <asp:Label runat="server" ID="AvailabilityLabel"></asp:Label></td>
        </tr>
    </table>
