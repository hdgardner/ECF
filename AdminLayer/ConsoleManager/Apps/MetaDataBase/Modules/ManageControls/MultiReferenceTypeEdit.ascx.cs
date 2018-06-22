using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MultiReferenceTypeEdit : System.Web.UI.UserControl
	{
		#region TypeName
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
		public string TypeName
		{
			get
			{
				if (Request.QueryString["type"] != null)
					return Request.QueryString["type"].ToString();
				else
					return string.Empty;
			}
		}
		#endregion

		#region BackMode
        /// <summary>
        /// Gets the back mode.
        /// </summary>
        /// <value>The back mode.</value>
		public string BackMode
		{
			get
			{
				if (Request.QueryString["back"] != null)
					return Request.QueryString["back"].ToString();
				else
					return string.Empty;

			}
		}
		#endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);
			this.imbtnCancel.ServerClick += new EventHandler(imbtnCancel_ServerClick);
			this.dgClasses.DeleteCommand += new DataGridCommandEventHandler(dgClasses_delete);
			this.dgClasses.EditCommand += new DataGridCommandEventHandler(dgClasses_edit);
			this.dgClasses.CancelCommand += new DataGridCommandEventHandler(dgClasses_cancel);
			this.dgClasses.UpdateCommand += new DataGridCommandEventHandler(dgClasses_update);
			BindToolbar();

			if (!IsPostBack)
				BindData();
		}

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			if (TypeName == string.Empty)
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "MReferenceTypeCreate").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "MReferenceTypeEdit").ToString();

			if (BackMode == "view" && TypeName != string.Empty)
				secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToEnumInfo").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type=" + TypeName);
			else
				secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");

			imbtnSave.CustomImage = Page.ResolveClientUrl("../../images/saveitem.gif");
			imbtnCancel.CustomImage = Page.ResolveClientUrl("../../images/cancel.gif");

			txtMRTName.Attributes.Add("onblur", "SetName('" + txtMRTName.ClientID + "','" + txtFriendlyName.ClientID + "','" + vldFriendlyName_Required.ClientID + "')");
		}
		#endregion

		#region Cancel
        /// <summary>
        /// Handles the ServerClick event of the imbtnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void imbtnCancel_ServerClick(object sender, EventArgs e)
		{
			if (BackMode == "view" && TypeName != string.Empty)
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type=" + TypeName);
			else
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");
		}
		#endregion

		#region Save
        /// <summary>
        /// Handles the ServerClick event of the imbtnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void imbtnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			if (TypeName == string.Empty)
			{
				string typeName = String.Empty;
				using (MetaClassManagerEditScope editScope = DataContext.Current.MetaModel.BeginEdit())
				{
					DataTable dt = (DataTable)ViewState["DT_Source"];
					List<MultiReferenceItem> mas = new List<MultiReferenceItem>();
					foreach (DataRow dr in dt.Rows)
						mas.Add(new MultiReferenceItem(dr["Name"].ToString(), dr["Name"].ToString(), dr["FriendlyName"].ToString()));
					MetaFieldType type = MultiReferenceType.Create(txtMRTName.Text, txtFriendlyName.Text, mas.ToArray());
					typeName = type.Name;
					editScope.SaveChanges();
				}
				//if (typeName != String.Empty)
				//    Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type={0}", typeName));
				//else
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeList.aspx");
			}
			else
			{
				MetaFieldType type = MetaDataWrapper.GetTypeByName(TypeName);
				type.FriendlyName = txtFriendlyName.Text;
				Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MultiReferenceTypeView.aspx?type={0}", TypeName));
			}
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("FriendlyName", typeof(string)));

			if (TypeName != string.Empty)
			{
				MetaFieldType mft = MetaDataWrapper.GetTypeByName(TypeName);
				if (mft != null)
				{
					txtMRTName.Text = mft.Name;
					txtMRTName.ReadOnly = true;
					txtMRTName.CssClass = "text-readonly";
					txtFriendlyName.Text = mft.FriendlyName;
				}

				DataRow dr;
				foreach (MultiReferenceItem mri in MultiReferenceType.GetMultiReferenceItems(mft))
				{
					dr = dt.NewRow();
					dr["Name"] = mri.MetaClassName;
					dr["FriendlyName"] = mri.MetaClassName;
					dt.Rows.Add(dr);
				}
				dgClasses.Columns[2].Visible = false;
			}
			ViewState["DT_Source"] = dt;
			BindDG();
		}
		#endregion

		#region BindDG
        /// <summary>
        /// Binds the DG.
        /// </summary>
		private void BindDG()
		{
			dgClasses.Columns[1].HeaderText = "MetaClass Name";
			if (TypeName == String.Empty)
				dgClasses.Columns[2].HeaderText = String.Format("<a href='#' onclick=\"{0}\"><img alt='' align='absmiddle' border='0' src='{1}' />&nbsp;New</a>",
					Page.ClientScript.GetPostBackClientHyperlink(lbNewClass, ""),
					CHelper.GetAbsolutePath("/images/newitem.gif"));
			DataTable dt = (DataTable)ViewState["DT_Source"];
			dgClasses.DataSource = dt.DefaultView;
			dgClasses.DataBind();
		}
		#endregion

		#region dgClasses Events
        /// <summary>
        /// Handles the Click event of the lbNewClass control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbNewClass_Click(object sender, EventArgs e)
		{
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();

			DataRow dr = dt.NewRow();
			dr["Name"] = "";
			dr["FriendlyName"] = "";
			dt.Rows.Add(dr);

			dgClasses.EditItemIndex = dt.Rows.Count - 1;
			dgClasses.DataKeyField = "Name";
			dgClasses.DataSource = dt.DefaultView;
			dgClasses.DataBind();

			foreach (DataGridItem dgi in dgClasses.Items)
			{
				DropDownList ddl = (DropDownList)dgi.Cells[1].FindControl("ddClasses");
				if (ddl != null)
				{
					Dictionary<int, string> dataSource = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
					List<string> list = new List<string>(dataSource.Values);
					list.Sort();
					ddl.DataSource = list;
					ddl.DataBind();
				}
			}
		}

        /// <summary>
        /// Handles the delete event of the dgClasses control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void dgClasses_delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sName = dgClasses.DataKeys[e.Item.ItemIndex].ToString();
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();
			DataRow[] dr = dt.Select("Name = '" + sName + "'");
			if (dr.Length > 0)
			{
				dt.Rows.Remove(dr[0]);
			}
			ViewState["DT_Source"] = dt;
			dgClasses.EditItemIndex = -1;
			BindDG();
		}

        /// <summary>
        /// Handles the edit event of the dgClasses control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void dgClasses_edit(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgClasses.EditItemIndex = e.Item.ItemIndex;
			dgClasses.DataKeyField = "Name";
			BindDG();

			foreach (DataGridItem dgi in dgClasses.Items)
			{
				DropDownList ddl = (DropDownList)dgi.Cells[1].FindControl("ddClasses");
				if (ddl != null)
				{
					Dictionary<int, string> dataSource = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
					List<string> list = new List<string>(dataSource.Values);
					list.Sort();
					ddl.DataSource = list;
					ddl.DataBind();
					CHelper.SafeSelect(ddl, e.Item.Cells[0].Text);
				}
			}
		}

        /// <summary>
        /// Handles the cancel event of the dgClasses control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void dgClasses_cancel(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgClasses.EditItemIndex = -1;
			BindDG();
		}

        /// <summary>
        /// Handles the update event of the dgClasses control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
		private void dgClasses_update(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sName = dgClasses.DataKeys[e.Item.ItemIndex].ToString();
			DataTable dt = ((DataTable)ViewState["DT_Source"]).Copy();
			DataRow[] dr = dt.Select("Name = '" + sName + "'");
			DropDownList ddl = (DropDownList)e.Item.FindControl("ddClasses");
			if (ddl != null)
			{
				DataRow[] drTest = dt.Select("Name = '" + ddl.SelectedValue + "'");
				if (drTest.Length == 0)
				{
					if (dr.Length > 0)
					{
						dr[0]["Name"] = ddl.SelectedValue;
						dr[0]["FriendlyName"] = ddl.SelectedValue;
					}
					else
					{
						DataRow drNew = dt.NewRow();
						drNew["Name"] = ddl.SelectedValue;
						drNew["FriendlyName"] = ddl.SelectedValue;
						dt.Rows.Add(drNew);
					}
				}
			}
			ViewState["DT_Source"] = dt;
			dgClasses.EditItemIndex = -1;
			BindDG();
		}
		#endregion
	}
}