<%@ Control Language="C#" AutoEventWireup="true" Inherits="Controls_Cart_CartView"
    CodeBehind="CartView.ascx.cs" %>
<%@ Register Src="SharedModules/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="cart" %>
<%@ Register Src="SharedModules/PriceLineModule.ascx" TagName="PriceLineModule" TagPrefix="cart" %>
<%@ Register Src="SharedModules/EntryQuantityControl.ascx" TagName="EntryQuantityControl"
    TagPrefix="cart" %>
   
<div runat="server" id="divViewer">
</div>
<div id="ecf-cart">
    <h1>
        <%=RM.GetString("BASKET_LABEL")%></h1>
    <div class="coupon" runat="server" id="CouponSummary">
        <%=RM.GetString("BASKET_COUPON_LABEL")%>:
        <asp:TextBox MaxLength="20" Width="150" runat="server" ID="DiscountCouponCode"></asp:TextBox>
        <asp:Button CssClass="coupon-button button" UseSubmitBehavior="true" runat="server" ID="ApplyCouponButton" OnClick="ApplyCouponButton_Click"
            Text='Apply Coupon'></asp:Button>
    </div>
    <div class="lineitems">
        <asp:GridView OnRowEditing="ShoppingCart_RowEditing" OnRowDeleting="ShoppingCart_RowDeleting"
            OnRowCreated="ShoppingCart_RowCreated" EnableViewState="true" runat="server"
            ID="ShoppingCart" SkinID="ShoppingCart" DataKeyNames="LineItemId,Quantity" AutoGenerateColumns="false"
            Style="width: 100%">
            <EmptyDataTemplate>
                <%#RM.GetString("SHOPPING_CART_EMPTY_LABEL")%>
            </EmptyDataTemplate>
            <Columns>
                <asp:TemplateField ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"
                    ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="MoveWishList" ToolTip='' CssClass="ecf-move" Text="move to Wish List"
                            CommandName="Edit" CausesValidation="false"></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="RemoveItem" ToolTip='' CssClass="ecf-delete" Text="remove"
                            CommandName="Delete" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="" ItemStyle-Width="50">
                    <ItemTemplate>
                        <cms:MetaImage OpenFullImage="true" Width="35" ShowThumbImage="true" ID="PrimaryImage"
                            DataSource="<%#GetEntryImageSource((Mediachase.Commerce.Orders.LineItem)Container.DataItem) %>"
                            PropertyName="PrimaryImage" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"
                    HeaderText="Description" ItemStyle-Width="400">
                    <ItemTemplate>
                        <cart:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="160px" HeaderText="Each" HeaderStyle-HorizontalAlign="right"
                    ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <cart:PriceLineModule ID="PriceLineModule1" runat="server" LineItem="<%#Container.DataItem%>">
                        </cart:PriceLineModule>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="70px" HeaderStyle-HorizontalAlign="Right" ItemStyle-VerticalAlign="Top"
                    ItemStyle-HorizontalAlign="Right" HeaderText="Qty">
                    <ItemTemplate>
                        <cart:EntryQuantityControl runat="server" MinQuantity='<%#DataBinder.Eval(Container.DataItem, "MinQuantity")%>'
                            MaxQuantity='<%#DataBinder.Eval(Container.DataItem, "MaxQuantity")%>' ID="Quantity">
                        </cart:EntryQuantityControl>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right"
                    ItemStyle-VerticalAlign="Top" ItemStyle-Width="180px" HeaderText="Total">
                    <ItemTemplate>
                        <%#Mediachase.Commerce.Shared.CurrencyFormatter.FormatCurrency((decimal)Eval("ExtendedPrice"), CartHelper.Cart.BillingCurrency)%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div class="summary">
            <table cellspacing="1" cellpadding="3" style="width: 100%" runat="server" id="CartSummary">
                <tr runat="server" id="DiscountTr" visible="false">
                    <td align="right" style="width: 90%">
                        Discount (<asp:Repeater ID="DiscountList" runat="server">
                            <SeparatorTemplate>
                                ,</SeparatorTemplate>
                            <ItemTemplate>
                                <%# DataBinder.Eval(Container.DataItem, "DisplayMessage")%></ItemTemplate>
                        </asp:Repeater>
                        ):
                    </td>
                    <td align="right">
                        <asp:Label ID="TotalDiscount" CssClass="ecf-price" Font-Bold="False" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td align="right" style="width: 90%">
                        <div class="subtotal">Sub total:</div>
                    </td>
                    <td align="right">
                        <asp:Label ID="SubTotal" Font-Bold="True" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="actions">
        <asp:Button ID="UpdateCartButton" CssClass="update-button button" runat="server" Text='Update Wish List' OnClick="UpdateButton_Click"></asp:Button>
        <asp:Button ID="ContinueButton" CssClass="continue-button button" runat="server" OnClick="ContinueButton_Click" Text='Continue Shopping'></asp:Button>                
        <asp:Button ID="CheckoutButton" UseSubmitBehavior="true" CssClass="checkout-button button" OnClick="CheckoutButton_Click" runat="server" Text='Proceed to checkout'></asp:Button>   
    </div>
    
    <table cellpadding="3" cellspacing="1" style="width: 100%">
        <tr>
            <td align="left">
            </td>
        </tr>
        <tr>
            <td valign="top">
                <br />
                <%=RM.GetString("SHOPPING_CART_STORE_POLICES")%>:
                <ul>
                    <li>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl=''>
						<%=RM.GetString("SHOPPING_CART_RETURN_POLICY") %>
                        </asp:HyperLink>
                    </li>
                    <li>
                        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl=''>
					<%=RM.GetString("SHOPPING_CART_PRIVACY")%>
                        </asp:HyperLink>
                    </li>
                </ul>
            </td>
            <td align="right" valign="top">
                <table width="350" runat="server" id="PaypalExpress" visible="false">
                    <tr>
                        <td align="left">
                            <cms:ThemedImageButton ID="PayPalExpressButton" runat="server" ImageUrl="images/btn_xpressCheckout.gif"
                                ImageAlign="left" />
                            <span style="font-size: 11px; font-family: Arial, Verdana;">Save time. Checkout securely.
                                Pay without sharing your financial information. </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>
