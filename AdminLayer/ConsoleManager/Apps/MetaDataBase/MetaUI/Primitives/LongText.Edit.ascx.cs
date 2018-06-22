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
	public partial class LongText_Edit : System.Web.UI.UserControl, IEditControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (ReadOnly)
				this.txtValue.Text = this.Page.Request[this.txtValue.UniqueID];
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
				vldValue_Required.Visible = !value;
			}
			get
			{
				return !vldValue_Required.Visible;
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
			set
			{
				txtValue.Height = Unit.Pixel(20 * value + 10 * (value - 1));
			}
			get
			{
				return (int)((txtValue.Height.Value + 10) / 30);
			}
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
				txtValue.ReadOnly = value;
				vldValue_Required.Enabled = !value;
				if (value)
				{
					txtValue.CssClass = "text-readonly";
				}
			}
			get
			{
				return txtValue.ReadOnly;
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

		}
		#endregion
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