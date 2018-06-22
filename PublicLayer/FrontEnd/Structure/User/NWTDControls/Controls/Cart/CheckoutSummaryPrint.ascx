<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CheckoutSummaryPrint.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.CheckoutSummaryPrint" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/LineItemTotal.ascx" TagName="LineItemTotal" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="cms" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/PriceLineModule.ascx" TagName="PriceLineModule" TagPrefix="cart" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Catalog/ISBN.ascx" TagName="ISBN" TagPrefix="NWTD" %>

<style>
    
    #nwtd-pageContent a {
		color:#000;
		text-decoration:none;
	}

    a {
		color:#000;
		text-decoration:none;
	}
	
	div.need-help,
	div.nwtd-Menu,
	img.curves,
	#nwtd-breadrumb,
	#nwtd-searchLink,
	div.nwtd-SearchByPublisher,
	#nwtd-Menu,
	#nwtd-submenu,
	div.footer-links,
	.buttons
	{
		display:none;
	}
	.nwtd-defaultPage #MainContentArea 
	{
		border-left:none;
		border-right:none;
		width: 90%;
	}
	
	#nwtd-pageContent.nwtd-homePageContent, #nwtd-pageContent.nwtd-defaultPageContent {
		margin:0;
		padding:0 0 0 0;
		width:100%;
		border: 0px;
	}
	
	#nwtd-page {
		background-color:white;
		background-image:none;
		background-repeat:repeat-y;
		float:left;
		padding-left:0;
		padding-right:0;
		width:100%;
	}
	
	#nwtd-pageHeader {
		display: none;
	}
	
	#MainContentArea .cart-header h1, #MainContentArea h1.cart-header {
		float:none;
		width:100%;
		border-bottom: 1px solid #000;
}
	
	.order-summary-shipping, .order-summary-billing, .order-summary-order {
		float:left;
		padding:10px 0 20px;
		margin: 0 8px 0 0;
		width:30%;
	}
	
	.order-summary-shipping th, .order-summary-billing th, .order-summary-order th {
		background-image:none;
		background-repeat:repeat-x;
		color:#FFFFFF;
		font-size:12px;
		padding:3px;
		margin: 0 0 0 0;
		text-align:left;
	}
	
	#center {
		margin-left:auto;
		margin-right:auto;
		width:100%;
	}

	table.nwtd-cartview {
		width:100%;
		/*border-top: 1px solid #000;*/
	}
	
	table.nwtd-cartview .nwtd-table-header th.col2-summary {
		width:200px;
	}
	
	table.nwtd-cartview th.col3 {
		width:100px;
	}
	
	table.nwtd-cartview th.col7 {
		width:80px;
	}
	
	table.nwtd-cartview th.col5 {
		width:45px;
	}
	
	table.nwtd-cartview .nwtd-table-header th.col2 {
		width:150px;
	}
	
	table.nwtd-cartview .nwtd-table-header th.col1 {
		display: none;
	}
	
	table.nwtd-cartview td.col1 {
		display: none;
}
	
	#print-logo {
		display: block;
	}
	#sidemenu 
	{
		display:none;
	}
	table.order-summary-details tr td address {
		width:140px;
	}
	div.nwtd-leftNav {
		display:none;
	}
</style>

<asp:Label runat="server" ID="OrderMessage" ForeColor="Red"></asp:Label>

<asp:Panel runat="server" ID="pnlOrderSubmitted" Visible="false">
	<h1>Thank you for your order</h1>
	<p>
		Your order confirmation number is <%=(this.CheckoutCart!= null)?this.CheckoutCart.GetString("WebConfirmation"):"" %>
	</p>
	<p>
		You can get status information regarding your order by logging into our website and selecting 'Check order status' from the Quick Links section.  Order status is typically available 24-48 hours after you place your order.  If you have any questions regarding this order or need to make changes, please contact us.
	</p>
	<p>
		We appreciate your business and look forward to serving you again.
	</p>

</asp:Panel>

<asp:Panel runat="server" ID="pnlOrderSummary">
	<%--The Delete Cart Confirmation Dialog--%>
<%--	<div class="nwtd-dialogBody nwtd-deleteCartDialog">
		<p>Are you sure you want to delete the wish list named <b><%=this.CheckoutCart.Name %></b>?</p>
		<asp:Button 
			runat="server" 
			ID="btnDeleteCart" 
			Text="Delete Wish List" 
			CssClass="delete-cart-confirm buttons" 
			onclick="btnDeleteCart_Click" 
		/>
		<input type="button" value="Cancel" class="cancel-btn buttons" />
	</div>
--%>
	<h1 class="cart-header"><asp:Literal runat="server" ID="lblTitle">Requisition Summary</asp:Literal></h1>
	<div class="float-right">
		<%--<input type="button value="Delete Cart" class="delete-cart-btn buttons nwtd-deleteCart" />--%>
			<asp:Button runat="server" ID="btnPrintHeader" Visible="false" Text="Print"  CssClass="print-btn buttons" OnClientClick="window.print()" />
		<asp:Panel CssClass="order-summary-requisition-notice" runat="server" ID="lblRequisitionNotSubmitted">
			<p>Your Requisition has not been submitted</p>
		</asp:Panel>
	</div>
	<asp:Button runat="server" ID="btnReturnToShippingInfo" CssClass="return-shipping-btn buttons" Text="Return to Shipping Info" onclick="btnReturnToShippingInfo_Click" />
	<asp:HyperLink ID="HyperLink1" CssClass="return-cart-btn buttons" NavigateUrl="~/Cart/View.aspx" runat="server">Back</asp:HyperLink>
	<br class="clear" />
	<div class="order-summary-shipping">
		<table cellpadding="0" cellspacing="0" border="0" width="300">
			<tr>
				<th>Shipping Information</th>
			</tr>
			<tr>
				<td>
					<table cellpadding="0" cellspacing="0" border="0" class="order-summary-details">
						<tr>
							<td><h4>Ship To:</h4></td>
							<td>
							<address>
								<%=ShippingAddress.FirstName %><br />
								<%=ShippingAddress.Line1 %><br />
								<%=ShippingAddress.Line2%><br />
								<%=ShippingAddress.City%>, 
								<%=ShippingAddress.State%> 
								<%=ShippingAddress.PostalCode%><br />
							</address>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	<div class="order-summary-billing">
		<table cellpadding="0" cellspacing="0" border="0" width="300">
			<tr>
				<th>Billing Information</th>
			    
			</tr>
			<tr>
				<td>
					<table cellpadding="0" cellspacing="0" border="0" class="order-summary-details">
						<tr>
							<td><h4>Bill To:</h4></td>
							<td>
							<address>
								<%=BillingAddress.FirstName %><br />
								<%=BillingAddress.Line1 %><br />
								<%=BillingAddress.Line2 %><br />
								<%=BillingAddress.City%>, 
								<%=BillingAddress.State %> 
								<%=BillingAddress.PostalCode%><br />
							</address>
							</td>                    
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	<div class="order-summary-order">
		<table cellpadding="0" cellspacing="0" border="0" width="100%">
			<tr>
				<th>Order Information</th>
			</tr>
			<tr>
				<td>
					<table cellpadding="0" cellspacing="0" border="0" class="order-summary-details">
						<tr>
							<td><h4>Contact Name:</h4></td>
							<td><%=this.CheckoutCart.GetString("OrderContactName")%></td>
						</tr>
						<tr>
							<td><h4>Contact Telephone:</h4></td>
							<td><%=this.CheckoutCart.GetString("OrderContactPhone")%></td>
						</tr>
						<tr>
							<td><h4>Contact Fax:</h4></td>
							<td><%=this.CheckoutCart.GetString("OrderFax")%></td>
						</tr>
						<tr>
							<td><h4>Purchase Order:</h4></td>
							<td><%=this.CheckoutCart.GetString("PurchaseOrder")%></td>
						</tr>
						<tr>
							<td><h4>Special Instructions:</h4></td>
							<td><p class="specialInstructions"><%=this.CheckoutCart.GetString("SpecialInstructions") %></p></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	<br  style="clear: both;" />
	<asp:GridView ID="gvCart" runat="server" SkinID="CartView" AutoGenerateColumns="false" DataKeyNames="LineItemId,Quantity" ShowFooter="true"  Width="100%" OnRowDataBound="gvCart_RowDataBound">
		<EmptyDataTemplate>
			This Wish List is Empty.
		</EmptyDataTemplate>
		<Columns>
			<asp:TemplateField FooterStyle-CssClass="cart-total" HeaderText="Title" HeaderStyle-CssClass="col2-summary">
				<ItemTemplate>
					<cms:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />
				</ItemTemplate>
				<FooterTemplate>
					Order Estimate before Tax and Shipping Charges:
				</FooterTemplate>
				<FooterStyle CssClass="cart-total"></FooterStyle>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Publisher No/ISBN13" HeaderStyle-CssClass="col3">
				<ItemTemplate>
					<NWTD:ISBN runat="server" Entry='<%#Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry( ((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId) %>' />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Grade" ItemStyle-CssClass="grade" HeaderStyle-CssClass="col4">
				<ItemTemplate>
					<%# Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry( ((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId).ItemAttributes["Grade"] %>
				</ItemTemplate>
				<ItemStyle CssClass="grade"></ItemStyle>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Year" HeaderStyle-CssClass="col5">
				<ItemTemplate>
					<asp:Literal runat="server" ID="litYear" />
				</ItemTemplate>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="col6">
				<ItemTemplate>
					<cart:PriceLineModule ID="PriceLineModule1" runat="server" LineItem="<%#Container.DataItem%>"></cart:PriceLineModule>
				</ItemTemplate>
                <ItemStyle CssClass="price"></ItemStyle>
			</asp:TemplateField>
			<asp:TemplateField HeaderText="Qty Charged" HeaderStyle-CssClass="col7">
				<ItemTemplate>
					<asp:Literal runat="server" ID="litQuantityCharged" />
				</ItemTemplate>
                <ItemStyle CssClass="qty-charged"></ItemStyle>
			</asp:TemplateField>
			<asp:TemplateField HeaderStyle-CssClass="col9">
				<HeaderTemplate>
					Qty Gratis
				</HeaderTemplate>
				<ItemTemplate>
					<%#  ((Mediachase.Commerce.Orders.LineItem)Container.DataItem)["Gratis"] %>
				</ItemTemplate>
				<ItemStyle CssClass="qty-gratis"></ItemStyle>
			</asp:TemplateField>
			<asp:TemplateField FooterStyle-CssClass="footer-cart-total" ItemStyle-CssClass="item-total" HeaderText="Total" HeaderStyle-CssClass="col8">
				<ItemTemplate>
					<NWTD:LineItemTotal LineItem="<%#Container.DataItem %>" runat="server" ID="LineItemTotal" />
				</ItemTemplate>
                <ItemStyle CssClass="total"></ItemStyle>
			</asp:TemplateField>
		</Columns>
	</asp:GridView>


	
	<asp:Panel runat="server" ID="pnlLevelBNotifications" CssClass="order-summary-notification-panel">
		<p>Your Requisition has not been submitted.</p>
		<p>To place your order, print your requesition and fax, mail, or e-mail it to us along with a purchase order.</p>
        <p><b>All no-charge items are subject to approval.<br />Prices are set by our publishers and are subject to change.</b></p> 
	</asp:Panel>

	<asp:Panel CssClass="nwtd-cartTaxAndShippingTotals" runat="server" ID="pnlShippingAndTax" Visible="false" >
		<table>
			<tr>
				<td class="lblTS">Tax:</td>
				<td><%=CurrencyFormatter.FormatCurrency( NWTD.Orders.Cart.CartTax(this.CheckoutCart), this.CheckoutCartHelper.Cart.BillingCurrency) %></td>
			</tr>
			<tr>
				<td class="lblTS">*Shipping:</td>
				<td><%=CurrencyFormatter.FormatCurrency( NWTD.Orders.Cart.CartShippingCharge(this.CheckoutCart), this.CheckoutCartHelper.Cart.BillingCurrency) %></td>
			</tr>
			<tr>
				<td class="lblTS">Total:</td>
				<td><%=CurrencyFormatter.FormatCurrency(NWTD.Orders.Cart.CartTotal(this.CheckoutCart, true, true), this.CheckoutCartHelper.Cart.BillingCurrency)%></td>
			</tr>
		</table>
		
	</asp:Panel>
	<br class="clear" />

	<div class="float-right">
		
		<asp:Button runat="server" ID="btnSubmitCart" Text="Submit" CssClass="submit-btn buttons" onclick="btnSubmitCart_Click" />
		<asp:Button runat="server" ID="btnPrintFooter" Text="Print"  CssClass="print-btn-blue buttons" OnClientClick="window.print()" />
		  
	</div>
	<asp:Panel runat="server" ID="pnlLevelANotifications" CssClass="order-summary-notification-panel" Visible="false">
		<p class="shipping-rate-notice">*Shipping rates are based on standard delivery charges.<br /> <br /> <b>All no-charge items are subject to approval.</b></p>
	</asp:Panel>
	<!-- submit - bottom right under total, print - next to H1, Delete - save over to the right with submit, return to shipping - bottom left  /-->

</asp:Panel>