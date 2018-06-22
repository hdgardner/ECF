using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaUI.Modules.SelectControls
{
	public partial class SelectMultiReference : System.Web.UI.UserControl
	{
		protected MetaFieldType mft = null;

		#region TypeName
		private string _typeName = "";
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
		public string TypeName
		{
			get { return _typeName; }
			set { _typeName = value; }
		}
		#endregion

		#region RefreshButton
		private string _refreshButton = String.Empty;
        /// <summary>
        /// Gets or sets the refresh button.
        /// </summary>
        /// <value>The refresh button.</value>
		public string RefreshButton
		{
			get { return _refreshButton; }
			set { _refreshButton = value; }
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadRequestVariables();
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region Events
        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.ddClasses.SelectedIndexChanged += new EventHandler(ddClasses_SelectedIndexChanged);

			this.grdMain.SortCommand += new DataGridSortCommandEventHandler(grdMain_SortCommand);
			this.btnSearch.Click += new EventHandler(btnSearch_Click);
			this.grdMain.PageIndexChanged += new DataGridPageChangedEventHandler(grdMain_PageIndexChanged);
		}

        /// <summary>
        /// Handles the PageIndexChanged event of the grdMain control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridPageChangedEventArgs"/> instance containing the event data.</param>
		protected void grdMain_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			ViewState["SelectItem_CurrentPage"] = e.NewPageIndex;
			BindGrid();
		}

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			BindGrid();
		}

        /// <summary>
        /// Handles the SortCommand event of the grdMain control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridSortCommandEventArgs"/> instance containing the event data.</param>
		protected void grdMain_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (ViewState["SelectItem_Sort"].ToString() == e.SortExpression)
				ViewState["SelectItem_Sort"] += " DESC";
			else
				ViewState["SelectItem_Sort"] = e.SortExpression;
			ViewState["SelectItem_CurrentPage"] = 0;
			BindGrid();
		}
		#endregion

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddClasses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddClasses_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}

		#region LoadRequestVariables
        /// <summary>
        /// Loads the request variables.
        /// </summary>
		private void LoadRequestVariables()
		{
			if (Request.QueryString["type"] != null)
			{
				TypeName = Request.QueryString["type"];
				mft = MetaDataWrapper.GetTypeByName(TypeName);
			}

			if (Request.QueryString["btn"] != null)
			{
				RefreshButton = Request.QueryString["btn"];
			}
		}
		#endregion

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			secHeader.Title = "Select Item";

			if (RefreshButton != String.Empty)	// Dialog Mode
			{
				string url = "javascript:window.close();";
                secHeader.AddImageLink(Page.ResolveClientUrl("~/Apps/MetaDataBase/images/close.gif"), "Close", url);
			}
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			lblTypeName.Text = CHelper.GetResFileString(mft.FriendlyName);

			ddClasses.Items.Clear();
			foreach (MetaClass mc in MultiReferenceType.GetAvailableReferences(mft))
			{
				ddClasses.Items.Add(new ListItem(mc.FriendlyName, mc.Name));
			}

			if (ddClasses.SelectedItem != null)
				BindGrid();
		}
		#endregion

		#region BindGrid
        /// <summary>
        /// Binds the grid.
        /// </summary>
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));

			string sSearch = tbSearchString.Text.Trim();

			MetaClass mc = MetaDataWrapper.GetMetaClassByName(ddClasses.SelectedValue);
			if (mc == null)
				return;

			MetaObject[] list;
			if (sSearch == String.Empty)
			{
				list = MetaObject.List(mc);
			}
			else
			{
				FilterElement filter = new FilterElement(mc.TitleFieldName, FilterElementType.Like, "%" + sSearch + "%");
				list = MetaObject.List(mc, filter);
			}

			foreach (MetaObject obj in list)
			{
				DataRow row = dt.NewRow();
				row["Id"] = obj.PrimaryKeyId;
				row["Name"] = obj.Properties[mc.TitleFieldName].Value.ToString();
				dt.Rows.Add(row);
			}

			if (ViewState["SelectItem_Sort"] == null)
				ViewState["SelectItem_Sort"] = "Name";
			if (ViewState["SelectItem_CurrentPage"] == null)
				ViewState["SelectItem_CurrentPage"] = 0;
			if (dt.Rows != null && dt.Rows.Count < grdMain.PageSize)
				grdMain.AllowPaging = false;
			else
				grdMain.AllowPaging = true;
			DataView dv = dt.DefaultView;
			dv.Sort = ViewState["SelectItem_Sort"].ToString();
			if (ViewState["SelectItem_CurrentPage"] != null && grdMain.AllowPaging)
				grdMain.CurrentPageIndex = (int)ViewState["SelectItem_CurrentPage"];
			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (DataGridItem dgi in grdMain.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibRelate");
				if (ib != null)
				{
					string sId = ddClasses.SelectedValue + "_" + dgi.Cells[0].Text;
					string sAction = CHelper.GetCloseRefreshString(RefreshButton.Replace("xxx", sId));
					ib.Attributes.Add("onclick", sAction);
				}
			}
		}
		#endregion
	}
}