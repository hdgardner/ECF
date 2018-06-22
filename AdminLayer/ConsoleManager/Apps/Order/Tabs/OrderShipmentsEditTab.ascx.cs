using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Orders;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using System.Collections.Specialized;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class OrderShipmentsEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";

        OrderGroup _order = null;
        List<GridItem> _removedItems = new List<GridItem>();

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindForm();
            }

			ShipmentList.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(ShipmentList_OnNeedRebind);
			ShipmentList.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(ShipmentList_OnNeedDataSource);
			ShipmentList.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(ShipmentList_DeleteCommand);
			ShipmentList.PreRender += new EventHandler(ShipmentList_PreRender);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		#region Grid events
		/// <summary>
        /// Handles the OnNeedDataSource event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void ShipmentList_OnNeedDataSource(object sender, EventArgs oArgs)
        {
            ShipmentList.DataSource = GetShipmentsDataSource();
        }

        /// <summary>
        /// Handles the OnNeedRebind event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void ShipmentList_OnNeedRebind(object sender, System.EventArgs oArgs)
        {
            ShipmentList.DataBind();
        }

        /// <summary>
        /// Handles the PreRender event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ShipmentList_PreRender(object sender, EventArgs e)
		{
			// Postback happens so the grid will be completely updated, make sure to save all the changes
			if (this.IsPostBack)
				ProcessTableEvents(_order);
		}

        /// <summary>
        /// Handles the DeleteCommand event of the ShipmentList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void ShipmentList_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			GridItem item = e.Item;
			int id = 0;
			if (item["ShipmentId"] != null && Int32.TryParse(item["ShipmentId"].ToString(), out id))
			{
				// find the existing one
				foreach (Shipment shipment in _order.OrderForms[0].Shipments)
				{
					if (shipment.ShipmentId == id)
					{
						StringCollection shipmentLineItems = shipment.LineItemIndexes;
						shipment.Delete();

						// update all lineitems that were associated with this shipment
						foreach (LineItem li in _order.OrderForms[0].LineItems)
						{
							if (shipmentLineItems.Contains(li.LineItemId.ToString()))
							{
								li.ShippingAddressId = String.Empty;
								li.ShippingMethodId = Guid.Empty;
								li.ShippingMethodName = String.Empty;
							}
						}
					}
				}
			}
			//_removedItems.Add(e.Item);
		}
		#endregion

		/// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(ShipmentList, "Order", "Shipments");
			BindShipmentsGrid();
        }

        /// <summary>
        /// Binds the shipments grid.
        /// </summary>
        private void BindShipmentsGrid()
        {
			object dataSource = GetShipmentsDataSource();
			if (dataSource != null)
			{
				ShipmentList.DataSource = dataSource;
				ShipmentList.DataBind();
			}
        }

        /// <summary>
        /// Gets the shipments data source.
        /// </summary>
        /// <returns></returns>
        private object GetShipmentsDataSource()
        {
			if (_order != null && _order.OrderForms.Count > 0 && _order.OrderForms[0].Shipments.Count > 0)
				return _order.OrderForms[0].Shipments;

            return null;
        }

        /// <summary>
        /// Processes the table events.
        /// </summary>
        /// <param name="po">The po.</param>
        private void ProcessTableEvents(OrderGroup order)
		{
			foreach (GridItem item in _removedItems)
			{
				//int id = 0;
				//if (item["ShipmentId"] != null && Int32.TryParse(item["ShipmentId"].ToString(), out id))
				//{
				//    // find the existing one
				//    foreach (Shipment shipment in order.OrderForms[0].Shipments)
				//    {
				//        if (shipment.ShipmentId == id)
				//        {
				//            StringCollection shipmentLineItems = shipment.LineItemIndexes;
				//            shipment.Delete();

				//            // update all lineitems that were associated with this shipment
				//            foreach (LineItem li in order.OrderForms[0].LineItems)
				//            {
				//                if (shipmentLineItems.Contains(li.LineItemId.ToString()))
				//                {
				//                    li.ShippingAddressId = String.Empty;
				//                    li.ShippingMethodId = Guid.Empty;
				//                    li.ShippingMethodName = String.Empty;
				//                }
				//            }
				//        }
				//    }
				//}
			}

			_removedItems.Clear();
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
			OrderGroup order = (OrderGroup)context[_OrderContextObjectString];
            ProcessTableEvents(order);
        }
        #endregion

        #region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _order = (OrderGroup)context[_OrderContextObjectString];
            ShipmentViewDialog.LoadContext(context);           
        }
        #endregion
    }
}