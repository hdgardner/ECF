using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MultiReferenceTypeList : System.Web.UI.UserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("FriendlyName", typeof(string));
			dt.Columns.Add("IsUsed", typeof(bool));

			foreach (MetaFieldType mfType in DataContext.Current.MetaModel.GetRegisteredTypes(McDataType.MultiReference))
			{
				DataRow row = dt.NewRow();
				row["Name"] = mfType.Name;
				row["FriendlyName"] = CHelper.GetResFileString(mfType.FriendlyName);
				row["IsUsed"] = MetaFieldType.IsUsed(mfType);
				dt.Rows.Add(row);
			}
			DataView dv = dt.DefaultView;
			if (this.Session["MRTList_Sort"] == null)
				this.Session["MRTList_Sort"] = "FriendlyName";
			dv.Sort = this.Session["MRTList_Sort"].ToString();

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");

				if (ib != null)
				{
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("GlobalMetaInfo", "Delete").ToString() + "?')");
				}
			}
		}
		#endregion

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "MReferenceTypeList").ToString();
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "Create").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeEdit.aspx");
		}
		#endregion

		#region grdMain_Sorting
        /// <summary>
        /// Handles the Sorting event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (this.Session["MRTList_Sort"].ToString() == e.SortExpression)
				this.Session["MRTList_Sort"] += " DESC";
			else
				this.Session["MRTList_Sort"] = e.SortExpression;
			BindData();
		}
		#endregion

		#region grdMain_RowCommand
        /// <summary>
        /// Handles the RowCommand event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewCommandEventArgs"/> instance containing the event data.</param>
		protected void grdMain_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(e.CommandArgument.ToString());
				if (!MetaFieldType.IsUsed(type))
					MultiReferenceType.Remove(type);
				BindData();
			}
			if (e.CommandName == "Edit")
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(e.CommandArgument.ToString());
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeEdit.aspx?type=" + type.Name);
			}
		}
		#endregion

		#region grdMain_RowDeleting
        /// <summary>
        /// Handles the RowDeleting event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewDeleteEventArgs"/> instance containing the event data.</param>
		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
		}
		#endregion
	}
}