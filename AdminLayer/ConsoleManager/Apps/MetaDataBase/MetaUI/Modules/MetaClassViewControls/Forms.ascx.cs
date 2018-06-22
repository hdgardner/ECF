﻿using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaUI.Modules.MetaClassViewControls
{
	public partial class Forms : MCDataBoundControl
	{
		protected readonly string className = "ClassName";
		protected readonly string dialogWidth = "1000";
		protected readonly string dialogHeight = "700";
		protected readonly string historyPostfix = "_History";

		#region DataItem
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				if (value is MetaClass)
					mc = (MetaClass)value;

				base.DataItem = value;
			}
		}
		#endregion

		#region MetaClass mc
		private MetaClass _mc;
		private MetaClass mc
		{
			get
			{
				if (_mc == null)
				{
					if (ViewState[className] != null)
						_mc = MetaDataWrapper.GetMetaClassByName(ViewState[className].ToString());
				}
				return _mc;
			}
			set
			{
				ViewState[className] = value.Name;
				_mc = value;
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
			ScriptManager sm1 = ScriptManager.GetCurrent(this.Page);
			if(sm1 != null)
				sm1.RegisterPostBackControl(btnRefresh);
		}

		#region DataBind
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			if (mc != null)
			{
				//lnkNew.Text = GetGlobalResourceObject("GlobalMetaInfo", "NewItem").ToString();
				//lnkNew.NavigateUrl = String.Format("javascript:ShowWizard(\"{4}?class={0}&btn={1}\", {2}, {3});",
				//    mc.Name, btnRefresh.UniqueID, 350, 460,
				//    CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormDocumentEdit.aspx"));

				BindGrid();
			}
		}
		#endregion

		#region CheckVisibility
        /// <summary>
        /// Checks the visibility.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
		public override bool CheckVisibility(object dataItem)
		{
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region BindGrid
        /// <summary>
        /// Binds the grid.
        /// </summary>
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Locale = CultureInfo.InvariantCulture;
			dt.Columns.Add("Id", typeof(string));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("EditLink", typeof(string));
			dt.Columns.Add("CanDelete", typeof(bool));
			dt.Columns.Add("PublicFormName", typeof(string));

			FormDocument[] mas = FormDocument.GetFormDocuments(mc.Name);
			foreach (FormDocument fd in mas)
			{
				DataRow row = dt.NewRow();
				row["Id"] = fd.Name;
				string name = "";
                if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId))
					name = CHelper.GetFormName(fd.MetaUITypeId);
				else
					name = String.Format("{0} ({1})", CHelper.GetFormName(fd.Name), CHelper.GetFormName(fd.MetaUITypeId));

				row["PublicFormName"] = "-1";

				row["Name"] = name;
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					mc.Name, btnRefresh.UniqueID,
					fd.Name, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaDataBase/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));

				//if (MetaUIManager.MetaUITypeIsSystem(fd.MetaUITypeId))
				//    row["CanDelete"] = false;
				//else
				row["CanDelete"] = true;

				dt.Rows.Add(row);
			}

			// History
			string historyClassName = HistoryManager.GetHistoryMetaClassName(mc.Name);
			mas = FormDocument.GetFormDocuments(historyClassName);
			foreach (FormDocument fd in mas)
			{
				DataRow row = dt.NewRow();
				row["Id"] = String.Format(CultureInfo.InvariantCulture, "{0}{1}", fd.Name, historyPostfix);
				string name = "";
                if (MetaUIManager.MetaUITypeIsSystem(fd.MetaClassName, fd.MetaUITypeId))
					name = CHelper.GetFormName(fd.MetaUITypeId);
				else
					name = String.Format("{0} ({1})", CHelper.GetFormName(fd.Name), CHelper.GetFormName(fd.MetaUITypeId));

				row["PublicFormName"] = "-1";

				row["Name"] = name;
				row["EditLink"] = String.Format("javascript:OpenSizableWindow(\"{5}?ClassName={0}&btn={1}&FormName={2}\", {3}, {4});",
					historyClassName, btnRefresh.UniqueID,
					fd.Name, dialogWidth, dialogHeight,
					ResolveClientUrl("~/Apps/MetaDataBase/MetaUI/Pages/Admin/CustomizeObjectView2.aspx"));

				//if (MetaUIManager.MetaUITypeIsSystem(fd.MetaUITypeId))
				//    row["CanDelete"] = false;
				//else
				row["CanDelete"] = true;

				dt.Rows.Add(row);
			}

			grdMain.DataSource = dt;
			grdMain.DataBind();

			foreach (DataGridItem row in grdMain.Items)
			{
				ImageButton ib = (ImageButton)row.FindControl("ibDelete");
				if (ib != null)
					ib.Attributes.Add("onclick", "return confirm('" + GetGlobalResourceObject("GlobalMetaInfo", "Delete").ToString() + "?')");
			}
		}
		#endregion

		#region grdMain_RowDataBound
		protected void grdMain_RowDataBound(object sender, DataGridItemEventArgs e)
		{
			TextBox tb = (TextBox)e.Item.FindControl("txtLink");
			if (tb == null)
				return;

			if (!String.IsNullOrEmpty(e.Item.Cells[0].Text) && e.Item.Cells[0].Text != "-1")
			{
				tb.Visible = true;
				string fName = e.Item.Cells[0].Text;
				string sPath = CHelper.GetAbsolutePath("/YourPublicPage.aspx"); // see IBN47 realization
				string ss = "ClassName=" + mc.Name + "&FormName=" + fName;
				ss = HttpUtility.UrlEncode(ss);
				string sc = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ss));
				sPath += "?uid=" + sc;
				tb.Text = String.Format("<iframe src='{0}' style='border: 0' width='100%' height='100%' frameborder='0' scrolling='auto'></iframe>", sPath);
				tb.Style.Add("margin-top", "10px");
			}
			else
				tb.Visible = false;
		}
		#endregion

		#region btnRefresh_Click
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			CHelper.RequireDataBind();

			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocument FormDocumentData = (FormDocument)Session[param];
			FormDocumentData.Save();

			if (FormDocumentData.MetaUITypeId == FormController.GeneralViewFormType || FormDocumentData.MetaUITypeId == FormController.GeneralViewHistoryFormType)
				MetaDataWrapper.AddClassAttribute(MetaDataWrapper.GetMetaClassByName(FormDocumentData.MetaClassName), "HasCompositePage", true);
		}
		#endregion

		#region grdMain_DeleteCommand
		protected void grdMain_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			string id = e.CommandArgument.ToString();

			FormDocument fd = FormDocument.Load(mc.Name, id);
			fd.Delete();
			if (id == FormController.GeneralViewFormType || id == FormController.GeneralViewHistoryFormType)
				MetaDataWrapper.RemoveClassAttribute(mc, "HasCompositePage");

			CHelper.RequireDataBind();
		}
		#endregion
	}
}