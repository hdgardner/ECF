using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.DataSources;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.User {
	
	/// <summary>
	/// This control prvides a dropdown list of districts from which to choose.
	/// </summary>
	[ControlValueProperty("OrganizationID"), SupportsEventValidation, ValidationProperty("OrganizationID")]
	public partial class DistrictSelector : System.Web.UI.UserControl {

        public NWTD.Depository Depository { get; set; }

        public Dictionary<string,string> States {
            get {
                Dictionary<string, string> states = new Dictionary<string, string>();
                if (this.Depository == NWTD.Depository.NWTD){
                    states.Add("OR", "Oregon");
                    states.Add("WA", "Washington");
                    states.Add("AK","Alaska");
                }
                else {
                    states.Add("NV", "Nevada");
                    states.Add("UT", "Utah");
                }
                return states;
            }
            
        }

		/// <summary>
		/// The selected State
		/// </summary>
		public string State {
			get { return this.ddlState.SelectedValue; }
		}

		/// <summary>
		/// The ID of the selected Organization (district)
		/// first we check the Districts dropdown for a selection. If there's no selection, we'll check the non-districts dropdown
		/// 
		/// </summary>
		public string OrganizationID {
			get {
				return (!string.IsNullOrEmpty(this.ddlDistricts.SelectedValue)?this.ddlDistricts.SelectedValue:this.ddlNonDistricts.SelectedValue);
			}
		}

		public override void DataBind() {

			/* This code is too slow to use.
			var searchParams = new Mediachase.Commerce.Profile.Search.ProfileSearchParameters();
			var searchOptions = new Mediachase.Commerce.Profile.Search.ProfileSearchOptions();
			searchOptions.Classes.Add("Organization");
			searchOptions.Namespace = "Mediachase.Commerce.Profile.System";
			searchOptions.StartingRecord = 0;
			searchOptions.RecordsToRetrieve = 5000;
			Organization[] orgs = ProfileContext.Current.FindOrganizations(searchParams, searchOptions);
			ddlDistricts.DataSource = orgs;
			ddlDistricts.DataBind();
			*/


		}

		/// <summary>
		/// During page load, get all the required JS loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			NWTD.Web.UI.ClientScript.AddRequiredScripts(this.Page);
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "DistrictSelector_js", CommerceHelper.GetAbsolutePath("Structure/User/NWTDControls/Scripts/DistrictSelector.js"));

            if (this.Page.IsPostBack) return;
            this.ddlState.DataSource = this.States;
            this.ddlState.DataBind();
		}

        protected void ddlDistricts_DataBound(object sender, EventArgs e){
            ddlDistricts.Items.Insert(0, new ListItem("Select School District", ""));
        }


		protected void ddlNonDistricts_DataBound(object sender, EventArgs e) {
			ListItem otherItem = ddlNonDistricts.Items.FindByText(ddlState.SelectedItem.Text);
			if (otherItem != null) {
				otherItem.Text = "Other";
				ddlNonDistricts.Items.Remove(otherItem);
				ddlNonDistricts.Items.Add(otherItem);
			}

			ddlNonDistricts.Items.Insert(0, new ListItem("Select option", ""));
		}
	}
}