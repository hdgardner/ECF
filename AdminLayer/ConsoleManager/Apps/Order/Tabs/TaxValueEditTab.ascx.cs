using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using System.Collections.Generic;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class TaxValueEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _TaxIdString = "taxid";
		private const string _TaxDtoContextString = "TaxDto";
		private const string _TaxIdContextString = "TaxId";

		private const string _GridTaxValueIdString = "TaxValueId";
        private const string _GridTaxIdString = "TaxId";
        private const string _GridPercentageString = "Percentage";
        private const string _GridTaxCategoryString = "TaxCategory";
        private const string _GridJurisdictionGroupIdString = "JurisdictionGroupId";
        private const string _GridJurisdictionGroupString = "JurisdictionGroup";
        private const string _GridAffectiveDateString = "AffectiveDate";

		List<GridItem> _removedItems = new List<GridItem>();

		private TaxDto _Tax = null;

		/// <summary>
		/// Gets the Tax id.
		/// </summary>
		/// <value>The Tax id.</value>
		public int TaxId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_TaxIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindForm();

			TaxValuesGrid.NeedRebind += new Grid.NeedRebindEventHandler(TaxValuesGrid_NeedRebind);
			TaxValuesGrid.NeedDataSource += new Grid.NeedDataSourceEventHandler(TaxValuesGrid_NeedDataSource);
			TaxValuesGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(TaxValuesGrid_DeleteCommand);
			TaxValuesGrid.PreRender += new EventHandler(TaxValuesGrid_PreRender);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		#region TaxValuesGrid event handlers
		/// <summary>
		/// Handles the NeedDataSource event of the TaxValuesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void TaxValuesGrid_NeedDataSource(object sender, EventArgs e)
		{
			TaxValuesGrid.DataSource = GetTaxValuesDataSource();
		}

		/// <summary>
		/// Handles the NeedRebind event of the TaxValuesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void TaxValuesGrid_NeedRebind(object sender, EventArgs e)
		{
			TaxValuesGrid.DataBind();
		}

		/// <summary>
		/// Handles the PreRender event of the TaxValuesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void TaxValuesGrid_PreRender(object sender, EventArgs e)
		{
			// Postback happens so the grid will be completely updated, make sure to save all the changes
			if (this.IsPostBack)
				ProcessTableEvents(_Tax);
		}

		/// <summary>
		/// Handles the DeleteCommand event of the TaxValuesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void TaxValuesGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			_removedItems.Add(e.Item);
		}
		#endregion

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			GridHelper.BindGrid(TaxValuesGrid, "Order", "TaxValues");
			BindTaxValuesGrid();
		}

		/// <summary>
		/// Binds the line items grid.
		/// </summary>
		private void BindTaxValuesGrid()
		{
			object dataSource = GetTaxValuesDataSource();
			if (dataSource != null)
			{
				TaxValuesGrid.DataSource = dataSource;
				TaxValuesGrid.DataBind();
			}
		}

		/// <summary>
		/// Gets the line items data source.
		/// </summary>
		/// <returns></returns>
		private object GetTaxValuesDataSource()
		{
			if (_Tax != null)
			{
				TaxDto.TaxValueRow[] valueRows = null;
				if (TaxId > 0)
					valueRows = (TaxDto.TaxValueRow[])_Tax.TaxValue.Select(String.Format("TaxId={0}", TaxId));
				else
					valueRows = (TaxDto.TaxValueRow[])_Tax.TaxValue.Select("", "", DataViewRowState.Added | DataViewRowState.ModifiedCurrent);

				DataTable dt = new DataTable();

				dt.Columns.AddRange(new DataColumn[6] { new DataColumn(_GridTaxValueIdString, typeof(int)),
                new DataColumn(_GridTaxIdString, typeof(int)),
				new DataColumn(_GridPercentageString, typeof(float)),
				new DataColumn(_GridTaxCategoryString, typeof(string)),
                new DataColumn(_GridJurisdictionGroupIdString, typeof(int)),
				new DataColumn(_GridAffectiveDateString, typeof(DateTime))});

				if (valueRows != null)
					foreach (TaxDto.TaxValueRow row in valueRows)
						dt.ImportRow(row);

                dt.Columns.Add(new DataColumn(_GridJurisdictionGroupString, typeof(string)));

                foreach (DataRow row in dt.Rows)
                {
                    // skip deleted rows
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    JurisdictionDto jurisdictions = JurisdictionManager.GetJurisdictionGroup((int)row[_GridJurisdictionGroupIdString]);
                    if (jurisdictions.JurisdictionGroup.Rows.Count > 0)
                    {
                        row[_GridJurisdictionGroupString] = jurisdictions.JurisdictionGroup[0].DisplayName;
                    }
                    else
                    {
                        row[_GridJurisdictionGroupString] = String.Empty;
                    }
                }

				return dt;
			}

			return null;
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Tax = (TaxDto)context[_TaxDtoContextString];
			TaxValueDialog.LoadContext(context);
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			TaxDto dto = (TaxDto)context[_TaxDtoContextString];

			if (_Tax != null && dto != null)
			{
				ProcessTableEvents(_Tax);
				dto.TaxValue.Merge(_Tax.TaxValue, false);
			}
		}

		/// <summary>
		/// Processes the table events.
		/// </summary>
		/// <param name="dto">The dto.</param>
		private void ProcessTableEvents(TaxDto dto)
		{
			if (dto == null)
				return;

			foreach (GridItem item in _removedItems)
			{
				int id = 0;
				if (item[_GridTaxValueIdString] != null && Int32.TryParse(item[_GridTaxValueIdString].ToString(), out id))
				{
					// find the value
					TaxDto.TaxValueRow value = dto.TaxValue.FindByTaxValueId(id);
					if (value != null)
						value.Delete();
				}
			}

			_removedItems.Clear();
		}
		#endregion
	}
}