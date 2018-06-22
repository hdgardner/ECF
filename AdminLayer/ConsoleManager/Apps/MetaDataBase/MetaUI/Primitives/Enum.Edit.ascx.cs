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

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Enum_Edit : System.Web.UI.UserControl, IEditControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IEditControl Members

		#region Value
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		public object Value
		{
			set
			{
				if (value != null)
					CHelper.SafeSelect(ddlValue, value.ToString());
			}
			get
			{
				if (ddlValue.Items.Count > 0 && ddlValue.SelectedValue != "0")
					return int.Parse(ddlValue.SelectedValue);
				else
					return null;
			}
		}
		#endregion

		#region ShowLabel
        /// <summary>
        /// Gets or sets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
        /// <summary>
        /// Gets or sets a value indicating whether [allow nulls].
        /// </summary>
        /// <value><c>true</c> if [allow nulls]; otherwise, <c>false</c>.</value>
		public bool AllowNulls
		{
			set
			{
				ViewState["AllowNulls"] = value;
			}
			get
			{
				if (ViewState["AllowNulls"] != null)
					return (bool)ViewState["AllowNulls"];
				else
					return false;
			}
		}
		#endregion

		#region RowCount
        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
		public bool ReadOnly
		{
			set
			{
				ddlValue.Enabled = !value;
				tdEdit.Visible = !value;
			}
			get
			{
				return !ddlValue.Enabled;
			}
		}
		#endregion

		#region LabelWidth
        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>The width of the label.</value>
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="field">The field.</param>
		public void BindData(MetaField field)
		{
			string sTypeName = field.TypeName;
			ViewState["FieldType"] = sTypeName;
			RebuildList(sTypeName);

			if (!ReadOnly && field.Attributes.ContainsKey(McDataTypeAttribute.EnumEditable)
				&& (bool)field.Attributes[McDataTypeAttribute.EnumEditable])
			{
				tdEdit.Visible = true;

				string url = CHelper.GetAbsolutePath(String.Format("/Common/EnumView.aspx?type={0}&btn={1}", sTypeName, Page.ClientScript.GetPostBackEventReference(btnRefresh, "")));

				btnEditItems.Attributes.Add("onclick", String.Format("OpenWindow(\"{0}\", 750, 500, 1)", url));
			}
			else
			{
				tdEdit.Visible = false;
			}
		}
		#endregion
		#endregion

		#region btnRefresh_Click
        /// <summary>
        /// Handles the Click event of the btnRefresh control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnRefresh_Click(object sender, EventArgs e)
		{
			RebuildList(ViewState["FieldType"].ToString());
		}
		#endregion

		#region RebuildList
        /// <summary>
        /// Rebuilds the list.
        /// </summary>
        /// <param name="sFieldType">Type of the s field.</param>
		private void RebuildList(string sFieldType)
		{
			object savedValue = Value;

			ddlValue.Items.Clear();
			if (AllowNulls)
				ddlValue.Items.Add(new ListItem(CHelper.GetResFileString("{GlobalFieldManageControls:NoValue}"), "0"));


			foreach (MetaEnumItem item in MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes[sFieldType]))
			{
				string text = CHelper.GetResFileString(item.Name);
				ddlValue.Items.Add(new ListItem(text, item.Handle.ToString()));
			}

			Value = savedValue;
		}
		#endregion

		#region IEditControl Members


        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
		public string FieldName
		{
			get
			{
				if (ViewState["FieldName"] != null)
					return ViewState["FieldName"].ToString();
				else
					return "";
			}
			set
			{
				ViewState["FieldName"] = value;
			}
		}

		#endregion
	}
}