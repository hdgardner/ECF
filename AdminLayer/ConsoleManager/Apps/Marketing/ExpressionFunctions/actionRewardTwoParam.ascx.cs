using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;

namespace Mediachase.Commerce.Manager.Marketing.ExpressionFunctions
{
	public partial class Count : System.Web.UI.UserControl, IFunctionValue
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
			//Amount
			containerAmount.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblAmountText.Style.Add(HtmlTextWriterStyle.Display, "inline");
			lblAmountText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbAmount.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbAmount, "")));
			tbAmount.TextChanged += new EventHandler(tb_TextChanged);
			lblAmountError.Text = string.Empty;

			//Quantity
			containerQuantity.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblQuantityText.Style.Add(HtmlTextWriterStyle.Display, "inline");
			lblQuantityText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbQuantity.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbQuantity, "")));
			tbQuantity.TextChanged += new EventHandler(tb_TextChanged);
			lblQuantityError.Text = string.Empty;

		}

		private void tb_TextChanged(object sender, EventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				CultureInfo ci = CultureInfo.CurrentCulture;
				double val = 0;

				int startPos = textBox.Text.IndexOf(ci.NumberFormat.NumberDecimalSeparator);
				string valStr = textBox.Text;
				if (startPos >= 0 && startPos + this.Scale + 1 <= valStr.Length)
				{
					valStr = valStr.Substring(0, startPos + this.Scale + 1);
				}

				if (double.TryParse(valStr, out val))
				{
					if (textBox == tbQuantity)
					{
						lblQuantityError.Text = string.Empty;
						Quantity = val;
					}
					else
					{
						lblAmountError.Text = string.Empty;
						Amount = val;
					}
					//BindFromValue();
					this.RaiseBubbleEvent(this, e);
				}
				else
				{
					if (textBox == tbQuantity)
					{
						lblQuantityError.Text = "*";
					}
					else
					{
						lblAmountError.Text = "*";
					}
				}

				BindFromValue();
			}
		}

		#region BindFromValue
		/// <summary>
		/// Binds from value.
		/// </summary>
		private void BindFromValue()
		{
			Quantity = Quantity;
			Amount = Amount;
		}
		#endregion

		private double Quantity
		{
			get
			{
				double retVal = 0;
				if (Value.Count > 1)
				{
					retVal = Convert.ToDouble(Value.ElementAt(1));
				}
				return retVal;
			}
			set
			{
				lblQuantityText.Text = value.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
				tbQuantity.Text = lblQuantityText.Text;
				if (Value.Count == 0)
				{
					Value.Add(0);
					Value.Add(value);
				}
				else if (Value.Count == 1)
				{
					Value.Add(value);
				}
				else
				{
					Value[1] = value;
				}
			}
		}
		private double Amount
		{
			get
			{
				double retVal = 0;
				if (Value.Count > 0)
				{
					retVal = Convert.ToDouble(Value.First());
				}
				return retVal;
			}
			set
			{
				lblAmountText.Text = value.ToString(string.Format("F{0}", this.Scale, this.Precision), System.Globalization.CultureInfo.CurrentCulture);
				tbAmount.Text = lblAmountText.Text;
				if (Value.Count == 0)
				{
					Value.Add(value);
				}
				else
				{
					Value[0] = value;
				}
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
				object retVal = ViewState["_params"];

				if (retVal == null)
				{
					retVal = new MethodElementParams();
					ViewState["_params"] = retVal;
				}

				return retVal as MethodElementParams;


			}
			set
			{
				ViewState["_params"] = value;
			}
		}

		#endregion
	}
}