using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class EnumEdit : System.Web.UI.UserControl
	{
		#region EnumName
        /// <summary>
        /// Gets the name of the enum.
        /// </summary>
        /// <value>The name of the enum.</value>
		public string EnumName
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
			BindTooblar();

			if (!IsPostBack)
				BindData();
		}

		#region Events
        /// <summary>
        /// Handles the ServerClick event of the imbtnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void imbtnCancel_ServerClick(object sender, EventArgs e)
		{
			if (BackMode == "view" && EnumName != string.Empty)
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/EnumView.aspx?type=" + EnumName);
			else
				Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/EnumList.aspx");
		}

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
			if (EnumName == string.Empty)
			{
				MetaFieldType type = MetaEnum.Create(txtEnumName.Text.Trim(), txtFriendlyName.Text.Trim(), chkMultiValue.Checked);
				Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/EnumView.aspx?type={0}", type.Name));
			}
			else
			{
				MetaDataWrapper.UpdateEnumFriendlyName(EnumName, txtFriendlyName.Text.Trim());
				Response.Redirect(String.Format("~/Apps/MetaDataBase/Pages/Admin/EnumView.aspx?type={0}", EnumName));
			}

		} 
		#endregion

		#region BindTooblar
        /// <summary>
        /// Binds the tooblar.
        /// </summary>
		private void BindTooblar()
		{
			if (EnumName == string.Empty)
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "EnumCreate").ToString();
			else
				secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "EnumEdit").ToString();

			if (BackMode == "view" && EnumName != string.Empty)
				secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToEnumInfo").ToString(), "~/Apps/MetaDataBase/Pages/Admin/EnumView.aspx?type=" + EnumName);
			else
				secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/EnumList.aspx");

			imbtnSave.CustomImage = Page.ResolveClientUrl("../../images/saveitem.gif");
			imbtnCancel.CustomImage = Page.ResolveClientUrl("../../images/cancel.gif");

			txtEnumName.Attributes.Add("onblur", "SetName('" + txtEnumName.ClientID + "','" + txtFriendlyName.ClientID + "','" + vldFriendlyName_Required.ClientID + "')");
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			if (EnumName != string.Empty)
			{
				MetaFieldType mft = MetaDataWrapper.GetEnumByName(EnumName);
				if (mft != null)
				{
					txtEnumName.Text = mft.Name;
					txtEnumName.ReadOnly = true;
					txtEnumName.CssClass = "text-readonly";
					txtFriendlyName.Text = mft.FriendlyName;
					if (mft.Attributes.ContainsKey(McDataTypeAttribute.EnumMultivalue) && mft.Attributes[McDataTypeAttribute.EnumMultivalue] != null)
						chkMultiValue.Checked = (bool)mft.Attributes[McDataTypeAttribute.EnumMultivalue];
					chkMultiValue.Enabled = false;
				}
			}
		}
		#endregion
	}
}