<%--This is the NEW checkout control, replacing SubmitCart.ascx.  --%>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Checkout.ascx.cs" Inherits="Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart.Checkout" %>
<%@ Register Assembly="OakTree.Web.UI" Namespace="OakTree.Web.UI.WebControls" TagPrefix="OakTree" %>

	<h1 class="cart-header">Shipping Information</h1>
    <%-- On 08/02/17, Heath created the following 'div' & asp panel to display a 'click continue for printer friendly document' message --%>
    <div class="float-right">
		<asp:Panel CssClass="shipping-info-continue-for-print-friendly" runat="server" ID="lblContinueForPrintFriendlyMsg">
			<p>Click 'Continue' for a printer friendly summary</p>
		</asp:Panel>
	</div>
	<br class="clear" />
    

	<asp:LinkButton CssClass="return-cart-btn buttons" runat="server" 
	onclick="btnReturnToCart_Click" CausesValidation="false">Back</asp:LinkButton>
<%--    <asp:HyperLink ID="HyperLink1" CssClass="return-cart-btn buttons"  NavigateUrl="~/Cart/View.aspx" runat="server">Back</asp:HyperLink>
--%>    	<asp:Button ID="btnCompleteAddresses" CssClass="review-order-btn buttons" runat="server" Text="Review Order" onclick="btnCompleteAddresses_Click" />
	<%--Level B Shipping Address--%>
	<br class="clear" />
	<div runat="server" id="fsLevelBShippingAddress" class="nwtd-cart left-block nwtd-levelBShipping">
		<h2>Shipping Address</h2>
		<div class="nwtd-form-field">
            <%-- On 08/02/17, Heath changed the following field's displayed label value from "Name" to "Ship To" --%>
			<asp:Label  runat="server" AssociatedControlID="tbShippingAddressName">Ship To</asp:Label>
			<asp:TextBox runat="server" ID="tbShippingAddressName"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator runat="server" ControlToValidate="tbShippingAddressName" ErrorMessage="Required" />

		</div>
		<div class="nwtd-form-field">
			<asp:Label ID="Label1" runat="server" AssociatedControlID="tbShippingAddress" Text="Address" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingAddress"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator runat="server" ControlToValidate="tbShippingAddress" ErrorMessage="Required" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblShippingCity" AssociatedControlID="tbShippingCity" Text="City" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingCity"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbShippingCity" ErrorMessage="Required" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblShippingState" AssociatedControlID="ddlShippingState" Text="State" />
			<asp:DropDownList runat="server" ID="ddlShippingState">
				<asp:ListItem Text="Oregon" Value="OR" />
				<asp:ListItem Text="Washington" Value="WA" />
				<asp:ListItem Text="Alaska" Value="AK" />
			</asp:DropDownList>
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblShippingZip" AssociatedControlID="tbShippingZip" Text="Zip" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbShippingZip"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ControlToValidate="tbShippingZip" ErrorMessage="Required" />
			<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"  Display="Dynamic" ControlToValidate="tbShippingZip" ValidationExpression="\d{5}(-\d{4})?" ErrorMessage="Invalid US Postal Code" />
		</div>
	<%--	<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Day Phone Number" ID="pnShippingDayPhone" runat="server" />
		<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Fax" ID="pnShippingFax" runat="server" />
--%></div>
	
	<%--Level A Shipping Address--%>
	<div runat="server" id="fsLevelAShippingAddress" class="nwtd-cart left-block" visible="false">
		<h2>Shipping Address</h2>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblShippingAddress" AssociatedControlID="ddlLevelAShippingAddress" Text="Shipping Address" />
			<asp:DropDownList 
				runat="server" 
				ID="ddlLevelAShippingAddress" 
				DataValueField="Name" 
				OnSelectedIndexChanged="ddlLevelAShippingAddress_SelectedIndexChanged"
				DataTextField="FirstName"
				AutoPostBack="true" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator23" runat="server" Display="Dynamic" ControlToValidate="ddlLevelAShippingAddress" InitialValue="-1" ErrorMessage="Required" />
		</div>
		<div class="nwtd-selected-shipping-address nwtd-form-field">
			<address>
				<asp:UpdatePanel ID="UpdatePanel1" runat="server">
					<Triggers>
						<asp:AsyncPostBackTrigger ControlID="ddlLevelAShippingAddress" />
					</Triggers>
					<ContentTemplate>

						<asp:Literal runat="server" ID="litSelectedShippingLine1" /><br />
						<asp:Literal runat="server" ID="litSelectedShippingLine2" /><br />
						<asp:Literal runat="server" ID="litSelectedShippingCity" /><br />
						<asp:Literal runat="server" ID="litSelectedShippingState" /><br />
						<asp:Literal runat="server" ID="litSelectedShippingZip" />
					</ContentTemplate>
				</asp:UpdatePanel>
			</address>
		</div>
	</div>
	
 <asp:Panel ID="Panel1" runat="server" DefaultButton="btnCompleteAddresses">

	<%--Level B Billing Address--%>
	<div runat="server" id="fsLevelBOrderAddress" class="nwtd-cart  nwtd-levelBBilling">
		<h2>Billing Address</h2>
		<div class="nwtd-form-field">
            <%-- On 08/02/17, Heath changed the following field's displayed label value from "Name" to "Bill To" --%>
			<asp:Label ID="Label5"  runat="server" AssociatedControlID="tbBillingAddressName">Bill To</asp:Label>
			<asp:TextBox runat="server" ID="tbBillingAddressName"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator  runat="server" ControlToValidate="tbBillingAddressName" ErrorMessage="Required" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label ID="Label2" runat="server" AssociatedControlID="tbBillingAddress" Text="Address" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingAddress"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="tbBillingAddress" ErrorMessage="Required" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="Label3" AssociatedControlID="tbBillingCity" Text="City" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingCity"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="tbBillingCity" ErrorMessage="Required" />
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblBillingState"  Text="State" AssociatedControlID="ddlBillingState" />
			<asp:DropDownList runat="server" ID="ddlBillingState">
				<asp:ListItem Text="Oregon" Value="OR" />
				<asp:ListItem Text="Washington" Value="WA" />
				<asp:ListItem Text="Alaska" Value="AK" />
			</asp:DropDownList>
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="Label4" AssociatedControlID="tbBillingZip" Text="Zip" />
			<asp:TextBox runat="server" EnableViewState="false" ID="tbBillingZip"></asp:TextBox>
			<span class="required">*</span>
			<asp:RequiredFieldValidator ID="RequiredFieldValidator5"  Display="Dynamic" runat="server" ControlToValidate="tbBillingZip" ErrorMessage="Required" />
			<asp:RegularExpressionValidator runat="server" Display="Dynamic" ControlToValidate="tbBillingZip" ValidationExpression="\d{5}(-\d{4})?" ErrorMessage="Invalid US Postal Code" />
		</div>
		<%--
		<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Day Phone Number" ID="pnBillingDayPhone" runat="server" />
		<OakTree:PhoneNumberField CssClass="nwtd-form-field phonenumber" LabelText="Fax" ID="pnBillingFax" runat="server" />
		--%>	
	</div>
	
	<%--Level A Billing Address--%>
	<div runat="server" id="fsLevelAOrderAddress" class="nwtd-cart" visible="false">
		<h2>Billing Address</h2>
		<div class="nwtd-form-field">
			<%=BillingAddress.FirstName %>
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
	<div class="nwtd-cart order-information">
		<h2>Order Information</h2>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblBillingPurchaseOrder" AssociatedControlID="tbBillingPurchaseOrder" Text="Purchase Order:" />
			<asp:TextBox runat="server" ID="tbBillingPurchaseOrder" />

			<asp:RequiredFieldValidator runat="server" ControlToValidate="tbBillingPurchaseOrder" ID="rqvBillingPurchaseOrder"  ErrorMessage="Required" Display="Dynamic"/>
			<span class="required" runat="server" ID="lblPORequired">*</span>
		</div>
		<div class="nwtd-form-field special-instructions">
			<asp:Label runat="server" ID="lblBillingSpecialInstructions" AssociatedControlID="tbBillingSpecialInstructions" CssClass="special-instructions" Text="Special Instructions:" />
			<asp:TextBox runat="server" ID="tbBillingSpecialInstructions" TextMode="MultiLine"></asp:TextBox>
		</div>
		<div class="nwtd-form-field">
			<asp:Label runat="server" ID="lblBillingContactName" AssociatedControlID="tbBillingContactName" Text="Contact Name:" />
			<asp:TextBox runat="server" ID="tbBillingContactName" />
			<span class="required">*</span>
			
			<asp:RequiredFieldValidator runat="server" ControlToValidate="tbBillingContactName" ErrorMessage="Required" Display="Dynamic" />
		</div>
		<div class="nwtd-form-field">
			<OakTree:PhoneNumberField ContainerType="SPAN" CssClass="phonenumber" LabelText="Contact Phone Number:" ID="pnBillingContact" runat="server" />
			<span class="required">*</span>
			<asp:RequiredFieldValidator runat="server" ControlToValidate="pnBillingContact" ErrorMessage="Required" Display="Dynamic" />
			<asp:CustomValidator runat="server" ControlToValidate="pnBillingContact" 
				ErrorMessage="Invalid Phone Number" Display="Dynamic" 
				 OnServerValidate="PhoneNumber_ServerValidate" />
		</div>

        <%-- On 08/02/17, Heath commented out the following "Fax" fields as they are no longer required --%>
        <%-- 
		    <div class="nwtd-form-field">
			    <OakTree:PhoneNumberField ContainerType="SPAN"  CssClass=" phonenumber" LabelText="Fax Number:" ID="pnFax" runat="server" />
			    <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="pnFax" ValidateEmptyText="true"
				    ErrorMessage="Invalid Phone Number" Display="Dynamic" 
				     OnServerValidate="PhoneNumber_ServerValidate" />
		    </div>
        --%>
	</div>
</asp:Panel>