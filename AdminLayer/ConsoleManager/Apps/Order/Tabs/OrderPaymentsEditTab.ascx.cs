using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Orders;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class OrderPaymentsEditTab : BaseUserControl, IAdminTabControl, IAdminContextControl
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

            PaymentOptionList.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(OnNeedRebind);
            PaymentOptionList.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(OnNeedDataSource);
            PaymentOptionList.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_DeleteCommand);
            PaymentOptionList.PreRender += new EventHandler(DefaultGrid_PreRender);
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
            _removedItems.Add(e.Item);
        }

        /// <summary>
        /// Gets the payments.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public IEnumerable<Payment> GetPayments(OrderGroup order)
        {
            foreach (OrderForm orderForm in order.OrderForms)
            {
                foreach (Payment payment in orderForm.Payments)
                {
                    yield return payment;
                }
            }
        }

        /// <summary>
        /// Processes the table events.
        /// </summary>
        /// <param name="order">The order.</param>
        private void ProcessTableEvents(OrderGroup order)
        {
            foreach (GridItem item in _removedItems)
            {
                int id = Int32.Parse(item["PaymentId"].ToString());
                // find the existing one
                foreach (Payment payment in GetPayments(order))
                {
                    if (payment.PaymentId == id)
                        payment.Delete();
                }
            }

            _removedItems.Clear();
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
            PaymentOptionList.DataSource = GetPaymentOptionDataSource();
        }

        /// <summary>
        /// Called when [need rebind].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnNeedRebind(object sender, System.EventArgs oArgs)
        {
            PaymentOptionList.DataBind();
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(PaymentOptionList, "Order", "Payments");
            BindPaymentOptions();
        }

        /// <summary>
        /// Binds the payment options.
        /// </summary>
        private void BindPaymentOptions()
        {
            if (_order != null && _order.OrderForms.Count > 0)
			{
				PaymentOptionList.DataSource = GetPaymentOptionDataSource();
				PaymentOptionList.DataBind();
			}
        }

        /// <summary>
        /// Gets the payment option data source.
        /// </summary>
        /// <returns></returns>
        private object GetPaymentOptionDataSource()
        {
			if (_order != null && _order.OrderForms.Count > 0)
				return _order.OrderForms[0].Payments;

            return null;
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
            PaymentViewDialog.LoadContext(context);           
        }
        #endregion
    }
}