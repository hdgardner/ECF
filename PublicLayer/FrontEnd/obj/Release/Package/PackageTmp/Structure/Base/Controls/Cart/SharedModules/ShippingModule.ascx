<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="Mediachase.eCF.PublicStore.SharedModules.ShippingModule" Codebehind="ShippingModule.ascx.cs" %>
<!-- Shipping table -->
<table style="width: 100%; text-align:left" class="ecf-checkout-navframe" border="0" cellpadding="3" cellspacing="3">
    <tr class="ecf-checkout-navframe">
        <td colspan="2" valign="top"><h3><%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%>:</h3>
            <asp:RadioButtonList AutoPostBack="false" ID="ShippingRatesList" OnSelectedIndexChanged="ShippingRatesList_SelectedIndexChanged" runat="server">
            </asp:RadioButtonList>
        </td>
    </tr>
</table>
