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

using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Primitives
{
	public partial class DateTime_Manage : System.Web.UI.UserControl, IManageControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region IManage Members
        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="AllowNulls">if set to <c>true</c> [allow nulls].</param>
        /// <returns></returns>
		public string GetDefaultValue(bool AllowNulls)
		{
			string retval = String.Empty;
			if (chkCurrentDateAsDefault.Checked)
			{
				if (chkUseTimeOffset.Checked)
					retval = "getutcdate()";
				else
					retval = "getdate()";
			}
			return retval;
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
				Attr.Add(McDataTypeAttribute.DateTimeMinValue, DateTime.Parse(txtMinValue.Text));
				Attr.Add(McDataTypeAttribute.DateTimeMaxValue, DateTime.Parse(txtMaxValue.Text));
				Attr.Add(McDataTypeAttribute.DateTimeUseUserTimeZone, chkUseTimeOffset.Checked);
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
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DateTimeMinValue))
				txtMinValue.Text = ((DateTime)mf.Attributes[McDataTypeAttribute.DateTimeMinValue]).ToString("yyyy-MM-dd");
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DateTimeMaxValue))
				txtMaxValue.Text = ((DateTime)mf.Attributes[McDataTypeAttribute.DateTimeMaxValue]).ToString("yyyy-MM-dd");
			chkCurrentDateAsDefault.Checked = (DefaultValue.Evaluate(mf) != null);
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DateTimeUseUserTimeZone))
				chkUseTimeOffset.Checked = (bool)mf.Attributes[McDataTypeAttribute.DateTimeUseUserTimeZone];
		}
		#endregion
	}
}