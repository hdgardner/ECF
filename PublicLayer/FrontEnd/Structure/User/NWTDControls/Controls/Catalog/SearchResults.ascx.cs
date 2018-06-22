using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.DataSources;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.WebUtility.Commerce;
using System.ComponentModel;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Catalog {
	
	/// <summary>
	/// Displays the results of a search. Note that this control doesn't actually perform the search.
	/// </summary>
	public partial class SearchResults : UserControl {

		/// <summary>
		/// Whether the user is a Nevada
		/// </summary>
		public bool IsNevadaUser { 
			get {
				if (NWTD.Profile.BusinessPartnerState != null)
					return NWTD.Profile.BusinessPartnerState.Equals("NV");
				return false;
			} 
		}

		/// <summary>
		/// The depository for the user
		/// </summary>
		public NWTD.InfoManager.Depository Depository { get; set; }

		/// <summary>
		/// The add to cart button at the top of the form
		/// </summary>
        public Button AddToCartTop {
            get { return this.btnAddSelectedTop; }
        }

		/// <summary>
		/// The add to cart button at the top of the form
		/// </summary>
        public Button AddToCartBottom {
            get { return this.btnAddToCurrentOrder; }
        }

		/// <summary>
		/// The results entries. This should get set by whatever code is actually cunduction the search.
		/// </summary>
		[Bindable(true)]
		public Entries Entries { get; set; }
		
		/// <summary>
		/// The total number of results. This should get set by whatever code is actually cunduction the search.
		/// </summary>
		[Bindable(true)]
		public int TotalResults { get; set; }

		/// <summary>
		/// The GridView where the results are displayed
		/// </summary>
		public GridView ResultsGrid { get { return this.gvSearchResults; } }
		
		/// <summary>
		/// Binds the search results information to the datasource object
		/// </summary>
		public override void DataBind() {
			
			this.dsSearchResults.TotalResults = this.TotalResults;
			this.dsSearchResults.CatalogEntries = this.Entries;

			base.DataBind();
		}

		#region Event Handlers

		/// <summary>
		/// Add an item to the current order. This is a backup for the Ajax version of this
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnAddToCurrentOrder_Click(object sender, EventArgs e) {
			foreach (GridViewRow row in this.gvSearchResults.Rows) {
				//this.Page.Request.Url.GetLeftPart(UriPartial.Path)
				CheckBox checkBox = row.FindControl("cbSelectEntry") as CheckBox;
				TextBox quantityBox = row.FindControl("tbQuantity") as TextBox;
				
				CartHelper ch = new CartHelper(global::NWTD.Profile.ActiveCart);
				decimal quantity;
				if (checkBox.Checked && decimal.TryParse(quantityBox.Text, out quantity)) {

					checkBox.Checked = false;
					quantityBox.Text = string.Empty;

					Entry itemToAdd = this.dsSearchResults.CatalogEntries[row.RowIndex];

					//if this item is already in the cart, get it's quantity, then add it to the entered quantity
					LineItem existingItem = ch.LineItems.SingleOrDefault(li => li.CatalogEntryId == itemToAdd.ID);
					if (existingItem != null) quantity += existingItem.Quantity;
					
					ch.AddEntry(this.dsSearchResults.CatalogEntries[row.RowIndex],quantity);
					//blSelectedItems.Items.Add(new ListItem(itemToAdd.Name, StoreHelper.GetEntryUrl(itemToAdd)));
				}
			}
			gvSearchResults.DataBind();
		}

		/// <summary>
		/// During page load, various scripts are loaded, and the last column of the search results grid for Nevada is shown if the current user is from Nevada
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {

			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			OakTree.Web.UI.ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this, "SearchResults", string.Empty);

			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryModal_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.modal.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "JqueryValidate_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/jquery.validate.js"));

			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "SearchResults_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/SearchResults.js"));

			if (this.Page.Request.QueryString.Count == 0)this.pnlSearchResults.Visible = false;

			//do a few special things if the user is from nevada
			this.gvSearchResults.Columns[this.gvSearchResults.Columns.Count - 1].Visible = this.IsNevadaUser;
			if (this.IsNevadaUser) this.pnlResultsWrapper.CssClass = "results-wrapper-nv";

			if(Request.IsAuthenticated)
				global::NWTD.Profile.EnsureCustomerCart();

			//set up the stuff in the cart selector modal
			if (global::NWTD.Orders.Cart.Reminder) {
				this.pnlSelectCart.Visible = true;
				List<Mediachase.Commerce.Orders.Cart> carts = global::NWTD.Orders.Cart.CurrentCustomerCarts;

				this.ddlCarts.DataSource = carts;
				this.ddlCarts.DataBind();

				foreach (ListItem item in this.ddlCarts.Items) {
					if (item.Value.Equals(global::NWTD.Profile.ActiveCart)) {
						item.Selected = true;
					    break;
					}
				}
			}
		

		}

		/// <summary>
		/// When we bind a row to an Entry, we need to
		/// <list type="bullet">
		/// <item>Format the Year field to have no decimal places</item>
		/// <item>Check the status code and do the appropriate action to the item</item>
		///	</list>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void gvSearchResults_RowDataBound(object sender, GridViewRowEventArgs e) {
			if(!e.Row.RowType.Equals(DataControlRowType.DataRow) )return;

			Entry entry = e.Row.DataItem as Entry;

 
			//Fiter by status code
            //Heath Gardner - 03/06/13 Below statement was erroneously returning "NWTD" for MSSD users, so I changed it to calculate Depository like Catalog.cs does 
			    //var statusCode  = this.Depository.Equals(NWTD.InfoManager.Depository.MSSD) ? entry.ItemAttributes["StatusCode_SLC"] : entry.ItemAttributes["StatusCode_PDX"];
            var statusCode = NWTD.Profile.CustomerDepository.Equals(NWTD.Depository.MSSD) ? entry.ItemAttributes["StatusCode_SLC"] : entry.ItemAttributes["StatusCode_PDX"];

			if (statusCode != null) {

				switch (statusCode.ToString()) {
					case "CX":
						e.Row.Enabled = false;
						e.Row.CssClass = e.Row.CssClass + " item-unavailable";
						break;
				}
			}

			//parse the year and set the control value
			Literal litYear = e.Row.FindControl("litYear") as Literal;
			if (litYear != null) {
				float year = 0f;
				float.TryParse(entry.ItemAttributes["Year"].ToString(), out year);
				if (year > 0) { litYear.Text = year.ToString("#"); }
			}
		}

		#endregion

	}
}