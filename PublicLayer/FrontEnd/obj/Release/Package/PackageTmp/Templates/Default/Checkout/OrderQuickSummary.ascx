<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderQuickSummary.ascx.cs" Inherits="Mediachase.Cms.Website.Templates.Default.Checkout.OrderQuickSummary" %>
<div class="order-summary">
        <fieldset>
            <legend>Your Order Summary</legend>
            <table width="100%" border="0" cellpadding="2" cellspacing="0" id="cart-summary-table">
            <tr class="promotion" id="promotion-exclusive" style='display:none'>
                <td class="title">Order Discount:</td>
                <td id="exclusive-discount-value" class="value"> $0.00</td>
            </tr>
            <tr class="sub-total" id="sub-total">
                <td class="title"><strong>Sub-total:</strong></td>
                <td class="value" id="subTotalValue">$499.00</td>
            </tr>
            <tr class="shipping" id="shipping">
                <td class="title">Shipping:</td>
                <td class="value" id="cartShippingCostValue">N/A</td>
            </tr>
            <tr id="tax-na" class="tax" >
                <td class="title">Tax: </td>
                <td class="value">N/A</td>
            </tr>
            <tr class="promotion" id="promotion-inclusive" style='display:none'>
                <td class="title">Order Discount:</td>
                <td id="inclusive-discount-value" class="value"> $0.00</td>
            </tr>
            <tr class="total" id="total">
                <td class="title">Total:</td>
                <td class="value" id="cartTotalValue">$499.00</td>
            </tr>
            </table>
        </fieldset>
</div>