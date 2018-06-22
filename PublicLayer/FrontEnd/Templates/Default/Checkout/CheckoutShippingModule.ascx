<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutShippingModule"
    CodeBehind="CheckoutShippingModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/ShippingModule.ascx"
    TagName="ShippingModule" TagPrefix="ecf" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Import Namespace="Mediachase.Commerce.Shared" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<div class="header">
    <h1>
        <%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%></h1>
    <div class="status">
        1.<%=RM.GetString("CHECKOUT_ADDRESS_LABEL")%><br />
        <b>2.<%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%></b><br />
        3.<%=RM.GetString("CHECKOUT_PAYMENT_LABEL")%><br />
        4.<%=RM.GetString("CHECKOUT_ORDER_CONFIRMATION_LABEL")%>
    </div>
</div>
<div class="clearfloat">
    &nbsp;</div>
<div class="shipping-options">
    <asp:DataList ID="ShippingsList" Style="width: 100%" ShowHeader="false" BorderWidth="0"
        BorderStyle="None" runat="server">
        <ItemTemplate>
            <div class="form">
                <div class="header">
                    <asp:Label ID="lblShippingNote" runat="server" Text='<%# String.Format("<b>{0}</b> {1}",RM.GetString("CHECKOUT_PAYPAL_SHIPPING_TO"), StoreHelper.GetAddressString(Container.DataItem as OrderAddress)) %>' /></div>
                <div class="body">
                    <div class="shipping-rates"><ecf:ShippingModule ID="ShippingModule1" OrderAddress="<%#(OrderAddress)Container.DataItem %>" CartHelper="<%#CartHelper %>" runat="Server"></ecf:ShippingModule></div>
                    <div class="shipping-items">
                    <asp:GridView Style="width: 100%" SkinID="ShippingView" DataSource="<%#GetItems((OrderAddress)Container.DataItem) %>"
                        ID="ItemsList" BorderStyle="Solid" AutoGenerateColumns="False" BorderWidth="0"
                        CellSpacing="3" GridLines="Horizontal" CellPadding="0"
                        runat="server" ShowHeader="false" Visible="true">
                        <EmptyDataTemplate>
                            <%=RM.GetString("GENERAL_NO_ADDITIONAL_OPTIONS")%>
                        </EmptyDataTemplate>
                        <Columns>
                            <asp:TemplateField HeaderText="<%$Resources:StoreResources,GENERAL_DESCRIPTION_LABEL%>">
                                <HeaderStyle Width="150px"></HeaderStyle>
                                <ItemTemplate>
                                    <div class="shipping-item">
                                        <!-- Should provide way to do delivery estimates -->
                                        <asp:Label ID="lblSku" runat="server">
                                            <%# StoreHelper.GetQuantityAsString((decimal)Eval("Quantity"))%> <%=" "+RM.GetString("ACCOUNT_ORDER_SHIPMENT_OF") + " " %>  <%# Eval("DisplayName") %>
                                        </asp:Label>
                                        <br />
                                        <span class="ecf-price">
                                            <%# CurrencyFormatter.FormatCurrency((decimal)Eval("ExtendedPrice"), CartHelper.Cart.BillingCurrency)%>
                                        </span>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:DataList>
</div>
