﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Apps.Marketing.ExpressionFunctions
{
	public partial class DecimalPercentFilter : System.Web.UI.UserControl, IConditionValue
	{
		#region prop: Scale
		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		/// <value>The scale.</value>
		private int Scale
		{
			get
			{
				return 2;
			}
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
				return 3;
			}
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

			if (double.TryParse(valStr, out val) && val >= 0 && val <= 100)
			{
				lblError.Text = string.Empty;
				this.Value = val;
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
			double val = 0;
			if (Value != null)
			{
				try
				{
					val = double.Parse(this.Value.ToString());
				}
				catch (FormatException)
				{
					//previos other typed value
				}
			}
			lblText.Text = val.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
			tbText1.Text = lblText.Text;
			lblText.Text += "%";
			this.Value = val;
		}
		#endregion

		#region IConditionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			BindFromValue();
		}


		#region prop: Value
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value
		{
			get
			{
				return ViewState["_Value"];
			}
			set
			{
				ViewState["_Value"] = value;
			}
		}
		#endregion
		#endregion

	}
}