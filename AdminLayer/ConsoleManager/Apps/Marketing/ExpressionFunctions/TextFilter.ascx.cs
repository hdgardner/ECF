using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Globalization;

namespace Mediachase.Commerce.Manager.Apps.Marketing.ExpressionFunctions
{
	public partial class TextFilter : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblText.Style.Add(HtmlTextWriterStyle.Display, "inline");

			lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			tbText1.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", this.Page.ClientScript.GetPostBackEventReference(tbText1, "")));
			tbText1.TextChanged += new EventHandler(tbText1_TextChanged);
			lblError.Text = string.Empty;
		}

		private static bool ValidateUserInputText(string text)
		{
			return !text.Contains("\"");
		}

		#region tbText1_TextChanged
		/// <summary>
		/// Handles the TextChanged event of the tbText1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void tbText1_TextChanged(object sender, EventArgs e)
		{
			string valStr = tbText1.Text;

			if (ValidateUserInputText(valStr))
			{
				lblError.Text = string.Empty;
				this.Value = valStr;
				//BindFromValue();
				this.RaiseBubbleEvent(this, e);
			}
			else
			{
				lblError.Text = "* (remove all \" characters)";
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
			string val = string.Empty;
			if (Value != null)
			{
				val = this.Value.ToString();
			}
			lblText.Text = val;
			tbText1.Text = lblText.Text;
			this.Value = val;
		}
		#endregion


		#region IConditionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			BindFromValue();
		}

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
	}
}