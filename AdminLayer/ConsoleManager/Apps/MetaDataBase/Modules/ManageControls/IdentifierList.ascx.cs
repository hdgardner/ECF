using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class IdentifierList : System.Web.UI.UserControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindToolbar();

			if (!this.IsPostBack)
				BindData();
		}

		#region grdMain_Sorting
        /// <summary>
        /// Handles the Sorting event of the grdMain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
		protected void grdMain_Sorting(object sender, GridViewSortEventArgs e)
		{
			if (Session["IdentifierList_Sort"].ToString() == e.SortExpression)
				Session["IdentifierList_Sort"] += " DESC";
			else
				Session["IdentifierList_Sort"] = e.SortExpression;
			BindData();
		} 
		#endregion

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "IdentifierList").ToString();
			secHeader.AddLink("<img src='" + CHelper.GetAbsolutePath("/images/newitem.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "Create").ToString(), "~/Apps/MetaDataBase/Pages/Admin/IdentifierEdit.aspx");
		} 
		#endregion

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
			dt.Columns.Add("Mask", typeof(string));
			dt.Columns.Add("CounterLength", typeof(int));
			dt.Columns.Add("CounterReset", typeof(string));
			dt.Columns.Add("Scope", typeof(string));
			dt.Columns.Add("IsUsed", typeof(bool));

			foreach (MetaFieldType mfType in MetaDataWrapper.GetIdentifierList())
			{
				DataRow dr = dt.NewRow();
				dr["Name"] = mfType.Name;
				dr["FriendlyName"] = CHelper.GetResFileString(mfType.FriendlyName);
				dr["Mask"] = mfType.Attributes[McDataTypeAttribute.IdentifierMask].ToString();
				dr["CounterLength"] = int.Parse(mfType.Attributes[McDataTypeAttribute.IdentifierMaskDigitLength].ToString());
				dr["CounterReset"] = GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + mfType.Attributes[McDataTypeAttribute.IdentifierPeriodType].ToString());
				dr["Scope"] = GetGlobalResourceObject("GlobalMetaInfo", "Scope" + mfType.Attributes[McDataTypeAttribute.IdentifierType].ToString());
				dr["IsUsed"] = MetaIdentifier.IsUsed(mfType);
				dt.Rows.Add(dr);
			}

			DataView dv = dt.DefaultView;
			if (Session["IdentifierList_Sort"] == null)
				Session["IdentifierList_Sort"] = "Name";
			dv.Sort = Session["IdentifierList_Sort"].ToString();

			grdMain.DataSource = dv;
			grdMain.DataBind();

			foreach (GridViewRow row in grdMain.Rows)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");

				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("GlobalMetaInfo", "Delete").ToString() + "?')");
			}

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
				MetaFieldType type = MetaDataWrapper.GetIdentifierByName(e.CommandArgument.ToString());

				if (!MetaIdentifier.IsUsed(type))
					MetaIdentifier.Remove(type);

				BindData();
			}
			if (e.CommandName == "Edit")
			{
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/IdentifierEdit.aspx?type=" + e.CommandArgument.ToString() + "&back=list");
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