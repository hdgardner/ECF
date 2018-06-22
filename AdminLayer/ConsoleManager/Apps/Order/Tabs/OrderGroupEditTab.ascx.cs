using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Core.Managers;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class OrderGroupEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";

        public OrderGroup _order = null;

		#region Public properties
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
		#endregion

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !CustomerName.CausedCallback)
            {
                LoadMemberItems(0, CustomerName.DropDownPageSize * 2, "");
                BindForm();
            }

            
            BindMetaForm();

			OrderGroupTrigger.ValueChanged += new EventHandler(OrderGroupTrigger_ValueChanged);
        }

		void OrderGroupTrigger_ValueChanged(object sender, EventArgs e)
		{
			LoadAddresses();

			if (_order != null && _order.OrderForms.Count > 0)
			{
				if (AddressesList.Items.Count > 0)
					ManagementHelper.SelectListItem(AddressesList, _order.OrderForms[0].BillingAddressId);
			}
		}


        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            /*
            WorkflowList.DataSource = WorkflowConfiguration.Instance.Workflows;
            WorkflowList.DataBind();
             * */

            // Bind Status
            OrderStatusList.DataSource = OrderStatusManager.GetOrderStatus();
            OrderStatusList.DataBind();

            // Bind Currency
            OrderCurrencyList.DataSource = CatalogContext.Current.GetCurrencyDto();
            OrderCurrencyList.DataBind();

            OrderGroup order = _order;

			if (order != null)
			{
				LoadAddresses();

				ComboBoxItem item = CustomerName.Items.FindByValue(order.CustomerId.ToString());
				if (item != null)
				{
					CustomerName.SelectedItem = item;
				}
				else
				{
					ComboBoxItem newItem = new ComboBoxItem();
					newItem.Text = order.CustomerName;
					newItem.Value = order.CustomerId.ToString();
					CustomerName.Items.Add(newItem);
					CustomerName.SelectedItem = newItem;
				}

				CustomerName.Text = order.CustomerName;
				//TrackingNo.Text = order.TrackingNumber;

				OrderSubTotal.Text = CurrencyFormatter.FormatCurrency(order.SubTotal, order.BillingCurrency);
				OrderTaxTotal.Text = CurrencyFormatter.FormatCurrency(order.TaxTotal, order.BillingCurrency);
				OrderShippingTotal.Text = CurrencyFormatter.FormatCurrency(order.ShippingTotal, order.BillingCurrency);
				OrderHandlingTotal.Text = CurrencyFormatter.FormatCurrency(order.HandlingTotal, order.BillingCurrency);
				
				OrderTotal.Text = CurrencyFormatter.FormatCurrency(order.Total, order.BillingCurrency);
				//OrderExpires.Value = order.ExpirationDate;

                if (_order.OrderForms.Count > 0)
                {
					if (AddressesList.Items.Count > 0)
						ManagementHelper.SelectListItem(AddressesList, _order.OrderForms[0].BillingAddressId);
                    DiscountTotal.Text = CurrencyFormatter.FormatCurrency(order.OrderForms[0].DiscountAmount, order.BillingCurrency);
                }

				ManagementHelper.SelectListItem(OrderStatusList, order.Status);
				ManagementHelper.SelectListItem(OrderCurrencyList, order.BillingCurrency);

				/*
				if (po.Status == OrderConfiguration.Instance.NewOrderStatus)
				{
					WorkflowDisabledDescription.Visible = false;
					RunWorkflowButton.Enabled = true;
				}
				else
				{
					RunWorkflowButton.Enabled = false;
					WorkflowDisabledDescription.Visible = true;
					WorkflowDisabledDescription.Text = String.Format("Can only run workflow for orders with \"{0}\" status", OrderConfiguration.Instance.NewOrderStatus);
				}
				 * */
			}
			else
			{
				ManagementHelper.SelectListItem(OrderCurrencyList, CommonSettingsManager.GetDefaultCurrency());
			}
        }

        /// <summary>
        /// Binds the meta form.
        /// </summary>
        private void BindMetaForm()
        {
            string metaClassName = Request.QueryString["class"];
            MetaClass metaClass = null;

			if ((_order == null || _order.OrderGroupId <= 0) || String.IsNullOrEmpty(metaClassName))
			{
				// Call clear meta cache so we have the latest meta class when binding to an item
				OrderContext.Current.ClearMetaCache();

				if (_order is Cart)
					// if this is new order/cart/payment plan
					metaClass = OrderContext.Current.ShoppingCartMetaClass;
				else
				{
					// if we are in edit mode
					if (this.ViewId.StartsWith("PaymentPlan", StringComparison.OrdinalIgnoreCase))
					{
						metaClass = OrderContext.Current.PaymentPlanMetaClass;
					}
					else if (this.ViewId.StartsWith("PurchaseOrder", StringComparison.OrdinalIgnoreCase))
					{
						metaClass = OrderContext.Current.PurchaseOrderMetaClass;
					}
					else if (this.ViewId.StartsWith("ShoppingCart", StringComparison.OrdinalIgnoreCase))
					{
						metaClass = OrderContext.Current.ShoppingCartMetaClass;
					}
				}
			}
			else
			{
				metaClass = MetaClass.Load(OrderContext.MetaDataContext, metaClassName);
			}

            // Bind Meta Form
            MetaDataTab.MetaClassId = metaClass.Id;
            Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();
            metaObjects.Add(MetaDataTab.Languages[0], _order);
            IDictionary dic = new ListDictionary();
            dic.Add("MetaObjectsContext", metaObjects);
            MetaDataTab.LoadContext(dic);
            MetaDataTab.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            CustomerName.DataRequested += new ComponentArt.Web.UI.ComboBox.DataRequestedEventHandler(CustomerName_DataRequested);
            this.Page.LoadComplete += new EventHandler(Page_LoadComplete);
			base.OnInit(e);
            MetaDataTab.MDContext = OrderContext.MetaDataContext;
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Page_LoadComplete(object sender, EventArgs e)
        {
            // Refresh data
            if (_order != null)
            {
                if (IsPostBack && HttpContext.Current.Items.Contains("Order-" + _order.Id))
                {
                    BindForm();
                }
            }
        }

        /// <summary>
        /// Handles the DataRequested event of the CustomerName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void CustomerName_DataRequested(object sender, ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs args)
        {
            LoadMemberItems(args.StartIndex, args.NumItems, args.Filter);
        }

		/// <summary>
		/// Loads order addresses to the dropdown.
		/// </summary>
		private void LoadAddresses()
		{
			if (_order != null && _order.OrderAddresses.Count > 0)
			{
				AddressesList.DataSource = _order.OrderAddresses;
				AddressesList.DataBind();
			}
			else
			{
				AddressesList.Items.Clear();
				AddressesList.Items.Add(new ListItem(Mediachase.Ibn.Web.UI.WebControls.UtilHelper.GetResFileString("{SharedStrings:Order_Select_Billing_Address}"), ""));
			}
		}

        /// <summary>
        /// Loads the member items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadMemberItems(int iStartIndex, int iNumItems, string sFilter)
        {
            int total = 0;
            ProfileSearchParameters pars = new ProfileSearchParameters();
            ProfileSearchOptions options = new ProfileSearchOptions();
            options.Namespace = "Mediachase.Commerce.Profile";
            pars.FreeTextSearchPhrase = sFilter;
            options.Classes.Add("Account");

            Account[] accounts = ProfileContext.Current.FindAccounts(pars, options, out total);
            foreach (Account acc in accounts)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Text = acc.Name;
                newItem.Value = acc.PrincipalId.ToString();
                CustomerName.Items.Add(newItem);
            }

            //CustomerName.DataSource = ProfileContext.Current.FindAccounts(pars, options, out total);
            CustomerName.ItemCount = total + 1;
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            OrderGroup order = (OrderGroup)context[_OrderContextObjectString];
            order.Status = OrderStatusList.SelectedValue;
            order.BillingCurrency = OrderCurrencyList.SelectedValue;
            order.AddressId = AddressesList.SelectedValue;
			if (!String.IsNullOrEmpty(order.AddressId) && order.OrderForms.Count > 0)
				order.OrderForms[0].BillingAddressId = order.AddressId;

            /*
            if(typeof(order) == typeof(OrderGroup))
                order.TrackingNumber = TrackingNo.Text;

            order.ExpirationDate = OrderExpires.Value;
             * */

			if (CustomerName.SelectedItem != null)
			{
				order.CustomerId = new Guid(CustomerName.SelectedValue);

				// assign order.CustomerName
				OrderAddress address = null;
				if (!String.IsNullOrEmpty(order.AddressId))
				{

					if (order.OrderAddresses != null && order.OrderAddresses.Count > 0)
						foreach (OrderAddress addr in order.OrderAddresses)
							if (String.Compare(addr.Name, order.AddressId, true) == 0)
							{
								address = addr;
								break;
							}
				}
				if (address != null)
					order.CustomerName = address.FirstName + " " + address.LastName;
				else
					order.CustomerName = CustomerName.SelectedItem.Text;
			}

            // Save meta data
            MetaDataTab.ObjectId = order.OrderGroupId;
            IDictionary dic = new ListDictionary();
            dic.Add("MetaObjectContext-" + MetaDataTab.Languages[0], order);
            MetaDataTab.SaveChanges(dic);
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
        }
        #endregion
    }
}
