<%@ Control Language="c#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.AccountOrderSkusModule" Codebehind="AccountOrderSkusModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx" TagName="AddressViewModule" TagPrefix="ecf" %>
<div class="column-left">
    <div class="form">
        <div class="header">
            <%=RM.GetString("ACCOUNT_ORDER_SKUS_PAYMENT_INFO")%></div>
        <div class="body" style="padding: 4px;">
            <asp:Repeater runat="server" ID="PaymentList" DataSource="<%#Order.OrderForms[0].Payments %>">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <label runat="server" id="Label2">
                        <%=RM.GetString("ACCOUNT_ORDER_SKUS_METHOD")%>:</label>
                    <asp:Label runat="server" ID="PaymentMethodLabel" Text='<%#Eval("PaymentMethodName")%>'></asp:Label>
                    <br />
                    <nobr><b>Customer Name:</b> <asp:Label runat="server" ID="CreditCardTypeLabel" Text='<%#Eval("CustomerName")%>'></asp:Label></nobr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <div>
                <b>
                    <%=RM.GetString("ACCOUNT_ORDER_SKUS_BILLING_INFO")%></b></div>
            <ecf:AddressViewModule ID="BillingAddress" runat="server"></ecf:AddressViewModule>
        </div>
    </div>
</div>
<div class="column-right" style="width:213px;padding: 3px 3px 3px 3px;">
    <div>
        <div style="text-align:right; float:left;">
            <%=RM.GetString("GENERAL_SUBTOTAL_LABEL")%>:
        </div>
        <div style="text-align:right;">
            <asp:Label runat="server" ID="SubTotal"></asp:Label>
        </div>
    </div>
    <div>
        <div style="text-align:right; float:left;">
            <%=RM.GetString("ACCOUNT_ORDER_SKUS_DISCOUNT")%>:
        </div>
        <div style="text-align:right;">
            <asp:Label runat="server" ID="CustomerDiscount"></asp:Label>
        </div>
    </div>
    <div>
        <div style="text-align:right;float:left">
            <%=RM.GetString("GENERAL_TAXES_LABEL") %>:
        </div>
        <div style="text-align:right;">
            <asp:Label runat="server" ID="Taxes"></asp:Label>
        </div>
    </div>
    <div style="padding: 0px 0px 10px 0px;">
        <div style="text-align:right;float:left">
            <%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_SHIPPING")%>:
        </div>
        <div style="text-align:right;">
            <asp:Label runat="server" ID="Shipping"></asp:Label>
        </div>
    </div>
    <div>
        <div style="text-align:right;float:left" ></div>
        <div style="text-align:right;">
            ----
        </div>
    </div>
    <div>
        <div style="text-align:right;float:left">
            <b>
                <%=RM.GetString("GENERAL_SKUS_TOTAL_LABEL")%>:</b>
        </div>
        <div style="text-align:right;">
            &nbsp;
            <asp:Label runat="server" ID="Total"></asp:Label>
        </div>
    </div>
</div>

