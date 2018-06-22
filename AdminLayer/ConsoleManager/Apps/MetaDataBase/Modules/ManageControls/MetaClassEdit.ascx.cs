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

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class MetaClassEdit : System.Web.UI.UserControl
	{
		private MetaClass _mc = null;

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
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);

			LoadRequestVariables();

			//if (_mc != null)
			//{
			//    CHelper.AddToContext(NavigationBlock.KeyContextMenu, "MetaClassView");
			//    CHelper.AddToContext(NavigationBlock.KeyContextMenuTitle, CHelper.GetResFileString(_mc.FriendlyName));
			//}

			BindInfo();

			if (!IsPostBack)
				BindData();
		}

		#region LoadRequestVariables
        /// <summary>
        /// Loads the request variables.
        /// </summary>
		private void LoadRequestVariables()
		{
			if (Request.QueryString["class"] != null)
			{
				ClassName = Request.QueryString["class"];
				_mc = MetaDataWrapper.GetMetaClassByName(ClassName);
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
			if (_mc != null)
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "EditTable").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "CreateTable").ToString();

			if (Back == "view" && _mc != null)
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", Page.ResolveClientUrl("../../images/cancel.gif"), GetGlobalResourceObject("GlobalMetaInfo", "BackToTableInfo").ToString()),
				  String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", _mc.Name));

				btnCancel.Attributes.Add("onclick", String.Format("window.location.href='MetaClassView.aspx?class={0}';return false;", _mc.Name));
			}
			else  // Back to List by default
			{
				secHeader.AddLink(
				  String.Format("<img src='{0}' border='0' align='absmiddle' width='16px' height='16px' />&nbsp;{1}", Page.ResolveClientUrl("../../images/cancel.gif"), GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString()),
				  "~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx");

				btnCancel.Attributes.Add("onclick", "window.location.href='MetaClassList.aspx'; return false;");
			}

			btnSave.CustomImage = Page.ResolveClientUrl("../../images/saveitem.gif");
			btnCancel.CustomImage = Page.ResolveClientUrl("../../images/cancel.gif");

			lgdClass.InnerText = GetGlobalResourceObject("GlobalMetaInfo", "TableInfo").ToString(); ;
			lgdField.InnerText = GetGlobalResourceObject("GlobalMetaInfo", "FieldInfo").ToString(); ;
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			if (_mc == null)
			{
				ddlOwnerType.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Private").ToString(), OwnerTypes.Private.ToString()));
				ddlOwnerType.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Public").ToString(), OwnerTypes.Public.ToString()));

				txtFieldName.Text = "Title";
				txtFieldFriendlyName.Text = "Title";
				txtMaxLen.Text = "100";

				txtClassName.Attributes.Add("onblur", "SetName('" + txtClassName.ClientID + "','" + txtClassFriendlyName.ClientID + "','" + vldClassFriendlyName_Required.ClientID + "')" + "; SetName('" + txtClassName.ClientID + "','" + txtClassPluralName.ClientID + "','" + vldClassPluralName_Required.ClientID + "')");
				txtFieldName.Attributes.Add("onblur", "SetName('" + txtFieldName.ClientID + "','" + txtFieldFriendlyName.ClientID + "','" + vldFieldFriendlyName_Required.ClientID + "')");
			}
			else
			{
				// Class
				txtClassName.Text = _mc.Name;
				txtClassName.Enabled = false;

				txtClassFriendlyName.Text = _mc.FriendlyName;
				txtClassPluralName.Text = _mc.PluralName;

				string ownerType = _mc.Attributes.GetValue<OwnerTypes>(MetaDataWrapper.OwnerTypeAttr, OwnerTypes.Undefined).ToString();
				ddlOwnerType.Items.Add(new ListItem((string)GetGlobalResourceObject("GlobalMetaInfo", ownerType), ownerType));
				ddlOwnerType.Enabled = false;

				chkSupportsCards.Checked = _mc.SupportsCards;
				chkSupportsCards.Enabled = false;

				// Field
				MetaField mf = MetaDataWrapper.GetTitleField(_mc);

				txtFieldName.Text = mf.Name;
				txtFieldName.Enabled = false;

				txtFieldFriendlyName.Text = mf.FriendlyName;

				txtMaxLen.Text = string.Format(CultureInfo.CurrentUICulture, "{0}", mf.Attributes[McDataTypeAttribute.StringMaxLength]);
				txtMaxLen.Enabled = false;
			}
		}
		#endregion

		#region btnSave_ServerClick
        /// <summary>
        /// Handles the ServerClick event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnSave_ServerClick(object sender, EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			if (_mc == null) // Create
			{

				OwnerTypes ownerType = (OwnerTypes)Enum.Parse(typeof(OwnerTypes), ddlOwnerType.SelectedValue);
				try
				{
					_mc = MetaDataWrapper.CreateMetaClass(txtClassName.Text.Trim(), txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(), ownerType, chkSupportsCards.Checked, txtFieldName.Text.Trim(), txtFieldFriendlyName.Text.Trim(), int.Parse(txtMaxLen.Text));
					Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", _mc.Name), true);
				}
				catch (MetaClassAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("GlobalMetaInfo", "TableExistsErrorMessage").ToString(), "'" + txtClassName.Text.Trim() + "'");
					lbError.Visible = true;
				}
				catch (MetaFieldAlreadyExistsException)
				{
					lbError.Text = string.Format(GetGlobalResourceObject("GlobalMetaInfo", "FieldExistsErrorMessage").ToString(), "'" + txtFieldName.Text.Trim() + "'");
					lbError.Visible = true;
				}
			}
			else // Update
			{
				MetaDataWrapper.UpdateMetaClass(_mc, txtClassFriendlyName.Text.Trim(), txtClassPluralName.Text.Trim(), txtFieldFriendlyName.Text.Trim());

				if (Back == "list")
					Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/MetaClassList.aspx", true);
				else
					Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/MetaClassView.aspx?class={0}", _mc.Name), true);
			}
		}
		#endregion
	}
}