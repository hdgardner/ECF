using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using ComponentArt.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Catalog.Search;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class LineItemsEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _LineItemIdString = "LineItemId";
		private const string _OrderContextObjectString = "OrderGroup";

        List<GridItem> _removedItems = new List<GridItem>();
        OrderGroup _order = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !LineItemsFilter.CausedCallback)
            {
                LoadItems(0, LineItemsFilter.DropDownPageSize * 2, "");
                BindForm();
            }

			LineItemsGrid.NeedRebind += new Grid.NeedRebindEventHandler(LineItemsGrid_NeedRebind);
			LineItemsGrid.NeedDataSource += new Grid.NeedDataSourceEventHandler(LineItemsGrid_NeedDataSource);
			LineItemsGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(LineItemsGrid_DeleteCommand);
			LineItemsGrid.PreRender += new EventHandler(LineItemsGrid_PreRender);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            LineItemsFilter.DataRequested += new ComboBox.DataRequestedEventHandler(LineItemsFilter_DataRequested);

            base.OnInit(e);
        }

        /// <summary>
        /// Loads the items.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
		private void LoadItems(int iStartIndex, int iNumItems, string sFilter)
		{
            LineItemsFilter.Items.Clear();

            // Changed to return all entries here
            string entryType = String.Empty; //Mediachase.Commerce.Catalog.Objects.EntryType.Product.ToString();

            CatalogSearchParameters pars = new CatalogSearchParameters();
            CatalogSearchOptions options = new CatalogSearchOptions();

            options.RecordsToRetrieve = iNumItems;
            options.Namespace = "Mediachase.Commerce.Catalog";
            options.StartingRecord = iStartIndex;
            options.ReturnTotalCount = true;
            pars.SqlWhereClause = String.Format("[CatalogEntry].Name like '%{0}%' OR [CatalogEntry].Code like '%{0}%'", sFilter);

            // Add catalogs
            CatalogDto catalogDto = CatalogContext.Current.GetCatalogDto();

            foreach (CatalogDto.CatalogRow catalogRow in catalogDto.Catalog)
            {
                pars.CatalogNames.Add(catalogRow.Name);
            }

            int totalRecords = 0;
            CatalogEntryDto dto = CatalogContext.Current.FindItemsDto(pars, options, ref totalRecords);
            foreach (CatalogEntryDto.CatalogEntryRow entryRow in dto.CatalogEntry)
            {
                ComboBoxItem item = new ComboBoxItem(entryRow.Name + " [" + entryRow.Code.ToString() + "]");
                item.Value = entryRow.Code.ToString();
                item["icon"] = Page.ResolveClientUrl(String.Format("~/app_themes/Default/images/icons/{0}.gif", entryRow.ClassTypeId));
                LineItemsFilter.Items.Add(item);
            }

            LineItemsFilter.ItemCount = totalRecords;
		}

        /// <summary>
        /// Handles the DataRequested event of the LineItemsFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
		void LineItemsFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
		{
			LoadItems(args.StartIndex, args.NumItems, args.Filter);
		}

		#region LineItemsGrid event handlers
        /// <summary>
        /// Handles the NeedDataSource event of the LineItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void LineItemsGrid_NeedDataSource(object sender, EventArgs e)
		{
			LineItemsGrid.DataSource = GetLineItemsDataSource();
		}

        /// <summary>
        /// Handles the NeedRebind event of the LineItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void LineItemsGrid_NeedRebind(object sender, EventArgs e)
		{
			LineItemsGrid.DataBind();
		}

        /// <summary>
        /// Handles the PreRender event of the LineItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void LineItemsGrid_PreRender(object sender, EventArgs e)
        {
            // Postback happens so the grid will be completely updated, make sure to save all the changes
            if(this.IsPostBack)
				ProcessTableEvents(_order);
        }

        /// <summary>
        /// Handles the DeleteCommand event of the LineItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void LineItemsGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
			int id = 0;
			if (e.Item[_LineItemIdString] != null && Int32.TryParse(e.Item[_LineItemIdString].ToString(), out id))
			{
				// find the existing one
				foreach (LineItem litem in GetLineItems(_order))
					if (litem.LineItemId == id)
					{
						litem.Delete();

						// remove this lineItem from all shipments
						foreach (Shipment shipment in _order.OrderForms[0].Shipments)
						{
							if (shipment.LineItemIndexes.Contains(id.ToString()))
								shipment.LineItemIndexes.Remove(id.ToString());
						}
					}
			}
			//_removedItems.Add(e.Item);
        }
		#endregion

        /// <summary>
        /// Gets the line items.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public IEnumerable<LineItem> GetLineItems(OrderGroup order)
        {
            foreach (OrderForm orderForm in order.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    yield return lineItem;
                }
            }
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(LineItemsGrid, "Order", "LineItems");
			BindLineItemsGrid();
        }

        /// <summary>
        /// Binds the line items grid.
        /// </summary>
		private void BindLineItemsGrid()
		{
			object dataSource = GetLineItemsDataSource();
			if (dataSource != null)
			{
				LineItemsGrid.DataSource = dataSource;
				LineItemsGrid.DataBind();
			}
		}

        /// <summary>
        /// Gets the line items data source.
        /// </summary>
        /// <returns></returns>
		private object GetLineItemsDataSource()
		{
			if (_order != null && _order.OrderForms.Count > 0 && _order.OrderForms[0].LineItems.Count > 0)
			{
				List<LineItem> list = new List<LineItem>();
				foreach (LineItem li in GetLineItems(_order))
					list.Add(li);

				return list;
			}

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
            //ProcessTableEvents(order);
        }

        /// <summary>
        /// Processes the table events.
        /// </summary>
        /// <param name="po">The po.</param>
        private void ProcessTableEvents(OrderGroup order)
        {
			//foreach (GridItem item in _removedItems)
			//{
			//    int id = 0;
			//    if (item[_LineItemIdString] != null && Int32.TryParse(item[_LineItemIdString].ToString(), out id))
			//    {
			//        // find the existing one
			//        foreach (LineItem litem in GetLineItems(order))
			//            if (litem.LineItemId == id)
			//                litem.Delete();
			//    }
			//}

			//_removedItems.Clear();
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
			LineItemAddressesDialog.LoadContext(context);
        }
        #endregion
    }
}