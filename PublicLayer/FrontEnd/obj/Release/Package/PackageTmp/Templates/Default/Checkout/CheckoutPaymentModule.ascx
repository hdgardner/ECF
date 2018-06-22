<%@ Control Language="C#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutPaymentModule"
    CodeBehind="CheckoutPaymentModule.ascx.cs" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressViewModule.ascx"
    TagName="AddressViewModule" TagPrefix="ecf" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/AddressEditModule.ascx"
    TagName="AddressNewModule" TagPrefix="ecf" %>
<div class="header">
    <h1>
        <%=RM.GetString("CHECKOUT_PAYMENT_LABEL")%></h1>
    <div class="status">
        1.<%=RM.GetString("CHECKOUT_ADDRESS_LABEL")%><br />
        2.<%=RM.GetString("CHECKOUT_SHIPPINGOPTIONS_LABEL")%><br />
        <b>3.<%=RM.GetString("CHECKOUT_PAYMENT_LABEL")%></b><br />
        4.<%=RM.GetString("CHECKOUT_ORDER_CONFIRMATION_LABEL")%>
    </div>
</div>
<div class="clearfloat">
    &nbsp;</div>
<div class="validation-error">
    <asp:CustomValidator ID="RadioButtonsPaymentCustomValidator" Display="Dynamic" runat="server"
        ErrorMessage="You must select a payment address" OnServerValidate="RadioButtons_Validate"></asp:CustomValidator>
</div>
<div class="form">
    <div class="header">
        <%=RM.GetString("ACCOUNT_NEW_Email_LABEL")%></div>
    <div class="body">
        <table border="0">
            <tr>
                <td align="right">
                    *<b><%=RM.GetString("ACCOUNT_NEW_Email_LABEL")%>:</b>
                </td>
                <td>
                    <asp:TextBox runat="server" MaxLength="255" Width="230" ID="OrderEmail" AutoCompleteType="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="OrderEmail"
                        Display="dynamic" ErrorMessage="<%$resources:StoreResources,ERROR_BLANK_Email %>"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="EmailValid" runat="server" ControlToValidate="OrderEmail"
                        Display="Dynamic" ErrorMessage="<%$resources:StoreResources,ERROR_INVALID_Email%>"
                        ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+"></asp:RegularExpressionValidator>
                    <%=RM.GetString("CHECKOUT_PAYPAL_PAYMENT_MESSAGE")%>
                </td>
            </tr>
        </table>
    </div>
</div>
<div class="form">
    <div class="header">
        <%=RM.GetString("CHECKOUT_BILLING_INFO_LABEL")%></div>
    <div class="body">
        <table id="tblAddresses" runat="server">
            <tr>
                <td>
                    <asp:RadioButton ID="rbBillingAddress" runat="server" GroupName="RadioButtonsAddressSelect"
                        Text='<%#RM.GetString("CHECKOUT_PAYPAL_ADDRESS_SELECT_LABEL")%>' /><br />
                    <asp:DropDownList runat="server" ID="AddressesList" DataValueField="AddressId" DataTextField="Name"
                        AutoPostBack="true" OnSelectedIndexChanged="AddressesList_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <ecf:AddressViewModule ID="AddressViewModule" runat="server" />
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton ID="rbBillingNewAddress" runat="server" GroupName="RadioButtonsAddressSelect"
                        Text='<%#RM.GetString("CHECKOUT_PAYPAL_ADDRESS_SELECT_LABEL")%>' />
                </td>
            </tr>
        </table>
        <ecf:AddressNewModule EnableClientScript="false" ID="AddressNewModule1" runat="server" />
    </div>
</div>
<div class="form">
    <div class="header">
        <%=RM.GetString("CHECKOUT_PAYMENT_OPTIONS_LABEL")%></div>
    <div class="body">
        <asp:DataList EnableViewState="true" runat="server" ID="PaymentOptionList" DataMember="PaymentMethod"
            DataKeyField="SystemKeyword" OnItemCreated="PaymentOptionList_ItemCreated" OnItemDataBound="PaymentOptionList_ItemDataBound">
            <ItemTemplate>
                <p>
                    <cms:GlobalRadioButton Font-Bold="true" GroupName="PaymentOption" Text='<%# DataBinder.Eval(Container.DataItem, "Name") %>'
                        runat="server" ID="PaymentOption" />
                    <br />
                    <br />
                    <asp:PlaceHolder EnableViewState="true" ID="PaymentOptionHolder" runat="server">
                    </asp:PlaceHolder>
                </p>
            </ItemTemplate>
        </asp:DataList>
    </div>
</div>
