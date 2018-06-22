using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Mediachase.Commerce.Profile;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Cms.Website.Structure.Base.Controls.Cart.SharedModules;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Schema;
using NWTD;
using Newtonsoft.Json;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// A Control for displaying a customer cart
	/// </summary>
	public partial class Cart : System.Web.UI.UserControl {

        private IDictionary<string,object> getItemDict(LineItem item, decimal quantity, decimal price)
        {
            IDictionary<string, object> i = new Dictionary<string, object>();
            IDictionary<string, object> data = new Dictionary<string, object>();
            i["quantity"] = quantity;
            i["unitprice"] = Decimal.Round(price, 2);
            i["description"] = item.DisplayName;
            i["currency"] = "USD";
            i["language"] = "en";
            i["supplierid"] = item.CatalogEntryId;
            i["supplierauxid"] = item.CatalogEntryId;
            i["classdomain"] = "UNSPSC";
            i["classification"] = "55101509";
            i["uom"] = "EA";

            data["manufacturer"] = null;
            data["manufacturer_id"] = item.CatalogEntryId;
            data["test_with_curly_bracket"] = null;
            data["test_with_quotes"] = null;
            data["test_with_square_bracket"] = null;
            i["data"] = data;

            return i;
        }

        #region Properties

        /// <summary>
        /// The name of the selected cart. Reads the querystring first, then checks the current user's active cart.
        /// It'll then create the cart if it doesn't exist
        /// </summary>
        public String SelectedCartName {
			get {
				if (!string.IsNullOrEmpty(Request["Cart"])) {
					return Request["Cart"];
				} else {
					NWTD.Profile.EnsureCustomerCart();
					return NWTD.Profile.ActiveCart;
				}
                
			}
		}

		/// <summary>
		/// An ECF CartHelper for the selectd cart
		/// </summary>
		public CartHelper SelectedCartHelper {
			get { return new CartHelper(SelectedCartName, ProfileContext.Current.UserId); }
		}

        /// <summary>
        /// Builds the WishList Name with ID included (Heath Gardner 08/02/17)
        /// </summary>
        public String cartWishListLabel {
            get
            {
                string currentWishListID = this.SelectedCartHelper.Cart.OrderGroupId.ToString();
                string currentWishListName = this.SelectedCartName.ToString();

                return string.Format("{0} (#{1})", currentWishListName, currentWishListID);
            }
        }

        /// <summary>
        /// Returns an encoded PunchOut2Go blob
        /// </summary>
        protected string EncodedPunchOut2GoParams
        {
            get
            {
                IDictionary<string, object> body = new Dictionary<string, object>();
                body["edit_mode"] = 0;
                body["total"] = Decimal.Round(this.SelectedCartHelper.Cart.Total, 2);
                body["currency"] = "USD";
                IList<IDictionary<string, object>> items = new List<IDictionary<string, object>>();
                foreach (LineItem item in this.SelectedCartHelper.LineItems)
                {
                    if (item.Quantity - (decimal)item["Gratis"] > 0)
                    {
                        items.Add(this.getItemDict(item, item.Quantity - (decimal)item["Gratis"], item.ListPrice));
                    }
                    if ((decimal) item["Gratis"] > 0)
                    {
                        items.Add(this.getItemDict(item, (decimal) item["Gratis"], 0));
                    }
                }
                body["items"] = items;
                IDictionary<string, object> bdict = new Dictionary<string, object>();
                bdict["body"] = body;
                string json = JsonConvert.SerializeObject(bdict);
                string base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
                return base64;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the line items from the cart is represented by the supplied name and belongs to the current user
        /// </summary>
        /// <param name="CartName"></param>
        /// <returns></returns>
        public static System.Collections.IEnumerable getLineItems(String CartName){
			CartHelper helper =  new CartHelper(CartName, ProfileContext.Current.UserId);
			return helper.LineItems;
		}
	
		/// <summary>
		/// Find each checked line item and delete it
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDeleteSelected_Click(object sender, EventArgs e) {
			CartHelper helper = this.SelectedCartHelper;
			foreach (GridViewRow row in gvCart.Rows) {
				CheckBox checkBox = row.FindControl("cbItemSelected") as CheckBox;
				if (checkBox.Checked) {
					int result = 0;
					if (int.TryParse(gvCart.DataKeys[row.RowIndex].Value.ToString(), out result)) {
						foreach (LineItem item in helper.LineItems) {
							if (item.LineItemId == result) {
								item.Delete();
							}
						}
						helper.Cart.AcceptChanges();
					} 
				}
			}
			gvCart.DataBind();

		}

		/// <summary>
		/// Saves the changes made to the cart being viewed
		/// </summary>
		protected bool SaveChanges() {
			CartHelper helper = this.SelectedCartHelper;
			int index = 0;
			foreach (LineItem item in helper.LineItems) {
				TextBox tbGratis = gvCart.Rows[index].FindControl("tbGratis") as TextBox;
				TextBox tbQuantityCharged = gvCart.Rows[index].FindControl("tbQuantityCharged") as TextBox;

                var gratisValidator = gvCart.Rows[index].FindControl("lblGratisValidator") as Label;
                var qtyValidator = gvCart.Rows[index].FindControl("lblQtyValidator") as Label;

                if (string.IsNullOrEmpty(tbGratis.Text.Trim()))
                {
                    gratisValidator.Visible = true;
                    return false;
                }
                
                if(string.IsNullOrEmpty(tbQuantityCharged.Text.Trim()))
                {
                    qtyValidator.Visible = true;
                    return false;
                }
                    

				decimal newGratis = 0;
				decimal.TryParse(tbGratis.Text.Trim(), out newGratis);

				decimal newQuantityCharged = 0;
				decimal.TryParse(tbQuantityCharged.Text.Trim(),out newQuantityCharged);
				decimal newQty = newGratis + newQuantityCharged;

				///TODO: We should be ensuring that there is a "Gratis" meta field before casting it 
				if ((decimal?)item["Gratis"] != newGratis) {
					item["Gratis"] = newGratis;
				}

				if (newQty <= 0) { // remove
					//item.Delete();
					item.Quantity = newQty;
				} else if (newQty != item.Quantity) // update
					item.Quantity = newQty;

				index++;
			}

			helper.RunWorkflow("CartValidate");
			Mediachase.Commerce.Orders.Cart cart =  helper.Cart;
			NWTD.Orders.Cart.AssignTotals(ref cart);

			helper.Cart.AcceptChanges();

			lblCartMessage.Text = "Your changes have been saved";
		    return true;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// When the object datasource is doing its selecting, we need to set the "CartName" input parameter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void dsCart_Selecting(object sender, ObjectDataSourceSelectingEventArgs e) {
			e.InputParameters["CartName"] = this.SelectedCartName;
		}

		/// <summary>
		/// Save the changes, then re-bind the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSaveChanges_Click(object sender, EventArgs e) {
			if(this.SaveChanges())
			    gvCart.DataBind();

		}

        /// <summary>
        /// Save the changes, then submit to PunchOut2Go
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmitPO2Go_Click(object sender, EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                if (this.SaveChanges())
                {
                    Session["po2go_outbound_params"] = this.EncodedPunchOut2GoParams;
                    Response.Redirect("/PO2Go.aspx");
                }
            }
        }

        /// <summary>
        /// When the data is bound to the Cart grid, show/hide certain buttons depending on whether the data is empty
        /// Also, re-name the page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvCart_DataBound(object sender, EventArgs e) {
			this.Page.Title = this.SelectedCartHelper.Cart.Name;
			if (SelectedCartHelper.LineItems.Count() == 0) {
				this.btnSubmit.Visible = false;
				this.btnDeleteSelectedTop.Visible = false;
				this.btnSubmitTop.Visible = false;
				this.btnSaveChanges.Visible= false;
				this.btnDeleteSelected.Visible = false;
			}
		}


		/// <summary>
		/// When the row gets bound to its data, we need to do some things to present the data correctly
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void gvCart_RowDataBound(object sender, GridViewRowEventArgs e) {

			//the footer row should only have two cells
			if (e.Row.RowType == DataControlRowType.Footer) {
				e.Row.Cells[0].Attributes["colspan"] = (e.Row.Cells.Count - 2).ToString();
				for (int i = 1; i < e.Row.Cells.Count - 2; i++) {
					e.Row.Cells[i].Visible = false;
				}
				
				e.Row.Cells[e.Row.Cells.Count - 2].Text = CurrencyFormatter.FormatCurrency(NWTD.Orders.Cart.CartTotal(this.SelectedCartHelper.Cart),this.SelectedCartHelper.Cart.BillingCurrency);
			}
			
			
			if (e.Row.RowType == DataControlRowType.DataRow) {
				LineItem item = e.Row.DataItem as LineItem;

				TextBox tbGratis = e.Row.FindControl("tbGratis") as TextBox;
				TextBox tbQuantityCharged = e.Row.FindControl("tbQuantityCharged") as TextBox;

				tbQuantityCharged.TabIndex = (short)((e.Row.RowIndex + 1) + e.Row.RowIndex);
				tbGratis.TabIndex = (short)((e.Row.RowIndex + 2) + e.Row.RowIndex);

				tbGratis.Text = item["Gratis"] == null? "0" : item["Gratis"].ToString();
                tbQuantityCharged.Text = item["Gratis"] == null ? item.Quantity.ToString("n0") : (item.Quantity - (decimal)item["Gratis"]).ToString("n0");

                if (tbQuantityCharged.Text.IndexOf(',') > -1)
                    tbQuantityCharged.Text = tbQuantityCharged.Text.Replace(",", string.Empty);

                //can't do this cuz it overwrites the alternating style css class, unfortunately we'll do it in javascript
                //if (item.Quantity < 1) e.Row.CssClass = e.Row.CssClass + " nwtd-cartrow-zero-quanity";

				//get the entry from the lineitem
				Entry entry = CatalogContext.Current.GetCatalogEntry(item.CatalogEntryId);
				
                if (entry == null) return; //if no entry was found, we need to abort, and leave the rest of the row empty

                //handle the ISBN column
                Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.ISBN isbn = e.Row.FindControl("ISBN") as Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog.ISBN;
                isbn.Entry = entry;
							
                //handle the year column
                String Year = string.Empty;
				if(entry.ItemAttributes["Year"] != null)
					 Year = !string.IsNullOrEmpty(entry.ItemAttributes["Year"].ToString())? decimal.Parse(entry.ItemAttributes["Year"].ToString()).ToString("#") : String.Empty;
                //we have to deal with the possibility of a null year.
                Literal litYear = e.Row.FindControl("litYear") as Literal;
                if (litYear != null) {
                    float year = 0f;
                    float.TryParse(entry.ItemAttributes["Year"].ToString(), out year);
                    if (year > 0) { litYear.Text = year.ToString("#"); }
                }
               
                //handle the grade column
                String Grade = entry.ItemAttributes["Grade"] != null ? entry.ItemAttributes["Grade"].ToString() : String.Empty;
				Literal litGrade = e.Row.FindControl("litGrade") as Literal;
				litGrade.Text = Grade;
				
				CheckBox cbItemSelected = e.Row.FindControl("cbItemSelected") as CheckBox;

				if (!NWTD.Orders.Cart.CartCanBeEdited(this.SelectedCartHelper.Cart)) {
					tbGratis.Enabled = false;
					tbQuantityCharged.Enabled = false;
					cbItemSelected.Enabled = false;
					
				}

			}
		}


		/// <summary>
		/// When a user clicks the submit button, we'll save the changes on the cart and send them off ot the checkout page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSubmit_Click(object sender, EventArgs e)
		{
		    
		    Page.Validate();

            if (Page.IsValid)
            {
                if(this.SaveChanges())
                    Response.Redirect(NavigationManager.GetUrl("Checkout", new object[] {"cart", this.SelectedCartName}));
            }
		}

		/// <summary>
		/// When the page loads, we'll get the hyperlinks set up, and make sure the js for this control is present
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {



			global::NWTD.Profile.SetSaleInformation();

			if (!NWTD.Orders.Cart.CartCanBeEdited(this.SelectedCartHelper.Cart)) {
				this.hlISBNQuickEntryBottom.Visible = false;
				this.hlISBNQuickEntryTop.Visible = false;
				this.btnDeleteSelected.Visible = false;
				this.btnDeleteSelectedTop.Visible = false;
				this.btnSaveChanges.Visible = false;
				this.btnSubmitTop.Visible = false;
				this.btnSubmit.Visible = false;
			}

			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "Cart_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/Cart.js"));
			OakTree.Web.UI.ControlHelper.RegisterControlInClientScript(Page.ClientScript, this, "Carts", "{updatePanelID:'" + this.udpViewCart.ClientID + "'}");

			string quickISBNUrl = NavigationManager.GetUrl("QuickISBN", new object[] { "cart", this.SelectedCartName });

			this.hlISBNQuickEntryBottom.NavigateUrl = quickISBNUrl;
			this.hlISBNQuickEntryTop.NavigateUrl = quickISBNUrl;
		}

        #endregion

    }
}