using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Mediachase.Cms;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Commerce.Engine.Template;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Exceptions;
using Mediachase.Commerce.Profile;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Marketing;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// The controller class for the checkout wizard. This class coordinates all the wizard views making sure the
    /// correct view is displayed and data is available for that view.
    /// </summary>
    public partial class CheckoutWizardModule : BaseStoreUserControl
    {
		public readonly string _SessionShippingAddressKey = "CheckoutWizard-ShippingAddressId";
		public readonly string _SessionBillingAddressKey = "CheckoutWizard-BillingAddressId";

		private const string _BillingAddressString = "Billing Address";
		private const string _ShippingAddressString = "Shipping Address";
		private const string _PreferredBillingAddressString = "Preferred Billing Address";
		private const string _PreferredShippingAddressString = "Preferred Shipping Address";

        string _StoreEmail = String.Empty;

        /// <summary>
        /// Gets the store email.
        /// </summary>
        /// <value>The store email.</value>
        public string StoreEmail
        {
            get
            {
                if (String.IsNullOrEmpty(_StoreEmail))
                    _StoreEmail = GlobalVariable.GetVariable("email", CMSContext.Current.SiteId);

                return _StoreEmail;
            }
        }

        string _StoreTitle = String.Empty;
        /// <summary>
        /// Gets the store title.
        /// </summary>
        /// <value>The store title.</value>
        public string StoreTitle
        {
            get
            {
                if (String.IsNullOrEmpty(_StoreTitle))
                    _StoreTitle = GlobalVariable.GetVariable("title", CMSContext.Current.SiteId);

                return _StoreTitle;
            }
        }

        private CartHelper _CartHelper = new CartHelper(Cart.DefaultName);
        /// <summary>
        /// Gets the cart helper.
        /// </summary>
        /// <value>The cart helper.</value>
        public CartHelper CartHelper{get{return _CartHelper;}}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (CartHelper.IsEmpty)
                Response.Redirect(NavigationManager.GetUrl("ShoppingCart"));

            if (Session[MarketingContext.ContextConstants.Coupons] != null)
                SetCouponCode(Session[MarketingContext.ContextConstants.Coupons].ToString());

			if (!this.IsPostBack)
			{
				InitializeData();
				DataBind();
			}

            // Set default button
            if (CheckoutWizard.ActiveStepIndex == 0)
            {
                Page.Form.DefaultButton = CheckoutWizard.FindControl("StartNavigationTemplateContainerID$StartNextButton").UniqueID;
            }
        }

        /// <summary>
        /// Sets the coupon code.
        /// </summary>
        /// <param name="couponCode">The coupon code.</param>
        private void SetCouponCode(string couponCode)
        {
            List<string> couponList = new List<string>();
            couponList.Add(couponCode);

            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(MarketingContext.ContextConstants.Coupons))
                MarketingContext.Current.MarketingProfileContext.Add(MarketingContext.ContextConstants.Coupons, couponList);
            else
                MarketingContext.Current.MarketingProfileContext[MarketingContext.ContextConstants.Coupons] = couponList;
        }

        /// <summary>
        /// Handles the Load event of the CheckoutWizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void CheckoutWizard_Load(object sender, EventArgs e)
        {
            // Perform data checks, since the wizard should be in certain state
            CheckWizardState();
        }

        /// <summary>
        /// Binds title for the page within wizard
        /// </summary>
        private void BindTitle()
        {
            // Bind page title
            //this.Page.Title = CommonHelper.MakePageTitle(CheckoutWizard.ActiveStep.Title);
        }

        /// <summary>
        /// Setups the wizard for processing.
        /// </summary>
        private void InitializeData()
        {
            // Reset shopping cart
            this.CartHelper.Reset();
            this.CartHelper.Cart.AcceptChanges();
        }

        /// <summary>
        /// Checks state of the wizard. If some variables do not exist anymore, for instance due to session timeout,
        /// make sure wizard is returns to the appropriate step
        /// </summary>
        private void CheckWizardState()
        {
            // Skip the shipment address step if addresses already selected
            if (this.CartHelper.IsAddressRequired)
                CheckoutWizard.ActiveStepIndex = CheckoutWizard.WizardSteps.IndexOf(this.ShippingAddressStep);

            if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.ShippingOptionsStep))
            {
                CheckoutShipping.PrepareStep();
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.PaymentStep))
            {
                CheckoutPayment.PrepareStep();
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.FinalStep))
            {
                CheckoutConfirm.PrepareStep();
            }
        }

        /// <summary>
        /// Called when [finish button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.WizardNavigationEventArgs"/> instance containing the event data.</param>
        protected void OnFinishButtonClick(Object sender, WizardNavigationEventArgs e)
        {
            // Prepare cart for checkout
            try
            {
                CartHelper.Cart.RunWorkflow("CartCheckout");
            }
            catch (PaymentException ex)
            {
                ErrorManager.GenerateError(ex.Message);
                return;
            }

            CartHelper.Cart.CustomerId = ProfileContext.Current.UserId;
            CartHelper.Cart.CustomerName = Profile.FullName;
            //CartHelper.Cart.CustomerId = ProfileContext.Current.Profile.n;

            PurchaseOrder po = null;

            // Save changes
            po = CartHelper.Cart.SaveAsPurchaseOrder();

            // Send emails
            SendEmails(po, CartHelper.PrimaryAddress.Email);

            // Save latest order id
            Session["LatestOrderId"] = po.OrderGroupId;

            // Remove old cart
            CartHelper.Cart.Delete();
            CartHelper.Cart.AcceptChanges();

            // Call post process method to allow payment gateway to do some magic
            if (CheckoutPayment != null && CheckoutPayment.PaymentOptionControl != null)
                (CheckoutPayment.PaymentOptionControl).PostProcess(CartHelper.OrderForm);

            // Redirect customer to receipt page
            if (CMSContext.Current.CurrentUrl.Contains("?"))
                Response.Redirect(CMSContext.Current.CurrentUrl + "&view=receipt");
            else
                Response.Redirect(CMSContext.Current.CurrentUrl + "?view=receipt");
        }

        private void SendEmails(PurchaseOrder order, string email)
        {
            // Add input parameter
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderGroup", order);

            // Send out emails
            // Create smtp client
            SmtpClient client = new SmtpClient();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(StoreEmail, StoreTitle);
            msg.IsBodyHtml = true;

            // Send confirmation email
            msg.Subject = String.Format("{1}: Order Confirmation for {0}", order.CustomerName, StoreTitle);
            msg.To.Add(new MailAddress(email, order.CustomerName));
            msg.Body = TemplateService.Process("order-purchaseorder-confirm", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);

            msg = new MailMessage();
            msg.From = new MailAddress(StoreEmail, StoreTitle);
            msg.IsBodyHtml = true;

            // Send notify email
            msg.Subject = String.Format("{1}: Order Notification {0}", order.TrackingNumber, StoreTitle);
            msg.To.Add(new MailAddress(StoreEmail, StoreTitle));
            msg.Body = TemplateService.Process("order-purchaseorder-notify", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);
        }

        private void SendEmails(PaymentPlan order, string email)
        {
            // Add input parameter
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("OrderGroup", order);

            // Send out emails
            // Create smtp client
            SmtpClient client = new SmtpClient();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(StoreEmail, StoreTitle);
            msg.IsBodyHtml = true;

            // Send confirmation email
            msg.Subject = String.Format("{1}: Payment Plan Confirmation for {0}", order.CustomerName, this.StoreTitle);
            msg.To.Add(new MailAddress(email, order.CustomerName));
            msg.Body = TemplateService.Process("order-paymentplan-confirm", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);

            msg = new MailMessage();
            msg.From = new MailAddress(StoreEmail, StoreTitle);
            msg.IsBodyHtml = true;

            // Send notify email
            msg.Subject = String.Format("{1}: Payment Plan Notification {0}", order.OrderGroupId, StoreTitle);
            msg.To.Add(new MailAddress(StoreEmail, StoreTitle));
            msg.Body = TemplateService.Process("order-paymentplan-notify", Thread.CurrentThread.CurrentCulture, dic);

            // send email
            client.Send(msg);
        }

        /// <summary>
        /// Called when [previous button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.WizardNavigationEventArgs"/> instance containing the event data.</param>
        protected void OnPreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            /*
            if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.ShippingOptionsStep))
            {
                if (ClientContext.Context.CurrentShippingAddress == null)
                {
                    if (!ClientCart.IsAddressRequired)
                    {
                        Response.Redirect(ClientHelper.FormatUrl("cart"));
                    }
                }
            }
             * */
        }

        /// <summary>
        /// Called when [next button click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.WizardNavigationEventArgs"/> instance containing the event data.</param>
        protected void OnNextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.ShippingAddressStep))
            {
                if (CheckoutShippingAddress.ValidateForm())
                {
                    OrderAddress address = CheckoutShippingAddress.ShippingAddress;

					// set current shipping address as customer's preferred shipping address
                    if (!Profile.IsAnonymous)
                    {
                         //Profile.Addresses.Count == 0
                        CustomerAddress custAddress = StoreHelper.ConvertToCustomerAddress(address);
                        custAddress.Name = _PreferredShippingAddressString;
                        Profile.PreferredShippingAddress = custAddress.Name;
						if (Profile.Account != null)
						{
							if (!StoreHelper.IsAddressInCollection(Profile.Account.Addresses, custAddress))
							{
								Profile.Account.Addresses.Add(custAddress);
								Profile.Account.Addresses.AcceptChanges();
							}
						}
                    }

                    // If name is not set, set to the default shipping address name
                    if (String.IsNullOrEmpty(address.Name))
                        address.Name = _ShippingAddressString;

					object shippingAddressId = Session[_SessionShippingAddressKey];

					OrderAddress orderAddress = shippingAddressId != null ? CartHelper.FindAddressByName(shippingAddressId.ToString()) : null;

					if (orderAddress == null)
                    {
                        // Save address for the order
                        CartHelper.Cart.OrderAddresses.Add(address);
                        CartHelper.Cart.AcceptChanges();

                        // Save address id in the viewstate
						Session[_SessionShippingAddressKey] = address.Name;

                        // Change address id for all line items
                        foreach (LineItem lineItem in CartHelper.LineItems)
                        {
                            if (String.IsNullOrEmpty(lineItem.ShippingAddressId))
                            {
                                lineItem.ShippingAddressId = address.Name.ToString();
                            }
                        }
                    }
                    else // simply modify the existing address
                    {
                        foreach(MetaField field in orderAddress.MetaClass.MetaFields)
                        {
							// copy all address fields except ids and name
                            if (!field.Name.Equals("OrderGroupAddressId", StringComparison.InvariantCultureIgnoreCase) && 
								!field.Name.Equals("OrderGroupId", StringComparison.InvariantCultureIgnoreCase) &&
								!field.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
                                orderAddress[field.Name] = address[field.Name];
                        }
                    }

                    CartHelper.Cart.AcceptChanges();

                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.ShippingOptionsStep))
            {
                CartHelper.Cart.AcceptChanges();
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.PaymentStep))
            {
                if (CheckoutPayment.ValidateData())
                {
                    OrderAddress address = CheckoutPayment.BillingAddress;

					// set current shipping address as customer's preferred billing address
					if (!Profile.IsAnonymous)
					{
						CustomerAddress custAddress = StoreHelper.ConvertToCustomerAddress(address);
						custAddress.Name = _PreferredBillingAddressString;
						Profile.PreferredBillingAddress = custAddress.Name;
						if (Profile.Account != null)
						{
							if (!StoreHelper.IsAddressInCollection(Profile.Account.Addresses, custAddress))
							{
								Profile.Account.Addresses.Add(custAddress);
								Profile.Account.Addresses.AcceptChanges();
							}
						}
					}

					object billingAddressId = Session[_SessionBillingAddressKey];

					OrderAddress orderAddress = billingAddressId != null ? CartHelper.FindAddressByName(billingAddressId.ToString()) : null;

                    if (orderAddress == null)
                    {
                        // If name is not set, set to the default shipping address name
                        if (String.IsNullOrEmpty(address.Name))
                            address.Name = "Billing Address";

                        // Save address for the order
                        CartHelper.Cart.OrderAddresses.Add(address);
                        CartHelper.Cart.AcceptChanges();

                        // Save the address
                        CartHelper.OrderForm.BillingAddressId = address.Name;

                        // Save address id in the viewstate
                        Session[_SessionBillingAddressKey] = address.Name;
                    }
                    else // simply modify the existing address
                    {
                        foreach (MetaField field in orderAddress.MetaClass.MetaFields)
                        {
							// copy all address fields except ids and name
							if (!field.Name.Equals("OrderGroupAddressId", StringComparison.InvariantCultureIgnoreCase) &&
								!field.Name.Equals("OrderGroupId", StringComparison.InvariantCultureIgnoreCase) &&
								!field.Name.Equals("Name", StringComparison.InvariantCultureIgnoreCase))
                                orderAddress[field.Name] = address[field.Name];
                        }

                        address = orderAddress;
                    }

                    // set primary address
                    CartHelper.OrderForm.BillingAddressId = address.Name.ToString();

                    // Save email address
                    address.Email = CheckoutPayment.EmailText;
                    //if (String.IsNullOrEmpty(Profile.Email))
                        Profile.Email = CheckoutPayment.EmailText;

                    //if(String.IsNullOrEmpty(Profile.FirstName))
                        Profile.FirstName = address.FirstName;

                    //if(String.IsNullOrEmpty(Profile.LastName))
                        Profile.LastName = address.LastName;

                    //if (String.IsNullOrEmpty(Profile.FullName))
                        Profile.FullName = String.Format("{0} {1}", Profile.FirstName, Profile.LastName);

					// Save profile
					Profile.Save();

                    // Set customer info
                    CartHelper.Cart.CustomerId = ProfileContext.Current.UserId;
                    CartHelper.Cart.CustomerName = String.Format("{0} {1}", address.FirstName, address.LastName);

                    // Remove all shipments before we run prepare workflow since it will recreate shipments there
                    foreach (OrderForm form in CartHelper.Cart.OrderForms)
                    {
                        foreach (Shipment shipment in form.Shipments)
                            shipment.Delete();

                        // Save items
                        CartHelper.Cart.AcceptChanges();
                    }

                    // Prepare cart for checkout
                    CartHelper.Cart.RunWorkflow("CartPrepare");

                    // Remove previous payments
                    foreach (Payment payment in CartHelper.OrderForm.Payments)
                        payment.Delete();

                    // Add Payments to the cart
                    Payment paymentInfo = CheckoutPayment.PaymentInfo;

                    // Check if any payment methods were selected
                    if (paymentInfo != null)
                    {
                        paymentInfo.PaymentMethodId = CheckoutPayment.PaymentMethod.PaymentMethodId;
                        paymentInfo.PaymentMethodName = CheckoutPayment.PaymentMethod.Name;
                        paymentInfo.Amount = CartHelper.OrderForm.Total;
                        CartHelper.OrderForm.Payments.Add(paymentInfo);
                    }

                    // Save changes
                    CartHelper.Cart.AcceptChanges();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Called when [active step changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void OnActiveStepChanged(object sender, EventArgs e)
        {
            if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.ShippingOptionsStep))
            {
                CheckoutWizard.StepNextButtonText = RM.GetString("CHECKOUT_SHIPPINGOPTIONS_NEXT");
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.PaymentStep))
            {
                CheckoutWizard.StepNextButtonText = RM.GetString("CHECKOUT_PAYMENT_PLACE_ORDER");
            }
            else if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.FinalStep))
            {
                CheckoutWizard.StepNextButtonText = RM.GetString("CHECKOUT_ORDER_CONFIRMATION_BUTTON");
            }

            // Set default button
            if (CheckoutWizard.ActiveStepIndex == CheckoutWizard.WizardSteps.IndexOf(this.FinalStep))
            {
                Page.Form.DefaultButton = CheckoutWizard.FindControl("FinishNavigationTemplateContainerID$FinishButton").UniqueID;
            }
            else
            {
                Page.Form.DefaultButton = CheckoutWizard.FindControl("StepNavigationTemplateContainerID$StepNextButton").UniqueID;
            }

            CheckWizardState();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.CheckoutShippingAddress.CartHelper = this.CartHelper;
            this.CheckoutShipping.CartHelper = this.CartHelper;
            this.CheckoutPayment.CartHelper = this.CartHelper;
            this.CheckoutConfirm.CartHelper = this.CartHelper;
            base.OnInit(e);
        }
    }
}