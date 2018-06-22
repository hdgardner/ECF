using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NWTD.InfoManager;
using Mediachase.Commerce.Profile;

namespace NWTD.Web.UI.UserControls{
	
	/// <summary>
	/// A user control that can be used in ECF to display data retrieved from NWTD's InfoManager service.
	/// </summary>
	public class InfoManagerUserControl : System.Web.UI.UserControl {
		private OrderInfoManagerClient _client;
		
		/// <summary>
		/// The web service client we'll use to get the data
		/// </summary>
		public OrderInfoManagerClient Client {
			get {
				if (this._client == null) {
					this._client = new OrderInfoManagerClient("OrderInfoBasic");
				}
				return this._client;
			}
		}

		/// <summary>
		/// The Current User's organization.
		/// </summary>
		public Organization Organization {
			get { 
				return  Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Organization;
			}
		}

	}
}
