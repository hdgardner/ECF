﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;


namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Url_Edit : System.Web.UI.UserControl, IEditControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			uniqValue_Required.ErrorMessage = GetGlobalResourceObject("Global", "MustBeUnique").ToString();
		}

		protected void uniqValue_Required_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;
			if (ViewState[FieldName + "FieldClassName"] != null)
			{
				MetaField field = MetaDataWrapper.GetMetaFieldByName(ViewState[FieldName + "FieldClassName"].ToString(), FieldName);
				if (!MetaObjectProperty.CheckUniqueValue(field, this.Value))
					args.IsValid = false;
			}
		}

		#region IEditControl
		#region BindData
		public void BindData(MetaField field)
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.StringMaxLength))
				txtValue.MaxLength = (int)field.Attributes[McDataTypeAttribute.StringMaxLength];
			if (field.Attributes.ContainsKey(McDataTypeAttribute.StringRegexPattern))
				vldValue_RegEx.ValidationExpression = field.Attributes[McDataTypeAttribute.StringRegexPattern].ToString();
			if (field.Attributes.GetValue<bool>(McDataTypeAttribute.StringIsUnique, false))
				ViewState[FieldName + "FieldClassName"] = field.Owner.Name;
		} 
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			get
			{
				return !vldValue_Required.Visible;
			}
			set
			{
				vldValue_Required.Visible = !value;
			}
		} 
		#endregion

		#region Label
		public string Label
		{
			get
			{
				return "";
			}
			set
			{

			}
		} 
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			get
			{
				return "";
			}
			set
			{

			}
		} 
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			get
			{
				return !txtValue.Enabled;
			}
			set
			{
				txtValue.Enabled = !value;
				vldValue_Required.Enabled = !value;
				vldValue_RegEx.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
		} 
		#endregion

		#region RowCount
		public int RowCount
		{
			get
			{
				return 1;
			}
			set
			{
			}
		} 
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			get
			{
				return true;
			}
			set
			{

			}
		} 
		#endregion

		#region Value
		public object Value
		{
			get
			{
				if (AllowNulls && (txtValue.Text.Trim() == string.Empty))
				{
					return null;
				}
				string txt = txtValue.Text.Trim().ToLower();
				if (txt.IndexOf("://") >= 0 || txt.IndexOf("\\\\") >= 0)
				{
					return txtValue.Text;
				}
				else
				{
					return "http://" + txtValue.Text;
				}
			}
			set
			{
				if (value != null)
				{
					txtValue.Text = value.ToString();
				}
			}
		} 
		#endregion

		#region FieldName
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
		#endregion
	}
}