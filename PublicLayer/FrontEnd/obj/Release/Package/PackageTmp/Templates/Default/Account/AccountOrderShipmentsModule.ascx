<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.AccountOrderShipmentsModule" Codebehind="AccountOrderShipmentsModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx" TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Import Namespace="Mediachase.Commerce.Shared" %>
<div class="form">
    <div class="header"><%=RM.GetString("ACCOUNT_ORDER_SHIPMENTS_LABEL") %></div>
    <div class="body" style="border-style:solid; border-width:0px 3px 3px 3px; margin-bottom: 6px;">
        <asp:Repeater runat="server" ID="ShipmentList" OnItemDataBound="ShipmentList_ItemDataBound">
            <ItemTemplate>
                <asp:Label runat="server" ID="StatusLabel" Visible="false" >
                    <div class="ecf-checkout-navframe" style="text-align:left; width:100%; padding:0px; vertical-align:middle;">
                            <%# GetShipmentStatus(Container.ItemIndex) %>
                    </div>
                </asp:Label>
                <div class="ecf-shipping-header">
                    <!-- Should provide way to do delivery estimates -->
                    <asp:HyperLink runat="server" ID="TrackingUrl" Target="_blank" Visible="false">Track shipment</asp:HyperLink><br />
                    <%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_PACKAGE")+" "%>
                    <%#Eval("ShippingMethodName") %>
                    <br />
                </div>
                <div class="ecf-shipping-content-address"> 
                    <asp:Label ID="Label6" runat="server" CssClass="ecf-checkout-grid-header"><%=RM.GetString("ACCOUNT_ORDER_SHIPPING_LABEL") %>:</asp:Label>
                    <br />
                    <ecf:AddressViewModule ID="AddressViewModule1" AddressInfo='<%#GetShippingAddress((string)Eval("ShippingAddressId")) %>'
                        runat="server"></ecf:AddressViewModule>
                    <br />
                </div>
                <div class="ecf-shipping-content-items">
                    <asp:GridView GridLines="None" CellSpacing="2" runat="server" DataSource='<%# GetShipmentLineItems((Shipment)Container.DataItem) %>'
                        AutoGenerateColumns="false" Style="width: 100%" ID="GridView1">
                        <Columns>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderStyle-CssClass="ecf-checkout-grid-header"
                                HeaderText="Items Ordered">
                                <ItemStyle VerticalAlign="Top" HorizontalAlign="Left" />
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# StoreHelper.GetQuantityAsString((decimal)DataBinder.Eval(Container, "DataItem.Quantity")) %>'></asp:Label>
                                    <%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_OF")%>: <b>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("DisplayName") %>'></asp:Label></b>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-HorizontalAlign="Right" HeaderStyle-CssClass="ecf-checkout-grid-header"
                                HeaderText="Price">
                                <ItemStyle VerticalAlign="Top" HorizontalAlign="Right" />
                                <ItemTemplate>
                                    <asp:Label ID="Label5" runat="server" Text='<%#CurrencyFormatter.FormatCurrency((decimal)DataBinder.Eval(Container, "DataItem.ExtendedPrice"), Order.BillingCurrency) %>'></asp:Label><br />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />
                    <div class="ecf-shipping-content-totals">
                        <div>
                            <div class="ecf-shipping-content-totalsleft">
                                <%=RM.GetString("ACCOUNT_ORDER_SKUS_DISCOUNT")%>:
                            </div>
                            <div>
                                <asp:Label runat="server" ID="lblTotalShipmentDiscount"></asp:Label>
                            </div>
                        </div>
                        <div>
                            <div class="ecf-shipping-content-totalsleft">
                                <%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_ITEMS_SUBTOTAL")%>:
                            </div>
                            <div>
                                <asp:Label ID="SubTotal" runat="server"></asp:Label>
                            </div>
                        </div>
                        <asp:Repeater runat="server" ID="Taxes">
                            <ItemTemplate>
                                <div>
                                    <div class="ecf-shipping-content-totalsleft">
                                        <%# DataBinder.Eval(Container.DataItem, "Name") %>
                                        (<%# DataBinder.Eval(Container.DataItem, "ItemAttributes.Attribute[0].Value[0]")%>%):
                                    </div>
                                    <div>
                                        <%# DataBinder.Eval(Container.DataItem, "ItemAttributes.ListPrice.FormattedPrice") %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div style="vertical-align:top;">
                            <div class="ecf-shipping-content-totalsleft">
                                <%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_SHIPPING")%>:
                            </div>
                            <div>
                                <asp:Label ID="ShippingCost" runat="server" Text='<%# CurrencyFormatter.FormatCurrency((decimal)Eval("ShipmentTotal"), Order.BillingCurrency)%>'></asp:Label>
                            </div>
                        </div>
                        <div style="vertical-align:top;">
                            <div class="ecf-shipping-content-totalsleft">
                                &nbsp;
                            </div>
                            <div>
                                -----
                            </div>
                        </div>
                        <div style="vertical-align:top;">
                            <div class="ecf-shipping-content-totalsleft">
                                <b><%=RM.GetString("ACCOUNT_ORDER_SHIPMENT_TOTAL_SHIPMENT")%>:</b>
                            </div>
                            <div>
                                <b><asp:Label ID="TotalCost" runat="server"></asp:Label></b>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        <div style="clear:left" />
    </div>
</div>
