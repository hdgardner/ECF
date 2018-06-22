using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;

namespace Mediachase.Commerce.Manager.Order
{
	/// <summary>
	///	Summary description for Taxes.
	/// </summary>
	public partial class Taxes : OrderBaseUserControl
	{
        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
		public string LanguageCode
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["lang"], String.Empty);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind(String.Empty);
		}

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

			base.OnInit(e);
		}

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
		void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
		{
			AdminView view = MyListView.CurrentListView.GetAdminView();
			foreach (ViewColumn column in view.Columns)
			{
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					LoadDataAndDataBind(sortExpression);
				}
			}
		}

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if ((IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID)) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
		}

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
		private void LoadDataAndDataBind(string sortExpression)
		{
			TaxDto dto = TaxManager.GetTaxDto(null);
			if (dto.Tax != null)
			{
                //string taxCategoryColumnKey = "TaxCategory";
                //string percentageColumnKey = "Percentage";
                string taxTypeNameColumnKey = "TaxTypeName";

                DataTable taxesTable = dto.Tax;
                //taxesTable.Columns.Add(percentageColumnKey, typeof(double));
                //taxesTable.Columns.Add(taxCategoryColumnKey, typeof(string));
                taxesTable.Columns.Add(taxTypeNameColumnKey, typeof(string));

				/*var tbl1 = from tax in dto.Tax 
						   join taxValue in dto.TaxValue
						   on tax.TaxId equals taxValue.TaxId 
						   select new { tax.TaxId, tax.Name, tax.TaxType, tax.SortOrder, Percentage = taxValue.Percentage, TaxCategory = taxValue.TaxCategory };*/

                for (int i = 0; i < dto.Tax.Rows.Count; i++)
                {
                    TaxDto.TaxRow currentTaxRow = (TaxDto.TaxRow)dto.Tax.Rows[i];
                    switch((TaxType)currentTaxRow.TaxType)
                    {
                        case TaxType.SalesTax:
                            currentTaxRow[taxTypeNameColumnKey] = Resources.SharedStrings.Sales;
                            break;
                        case TaxType.ShippingTax:
                            currentTaxRow[taxTypeNameColumnKey] = Resources.SharedStrings.Shipping;
                            break;
                        default:
                            currentTaxRow[taxTypeNameColumnKey] = String.Empty;
                            break;
                    }
                }

                //for (int i = 0; i < dto.Tax.Rows.Count; i++)
                //{
                //    TaxDto.TaxRow currentTaxRow = (TaxDto.TaxRow)dto.Tax.Rows[i];
                //    var valueRow = from taxValue in dto.TaxValue where taxValue.TaxId == currentTaxRow.TaxId select taxValue;
                //    bool fillWithEmptyValues = true;
                //    if (valueRow != null)
                //    {
                //        DataView dv = valueRow.AsDataView();
                //        if (dv.Count > 0) // must be 1
                //        {
                //            taxesTable.Rows[i][percentageColumnKey] = dv[0]["Percentage"];
                //            taxesTable.Rows[i][taxCategoryColumnKey] = dv[0]["TaxCategory"];
                //            fillWithEmptyValues = false;
                //        }
                //    }

                //    if (fillWithEmptyValues)
                //    {
                //        taxesTable.Rows[i][percentageColumnKey] = 0;
                //        taxesTable.Rows[i][taxCategoryColumnKey] = "";
                //    }
                //}

				DataView view = taxesTable.DefaultView;
				view.Sort = sortExpression;
				MyListView.DataSource = view;
			}

			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("TaxId");
			MyListView.DataBind();
		}
	}
}
