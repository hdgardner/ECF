<%@ Control Language="C#" Inherits="Mediachase.Cms.Website.Templates.Default.Modules.CheckoutWizardModule" Codebehind="CheckoutWizardModule.ascx.cs" %>
<%@ Register Src="CheckoutAddressModule.ascx" TagName="CheckoutAddressModule" TagPrefix="ecf" %>
<%@ Register Src="CheckoutShippingModule.ascx" TagName="CheckoutShippingModule" TagPrefix="ecf" %>
<%@ Register Src="CheckoutPaymentModule.ascx" TagName="CheckoutPaymentModule" TagPrefix="ecf" %>
<%@ Register Src="CheckoutConfirmModule.ascx" TagName="CheckoutConfirmModule" TagPrefix="ecf" %>

<div id="ecf-checkout">
<asp:Wizard ID="CheckoutWizard" OnLoad="CheckoutWizard_Load" OnNextButtonClick="OnNextButtonClick" 
	OnPreviousButtonClick="OnPreviousButtonClick" OnFinishButtonClick="OnFinishButtonClick"
	 runat="server" OnActiveStepChanged="OnActiveStepChanged" 
	 DisplaySideBar="False" style="width: 100%" StartNextButtonText='<%# RM.GetString("CHECKOUT_ADDRESS_PROCEED") %>' 
	 StepPreviousButtonText='<%# RM.GetString("CHECKOUT_BACK_BUTTON") %>' 
	 FinishCompleteButtonText='<%# RM.GetString("CHECKOUT_ORDER_CONFIRMATION_BUTTON") %>' 
	 FinishPreviousButtonText='<%# RM.GetString("CHECKOUT_BACK_BUTTON") %>' 
	 CancelDestinationPageUrl="~" NavigationButtonStyle-CssClass="next-button button" FinishCompleteButtonStyle-CssClass="finish-button button">
    <WizardSteps>
        <asp:WizardStep runat="server" ID="ShippingAddressStep" 
			Title='<%#RM.GetString("CHECKOUT_ADDRESS_LABEL")%>'>
            <ecf:CheckoutAddressModule id="CheckoutShippingAddress" runat="server"/>
        </asp:WizardStep>
        <asp:WizardStep runat="server" ID="ShippingOptionsStep" 
			Title='<%#RM.GetString("CHECKOUT_PAYPAL_WIZARD_SHIPPING") %>'>
            <ecf:CheckoutShippingModule ID="CheckoutShipping" runat="server"/>
        </asp:WizardStep>
        <asp:WizardStep runat="server" ID="PaymentStep" 
			Title='<%#RM.GetString("CHECKOUT_PAYPAL_WIZARD_PAYMENT") %>'>
            <ecf:CheckoutPaymentModule ID="CheckoutPayment" runat="server"/>
        </asp:WizardStep>
        <asp:WizardStep runat="server" ID="FinalStep" 
			Title='<%#RM.GetString("CHECKOUT_PAYPAL_WIZARD_CONFIRM") %>'>
            <ecf:CheckoutConfirmModule runat="server" ID="CheckoutConfirm"/>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
</div>