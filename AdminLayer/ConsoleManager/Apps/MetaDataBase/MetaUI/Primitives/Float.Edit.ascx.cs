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

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;


namespace Mediachase.Ibn.Web.UI.MetaUI.Primitives
{
	public partial class Float_Edit : System.Web.UI.UserControl, IEditControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.txtValue.Text = this.Page.Request[this.txtValue.UniqueID];
		}
		#region IEditControl
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="field">The field.</param>
		void IEditControl.BindData(MetaField field) 
		{
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DoubleMinValue))
				vldValue_Range.MinimumValue = field.Attributes[McDataTypeAttribute.DoubleMinValue].ToString();
			if (field.Attributes.ContainsKey(McDataTypeAttribute.DoubleMaxValue))
				vldValue_Range.MaximumValue = field.Attributes[McDataTypeAttribute.DoubleMaxValue].ToString();
		}

		// Properties
        /// <summary>
        /// Gets or sets a value indicating whether [allow nulls].
        /// </summary>
        /// <value><c>true</c> if [allow nulls]; otherwise, <c>false</c>.</value>
		bool IEditControl.AllowNulls
		{
			get
			{
				return !this.vldValue_Required.Visible;
			}
			set
			{
				this.vldValue_Required.Visible = !value;
			}
		}
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
		string IEditControl.Label
		{
			get
			{
				return "";
			}
			set
			{
				
			}
		}
        /// <summary>
        /// Gets or sets the width of the label.
        /// </summary>
        /// <value>The width of the label.</value>
		string IEditControl.LabelWidth
		{
			get
			{
				return "";
			}
			set
			{
				
			}
		}
        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
		bool IEditControl.ReadOnly
		{
			get
			{
				return this.txtValue.ReadOnly;
			}
			set
			{
				this.txtValue.ReadOnly = value;
				this.vldValue_Required.Enabled = !value;
				if (value)
				{
					this.txtValue.CssClass = "text-readonly";
				}
			}
		}
        /// <summary>
        /// Gets or sets the row count.
        /// </summary>
        /// <value>The row count.</value>
		int IEditControl.RowCount
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}
        /// <summary>
        /// Gets or sets a value indicating whether [show label].
        /// </summary>
        /// <value><c>true</c> if [show label]; otherwise, <c>false</c>.</value>
		bool IEditControl.ShowLabel
		{
			get
			{
				return true;
			}
			set
			{
				
			}
		}
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
		object IEditControl.Value
		{
			get
			{
				if (((IEditControl)this).AllowNulls && (this.txtValue.Text.Trim() == string.Empty))
				{
					return null;
				}
				return double.Parse(this.txtValue.Text.Trim());
			}
			set
			{
				if (value != null)
				{
					this.txtValue.Text = value.ToString();
				}
			}
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