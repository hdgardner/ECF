using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Apps.Marketing.ExpressionFunctions
{
	public partial class BooleanFilter : System.Web.UI.UserControl, IConditionValue
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			container.Style.Add(HtmlTextWriterStyle.Display, "none");
			lblText.Style.Add(HtmlTextWriterStyle.Display, "inline");

			lblText.Attributes.Add("onclick", "this.previousSibling.style.display = 'inline'; this.style.display = 'none';");

			ddlValue.Attributes.Add("onfocus", String.Format("onfocusDefaultHandler(this, \"{0}\");", 
									this.Page.ClientScript.GetPostBackEventReference(ddlValue, "")));
			ddlValue.SelectedIndexChanged += new EventHandler(ddlValue_SelectedIndexChanged);
			ddlValue.Items.Clear();
			ddlValue.Items.Add(new ListItem(true.ToString(), true.ToString().ToLower()));
			ddlValue.Items.Add(new ListItem(false.ToString(), false.ToString().ToLower()));

			lblError.Text = string.Empty;

			BindFromValue();
		}

		private static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}

		protected virtual void ddlValue_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Value = ddlValue.SelectedValue;
				lblError.Text = string.Empty;
				this.RaiseBubbleEvent(this, e);
			}
			catch (FormatException)
			{
				lblError.Text = "*";
			}

			BindFromValue();
		}

		private void BindFromValue()
		{
			bool val = true;
			
			if (this.Value != null)
			{
				bool boolVal;
				if(Boolean.TryParse(this.Value.ToString(), out boolVal))
				{
					val = boolVal;
				}
			}
			string strValue = val.ToString().ToLower();
			Value = strValue;
			lblText.Text = val.ToString();
			SafeSelect(ddlValue, strValue);
		}

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