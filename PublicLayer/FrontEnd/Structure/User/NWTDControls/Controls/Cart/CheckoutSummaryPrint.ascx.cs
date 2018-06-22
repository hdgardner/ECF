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
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// The a control used for displaying order summary. This control varies contextually. 
	/// For example, level A users will see a checkout button, while Level B users will only see a print button.
	/// 
	/// </summary>
	public partial class CheckoutSummaryPrint : System.Web.UI.UserControl {

		#region Private Fields

        private PurchaseOrder _puchaseOrder = null;
		private CartHelper _checkoutCartHelper;
		private Mediachase.Commerce.Orders.Cart _checkoutCart;
		private OrderAddress _shippingAddress;
		private OrderAddress _billingAddress;
		string _StoreEmail = String.Empty;
		string _StoreTitle = String.Empty;

		#endregion

		#region Properties

        protected Guid UserId { get
        {
            if (string.IsNullOrEmpty(Request.QueryString["uid"]))
                return Guid.Empty;

            return new Guid(Request.QueryString["uid"]);
        } }
		/// <summary>
		/// The Purchase order associated witht he cart.
		/// </summary>
        protected PurchaseOrder PurchaseOrder {
            get {
                
                if(this._puchaseOrder == null)
                    this._puchaseOrder = 
                        PurchaseOrder.LoadByCustomer(UserId)
                        .OfType<PurchaseOrder>()
                        .SingleOrDefault(po=>po.Name == CheckoutCart.Name);
                return this._puchaseOrder; }
        }

		/// <summary>
		/// The organization (for NWTD purposes, District) for the current user
		/// </summary>
		protected Organization UserOrganization {
			get {
				return Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Organization;
			}
		}

		/// <summary>
		/// Whether the user is a Level A user
		/// </summary>
		protected bool UserIsLevelA {
			get
			{
			    if (string.IsNullOrEmpty(Request.QueryString["IsLevelA"]))
			        return false;

                return bool.Parse(Request.QueryString["IsLevelA"]);
			}
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
					_checkoutCart = Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(UserId, cartname);
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
				}
				return this._shippingAddress;
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

				}
				return this._billingAddress;
			}

		}


		/// <summary>
		/// Gets the commerce store email.
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
		/// Gets the commerce store title.
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


		//TODO: do we need to do this??? OR should it be in the other screen???
		protected void SaveChanges() {
			//since addresses might have changed, we need to make sure the tax/shipping prices got updated
			Mediachase.Commerce.Orders.Cart cart = this.CheckoutCart;
			NWTD.Orders.Cart.AssignTotals(ref cart);
			this.CheckoutCart.AcceptChanges();
		}

		private void SendConfirmationEmail() {
			// Add input parameter
			Dictionary<string, object> dic = new Dictionary<string, object>();
			dic.Add("PurchaseOrder", this.CheckoutCart);
            dic.Add("MetaData", new CartMetaDataInfo(){WebConfirmation = this.CheckoutCart.GetString("WebConfirmation")});
			// Send out emails
			// Create smtp client
			SmtpClient client = new SmtpClient();

			MailMessage msg = new MailMessage();
			msg.From = new MailAddress(StoreEmail, StoreTitle);
			msg.IsBodyHtml = true;

			// Send confirmation email
			msg.Subject = String.Format("{1}: Order Confirmation for {0}", this.CheckoutCart.CustomerName, StoreTitle);
			//we'll use the current user's addres rather than that associated with the order (could be a district or something)
            //msg.To.Add(new MailAddress(this.CheckoutCartHelper.PrimaryAddress.Email, this.CheckoutCart.CustomerName));
            msg.To.Add(new MailAddress(Mediachase.Commerce.Profile.ProfileContext.Current.User.Email, this.CheckoutCart.CustomerName));

			string templateName = NWTD.Profile.CustomerDepository == NWTD.Depository.MSSD ? "mssd-order-confirm" : "nwtd-order-confirm";
			msg.Body = TemplateService.Process(templateName, Thread.CurrentThread.CurrentCulture, dic);

			// send email
			client.Send(msg);
		}
        
		
		public class CartMetaDataInfo {
            public string WebConfirmation;
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

			this.OrderMessage.Text = string.Empty;

            NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryModal_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.modal.js"));
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CheckoutSummary_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/CheckoutSummary.js"));

			if (this.CheckoutCart == null) {
				this.OrderMessage.Text = "You are trying to view the summary for a cart that does not exist;";
				this.pnlOrderSubmitted.Visible = false;
				this.pnlOrderSummary.Visible = false;
				return;
			}


		    CMSContext.Current.OverrideAccess = true;

			if (this.CheckoutCart.Status == NWTD.Orders.Cart.CART_STATUS.SUBMITTED.ToString()) {

                this.ShowOrderComplete();

                
                //Response.Redirect(NavigationManager.GetUrl("ManageCarts"));
			}

			global::NWTD.Profile.SetSaleInformation();
			
			btnSubmitCart.Visible = this.UserIsLevelA;


			//populate the shiping and billing address fields
			if (this.UserIsLevelA) {
				//Level A Address Binding

				this.pnlShippingAndTax.Visible = true;
				//this.lblTitle.Text = "Order Summary";
            
            //The following adds the WebConfirmationNumber to the form header/title of the OrderSummary PDF (Heath Gardner 08/19/13)
                string webConf = this.CheckoutCart.GetString("WebConfirmation").ToString();
                if (!String.IsNullOrEmpty(webConf))
                {
                    //Create the form header/title text
                    string fullTitle = string.Format("Submitted Order -- Confirmation # {0}", webConf);
                    //Set the form header/title equal to the formatted text
                    this.lblTitle.Text = fullTitle;
                }
                else //deal with any NULL confirmation number
                {
                    this.lblTitle.Text = "Submitted Order -- Confirmation";
                }
            ////
                this.pnlLevelANotifications.Visible = true;
				this.pnlLevelBNotifications.Visible = false;
				this.btnPrintFooter.Visible = false;
				this.btnPrintHeader.Visible = true;
				this.lblRequisitionNotSubmitted.Visible = false;
			}

			//set the grid to have the line items as its data source
			this.gvCart.DataSource = this.CheckoutCartHelper.LineItems;
			this.gvCart.DataBind();

		}

		/// <summary>
		/// When a row is bound, do a few things.
		/// <list type="bullet">
		/// <item>Add the totals to the footer row</item>
		/// <item>Calculate the price based on quantity-gratis</item>
		/// <item>Handle null years coming in</item>
		/// </list>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
				Entry entry = CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId);

				Literal litYear = e.Row.FindControl("litYear") as Literal;
				if (litYear != null) {
					float year = 0f;
					float.TryParse(entry.ItemAttributes["Year"].ToString(), out year);
					if (year > 0) { litYear.Text = year.ToString("#"); }
				}

				
				Literal tbQuantityCharged = e.Row.FindControl("litQuantityCharged") as Literal;
				tbQuantityCharged.Text = item["Gratis"] == null ? item.Quantity.ToString("n0") : (item.Quantity - (decimal)item["Gratis"]).ToString("n0");
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
		protected void btnSubmitCart_Click(object sender, EventArgs e) {
			//This only occurs for Level A USERS
			//set the status on the cart
			this.CheckoutCart.Status = NWTD.Orders.Cart.CART_STATUS.SUBMITTED.ToString();
			//change the name of the cart so future carts with the same name can be created
			this.CheckoutCart.Name = this.CheckoutCart.Name + "_" + this.CheckoutCart.Id;
            string webConfirmationNumber = NWTD.Orders.Cart.GenerateOrderNumber(this.CheckoutCart); //generate a confirmation numbmer
            
            this.CheckoutCart["WebConfirmation"] = webConfirmationNumber; //set the confirmation number on the cart
            this.SaveChanges();//save the cart
           
            //send the email
			try {
                //we won't be converting to a PurchaseOrder anymore. Keep it as a cart, change the cart's status.
                //this.CheckoutCart.OrderNumberMethod = new Mediachase.Commerce.Orders.Cart.CreateOrderNumber(NWTD.Orders.Cart.GenerateOrderNumber);
				//this._puchaseOrder = this.CheckoutCart.SaveAsPurchaseOrder();
				try {
					this.SendConfirmationEmail();
				}
				catch (Exception ex) {
					this.OrderMessage.Text += string.Format("Your order was submitted, however there was a problem sending your confirmation email: {0}", ex.Message);
				}
			}
			catch (Exception ex) {
				this.OrderMessage.Text += string.Format("There was an error creating a purchase order: {0}", ex.Message);
			}
			
            //indicate that the order is complete
            this.ShowOrderComplete();
			//Response.Redirect(NavigationManager.GetUrl("OrderThanks"));

		}



		protected void lbGoToAddress_Command(object sender, CommandEventArgs e) {
			//if (e.CommandArgument.ToString() == "ShipTo") wzSubmitCart.ActiveStepIndex = wzSubmitCart.WizardSteps.IndexOf(wsShippingAddress);
			//if (e.CommandArgument.ToString() == "BillTo") wzSubmitCart.ActiveStepIndex = wzSubmitCart.WizardSteps.IndexOf(wsBillingAddress);
		}


		/// <summary>
		/// Send the user back to the shipping info page for the current cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnReturnToShippingInfo_Click(object sender, EventArgs e) {
			Response.Redirect(NavigationManager.GetUrl("Checkout", new object[] { "cart", this.CheckoutCart.Name }));
		}



		//protected void btnDeleteCart_Click(object sender, EventArgs e) {
		//    Guid userId = ProfileContext.Current.UserId;

		//    CartHelper helper = new CartHelper(this.CheckoutCart.Name, userId);

		//    helper.Cart.Delete();
		//    helper.Cart.AcceptChanges();

		//    Response.Redirect(NavigationManager.GetUrl("ManageCarts"));
			
		//}

		/// <summary>
		/// Once an order is submitted, shows that the order is complete by hiding and showing the appriate panel
		/// </summary>
        public void ShowOrderComplete() {
            this.pnlOrderSubmitted.Visible = true;
            this.pnlOrderSummary.Visible = false;
        }

		#endregion

	}
}