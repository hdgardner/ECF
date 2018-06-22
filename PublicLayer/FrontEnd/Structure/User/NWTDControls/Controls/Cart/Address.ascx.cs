using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using System.ComponentModel;
using Mediachase.Commerce.Orders;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.Cart {
	
	/// <summary>
	/// A control for displaying an customer address from an order
	/// </summary>
	public partial class Address : System.Web.UI.UserControl {
		
		private CustomerAddress _customerAddress;
		
		/// <summary>
		/// The order address
		/// </summary>
		public OrderAddress OrderAddress { get; set; }
		
		public CustomerAddress CustomerAddress { 
			get{
				if (this._customerAddress == null) {
					if (this.OrderAddress != null) {
						this._customerAddress = Mediachase.Cms.WebUtility.Commerce.StoreHelper.ConvertToCustomerAddress(this.OrderAddress);
					}
				}
				return this._customerAddress;
			} 
			set{
				this._customerAddress = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e) {
			this.litName.Text = this.CustomerAddress.Name;
			this.litCity.Text = this.CustomerAddress.City;
			this.litLine1.Text = this.CustomerAddress.Line1;
			this.litPostalCode.Text = this.CustomerAddress.PostalCode;
			this.litRegionName.Text = this.CustomerAddress.RegionName;
		}
	}

}