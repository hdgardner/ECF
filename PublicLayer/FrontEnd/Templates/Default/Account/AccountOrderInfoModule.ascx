<%@ Control Language="C#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.AccountOrderInfoModule" Codebehind="AccountOrderInfoModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx" TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Register TagPrefix="ecf" TagName="AccountOrderShipments" Src="AccountOrderShipmentsModule.ascx" %>
<%@ Register TagPrefix="ecf" TagName="AccountOrderSkus" Src="AccountOrderSkusModule.ascx" %>
<div class="header">
    <h1><%=RM.GetString("ACCOUNT_ORDER_TITLE")%></h1>
</div>
<div class="clearfloat">&nbsp;</div>
<br />
<% if (Order != null) { %>
<div style="width:100%;">
    <div>
        <div style="vertical-align:top;">
            <div style="padding: 1px 1px 1px 1px; border-style:none;">
                <div>
                    <div style="text-align:left;">
                        <b><%=RM.GetString("ACCOUNT_ORDER_NUMBER_LABEL")%>:</b>
                    </div>
                    <div>
                        <asp:Label ID="OrderNumber" runat="server"><%= Order.TrackingNumber %></asp:Label>
                    </div>
                </div>
                <div>
                    <div>
                        <b><%=RM.GetString("ACCOUNT_ORDER_DATE_LABEL")%>:</b>
                    </div>
                    <div>
                        <asp:Label ID="OrderDate" runat="server"><%= Mediachase.Cms.Util.CommonHelper.GetUserDateTime(Order.Created) %></asp:Label>
                    </div>
                </div>
                <div>
                    <div>
                        <b><%=RM.GetString("ACCOUNT_ORDER_STATUS_LABEL")%>:</b>
                    </div>
                    <div>
                        <asp:Label ID="OrderStatus" runat="server"><%= Order.Status %></asp:Label>
                    </div>
                </div>
                <div>
                    <div>
                        <b><%=RM.GetString("ACCOUNT_ORDER_PAYMENT_LABEL")%>:</b>
                    </div>
                    <div>
                        <asp:Label ID="OrderPaymentProcessed" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="SkusPurchased">
        </asp:PlaceHolder>
    </div>
</div>
<br />
<div id="ecf-checkout">
        <div class="form">
            <div class="header"><%=RM.GetString("ACCOUNT_ORDER_EMAIL")%></div>
            <div class="body" style="padding: 2px 2px 2px 2px;">
                <asp:Label ID="OrderEmail" runat="server"></asp:Label>
            </div>
        </div>
    <ecf:AccountOrderShipments Order="<%#Order %>" ID="OrderShipments" runat="server"></ecf:AccountOrderShipments>
    <ecf:AccountOrderSkus Order="<%#Order %>" ID="AccountOrderSkus" runat="server"></ecf:AccountOrderSkus>
</div>
<br />
<% } %>
