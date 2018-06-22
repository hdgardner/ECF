using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using NWTD.InfoManager;
 
namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS {
	
	/// <summary>
	/// This control provides an interface for Searching Orders in NWTD's InfoManager service.
	/// </summary>
	public partial class SearchOrders : NWTD.Web.UI.UserControls.InfoManagerUserControl {

		#region Event handlers

		/// <summary>
		/// When the page loads, we need to hide the address dropdown (it can still be dynamically shown with js)
		/// We also need to bind the current user's organization's addresses to the list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			OakTree.Web.UI.ControlHelper.RegisterControlInClientScript(this.Page.ClientScript, this.pnlSearchCriteria, "OrderSearchCriteria", string.Empty);
			this.ddlAddress.Attributes.CssStyle.Add("display", "none");
             
            if (this.Page.IsPostBack) return;

            if (this.UserOrganization != null) {
				this.ddlAddress.DataSource = UserOrganization.Addresses.Cast<CustomerAddress>().AsQueryable().Where(a => a.GetString("Type") == "S").OrderBy(a => a.FirstName);
				this.ddlAddress.DataBind();
			}
		}

		/// <summary>
		/// Executes the search and binds the results to the grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCatalogSearch_Click(object sender, EventArgs e) {

            var sortMode = OrderSearchResultSortMode.CreateDate;
            if(!string.IsNullOrEmpty(ddlSortBy.SelectedValue))
            {
                sortMode = (OrderSearchResultSortMode)int.Parse(ddlSortBy.SelectedValue);
            }
            
			this.gvSearchResults.DataSource = this.Search(sortMode);
			this.gvSearchResults.DataBind();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The purchase order we're searching
		/// </summary>
		public string PO{
			get{
				if(this.ddlSearchCriteria.SelectedValue == "PO") return this.EnsureValue(this.tbSearchBox.Text);
				return null;
			}
		}

		/// <summary>
		/// The invoice number we're searching
		/// </summary>
		public string InvoiceNumber{
			get{
				if(this.ddlSearchCriteria.SelectedValue == "Invoice") return this.EnsureValue(this.tbSearchBox.Text);
				return null;
			}
		}

		/// <summary>
		/// The order number we're searching
		/// </summary>
		public string OrderNumber{
			get{
				if(this.ddlSearchCriteria.SelectedValue == "Order") return this.EnsureValue(this.tbSearchBox.Text);
				return null;
			}
		}

		/// <summary>
		/// The web confirmation number (customer reference number) we're searching
		/// </summary>
		public string WebConfirmationNumber{
			get{
				if(this.ddlSearchCriteria.SelectedValue == "WebConfirmation") return this.EnsureValue( this.tbSearchBox.Text);
				return null;
			}
		}

		/// <summary>
		/// the isbn we're searching
		/// </summary>
		public string ISBN{
			get{
				if(this.ddlSearchCriteria.SelectedValue == "ISBN") return this.EnsureValue( this.tbSearchBox.Text);
				return null;
			}
		}

		/// <summary>
		/// the address we're searching for
		/// </summary>
		public string AddressKey {
			get {
				if (this.ddlSearchCriteria.SelectedValue == "ShipTo") return this.EnsureValue(this.ddlAddress.SelectedValue);
				return null;
			}
		}

		/// <summary>
		/// the organization (district) to which the current user belongs
		/// </summary>
		public Organization UserOrganization {
			get { return Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Organization; }
		}


		#endregion

		#region Methods

		/// <summary>
		/// Since we can't pass empty strings to the search (it'll return incorrect results) we have to null them if they're empty
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		private string EnsureValue(string Value){
			return string.IsNullOrEmpty(Value) ? null : Value;
		}

		/// <summary>
		/// executes the search with criteria based on the current state of the UI
		/// </summary>
		/// <returns></returns>
		public OrderHeaderDataSet.OrderHeadersDataTable Search(OrderSearchResultSortMode sortMode = OrderSearchResultSortMode.CreateDate) {
			DateTime startDate = DateTime.Now.AddDays((0 - int.Parse(this.ddlDateRange.SelectedValue)));

			OrderHeaderDataSet.OrderHeadersDataTable results = this.Client.SearchOrderHeaders(
				global::NWTD.Profile.GetUserBusinessPartnerID(Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account),
				//"305800", //business partner id
				startDate, //created date start
				DateTime.Now, //created date end
				sortMode, //sort
				NWTD.InfoManager.SortDirection.Descending, //direction
				1, //start idnex
				1000, //number of items
				this.InvoiceNumber, //_invoice_number
				this.PO, //_customer_ref_number
				this.ISBN, //_contains_isbn
				this.AddressKey, //_address_key
				this.WebConfirmationNumber, //_web_confirmation_number
				this.OrderNumber //_order_id
				);

			return  results;
		}



		#endregion

	    protected void HandleSortByIndexChanged(object sender, EventArgs e)
	    {
	        var sortMode = (OrderSearchResultSortMode) int.Parse(ddlSortBy.SelectedValue);
	        var results = Search(sortMode);
	        this.gvSearchResults.DataSource = results;
            this.gvSearchResults.DataBind();
	    }
	}
}