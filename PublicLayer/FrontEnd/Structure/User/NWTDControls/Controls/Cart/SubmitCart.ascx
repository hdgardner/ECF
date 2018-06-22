<%-- This is the OLD checkout control, which had multiple pages. It was replaced with Checkout.ascx--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitCart.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.SubmitCart" %>
<%@ Register Assembly="OakTree.Web.UI" Namespace="OakTree.Web.UI.WebControls" TagPrefix="OakTree" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/LineItemTotal.ascx" TagName="LineItemTotal" TagPrefix="NWTD" %>
<%@ Register Src="~/Structure/User/NWTDControls/Controls/Cart/CartItemModule.ascx" TagName="CartItemModule" TagPrefix="cms" %>
<%@ Register Src="~/Structure/Base/Controls/Cart/SharedModules/PriceLineModule.ascx" TagName="PriceLineModule" TagPrefix="cart" %>
<%--<h2>Complete Cart: <%=this.CheckoutCart.Name %></h2>--%>
<asp:BulletedList runat="server" ID="blStepsList" CssClass="nwtd-checkoutWizardStepsList" DataTextField="Title">
</asp:BulletedList>
<asp:Wizard 
	runat="server" 
	ID="wzSubmitCart" 
	CssClass="nwtd-checkout" 
	OnFinishButtonClick="wzSubmitCart_FinishButtonClick" 
	OnActiveStepChanged="wzSubmitCart_ActiveStepChanged" 
	OnNextButtonClick="wzSubmitCart_NextButtonClick" 
	DisplaySideBar="false">
	<StartNavigationTemplate>
		<asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Next" CausesValidation="true" ValidationGroup="LevelBBillingValidationGroup" />
	</StartNavigationTemplate>
	<WizardSteps>
		<asp:WizardStep runat="server" ID="wsShippingAddress" Title="Shipping Information">
			<div class="nwtd-checkoutShippingStep">
				<h1>Shipping Information</h1>
				<%--Level B Shipping Address--%>
				<div runat="server" id="fsLevelBShippingAddress" class="nwtd-cart">
					<h2>Shipping Address</h2>
					<div class="nwtd-form-field">
						<asp:Label runat="server" AssociatedControlID="tbShippingAddress" Text="Address" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingAddress"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvShippingAddress" ControlToValidate="tbShippingAddress" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblShippingCity" AssociatedControlID="tbShippingCity" Text="City" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingCity"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvShippingCity" ControlToValidate="tbShippingCity" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<OakTree:StateSelector EnableViewState="false" LabelText="State" CssClass="nwtd-form-field" ID="ssShippingState" runat="server" ContainerType="DIV" Country="US" LabelPosition="LEFT" SelectedState="AL" StateDisplaymode="FULL_NAME" />
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblShippingZip" AssociatedControlID="tbShippingZip" Text="Zip" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingZip"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvShippingZip" ControlToValidate="tbShippingZip" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Day Phone Number" ID="pnShippingDayPhone" runat="server" />
					<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Fax" ID="pnShippingFax" runat="server" />
				</div>
				
				<%--Level A Shipping Address--%>
				<div runat="server" id="fsLevelAShippingAddress" class="nwtd-cart" visible="false">
					<h2>Shipping Address</h2>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblShippingAddress" AssociatedControlID="ddlLevelAShippingAddress" Text="Shipping Address" />
						<asp:DropDownList runat="server" ID="ddlLevelAShippingAddress" DataValueField="Name" DataTextField="Line1" />
					</div>
				</div>
				
				<%--Additional Shipping Information (Level A and Level B)--%>
				<div class="nwtd-cart">
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblShippingSpecialInstructions" AssociatedControlID="tbShippingSpecialInstructions" Text="Special Instructions" />
						<asp:TextBox runat="server" TextMode="MultiLine" ID="tbShippingSpecialInstructions"></asp:TextBox>
					</div>
				</div>
			</div>
		</asp:WizardStep>
				
		<asp:WizardStep runat="server" ID="wsBillingAddress" Title="Billing Information">
			<div class="nwtd-checkoutBillingStep">
				<h1>Billing Information</h1>
				<%--Level B Billing Address--%>
				<div runat="server" id="fsLevelBOrderAddress" class="nwtd-cart">
					<h2>Billing Address</h2>
					<asp:LinkButton runat="server" ID="lbCopyShippingAddress" OnCommand="lbCopyShippingAddress_Command"  Text="Copy From Shipping Address" ></asp:LinkButton>
					<div class="nwtd-form-field">
						<asp:Label ID="Label1" runat="server" AssociatedControlID="tbBillingAddress" Text="Address" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingAddress"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvBillingAddress" ControlToValidate="tbBillingAddress" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="Label2" AssociatedControlID="tbBillingCity" Text="City" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingCity"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvBillingCity" ControlToValidate="tbBillingCity" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<OakTree:StateSelector EnableViewState="false" LabelText="State" CssClass="nwtd-form-field" ID="ssBillingState" runat="server" ContainerType="DIV" Country="US" LabelPosition="LEFT" SelectedState="AL" StateDisplaymode="FULL_NAME" />
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="Label3" AssociatedControlID="tbBillingZip" Text="Zip" />
						<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingZip"></asp:TextBox>
						<span class="required">*</span>
						<asp:CustomValidator EnableClientScript="false" ValidateEmptyText="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" ID="cvBillingZip" ControlToValidate="tbBillingZip" ErrorMessage="Required" OnServerValidate="ValidateLevelBField" />
					</div>
					<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Day Phone Number" ID="pnBillingDayPhone" runat="server" />
					<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Fax" ID="pnBillingFax" runat="server" />
				</div>
				
				<%--Level A Billing Address--%>
				<div runat="server" id="fsLevelAOrderAddress" class="nwtd-cart" visible="false">
					<h2>Billing Address</h2>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblLevelABillingAddress" />
						<address runat="server" id="Address1">
							<%=BillingAddress.Line1 %><br />
							<%=BillingAddress.Line2 %><br />
							<%=BillingAddress.City%><br />
							<%=BillingAddress.State %><br />
							<%=BillingAddress.PostalCode%><br />
						</address>
					</div>
				</div>
				
				<%--Additional Billing Information (Level A and Level B)--%>
				<div class="nwtd-cart">
					<h2>Order Information</h2>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblBillingPurchaseOrder" AssociatedControlID="tbBillingPurchaseOrder" Text="Purchase Order:" />
						<asp:TextBox runat="server" ID="tbBillingPurchaseOrder" />
					</div>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblBillingSpecialInstructions" AssociatedControlID="tbBillingSpecialInstructions" Text="Special Instructions:" />
						<asp:TextBox runat="server" ID="tbBillingSpecialInstructions" TextMode="MultiLine"></asp:TextBox>
					</div>
					<div class="nwtd-form-field">
						<asp:Label runat="server" ID="lblBillingContactName" AssociatedControlID="tbBillingContactName" Text="Contact Name" />
						<asp:TextBox runat="server" ID="tbBillingContactName" />
					</div>
					<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Contact Phone Number" ID="pnBillingContact" runat="server" />
				</div>
			</div>
		</asp:WizardStep>
		
		<asp:WizardStep runat="server" ID="wsOrderSummary" Title="Order Confirmation">
			<div class="nwtd-checkoutSummaryStep">
				<h1>Order Summary</h1>
				<table class="order-detail" border="0">
					<tr>
						<th style="text-align:right;">
							<h4>Shipping Information</h4>
						</th>
						<th>
							<asp:LinkButton 
								CommandName="GoToAddress" 
								CommandArgument="ShipTo" 
								runat="server" 
								ID="lbGoToShippingAddress" 
								Text="Edit" 
								OnCommand="lbGoToAddress_Command" />
						</th>
						<th style="text-align:right;">
							<h4>Billing Information</h4>
						</th>
						<th>
							<asp:LinkButton 
								CommandName="GoToAddress" 
								CommandArgument="BillTo" 
								runat="server" 
								ID="lbGoToBillingAddress" Text="Edit" 
								OnCommand="lbGoToAddress_Command" />
						</th>
						<th style="text-align:right;">
							<h4>Order Information</h4>
						</th>
						<th><asp:LinkButton 
								CommandName="GoToAddress" 
								CommandArgument="BillTo" 
								runat="server" 
								ID="lbGoToBillingInformation" 
								Text="Edit" 
								OnCommand="lbGoToAddress_Command" />
						</th>
					</tr>
					<tr>
						<td style="text-align:right;" rowspan="3">
							<h4>Ship To:</h4>
						</td>
						<td rowspan="3">
							<address>
								<%=ShippingAddress.Line1 %><br />
								<%=ShippingAddress.Line2%><br />
								<%=ShippingAddress.City%>, 
								<%=ShippingAddress.State%> 
								<%=ShippingAddress.PostalCode%><br />
							</address>
						</td>
						<td style="text-align:right;" rowspan="4">
							<h4>Bill To:</h4>
						</td>
						<td style="width:15%;" rowspan="4">
							<address>
								<%=BillingAddress.Line1 %><br />
								<%=BillingAddress.Line2 %><br />
								<%=BillingAddress.City%>, 
								<%=BillingAddress.State %>
								<%=BillingAddress.PostalCode%><br />
							</address>
						</td>
						<td style="text-align:right">
							<h4>Contact Name:</h4>
						</td>
						<td><%=this.tbBillingContactName.Text %></td>
					</tr>
					<tr>
						<td style="text-align:right;">
							<h4>Contact Telephone:</h4>
						</td>
						<td><%=this.pnBillingContact.Text%></td>
					</tr>
					<tr>
						<td style="text-align:right;">
							<h4>Purchase Order:</h4>
						</td>
						<td><%=this.tbBillingPurchaseOrder.Text %></td>
					</tr>
					<tr>
						<td style="text-align:right;">
							<h4>Special Instructions</h4>
						</td>
						<td>
							<%=this.tbBillingSpecialInstructions.Text %>
						</td>
						<td style="text-align:right;">
							<h4>Special Instructions:</h4>
						</td>
						<td><%=this.tbShippingSpecialInstructions.Text %></td>
					</tr>
					<tr><td colspan="6"><hr style="width:100%;" /></td></tr>
				</table>
				<asp:GridView ID="gvCart" runat="server" SkinID="CartView" AutoGenerateColumns="false" DataKeyNames="LineItemId,Quantity" ShowFooter="true" OnRowDataBound="gvCart_RowDataBound">
					<EmptyDataTemplate>
						This Wish List is Empty.
					</EmptyDataTemplate>
					<Columns>
						<asp:TemplateField FooterStyle-CssClass="cart-total" HeaderText="Title">
							<ItemTemplate>
								<cms:CartItemModule LineItem="<%#Container.DataItem %>" ID="CartItemModule1" runat="server" />
							</ItemTemplate>
							<FooterTemplate>
								Order Estimate before Tax and Shipping Charges:
							</FooterTemplate>
							<FooterStyle CssClass="cart-total"></FooterStyle>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Publisher No/ISBN13">
							<ItemTemplate>
								<%# Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry( ((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId).ItemAttributes["ISBN10"] %><br />
								<%# Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry( ((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId).ItemAttributes["ISBN13"] %>
							</ItemTemplate>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Grade" ItemStyle-CssClass="grade">
							<ItemTemplate>
								<%# Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry( ((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId).ItemAttributes["Grade"] %>
							</ItemTemplate>
							<ItemStyle CssClass="grade"></ItemStyle>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Year" HeaderStyle-CssClass="year" ItemStyle-CssClass="year">
							<ItemTemplate>
								<%# float.Parse(Mediachase.Commerce.Catalog.CatalogContext.Current.GetCatalogEntry(((Mediachase.Commerce.Orders.LineItem)Container.DataItem).CatalogEntryId).ItemAttributes["Year"].ToString()).ToString("#") %>
							</ItemTemplate>
							<HeaderStyle CssClass="year"></HeaderStyle>
							<ItemStyle CssClass="year"></ItemStyle>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="price" ItemStyle-CssClass="price">
							<ItemTemplate>
								<cart:PriceLineModule ID="PriceLineModule1" runat="server" LineItem="<%#Container.DataItem%>"></cart:PriceLineModule>
							</ItemTemplate>
							<HeaderStyle CssClass="price"></HeaderStyle>
							<ItemStyle CssClass="price"></ItemStyle>
						</asp:TemplateField>
						<asp:TemplateField HeaderText="Qty Charged" ItemStyle-CssClass="quantity" HeaderStyle-CssClass="quantity">
							<ItemTemplate>
								<asp:Literal runat="server" ID="litQuantityCharged" />
							</ItemTemplate>
							<HeaderStyle CssClass="quantity"></HeaderStyle>
							<ItemStyle CssClass="quantity"></ItemStyle>
						</asp:TemplateField>
						<asp:TemplateField ItemStyle-CssClass="quantity" HeaderStyle-CssClass="quantity">
							<HeaderTemplate>
								Qty <a href="#">Gratis</a>
							</HeaderTemplate>
							<ItemTemplate>
								<%#  ((Mediachase.Commerce.Orders.LineItem)Container.DataItem)["Gratis"] %>
							</ItemTemplate>
							<HeaderStyle CssClass="quantity"></HeaderStyle>
							<ItemStyle CssClass="quantity"></ItemStyle>
						</asp:TemplateField>
						<asp:TemplateField FooterStyle-CssClass="cart-total" ItemStyle-CssClass="item-total" HeaderText="Total" HeaderStyle-CssClass="item-total">
							<ItemTemplate>
								<NWTD:LineItemTotal LineItem="<%#Container.DataItem %>" runat="server" ID="LineItemTotal" />
							</ItemTemplate>
							<FooterStyle CssClass="cart-total"></FooterStyle>
							<HeaderStyle CssClass="item-total"></HeaderStyle>
							<ItemStyle CssClass="item-total"></ItemStyle>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
				<asp:Panel CssClass="nwtd-cartTaxAndShippingTotals" runat="server" ID="pnlShippingAndTax" Visible="false" >
					<table>
						<tr>
							<td class="lblTS">Tax:</td>
							<td><%=CurrencyFormatter.FormatCurrency( NWTD.Orders.Cart.CartTax(this.CheckoutCart), this.CheckoutCartHelper.Cart.BillingCurrency) %></td>
						</tr>
						<tr>
							<td class="lblTS">Shipping:</td>
							<td><%=CurrencyFormatter.FormatCurrency( NWTD.Orders.Cart.CartShippingCharge(this.CheckoutCart), this.CheckoutCartHelper.Cart.BillingCurrency) %></td>
						</tr>
						<tr>
							<td class="lblTS">Total:</td>
							<td><%=CurrencyFormatter.FormatCurrency(NWTD.Orders.Cart.CartTotal(this.CheckoutCart, true, true), this.CheckoutCartHelper.Cart.BillingCurrency)%></td>
						</tr>
					</table>
				</asp:Panel>
			</div>
		</asp:WizardStep>
		<asp:WizardStep runat="server" StepType="Complete">
			<h2>Thank you for your submission</h2>
			<p>Your order has been submitted.</p>
		</asp:WizardStep>
	</WizardSteps>
	<FinishNavigationTemplate>
		<asp:Button ID="FinishPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
		<asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Done" />
	</FinishNavigationTemplate>
	<StepNavigationTemplate>
		<asp:Button ID="StepPreviousButton" runat="server" CommandName="MovePrevious" Text="Previous" />
		<asp:Button ID="StepNextButton" CausesValidation="true" ValidationGroup="LevelBBillingValidationGroup" runat="server" CommandName="MoveNext" Text="Next" />
	</StepNavigationTemplate>
</asp:Wizard>

