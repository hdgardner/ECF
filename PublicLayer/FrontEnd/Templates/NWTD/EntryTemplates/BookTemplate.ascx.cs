using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using System.Collections;
using Mediachase.Cms.WebUtility.Commerce;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility.BaseControls;

namespace Mediachase.Cms.Website.Templates.NWTD.EntryTemplates {
	public partial class BookTemplate : BaseEntryTemplate {

		public global::NWTD.InfoManager.Depository Depository {get;set;}

		public override void LoadContext(IDictionary context) {
			if (context.Contains("Depository")) this.Depository = (global::NWTD.InfoManager.Depository)(context["Depository"]);
			base.LoadContext(context);

		}

		/// <summary>
		/// Loads the new entry.
		/// </summary>
		/// <returns></returns>
		protected override Entry LoadNewEntry() {
			// Load using base implementation
			Entry entry = base.LoadNewEntry();
			

			// Add additional association to accomodate service plan and demonstrate dynamic products
			List<Association> associationList = new List<Association>();

			Association optionsAssoc = null;
			if (entry.Associations != null) {
				foreach (Association assoc in entry.Associations) {
					if (assoc.Name.Equals("AdditionalOptions", StringComparison.OrdinalIgnoreCase)) {
						optionsAssoc = assoc;
					}

					associationList.Add(assoc);
				}
			}

			if (optionsAssoc == null) {
				optionsAssoc = new Association();
				optionsAssoc.Name = "AdditionalOptions";
				optionsAssoc.Description = "Additional Options";
				associationList.Add(optionsAssoc);
			}

			List<EntryAssociation> entryAssociationList = new List<EntryAssociation>();

			if (optionsAssoc.EntryAssociations == null)
				optionsAssoc.EntryAssociations = new EntryAssociations();

			if (optionsAssoc.EntryAssociations.Association != null) {
				foreach (EntryAssociation assoc in optionsAssoc.EntryAssociations.Association) {
					entryAssociationList.Add(assoc);
				}
			}

			// Add new association and made up entry
			Entry newEntry = new Entry();
			newEntry.ID = "@2YEARSERVICEPLAN-" + entry.ID;
			newEntry.Name = "2 Years Service Plan";
			newEntry.ItemAttributes = new ItemAttributes();
			newEntry.ItemAttributes.MinQuantity = 1;
			newEntry.ItemAttributes.MaxQuantity = 1;
			newEntry.ParentEntry = entry;

			Price listPrice = StoreHelper.GetSalePrice(entry, 1);
			newEntry.ItemAttributes.ListPrice = ObjectHelper.CreatePrice(decimal.Multiply(listPrice.Amount, (decimal)0.10), listPrice.CurrencyCode);

			EntryAssociation newAssociation = new EntryAssociation();
			newAssociation.AssociationType = "OPTIONAL";
			newAssociation.SortOrder = 0;
			newAssociation.AssociationDesc = "";
			newAssociation.Entry = newEntry;
			entryAssociationList.Add(newAssociation);

			optionsAssoc.EntryAssociations.Association = entryAssociationList.ToArray();
			entry.Associations = associationList.ToArray();

			return entry;
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnLoad(EventArgs e) {

			//non anon users allowed. ECF doesn't allow page-level Access control... only folder-level
			if (!Page.User.Identity.IsAuthenticated) {
				Response.Redirect(ResolveUrl( string.Format("~/login.aspx?ReturnUrl={0}",HttpUtility.UrlEncode( CMSContext.Current.CurrentUrl)) ));
			}

			var statusCode  = this.Depository.Equals(global::NWTD.InfoManager.Depository.MSSD) ? this.Entry.ItemAttributes["StatusCode_SLC"] : this.Entry.ItemAttributes["StatusCode_PDX"];
			
			if (statusCode != null) {

				switch (statusCode.ToString()) {
					case "CX":
						this.addToCart.Visible = false;
						break;
				}
			}

			

			base.OnLoad(e);
			DataBind();
		}

		/// <summary>
		/// Utility method for turning an object into a year string from the entity's metadata
		/// </summary>
		/// <param name="Year"></param>
		/// <returns></returns>
		protected  string ParseYear(object Year) {
			string yearString = Year.ToString();
			float yearFloat = 0;
			//if the string can be parsted, return it with no decimal places
			if (float.TryParse(yearString, out yearFloat)) {
				return yearFloat.ToString("#");
			}
			//otherwise, return the default text
			return "n/a";
		}
	}
}