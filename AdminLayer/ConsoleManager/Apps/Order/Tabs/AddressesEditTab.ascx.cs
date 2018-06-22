using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders.Dto;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class AddressesEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";

        OrderGroup _order;
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

            AddressList.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(OnNeedRebind);
            AddressList.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(OnNeedDataSource);
            AddressList.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_DeleteCommand);
            AddressList.PreRender += new EventHandler(DefaultGrid_PreRender);
        }

        /// <summary>
        /// Handles the PreRender event of the DefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DefaultGrid_PreRender(object sender, EventArgs e)
        {
            // Postback happens so the grid will be completely updated, make sure to save all the changes
            if (this.IsPostBack)
                ProcessTableEvents(_order);
        }

        /// <summary>
        /// Handles the DeleteCommand event of the DefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void DefaultGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            //_removedItems.Add(e.Item);
			int id = Int32.Parse(e.Item["OrderGroupAddressId"].ToString());
			// find the existing one
			foreach (OrderAddress addr in _order.OrderAddresses)
			{
				if (addr.OrderGroupAddressId == id)
					addr.Delete();
			}
        }

        /// <summary>
        /// Processes the table events.
        /// </summary>
        /// <param name="po">The po.</param>
        private void ProcessTableEvents(OrderGroup order)
        {
			//foreach (GridItem item in _removedItems)
			//{
			//    int id = Int32.Parse(item["OrderGroupAddressId"].ToString());
			//    // find the existing one
			//    foreach (OrderAddress addr in order.OrderAddresses)
			//    {
			//        if (addr.OrderGroupAddressId == id)
			//            addr.Delete();
			//    }
			//}

			//_removedItems.Clear();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Called when [need data source].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnNeedDataSource(object sender, EventArgs oArgs)
        {
            AddressList.DataSource = GetAddressesDataSource();
        }
        /// <summary>
        /// Called when [need rebind].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnNeedRebind(object sender, System.EventArgs oArgs)
        {
            AddressList.DataBind();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(AddressList, "Order", "Addresses");
            BindAddresses();
        }

        /// <summary>
        /// Binds the addresses.
        /// </summary>
        private void BindAddresses()
        {
            if (_order != null && _order.OrderAddresses.Count > 0)
			{
				AddressList.DataSource = GetAddressesDataSource();
				AddressList.DataBind();
			}
        }

        /// <summary>
        /// Gets the addresses data source.
        /// </summary>
        /// <returns></returns>
        private object GetAddressesDataSource()
        {
			if (_order != null && _order.OrderAddresses.Count > 0)
				return _order.OrderAddresses;

            return null;
        }

        /// <summary>
        /// Handles the ItemDataBound event of the PaymentOptionList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.</param>
        protected void PaymentOptionList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            /*
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem == null)
                    return;

                PaymentMethodDto.PaymentMethodRow listItem = ((PaymentMethodDto.PaymentMethodRow)((DataRowView)e.Item.DataItem).Row);

                // Check the item if it has been already selected
                if (ViewState["PaymentMethod"] != null)
                {
                    Guid selectedPayment = new Guid(ViewState["PaymentMethod"].ToString());
                    if (listItem.PaymentMethodId == selectedPayment)
                    {
                        //GlobalRadioButton radioButton = (GlobalRadioButton)e.Item.FindControl("PaymentOption");
                        //radioButton.Checked = true;
                    }
                }

                // Retrieve the Label control in the current DataListItem.
                PlaceHolder optionPane = (PlaceHolder)e.Item.FindControl("PaymentOptionHolder");
                System.Web.UI.Control paymentCtrl = ManagementHelper.LoadPaymentPlugin(this, listItem.SystemKeyword);
                paymentCtrl.ID = listItem.SystemKeyword;
                paymentCtrl.EnableViewState = true;
                optionPane.Controls.Add(paymentCtrl);
                //TestPaymentOptionHolder.Controls.Add(paymentCtrl);
            }
            */
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();            
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
            AddressViewDialog.LoadContext(context);           
        }
        #endregion
    }
}