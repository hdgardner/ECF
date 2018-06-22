using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NWTD.InfoManager;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.Pages;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS {
	
	/// <summary>
	/// Gets a list of local representatives for a given publisher and depository from NWTD's InfoManager service
	/// </summary>
	public partial class PublisherRepresentatives : System.Web.UI.UserControl, ICmsDataAdapter {

		#region Properties

		/// <summary>
		/// The Publisher code for which we'd like to get the local reps.
		/// This is retrieved from the query string
		/// </summary>
		protected string Code {
			get {
				if (!string.IsNullOrEmpty( Request["publisher"])) {
					return Request["publisher"];
				}
				return null;
			}
		}

		/// <summary>
		/// The Depository we'll be passing when we get the list
		/// </summary>
		protected Depository Depository { get; set; }

		#endregion

		#region Event Handlers

		/// <summary>
		/// When the page loads, we bind the repeater to the results of a call to NWTD's web service
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			OrderInfoManagerClient client = new OrderInfoManagerClient("OrderInfoBasic");	

			//if no code is passed, we must not try to call the service
			if(!string.IsNullOrEmpty(this.Code)){
				NWTD.InfoManager.PubRepresentativesSearchResult reps = client.GetPublisherRepresentatives(this.Depository, this.Code);
				
				this.rpRepresentatives.DataSource = reps.representatives;
				this.litPublisherName.Text = reps.publisher.name;
				this.rpRepresentatives.DataBind();
			}
			
		}

		/// <summary>
		/// When an item is bound to the repeater, we'll need to populate some sub-repeaters with
		/// phone and email information
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void rpRepresentatives_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			Repeater rpRepPhones = e.Item.FindControl("rpRepPhones") as Repeater;
			Repeater rpRepEmails = e.Item.FindControl("rpRepEmails") as Repeater;

			PubRepresentative rep = e.Item.DataItem as PubRepresentative;

			rpRepPhones.DataSource = rep.phones;
			rpRepPhones.DataBind();

			rpRepEmails.DataSource = rep.emails;
			rpRepEmails.DataBind();
		}

		#endregion

		#region ICmsDataAdapter Members
		/// <summary>
		/// Grabs the parameter information from the CMS Control Settings
		/// </summary>
		/// <param name="param"></param>
		public void SetParamInfo(object param) {
			try {
				ControlSettings settings = (ControlSettings)param;

				if (settings != null && settings.Params != null) {
					if (settings.Params["Depository"] != null)
						this.Depository = (Depository)Enum.Parse(typeof(Depository), settings.Params["Depository"].ToString());
				}

				this.DataBind();
			}
			catch {
			}
		}

		#endregion
	}
}