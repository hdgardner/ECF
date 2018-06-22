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
	public partial class Float_Manage : System.Web.UI.UserControl, IManageControl
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
				CultureInfo invariantCulture = new CultureInfo("");
				Mediachase.Ibn.Data.Meta.Management.AttributeCollection Attr = new Mediachase.Ibn.Data.Meta.Management.AttributeCollection();
				if (txtMinValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DoubleMinValue, Double.Parse(txtMinValue.Text));
				if (txtMaxValue.Text.Trim() != String.Empty)
					Attr.Add(McDataTypeAttribute.DoubleMaxValue, Double.Parse(txtMaxValue.Text));
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
			double minValue = -1000000000000.00;
			double maxValue = 1000000000000.00;
			double defaultValue = 0.00;

			txtMinValue.Text = minValue.ToString("f");
			txtMaxValue.Text = maxValue.ToString("f");
			txtDefaultValue.Text = defaultValue.ToString("f");
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mf">The mf.</param>
		public void BindData(MetaField mf)
		{
			CultureInfo invariantCulture = new CultureInfo("");
			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DoubleMinValue))
				txtMinValue.Text = ((double)mf.Attributes[McDataTypeAttribute.DoubleMinValue]).ToString("f");
			else
				txtMinValue.Text = "";

			if (mf.Attributes.ContainsKey(McDataTypeAttribute.DoubleMaxValue))
				txtMaxValue.Text = ((double)mf.Attributes[McDataTypeAttribute.DoubleMaxValue]).ToString("f");
			else
				txtMaxValue.Text = "";

			if (mf.DefaultValue != String.Empty)
				txtDefaultValue.Text = ((double)DefaultValue.Evaluate(mf)).ToString("f");
			else
				txtDefaultValue.Text = "";
		}
		#endregion
	}
}