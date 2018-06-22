using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Controls.Util;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.MetaUI
{
	public partial class FormDocumentDesigner : System.Web.UI.UserControl
	{
		#region MetaClassName
		public string MetaClassName
		{
			get
			{
				if (ViewState[this.ID + "_MetaClassName"] == null)
					ViewState[this.ID + "_MetaClassName"] = (Request["ClassName"] != null) ? Request["ClassName"] : "";
				return (string)ViewState[this.ID + "_MetaClassName"];
			}
			set
			{
				ViewState[this.ID + "_MetaClassName"] = value;
			}
		}
		#endregion

		#region FormName
		public string FormName
		{
			get
			{
				if (ViewState[this.ID + "_FormName"] == null)
					ViewState[this.ID + "_FormName"] = (Request["FormName"] != null) ? Request["FormName"] : "";
				return (string)ViewState[this.ID + "_FormName"];
			}
			set
			{
				ViewState[this.ID + "_FormName"] = value;
			}
		}
		#endregion

		#region FormDocumentData
		public FormDocument FormDocumentData
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

		#region CanAddNewForm
		public bool CanAddNewForm
		{
			get
			{
				if (ViewState[this.ID + "_CanAddNewForm"] == null)
					ViewState[this.ID + "_CanAddNewForm"] = false;
				return (bool)ViewState[this.ID + "_CanAddNewForm"];
			}
			set
			{
				ViewState[this.ID + "_CanAddNewForm"] = value;
			}
		}
		#endregion

		private string scriptNewForm = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 460, {2});\">{1}</a>";
		private string scriptNewSection = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 520, {2});\">{1}</a>";
		private string scriptNewItem = "<a href='#' onclick=\"javascript:OpenFDDPopUp('{0}', 350, 550, {2});\">{1}</a>";

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			MainMetaToolbar.ClassName = "";
			MainMetaToolbar.ViewName = "";
			MainMetaToolbar.PlaceName = "FormDocumentDesigner";

			topMetaBar.ClassName = "";
			topMetaBar.ViewName = "";
			topMetaBar.PlaceName = "FormDocumentDesignerTop";

			if (!Page.IsPostBack)
			{
				if (!CanAddNewForm && (String.IsNullOrEmpty(MetaClassName) || String.IsNullOrEmpty(FormName)))
					throw new Exception("MetaClassName and FormName are required properties!");
				BindDD();
			}
			tdRight.Visible = false;
		}

		#region BindDD
        /// <summary>
        /// Binds the DD.
        /// </summary>
		private void BindDD()
		{
			if (CanAddNewForm)
			{
				Dictionary<int, string> dic = Mediachase.Ibn.Data.Meta.Management.SqlSerialization.MetaClassId.GetIds();
				List<string> list = new List<string>(dic.Values);
				list.Sort();

				ddClasses.DataSource = list;
				ddClasses.DataBind();

				if (!String.IsNullOrEmpty(MetaClassName))
					CHelper.SafeSelect(ddClasses, MetaClassName);

				MetaClassName = ddClasses.SelectedValue;
			}
			else
				lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);

			ddClasses.Visible = CanAddNewForm;
			lblTempClassName.Visible = !CanAddNewForm;

			BindForms();
		} 
		#endregion

		#region BindForms
        /// <summary>
        /// Binds the forms.
        /// </summary>
		private void BindForms()
		{
			string className = MetaClassName;

			if (CanAddNewForm)
			{
				ddFormName.Items.Clear();
				FormDocument[] mas = FormDocument.GetFormDocuments(className);
				foreach (FormDocument fd in mas)
					ddFormName.Items.Add(new ListItem(CHelper.GetFormName(fd.Name), fd.Name));

				if (!String.IsNullOrEmpty(FormName))
					CHelper.SafeSelect(ddFormName, FormName);

				FormName = (ddFormName.SelectedItem != null) ? ddFormName.SelectedValue : "";
			}
			else
				lblTempFormName.Text = FormName;

			ddFormName.Visible = CanAddNewForm;
			lblTempFormName.Visible = !CanAddNewForm;

			BindRenderer();
		} 
		#endregion

		#region BindRenderer
        /// <summary>
        /// Binds the renderer.
        /// </summary>
		private void BindRenderer()
		{
			FormDocumentData = null;
			try
			{
				FormDocumentData = FormDocument.Load(MetaClassName, FormName);

                if (MetaUIManager.MetaUITypeIsSystem(FormDocumentData.MetaClassName, FormDocumentData.MetaUITypeId))
					lblFormName.Text = CHelper.GetFormName(FormName);
				else
					lblFormName.Text = String.Format("{0} ({1})", CHelper.GetFormName(FormName), CHelper.GetFormName(FormDocumentData.MetaUITypeId));
			}
			catch { }
			BindRendererInner();
		} 
		#endregion

		#region BindRendererInner
        /// <summary>
        /// Binds the renderer inner.
        /// </summary>
		private void BindRendererInner()
		{
			fRenderer.FormDocumentData = FormDocumentData;
			fRenderer.DataBind();

			if (FormDocumentData == null)
			{
				ddFormName.Enabled = false;
				tblMove.Visible = false;
				tblLinks.Visible = false;
			}
			else
			{
				if (ddFormName.Visible)
					ddFormName.Enabled = true;
				tblMove.Visible = true;
				tblLinks.Visible = true;
			}
		} 
		#endregion

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnPreRender(EventArgs e)
		{
			ApplyActions();
			//txtXml.Text = FormDocumentData.GetFormTableXml();
 			base.OnPreRender(e);
		}

		#region ApplyActions
        /// <summary>
        /// Applies the actions.
        /// </summary>
		private void ApplyActions()
		{
			imgLeft.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/Arrows/Left.gif");
			imgUp.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/Arrows/Up.gif");
			imgRight.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/Arrows/Right.gif");
			imgDown.ImageUrl = CHelper.GetAbsolutePath("/Apps/MetaDataBase/Images/Arrows/Down.gif");

			imgLeft.Style.Add("cursor", "pointer");
			imgUp.Style.Add("cursor", "pointer");
			imgRight.Style.Add("cursor", "pointer");
			imgDown.Style.Add("cursor", "pointer");

			string clientID = fRenderer.TableContainer;
			imgLeft.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbLeft.UniqueID));
			imgUp.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbTop.UniqueID));
			imgRight.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbRight.UniqueID));
			imgDown.Attributes.Add("onclick", String.Format("__doPostBack('{1}', $find('{0}').getSelection());", clientID, lbDown.UniqueID));

			lbRemoveField.Attributes.Add("onclick", String.Format("if(confirm('{2}')){{__doPostBack('{1}', $find('{0}').getSelection());}} else return false;", clientID, lbRemoveField.UniqueID, GetGlobalResourceObject("MetaForm", "WarningField").ToString()));
			lbRemoveSection.Attributes.Add("onclick", String.Format("if(confirm('{2}')){{__doPostBack('{1}', $find('{0}').getSelection());}} else return false;", clientID, lbRemoveSection.UniqueID, GetGlobalResourceObject("MetaForm", "WarningSection").ToString()));

			if (CanAddNewForm)
				lblNewForm.Text = String.Format(scriptNewForm,
					String.Format("{2}?btn={0}&class={1}",
						lbNewForm.UniqueID, ddClasses.SelectedItem != null ? ddClasses.SelectedValue : "",
						CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormDocumentEdit.aspx")),
					GetGlobalResourceObject("MetaForm", "AddForm").ToString(),
					"null");

			string uid = Guid.NewGuid().ToString("N");
			hFieldKey.Value = uid;
			Session[uid] = FormDocumentData;
			lblEditForm.Text = String.Format(scriptNewForm,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormDocumentEdit.aspx"),
					uid, lbNewForm.UniqueID),
				GetGlobalResourceObject("MetaForm", "EditForm").ToString(),
				"null");

			lblNewSection.Text = String.Format(scriptNewSection,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormSectionEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("MetaForm", "AddSection").ToString(),
				"null");

			lblEditSection.Text = String.Format(scriptNewSection,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormSectionEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("MetaForm", "EditSection").ToString(),
				"'" + clientID + "'");

			lblAddField.Text = String.Format(scriptNewItem,
				String.Format("{0}?add=1&uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormItemEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("MetaForm", "AddField").ToString(),
				"'" + clientID + "'");

			lblEditField.Text = String.Format(scriptNewItem,
				String.Format("{0}?uid={1}&btn={2}",
					CHelper.GetAbsolutePath("/Apps/MetaDataBase/MetaUI/Pages/Public/FormItemEdit.aspx"),
					uid, lbAddSection.UniqueID),
				GetGlobalResourceObject("MetaForm", "EditField").ToString(),
				"'" + clientID + "'");
		} 
		#endregion

		#region lbDown_Click
        /// <summary>
        /// Handles the Click event of the lbDown control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbDown_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionDown(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemDown(uid);
			}

			BindRendererInner();
		} 
		#endregion

		#region lbRight_Click
        /// <summary>
        /// Handles the Click event of the lbRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbRight_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionRight(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemRight(uid);
			}

			BindRendererInner();
		} 
		#endregion

		#region lbTop_Click
        /// <summary>
        /// Handles the Click event of the lbTop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbTop_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionUp(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemUp(uid);
			}

			BindRendererInner();
		} 
		#endregion

		#region lbLeft_Click
        /// <summary>
        /// Handles the Click event of the lbLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbLeft_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}

			FormController fController = new FormController(FormDocumentData);
			FormSection itemSection = fController.GetSectionByUid(uid);
			if (itemSection != null)
				fController.MoveSectionLeft(uid);
			else
			{
				FormItem item = fController.GetSTLItemByUid(uid);
				if (item != null)
					fController.MoveFormItemLeft(uid);
			}

			BindRendererInner();
		} 
		#endregion

		#region Add/Edit Form
        /// <summary>
        /// Handles the Click event of the lbNewForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbNewForm_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocumentData = (FormDocument)Session[param];

			MetaClassName = FormDocumentData.MetaClassName;
			FormName = FormDocumentData.Name;

			fRenderer.FormDocumentData = FormDocumentData;
			fRenderer.DataBind();

			#region Visibility Elements
			if (CanAddNewForm)
			{
				bool changeForms = (ddClasses.SelectedItem == null || ddClasses.SelectedValue != MetaClassName);

				CHelper.SafeSelect(ddClasses, MetaClassName);
				if (ddClasses.SelectedItem == null || ddClasses.SelectedValue != MetaClassName)
				{
					ddClasses.Visible = false;
					lblTempClassName.Visible = true;
					lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
				}
				else
				{
					ddClasses.Visible = true;
					lblTempClassName.Visible = false;
				}

				if (changeForms)
				{
					ddFormName.Visible = false;
					lblTempFormName.Visible = true;
					lblTempFormName.Text = CHelper.GetFormName(FormName);
				}
				else
				{
					CHelper.SafeSelect(ddFormName, FormName);
					if (ddFormName.SelectedItem == null || ddFormName.SelectedValue != FormName)
					{
						ddFormName.Visible = false;
						lblTempFormName.Visible = true;
						lblTempFormName.Text = CHelper.GetFormName(FormName);
					}
					else
					{
						ddFormName.Visible = true;
						lblTempFormName.Visible = false;
					}
				}
			}
			else
			{
				lblTempClassName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
				lblTempFormName.Text = CHelper.GetFormName(FormName);
			}

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
			lblFormName.Text = CHelper.GetFormName(FormName);

			if (FormDocumentData == null)
			{
				ddFormName.Enabled = false;
				tblMove.Visible = false;
				tblLinks.Visible = false;
			}
			else
			{
				ddFormName.Enabled = true;
				tblMove.Visible = true;
				tblLinks.Visible = true;

                if (MetaUIManager.MetaUITypeIsSystem(FormDocumentData.MetaClassName, FormDocumentData.MetaUITypeId))
					lblFormName.Text = CHelper.GetFormName(FormDocumentData.Name);
				else
					lblFormName.Text = String.Format("{0} ({1})", CHelper.GetFormName(FormDocumentData.Name), CHelper.GetFormName(FormDocumentData.MetaUITypeId));
			}
			#endregion
		}
		#endregion
		
		#region Add/Edit Section
        /// <summary>
        /// Handles the Click event of the lbAddSection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbAddSection_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			if (String.IsNullOrEmpty(param))
				return;

			FormDocumentData = (FormDocument)Session[param];

			BindRendererInner();
		} 
		#endregion

		#region RemoveSection
        /// <summary>
        /// Handles the Click event of the lbRemoveSection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbRemoveSection_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}
			FormController fController = new FormController(FormDocumentData);
			fController.RemoveSection(uid);

			BindRendererInner();
		} 
		#endregion

		#region RemoveField
        /// <summary>
        /// Handles the Click event of the lbRemoveField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbRemoveField_Click(object sender, EventArgs e)
		{
			string param = Request.Params.Get("__EVENTARGUMENT");
			Guid uid = Guid.Empty;
			try
			{
				uid = new Guid(param);
			}
			catch
			{
				return;
			}
			FormController fController = new FormController(FormDocumentData);
			fController.RemoveFormItem(uid);

			BindRendererInner();
		} 
		#endregion

		#region Save
        /// <summary>
        /// Handles the Click event of the lbSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void lbSave_Click(object sender, EventArgs e)
		{
			FormDocumentData.Save();
			MetaClassName = FormDocumentData.MetaClassName;
			FormName = FormDocumentData.Name;

			BindDD();
		}

		protected void lbSaveClose_Click(object sender, EventArgs e)
		{
			FormDocumentData.Save();

			string retVal = "try {window.opener.location.href=window.opener.location.href;} catch (e) {;}setTimeout('window.close();', 500);";
			ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString("N"), retVal, true);
		}

		protected void lbReCreate_Click(object sender, EventArgs e)
		{
			FormDocumentData = FormController.CreateFormDocument(MetaClassName, FormName);

			BindRendererInner();
		} 
		#endregion

		#region Change Class
        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddClasses control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddClasses_SelectedIndexChanged(object sender, EventArgs e)
		{
			MetaClassName = ddClasses.SelectedValue;
			BindForms();

			lblTableName.Text = CHelper.GetResFileString(MetaDataWrapper.GetMetaClassByName(MetaClassName).FriendlyName);
		}
		#endregion

		#region Change Form
        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddFormName control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddFormName_SelectedIndexChanged(object sender, EventArgs e)
		{
			FormName = ddFormName.SelectedValue;
			BindRenderer();
		}
		#endregion
	}
}