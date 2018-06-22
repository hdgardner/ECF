using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core.Layout;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaCardEdit : System.Web.UI.UserControl
	{
		MetaClass mc = null;

		#region OwnerClassName
		private string _ownerClassName = String.Empty;
        /// <summary>
        /// Gets or sets the name of the owner class.
        /// </summary>
        /// <value>The name of the owner class.</value>
		public string OwnerClassName
		{
			get { return _ownerClassName; }
			set { _ownerClassName = value; }
		}
		#endregion

		#region ClassName
		private string _className = "";
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}
		#endregion

		#region Back
		private string _back = "";
        /// <summary>
        /// Gets or sets the back.
        /// </summary>
        /// <value>The back.</value>
		public string Back
		{
			get { return _back; }
			set { _back = value; }
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
			BindInfo();

			if (!IsPostBack)
				BindData();
		}

		#region Page_Init
        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);
		}
		#endregion

		#region LoadRequestVariables
        /// <summary>
        /// Loads the request variables.
        /// </summary>
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				mc = MetaDataWrapper.GetMetaClassByName(ClassName);
			}

			if (Request.QueryString["owner"] != null)
			{
				OwnerClassName = Request.QueryString["owner"];
			}

			if (Request.QueryString["back"] != null)
				Back = Request.QueryString["back"].ToLower();
		}
		#endregion

		#region BindInfo
        /// <summary>
        /// Binds the info.
        /// </summary>
		private void BindInfo()
		{
			if (mc != null)
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "CardEdit").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "CardCreate").ToString();

			if (Back == "view" && mc != null)
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", Page.ResolveClientUrl("../../images/cancel.gif"), GetGlobalResourceObject("GlobalMetaInfo", "BackToTableInfo").ToString()),
				  String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", mc.Name));

				imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", mc.Name));
			}
			else if (Back == "owner" && OwnerClassName != String.Empty)
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", Page.ResolveClientUrl("../../images/cancel.gif"), GetGlobalResourceObject("GlobalMetaInfo", "Back").ToString()),
				  String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", OwnerClassName));

				imbtnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}'; return false;", OwnerClassName));
			}
			else  // Back to List by default
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", Page.ResolveClientUrl("../../images/cancel.gif"), GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString()),
				  "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx");

				imbtnCancel.Attributes.Add("onclick", "window.location.href='MetaClassList.aspx'; return false;");
			}

			imbtnSave.CustomImage = Page.ResolveClientUrl("../../images/saveitem.gif");
			imbtnCancel.CustomImage = Page.ResolveClientUrl("../../images/cancel.gif");
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			if (mc == null)
			{
				if (OwnerClassName != String.Empty)
				{
					MetaClass ownerClass = MetaDataWrapper.GetMetaClassByName(OwnerClassName);
					ddlClass.Items.Add(new ListItem(CHelper.GetResFileString(ownerClass.FriendlyName), ownerClass.Name));
					ddlClass.Enabled = false;
				}
				else
				{
					foreach (MetaClass cls in MetaDataWrapper.GetMetaClassesSupportedCards())
					{
						ddlClass.Items.Add(new ListItem(cls.Name, cls.Name));
					}
				}

				txtClassName.Attributes.Add("onblur", "SetName('" + txtClassName.ClientID + "','" + txtClassFriendlyName.ClientID + "','" + vldClassFriendlyName_Required.ClientID + "')" + "; SetName('" + txtClassName.ClientID + "','" + txtClassPluralName.ClientID + "','" + vldClassPluralName_Required.ClientID + "')");
			}
			else
			{
				MetaClass ownerClass = MetaDataWrapper.GetOwnerClass(mc);
				ddlClass.Items.Add(new ListItem(ownerClass.Name, ownerClass.Name));
				ddlClass.Enabled = false;

				txtClassName.Text = mc.Name;
				txtClassName.Enabled = false;

				txtClassFriendlyName.Text = mc.FriendlyName;
				txtClassPluralName.Text = mc.PluralName;
			}
		}
		#endregion

		#region imbtnSave_ServerClick
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

			if (mc == null) // Create
			{
				try
				{
					mc = MetaDataWrapper.CreateCard(ddlClass.SelectedValue,
					txtClassName.Text.Trim(), txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim());

                    FormDocument fDocBase = FormController.CreateFormDocument(mc.Name, FormController.BaseFormName);
                    fDocBase.Save();

                    FormDocument fDocCreate = FormController.CreateFormDocument(mc.Name, FormController.CreateFormName);
                    fDocCreate.Save();

					Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", mc.Name), true);
				}
				catch (MetaClassAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("GlobalMetaInfo", "CardExistsErrorMessage").ToString(), "'" + txtClassName.Text.Trim() + "'");
					lbError.Visible = true;
				}
			}
			else  // Update
			{
				MetaDataWrapper.UpdateCard(mc, txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim());

				if (Back == "list")
					Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx", true);
				else
					Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", mc.Name), true);
			}
		}
		#endregion
	}
}