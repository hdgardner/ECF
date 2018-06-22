using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Orders.Dto;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	public partial class LineItemEditPopup : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _OrderContextObjectString = "OrderGroup";
		private const string _PostBackArgumentString = "lineItemChanged";
		
        OrderGroup _order = null;

		int _LineItemId = 0;
		string _CatalogEntryId = null; // needed when new LineItem is created
		CatalogEntryDto _CatalogEntryDto = null;
		LineItem _selectedLineItem = null;

        /// <summary>
        /// Gets or sets the line item id.
        /// </summary>
        /// <value>The line item id.</value>
		public int LineItemId
		{
			get
			{
				if (_LineItemId == 0)
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
					{
						int index = this.DialogTrigger.Value.IndexOf('|');
						string idString = this.DialogTrigger.Value;

						if (index > 0)
							// cut off CatalogEntryId
							idString = this.DialogTrigger.Value.Substring(0, index);

						_LineItemId = Int32.Parse(idString);
					}
				}

				return _LineItemId;
			}
			set
			{
				_LineItemId = value;
			}
		}

		/// <summary>
		/// Gets the line item.
		/// </summary>
		private LineItem SelectedLineItem
		{
			get
			{
				if (_selectedLineItem == null)
				{
					if (_order != null && LineItemId != 0)
						_selectedLineItem = _order.OrderForms[0].LineItems.FindItem(LineItemId);
				}

				return _selectedLineItem;
			}
		}

        /// <summary>
        /// Gets or sets the catalog entry code.
        /// </summary>
        /// <value>The catalog entry code.</value>
		public string CatalogEntryCode
		{
			get
			{
				if (String.IsNullOrEmpty(_CatalogEntryId))
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
					{
						int index = this.DialogTrigger.Value.IndexOf('|');
						if (index > 0 && index < this.DialogTrigger.Value.Length - 1)
							_CatalogEntryId = this.DialogTrigger.Value.Substring(index + 1, this.DialogTrigger.Value.Length - index - 1);
					}
				}

				return _CatalogEntryId;
			}
			set
			{
				_CatalogEntryId = value;
			}
		}

        /// <summary>
        /// Gets the catalog entry dto.
        /// </summary>
        /// <returns></returns>
		public CatalogEntryDto GetCatalogEntryDto()
		{
            if (_CatalogEntryDto == null && !String.IsNullOrEmpty(CatalogEntryCode))
			{
				_CatalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(CatalogEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

				if (_CatalogEntryDto.CatalogEntry.Count == 0)
					_CatalogEntryDto = null;
			}
			return _CatalogEntryDto;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			string postback = Page.ClientScript.GetPostBackEventReference(SaveChangesButton, _PostBackArgumentString);

			string scriptString = "if (Page_ClientValidate('" + SaveChangesButton.ValidationGroup + "')) {\r\n" +
				"this.disabled = true;\r\n" +
				postback + ";\r\n" +
				"return false;}\r\n";

			SaveChangesButton.OnClientClick = scriptString; //OrderLineItemSaveChangesButton_onClientClick(this);

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
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			Page.LoadComplete += new EventHandler(Page_LoadComplete);
			base.OnInit(e);

            MetaDataTab.MDContext = OrderContext.MetaDataContext;
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
				LineItem lineItem = null;

				if (LineItemId != 0) // find existing
					lineItem = _order.OrderForms[0].LineItems.FindItem(LineItemId);

				// if not found, create new
				if (lineItem == null)
				{
					lineItem = _order.OrderForms[0].LineItems.AddNew();
					lineItem.CatalogEntryId = CatalogEntryIdLabel.Text;
				}

				// update lineItem fields
				if (lineItem != null)
				{
					decimal price = Decimal.Parse(ListPrice.Text);
					lineItem.DisplayName = DisplayName.Text;
					lineItem.ListPrice = price;
					lineItem.PlacedPrice = price;
					lineItem.LineItemDiscountAmount = Decimal.Parse(DiscountAmount.Text);
					lineItem.Quantity = Decimal.Parse(Quantity.Text);
					decimal extPrice = lineItem.Quantity * price;
					lineItem.ExtendedPrice = extPrice > lineItem.LineItemDiscountAmount ? extPrice - lineItem.LineItemDiscountAmount : extPrice;
					//ListItem selectedAddressItem = LineItemAddressesFilter.Items.FindByValue(SelectedAddressField.Value);
					//if (selectedAddressItem != null)
					//    lineItem.ShippingAddressId = selectedAddressItem.Value;

					//ListItem selectedSMItem = ShippingMethodList.Items.FindByValue(SelectedShippingMethodField.Value);
					//if (selectedSMItem != null)
					//    lineItem.ShippingMethodId = new Guid(selectedSMItem.Value);
				}

				//lineItem.AcceptChanges();

				// Put a dictionary key that can be used by other tabs
				MetaDataTab.ObjectId = lineItem.LineItemId;
				IDictionary dic = new ListDictionary();
				dic.Add(MetaDataTab._MetaObjectContextKey + MetaDataTab.Languages[0], lineItem);
				MetaDataTab.SaveChanges(dic);
			}

			ScriptManager.RegisterStartupScript(MetaDataTab, typeof(LineItemEditPopup), "DialogClose", "LineItemAddresses_CloseDialog();", true);
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
		private void BindForm(bool reset)
		{
			if (SelectedLineItem != null)
			{
				if (reset)
				{
					string shippingMethod = String.Empty;
					ShippingMethodDto smDto = ShippingManager.GetShippingMethod(SelectedLineItem.ShippingMethodId);
					if (smDto != null && smDto.ShippingMethod.Count > 0)
					{
						shippingMethod = smDto.ShippingMethod[0].DisplayName;
						if (!String.IsNullOrEmpty(SelectedLineItem.ShippingMethodName))
							shippingMethod += String.Format(" ({0})", SelectedLineItem.ShippingMethodName);
					}

					SetFormFieldsValues(SelectedLineItem.CatalogEntryId,
							SelectedLineItem.DisplayName,
							SelectedLineItem.ListPrice,
							SelectedLineItem.LineItemDiscountAmount,
							SelectedLineItem.Quantity,
							SelectedLineItem.ShippingAddressId,
							shippingMethod);
				}

				MetaDataTab.ObjectId = SelectedLineItem.LineItemId;
			}
			else if (reset)
			{
				CatalogEntryDto dto = GetCatalogEntryDto();
				if (dto != null)
				{
					SetFormFieldsValues(CatalogEntryCode,
						dto.CatalogEntry[0].Name,
						dto.Variation.Count > 0 ? dto.Variation[0].ListPrice : 0m,
						0m,
						1,
						String.Empty,
						String.Empty);
				}
				else
					SetFormFieldsValues(String.Empty, String.Empty, 0m, 0m, 1m, String.Empty, String.Empty);
			}

			// Bind Meta Form
			BindMetaForm();
		}

		private void BindMetaForm()
		{
			// Bind Meta Form
			MetaDataTab.MetaClassId = OrderContext.Current.LineItemMetaClass.Id;

			Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();
			metaObjects.Add(MetaDataTab.Languages[0], SelectedLineItem);

			IDictionary dic = new ListDictionary();
			dic.Add("MetaObjectsContext", metaObjects);

			MetaDataTab.LoadContext(dic);

			MetaDataTab.DataBind();
		}

        /// <summary>
        /// Sets the form fields values.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="listPrice">The list price.</param>
        /// <param name="discount">The discount.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="selectedAddressId">The selected address id.</param>
		private void SetFormFieldsValues(string catalogEntryId, string displayName, decimal listPrice, decimal discount, decimal quantity, string shippingAddress, string shippingMethod)
		{
			string formatString = "#0.00";

			CatalogEntryIdLabel.Text = catalogEntryId;
			DisplayName.Text = displayName;
			ListPrice.Text = listPrice.ToString(formatString);
			DiscountAmount.Text = discount.ToString(formatString);
			Quantity.Text = quantity.ToString(formatString);
			lblShippingAddress.Text = shippingAddress;
			lblShippingMethod.Text = shippingMethod;
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