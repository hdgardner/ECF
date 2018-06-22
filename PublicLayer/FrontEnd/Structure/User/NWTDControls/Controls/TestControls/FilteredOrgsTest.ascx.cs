using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.TestControls {
	public partial class FilteredOrgsTest : System.Web.UI.UserControl {

		public string State {
			get {
				return this.tbState.Text;
			}
			set {
				this.tbState.Text = value;
			}
		}

		protected void btnSubmit_Click(object sender, EventArgs e) {
			
			var searchParams = new Mediachase.Commerce.Profile.Search.ProfileSearchParameters();

			//We've tried experemnting with setting more properties on the ProfileSearchParameters object, but have had no success...
			//searchParams.JoinSourceTable = "Principal";
			//searchParams.JoinSourceTableKey = "Id";
			//searchParams.JoinTargetQuery = "(select distinct ObjectId, BusinessPartnerState from Principal_Organization) Principal_Organization";
			//searchParams.JoinTargetTableKey = "ObjectId";
			
			searchParams.SqlMetaWhereClause = String.Format("Meta.BusinessPartnerState = '{0}'", this.State);


			var searchOptions = new Mediachase.Commerce.Profile.Search.ProfileSearchOptions();

			searchOptions.Classes.Add("Organization");
			searchOptions.Namespace = "Mediachase.Commerce.Profile.System";
			searchOptions.StartingRecord = 0;
			
			Organization[] orgs = ProfileContext.Current.FindOrganizations(searchParams, searchOptions);

			this.rpFilteredOrgs.DataSource = orgs;
			this.rpFilteredOrgs.DataBind();

		}
	}
}