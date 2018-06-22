using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using ComponentArt.Web.UI;
using System.Linq;
using System.Linq.Expressions;
using System.Data;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class OrderShipmentEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";
		private const string _PostBackArgumentString = "shipmentChanged";

		OrderGroup _order = null;
		Shipment _shipment = null;

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			Page.LoadComplete += new EventHandler(Page_LoadComplete);
            base.OnInit(e);

            MetaDataTab.MDContext = OrderContext.MetaDataContext;
        }

        int _ShipmentId = 0;
        /// <summary>
        /// Gets or sets the shipment id.
        /// </summary>
        /// <value>The shipment id.</value>
        public int ShipmentId
        {
            get
            {
                if (_ShipmentId == 0)
                {
                    if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
                    {
                        _ShipmentId = Int32.Parse(this.DialogTrigger.Value);
                    }
                }

                return _ShipmentId;
            }
            set
            {
                _ShipmentId = value;
            }
        }

		private Shipment SelectedShipment
		{
			get
			{
				if (_shipment == null)
				{
					if (_order != null && ShipmentId != 0)
					{
						ShipmentCollection shipments = _order.OrderForms[0].Shipments;

						foreach (Shipment shipment in shipments)
						{
							if (shipment.ShipmentId == ShipmentId)
							{
								_shipment = shipment;
								break;
							}
						}
					}
				}

				return _shipment;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			AddressList.Attributes["onchange"] =
				String.Format("javascript:OrderShipment_UpdateSelectedField($get('{0}'), $get('{1}'));",
				AddressList.ClientID, SelectedAddressField.ClientID);

			ShippingMethodList.Attributes["onchange"] =
				String.Format("javascript:OrderShipment_UpdateSelectedField($get('{0}'), $get('{1}'));",
				ShippingMethodList.ClientID, SelectedShippingMethodField.ClientID);

			bool reset = false;
			if (Request.Form["__EVENTTARGET"] == DialogTrigger.UniqueID)
				reset = true;
			BindForm(reset);
			//BindMetaForm();

			DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
        }

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (String.Compare(Request.Form["__EVENTTARGET"], SaveChangesButton.UniqueID, StringComparison.Ordinal) == 0)
				SaveChangesButton_Click(SaveChangesButton, null);
		}

        /// <summary>
        /// Handles the ValueChanged event of the DialogTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DialogTrigger_ValueChanged(object sender, EventArgs e)
        {
            BindForm(true);
        }

        /// <summary>
        /// Handles the Click event of the SaveChangesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveChangesButton_Click(object sender, EventArgs e)
        {
			if (String.Compare(Request.Form["__EVENTARGUMENT"], _PostBackArgumentString) == 0)
			{
				if (_order != null)
				{
					if (SelectedShipment == null)
						_shipment = _order.OrderForms[0].Shipments.AddNew();

					decimal parsedDecimal = 0;
					ListItem selectedSMItem = ShippingMethodList.Items.FindByValue(SelectedShippingMethodField.Value);
					if (selectedSMItem != null)
						SelectedShipment.ShippingMethodId = new Guid(selectedSMItem.Value);
					else
						SelectedShipment.ShippingMethodId = Guid.NewGuid();
					SelectedShipment.ShippingMethodName = MethodName.Text;
					ListItem selectedAddressItem = AddressList.Items.FindByText(SelectedAddressField.Value);
					if (selectedAddressItem != null)
						SelectedShipment.ShippingAddressId = selectedAddressItem.Text;
					else
						SelectedShipment.ShippingAddressId = AddressList.SelectedItem.Text;
					SelectedShipment.ShipmentTrackingNumber = TrackingNumber.Text;
					if (decimal.TryParse(ShipmentTotal.Text, out parsedDecimal))
						SelectedShipment.ShipmentTotal = parsedDecimal;
					if (decimal.TryParse(DiscountAmount.Text, out parsedDecimal))
						SelectedShipment.ShippingDiscountAmount = parsedDecimal;
					SelectedShipment.Status = Status.Text;

					// update lineItems
					foreach (DataListItem dlItem in ShipmentItemsList.Items)
					{
						CheckBox chb = dlItem.FindControl("chbIsInShipment") as CheckBox;
						if (chb != null)
						{
							bool itemRemoved = false;
							string lineItemIndex = ShipmentItemsList.DataKeys[dlItem.ItemIndex].ToString();
							if (chb.Checked && !SelectedShipment.LineItemIndexes.Contains(lineItemIndex))
							{
								// add lineItem
								SelectedShipment.LineItemIndexes.Add(lineItemIndex);
							}
							else if (!chb.Checked && SelectedShipment.LineItemIndexes.Contains(lineItemIndex))
							{
								// remove lineItem
								SelectedShipment.LineItemIndexes.Remove(lineItemIndex);
								itemRemoved = true;
							}

							if (itemRemoved)
								UpdateLineItemShippingProperties(lineItemIndex, String.Empty, Guid.Empty, String.Empty);
							else if (chb.Checked)
							{
								// if item was added or already existed, update it
								UpdateLineItemShippingProperties(lineItemIndex, SelectedShipment.ShippingAddressId, SelectedShipment.ShippingMethodId, SelectedShipment.ShippingMethodName);
							}
						}
					}

					// Put a dictionary key that can be used by other tabs
					IDictionary dic = new ListDictionary();
					dic.Add(MetaDataTab._MetaObjectContextKey + MetaDataTab.Languages[0], SelectedShipment);
					MetaDataTab.SaveChanges(dic);

					ScriptManager.RegisterStartupScript(MetaDataTab, typeof(OrderShipmentEditTab), "DialogClose", "Shipment_CloseDialog();", true);
				}
			}
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        private void BindForm(bool reset)
        {
			ShippingMethodList.DataSource = ShippingManager.GetShippingMethods(null);
			ShippingMethodList.DataBind();

			if (_order != null)
			{
				AddressList.DataSource = _order.OrderAddresses;
				AddressList.DataBind();

				if (String.IsNullOrEmpty(SelectedAddressField.Value))
					SelectedAddressField.Value = AddressList.SelectedValue;

				if (String.IsNullOrEmpty(SelectedShippingMethodField.Value))
					SelectedShippingMethodField.Value = ShippingMethodList.SelectedValue;

				if (SelectedShipment != null)
				{
					String formatString = "#0.00";

					if (reset)
					{
						ManagementHelper.SelectListItem(ShippingMethodList, SelectedShipment.ShippingMethodId);
						ManagementHelper.SelectListItem(AddressList, SelectedShipment.ShippingAddressId);

						SelectedAddressField.Value = AddressList.SelectedValue;
						SelectedShippingMethodField.Value = ShippingMethodList.SelectedValue;

						MethodName.Text = SelectedShipment.ShippingMethodName;

						TrackingNumber.Text = SelectedShipment.ShipmentTrackingNumber;
						ShipmentTotal.Text = SelectedShipment.ShipmentTotal.ToString(formatString);
						DiscountAmount.Text = SelectedShipment.ShippingDiscountAmount.ToString(formatString);
						Status.Text = SelectedShipment.Status;

						//lblItemsToShip.Text = Shipment.GetShipmentLineItemsString(SelectedShipment, 3, "<br />");
						LoadShipmentLineItems();
					}

					MetaDataTab.ObjectId = SelectedShipment.ShipmentId;
				}
				else if (reset)
				{
					MethodName.Text = String.Empty;

					TrackingNumber.Text = String.Empty;
					ShipmentTotal.Text = "0";
					DiscountAmount.Text = "0";
					Status.Text = String.Empty;
					LoadShipmentLineItems();
				}

				// Bind Meta classes
				// Bind Meta Form
				BindMetaForm();
			}
        }

		private void BindMetaForm()
		{
			// Bind Meta Form
			MetaDataTab.MetaClassId = OrderContext.Current.ShipmentMetaClass.Id;

			Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();
			metaObjects.Add(MetaDataTab.Languages[0], SelectedShipment);

			IDictionary dic = new ListDictionary();
			dic.Add("MetaObjectsContext", metaObjects);

			MetaDataTab.LoadContext(dic);

			MetaDataTab.DataBind();
		}

		/// <summary>
		/// Loads the shipment LineItems.
		/// </summary>
		/// <param name="iStartIndex">Start index of the i.</param>
		/// <param name="iNumItems">The i num items.</param>
		/// <param name="sFilter">The s filter.</param>
		private void LoadShipmentLineItems()
		{
			string indexColumnName = "LineItemIndex";
			string idColumnName = "LineItemId";
			string checkedColumnName = "Checked";
			string nameColumnName = "Name";

			DataTable lineItemsDT = new DataTable();
			lineItemsDT.Columns.AddRange(new DataColumn[4]{
					new DataColumn(indexColumnName, typeof(int)),
					new DataColumn(idColumnName, typeof(int)),
					new DataColumn(checkedColumnName, typeof(bool)),
					new DataColumn(nameColumnName, typeof(string))
					});

			// get items that are not associated with any shipment yet
			ShipmentCollection allShipments = _order.OrderForms[0].Shipments;
			LineItemCollection orderLineItems = _order.OrderForms[0].LineItems;
			int index = -1;
			foreach (LineItem li in orderLineItems)
			{
				index++;
				bool isInShipment = false;
				bool isInCurrentShipment = false;

				if (SelectedShipment != null && SelectedShipment.LineItemIndexes.Contains(index.ToString()))
				{
					// if it is current shipment, add lineItem to the grid
					isInCurrentShipment = true;
				}
				else
				{
					foreach (Shipment shipment in allShipments)
					{
						if (shipment != SelectedShipment && shipment.LineItemIndexes.Contains(index.ToString()))
						{
							isInShipment = true;
							break;
						}
					}
				}

				if (!isInShipment || isInCurrentShipment)
				{
					// if it is current shipment, add lineItem to the grid
					DataRow row = lineItemsDT.NewRow();
					row[indexColumnName] = index;
					row[idColumnName] = li.LineItemId;
					row[checkedColumnName] = isInCurrentShipment;
					row[nameColumnName] = li.DisplayName;
					lineItemsDT.Rows.Add(row);
				}
			}

			ShipmentItemsList.DataSource = lineItemsDT.DefaultView;
			ShipmentItemsList.DataBind();
		}

		private void UpdateLineItemShippingProperties(string lineItemId, string shippingAddressId, Guid shippingMethodId, string shippingMethodName)
		{
			if (_order != null)
			{
				LineItem li = _order.OrderForms[0].LineItems[Int32.Parse(lineItemId)];
				if (li != null)
				{
					li.ShippingAddressId = shippingAddressId;
					li.ShippingMethodId = shippingMethodId;
					li.ShippingMethodName = shippingMethodName;
				}
			}
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            //OrderGroup order = (OrderGroup)context[_OrderContextObjectString];
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