using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
    public partial class PromotionCodeEditTab : MarketingBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _PromotionDtoString = "PromotionDto";
		private const string _CatalogNodeIdString = "CatalogNodeId";
		private const string _CatalogNodeNameString = "CatalogNodeName";
		private const string _ExpressionIdString = "ExpressionId";
		private const string _ExpressionNameString = "ExpressionName";
		private const string _CatalogEntryIdString = "CatalogEntryId";
		private const string _CatalogEntryNameString = "CatalogEntryName";
		private const string _CatalogNameString = "CatalogName";

        List<GridItem> _addedItems = new List<GridItem>();
        List<GridItem> _removedItems = new List<GridItem>();

        PromotionDto _promotion = null;

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !NodeFilter.CausedCallback && !EntryFilter.CausedCallback)
            {
                LoadCatalogs(0, CatalogFilter.DropDownPageSize * 2, "");
                LoadExpressions(0, ExpressionFilter.DropDownPageSize * 2, "");
                BindForm();
            }
        }

        /// <summary>
        /// Loads the catalogs.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadCatalogs(int iStartIndex, int iNumItems, string sFilter)
        {
            CatalogDto dto = CatalogContext.Current.GetCatalogDto();

            CatalogFilter.Items.Clear();
            CatalogFilter.DataSource = dto;
            CatalogFilter.DataBind();
        }

        /// <summary>
        /// Loads the expressions.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadExpressions(int iStartIndex, int iNumItems, string sFilter)
        {
			ExpressionDto dto = ExpressionManager.GetExpressionDto(ExpressionCategory.GetExpressionCategory(ExpressionCategory.CategoryKey.Promotion).Key);

            ExpressionFilter.DataSource = dto;
            ExpressionFilter.DataBind();
        }

        /// <summary>
        /// Loads the catalog nodes.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadCatalogNodes(int iStartIndex, int iNumItems, string sFilter)
        {
            NodeFilter.Items.Clear();
            if (sFilter.Length > 0)
            {
                CatalogNodeDto dto = CatalogContext.Current.GetCatalogNodesDto(sFilter);

                for (int i = 0; i < dto.CatalogNode.Rows.Count; i++)
                {
                    DataRow oRow = (DataRow)dto.CatalogNode.Rows[i];
                    ComboBoxItem item = new ComboBoxItem(oRow["Name"].ToString());
                    item.Value = oRow["Code"].ToString() + "$" + sFilter;
                    NodeFilter.Items.Add(item);
                }
            }

            NodeFilter.ItemCount = NodeFilter.Items.Count;
        }

        /// <summary>
        /// Loads the catalog entries.
        /// </summary>
        /// <param name="iStartIndex">Start index of the i.</param>
        /// <param name="iNumItems">The i num items.</param>
        /// <param name="sFilter">The s filter.</param>
        private void LoadCatalogEntries(int iStartIndex, int iNumItems, string sFilter)
        {
            int index = sFilter.IndexOf("$");

            CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntriesByNodeDto(sFilter.Substring(index+1), sFilter.Substring(0, index));

            EntryFilter.Items.Clear();

            foreach (CatalogEntryDto.CatalogEntryRow row in dto.CatalogEntry)
            {
                ComboBoxItem item = new ComboBoxItem(row.Name);
                item.Value = row.Code;
                EntryFilter.Items.Add(item);
            }

            EntryFilter.ItemCount = dto.CatalogEntry.Count;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            DefaultGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_DeleteCommand);
            DefaultGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_InsertCommand);
            DefaultGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_InsertCommand);
            DefaultGrid.PreRender += new EventHandler(DefaultGrid_PreRender);
            NodeFilter.DataRequested += new ComboBox.DataRequestedEventHandler(NodeFilter_DataRequested);
            EntryFilter.DataRequested += new ComboBox.DataRequestedEventHandler(EntryFilter_DataRequested);

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the DataRequested event of the EntryFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void EntryFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadCatalogEntries(args.StartIndex, args.NumItems, args.Filter);
        }

        /// <summary>
        /// Handles the DataRequested event of the NodeFilter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="ComponentArt.Web.UI.ComboBoxDataRequestedEventArgs"/> instance containing the event data.</param>
        void NodeFilter_DataRequested(object sender, ComboBoxDataRequestedEventArgs args)
        {
            LoadCatalogNodes(args.StartIndex, args.NumItems, args.Filter);
        }

        /// <summary>
        /// Handles the PreRender event of the DefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DefaultGrid_PreRender(object sender, EventArgs e)
        {
            // Postback happens so the grid will be completely updated, make sure to save all the changes
            if(this.IsPostBack)
                ProcessTableEvents(_promotion);
        }

        /// <summary>
        /// Handles the DeleteCommand event of the DefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void DefaultGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            foreach (GridItem item in _addedItems)
            {
                if (_addedItems.Contains(item))
                    _addedItems.Remove(item);
            }

            _removedItems.Add(e.Item);
        }

        /// <summary>
        /// Handles the InsertCommand event of the DefaultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
        void DefaultGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            foreach (GridItem item in _removedItems)
            {
                if (_removedItems.Contains(item))
                    _removedItems.Remove(item);
            }

            _addedItems.Add(e.Item);
        }

        /*
        public IEnumerable<LineItem> GetLineItems(PurchaseOrder po)
        {
            foreach (OrderForm orderForm in po.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    yield return lineItem;
                }
            }
        }
         * */

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(DefaultGrid, "Marketing", "PromotionCond-List");

            if (_promotion != null)
            {
                SecurityManager.CheckRolePermission("marketing:expr:mng:edit");

                DataTable table = _promotion.PromotionCondition.Copy();

                table.Columns.Add(new DataColumn(_ExpressionNameString));
                table.Columns.Add(new DataColumn(_CatalogNodeNameString));
                table.Columns.Add(new DataColumn(_CatalogEntryNameString));

                foreach (DataRow row in table.Rows)
                {
                    if (row[_ExpressionIdString] != DBNull.Value && !String.IsNullOrEmpty(row[_ExpressionIdString].ToString()))
                        row[_ExpressionNameString] = GetExpressionName(Int32.Parse(row[_ExpressionIdString].ToString()));
                    if (row[_CatalogNodeIdString] != DBNull.Value && !String.IsNullOrEmpty(row[_CatalogNodeIdString].ToString()))
                        row[_CatalogNodeNameString] = GetCatalogNodeName(row[_CatalogNodeIdString].ToString());
                    if (row[_CatalogEntryIdString] != DBNull.Value && !String.IsNullOrEmpty(row[_CatalogEntryIdString].ToString()))
                        row[_CatalogEntryNameString] = GetCatalogEntryName(row[_CatalogEntryIdString].ToString());
                }

                DefaultGrid.DataSource = table;
                DefaultGrid.DataBind();
                /*
                DataTable tbl = _promotion.PromotionCondition.Copy();
                DataColumn column = tbl.Columns.Add();
                column.ColumnName = _ExpressionNameString;
                 * */
                /*
                ArrayList list = new ArrayList();
                foreach(LineItem li in GetLineItems(_po))
                {
                    list.Add(li);
                }
                DefaultGrid.DataSource = list;
                DefaultGrid.DataBind();
                 * */
            }
            else
                SecurityManager.CheckRolePermission("marketing:expr:mng:create");
        }

        /// <summary>
        /// Gets the name of the expression.
        /// </summary>
        /// <param name="expressionId">The expression id.</param>
        /// <returns></returns>
        protected string GetExpressionName(int expressionId)
        {
            ExpressionDto expr = ExpressionManager.GetExpressionDto(expressionId);
            return expr.Expression[0].Name;
        }

        /// <summary>
        /// Gets the name of the catalog node.
        /// </summary>
        /// <param name="catalogNodeId">The catalog node id.</param>
        /// <returns></returns>
        protected string GetCatalogNodeName(object catalogNodeId)
        {
            if (catalogNodeId == null)
                return String.Empty;

            CatalogNodeDto node = CatalogContext.Current.GetCatalogNodeDto(catalogNodeId.ToString());

            if (node.CatalogNode.Count > 0)
                return node.CatalogNode[0].Name;
            else
                return String.Empty;
        }

        /// <summary>
        /// Gets the name of the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        protected string GetCatalogEntryName(object catalogEntryId)
        {
            if (catalogEntryId == null)
                return String.Empty;

            CatalogEntryDto entry = CatalogContext.Current.GetCatalogEntryDto(catalogEntryId.ToString());

            if (entry.CatalogEntry.Count > 0)
                return entry.CatalogEntry[0].Name;
            else
                return String.Empty;
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
            PromotionDto promo = (PromotionDto)context[_PromotionDtoString];
            ProcessTableEvents(promo);
        }

        /// <summary>
        /// Processes the table events.
        /// </summary>
        /// <param name="promo">The promo.</param>
        private void ProcessTableEvents(PromotionDto promo)
        {
            foreach (GridItem item in _addedItems)
            {
                int expressionId = item[_ExpressionIdString] == null ? 0 : Int32.Parse(item[_ExpressionIdString].ToString());
                /*
                int nodeId = item[_CatalogNodeIdString] == null ? 0 : Int32.Parse(item[_CatalogNodeIdString].ToString());
                int entryId = item[_CatalogEntryIdString] == null ? 0 : Int32.Parse(item[_CatalogEntryIdString].ToString());
                 * */
                string nodeId = item[_CatalogNodeIdString] == null ? String.Empty : item[_CatalogNodeIdString].ToString();
                string entryId = item[_CatalogEntryIdString] == null ? String.Empty : item[_CatalogEntryIdString].ToString();

                string catalogName = item[_CatalogNameString] == null ? String.Empty : item[_CatalogNameString].ToString();


                PromotionDto.PromotionConditionRow row = promo.PromotionCondition.NewPromotionConditionRow();

                row.CatalogEntryId = entryId.ToString();
                row.CatalogName = catalogName;
                row.CatalogNodeId = nodeId.ToString();
                row.ExpressionId = expressionId;
                row.PromotionId = promo.Promotion[0].PromotionId;
                promo.PromotionCondition.Rows.Add(row);
            }

            _addedItems.Clear();

            foreach (GridItem item in _removedItems)
            {
                int id = Int32.Parse(item["PromotionConditionId"].ToString());
                // find the existing one
                foreach (PromotionDto.PromotionConditionRow row in promo.PromotionCondition.Rows)
                {
                    if (row.PromotionConditionId == id)
                        row.Delete();
                }
            }

            _removedItems.Clear();
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _promotion = (PromotionDto)context[_PromotionDtoString];
        }

        #endregion
    }
}