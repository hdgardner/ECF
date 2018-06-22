using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NWTD.InfoManager;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS {

	/// <summary>
	/// Code for displaying the details of an order retrieved from NWTD's InfoManager service.
	/// </summary>
	public partial class OrderDetails : NWTD.Web.UI.UserControls.InfoManagerUserControl{

		#region Fields

		private Order _order;

		#endregion

		#region Properties

		/// <summary>
		/// The order in InfoManager to retrieve
		/// </summary>
		public string OrderID {
			get {
				return this.Request["OrderId"];
			}
			
		}

		/// <summary>
		/// The order in Infomanager
		/// </summary>
		public Order Order {
			get {
				if (this._order == null) {
					if (!string.IsNullOrEmpty(this.OrderID)) {
						this._order = this.Client.GetOrderDetailByERPId(int.Parse(this.OrderID)); 
					}
				}
				return this._order;
			}
		
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// During page load, the businessparnter ID is retrieved fromt he current user and compared with the Order
		/// If it matches, we show the Order. Otehrwise an acces denied panel is shown.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
			if (this.Order != null) {

				if (this.Order.Header.BusinessPartnerId != NWTD.Profile.BusinessPartnerID) {
					this.pnlAccessDenied.Visible = true;
					this.pnlOrderDetails.Visible = false;
					this.gvOderItems.Visible = false;
					return;
				}

				this.gvOderItems.DataSource = this.Order.Lines;
				this.gvOderItems.DataBind();
			}
		}

		/// <summary>
		/// This evenat hanler handles sorting choices in the sort dropdown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e) {
			switch(this.ddlSort.SelectedValue){
				case "ISBN":
					this.gvOderItems.DataSource =  this.Order.Lines.OrderBy(line => line.ItemCode);
					break;
				case "ShipDate":
					//TODO: we're going to have to create our own comparision function, since the Invoice could be null
					this.gvOderItems.DataSource = this.Order.Lines.OrderByDescending(lines => (lines.Invoice != null ?  lines.Invoice.Header.ShipDate: new DateTime()));
					break; 
				case "LineNumber":
					this.gvOderItems.DataSource =  this.Order.Lines.OrderBy(lines => int.Parse( lines.Reserved1));
					break;
				default:
					return; 
			}

			//this.gvOderItems.DataSource = this.Order.Lines;
			this.gvOderItems.DataBind();
		}

		#endregion


	}
}