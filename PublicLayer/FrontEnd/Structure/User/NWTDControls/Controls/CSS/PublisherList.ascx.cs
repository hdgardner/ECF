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
	/// Retrieves a list of publishers for a given depository from NWTD's InfoManager service
	/// </summary>
	public partial class PublisherList : System.Web.UI.UserControl, ICmsDataAdapter {
		
		/// <summary>
		/// The Depository we'll be passing when we get the list
		/// </summary>
		protected Depository Depository { get; set; }
		
		/// <summary>
		/// When the page loads, we'll use NWTD's web service to grab the names of the publishers for this Depository
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			OrderInfoManagerClient client = new OrderInfoManagerClient("OrderInfoBasic");
			this.gvPublisherList.DataSource = client.GetPublishersByDepository(this.Depository);
			this.gvPublisherList.DataBind();
		}

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