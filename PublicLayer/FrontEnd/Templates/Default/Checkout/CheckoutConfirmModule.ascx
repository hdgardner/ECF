<%@ Control Language="C#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutConfirmModule"
    CodeBehind="CheckoutConfirmModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx"
    TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Register Src="CheckoutOrderShipmentsModule.ascx" TagName="CheckoutOrderShipments"
    TagPrefix="ecf" %>
<%@ Register TagPrefix="ecf" TagName="CheckoutOrderSkus" Src="CheckoutOrderSkusModule.ascx" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<div class="header">
    <h1><%=RM.GetString("CHECKOUT_ORDER_CONFIRMATION_LABEL")%></h1>
    <div class="status">
                        1.<%=RM.GetString("CHECKOUT_ADDRESS_LABEL")%><br />
                        2.<%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%><br />
                        3.<%=RM.GetString("CHECKOUT_PAYMENT_LABEL")%><br />
                        <b>4.<%=RM.GetString("CHECKOUT_ORDER_CONFIRMATION_LABEL")%></b>
    </div>
</div>
<div class="clearfloat">&nbsp;</div>
<div class="form">
    <div class="header"><%=RM.GetString("ACCOUNT_ORDER_EMAIL")%></div>
    <div class="body" style="padding: 2px 2px 2px 2px;">
        <asp:Label ID="OrderEmail" runat="server"><%= CartHelper.PrimaryAddress.Email%></asp:Label>
    </div>
</div>
<div class="form">
    <ecf:CheckoutOrderShipments OrderGroup="<%#(OrderGroup)CartHelper.Cart %>" ID="OrderShipments" runat="server"></ecf:CheckoutOrderShipments>
</div>
<ecf:CheckoutOrderSkus CartHelper="<%#CartHelper %>" ID="AccountOrderSkus" runat="server"></ecf:CheckoutOrderSkus>
