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
using Mediachase.Commerce.Manager;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Interfaces;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace Mediachase.Commerce.Manager.Core.Controls
{
    public partial class EcfListViewControl : BaseUserControl
	{
		#region Public Properties
		string _AppId = String.Empty;
        /// <summary>
        /// Gets or sets the app id.
        /// </summary>
        /// <value>The app id.</value>
		public string AppId
		{
			get
			{
				return _AppId;
			}
			set
			{
				_AppId = value;
			}
		}

		string _ViewId = String.Empty;
        /// <summary>
        /// Gets or sets the view id.
        /// </summary>
        /// <value>The view id.</value>
		public string ViewId
		{
			get
			{
				return _ViewId;
			}
			set
			{
				_ViewId = value;
			}
		}

		bool _showTopToolbar = false;
        /// <summary>
        /// Gets or sets a value indicating whether [show top toolbar].
        /// </summary>
        /// <value><c>true</c> if [show top toolbar]; otherwise, <c>false</c>.</value>
		public bool ShowTopToolbar
		{
			get
			{
				return _showTopToolbar;
			}
			set
			{
				_showTopToolbar = value;
			}
		}

        /// <summary>
        /// Gets the current list view.
        /// </summary>
        /// <value>The current list view.</value>
		public EcfListView CurrentListView
		{
			get
			{
				return MainListView;
			}
		}

        /// <summary>
        /// Gets the current pager.
        /// </summary>
        /// <value>The current pager.</value>
		public DataPager CurrentPager
		{
			get
			{
				return MainListView.FindControl("mainListViewPager2") as DataPager;
			}
		}

        /// <summary>
        /// Gets the inner list view table.
        /// </summary>
        /// <value>The inner list view table.</value>
		public HtmlTable InnerListViewTable
		{
			get
			{
    		    return MainListView.FindControl(EcfListView.DefaultInnerTableId) as HtmlTable;
			}
		}

        /// <summary>
        /// Gets the inner list view table header.
        /// </summary>
        /// <value>The inner list view table header.</value>
        public HtmlTableRow InnerListViewTableHeader
        {
            get
            {
                return MainListView.FindControl(EcfListView.DefaultTableHeaderId) as HtmlTableRow;
            }
        }

        /// <summary>
        /// Gets the main update panel.
        /// </summary>
        /// <value>The main update panel.</value>
        public UpdatePanel MainUpdatePanel
		{
			get
			{
                return panelMainListView;
			}
		}

		string _DataKey;
        /// <summary>
        /// Gets or sets the data key.
        /// </summary>
        /// <value>The data key.</value>
		public string DataKey
		{
			get
			{
				return _DataKey;
			}
			set
			{
				_DataKey = value;
			}
		}

        /// <summary>
        /// Gets or sets the data source ID.
        /// </summary>
        /// <value>The data source ID.</value>
		public string DataSourceID
		{
			get
			{
				return MainListView.DataSourceID;
			}
			set
			{
				MainListView.DataSourceID = value;
			}
		}

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public object DataSource
        {
            get
            {
                return MainListView.DataSource;
            }
            set
            {
                MainListView.DataSource = value;
            }
        }
		
        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        public string DataMember
        {
            get
            {
                return MainListView.DataMember;
            }
            set
            {
                MainListView.DataMember = value;
            }
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
		public int PageSize
		{
			get
			{
				return MainListView.CurrentPageSize;
			}
		}
		#endregion

        /// <summary>
        /// Gets the list view client ID.
        /// </summary>
        /// <returns></returns>
        public string GetListViewClientID()
        {
            return InnerListViewTable != null ? InnerListViewTable.ClientID : String.Empty;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            AdminView view = ManagementContext.Current.FindView(this.AppId, this.ViewId);

            // Check view permissions
            if (view.Attributes.Contains("permissions"))
            {
                if (!ProfileContext.Current.CheckPermission(view.Attributes["permissions"].ToString()))
                    throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            }

			Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "InitializeEcfListViewConstants",
				String.Format("CSManagementClient.EcfListView_PrimaryKeySeparator = '{0}';", EcfListView.ExtenderPrimaryKeyAttributeSeparator), true);

			HtmlTable tbl = this.FindControl("topTable") as HtmlTable;
			if (tbl != null)
			{
				if (this.ShowTopToolbar)
				{
					tbl.Visible = true;

					// bind MetaToolbar
					MetaToolbar1.ViewName = this.AppId;
					MetaToolbar1.PlaceName = this.ViewId;
					MetaToolbar1.GridClientId = GetListViewClientID();
					MetaToolbar1.DataBind();
				}
				else
				{
					DockTop.DefaultSize = 0;
					DockTop.Visible = false;
					tbl.Visible = false;
				}
				//tbl.Style["display"] = "none";
			}

			if (InnerListViewTable != null)
                gvHeaderExtender.TargetControlID = InnerListViewTable.UniqueID;

			gvHeaderExtender.ContextKey = UtilHelper.JsonSerialize(new EcfListViewContextKey(this.AppId, this.ViewId));
			gvHeaderExtender.ServicePath = ResolveUrl("~/Apps/Core/Controls/WebServices/EcfListViewExtenderService.asmx");

			DropDownList pagingList = MainListView.FindControl(EcfListView.DefaultPagingDropdownId) as DropDownList;
			if (pagingList != null)
				pagingList.SelectedIndexChanged += new EventHandler(ddPaging_SelectedIndexChanged);

			if (!IsPostBack && CurrentPager != null)
			{
				CurrentPager.SetPageProperties(0, CurrentListView.CurrentPageSize, true);
			}
		}

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
            if (InnerListViewTable != null)
            {
                BindGridStyles();
                BindExtenderColumns();
            }
            else
            {
                if (MainListView.Items.Count == 0)
                    gvHeaderExtender.TargetControlID = this.FindControl("emptyTable").UniqueID;
            }

			// visibility will be set to visible by extender when grid is rendered (this fixes bug with blinking grid while rendering)
			if (InnerListViewTable != null)
				InnerListViewTable.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			MainListView.AppId = this.AppId;
			MainListView.ViewId = this.ViewId;
			base.DataBind();
		}

        /// <summary>
        /// Resets the page number.
        /// </summary>
		public void ResetPageNumber()
		{
			ResetPageNumber(false);
		}

        /// <summary>
        /// Resets the page number.
        /// </summary>
        /// <param name="dataBind">if set to <c>true</c> [data bind].</param>
		private void ResetPageNumber(bool dataBind)
		{
			DataPager pager = CurrentPager;
			if (pager != null)
				pager.SetPageProperties(0, MainListView.CurrentPageSize, dataBind);
		}

        /// <summary>
        /// Binds the grid info.
        /// </summary>
		private void BindGridInfo()
		{
			if (!String.IsNullOrEmpty(DataKey))
				MainListView.DataKeyNames = new string[1] { DataKey };
		}

        /// <summary>
        /// Binds the grid styles.
        /// </summary>
        private void BindGridStyles()
        {
            if (InnerListViewTable != null)
                InnerListViewTable.Attributes["class"] = "ecf-Grid";
        }

        /// <summary>
        /// Binds the extender columns.
        /// </summary>
		private void BindExtenderColumns()
		{
			List<GridViewColumnInfo> ciCollection = new List<GridViewColumnInfo>();

			// fill GridViewColumnInfo collection
			AdminView view = MainListView.GetAdminView();
			if (view != null)
			{
				// get settings
				string gridSettingsKey = CMPageSettings.MakeGridSettingsKey(this.AppId, this.ViewId);
				string oldGridSettings = ProfileContext.Current.Profile.PageSettings.GetSettingString(gridSettingsKey);
				EcfListViewPreferences prefs = null;
				if (!String.IsNullOrEmpty(oldGridSettings))
					prefs = UtilHelper.JsonDeserialize<EcfListViewPreferences>(oldGridSettings);

				// set columns' properties
				gvHeaderExtender.ColumnsInfo.Clear();
				foreach (ViewColumn viewColumn in view.Columns)
				{
					if (!viewColumn.Visible)
						continue;

					GridViewColumnInfo gvColumn = new GridViewColumnInfo();
					gvColumn.Sortable = viewColumn.ColumnType == ColumnType.Action ? false : viewColumn.AllowSorting;
					gvColumn.Resizable =
						viewColumn.ColumnType == ColumnType.CheckBox || viewColumn.ColumnType == ColumnType.Action ?
						false : viewColumn.Resizable;
					int width = 0;
					if (viewColumn.ColumnType == ColumnType.CheckBox)
						// checkbox column should be of fixed width
						width = EcfListView.DefaultCheckboxColumnWidth;
					else
					{
						// set width
						if (prefs != null && prefs.ColumnProperties[viewColumn.ColumnIndex.ToString()] != null)
							width = Int32.Parse((string)prefs.ColumnProperties[viewColumn.ColumnIndex.ToString()]);
						else if (!Int32.TryParse(viewColumn.Width, out width))
							width = EcfListView.DefaultColumnWidth;
					}
					gvColumn.Width = width;

					gvHeaderExtender.ColumnsInfo.Add(gvColumn);
				}
			}

			this.gvHeaderExtender.HeaderHeight = EcfListView.DefaultHeaderHeight;

			this.gvHeaderExtender.StyleInfo = new Mediachase.Ibn.Web.UI.WebControls.GridStylesInfo(MainListView.HeaderCssClass, MainListView.GridCssClass, MainListView.FooterCssClass, MainListView.HeaderInnerCssClass, MainListView.GridInnerCssClass, MainListView.GridSelectedRowCssClass);
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddPaging control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddPaging_SelectedIndexChanged(object sender, EventArgs e)
		{
			DropDownList ddPaging = (DropDownList)sender;

			if (Convert.ToInt32(ddPaging.SelectedValue) != -1)
			{
				MainListView.CurrentPageSize = Convert.ToInt32(ddPaging.SelectedValue);
			}
			else
				MainListView.CurrentPageSize = Int32.MaxValue;

			// save new pageSize
			EcfListView.SavePageSize(this.Page, this.ViewId, MainListView.CurrentPageSize);

			ResetPageNumber(true);
		}
	}

	/// <summary>
	/// Object for passing as a contextKey for grid header extender
	/// </summary>
	class EcfListViewContextKey
	{
		private string _appId;

		public string AppId
		{
			get { return _appId; }
			set { _appId = value; }
		}

		private string _viewId;

		public string ViewId
		{
			get { return _viewId; }
			set { _viewId = value; }
		}

		#region .ctor
		public EcfListViewContextKey()
		{
		}

		public EcfListViewContextKey(string appId, string viewId)
		{
			this.AppId = appId;
			this.ViewId = viewId;
		}
		#endregion
	}
}