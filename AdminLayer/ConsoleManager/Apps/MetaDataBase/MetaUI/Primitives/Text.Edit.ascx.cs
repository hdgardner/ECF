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
	public partial class Text_Edit : System.Web.UI.UserControl, IEditControl
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

		#region IEditControl Members

		#region Value
		public object Value
		{
			set
			{
				if (value != null)
					txtValue.Text = value.ToString();
			}
			get
			{
				if (AllowNulls && txtValue.Text.Trim() == String.Empty)
					return null;
				else
					return txtValue.Text.Trim();
			}
		}
		#endregion

		#region ShowLabel
		public bool ShowLabel
		{
			set { }
			get { return true; }
		}
		#endregion

		#region Label
		public string Label
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region AllowNulls
		public bool AllowNulls
		{
			set
			{
				vldValue_Required.Visible = !value;
			}
			get
			{
				return !vldValue_Required.Visible;
			}
		}
		#endregion

		#region RowCount
		public int RowCount
		{
			set { }
			get { return 1; }
		}
		#endregion

		#region ReadOnly
		public bool ReadOnly
		{
			set
			{
				txtValue.Enabled = !value;
				vldValue_Required.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
			get
			{
				return !txtValue.Enabled;
			}
		}
		#endregion

		#region LabelWidth
		public string LabelWidth
		{
			set { }
			get { return ""; }
		}
		#endregion

		#region BindData
		public void BindData(MetaField field)
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.StringMaxLength))
				txtValue.MaxLength = (int)field.Attributes[McDataTypeAttribute.StringMaxLength];
			if(field.Attributes.GetValue<bool>(McDataTypeAttribute.StringIsUnique, false))
				ViewState[FieldName + "FieldClassName"] = field.Owner.Name;
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