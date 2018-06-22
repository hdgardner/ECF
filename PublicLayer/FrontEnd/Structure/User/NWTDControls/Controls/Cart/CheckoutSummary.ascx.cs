using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using WebSupergoo.ABCpdf8;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// The a control used for displaying order summary. This control varies contextually. 
	/// For example, level A users will see a checkout button, while Level B users will only see a print button.
	/// 
	/// </summary>
	public partial class CheckoutSummary : System.Web.UI.UserControl {

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


	  

	    /// <summary>
		/// The Purchase order associated witht he cart.
		/// </summary>
        protected PurchaseOrder PurchaseOrder {
            get {
                
                if(this._puchaseOrder == null)
                    this._puchaseOrder = 
                        PurchaseOrder.LoadByCustomer(Mediachase.Commerce.Profile.ProfileContext.Current.UserId)
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

        public static string GetHost()
        {
            // the following is the original code of this function, prior to Heath's https compatability changes on 03/01/18, below
            //return "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
            
            // On 03/01/18, Heath added all the following for https compatability -----------------
            string protocolIdentifier = string.Empty;

            if (HttpContext.Current.Request.IsSecureConnection)
                protocolIdentifier = "https://";
            else
                protocolIdentifier = "http://";

            return protocolIdentifier + HttpContext.Current.Request.ServerVariables["HTTP_HOST"];            
            // ------------------------------------------------------------------------------------
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
            msg.Attachments.Add(new Attachment(Server.MapPath(string.Format("~/Orders/{0}.pdf", this.CheckoutCart.GetString("WebConfirmation")))));
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

			if (this.CheckoutCart.Status == NWTD.Orders.Cart.CART_STATUS.SUBMITTED.ToString()) {

                this.ShowOrderComplete();

                
                //Response.Redirect(NavigationManager.GetUrl("ManageCarts"));
			}

			global::NWTD.Profile.SetSaleInformation();
			
			btnSubmitCart.Visible = this.UserIsLevelA;

            //Hide/Show fields based on user level
            if (this.UserIsLevelA)
            {
                //Level A field settings
                this.pnlShippingAndTax.Visible = true;
                this.lblTitle.Text = "Order Summary";
                this.pnlLevelANotifications.Visible = true;
                this.pnlLevelBNotifications.Visible = false;
                this.btnPrintFooter.Visible = false;
                this.btnPrintHeader.Visible = true;
                this.lblRequisitionNotSubmitted.Visible = false;
            }
            else
            {
                //Level B field settings

                //Populate WishList Name & ID# for Requisitions (Heath Gardner 08/19/13)
                //Get the necessary values
                string wishListName = this.CheckoutCart.Name.ToString();
                string wishListNumber = this.CheckoutCart.OrderGroupId.ToString();
                //Build the text to return
                // On 08/02/17, Heath moved the WishList Name & ID to the form title per Scott S. & Customer Service request.
                //  I also reorganized the Wish List string below to display the WL Name and then Number. This change was
                //  made to keep the look here similiar to how it is displayed on the Cart/WishList view printouts (hdg)
                //this.wishListID.Text = string.Format("Wish List #{0} ({1})",wishListNumber, wishListName);
                this.lblTitle.Text = string.Format("Requisition Summary<br>{0} (#{1})", wishListName, wishListNumber);
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
           
            //Generate Confirmation Number
            string webConfirmationNumber = NWTD.Orders.Cart.GenerateOrderNumber(this.CheckoutCart);
            //Set the Confirmation Number on the cart
            this.CheckoutCart["WebConfirmation"] = webConfirmationNumber;
            //Save Confirmation # to cart so it can be used by CheckoutSummaryPrint for PDF (Heath Gardner 08/19/13)
            //NOTE: Must do save here before changing status and cart name. Otherwise cart won't be found for CheckoutSummaryPrint (hg)
            this.SaveChanges();

			//Set cart status to SUBMITTED
			this.CheckoutCart.Status = NWTD.Orders.Cart.CART_STATUS.SUBMITTED.ToString();
			//change the name of the cart so future carts with the same name can be created
			this.CheckoutCart.Name = this.CheckoutCart.Name + "_" + this.CheckoutCart.Id;
           
            //The following two lines have been moved to above, so we can not only generate the number but SAVE it as well (Heath Gardner 08/19/13)
            //string webConfirmationNumber = NWTD.Orders.Cart.GenerateOrderNumber(this.CheckoutCart); //generate a confirmation numbmer
            //this.CheckoutCart["WebConfirmation"] = webConfirmationNumber; //set the confirmation number on the cart

            //Create the Order Summary pdf via abcPDF
            using (var doc = new Doc())
            {
                doc.HtmlOptions.Media = MediaType.Print;

             //Heath Gardner Replaced below "AddImageUrl" code to work with multiple page documents per WebSuperGoo's documentation
                //doc.AddImageUrl(string.Format("{0}/checkout/ordersummaryprint.aspx?cart={1}&uid={2}&IsLevelA={3}",
                //                              GetHost(), Request["cart"], ProfileContext.Current.UserId,
                //                              NWTD.Profile.CurrentUserLevel.Equals(NWTD.UserLevel.A)));

         ////////Beginning of Added functionality to "inset", create mulitple page PDFs, and to "flatten" the PDF (Heath Gardner 03/22/13)
                int docID;
               
                //Inset doc object edges so PDF Margins are acceptable (HG 03/22/13)
                doc.Rect.Inset(30, 30);
                
                //Create the first page and save the docID (HG 03/22/13)
                    //Added 'true' to end of AddImageUrl call to "disable cache". Fixes duplicate PDF issue (Heath Gardner 03/22/13) 
                docID = doc.AddImageUrl(string.Format("{0}/checkout/ordersummaryprint.aspx?cart={1}&uid={2}&IsLevelA={3}",
                                              GetHost(), Request["cart"], ProfileContext.Current.UserId,
                                              NWTD.Profile.CurrentUserLevel.Equals(NWTD.UserLevel.A)),true,0,true);   
                
                //Chain the subsequent pages together. Stop when we reach a page that is not truncated (HG 03/22/13)
                while (true)
                {
                    doc.FrameRect();
                    if (!doc.Chainable(docID))
                        break;
                    doc.Page = doc.AddPage();
                    docID = doc.AddImageToChain(docID);
                }
                
                //Flatten each page of the PDF document per abcPDF's best practices (HG 03/22/13)
                for (int i = 1; i <= doc.PageCount; i++)
                {
                    doc.PageNumber = i;
                    doc.Flatten();
                }
        ////////End of of Added functionality to "inset", create mulitple page PDFs, and to "flatten" the PDF (Heath Gardner 03/22/13)

                doc.Save(Server.MapPath(string.Format("~/Orders/{0}.pdf", webConfirmationNumber)));
                doc.Clear();
            }
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