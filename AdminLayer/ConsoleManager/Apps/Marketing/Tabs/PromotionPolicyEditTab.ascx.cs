using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Marketing.Tabs
{
    public partial class PromotionPolicyEditTab : MarketingBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _PromotionDtoString = "PromotionDto";
		private const string _LineItemIdString = "LineItemId";

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
            if (!Page.IsPostBack && !ExpressionFilter.CausedCallback)
            {
                LoadExpressions(0, ExpressionFilter.DropDownPageSize * 2, "");
                BindForm();
            }
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

            ExpressionFilter.DataSource = dto.Expression;
            ExpressionFilter.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            PolicyGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_DeleteCommand);
            PolicyGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_InsertCommand);
            PolicyGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(DefaultGrid_InsertCommand);
            PolicyGrid.PreRender += new EventHandler(DefaultGrid_PreRender);
            base.OnInit(e);
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

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            GridHelper.BindGrid(PolicyGrid, "Marketing", "PromotionPolicy-List");

            if (_promotion != null)
            {
                //first check permissions
                //if permissions not present, deny
                SecurityManager.CheckRolePermission("marketing:policies:mng:edit");

                PolicyGrid.DataSource = _promotion.PromotionPolicy;
                PolicyGrid.DataBind();
            }
            else
            {
                SecurityManager.CheckRolePermission("marketing:policies:mng:create");
            }
        }

        /// <summary>
        /// Gets the policy.
        /// </summary>
        /// <param name="policyId">The policy id.</param>
        /// <returns></returns>
        protected PolicyDto GetPolicy(int policyId)
        {
            PolicyDto policy = PolicyManager.GetPolicyDto(policyId);
            return policy;
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
                int id = item[_LineItemIdString] == null ? 0 : Int32.Parse(item[_LineItemIdString].ToString());
                string entryid = item["CatalogEntryId"].ToString();
                decimal qty = 0;
                Decimal.TryParse(item["Quantity"].ToString(), out qty);
                string name = item["DisplayName"].ToString();
                decimal price = 0;
                Decimal.TryParse(item["ListPrice"].ToString(), out price);

                LineItem lineItem = null;
                // find the existing one
                /*
                foreach (LineItem litem in GetLineItems(po))
                {
                    if (litem.ObjectState != MetaObjectState.Added)
                    {
                        if (litem.LineItemId == id)
                        {
                            lineItem = litem;
                            break;
                        }
                    }
                }
                 * */

                if (lineItem == null)
                {
                    lineItem = new LineItem();
                    //po.OrderForms[0].LineItems.Add(lineItem);
                }

                lineItem.CatalogEntryId = entryid;
                lineItem.Quantity = qty;
                lineItem.DisplayName = name;

                lineItem.ListPrice = price;
                lineItem.PlacedPrice = price;
                lineItem.ExtendedPrice = qty * price;
            }

            _addedItems.Clear();

            foreach (GridItem item in _removedItems)
            {
                int id = Int32.Parse(item[_LineItemIdString].ToString());
                // find the existing one
                /*
                foreach (LineItem litem in GetLineItems(po))
                {
                    if (litem.LineItemId == id)
                        litem.Delete();
                }
                 * */
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