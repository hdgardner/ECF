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

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class Image_Manage : System.Web.UI.UserControl, IManageControl
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
			return null;
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
				if (txtWidth.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.ImageWidth, int.Parse(txtWidth.Text));
				if (txtHeight.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.ImageHeight, int.Parse(txtHeight.Text));
				Attr.Add(McDataTypeAttribute.ImageShowBorder, chkShowBorder.Checked);
				Attr.Add(McDataTypeAttribute.FileNameRegexPattern, txtRegexPattern.Text);
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

		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mf">The mf.</param>
		public void BindData(MetaField mf)
		{
			txtRegexPattern.Text = mf.Attributes[McDataTypeAttribute.FileNameRegexPattern].ToString();

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.ImageWidth))
				txtWidth.Text = mf.Attributes[McDataTypeAttribute.ImageWidth].ToString();
			else
				txtWidth.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.ImageHeight))
				txtHeight.Text = mf.Attributes[McDataTypeAttribute.ImageHeight].ToString();
			else
				txtHeight.Text = "";

			chkShowBorder.Checked = (bool)mf.Attributes[McDataTypeAttribute.ImageShowBorder];
		}
		#endregion
	}
}