using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Web.Console.Controls
{
    public class EcfListView : System.Web.UI.WebControls.ListView
    {
        public const int DefaultPageSize = 20;
        public const int DefaultCheckboxColumnWidth = 24;
        public const int DefaultColumnWidth = 100;
        public const int DefaultHeaderHeight = 24;
        public const string DefaultTableHeaderId = "headerTRow";
        public const string DefaultInnerTableId = "lvTable";
        public const string DefaultFooterCellId = "tdFooter";
        public const string DefaultFooterDivId = "footerDiv";
        public const string DefaultPagingDropdownId = "ddPaging";
        public const string DefaultSortImageControlPrefix = "img";
        private const string DefaultImageUrlAscending = "grid/asc.gif";
        private const string DefaultImageUrlDescending = "grid/desc.gif";

        public const string ExtenderPrimaryKeyAttributeSeparator = "::"; // separator between items in a primary key
        private const string PrimaryKeysListSeparator = ";;"; // separator between primary keys. List of PrimaryKeys should look as following: item11::item12::item13;;item21::item22::item23;;item31::item32::item33
        private const string ExtenderPrimaryKeyAttributeName = "ibn_primaryKeyId";
        private const string ExtenderCheckboxGridAttributeName = "ibn_serverGridCheckbox";

        public const int ColumnWithCheckboxes = 0;

        public const string GridCommandParameterName = "GridCommand"; // used for columns with Type = Action

		#region Column Attribute Names
		private const string _navigateUrlKey = "NavigateUrl";
		private const string _dataNavigateUrlFormatString = "DataNavigateUrlFormatString";
		private const string _dataNavigateUrlFields = "DataNavigateUrlFields";
		private const string _dataTextFormatString = "DataTextFormatString";
		private const string _dataTextFields = "DataTextFields";
		private const string _checkboxEnableDataField = "CheckboxEnableField";
		#endregion

		#region Private Properties

		// Properties' constants
        private const string _AppId = "_AppId";
        private const string _ViewId = "_ViewId";
        private const string _PrimaryKeyId = "_PrimaryKeyId";
        private const string _ShowCheckboxes = "_ShowCheckboxes";
        private const string _ShowPaging = "_ShowPaging";
        private const string _CurrentPageSize = "_CurrentPageSize";

        private const string _ImagesBaseUrl = "~/App_Themes/Default/Images/";
        private const string _ImageUrlAscending = "_ImageUrlAscending";
        private const string _ImageUrlDescending = "_ImageUrlDescending";
        private const string _SortExpressionDefault = "_SortExpressionDefault";
        private const string _SortDirectionDefault = "_SortDirectionDefault";
        private const string _SortProperties = "_SortProperties";

        // Styles' constants
        private const string _TableCssClass = "_TableCssClass";
        private const string _TableHeadingRowCssClass = "_TableHeadingRowCssClass";
        private const string _TableHeadingCellCssClass = "_TableHeadingCellCssClass";
        private const string _TableCellCssClass = "_TableCellCssClass";
        private const string _TableRowCssClass = "_TableRowCssClass";
        private const string _TableAltRowCssClass = "_TableAltRowCssClass";
        private const string _TableSelectedRowCssClass = "_TableSelectedRowCssClass";
        private const string _PagerRowCssClass = "_PagerRowCssClass";
        private const string _PagerTextCssClass = "_PagerTextCssClass";
        private const string _PagerDropdownCssClass = "_PagerDropdownCssClass";

        #endregion

        #region styles: Default constants
        private readonly string defaultHeaderCssClass = "serverGridHeader";
        private readonly string defaultGridCssClass = "serverGridBody";
        private readonly string defaultFooterCssClass = "serverGridFooter";
        private readonly string defaultHeaderInnerCssClass = "serverGridHeaderInner";
        private readonly string defaultGridInnerCssClass = "serverGridInner";
        private readonly string defaultGridSelectedRowCssClass = "serverGridSelectedRow";
        #endregion

        #region Public Properties
        #region Common

        /// <summary>
        /// Gets or sets the app id.
        /// </summary>
        /// <value>The app id.</value>
        public string AppId
        {
            get
            {
                return (this.ViewState[_AppId] == null) ? null : (string)this.ViewState[_AppId];
            }
            set
            {
                if (this.ViewState[_AppId] != null && String.Compare(this.ViewState[_AppId].ToString(), value, true) != 0)
                    _currentViewInitialized = false; // reset current view
                this.ViewState[_AppId] = value;
            }
        }

        /// <summary>
        /// Gets or sets the view id.
        /// </summary>
        /// <value>The view id.</value>
        public string ViewId
        {
            get
            {
                return (this.ViewState[_ViewId] == null) ? null : (string)this.ViewState[_ViewId];
            }
            set
            {
                if (this.ViewState[_ViewId] != null && String.Compare(this.ViewState[_ViewId].ToString(), value, true) != 0)
                    _currentViewInitialized = false; // reset current view

                this.ViewState[_ViewId] = value;
            }
        }

        /// <summary>
        /// Gets or sets the primary key id.
        /// </summary>
        /// <value>The primary key id.</value>
        public string PrimaryKeyId
        {
            get
            {
                return (this.ViewState[_PrimaryKeyId] == null) ? null : (string)this.ViewState[_PrimaryKeyId];
            }
            set
            {
                this.ViewState[_PrimaryKeyId] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show checkboxes].
        /// </summary>
        /// <value><c>true</c> if [show checkboxes]; otherwise, <c>false</c>.</value>
        public bool ShowCheckboxes
        {
            get
            {
                return (this.ViewState[_ShowCheckboxes] == null) ? false : Convert.ToBoolean(this.ViewState[_ShowCheckboxes].ToString());
            }
            set
            {
                this.ViewState[_ShowCheckboxes] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show paging].
        /// </summary>
        /// <value><c>true</c> if [show paging]; otherwise, <c>false</c>.</value>
        public bool ShowPaging
        {
            get
            {
                return (this.ViewState[_ShowPaging] == null) ? true : (bool)this.ViewState[_ShowPaging];
            }
            set
            {
                this.ViewState[_ShowPaging] = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the current page.
        /// </summary>
        /// <value>The size of the current page.</value>
        public int CurrentPageSize
        {
            get
            {
                return (this.ViewState[_CurrentPageSize] == null) ? DefaultPageSize : (int)this.ViewState[_CurrentPageSize];
            }
            set
            {
                this.ViewState[_CurrentPageSize] = value;
            }
        }

        /// <summary>
        /// Gets or sets the images base URL.
        /// </summary>
        /// <value>The images base URL.</value>
        public string ImagesBaseUrl
        {
            get
            {
                return (this.ViewState[_ImagesBaseUrl] == null) ? string.Empty : (string)this.ViewState[_ImagesBaseUrl];
            }
            set
            {
                this.ViewState[_ImagesBaseUrl] = value;
            }
        }

        /// <summary>
        /// Gets or sets the image URL ascending.
        /// </summary>
        /// <value>The image URL ascending.</value>
        public string ImageUrlAscending
        {
            get
            {
                return (this.ViewState[_ImageUrlAscending] == null) ? EcfListView.DefaultImageUrlAscending : (string)this.ViewState[_ImageUrlAscending];
            }
            set
            {
                this.ViewState[_ImageUrlAscending] = value;
            }
        }

        /// <summary>
        /// Gets or sets the image URL descending.
        /// </summary>
        /// <value>The image URL descending.</value>
        public string ImageUrlDescending
        {
            get
            {
                return (this.ViewState[_ImageUrlDescending] == null) ? EcfListView.DefaultImageUrlDescending : (string)this.ViewState[_ImageUrlDescending];
            }
            set
            {
                this.ViewState[_ImageUrlDescending] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort expression default.
        /// </summary>
        /// <value>The sort expression default.</value>
        public string SortExpressionDefault
        {
            get
            {
                return (this.ViewState[_SortExpressionDefault] == null) ? string.Empty : (string)this.ViewState[_SortExpressionDefault];
            }
            set
            {
                this.ViewState[_SortExpressionDefault] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort direction default.
        /// </summary>
        /// <value>The sort direction default.</value>
        public SortDirection SortDirectionDefault
        {
            get
            {
                return (this.ViewState[_SortDirectionDefault] == null) ? SortDirection.Ascending : (SortDirection)this.ViewState[_SortDirectionDefault];
            }
            set
            {
                this.ViewState[_SortDirectionDefault] = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort properties.
        /// </summary>
        /// <value>The sort properties.</value>
        public SortViewState SortProperties
        {
            get
            {
                if (this.ViewState[_SortProperties] == null)
                {
                    SortViewState ivs = new SortViewState(base.SortExpression, base.SortDirection);
                    //SortProperties = ivs;
                    return ivs;
                }
                else
                    return (SortViewState)this.ViewState[_SortProperties];
            }
            set
            {
                this.ViewState[_SortProperties] = value;
            }
        }

		public override string SortExpression
		{
			get
			{
				if (SortProperties != null && !String.IsNullOrEmpty(SortProperties.SortExpression))
					return String.Format("{0} {1}", SortProperties.SortExpression, SortProperties.SortDirection == SortDirection.Ascending ? "ASC" : "DESC");
				else
					return base.SortExpression;
			}
		}

		private EcfListViewPreferences _CurrentUserPreferences;

		/// <summary>
		/// Gets or sets the user preferences.
		/// </summary>
		/// <value>The user preferences.</value>
		public EcfListViewPreferences UserPreferences
		{
			get
			{
				if (_CurrentUserPreferences == null)
				{
					string gridSettingsKey = CMPageSettings.MakeGridSettingsKey(this.AppId, this.ViewId);
					string settings = ProfileContext.Current.Profile.PageSettings.GetSettingString(gridSettingsKey);
					if (!String.IsNullOrEmpty(settings))
						_CurrentUserPreferences = UtilHelper.JsonDeserialize<EcfListViewPreferences>(settings);
				}

				return _CurrentUserPreferences;
			}
		}
        #endregion

        #region Styles
        /// <summary>
        /// Gets or sets the table CSS class.
        /// </summary>
        /// <value>The table CSS class.</value>
        public string TableCssClass
        {
            get
            {
                return (this.ViewState[_TableCssClass] == null) ? string.Empty : (string)this.ViewState[_TableCssClass];
            }
            set
            {
                this.ViewState[_TableCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table heading row CSS class.
        /// </summary>
        /// <value>The table heading row CSS class.</value>
        public string TableHeadingRowCssClass
        {
            get
            {
                return (this.ViewState[_TableHeadingRowCssClass] == null) ? string.Empty : (string)this.ViewState[_TableHeadingRowCssClass];
            }
            set
            {
                this.ViewState[_TableHeadingRowCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table heading cell CSS class.
        /// </summary>
        /// <value>The table heading cell CSS class.</value>
        public string TableHeadingCellCssClass
        {
            get
            {
                return (this.ViewState[_TableHeadingCellCssClass] == null) ? string.Empty : (string)this.ViewState[_TableHeadingCellCssClass];
            }
            set
            {
                this.ViewState[_TableHeadingCellCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table cell CSS class.
        /// </summary>
        /// <value>The table cell CSS class.</value>
        public string TableCellCssClass
        {
            get
            {
                return (this.ViewState[_TableCellCssClass] == null) ? string.Empty : (string)this.ViewState[_TableCellCssClass];
            }
            set
            {
                this.ViewState[_TableCellCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table row CSS class.
        /// </summary>
        /// <value>The table row CSS class.</value>
        public string TableRowCssClass
        {
            get
            {
                return (this.ViewState[_TableRowCssClass] == null) ? string.Empty : (string)this.ViewState[_TableRowCssClass];
            }
            set
            {
                this.ViewState[_TableRowCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table alt row CSS class.
        /// </summary>
        /// <value>The table alt row CSS class.</value>
        public string TableAltRowCssClass
        {
            get
            {
                return (this.ViewState[_TableAltRowCssClass] == null) ? string.Empty : (string)this.ViewState[_TableAltRowCssClass];
            }
            set
            {
                this.ViewState[_TableAltRowCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the table selected row CSS class.
        /// </summary>
        /// <value>The table selected row CSS class.</value>
        public string TableSelectedRowCssClass
        {
            get
            {
                return (this.ViewState[_TableSelectedRowCssClass] == null) ? string.Empty : (string)this.ViewState[_TableSelectedRowCssClass];
            }
            set
            {
                this.ViewState[_TableSelectedRowCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the pager row CSS class.
        /// </summary>
        /// <value>The pager row CSS class.</value>
        public string PagerRowCssClass
        {
            get
            {
                return (this.ViewState[_PagerRowCssClass] == null) ? string.Empty : (string)this.ViewState[_PagerRowCssClass];
            }
            set
            {
                this.ViewState[_PagerRowCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the pager text CSS class.
        /// </summary>
        /// <value>The pager text CSS class.</value>
        public string PagerTextCssClass
        {
            get
            {
                return (this.ViewState[_PagerTextCssClass] == null) ? string.Empty : (string)this.ViewState[_PagerTextCssClass];
            }
            set
            {
                this.ViewState[_PagerTextCssClass] = value;
            }
        }

        /// <summary>
        /// Gets or sets the pager dropdown CSS class.
        /// </summary>
        /// <value>The pager dropdown CSS class.</value>
        public string PagerDropdownCssClass
        {
            get
            {
                return (this.ViewState[_PagerDropdownCssClass] == null) ? string.Empty : (string)this.ViewState[_PagerDropdownCssClass];
            }
            set
            {
                this.ViewState[_PagerDropdownCssClass] = value;
            }
        }
        #endregion

        #region --- prop: Styles ---

        #region prop: HeaderCssClass
        /// <summary>
        /// Gets or sets the header CSS class.
        /// </summary>
        /// <value>The header CSS class.</value>
        public string HeaderCssClass
        {
            get
            {
                if (ViewState["_HeaderCssClass"] == null)
                    return defaultHeaderCssClass;

                return ViewState["_HeaderCssClass"].ToString();
            }
            set { ViewState["_HeaderCssClass"] = value; }
        }
        #endregion

        #region prop: GridCssClass
        /// <summary>
        /// Gets or sets the grid CSS class.
        /// </summary>
        /// <value>The grid CSS class.</value>
        public string GridCssClass
        {
            get
            {
                if (ViewState["_GridCssClass"] == null)
                    return defaultGridCssClass;

                return ViewState["_GridCssClass"].ToString();
            }
            set { ViewState["_GridCssClass"] = value; }
        }
        #endregion

        #region prop: FooterCssClass
        /// <summary>
        /// Gets or sets the footer CSS class.
        /// </summary>
        /// <value>The footer CSS class.</value>
        public string FooterCssClass
        {
            get
            {
                if (ViewState["_FooterCssClass"] == null)
                    return defaultFooterCssClass;

                return ViewState["_FooterCssClass"].ToString();
            }
            set { ViewState["_FooterCssClass"] = value; }
        }
        #endregion

        #region prop: GridInnerCssClass
        /// <summary>
        /// Gets or sets the grid inner CSS class.
        /// </summary>
        /// <value>The grid inner CSS class.</value>
        public string GridInnerCssClass
        {
            get
            {
                if (ViewState["_GridInnerCssClass"] == null)
                    return defaultGridInnerCssClass;

                return ViewState["_GridInnerCssClass"].ToString();
            }
            set { ViewState["_GridInnerCssClass"] = value; }
        }
        #endregion

        #region prop: HeaderInnerCssClass
        /// <summary>
        /// Gets or sets the header inner CSS class.
        /// </summary>
        /// <value>The header inner CSS class.</value>
        public string HeaderInnerCssClass
        {
            get
            {
                if (ViewState["_HeaderInnerCssClass"] == null)
                    return defaultHeaderInnerCssClass;

                return ViewState["_HeaderInnerCssClass"].ToString();
            }
            set { ViewState["_HeaderInnerCssClass"] = value; }
        }
        #endregion

        #region prop: GridSelectedRowCssClass
        /// <summary>
        /// Gets or sets the grid selected row CSS class.
        /// </summary>
        /// <value>The grid selected row CSS class.</value>
        public string GridSelectedRowCssClass
        {
            get
            {
                if (ViewState["_GridSelectedRowCssClass"] == null)
                    return defaultGridSelectedRowCssClass;

                return ViewState["_GridSelectedRowCssClass"].ToString();
            }
            set { ViewState["_GridSelectedRowCssClass"] = value; }
        }
        #endregion

        #endregion

		/// <summary>
		/// Parses passed <paramref name="sortString"/> and assigns SortProperties for the view.
		/// </summary>
		/// <param name="sortString"></param>
		public void SetSortProperties(string sortString)
		{
			if (!String.IsNullOrEmpty(sortString))
			{
				int spaceIndex = sortString.Trim().IndexOf(' ');
				if (spaceIndex > 0)
				{
					SortDirection dir = SortDirection.Ascending;
					string sortDirection = sortString.Substring(spaceIndex + 1);
					if (String.Compare(sortDirection, "DESC", StringComparison.OrdinalIgnoreCase) == 0)
						dir = SortDirection.Descending;
					SortViewState svs = new SortViewState(sortString.Substring(0, spaceIndex), dir);
					SortProperties = svs;
				}
			}
		}
        #endregion

        private bool _currentViewInitialized;
        private AdminView _CurrentView;

        /// <summary>
        /// Gets the admin view.
        /// </summary>
        /// <returns></returns>
        public AdminView GetAdminView()
        {
            if (!_currentViewInitialized)
                _CurrentView = ManagementContext.Current.FindView(this.AppId, this.ViewId);

            return _CurrentView;
        }

        #region Public Methods
        /// <summary>
        /// Makes the primary key id string.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        public static string MakePrimaryKeyIdString(params string[] ids)
        {
            StringBuilder retVal = new StringBuilder();
            if (ids != null && ids.Length > 0)
            {
                for (int i = 0; i < ids.Length - 1; i++)
                    retVal.AppendFormat("{0}{1}", ids[i], ExtenderPrimaryKeyAttributeSeparator);

                retVal.Append(ids[ids.Length - 1]);
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Gets the primary key id string items.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns></returns>
        public static string[] GetPrimaryKeyIdStringItems(string primaryKey)
        {
            string[] retVal = null;
            if (!String.IsNullOrEmpty(primaryKey))
                retVal = primaryKey.Split(new string[] { ExtenderPrimaryKeyAttributeSeparator }, StringSplitOptions.None);

            return retVal;
        }

		public static int GetSavedPageSize(Page page, string viewId, int defaultValue)
		{
			int pageSize = defaultValue;
			try
			{

				//int pageSize = ProfileContext.Current.Profile.PageSettings.GetSettingInt(CMPageSettings.MakeGridPageSizeKey(viewId), pageSize);

				// read pageSize from a cookie
				HttpCookie pageSizeCookie = page.Request.Cookies[CMPageSettings.MakeGridPageSizeKey(viewId)];
				if (pageSizeCookie != null)
					if (!Int32.TryParse(pageSizeCookie.Value, out pageSize))
						pageSize = defaultValue;
			}
			catch { }

			return pageSize;
		}

		public static void SavePageSize(Page page, string viewId, int newSize)
		{
			try
			{
				//ProfileContext.Current.Profile.PageSettings.SetSettingInt(CMPageSettings.MakeGridPageSizeKey(viewId), newSize);
				//ProfileContext.Current.Profile.Save();

				// save pageSize in a cookie
				HttpCookie pageSizeCookie = page.Request.Cookies[CMPageSettings.MakeGridPageSizeKey(viewId)];
				if (pageSizeCookie == null)
					pageSizeCookie = new HttpCookie(CMPageSettings.MakeGridPageSizeKey(viewId));

				pageSizeCookie.Value = newSize.ToString();
				pageSizeCookie.Expires = DateTime.Now.AddYears(1);
				//pageSizeCookie.Domain = page.Request.
				HttpContext.Current.Response.Cookies.Set(pageSizeCookie);
			}
			catch { }
		}

		public static SortViewState GetSavedSorting(Page page, string viewId, SortViewState defaultValue)
		{
			SortViewState sorting = defaultValue;

			if (sorting == null)
				sorting = new SortViewState(String.Empty, SortDirection.Ascending);

			try
			{
				string sortingKey = viewId + "-sorting";
				string directionSortingKey = sortingKey + "_direction";
				string expressionSortingKey = sortingKey + "_expression";

			    // read sorting from a cookie
			    HttpCookie sortingCookie = page.Request.Cookies[sortingKey];
				if (sortingCookie != null)
				{
					try
					{
						// get direction
						string[] values = sortingCookie.Values.GetValues(directionSortingKey);
						if (values != null && values.Length > 0)
							sorting.SortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), values[0]);

						// get expression
						values = sortingCookie.Values.GetValues(expressionSortingKey);
						if (values != null && values.Length > 0)
							sorting.SortExpression = values[0];
					}
					catch 
					{
						sorting = defaultValue;
					}
				}
			}
			catch { }

			return sorting;
		}

		public static void SaveSorting(Page page, string viewId, SortViewState newValue)
		{
			try
			{
				string sortingKey = viewId + "-sorting";
				string directionSortingKey = sortingKey + "_direction";
				string expressionSortingKey = sortingKey + "_expression";

				// save sorting in a cookie
				HttpCookie sortingCookie = page.Request.Cookies[sortingKey];
				if (sortingCookie == null)
					sortingCookie = new HttpCookie(sortingKey);

				sortingCookie.Values.Set(directionSortingKey, newValue.SortDirection.ToString());
				sortingCookie.Values.Set(expressionSortingKey, newValue.SortExpression);
				sortingCookie.Expires = DateTime.Now.AddYears(1);
				HttpContext.Current.Response.Cookies.Set(sortingCookie);
			}
			catch { }
		}

		public static string MakeSortExpression(SortViewState sorting)
		{
			string direction = sorting.SortDirection == SortDirection.Descending ? "DESC" : "ASC";
			return String.Format("{0} {1}", sorting.SortExpression, direction);
		}
        #endregion

        #region Event Handlers
        /// <summary>
        /// Raises the <see cref="E:LayoutCreated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLayoutCreated(EventArgs e)
        {
            // apply styles
            HtmlTable mainTable = this.FindControl(DefaultInnerTableId) as HtmlTable;
            if (mainTable != null)
                mainTable.Attributes["class"] = this.TableCssClass;

            HtmlTableRow headerTRow = this.FindControl(DefaultTableHeaderId) as HtmlTableRow;
            if (headerTRow != null)
                headerTRow.Attributes["class"] = this.TableHeadingRowCssClass;

            int colCount = BindHeader(headerTRow);

            HtmlTableCell footerCell = this.FindControl(DefaultFooterCellId) as HtmlTableCell;
            if (footerCell != null)
                footerCell.ColSpan = colCount;

            HtmlGenericControl footer = this.FindControl(DefaultFooterDivId) as HtmlGenericControl;
            if (footer != null)
            {
                footer.Attributes["class"] = this.PagerRowCssClass;
                footer.Attributes.CssStyle["width"] = "100%";
            }

            DropDownList paging = this.FindControl(DefaultPagingDropdownId) as DropDownList;
            if (paging != null)
                paging.CssClass = this.PagerDropdownCssClass;

            base.OnLayoutCreated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.WebControls.BaseDataBoundControl.DataBound"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnDataBound(EventArgs e)
        {
            if (!this.ShowPaging)
            {
                // hide paging
                HtmlGenericControl footerDiv = this.FindControl(DefaultFooterDivId) as HtmlGenericControl;
                if (footerDiv != null)
                    footerDiv.Visible = false;
            }

            // adjust selected value in dropdown
            DropDownList paging = this.FindControl(DefaultPagingDropdownId) as DropDownList;
            if (paging != null)
            {
                if (!this.ShowPaging)
                    ManagementHelper.SelectListItem(paging, "-1");
                else
                    if (!Page.IsPostBack)
                    {
                        int pageSize = EcfListView.GetSavedPageSize(this.Page, this.ViewId, Math.Max(this.MaximumRows, DefaultPageSize));
                        ManagementHelper.SelectListItem(paging, pageSize.ToString());
                        this.CurrentPageSize = pageSize;
                    }
            }

            base.OnDataBound(e);
        }

        /// <summary>
        /// Creates the child controls.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="dataBinding">if set to <c>true</c> [data binding].</param>
        /// <returns></returns>
        protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
        {
            if (dataSource != null && dataSource.GetEnumerator().MoveNext())
                this.InsertItemPosition = InsertItemPosition.None;
            else
                this.InsertItemPosition = InsertItemPosition.FirstItem;

            if (dataSource == null)
                return base.CreateChildControls(dataSource, false);
            else
                return base.CreateChildControls(dataSource, dataBinding);
        }

        /// <summary>
        /// Raises the <see cref="E:ItemCreated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewItemEventArgs"/> instance containing the event data.</param>
        protected override void OnItemCreated(ListViewItemEventArgs e)
        {
            // cannot bind rows without information about rows configuration
            AdminView adminView = GetAdminView();
            if (adminView == null || adminView.Columns.Count <= 0)
                return;

            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlTableRow tRow = FindItemRow(e.Item);
                if (tRow != null)
                {
                    ListViewDataItem lvDataItem = (ListViewDataItem)e.Item;
                    if (lvDataItem != null && lvDataItem.DataItem != null)
                    {
                        //DataRow dataRow = lvDataItem.DataItem as DataRow;
                        //if (dataRow != null)
                        //{
                        // add data cells
                        BindDataRows(adminView, tRow, lvDataItem.DataItem, lvDataItem.DataItemIndex);
                        //}
                    }
                }
            }

            base.OnItemCreated(e);
        }

        /// <summary>
        /// Raises the <see cref="E:Sorting"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
        protected override void OnSorting(ListViewSortEventArgs e)
        {
            SortDirection sortDirection = e.SortDirection;

            // determine sortDirection
            if (this.SortProperties != null && !String.IsNullOrEmpty(this.SortProperties.SortExpression) &&
                String.Compare(this.SortProperties.SortExpression, e.SortExpression, true) == 0)
                sortDirection = this.SortProperties.SortDirection == SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;

            e.SortDirection = sortDirection;

            // Check the sort direction to set the image URL accordingly.
            string imgUrl;
            if (sortDirection == SortDirection.Ascending)
                imgUrl = this.ImagesBaseUrl + EcfListView.DefaultImageUrlAscending;
            else
                imgUrl = this.ImagesBaseUrl + EcfListView.DefaultImageUrlDescending;

            SortViewState ivs = this.SortProperties;
            if (ivs != null)
                ivs = new SortViewState(e.SortExpression, sortDirection);
            else
            {
                ivs.SortExpression = e.SortExpression;
                ivs.SortDirection = sortDirection;
            }
            this.SortProperties = ivs;

            AdminView adminView = this.GetAdminView();
            if (adminView == null)
                return;

			// save sorting proterties
			SaveSorting(this.Page, this.ViewId, this.SortProperties);

            Thread.Sleep(800);

            // add image to the column that is being sorted, hide images for all other columns
            for (int i = 0; i < adminView.Columns.Count; i++)
            {
                if (!adminView.Columns[i].Visible)
                    continue;

                ViewColumn column = adminView.Columns[i];
                if (column.AllowSorting)
                {
                    if (String.Compare(column.DataField, e.SortExpression, true) == 0) // this column is being sorted
                    {
                        // add asc/desc image to the header
                        Image img = this.FindControl(GetHeaderSortImageId(column)) as Image;
                        if (img != null)
                        {
                            img.ImageUrl = imgUrl;
                            img.Visible = true;
                        }
                    }
                    else
                    {
                        // remove asc/desc images from the header
                        Image img = this.FindControl(GetHeaderSortImageId(column)) as Image;
                        if (img != null)
                            img.Visible = false;
                    }
                }
            }

            base.OnSorting(e);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Searches for HtmlTableRow in which item data should be loaded
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private HtmlTableRow FindItemRow(ListViewItem item)
        {
            // loop through item's controls collection
            for (int i = 0; i < item.Controls.Count; i++)
            {
                // found row; return it
                if (item.Controls[i].GetType() == typeof(HtmlTableRow))
                    return (HtmlTableRow)item.Controls[i];
            }
            return null;
        }

        /// <summary>
        /// Creates CheckBox control that can be added to a table cell
        /// </summary>
        /// <param name="chbId">The CHB id.</param>
        /// <param name="addGridAttr">if set to <c>true</c> [add grid attr].</param>
        /// <returns></returns>
        private CheckBox CreateCheckBox(string chbId, bool addGridAttr)
        {
            CheckBox chb = new CheckBox();
            chb.AutoPostBack = false;
            chb.ID = String.Format("{0}_cb", chbId);
            if (addGridAttr)
                chb.InputAttributes[ExtenderCheckboxGridAttributeName] = "1";
            return chb;
        }

        private string GetHeaderSortImageId(ViewColumn column)
        {
            return String.Concat(EcfListView.DefaultSortImageControlPrefix, "_", column.GetColumnUniqueId());
        }

        /// <summary>
        /// BindHeader
        /// </summary>
        /// <param name="headerRow">The header row.</param>
        /// <returns>ColumnCount</returns>
        private int BindHeader(HtmlTableRow headerRow)
        {
            AdminView view = this.GetAdminView();
            if (view == null)
                return 0;

            int colCount = 0;

			// Bind Header
            headerRow.Controls.Clear();

            foreach (ViewColumn viewColumn in view.Columns)
            {
                if (!viewColumn.Visible)
                    continue;

                colCount++;

                if (viewColumn.ColumnType == ColumnType.CheckBox)
                {
                    // show checkboxes
                    this.ShowCheckboxes = true;
                    continue;
                }

                HtmlTableCell cell = new HtmlTableCell("th");
                cell.Attributes["class"] = this.TableHeadingCellCssClass;

                if (viewColumn.AllowSorting && viewColumn.ColumnType != ColumnType.Action)
                {
                    LinkButton lnkButton = new LinkButton();
                    lnkButton.ID = "btn" + "_" + viewColumn.GetColumnUniqueId();
                    lnkButton.CommandName = "Sort";
                    lnkButton.CommandArgument = viewColumn.GetSortExpression();
                    lnkButton.Text = viewColumn.GetLocalizedHeadingString();

                    cell.Controls.Add(lnkButton);
                }
                else
                {
                    Label lbl = new Label();
                    lbl.ID = "lbl" + viewColumn.GetColumnUniqueId();
                    lbl.Text = viewColumn.GetLocalizedHeadingString();
                    cell.Controls.Add(lbl);
                }

                #region Add sorting controls
                if (viewColumn.AllowSorting)
                {
                    SortViewState sortViewState = this.SortProperties;
                    if (sortViewState != null)
                    {
                        Image image = new Image();
                        image.ID = GetHeaderSortImageId(viewColumn);
                        image.BorderStyle = BorderStyle.None;
                        image.ImageUrl = sortViewState.GetImageUrl(this);
                        if (sortViewState.SortDirection == SortDirection.Ascending)
                        {
                            image.ToolTip = "Ascending";
                            image.AlternateText = "Ascending";
                        }
                        else
                        {
                            image.ToolTip = "Descending";
                            image.AlternateText = "Descending";
                        }

                        image.ImageAlign = ImageAlign.AbsMiddle;
                        image.Visible = String.Compare(sortViewState.SortExpression, viewColumn.DataField, true) == 0 ? true : false;

                        cell.Controls.Add(image);
                    }
                }
                #endregion

				string width = String.Empty;
				if (UserPreferences != null && UserPreferences.ColumnProperties[viewColumn.ColumnIndex.ToString()] != null)
					width = UserPreferences.ColumnProperties[viewColumn.ColumnIndex.ToString()] + "px";
				else
					width = viewColumn.Width + "px";
				cell.Attributes.CssStyle["width"] = width;
                headerRow.Controls.Add(cell);
            }

            if (this.ShowCheckboxes)
            {
                // create checkbox in the first column
                CheckBox chb = new CheckBox();
                chb.AutoPostBack = false;
                if (this.DataKeyNames == null || this.DataKeyNames.Length == 0)
                    chb.ID = String.Format("{0}_cb", "header");
                else
                    chb.ID = String.Format("{0}_cb", this.DataKeyNames[0]);

                chb.InputAttributes["onclick"] = "ibn_serverGridCheckboxHandler(this)"; // onclick handler that selects all checkboxes in a column

                HtmlTableCell chbCell = new HtmlTableCell("th");
                chbCell.Controls.Add(chb);
                chbCell.Attributes["class"] = this.TableHeadingCellCssClass;
                chbCell.Attributes.CssStyle["width"] = EcfListView.DefaultCheckboxColumnWidth.ToString() + "px";
                headerRow.Controls.AddAt(0, chbCell);
            }

            headerRow.Attributes.CssStyle["height"] = "20px";

            return colCount;
        }

        /// <summary>
        /// Binds the data rows.
        /// </summary>
        /// <param name="adminView">The admin view.</param>
        /// <param name="tableRow">The table row.</param>
        /// <param name="dataItem">The data item.</param>
        /// <param name="currentIndex">Index of the current.</param>
        private void BindDataRows(AdminView adminView, HtmlTableRow tableRow, object dataItem, int currentIndex)
        {
            CultureInfo ci = ManagementContext.Current.ConsoleUICulture;

            // loop through all rows specified in config
            ViewColumnCollection columns = adminView.Columns;
            for (int iRow = 0; iRow < columns.Count; iRow++)
            {
                if (!columns[iRow].Visible)
                    continue;

                HtmlTableCell dataCell = new HtmlTableCell();
                dataCell.Attributes["class"] = this.TableCellCssClass;

                string width = String.Empty;

                if (columns[iRow].ColumnType == ColumnType.CheckBox)
                    width = EcfListView.DefaultCheckboxColumnWidth.ToString();
                else
                {
					if (UserPreferences != null && UserPreferences.ColumnProperties[columns[iRow].ColumnIndex.ToString()] != null)
						width = (string)UserPreferences.ColumnProperties[columns[iRow].ColumnIndex.ToString()];
					else
					{
						int iWidth = DefaultColumnWidth;
						if (!Int32.TryParse(columns[iRow].Width, out iWidth))
							iWidth = EcfListView.DefaultColumnWidth;
						width = iWidth.ToString();
					}
                }
				
                dataCell.Attributes.CssStyle["width"] = width + "px";

                bool addCheckBox = false; // true, if currently added item is checkbox

                // primary key string for adding to <tr> and <td> (which have checkboxes)
                string primaryKeyString = GetRowPrimaryKeyString(dataItem);

                // bind column depending on column type
                if (columns[iRow].ColumnType == ColumnType.CustomTemplate)
                {
                    // bind template
                    string url = columns[iRow].Template.ControlUrl;
                    if (!url.StartsWith("~"))
                        url = String.Format("~/apps/{0}/{1}", adminView.Module.Name, url);

                    Control ctrl = Page.LoadControl(url);
                    if (ctrl != null)
                    {
                        ((IEcfListViewTemplate)ctrl).DataItem = dataItem;
                        dataCell.Controls.Add(ctrl);
                        ctrl.DataBind();
                    }
                }
                else if (columns[iRow].ColumnType == ColumnType.CheckBox)
                {
                    CheckBox chb = null;

					bool checkboxEnabled = true;

					// enable/disable checkbox (default value - enabled)
					if (columns[iRow].Attributes[_checkboxEnableDataField] != null)
					{
						string enableCheckboxKey = columns[iRow].Attributes[_checkboxEnableDataField].ToString();
						object val = DataBinder.Eval(dataItem, GetEvalString(dataItem, enableCheckboxKey));
						if (val != null && val.GetType() == typeof(Boolean) && !(bool)val)
							checkboxEnabled = false;
					}

					if (this.DataKeyNames == null || this.DataKeyNames.Length == 0)
						chb = CreateCheckBox(currentIndex.ToString(), checkboxEnabled /*don't allow to check this checkbox if it's disabled*/);
                    else
                        chb = CreateCheckBox(DataBinder.Eval(dataItem, GetEvalString(dataItem, this.DataKeyNames[0])).ToString(), checkboxEnabled);

					chb.Enabled = checkboxEnabled;
                    dataCell.Align = "middle";
                    dataCell.Controls.Add(chb);
                    dataCell.Attributes[ExtenderPrimaryKeyAttributeName] = primaryKeyString;
                    addCheckBox = true;
                }
                else if (columns[iRow].ColumnType == ColumnType.DropDown)
                {
                }
                else if (columns[iRow].ColumnType == ColumnType.Text || columns[iRow].ColumnType == ColumnType.None)
                {
                    Label lblData = new Label();
                    object val = DataBinder.Eval(dataItem, GetEvalString(dataItem, columns[iRow].DataField));
                    lblData.ID = "lbl" + "_" + columns[iRow].GetColumnUniqueId();
                    lblData.Text = GetDataCellValue(val, columns[iRow].FormatString, ci);
                    lblData.Attributes["width"] = "100%";
                    dataCell.Controls.Add(lblData);
                }
                else if (columns[iRow].ColumnType == ColumnType.DateTime)
                {
                    Label lblData = new Label();
                    object val = DataBinder.Eval(dataItem, GetEvalString(dataItem, columns[iRow].DataField));
                    lblData.ID = "lbl" + "_" + columns[iRow].GetColumnUniqueId();

                    if (val is DateTime)
                    {
                        lblData.Text = GetDataCellValue(ManagementHelper.FormatDateTime((DateTime)val), columns[iRow].FormatString, ci);
                        lblData.ToolTip = ManagementHelper.FormatAgoDateTime((DateTime)val);
                    }
                    else
                    {
                        lblData.Text = GetDataCellValue(val, columns[iRow].FormatString, ci);
                    }

                    lblData.Attributes["width"] = "100%";
                    dataCell.Controls.Add(lblData);
                }
                else if (columns[iRow].ColumnType == ColumnType.HyperLink)
                {
                    // bind hyperlink field
                    HyperLink hyperLink = new HyperLink();
                    object val = String.Empty;

                    if (!String.IsNullOrEmpty(columns[iRow].DataField))
                        val = DataBinder.Eval(dataItem, GetEvalString(dataItem, columns[iRow].DataField));

                    if (columns[iRow].Attributes.Contains("id"))
                        hyperLink.ID = columns[iRow].Attributes["id"].ToString() + columns[iRow].ColumnIndex;
                    else
                        hyperLink.ID = "link" + "_" + columns[iRow].GetColumnUniqueId();

                    if (columns[iRow].Attributes.Contains(_navigateUrlKey))
                    {
                        hyperLink.NavigateUrl = columns[iRow].Attributes[_navigateUrlKey].ToString();
                    }
                    else
                    {
                        if (columns[iRow].Attributes.Contains(_dataNavigateUrlFormatString) && columns[iRow].Attributes.Contains(_dataNavigateUrlFields))
                        {
                            hyperLink.NavigateUrl = BindAndFormat(dataItem, columns[iRow].Attributes[_dataNavigateUrlFormatString].ToString(), columns[iRow].Attributes[_dataNavigateUrlFields].ToString().Split(new char[] { ',' }));
                        }
                    }

                    if (columns[iRow].Attributes.Contains(_dataTextFormatString) && columns[iRow].Attributes.Contains(_dataTextFields))
                    {
                        hyperLink.Text = BindAndFormat(dataItem, columns[iRow].Attributes[_dataTextFormatString].ToString(), columns[iRow].Attributes[_dataTextFields].ToString().Split(new char[] { ',' }));
                    }
                    else
                    {
                        hyperLink.Text = GetDataCellValue(val);
                    }

                    hyperLink.Attributes["width"] = "100%";
                    dataCell.Controls.Add(hyperLink);
                }
                else if (columns[iRow].ColumnType == ColumnType.Action)
                {
                    // bind action field
                    for (int actionIndex = 0; actionIndex < columns[iRow].Actions.Count; actionIndex++)
                    {
                        ViewColumnAction currentAction = columns[iRow].Actions[actionIndex];

                        ImageButton imgButton = new ImageButton();
                        imgButton.ID = "ib" + Guid.NewGuid().ToString();
                        imgButton.ImageAlign = ImageAlign.Middle;
                        imgButton.ImageUrl = Page.ResolveClientUrl(currentAction.ImageUrl);

                        #region add command to the ImageButton
                        imgButton.CommandName = "Custom";
                        imgButton.ToolTip = currentAction.ToolTip; // TODO: get tooltip from resources

                        #region make command arguments
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        // add primary key
                        dic.Add(GridCommandParameterName, Boolean.TrueString);
                        dic.Add("primaryKeyId", primaryKeyString);
                        dic.Add("permissions", currentAction.Attributes.Contains("permissions") ? currentAction.Attributes["permissions"].ToString() : String.Empty);
                        // add other parameters to the dictionary
                        if (!String.IsNullOrEmpty(currentAction.CommandParameters))
                        {
                            string[] cmdParamsArray = currentAction.CommandParameters.Split(new char[] { ',' });
                            if (cmdParamsArray != null && cmdParamsArray.Length > 0)
                                foreach (string sName in cmdParamsArray)
                                {
                                    object val = DataBinder.Eval(dataItem, GetEvalString(dataItem, sName));
                                    if (val != null)
                                        dic.Add(sName, val.ToString());
                                }
                        }
                        #endregion

                        // add command to the imageButton
                        CommandParameters cp = new CommandParameters(currentAction.CommandName, dic);
                        string clientScript = "";
                        bool enabled = true;
                        if (!String.IsNullOrEmpty(currentAction.CommandName))
                            clientScript = CommandManager.GetCurrent(this.Page).AddCommand(String.Empty, this.AppId, this.ViewId, cp, out enabled);
                        imgButton.OnClientClick = String.Format("{0} return false;", clientScript);
                        #endregion

                        if (actionIndex != columns[iRow].Actions.Count - 1)
                        {
                            // add space between images, do not add space after the last image
                            imgButton.Style.Add(HtmlTextWriterStyle.PaddingRight, "5px");
                        }

                        if (enabled)
                            dataCell.Controls.Add(imgButton);
                    }

                    dataCell.Align = "middle";
                    if (dataCell.Controls.Count == 0)
                    {
                        // add space to dataCell
                        dataCell.InnerHtml = "&nbsp;";
                    }
                }

                // add primary key to <tr> and <td>
                tableRow.Attributes[ExtenderPrimaryKeyAttributeName] = primaryKeyString;

                // add current cell to row
                if (addCheckBox)
                    tableRow.Controls.AddAt(0, dataCell);
                else
                    tableRow.Controls.Add(dataCell);
            }
        }

        /// <summary>
        /// Binds and format.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <param name="format">The format.</param>
        /// <param name="fields">The fields.</param>
        /// <returns></returns>
        private string BindAndFormat(object dataItem, string format, string[] fields)
        {
            if (String.IsNullOrEmpty(format) || fields.Length == 0)
                return String.Empty;

            object[] dataValues = new object[fields.Length];
            for (int j = 0; j < fields.Length; j++)
            {
                if (fields[j] != null)
                {
                    dataValues[j] = DataBinder.Eval(dataItem, GetEvalString(dataItem, fields[j]));
                }
            }

            return String.Format(format, dataValues);
        }

        /// <summary>
        /// Gets the data cell value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string GetDataCellValue(object val)
        {
            return (val == null || val == DBNull.Value || String.IsNullOrEmpty(val.ToString())) ? "&nbsp;" : val.ToString();
        }

        /// <summary>
        /// Gets formatted data cell value.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="format">Format string.</param>
        /// <returns></returns>
        public static string GetDataCellValue(object val, string format, CultureInfo ci)
        {
            if (String.IsNullOrEmpty(format))
                return GetDataCellValue(val);
            else
                return (val == null || val == DBNull.Value || String.IsNullOrEmpty(val.ToString())) ?
                    "&nbsp;" : String.Format(ci, format, val);
        }

        /// <summary>
        /// Makes string that can be used in DataBinder.Eval function for the specified dataItem.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public static string GetEvalString(object dataItem, string field)
        {
            string retVal = String.Empty;
            if (NeedBrackets(dataItem))
                retVal = String.Concat("[", field, "]");
            else
                retVal = field;

            return retVal;
        }

        /// <summary>
        /// Needs the brackets.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
        private static bool NeedBrackets(object dataItem)
        {
            if (dataItem != null && dataItem.GetType() == typeof(DataRow))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the row primary key string.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
        private string GetRowPrimaryKeyString(object dataItem)
        {
            StringBuilder retVal = new StringBuilder();

            if (!string.IsNullOrEmpty(this.PrimaryKeyId))
            {
                string[] fieldNames = this.PrimaryKeyId.Split(new string[] { ExtenderPrimaryKeyAttributeSeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (fieldNames != null && fieldNames.Length > 0)
                {
                    for (int i = 0; i < fieldNames.Length - 1; i++)
                        retVal.AppendFormat("{0}{1}", DataBinder.Eval(dataItem, GetEvalString(dataItem, fieldNames[i])).ToString(), ExtenderPrimaryKeyAttributeSeparator);

                    retVal.Append(DataBinder.Eval(dataItem, GetEvalString(dataItem, fieldNames[fieldNames.Length - 1])).ToString());
                }
            }

            return retVal.ToString();
        }
        #endregion

        #region GetCheckedCollection
        /// <summary>
        /// Inners the get checked collection.
        /// </summary>
        /// <returns></returns>
        protected string InnerGetCheckedCollection()
        {
            //List<string> checkedItems = new List<string>();
            StringBuilder sb = new StringBuilder();

            if (!this.ShowCheckboxes)
                return String.Empty;

            if (this.Items.Count > 0)
            {
                // loop through all rows
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].ItemType == ListViewItemType.DataItem)
                    {
                        if (this.Items[i].Controls.Count == 0)
                            continue;

                        // get HtmlTableRow by ListViewItem
                        HtmlTableRow row = this.FindItemRow(this.Items[i]);

                        // get checkbox in the row
                        if (row != null && row.Cells.Count > 0 &&
                            row.Attributes[EcfListView.ExtenderPrimaryKeyAttributeName] != null)
                        {
                            // look for checkbox in the first cell
                            for (int ctrlIndex = 0; ctrlIndex < row.Cells[0].Controls.Count; ctrlIndex++)
                            {
                                CheckBox chb = row.Cells[EcfListView.ColumnWithCheckboxes].Controls[ctrlIndex] as CheckBox;
                                //row.Cells.AsQueryable()
                                // if checkbox is found, get its checked value
                                if (chb != null)
                                {
                                    if (chb.Checked && chb.InputAttributes[EcfListView.ExtenderCheckboxGridAttributeName] != null)
                                        sb.AppendFormat("{0}{1}", row.Attributes[EcfListView.ExtenderPrimaryKeyAttributeName], EcfListView.PrimaryKeysListSeparator);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the checked collection.
        /// </summary>
        /// <returns></returns>
        public string[] GetCheckedCollection()
        {
            string[] retVal = null;

            string checkedCollectionString = InnerGetCheckedCollection();
            if (!String.IsNullOrEmpty(checkedCollectionString))
                retVal = checkedCollectionString.Split(new string[] { EcfListView.PrimaryKeysListSeparator }, StringSplitOptions.RemoveEmptyEntries);

            return retVal;
        }

        /// <summary>
        /// Returns collection of checked items (primary keys)
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="lvId">The lv id.</param>
        /// <returns></returns>
        public static string[] GetCheckedCollection(Page currentPage, string lvId)
        {
            EcfListView ecfLV = null;
            ecfLV = ManagementHelper.GetControlFromCollection<EcfListView>(currentPage.Controls, lvId);

            if (ecfLV != null)
                return ecfLV.GetCheckedCollection();
            else
                return null;
        }
        #endregion

        #region GetCheckedDataItemCollection
        /// <summary>
        /// Inners the get checked collection.
        /// </summary>
        /// <returns></returns>
        protected ListViewDataItem[] InnerGetCheckedDataItemCollection()
        {
            List<ListViewDataItem> items = new List<ListViewDataItem>();

            if (!this.ShowCheckboxes)
                return null;

            if (this.Items.Count > 0)
            {
                // loop through all rows
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].ItemType == ListViewItemType.DataItem)
                    {
                        if (this.Items[i].Controls.Count == 0)
                            continue;

                        // get HtmlTableRow by ListViewItem
                        HtmlTableRow row = this.FindItemRow(this.Items[i]);

                        // get checkbox in the row
                        if (row != null && row.Cells.Count > 0 &&
                            row.Attributes[EcfListView.ExtenderPrimaryKeyAttributeName] != null)
                        {
                            // look for checkbox in the first cell
                            for (int ctrlIndex = 0; ctrlIndex < row.Cells[0].Controls.Count; ctrlIndex++)
                            {
                                CheckBox chb = row.Cells[EcfListView.ColumnWithCheckboxes].Controls[ctrlIndex] as CheckBox;
                                //row.Cells.AsQueryable()
                                // if checkbox is found, get its checked value
                                if (chb != null)
                                {
                                    if (chb.Checked && chb.InputAttributes[EcfListView.ExtenderCheckboxGridAttributeName] != null)
                                        items.Add(this.Items[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return items.ToArray();
        }

        /// <summary>
        /// Returns collection of checked items (data items)
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="lvId">The lv id.</param>
        /// <returns></returns>
        public static ListViewDataItem[] GetCheckedDataItemCollection(Page currentPage, string lvId)
        {
            EcfListView ecfLV = null;
            ecfLV = ManagementHelper.GetControlFromCollection<EcfListView>(currentPage.Controls, lvId);

            if (ecfLV != null)
                return ecfLV.InnerGetCheckedDataItemCollection();
            else
                return null;
        }
        #endregion
    }

    [Serializable]
    public class SortViewState : ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortViewState"/> class.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
        /// <param name="sortDirection">The sort direction.</param>
        public SortViewState(string sortExpression, SortDirection sortDirection)
        {
            _sortExpression = sortExpression;
            _sortDirection = sortDirection;
        }

        string _sortExpression;
        SortDirection _sortDirection;

        /// <summary>
        /// Gets or sets the sort expression.
        /// </summary>
        /// <value>The sort expression.</value>
        public string SortExpression
        {
            get { return _sortExpression; }
            set { _sortExpression = value; }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        public SortDirection SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <returns></returns>
        public string GetImageUrl(EcfListView grid)
        {
            return String.Concat(grid.ImagesBaseUrl, this.SortDirection == SortDirection.Descending ? grid.ImageUrlDescending : grid.ImageUrlAscending);
        }

		#region ISerializable Members
        /// <summary>
		/// Initializes a new instance of the <see cref="CMPageSettings"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		protected SortViewState(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

			try
			{
				_sortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), info.GetString("SortDirection"));
			}
			catch
			{
				_sortDirection = SortDirection.Ascending;
			}

			_sortExpression = info.GetString("SortExpression");
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

			info.AddValue("SortDirection", _sortDirection.ToString());
			info.AddValue("SortExpression", _sortExpression);
        }
        #endregion
    }

	/// <summary>
	/// Object for passing as a contextKey for grid header extender
	/// </summary>
	[Serializable]
	public class EcfListViewPreferences
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

		private int _pageSize;

		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		private string _sortExpression;

		public string SortExpression
		{
			get { return _sortExpression; }
			set { _sortExpression = value; }
		}

		IDictionary _columnProperties = new ListDictionary();

		/// <summary>
		/// Keys - string (column index), values - string (width)
		/// </summary>
		public IDictionary ColumnProperties
		{
			get { return _columnProperties; }
			set { _columnProperties = value; }
		}

		#region .ctor
		public EcfListViewPreferences()
		{
		}

		public EcfListViewPreferences(string appId, string viewId)
		{
			this.AppId = appId;
			this.ViewId = viewId;
		}
		#endregion
	}
}