<%@ Control Language="C#" AutoEventWireup="true" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutAddressModule"
    CodeBehind="CheckoutAddressModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx"
    TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressEditModule.ascx"
    TagName="AddressNewModule" TagPrefix="ecf" %>
<%@ Import Namespace="Mediachase.Commerce.Orders" %>
<%@ Import Namespace="Mediachase.Commerce.Profile" %>
<%@ Import Namespace="Mediachase.Cms.WebUtility.Commerce" %>
<%@ Register Src="OrderQuickSummary.ascx" TagName="OrderQuickSummary" TagPrefix="order" %>
<div class="header">
    <h1>
        <%=RM.GetString("CHECKOUT_ADDRESS_LABEL")%></h1>
    <div class="status">
        <b>1.<%=RM.GetString("CHECKOUT_ADDRESS_LABEL")%></b><br />
        2.<%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%><br />
        3.<%=RM.GetString("CHECKOUT_PAYMENT_LABEL")%><br />
        4.<%=RM.GetString("CHECKOUT_ORDER_CONFIRMATION_LABEL")%>
    </div>
</div>
<div class="clearfloat">
    &nbsp;</div>
<div class="validation-error">
    <asp:CustomValidator ID="RadioButtonsCustomValidator" Display="Dynamic" runat="server"
        ErrorMessage='<%=RM.GetString("CHECKOUT_ADDRESS_ERROR_MESSAGE") %>' OnServerValidate="RadioButtons_Validate">
    </asp:CustomValidator>
</div>
<div class="clearfloat">
    &nbsp;</div>
<div class="shipping-address">
    <div class="form" runat="server" id="trShippingAddresses">
        <div class="header">
            <%=RM.GetString("CHECKOUT_SHIPPING_INFO_LABEL")%></div>
        <div class="body">
            <asp:HyperLink ID="hlAddNewAddress" runat="server"><%=RM.GetString("ACCOUNT_ADDRESS_NEW")%></asp:HyperLink>
            <div class="shipping-address-list">
                <asp:DataList DataKeyField="AddressId" RepeatDirection="Horizontal" RepeatColumns="2"
                    CellPadding="0" CellSpacing="5" runat="server" ID="AddressList" Style="width: 100%">
                    <ItemTemplate>
                        <cms:GlobalRadioButton Font-Bold="true" GroupName="RadioButtonsAddressSelect" Text='<%#RM.GetString("CHECKOUT_ADDRESS_SHIP_TO_THIS_ADDRESS") %>'
                            runat="server" ID="rbShipToAddress" />
                        <ecf:AddressViewModule ID="AddressViewModule1" AddressInfo='<%# StoreHelper.ConvertToOrderAddress((CustomerAddress)Container.DataItem) %>'
                            runat="server" />
                        <br />
                        <asp:Button ID="btnEditAddress" OnClick="EditAddress" runat="server" Text='  <%#RM.GetString("ACCOUNT_ADDRESS_EDIT")%>  ' />
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <cms:ThemedImage ID="ThemedImage1" runat="server" Width="2" Height="1" ImageUrl="images/spacer.gif" />
                    </SeparatorTemplate>
                    <FooterTemplate>
                        <cms:ThemedImage ID="ThemedImage2" runat="server" Width="123" Height="20" ImageUrl="images/spacer.gif" />
                    </FooterTemplate>
                </asp:DataList>
            </div>
        </div>
    </div>
    <div class="form">
        <div class="header">
            <%=RM.GetString("CHECKOUT_ADDRESS_NEW_SHIPPING_ADDRESS")%></div>
        <div class="body">
            <div class="options">
            <cms:GlobalRadioButton Font-Bold="true" GroupName="RadioButtonsAddressSelect" Text='<%#RM.GetString("CHECKOUT_ADDRESS_SHIP_TO_THIS_ADDRESS") %>'
                runat="server" ID="rbShipToNewAddress" />
            </div>
            <ecf:AddressNewModule EnableClientScript="false" Validate="false" ID="AddressNewModule1"
                runat="server" />
        </div>
    </div>
</div>
<order:OrderQuickSummary ID="OrderQuickSummary1" runat="server" Visible="false" />
