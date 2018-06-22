using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Apps.Marketing.ExpressionFunctions
{
	public partial class actionRewardOneParam1 : System.Web.UI.UserControl, IFunctionValue
	{
		private MethodElementParams _params = null;

		#region prop: Scale
		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		/// <value>The scale.</value>
		private int Scale
		{
			get
			{
				if (ViewState["_Scale"] == null)
					return 2;
				return Convert.ToInt32(ViewState["_Scale"].ToString(), CultureInfo.InvariantCulture);
			}
			set { ViewState["_Scale"] = value; }
		}
		#endregion

		#region prop: Precision
		/// <summary>
		/// Gets or sets the precision.
		/// </summary>
		/// <value>The scale.</value>
		private int Precision
		{
			get
			{
				if (ViewState["Precision"] == null)
					return 6;
				return Convert.ToInt32(ViewState["Precision"].ToString(), CultureInfo.InvariantCulture);
			}
			set { ViewState["Precision"] = value; }
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblText.Style.Add(HtmlTextWriterStyle.Display, "inline");

			lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbText1.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText1, "")));
			tbText1.TextChanged += new EventHandler(tbText1_TextChanged);
			lblError.Text = string.Empty;
		}

		#region tbText1_TextChanged
		/// <summary>
		/// Handles the TextChanged event of the tbText1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tbText1_TextChanged(object sender, EventArgs e)
		{
			CultureInfo ci = CultureInfo.CurrentCulture;
			double val = 0;

			int startPos = tbText1.Text.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
			string valStr = tbText1.Text;
			if (startPos >= 0 && startPos + this.Scale + 1 <= valStr.Length)
			{
				valStr = valStr.Substring(0, startPos + this.Scale + 1);
			}

			if (double.TryParse(valStr, out val) )
			{
				lblError.Text = string.Empty;
				ParamValue = val;
				//BindFromValue();
				this.RaiseBubbleEvent(this, e);
			}
			else
			{
				lblError.Text = "*";
			}

			BindFromValue();
		}
		#endregion

		#region BindFromValue
		/// <summary>
		/// Binds from value.
		/// </summary>
		private void BindFromValue()
		{
			ParamValue = ParamValue;
		}
		#endregion

		private double ParamValue
		{
			get
			{
				double retVal = 0;
				if (Value.Count != 0)
				{
					retVal = Convert.ToDouble(Value.First());
				}
				return retVal;
			}
			set
			{
				lblText.Text = value.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
				tbText1.Text = lblText.Text;
				this.Value.Clear();
				this.Value.Add(value);
			}
		}

		#region IFunctionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node)
		{
			BindFromValue();
		}

		public MethodElementParams Value
		{
			get
			{
				if (ViewState["_params"] == null)
					ViewState["_params"] = new MethodElementParams();
				return (MethodElementParams)ViewState["_params"];

			}
			set
			{
				ViewState["_params"] = value;
			}
		}

		#endregion
	}
}