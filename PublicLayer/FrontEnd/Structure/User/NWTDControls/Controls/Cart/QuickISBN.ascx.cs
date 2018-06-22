using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// This control provides an interface for users to quicly add line items to the current cart by adding ISBN numbers to a text box
	/// </summary>
	public partial class QuickISBN : BaseStoreUserControl {
		protected void Page_Load(object sender, EventArgs e) {

		}

		/// <summary>
		/// The name of the cart to which ISBN entries shall be added
		/// </summary>
		public String SelectedCartName {
			get {
				if (!string.IsNullOrEmpty(Request["Cart"])) {
					return Request["Cart"];
				} else {
					return NWTD.Profile.ActiveCart;
				}
			}
		}

		public CartHelper SelectedCartHelper {
			get { return new CartHelper(SelectedCartName, ProfileContext.Current.UserId); }
		}

		private List<Entry> _successfulEntries;
		private List<string> _failedEntries;
		
		/// <summary>
		/// A list of entries that were sucessfully added
		/// </summary>
		protected List<Entry> SuccessfulEntries {
			get { 
				if (_successfulEntries == null) _successfulEntries = new List<Entry>();
				return _successfulEntries;
			} 
		}

		/// <summary>
		/// A list of entries that failed to be added
		/// </summary>
		protected List<string> FailedEntries {
			get {
				if (_failedEntries == null) _failedEntries = new List<string>();
				return _failedEntries;
			}
		}


		/// <summary>
		/// When a users click finish, loop through the entries (one per line) and try to find one in the catalog.
		/// If one is found, add it.
		/// At the end, show a list of successful and failed entries.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void wzImportISBNS_FinishButtonClick(object sender, WizardNavigationEventArgs e) {
			string[] ISBNs = tbISNBS.Text.Split(System.Environment.NewLine.ToCharArray());
			IEnumerable<string> values = ISBNs.Where(isbn => !string.IsNullOrEmpty(isbn));
			foreach (string code in values) {

				//clean it up, removing spaces from ends and hypens
				string cleanCode = code.Trim().Replace("-", string.Empty);

				Entry entry = CatalogContext.Current.GetCatalogEntry(cleanCode, new Mediachase.Commerce.Catalog.Managers.CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
				if (entry != null && NWTD.Catalog.IsEntryAvailable(entry)) {

					CartHelper ch = this.SelectedCartHelper;

					LineItem existingItem = ch.LineItems.SingleOrDefault(li => li.CatalogEntryId == entry.ID);
					if (existingItem != null) {
						this.FailedEntries.Add(string.Format("{0} - This item is already in your Wish List and cannot be added a second time.  If you need more, please return to your Wish List and increase the quantity of the existing item.", code));
					}
					else {

						ch.AddEntry(entry, 0);
						this.SuccessfulEntries.Add(entry);
					}
				} else {
					this.FailedEntries.Add(string.Format("{0} - The item with this ISBN does not exist.", code));
				}
			}

			this.blFailedISBNS.DataSource = this.FailedEntries;
			this.blFailedISBNS.DataBind();

			this.gvImportedEntries.DataSource = this.SuccessfulEntries;
			this.gvImportedEntries.DataBind();

			if (this.SuccessfulEntries.Count > 0) this.pnlImportSuccess.Visible = true;
			if (this.FailedEntries.Count > 0) this.pnlImportFailure.Visible = true;

		}


        protected void gvImportedEntries_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        // On 08/19/13, Heath Gardner commented out this function's logic & replaced it with a lookup of the value directly in QuickISBN.ascx
        // In QuickISBN.ascx: "<%# ((Mediachase.Commerce.Catalog.Objects.ItemAttributes)Eval("ItemAttributes"))["Year"]%>" 
        // replaces "<asp:Literal ID="litYear"></asp:Literal>"
        // This original configuration was NOT working and all items showed blank copyright. I don't know why this wasn't caught during testing???

            //Entry entry = e.Row.DataItem as Entry;
            //Literal litYear = e.Row.FindControl("litYear") as Literal;
            //if (litYear != null)
            //{
                //float year = 0f;
                //float.TryParse(entry.ItemAttributes["Year"].ToString(), out year);
                //if (year > 0) { litYear.Text = year.ToString("#"); }
            //}
        }


		/// <summary>
		/// If someone cancels, send them back to the cart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void wzImportISBNS_CancelButtonClick(object sender, EventArgs e) {
			Response.Redirect(NavigationManager.GetUrl("ViewCart", new object[] { "cart", this.SelectedCartName }));
		}
	}
}