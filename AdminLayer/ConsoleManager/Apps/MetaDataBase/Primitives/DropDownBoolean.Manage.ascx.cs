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

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class DropDownBoolean_Manage : System.Web.UI.UserControl, IManageControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManageControl Members
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="AllowNulls">if set to <c>true</c> [allow nulls].</param>
        /// <returns></returns>
		public string GetDefaultValue(bool AllowNulls)
		{
			return ddlDefaultValue.SelectedValue;
		}

        /// <summary>
        /// Gets the field attributes.
        /// </summary>
        /// <value>The field attributes.</value>
		public Mediachase.Ibn.Data.Meta.Management.AttributeCollection FieldAttributes
		{
			get
			{
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				Attr.Add(McDataTypeAttribute.BooleanTrueText, txtYesText.Text);
				Attr.Add(McDataTypeAttribute.BooleanFalseText, txtNoText.Text);
				return Attr;
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mc">The mc.</param>
        /// <param name="FieldType">Type of the field.</param>
		public void BindData(MetaClass mc, string FieldType)
		{
			if (ddlDefaultValue.Items.Count == 0)
			{
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("GlobalFieldManageControls", "Yes").ToString(), "1"));
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("GlobalFieldManageControls", "No").ToString(), "0"));
			}
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mf">The mf.</param>
		public void BindData(MetaField mf)
		{
			txtYesText.Text = mf.Attributes[McDataTypeAttribute.BooleanTrueText].ToString();
			txtNoText.Text = mf.Attributes[McDataTypeAttribute.BooleanFalseText].ToString();

			if (ddlDefaultValue.Items.Count == 0)
			{
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("GlobalFieldManageControls", "Yes").ToString(), "1"));
				ddlDefaultValue.Items.Add(new ListItem(GetGlobalResourceObject("GlobalFieldManageControls", "No").ToString(), "0"));
			}

			if ((bool)DefaultValue.Evaluate(mf))
				CHelper.SafeSelect(ddlDefaultValue, "1");
			else
				CHelper.SafeSelect(ddlDefaultValue, "0");
		}
		#endregion
	}
}