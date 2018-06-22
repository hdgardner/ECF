using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	public partial class CountryRegionsTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _CountryIdString = "countryid";
		private const string _CountryDtoString = "CountryDto";

		private const string _GridStateProvinceIdString = "StateProvinceId";
		private const string _GridStateProvinceNameString = "Name";
		private const string _GridOrderingString = "Ordering";
		private const string _GridVisibleString = "Visible";

		private CountryDto _Country = null;

		List<GridItem> _addedItems = new List<GridItem>();
		List<GridItem> _removedItems = new List<GridItem>();

		/// <summary>
		/// Gets the Country id.
		/// </summary>
		/// <value>The Country id.</value>
		public int CountryId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_CountryIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack && !RegionsGrid.CausedCallback)
				BindForm();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			RegionsGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(RegionsGrid_DeleteCommand);
			RegionsGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(RegionsGrid_InsertCommand);
			RegionsGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(RegionsGrid_InsertCommand);
			RegionsGrid.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(RegionsGrid_OnNeedRebind);
			RegionsGrid.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(RegionsGrid_OnNeedDataSource);

			base.OnInit(e);
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			GridHelper.BindGrid(RegionsGrid, "Order", "CountryRegions");

			if (_Country != null)
			{	
				BindGridData();
			}
		}

		/// <summary>
		/// Binds the grid.
		/// </summary>
		private void BindGridData()
		{
			SetRegionsGridDataSource();
			//RegionsGrid.PagerPosition = GridElementPosition.BottomRight;
			//RegionsGrid.AllowPaging = true;
			RegionsGrid.PageSize = 100;
			
			RegionsGrid.DataBind();
		}

		/// <summary>
		/// Sets the association items grid data source.
		/// </summary>
		private void SetRegionsGridDataSource()
		{
			RegionsGrid.DataSource = _Country != null ? _Country.StateProvince.DefaultView : null;
			//RegionsGrid.RecordCount = _Country.StateProvince.Count;
		}

		#region RegionsGrid Methods
		/// <summary>
		/// Handles the DeleteCommand event of the RegionsGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void RegionsGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			string name = e.Item[_GridStateProvinceNameString].ToString();

			if (_Country != null)
			{
				CountryDto.StateProvinceRow[] spRows = (CountryDto.StateProvinceRow[])_Country.StateProvince.Select(String.Format("Name='{0}'", name));
				if (spRows != null && spRows.Length > 0)
					spRows[0].Delete();
			}
		}

		/// <summary>
		/// Handles the InsertCommand event of the RegionsGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void RegionsGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			string name = e.Item[_GridStateProvinceNameString].ToString();

			if (_Country != null)
			{
				CountryDto.StateProvinceRow spRow = null;

				CountryDto.StateProvinceRow[] spRows = (CountryDto.StateProvinceRow[])_Country.StateProvince.Select(String.Format("Name='{0}'", name));
				if (spRows == null || spRows.Length == 0)
				{
					spRow = _Country.StateProvince.NewStateProvinceRow();
					spRow.CountryId = CountryId;
				}
				else
					spRow = spRows[0];

				spRow.Name = name;
				spRow.Ordering = Int32.Parse(e.Item[_GridOrderingString].ToString());
				spRow.Visible = Boolean.Parse(e.Item[_GridVisibleString].ToString());

				if (spRow.RowState == DataRowState.Detached)
					_Country.StateProvince.Rows.Add(spRow);
			}
		}

		/// <summary>
		/// Handles the OnNeedDataSource event of the RegionsGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void RegionsGrid_OnNeedDataSource(object sender, EventArgs oArgs)
		{
			SetRegionsGridDataSource();
		}

		/// <summary>
		/// Handles the OnNeedRebind event of the RegionsGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="oArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		public void RegionsGrid_OnNeedRebind(object sender, System.EventArgs oArgs)
		{
			RegionsGrid.DataBind();
		}
		#endregion

		/// <summary>
		/// Checks if entered region name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		protected void RegionNameCheck(object sender, ServerValidateEventArgs args)
		{
			// load country by code
			if (CountryId > 0)
			{
				CountryDto dto = CountryManager.GetCountry(CountryId, true);

				// check if country with specified code is loaded
				if (dto != null && dto.Country.Count > 0 &&
					dto.Country[0].CountryId != CountryId &&
					dto.StateProvince.Select(String.Format("Name='{0}'", args.Value)) != null)
				{
					args.IsValid = false;
					return;
				}
			}

			args.IsValid = true;
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Country = (CountryDto)context[_CountryDtoString];
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CountryDto dto = (CountryDto)context[_CountryDtoString];

			if (dto == null || dto.Country.Count == 0)
				// dto must be created in base control that holds tabs
				return;

			CountryDto.CountryRow countryRow = dto.Country[0];

			dto.StateProvince.Merge(_Country.StateProvince, false);

			//countryRow.Name = tbCountryName.Text;
			//countryRow.Code = CodeText.Text;
			//countryRow.Ordering = Int32.Parse(SortOrder.Text);
			//countryRow.Visible = IsVisible.IsSelected;

			//if (countryRow.RowState == DataRowState.Detached)
			//    dto.Country.Rows.Add(countryRow);
		}
		#endregion
	}
}