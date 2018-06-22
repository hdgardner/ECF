using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;
using NWTD.Orders;
using System.Net.Mail;
using Mediachase.Commerce.Engine.Template;
using System.Threading;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	public partial class SubmitCart : System.Web.UI.UserControl {

		#region Private Fields

		private CartHelper _checkoutCartHelper;
		private Mediachase.Commerce.Orders.Cart _checkoutCart;
		private OrderAddress _shippingAddress;
		private OrderAddress _billingAddress;
		string _StoreEmail = String.Empty;
		string _StoreTitle = String.Empty;

		#endregion

		#region Properties

		protected Organization UserOrganization {
			get {
				return Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Organization;
			}
		}

		protected bool UserIsLevelA {
			get { return NWTD.Profile.CurrentUserLevel.Equals(NWTD.UserLevel.A); }
		}

		/// <summary>
		/// A CartHelper for the Cart being checked out
		/// </summary>
		public CartHelper CheckoutCartHelper {
			get {
				if (this._checkoutCartHelper == null) {
					this._checkoutCartHelper = new CartHelper(this.CheckoutCart);
				}
				return this._checkoutCartHelper;
			}
		}

		/// <summary>
		/// The Cart being checked out
		/// </summary>
		public Mediachase.Commerce.Orders.Cart CheckoutCart {
			get {
				if (_checkoutCart == null) {
					string cartname = string.IsNullOrEmpty(Request["cart"]) ? Mediachase.Commerce.Orders.Cart.DefaultName : Request["cart"];
					_checkoutCart = Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(ProfileContext.Current.UserId, cartname);
				}
				return _checkoutCart;
			}

		}

		/// <summary>
		/// Gets the Shipping address for the cart from the Cart's addresses collection, 
		/// creates one if it doesn't exist (for level B, it just creates a new one, 
		/// for Level A, it creates one based on the pre-determined CustomerAddress)
		/// </summary>
		protected OrderAddress ShippingAddress {
			get {
				if (this._shippingAddress == null) {
					
					//First, check the cart for a ShippingAddress (we have to look to see if any of the line items have an address)
					this._shippingAddress = NWTD.Orders.Cart.FindCartShippingAddress(this.CheckoutCart);

					//this._shippingAddress = CheckoutCartHelper.FindAddressByName(NWTD.Orders.Cart.SHIPPING_ADDRESS_NAME);
					//if there is none, create one
					if (this._shippingAddress == null) {

						if (UserIsLevelA) {
							//TODO: This needs figuring out
							//this._shippingAddress remains null for now, it will be created when we save the address
						} else {
							this._shippingAddress = new OrderAddress();
							this._shippingAddress.Name = NWTD.Orders.Cart.SHIPPING_ADDRESS_NAME;
							this.CheckoutCart.OrderAddresses.Add(this._shippingAddress);
							this._shippingAddress["TaxRate"] = 0;
							this._shippingAddress["IsFreightTaxable"] = false;
							this._shippingAddress["SBOAddressId"] = string.Empty;
							this.CheckoutCart.AcceptChanges();
						}
					}
				}
				return this._shippingAddress; 
			}
			set { 
				OrderAddress existingAddress = this.CheckoutCartHelper.FindAddressByName(value.Name);
				if (existingAddress != null) {
					existingAddress.Delete();
					this.CheckoutCart.OrderAddresses.AcceptChanges();
					//this.CheckoutCart.OrderAddresses[this.CheckoutCart.OrderAddresses.IndexOf(existingAddress)] = value;
				} 
				this.CheckoutCart.OrderAddresses.Add(value);
				value.AcceptChanges(); //this is for dealing with custom meta fields stored in the order address
				this.CheckoutCart.AcceptChanges();
				this._shippingAddress = value;
			
			} 
		}

		/// <summary>
		/// Gets the Billing address for the cart from the Cart's addresses collection, 
		/// creates one if it doesn't exist (for level B, it just creates a new one, 
		/// for Level A, it creates one based on the pre-determined CustomerAddress)
		/// </summary>
		protected OrderAddress BillingAddress {
			get {
				if (this._billingAddress == null) {
					//first, we'll see if the cart already has a billing address
					
					//get the order form. DO NOT use the OrderForm property on the CartHelper. 
					//It looks for an order form named "Default" and cretes a new one if it doesn't find one. That's NOT what we want.
					OrderForm orderForm = (this.CheckoutCartHelper.GetOrderForm(this.CheckoutCart.Name));
					if (!string.IsNullOrEmpty(orderForm.BillingAddressId)) {
						this._billingAddress = CheckoutCartHelper.FindAddressByName(orderForm.BillingAddressId);
					}
					//this._billingAddress = CheckoutCartHelper.FindAddressByName(NWTD.Orders.Cart.BILLING_ADDRESS_NAME);
					
					//if the cart doesn't have a billing address, we need to add one
					if (this._billingAddress == null) {
						//if the user is a level A User, 
						//we need to get the pre-determined billing address from the user's organization
						//and convert it to an order address
						if (this.UserIsLevelA) {
							//grab the first billing address from the Level A User's Organazation's Addresses
							foreach (CustomerAddress address in UserOrganization.Addresses) {
								if (address["Type"].ToString() == "B") {
									this._billingAddress = StoreHelper.ConvertToOrderAddress(address);
									break;
								}
							}
							if (this._billingAddress == null) {
								throw new Exception("The Organization for this customer does not have a Billing Address");
							}
							
						}
						
						//otherwise, we'll create a new one
						else { this._billingAddress = new OrderAddress() { Name = NWTD.Orders.Cart.BILLING_ADDRESS_NAME }; }

						this.CheckoutCart.OrderAddresses.Add(this._billingAddress);
					}



					orderForm.BillingAddressId = this._billingAddress.Name;

					this._billingAddress["TaxRate"] = 0;
					this._billingAddress["IsFreightTaxable"] = false;
					this._billingAddress["SBOAddressId"] = string.Empty;
					this._billingAddress.AcceptChanges();
		
					this.CheckoutCart.AcceptChanges();
				}
				return this._billingAddress;
			}
			set { this._billingAddress = value; }
		}


		/// <summary>
		/// Gets the store email.
		/// </summary>
		/// <value>The store email.</value>
		public string StoreEmail {
			get {
				if (String.IsNullOrEmpty(_StoreEmail))
					_StoreEmail = GlobalVariable.GetVariable("email", CMSContext.Current.SiteId);

				return _StoreEmail;
			}
		}

		/// <summary>
		/// Gets the store title.
		/// </summary>
		/// <value>The store title.</value>
		public string StoreTitle {
			get {
				if (String.IsNullOrEmpty(_StoreTitle))
					_StoreTitle = GlobalVariable.GetVariable("title", CMSContext.Current.SiteId);

				return _StoreTitle;
			}
		}

		#endregion

		#region Methods

		protected void SaveBillingAddress() {
			//Save the level B Address information from the form (level A is predetermined)
			if (!this.UserIsLevelA) { 
				//BillingAddress.City = tbShippingCity.Text;
            //Replaced wrong City mapping above with correct mapping below (Heath Gardner 06/25/13)
            // Issue ID 1075 - "ECF - Standard User’s Bill-To City is overwritten with their Ship-To City"
                BillingAddress.City = tbBillingCity.Text;
				BillingAddress.State = ssBillingState.SelectedState;
				BillingAddress.Line1 = tbBillingAddress.Text;
				BillingAddress.PostalCode = tbBillingZip.Text;
				BillingAddress.DaytimePhoneNumber = pnBillingDayPhone.Text;
				//BillingAddress.EveningPhoneNumber = pnBillingEveningPhone.Text;
				BillingAddress.FaxNumber = pnBillingFax.Text;
				BillingAddress.AcceptChanges();
			}
			//set the cart billing address
			this.CheckoutCartHelper.GetOrderForm(this.CheckoutCart.Name).BillingAddressId = BillingAddress.Name;

			//save the billing meta data
			this.CheckoutCart["OrderContactName"] = this.tbBillingContactName.Text;
			this.CheckoutCart["OrderContactPhone"] = this.pnBillingContact.Text;
			this.CheckoutCart["PurchaseOrder"] = this.tbBillingPurchaseOrder.Text;
			this.CheckoutCart["SpecialInstructions"] = this.tbBillingSpecialInstructions.Text;
			this.CheckoutCart.AcceptChanges();

		}

		protected void SaveShippingAddress() {
			if (this.UserIsLevelA) {
				//TODO: Save level A address
				//this.ShippingAddress =  CheckoutCartHelper.FindAddressById(this.ddlLevelAShippingAddress.SelectedValue);

				foreach (CustomerAddress address in this.UserOrganization.Addresses) {
					if (address.Name == ddlLevelAShippingAddress.SelectedValue) {
						
						//convert the selected address into an OrderAddress, including custom meta data.
						OrderAddress shippingAddress = StoreHelper.ConvertToOrderAddress(address);
						shippingAddress["TaxRate"] = address["TaxRate"];
						shippingAddress["IsFreightTaxable"] = address["IsFreightTaxable"];
						shippingAddress["SBOAddressId"] = address["SBOAddressId"];
						
						this.ShippingAddress = shippingAddress;
					}
				}

			} else {
				//Save the level B Address
				ShippingAddress.City = tbShippingCity.Text;
				ShippingAddress.State = ssShippingState.SelectedState;
				ShippingAddress.Line1 = tbShippingAddress.Text;
				ShippingAddress.PostalCode = tbShippingZip.Text;
				ShippingAddress.DaytimePhoneNumber = pnShippingDayPhone.Text;
				ShippingAddress.FaxNumber = pnShippingFax.Text;
				ShippingAddress.AcceptChanges();
			}

			//save the meta data
			this.CheckoutCart["ShippingInstructions"] = this.tbShippingSpecialInstructions.Text;

			//set the line item addresses
			foreach (LineItem lineItem in this.CheckoutCartHelper.LineItems) {
				lineItem.ShippingAddressId = ShippingAddress.Name;
			}
			//TODO:Save the shipping Meta Data
			this.CheckoutCart.AcceptChanges();
		}

		protected void SaveChanges() {
			//since addresses might have changed, we need to make sure the tax/shipping prices got updated
			Mediachase.Commerce.Orders.Cart cart = this.CheckoutCart;
			NWTD.Orders.Cart.AssignTotals(ref cart);
			this.CheckoutCart.AcceptChanges();
		
		}

		protected void UpdateStepList() {
			//in the list of steps, make the current step have boldface font
			foreach (ListItem item in this.blStepsList.Items) {
				if (item.Text.Equals(this.wzSubmitCart.ActiveStep.Title)) {
					item.Attributes.CssStyle.Add("font-weight", "bold");
				} else {
					item.Attributes.CssStyle.Remove("font-weight");
				}
			}
		}


		private void SendConfirmationEmail() {
			// Add input parameter
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("OrderGroup",this.CheckoutCart);

			// Send out emails
			// Create smtp client
			SmtpClient client = new SmtpClient();

			MailMessage msg = new MailMessage();
			msg.From = new MailAddress(StoreEmail, StoreTitle);
			msg.IsBodyHtml = true;

			// Send confirmation email
			msg.Subject = String.Format("{1}: Order Confirmation for {0}", this.CheckoutCart.CustomerName, StoreTitle);
			msg.To.Add(new MailAddress(this.CheckoutCartHelper.PrimaryAddress.Email, this.CheckoutCart.CustomerName));
			msg.Body = TemplateService.Process("order-purchaseorder-confirm", Thread.CurrentThread.CurrentCulture, dic);

			// send email
			client.Send(msg);
		}

		/// <summary>
		/// Validates fields that pertain to Level B users
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		protected void ValidateLevelBField(object source, ServerValidateEventArgs args) {
			if (string.IsNullOrEmpty(args.Value) && !this.UserIsLevelA) {
				args.IsValid = false;
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// When the page loads, we have to bind all the existing information about the cart to the edit fields in the wizard
		/// We also need to do some branching for Level A and B users, showing and hiding the appropriate controls.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			this.blStepsList.DataSource = this.wzSubmitCart.WizardSteps;
			this.blStepsList.DataBind();

			this.blStepsList.Items[0].Attributes.CssStyle.Add("font-weight", "bold");

			//populate the shiping and billing address fields
			if (this.UserIsLevelA) {
				//Level A Address Binding
		
				this.lblLevelABillingAddress.Text = this.BillingAddress.Name; //only get Shipping addresses to choose from
				this.ddlLevelAShippingAddress.DataSource = UserOrganization.Addresses.Cast<CustomerAddress>().AsQueryable().Where(a=>a.GetString("Type") == "S");
				this.ddlLevelAShippingAddress.DataBind();

				this.pnlShippingAndTax.Visible = true;

				if (this.ShippingAddress != null) {
					foreach (ListItem item in this.ddlLevelAShippingAddress.Items) {
						if (item.Value == this.ShippingAddress.Name) item.Selected = true;
					}
				}

			} else {
				//Level B Address Binding
				this.ssShippingState.DefaultState = this.ShippingAddress.State;
				this.tbShippingAddress.Text = this.ShippingAddress.Line1;
				this.tbShippingCity.Text = this.ShippingAddress.City;
				this.tbShippingZip.Text = this.ShippingAddress.PostalCode;
				this.pnShippingDayPhone.Text = this.ShippingAddress.DaytimePhoneNumber;
				this.pnShippingFax.Text = this.ShippingAddress.FaxNumber;

				this.ssBillingState.DefaultState = this.BillingAddress.State;
				this.tbBillingAddress.Text = this.BillingAddress.Line1;
				this.tbBillingCity.Text = this.BillingAddress.City;
				this.tbBillingZip.Text = this.BillingAddress.PostalCode;
				this.pnBillingDayPhone.Text = this.BillingAddress.DaytimePhoneNumber;
				this.pnBillingFax.Text = this.BillingAddress.FaxNumber;

			}

			//Level A and B meta field binding
			this.tbShippingSpecialInstructions.Text = this.CheckoutCart["ShippingInstructions"] != null ? this.CheckoutCart["ShippingInstructions"] .ToString() : string.Empty;
			this.tbBillingContactName.Text = this.CheckoutCart["OrderContactName"] != null ? this.CheckoutCart["OrderContactName"].ToString() : string.Empty;
			this.tbBillingPurchaseOrder.Text = this.CheckoutCart["PurchaseOrder"] != null ? this.CheckoutCart["PurchaseOrder"].ToString() : string.Empty;
			this.pnBillingContact.Text = this.CheckoutCart["OrderContactPhone"] != null ? this.CheckoutCart["OrderContactPhone"].ToString() : string.Empty;
			this.tbBillingSpecialInstructions.Text = this.CheckoutCart["SpecialInstructions"] != null ? this.CheckoutCart["SpecialInstructions"].ToString() : string.Empty;

			//set the grid to have the line items as its data source
			this.gvCart.DataSource = this.CheckoutCartHelper.LineItems;

			if (!this.Page.IsPostBack) {

				//hide the finish button unless the user is a Level A User
				Button finishButton = wzSubmitCart.FindControl("FinishNavigationTemplateContainerID").FindControl("FinishButton") as Button;
				finishButton.Visible = this.UserIsLevelA;

				//if this is a level A users, we're going to change what address info is displayed in both the input section and the confirmation section
				if (this.UserIsLevelA) {
					this.fsLevelAOrderAddress.Visible = true;
					this.fsLevelAShippingAddress.Visible = true;
					this.fsLevelBOrderAddress.Visible = false;
					this.fsLevelBShippingAddress.Visible = false;
					this.lbGoToBillingAddress.Visible = false;
					//this.addrLevelBBillTo.Visible = false;
					//this.addrLevelBShipTo.Visible = false;
				}
			}
		}

		/// <summary>
		/// When a Level A User clicks finish several things occur:
		/// <list type="bullet">
		///	<item>The status of the cart is changed to submitted.</item>
		///	<item>The name of the cart is changed so that new carts can be created with the cart's name.</item>
		///	<item>An email is sent out informing the user that the cart was submitted.</item>
		/// </list>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void wzSubmitCart_FinishButtonClick(object sender, WizardNavigationEventArgs e) {
			//This only occurs for Level A USERS
			//set the status on the cart
			this.CheckoutCart.Status = NWTD.Orders.Cart.CART_STATUS.SUBMITTED.ToString();
			//change the name of the cart so future carts with the same name can be created
			this.CheckoutCart.Name = this.CheckoutCart.Name + "_" + this.CheckoutCart.Id;
			this.SaveChanges();
			//Response.Redirect("~/");
		}

		/// <summary>
		/// When a user changes steps, we have to do a few things manually, such as make the current step bold in the list of steps, and bind the summary grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void wzSubmitCart_ActiveStepChanged(object sender, EventArgs e) {
			this.UpdateStepList();
			//if we're on the last step, re-bind the cart
			if (this.wzSubmitCart.ActiveStep.Equals(this.wsOrderSummary)) {
				this.gvCart.DataBind();
			}
		}

		protected void lbGoToAddress_Command(object sender, CommandEventArgs e) {
			if (e.CommandArgument.ToString() == "ShipTo") wzSubmitCart.ActiveStepIndex = wzSubmitCart.WizardSteps.IndexOf(wsShippingAddress);
			if (e.CommandArgument.ToString() == "BillTo") wzSubmitCart.ActiveStepIndex = wzSubmitCart.WizardSteps.IndexOf(wsBillingAddress);
		}

		protected void gvCart_RowDataBound(object sender, GridViewRowEventArgs e) {
			if (e.Row.RowType == DataControlRowType.Footer) {
				e.Row.Cells[0].Attributes["colspan"] = (e.Row.Cells.Count - 1).ToString();
				for (int i = 1; i < e.Row.Cells.Count - 1; i++) {
					e.Row.Cells[i].Visible = false;
				}

				e.Row.Cells[e.Row.Cells.Count - 1].Text = CurrencyFormatter.FormatCurrency(NWTD.Orders.Cart.CartTotal(this.CheckoutCartHelper.Cart), this.CheckoutCartHelper.Cart.BillingCurrency);
				
			
			}

			if (e.Row.RowType == DataControlRowType.DataRow) {
				LineItem item = e.Row.DataItem as LineItem;
				Literal tbQuantityCharged = e.Row.FindControl("litQuantityCharged") as Literal;
				tbQuantityCharged.Text = item["Gratis"] == null ? item.Quantity.ToString("n0") : (item.Quantity - (decimal)item["Gratis"]).ToString("n0");
			}
		}

		protected void wzSubmitCart_NextButtonClick(object sender, WizardNavigationEventArgs e) {
			if (!this.Page.IsValid) {
				e.Cancel = true;
				return;
			}

			switch (this.wzSubmitCart.WizardSteps[e.CurrentStepIndex].ID) {
				case "wsBillingAddress":
					this.SaveBillingAddress();
					break;
				case "wsShippingAddress":
					this.SaveShippingAddress();
					break;
			}
		}

		protected void lbCopyShippingAddress_Command(object sender, CommandEventArgs e) {
			this.tbBillingAddress.Text = this.tbShippingAddress.Text;
			this.tbBillingCity.Text = this.tbShippingCity.Text;
			this.tbBillingZip.Text = this.tbShippingZip.Text;
			this.ssBillingState.DefaultState = this.ssShippingState.SelectedState;
			this.pnBillingDayPhone.PhoneNumber = this.pnShippingDayPhone.PhoneNumber;
			this.pnBillingFax.PhoneNumber = this.pnShippingFax.PhoneNumber;
			this.UpdateStepList();
		}

		#endregion


	}
}