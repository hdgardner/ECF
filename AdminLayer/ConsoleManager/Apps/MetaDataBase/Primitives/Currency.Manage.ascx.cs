using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
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
	public partial class Currency_Manage : System.Web.UI.UserControl, IManageControl
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
			if (txtDefaultValue.Text.Trim() == String.Empty)
				return String.Empty;
			CultureInfo invariantCulture = new CultureInfo("");
			return Double.Parse(txtDefaultValue.Text).ToString(invariantCulture);
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
				Attr.Add(McDataTypeAttribute.CurrencyAllowNegative, chkAllowNegative.Checked);
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
			decimal defaultValue = 0;
			txtDefaultValue.Text = defaultValue.ToString("f");
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mf">The mf.</param>
		public void BindData(MetaField mf)
		{
			if (mf.DefaultValue != String.Empty)
				txtDefaultValue.Text = ((decimal)DefaultValue.Evaluate(mf)).ToString("f");
			else
				txtDefaultValue.Text = "";

			chkAllowNegative.Checked = (bool)mf.Attributes[McDataTypeAttribute.CurrencyAllowNegative];
		}
		#endregion
	}
}