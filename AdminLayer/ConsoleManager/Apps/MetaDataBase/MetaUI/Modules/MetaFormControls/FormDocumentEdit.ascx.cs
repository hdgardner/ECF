using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core.Configuration;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormDocumentEdit : System.Web.UI.UserControl
	{
		#region RefreshButton
		public string RefreshButton
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["btn"] != null)
					retval = Request.QueryString["btn"];
				return retval;
			}
		}
		#endregion

		#region uid
		protected string uid
		{
			get
			{
				if (!String.IsNullOrEmpty(Request["uid"]))
					return Request["uid"];
				else
					return "";
			}
		}
		#endregion

		#region FormDocumentData
		private FormDocument FormDocumentData
		{
			get
			{
				if (ViewState[this.ID + "_FormDocumentData"] == null)
					return null;
				return (FormDocument)ViewState[this.ID + "_FormDocumentData"];
			}
			set
			{
				ViewState[this.ID + "_FormDocumentData"] = value;
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
			if (String.IsNullOrEmpty(Request["class"]))
				throw new Exception("ClassName is required!");

			if (!Page.IsPostBack)
			{
				if (!String.IsNullOrEmpty(uid))
				{
					#region Edit
					lblTitle.Text = GetGlobalResourceObject("MetaForm", "EditForm").ToString();
					lblComments.Text = GetGlobalResourceObject("MetaForm", "EditFormComment").ToString();

					FormDocumentData = ((FormDocument)Session[uid]).Copy();
					lblClass.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(FormDocumentData.MetaClassName).FriendlyName);
					ddClasses.Visible = false;

                    if (MetaUIManager.MetaUITypeIsSystem(ddClasses.SelectedValue, FormDocumentData.MetaUITypeId))
						lblForm.Text = CHelper.GetFormName(FormDocumentData.Name);
					else
						lblForm.Text = String.Format("{0} ({1})", CHelper.GetFormName(FormDocumentData.Name), CHelper.GetFormName(FormDocumentData.MetaUITypeId));

					ddFormType.Visible = false;

					BindValues();
					#endregion
				}
				else
				{
					#region Create
					lblTitle.Text = GetGlobalResourceObject("MetaForm", "AddForm").ToString();
					lblComments.Text = GetGlobalResourceObject("MetaForm", "AddFormComment").ToString();

					Dictionary<int, string> dic = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
					List<string> list = new List<string>(dic.Values);
					list.Sort();

					ddClasses.DataSource = list;
					ddClasses.DataBind();

					string metaclassName = Request["class"];
					if (!String.IsNullOrEmpty(metaclassName))
						CHelper.SafeSelect(ddClasses, metaclassName);

					//lblClass.Visible = false;

					ddClasses.Visible = false;
					lblClass.Visible = true;
					lblClass.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(ddClasses.SelectedValue).FriendlyName);

					ddFormType.Items.Clear();
					MetaUITypeElement[] mas = MetaUIManager.GetCreatableUITypes(metaclassName, MetaUITypeElement.FormCategory);
					foreach (MetaUITypeElement elem in mas)
						ddFormType.Items.Add(new ListItem(CHelper.GetFormName(elem.Name), elem.Id));

					txtCellPadding.Text = "5";
					#endregion
				}
			}
			lblError.Visible = false;
			btnSave.InnerText = GetGlobalResourceObject("MetaForm", "Save").ToString();
			btnCancel.InnerText = GetGlobalResourceObject("MetaForm", "Cancel").ToString();
		}

		#region BindValues
        /// <summary>
        /// Binds the values.
        /// </summary>
		private void BindValues()
		{
			//set layout type
			switch (FormDocumentData.FormTable.Columns)
			{
				case "50%;*":
				case "50%;50%":
				case "*;50%":
					rb11.Checked = true;
					break;
				case "35%;*":
					rb12.Checked = true;
					break;
				case "*;35%":
					rb21.Checked = true;
					break;
				case "33%;33%;*":
				case "33%;*;33%":
				case "*;33%;33%":
					rb111.Checked = true;
					rb11.Disabled = true;
					rb12.Disabled = true;
					rb21.Disabled = true;
					break;
				default:
					rb111.Disabled = true;
					rb11.Disabled = true;
					rb12.Disabled = true;
					rb21.Disabled = true;

					rb11.Checked = false;
					break;
			}
			txtCellPadding.Text = FormDocumentData.FormTable.CellPadding.ToString();
		}
		#endregion

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddFormType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddFormType_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
            divTitle.Visible = (ddFormType.Visible && !MetaUIManager.MetaUITypeIsSystem(ddClasses.SelectedValue, ddFormType.SelectedValue));
			base.OnPreRender(e);
		}

		#region btnSave_ServerClick
        /// <summary>
        /// Handles the ServerClick event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (!String.IsNullOrEmpty(uid))
			{
				#region Edit
				string columns = FormDocumentData.FormTable.Columns;
				int colsCount = columns.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Length;
				bool needToAdd = false;
				if (!rb11.Disabled && rb11.Checked)
					columns = "50%;*";

				if (!rb12.Disabled && rb12.Checked)
					columns = "35%;*";

				if (!rb21.Disabled && rb21.Checked)
					columns = "*;35%";

				if (!rb111.Disabled && rb111.Checked)
				{
					columns = "33%;33%;*";
					if (colsCount < 3)
						needToAdd = true;
				}

				FormDocumentData.FormTable.Columns = columns;
				if (needToAdd)
				{
					foreach (FormRow row in FormDocumentData.FormTable.Rows)
					{
						bool needCell = false;
						foreach (FormCell cell in row.Cells)
						{
							if (cell.ColSpan > 1 && cell.ColSpan < 3)
								cell.ColSpan = 3;
							if (cell.ColSpan == 1)
								needCell = true;
						}
						if (needCell)
						{
							FormCell cell = new FormCell("cell_23", 1);
							row.Cells.Add(cell);
						}
					}
				}

				if (!String.IsNullOrEmpty(txtCellPadding.Text))
					FormDocumentData.FormTable.CellPadding = int.Parse(txtCellPadding.Text);
				#endregion
			}
			else
			{
				#region Create
				string className = ddClasses.SelectedValue;
				try
				{
					string formName = "";
                    if (!MetaUIManager.MetaUITypeIsSystem(ddClasses.SelectedValue, ddFormType.SelectedValue))
						formName = txtTitle.Text;
					else
						formName = ddFormType.SelectedValue;

					string columns = "50%;*";

					if (rb11.Checked)
						columns = "50%;*";
					else if (rb12.Checked)
						columns = "35%;*";
					else if (rb21.Checked)
						columns = "*;35%";
					else if (rb111.Checked)
						columns = "33%;33%;*";

					//set columns

					int cellPadding = 5;
					if (!String.IsNullOrEmpty(txtCellPadding.Text))
						cellPadding = int.Parse(txtCellPadding.Text);

					FormDocumentData = FormController.CreateFormDocument(className, formName, ddFormType.SelectedValue, columns, Unit.Percentage(100), cellPadding);
				}
				catch
				{
					lblError.Visible = true;
					return;
				}
				#endregion
			}

			string newUid = Guid.NewGuid().ToString("N");
			Session[newUid] = FormDocumentData;
			CloseAndRefresh(newUid);
		}
		#endregion

		#region CloseAndRefresh
        /// <summary>
        /// Closes the and refresh.
        /// </summary>
        /// <param name="itemUid">The item uid.</param>
		private void CloseAndRefresh(string itemUid)
		{
			// OR: ���� �������� RefreshButton, �� ��������� ������������ ���� ����� � �������
			if (RefreshButton != String.Empty)
			{
				string script = String.Format("try{{window.opener.__doPostBack('{0}', '{1}')}}catch(e){{;}}window.close();", RefreshButton, itemUid);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), script, true);
			}
			else
			{
				string backRefresh = ";";
				if (!String.IsNullOrEmpty(Request["backRefresh"]))
					backRefresh = "window.opener." + Request["backRefresh"] + "(" + itemUid + ");";
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "try{" + backRefresh + "}catch(e){;}window.close();", true);
			}
		}
		#endregion
	}
}