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
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class CurrencyRatesTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _CurrencyIdString = "currencyid";
		private const string _CurrencyDtoContextString = "CurrencyDto";
		private const string _CurrencyIdContextString = "CurrencyId";

		private const string _GridCurrencyRateIdString = "CurrencyRateId";
		private const string _GridToCurrencyIdString = "ToCurrencyId";
		private const string _GridCurrencyCodeString = "CurrencyCode"; // "To" currency code
		private const string _GridEndOfDayRateString = "EndOfDayRate";
		private const string _GridCurrencyRateDateString = "CurrencyRateDate";
		private const string _GridModifiedDateString = "ModifiedDate";

		List<GridItem> _removedItems = new List<GridItem>();

		private CurrencyDto _Currency = null;

		/// <summary>
		/// Gets the Currency id.
		/// </summary>
		/// <value>The Currency id.</value>
		public int CurrencyId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_CurrencyIdString);
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

			CurrencyRatesGrid.NeedRebind += new Grid.NeedRebindEventHandler(CurrencyRatesGrid_NeedRebind);
			CurrencyRatesGrid.NeedDataSource += new Grid.NeedDataSourceEventHandler(CurrencyRatesGrid_NeedDataSource);
			CurrencyRatesGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(CurrencyRatesGrid_DeleteCommand);
			CurrencyRatesGrid.PreRender += new EventHandler(CurrencyRatesGrid_PreRender);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		#region CurrencyRatesGrid event handlers
		/// <summary>
		/// Handles the NeedDataSource event of the CurrencyRatesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrencyRatesGrid_NeedDataSource(object sender, EventArgs e)
		{
			CurrencyRatesGrid.DataSource = GetCurrencyRatesDataSource();
		}

		/// <summary>
		/// Handles the NeedRebind event of the CurrencyRatesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrencyRatesGrid_NeedRebind(object sender, EventArgs e)
		{
			CurrencyRatesGrid.DataBind();
		}

		/// <summary>
		/// Handles the PreRender event of the CurrencyRatesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrencyRatesGrid_PreRender(object sender, EventArgs e)
		{
			// Postback happens so the grid will be completely updated, make sure to save all the changes
			if (this.IsPostBack)
				ProcessTableEvents(_Currency);
		}

		/// <summary>
		/// Handles the DeleteCommand event of the CurrencyRatesGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void CurrencyRatesGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			_removedItems.Add(e.Item);
		}
		#endregion

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			GridHelper.BindGrid(CurrencyRatesGrid, "Catalog", "CurrencyRates");
			BindCurrencyRatesGrid();
		}

		/// <summary>
		/// Binds the line items grid.
		/// </summary>
		private void BindCurrencyRatesGrid()
		{
			object dataSource = GetCurrencyRatesDataSource();
			if (dataSource != null)
			{
				CurrencyRatesGrid.DataSource = dataSource;
				CurrencyRatesGrid.DataBind();
			}
		}

		/// <summary>
		/// Gets the line items data source.
		/// </summary>
		/// <returns></returns>
		private object GetCurrencyRatesDataSource()
		{
			if (_Currency != null)
			{
				CurrencyDto.CurrencyRateRow[] rateRows = null;
				if (CurrencyId > 0)
					rateRows = (CurrencyDto.CurrencyRateRow[])_Currency.CurrencyRate.Select(String.Format("FromCurrencyId={0}", CurrencyId));
				else
					rateRows = (CurrencyDto.CurrencyRateRow[])_Currency.CurrencyRate.Select("", "", DataViewRowState.Added | DataViewRowState.ModifiedCurrent);

				DataTable dt = new DataTable();
				//if (curDto != null && curDto.CurrencyRate.Count > 0)
				//	dt.Merge(curDto.CurrencyRate);

				dt.Columns.AddRange(new DataColumn[5] { new DataColumn(_GridCurrencyRateIdString, typeof(int)), 
				new DataColumn(_GridToCurrencyIdString, typeof(int)),
				new DataColumn(_GridEndOfDayRateString, typeof(double)),
				new DataColumn(_GridCurrencyRateDateString, typeof(DateTime)),
				new DataColumn(_GridModifiedDateString, typeof(DateTime))});

				if (rateRows != null)
					foreach (CurrencyDto.CurrencyRateRow row in rateRows)
						dt.ImportRow(row);

				dt.Columns.Add(new DataColumn(_GridCurrencyCodeString, typeof(string)));

				foreach (DataRow row in dt.Rows)
				{
					// skip deleted rows
					if (row.RowState == DataRowState.Deleted)
						continue;

					// set currency code
					CurrencyDto.CurrencyRow[] currencyRows = (CurrencyDto.CurrencyRow[])_Currency.Currency.Select(String.Format("CurrencyId={0}", row[_GridToCurrencyIdString]));
					if (currencyRows != null && currencyRows.Length > 0)
						row[_GridCurrencyCodeString] = currencyRows[0].CurrencyCode;
					else
						row[_GridCurrencyCodeString] = String.Empty;
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
			_Currency = (CurrencyDto)context[_CurrencyDtoContextString];
			CurrencyRateDialog.LoadContext(context);
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CurrencyDto dto = (CurrencyDto)context[_CurrencyDtoContextString];

			if (_Currency != null && dto != null)
			{
				ProcessTableEvents(_Currency);
				dto.CurrencyRate.Merge(_Currency.CurrencyRate, false);
			}
		}

		/// <summary>
		/// Processes the table events.
		/// </summary>
		/// <param name="dto">The dto.</param>
		private void ProcessTableEvents(CurrencyDto dto)
		{
			if (dto == null)
				return;

			foreach (GridItem item in _removedItems)
			{
				int id = 0;
				if (item[_GridCurrencyRateIdString] != null && Int32.TryParse(item[_GridCurrencyRateIdString].ToString(), out id))
				{
					// find the rate
					CurrencyDto.CurrencyRateRow rate = dto.CurrencyRate.FindByCurrencyRateId(id);
					if (rate != null)
						rate.Delete();
				}
			}

			_removedItems.Clear();
		}
		#endregion
	}
}