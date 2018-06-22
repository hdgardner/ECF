using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order
{
    /// <summary>
    /// Displays main order screen.
    /// </summary>
    public partial class OrderView : OrderBaseUserControl
    {
		private const string _OrderGroupDtoSessionKey = "ECF.OrderGroup.Edit";
		private const string _OrderContextObjectString = "OrderGroup";

		private const string _PaymentPlanViewId = "PaymentPlan";
		private const string _PurchaseOrderViewId = "PurchaseOrder";
		private const string _ShoppingCartViewId = "ShoppingCart";

		private const string _PaymentPlanClassName = "PaymentPlan";
		private const string _PurchaseOrderClassName = "PurchaseOrder";
		private const string _ShoppingCartClassName = "ShoppingCart";

        /// <summary>
        /// Gets the app id.
        /// </summary>
        /// <value>The app id.</value>
        public string AppId
        {
            get
            {
                return ManagementHelper.GetAppIdFromQueryString();
            }
        }

        /// <summary>
        /// Gets the view id.
        /// </summary>
        /// <value>The view id.</value>
        public string ViewId
        {
            get
            {
                return ManagementHelper.GetViewIdFromQueryString();
            }
        }

        /// <summary>
        /// Gets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("id");
            }
        }

        /// <summary>
        /// Gets the customer id.
        /// </summary>
        /// <value>The customer id.</value>
        public Guid CustomerId
        {
            get
            {
                return ManagementHelper.GetGuidFromQueryString("CustomerId");
            }
        }

		/// <summary>
		/// Gets the type of the filter.
		/// </summary>
		/// <value>The type of the filter.</value>
		public string FilterType
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["filter"], "all");
			}
		}

		/// <summary>
		/// Gets the type of the class.
		/// </summary>
		/// <value>The type of the class.</value>
		public string ClassType
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["class"], _PurchaseOrderClassName);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();

            // bind top toolbar
            MetaToolbar1.ViewName = this.AppId;
            MetaToolbar1.PlaceName = this.ViewId;
            MetaToolbar1.DataBind();

            if (!this.IsPostBack)
                DataBind();
        }

        /// <summary>
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private OrderGroup LoadFresh()
        {
            OrderGroup order = null;
            if (this.ViewId.StartsWith(_PaymentPlanViewId, StringComparison.OrdinalIgnoreCase))
                order = OrderContext.Current.GetPaymentPlan(CustomerId, OrderGroupId);
            else if (this.ViewId.StartsWith(_PurchaseOrderViewId, StringComparison.OrdinalIgnoreCase))
                order = OrderContext.Current.GetPurchaseOrder(CustomerId, OrderGroupId);
            else if (this.ViewId.StartsWith(_ShoppingCartViewId, StringComparison.OrdinalIgnoreCase))
                order = OrderContext.Current.GetCart(CustomerId, OrderGroupId);

            // persist in session
			Session[_OrderGroupDtoSessionKey] = order;

            return order;
        }

		private Cart CreateFreshCart()
		{
			Cart cart = null;
			// create empty order (cart)
			Guid userId = CustomerId;
			string name = String.Empty;
			if (userId == Guid.Empty)
			{
                userId = ProfileContext.Current.UserId;
                    /*
				if (ProfileContext.Current.Profile != null &&
					ProfileContext.Current.Profile.Account != null)
				{
					userId = ProfileContext.Current.Profile.Account.PrincipalId;
					name = ProfileContext.Current.Profile.Account.Name;
				}

                ProfileContext.Current.UserId
                     * */
			}
			cart = OrderContext.Current.GetCart(Cart.DefaultName, userId);
			// clear the cart
			cart.OrderAddresses.Clear();
			cart.OrderForms.Clear();
			cart.ShippingTotal = 0;
			cart.SubTotal = 0;
			cart.TaxTotal = 0;
			cart.Total = 0;
			cart.Status = String.Empty;

			// set initial values
			cart.BillingCurrency = CommonSettingsManager.GetDefaultCurrency();
			cart.CustomerName = name;

			OrderForm orderForm = new OrderForm();
			orderForm.Name = Cart.DefaultName;
			cart.OrderForms.Add(orderForm);

			return cart;
		}

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            //if (OrderGroupId > 0)
            {
                OrderGroup order = null;
				bool cartLoaded = false;
				Cart cart = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    order = LoadFresh();

					if (order == null)
					{
						cart = CreateFreshCart();

						cartLoaded = true;

						// persist in session
						Session[_OrderGroupDtoSessionKey] = cart;
					}
                }
                else // load from session
                {
					order = (OrderGroup)Session[_OrderGroupDtoSessionKey];

                    if (order == null)
                        order = LoadFresh();
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
				if (cartLoaded)
					dic.Add(_OrderContextObjectString, cart);
				else
					dic.Add(_OrderContextObjectString, order);

                // Call tabs load context
                ViewControl.LoadContext(dic);

                // Load order group context
                OrderGroupEdit.LoadContext(dic);
            }

			EditSaveControl.SavedClientScript = String.Format("CSManagementClient.ChangeView('Order', 'Orders-List', 'filter={0}&class={1}');", FilterType, ClassType);
			EditSaveControl.CancelClientScript = String.Format("CSManagementClient.ChangeView('Order', 'Orders-List', 'filter={0}&class={1}');", FilterType, ClassType);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);

            if (ViewControl != null)
            {
                ViewControl.AppId = ManagementHelper.GetAppIdFromQueryString();
                ViewControl.ViewId = ManagementHelper.GetViewIdFromQueryString();
                ViewControl.MDContext = Mediachase.Commerce.Orders.OrderContext.MetaDataContext;
            }
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void EditSaveControl_SaveChanges(object sender, EventArgs e)
        {
            OrderGroup order = (OrderGroup)Session[_OrderGroupDtoSessionKey];
			bool cartLoaded = false;
			Cart cart = null;

			if (order == null && OrderGroupId > 0)
			{
				if (this.ViewId.StartsWith(_PaymentPlanViewId, StringComparison.OrdinalIgnoreCase))
					order = OrderContext.Current.GetPaymentPlan(CustomerId, OrderGroupId);
				else if (this.ViewId.StartsWith(_PurchaseOrderViewId, StringComparison.OrdinalIgnoreCase))
					order = OrderContext.Current.GetPurchaseOrder(CustomerId, OrderGroupId);
				else if (this.ViewId.StartsWith(_ShoppingCartViewId, StringComparison.OrdinalIgnoreCase))
					order = OrderContext.Current.GetCart(CustomerId, OrderGroupId);
			}

			// if order is still null, create an empty cart
			if (order == null)
			{
				cart = CreateFreshCart();
				cartLoaded = true;
				Session[_OrderGroupDtoSessionKey] = cart;
			}
            
            // Put a dictionary key that can be used by other tabs
            IDictionary dic = new ListDictionary();
			if(cartLoaded)
				dic.Add(_OrderContextObjectString, cart);
			else
				dic.Add(_OrderContextObjectString, order);

            // Call tabs save
            OrderGroupEdit.SaveChanges(dic);
            ViewControl.SaveChanges(dic);

			Cart cartFromSession = order as Cart;

			if (cartFromSession != null) // if we're in create mode, save cart as order (payment plan, cart)
			{
				if (this.ViewId.StartsWith(_PaymentPlanViewId, StringComparison.OrdinalIgnoreCase))
					cartFromSession.SaveAsPaymentPlan();
				else if (this.ViewId.StartsWith(_PurchaseOrderViewId, StringComparison.OrdinalIgnoreCase))
					cartFromSession.SaveAsPurchaseOrder();
				else if (this.ViewId.StartsWith(_ShoppingCartViewId, StringComparison.OrdinalIgnoreCase))
					cartFromSession.AcceptChanges();
			}
			else
			{
				// Persist changes to the database
				order.AcceptChanges();
			}

			Session.Remove(_OrderGroupDtoSessionKey);
        }
    }
}
