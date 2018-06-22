using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Management;

namespace Mediachase.Ibn.Web.UI.Apps.MetaDataBase.Modules.ManageControls
{
	public partial class IdentifierEdit : System.Web.UI.UserControl
	{
		#region IdName
        /// <summary>
        /// Gets the name of the id.
        /// </summary>
        /// <value>The name of the id.</value>
		public string IdName
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

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.imbtnCancel.ServerClick += new EventHandler(imbtnCancel_ServerClick);
			this.imbtnSave.ServerClick += new EventHandler(imbtnSave_ServerClick);

			if (!IsPostBack)
				BindData();

			BindToolbar();
		}

		#region BindToolbar
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
		private void BindToolbar()
		{
			secHeader.Title = GetGlobalResourceObject("GlobalMetaInfo", "NewIdentifier").ToString();
			secHeader.AddLink("<img src='" + Page.ResolveClientUrl("../../images/cancel.gif") + "' border='0' align='absmiddle' />&nbsp;" + GetGlobalResourceObject("GlobalMetaInfo", "BackToList").ToString(), "~/Apps/MetaDataBase/Pages/Admin/IdentifierList.aspx");

			imbtnSave.CustomImage = Page.ResolveClientUrl("../../images/saveitem.gif");
			imbtnCancel.CustomImage = Page.ResolveClientUrl("../../images/cancel.gif");

			txtName.Attributes.Add("onblur", "SetName('" + txtName.ClientID + "','" + txtFriendlyName.ClientID + "','" + rfvFriendlyName.ClientID + "')");
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			string str = MetaIdentifierType.Field.ToString();
			ddlScope.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Scope" + str).ToString(), str));
			str = MetaIdentifierType.Class.ToString();
			ddlScope.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Scope" + str).ToString(), str));
			str = MetaIdentifierType.Global.ToString();
			ddlScope.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "Scope" + str).ToString(), str));

			str = MetaIdentifierPeriodType.None.ToString();
			ddlCounterReset.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + str).ToString(), str));
			str = MetaIdentifierPeriodType.Year.ToString();
			ddlCounterReset.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + str).ToString(), str));
			str = MetaIdentifierPeriodType.Quarter.ToString();
			ddlCounterReset.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + str).ToString(), str));
			str = MetaIdentifierPeriodType.Month.ToString();
			ddlCounterReset.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + str).ToString(), str));
			str = MetaIdentifierPeriodType.Day.ToString();
			ddlCounterReset.Items.Add(new ListItem(GetGlobalResourceObject("GlobalMetaInfo", "CounterReset" + str).ToString(), str));
			if (IdName != string.Empty)
			{
				MetaFieldType mft = MetaDataWrapper.GetIdentifierByName(IdName);
				if (mft != null)
				{
					txtName.Text = mft.Name;
					txtName.ReadOnly = true;
					txtName.CssClass = "text-readonly";
					txtFriendlyName.Text = mft.FriendlyName;
					if (mft.Attributes.ContainsKey(McDataTypeAttribute.IdentifierType) && mft.Attributes[McDataTypeAttribute.IdentifierType] != null)
						ddlScope.SelectedValue = mft.Attributes[McDataTypeAttribute.IdentifierType].ToString();
					ddlScope.Enabled = false;
					if (mft.Attributes.ContainsKey(McDataTypeAttribute.IdentifierPeriodType) && mft.Attributes[McDataTypeAttribute.IdentifierPeriodType] != null)
						ddlCounterReset.SelectedValue = mft.Attributes[McDataTypeAttribute.IdentifierPeriodType].ToString();
					if (mft.Attributes.ContainsKey(McDataTypeAttribute.IdentifierMask) && mft.Attributes[McDataTypeAttribute.IdentifierMask] != null)
						txtMask.Text = mft.Attributes[McDataTypeAttribute.IdentifierMask].ToString();
					if (mft.Attributes.ContainsKey(McDataTypeAttribute.IdentifierMaskDigitLength) && mft.Attributes[McDataTypeAttribute.IdentifierMaskDigitLength] != null)
						txtCounterLen.Text = mft.Attributes[McDataTypeAttribute.IdentifierMaskDigitLength].ToString();

				}
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

			if (IdName == string.Empty)
			{
				MetaIdentifier.Create(txtName.Text.Trim(), txtFriendlyName.Text.Trim(),
				 (MetaIdentifierType)Enum.Parse(typeof(MetaIdentifierType), ddlScope.SelectedValue),
				 (MetaIdentifierPeriodType)Enum.Parse(typeof(MetaIdentifierPeriodType), ddlCounterReset.SelectedValue),
				 txtMask.Text.Trim(), int.Parse(txtCounterLen.Text), true);

			}
			else
			{
				MetaDataWrapper.UpdateMetaIdentifier(IdName, txtFriendlyName.Text.Trim(), (MetaIdentifierType)Enum.Parse(typeof(MetaIdentifierType), ddlScope.SelectedValue),
					(MetaIdentifierPeriodType)Enum.Parse(typeof(MetaIdentifierPeriodType), ddlCounterReset.SelectedValue), txtMask.Text.Trim(), int.Parse(txtCounterLen.Text));

			}
			Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/IdentifierList.aspx", true);

		}
		#endregion

		#region imbtnCancel_ServerClick
        /// <summary>
        /// Handles the ServerClick event of the imbtnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void imbtnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Apps/MetaDataBase/Pages/Admin/IdentifierList.aspx", true);
		}
		#endregion
	}
}