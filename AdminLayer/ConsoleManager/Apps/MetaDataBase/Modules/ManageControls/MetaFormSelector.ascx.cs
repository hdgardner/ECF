using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Layout;
using System.Collections.Generic;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.ManageControls
{
	public partial class MetaFormSelector : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region .prop MetaForms
		public List<FormDocument> MetaForms
		{
			get
			{
				List<FormDocument> retVal = new List<FormDocument>();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					if (!dgi.Cells[1].Text.Equals("true"))
						continue;

					string className = dgi.Cells[0].Text;
					CheckBox cb = (CheckBox)dgi.FindControl("cbIsAdd");
					if (cb != null && cb.Checked)
					{
						FormDocument fd = new FormDocument();
						fd.MetaClassName = className;
						fd.Name = grdMain.DataKeys[dgi.ItemIndex].ToString();
						retVal.Add(fd);
					}
				}
				return retVal;
			}
		} 
		#endregion

		#region .prop MetaViews
		public List<string> MetaViews
		{
			get
			{
				List<string> retVal = new List<string>();
				foreach (DataGridItem dgi in grdMain.Items)
				{
					if (dgi.Cells[1].Text.Equals("true"))
						continue;

					CheckBox cb = (CheckBox)dgi.FindControl("cbIsAdd");
					if (cb != null && cb.Checked)
					{
						string sName = grdMain.DataKeys[dgi.ItemIndex].ToString();
						retVal.Add(sName);
					}
				}
				return retVal;
			}
		}
		#endregion

		#region BindData
		public void BindData(string metaClassName)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("DisplayName", typeof(string)));
			dt.Columns.Add(new DataColumn("ClassName", typeof(string)));
			dt.Columns.Add(new DataColumn("IsForm", typeof(string)));

			string[] forms = MetaUIManager.GetMetaFormList(metaClassName);
			foreach (string name in forms)
			{
				AddRow(dt, metaClassName, name, true, false);
			}
			
			string[] views = MetaUIManager.GetMetaViewList(metaClassName);
			foreach (string name in views)
				AddRow(dt, metaClassName, name, false, false);

			grdMain.DataKeyField = "Name";
			grdMain.DataSource = dt.DefaultView;
			grdMain.DataBind();
		}

		private void AddRow(DataTable dt, string metaClassName, string name, bool isForm, bool isHistory)
		{
			DataRow dr = dt.NewRow();
			dr["Name"] = name;
			dr["ClassName"] = metaClassName;
			dr["IsForm"] = isForm.ToString().ToLower();
			string sName = String.Format(" {0}{1}",
				(isForm ? CHelper.GetFormName(name) : ((name.IndexOf("_History") >= 0) ? GetGlobalResourceObject("Common", "HistoryView").ToString() : GetGlobalResourceObject("Common", "ListView").ToString())),
				((isForm && name != FormController.BaseFormType && name != FormController.CreateFormType && name != FormController.GeneralViewFormType && name != FormController.ShortViewFormType && name != FormController.GeneralViewHistoryFormType) ? " (" + GetGlobalResourceObject("MetaForm", "FormName").ToString() + ")" : ""));
			
			dr["DisplayName"] = sName;
			dt.Rows.Add(dr);
		}
		#endregion
	}
}