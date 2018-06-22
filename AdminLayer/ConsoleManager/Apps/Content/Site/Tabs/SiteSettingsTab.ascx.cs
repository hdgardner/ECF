using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Cms.Dto;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Site.Tabs
{
	public partial class SiteSettingsTab : BaseUserControl, IAdminContextControl, IAdminTabControl
    {
		private const string _SiteIdString = "SiteId";
		private const string _SiteDtoSourceString = "SiteDtoSrc";
		private const string _SiteDtoDestinationString = "SiteDtoDest";
		private const string _SiteEditSessionKey = "ECF.Site.Edit";

		private const string _gridGlobalVariableIdColumn = "GlobalVariableId";
		private const string _gridKeyColumn = "KEY";
		private const string _gridValueColumn = "VALUE";

		private readonly string[] _systemVariables = new string[]{
			"url", "email", "phone", "address", "meta_keywords", "meta_description", "default_template", 
			"page_include", "title", "sitetheme", "cm_url"};

		private SiteDto _SiteDtoSrc = null;
		private SiteDto _SiteDtoDest = null;

		List<GridItem> _addedItems = new List<GridItem>();
		List<GridItem> _removedItems = new List<GridItem>();

		/// <summary>
		/// Gets the site id.
		/// </summary>
		/// <value>The site id.</value>
		public Guid SiteId
		{
			get
			{
				return ManagementHelper.GetGuidFromQueryString(_SiteIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "InitializeSystemVariables", String.Format("var systemVariablesArray = {0};", GetSystemVariablesArray()), true);

			if (!Page.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			SiteSettingsDefaultGrid.DeleteCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(SiteSettingsDefaultGrid_DeleteCommand);
			SiteSettingsDefaultGrid.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(SiteSettingsDefaultGrid_InsertCommand);
			SiteSettingsDefaultGrid.InsertCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(SiteSettingsDefaultGrid_InsertCommand);

			base.OnInit(e);
		}

		/// <summary>
		/// Binds a data source to the invoked server control and all its child controls.
		/// </summary>
		public override void DataBind()
		{
			base.DataBind();
			BindForm();
		}

		#region Grid Commands
		/// <summary>
		/// Handles the DeleteCommand event of the SiteSettingsDefaultGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void SiteSettingsDefaultGrid_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			string key = e.Item[_gridKeyColumn].ToString();

			foreach (GridItem item in _addedItems)
			{
				if (String.Compare(item[_gridKeyColumn].ToString(), key, true) == 0)
				{
					_addedItems.Remove(item);
					break;
				}
			}

			_removedItems.Add(e.Item);
		}

		/// <summary>
		/// Handles the InsertCommand event of the SiteSettingsDefaultGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="ComponentArt.Web.UI.GridItemEventArgs"/> instance containing the event data.</param>
		void SiteSettingsDefaultGrid_InsertCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
		{
			string key = e.Item[_gridKeyColumn].ToString();

			foreach (GridItem item in _removedItems)
			{
				if (String.Compare(item[_gridKeyColumn].ToString(), key, true) == 0)
				{
					_removedItems.Remove(item);
					break;
				}
			}

			_addedItems.Add(e.Item);
		}
		#endregion

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			GridHelper.BindGrid(SiteSettingsDefaultGrid, "Content", "Site-Edit");

			if (SiteId != Guid.Empty && _SiteDtoDest != null && _SiteDtoDest.Site.Count > 0)
			{
				string variablesFilter = MakeVariablesFilter();
				SiteDto.main_GlobalVariablesRow[] rows = (SiteDto.main_GlobalVariablesRow[])_SiteDtoDest.main_GlobalVariables.Select(variablesFilter, "KEY");

				SiteSettingsDefaultGrid.DataSource = rows;
			}

			SiteSettingsDefaultGrid.DataBind();
		}

		private string GetSystemVariablesArray()
		{
			StringBuilder sbArray = new StringBuilder();
			sbArray.Append("[");

			if (_systemVariables.Length > 0)
			{
				for (int index = 0; index < _systemVariables.Length - 1; index++)
					sbArray.AppendFormat("'{0}', ", _systemVariables[index]);

				sbArray.AppendFormat("'{0}'", _systemVariables[_systemVariables.Length - 1]);
			}

			sbArray.Append("]");

			return sbArray.ToString();
		}

		private string MakeVariablesFilter()
		{
			StringBuilder filterString = new StringBuilder();

			if (_systemVariables.Length > 0)
			{
				for (int index = 0; index < _systemVariables.Length - 1; index++)
					filterString.AppendFormat("KEY<>'{0}' and ", _systemVariables[index]);

				filterString.AppendFormat("KEY<>'{0}'", _systemVariables[_systemVariables.Length - 1]);
			}

			return filterString.ToString();
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_SiteDtoSrc = (SiteDto)context[_SiteDtoSourceString];
			_SiteDtoDest = (SiteDto)context[_SiteDtoDestinationString];
		}
		#endregion

		#region IAdminTabControl Members

		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			SiteDto dto = (SiteDto)context[_SiteDtoDestinationString];

			if (dto != null)
			{
				// remove items
				if (_removedItems.Count > 0)
				{
					StringBuilder filterString = new StringBuilder();
					for (int index = 0; index < _removedItems.Count - 1; index++)
						filterString.AppendFormat("KEY='{0}' OR ", _removedItems[index][_gridKeyColumn]);

					filterString.AppendFormat("KEY='{0}'", _removedItems[_removedItems.Count - 1][_gridKeyColumn]);

					SiteDto.main_GlobalVariablesRow[] removedRows = (SiteDto.main_GlobalVariablesRow[])dto.main_GlobalVariables.Select(filterString.ToString());
					if (removedRows != null)
						foreach (SiteDto.main_GlobalVariablesRow removedRow in removedRows)
							removedRow.Delete();
				}

				foreach (GridItem item in SiteSettingsDefaultGrid.Items)
				{
					SiteDto.main_GlobalVariablesRow currentRow = null;
					SiteDto.main_GlobalVariablesRow[] addedRows = (SiteDto.main_GlobalVariablesRow[])dto.main_GlobalVariables.Select(String.Format("KEY='{0}'", item[_gridKeyColumn]));
					if (addedRows != null && addedRows.Length > 0)
						currentRow = addedRows[0];
					else
					{
						// add a new key-value pair
						currentRow = dto.main_GlobalVariables.Newmain_GlobalVariablesRow();
						currentRow.SiteId = Guid.Empty;
						currentRow.KEY = item[_gridKeyColumn].ToString();
					}

					currentRow.VALUE = (string)item[_gridValueColumn];

					if (currentRow.RowState == DataRowState.Detached)
						dto.main_GlobalVariables.Addmain_GlobalVariablesRow(currentRow);
				}
			}
		}
		#endregion
    }
}