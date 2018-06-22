using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// A control for selecting from a list of addresses for a given Oranization
	/// </summary>
	public partial class OrganizationAddressSelector : System.Web.UI.UserControl {

		private string _selecetedAddressId;
		
		/// <summary>
		/// The internal ID of the selected address
		/// </summary>
		public string SelectedAddressId { 
			set {
				this._selecetedAddressId = value;
				this.rptShippingAddresses.DataBind();
			} 
		}

		/// <summary>
		/// Teh
		/// </summary>
		public CustomerAddress Address {
			get{
				foreach (RepeaterItem item in rptShippingAddresses.Items ) {
					GlobalRadioButton btn = item.FindControl("rbSelectedAddress") as GlobalRadioButton;
					if (btn.Checked)
						return ((Address)item.FindControl("CustomerAddress")).CustomerAddress;
				}
				return null;
			}
		}

		protected override void OnInit(EventArgs e) {
			if (ProfileContext.Current.Profile.Account.Organization != null) {
				this.rptShippingAddresses.DataSource = ProfileContext.Current.Profile.Account.Organization.Addresses;
			}
			this.rptShippingAddresses.DataBind();
			base.OnInit(e);
		}

		protected void rptShippingAddresses_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {

				if ( this._selecetedAddressId == null && e.Item.ItemIndex == 0 ) {
					GlobalRadioButton btn = e.Item.FindControl("rbSelectedAddress") as GlobalRadioButton;
					btn.Checked = true;
					return;
				}

				CustomerAddress address = e.Item.DataItem as CustomerAddress;
				if (address.Name.Equals(this._selecetedAddressId)) {
					GlobalRadioButton btn = e.Item.FindControl("rbSelectedAddress") as GlobalRadioButton;
					btn.Checked = true;
				}
			}
		}
	}
}