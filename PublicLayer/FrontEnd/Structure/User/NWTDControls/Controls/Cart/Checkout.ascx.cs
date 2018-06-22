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
	
	/// <summary>
	/// This is the control where shipping and billing information is added to the cart. 
	/// It is one of the more complicated controls in the system because of the amount of branching that takes place between Level A users and Level B users and different depositories. 
	/// This control is located on the page ~/Cart/Submit.aspx and is typically accessed by clicking the Add Shipping Info button on the view carts page.
	/// </summary>
	public partial class Checkout : System.Web.UI.UserControl {

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

            get
            {
                if (this._shippingAddress == null)
                {

                    //First, check the cart for a ShippingAddress (we have to look to see if any of the line items have an address)
                    this._shippingAddress = NWTD.Orders.Cart.FindCartShippingAddress(this.CheckoutCart);
                }
                return this._shippingAddress;
            }

		    set {
                //TODO: This setter has too many side effects. 
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

                    //JF 10092012 Bug #250 - No longer retrieves previously saved billing address associated w/ cart 
                    ////get the order form. DO NOT use the OrderForm property on the CartHelper. 
                    ////It looks for an order form named "Default" and cretes a new one if it doesn't find one. That's NOT what we want.
                    OrderForm orderForm = (this.CheckoutCartHelper.GetOrderForm(this.CheckoutCart.Name));
                    //if (!string.IsNullOrEmpty(orderForm.BillingAddressId)) {
                    //    this._billingAddress = CheckoutCartHelper.FindAddressByName(orderForm.BillingAddressId);
                    //}
					//this._billingAddress = CheckoutCartHelper.FindAddressByName(NWTD.Orders.Cart.BILLING_ADDRESS_NAME);

					//if the cart doesn't have a billing address, we need to add one
				    if (this._billingAddress == null)
				    {
				        //if the user is a level A User, 
				        //we need to get the pre-determined billing address from the user's organization
				        //and convert it to an order address
				        if (this.UserIsLevelA)
				        {
				            //grab the first billing address from the Level A User's Organazation's Addresses
				            foreach (CustomerAddress address in UserOrganization.Addresses)
				            {
				                if (address["Type"].ToString() == "B")
				                {
				                    this._billingAddress = StoreHelper.ConvertToOrderAddress(address);
				                    break;
				                }
				            }
				            if (this._billingAddress == null)
				            {
				                throw new Exception("The Organization for this customer does not have a Billing Address");
				            }

				        }
				        else if (!this.UserIsLevelA)
				        {
				            if (orderForm != null && !string.IsNullOrEmpty(orderForm.BillingAddressId))
				            {
				                this._billingAddress = CheckoutCartHelper.FindAddressByName(orderForm.BillingAddressId);
				            }
				            else
				            {
				                this._billingAddress = new OrderAddress() {Name = NWTD.Orders.Cart.BILLING_ADDRESS_NAME};
				            }
				        }


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


		#endregion

		#region Methods

        /// <summary>
        /// Saves the cart-level metadata from the form fieds
        /// </summary>
        protected void SaveMetaData() {
            //save the billing meta data
            this.CheckoutCart["OrderContactName"] = this.tbBillingContactName.Text;
            this.CheckoutCart["OrderContactPhone"] = this.pnBillingContact.Text;
            this.CheckoutCart["PurchaseOrder"] = this.tbBillingPurchaseOrder.Text;
            this.CheckoutCart["SpecialInstructions"] = this.tbBillingSpecialInstructions.Text;
            //On 08/02/17 Heath changed the following Fax# field to always save as an empty string since it 
            // is no longer on the form. I chose to do empty string in case fax# is still a required save value
            //this.CheckoutCart["OrderFax"] = this.pnFax.Text;
            this.CheckoutCart["OrderFax"] = string.Empty;
            this.CheckoutCart.AcceptChanges();
        }

        /// <summary>
        /// Saves the billing address based on the form information. For level B users, fields are filled out. 
        /// For level A users, billing address is pre-determined
        /// </summary>
		protected void SaveBillingAddress() {
			//Save the level B Address information from the form (level A is predetermined)
			if (!this.UserIsLevelA) {

				BillingAddress.FirstName = tbBillingAddressName.Text;
				//BillingAddress.City = tbShippingCity.Text;
            //Replaced wrong City mapping above with correct mapping below (Heath Gardner 06/25/13)
            // Issue ID 1075 - "ECF - Standard User’s Bill-To City is overwritten with their Ship-To City"
                BillingAddress.City = tbBillingCity.Text;
				BillingAddress.State = ddlBillingState.SelectedValue;
				BillingAddress.Line1 = tbBillingAddress.Text;
				BillingAddress.PostalCode = tbBillingZip.Text;
                //we'll use the "Contact Phone" since the phone nubmer field was eliminated from billing address
                BillingAddress.DaytimePhoneNumber = pnBillingContact.Text;
                //BillingAddress.DaytimePhoneNumber = pnBillingDayPhone.Text;
				//BillingAddress.EveningPhoneNumber = pnBillingEveningPhone.Text;
				//BillingAddress.FaxNumber = pnBillingFax.Text;
               
                BillingAddress.AcceptChanges();
			}
			//set the cart billing address
			this.CheckoutCartHelper.GetOrderForm(this.CheckoutCart.Name).BillingAddressId = BillingAddress.Name;


			this.CheckoutCart.AcceptChanges();

		}

        /// <summary>
        /// Saves the shipping address based on the form information. For level B users, fields are filled out. 
        /// For level A users, billing address is selected from a dropdown of exising addresses
        /// </summary>
		protected bool SaveShippingAddress() {
			if (this.UserIsLevelA) {

                if (ddlLevelAShippingAddress.SelectedIndex == 0) return false;

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

			}
			else {
				//Save the level B Address

                //this._shippingAddress = CheckoutCartHelper.FindAddressByName(NWTD.Orders.Cart.SHIPPING_ADDRESS_NAME);
                //if there is none, create one
                if (this.ShippingAddress == null && !UserIsLevelA)
                {
                    OrderAddress address = new OrderAddress(){Name = NWTD.Orders.Cart.SHIPPING_ADDRESS_NAME};

                    address["TaxRate"] = 0;
                    address["IsFreightTaxable"] = false;
                    address["SBOAddressId"] = string.Empty;

                    this.ShippingAddress = address;
                }

				ShippingAddress.FirstName = tbShippingAddressName.Text;
				ShippingAddress.City = tbShippingCity.Text;
				ShippingAddress.State = ddlShippingState.SelectedValue;
				ShippingAddress.Line1 = tbShippingAddress.Text;
				ShippingAddress.PostalCode = tbShippingZip.Text;
				//ShippingAddress.DaytimePhoneNumber = pnShippingDayPhone.Text;
				//ShippingAddress.FaxNumber = pnShippingFax.Text;
				ShippingAddress.AcceptChanges();
                bool hasAddress = false;
                foreach(OrderAddress address in this.CheckoutCart.OrderAddresses){
                    if(address.Name == ShippingAddress.Name) hasAddress = true;
                }
                if(!hasAddress) this.CheckoutCart.OrderAddresses.Add(ShippingAddress);
			}


			//set the line item addresses
			foreach (LineItem lineItem in this.CheckoutCartHelper.LineItems) {
				lineItem.ShippingAddressId = ShippingAddress.Name;
			}
			//TODO:Save the shipping Meta Data
			this.CheckoutCart.AcceptChanges();
            return true;
		}


		//TODO: move this to the cart summary page
		protected void SaveChanges() {
			//since addresses might have changed, we need to make sure the tax/shipping prices got updated
			Mediachase.Commerce.Orders.Cart cart = this.CheckoutCart;
			NWTD.Orders.Cart.AssignTotals(ref cart);
			this.CheckoutCart.AcceptChanges();

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


		protected void BindSelectedLevelAAddress() {
			IQueryable<CustomerAddress> addresses = UserOrganization.Addresses.Cast<CustomerAddress>().AsQueryable().Where(a => a.GetString("Name") == ddlLevelAShippingAddress.SelectedValue);
			if (addresses.Any()) {
				CustomerAddress address = addresses.First();
				this.litSelectedShippingLine1.Text = address.Line1;
				this.litSelectedShippingLine2.Text = address.Line2;
				this.litSelectedShippingState.Text = address.State;
				this.litSelectedShippingCity.Text = address.City;
				this.litSelectedShippingZip.Text = address.PostalCode;
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


			//populate the shiping and billing address fields
			if (this.UserIsLevelA) {
				//Level A Address Binding
				this.ddlLevelAShippingAddress.DataSource = UserOrganization.Addresses.Cast<CustomerAddress>().AsQueryable().Where(a => a.GetString("Type") == "S").OrderBy(a => a.FirstName);

				this.ddlLevelAShippingAddress.DataBind();
                ddlLevelAShippingAddress.Items.Insert(0, new ListItem("-- Select a Shipping Address --", "-1"));


				if (this.ShippingAddress != null) {
					foreach (ListItem item in this.ddlLevelAShippingAddress.Items) {
						if (item.Value == this.ShippingAddress.Name) item.Selected = true;
					}
				}
			}
			else {
                //Level B Address Binding

				if(NWTD.Profile.CustomerDepository.Equals(NWTD.Depository.MSSD)){
					this.ddlBillingState.Items.Clear();
					this.ddlBillingState.Items.Add(new ListItem("Utah","UT"));
					this.ddlBillingState.Items.Add(new ListItem("Nevada", "NV"));
					this.ddlShippingState.Items.Clear();
					this.ddlShippingState.Items.Add(new ListItem("Utah","UT"));
					this.ddlShippingState.Items.Add(new ListItem("Nevada", "NV"));
				}

                if (this.ShippingAddress != null){

                    if (this.ddlShippingState.Items.FindByValue(this.ShippingAddress.State) != null)
                        this.ddlShippingState.SelectedValue = this.ShippingAddress.State;

					this.tbShippingAddressName.Text = this.ShippingAddress.FirstName;
                    this.tbShippingAddress.Text = this.ShippingAddress.Line1;
                    this.tbShippingCity.Text = this.ShippingAddress.City;
                    this.tbShippingZip.Text = this.ShippingAddress.PostalCode;
                }
                

				if (this.ddlBillingState.Items.FindByValue(this.BillingAddress.State) != null)
					this.ddlBillingState.SelectedValue = this.BillingAddress.State;
				this.tbBillingAddressName.Text = this.BillingAddress.FirstName;
				this.tbBillingAddress.Text = this.BillingAddress.Line1;
				this.tbBillingCity.Text = this.BillingAddress.City;
				this.tbBillingZip.Text = this.BillingAddress.PostalCode;

			}

			//Level A and B meta field binding
			
			this.tbBillingContactName.Text = this.CheckoutCart["OrderContactName"] != null ? this.CheckoutCart["OrderContactName"].ToString() : string.Empty;
			this.tbBillingPurchaseOrder.Text = this.CheckoutCart["PurchaseOrder"] != null ? this.CheckoutCart["PurchaseOrder"].ToString() : string.Empty;
			this.rqvBillingPurchaseOrder.Enabled = this.UserIsLevelA; //only validate the PO field for level A users
			
			this.pnBillingContact.Text = this.CheckoutCart["OrderContactPhone"] != null ? this.CheckoutCart["OrderContactPhone"].ToString() : string.Empty;
            //On 08/02/17 Heath commented out the following Fax# field as it is no longer required
            //this.pnFax.Text = this.CheckoutCart["OrderFax"] != null ? this.CheckoutCart["OrderFax"].ToString() : string.Empty;

            this.tbBillingSpecialInstructions.Text = this.CheckoutCart["SpecialInstructions"] != null ? this.CheckoutCart["SpecialInstructions"].ToString() : string.Empty;



			if (!this.Page.IsPostBack) {


				//if this is a level A users, we're going to change what address info is displayed in both the input section and the confirmation section
				if (this.UserIsLevelA) {
					this.fsLevelAOrderAddress.Visible = true;
					this.fsLevelAShippingAddress.Visible = true;
					this.fsLevelBOrderAddress.Visible = false;
					this.fsLevelBShippingAddress.Visible = false;
                    
					//this.addrLevelBBillTo.Visible = false;
					//this.addrLevelBShipTo.Visible = false;
					this.BindSelectedLevelAAddress();

                    //On 08/02/17, Heath added the following to make the printer friendly message visible for level B users only
                    this.lblContinueForPrintFriendlyMsg.Visible = false;
				}

			    rqvBillingPurchaseOrder.Visible = UserIsLevelA;
			    lblPORequired.Visible = UserIsLevelA;
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
		protected void btnCompleteAddresses_Click(object sender, EventArgs e) {
			if (!this.Page.IsValid) return;
			this.SaveBillingAddress();
            if (!this.SaveShippingAddress()) return;
            this.SaveMetaData();

			Response.Redirect(NavigationManager.GetUrl("OrderSummary", new object[] { "cart", this.CheckoutCart.Name }));

		}

		/// <summary>
		/// When the selected shipping address changes, rebind
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlLevelAShippingAddress_SelectedIndexChanged(object sender, EventArgs e) {
			this.BindSelectedLevelAAddress();
		}

		/// <summary>
		/// Responst to the "Return to Cart" button being clicked, saving addresses and metadata, and sending the user to the view cart page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnReturnToCart_Click(object sender, EventArgs e) {
			this.SaveBillingAddress();
            this.SaveShippingAddress();
	

			this.SaveMetaData();

			Response.Redirect(NavigationManager.GetUrl("ViewCart", new object[] { "cart", this.CheckoutCart.Name }));

		}

		/// <summary>
		/// Validates the various phone number fields in the form when the from is submitted.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		protected void PhoneNumber_ServerValidate(object source, ServerValidateEventArgs args) {
		
			
			CustomValidator validatorControl = (CustomValidator)source;

			OakTree.Web.UI.WebControls.PhoneNumberField control = this.FindControl(validatorControl.ControlToValidate) as OakTree.Web.UI.WebControls.PhoneNumberField;
			if (control == null || control.PhoneNumber.IsEmpty) return;
			if (!control.PhoneNumber.IsValid) args.IsValid = false;
			int result;
			args.IsValid = (int.TryParse(control.PhoneNumber.Prefix, out result) && int.TryParse(control.PhoneNumber.Suffix, out result) && int.TryParse(control.PhoneNumber.AreaCode, out result));
		}


		#endregion
	}
}