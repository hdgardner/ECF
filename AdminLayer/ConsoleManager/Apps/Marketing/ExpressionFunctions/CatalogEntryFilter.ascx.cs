using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Text;

namespace Mediachase.Commerce.Manager.Apps.Marketing.ExpressionFunctions
{
	public partial class CatalogEntryFilter : System.Web.UI.UserControl, IConditionValue
	{
		public string ControlId
		{
			get 
			{
				if (ViewState["_ControlID"] != null)
					return ViewState["_ControlID"].ToString();
				return string.Empty;
			}
			set 
			{
				ViewState["_ControlID"] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			Dictionary<string, string> dic = new Dictionary<string,string>();
			dic.Add("ExpressionId", "-1");
			dic.Add("ControlId", ControlId);
			string ids = String.Empty;
			if (Value != null)
			{
				Dictionary<string, string> dicItems = Value as Dictionary<string, string>;
				if (dicItems != null && dicItems.Count > 0)
				{
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < dicItems.Count; i++)
					{
						sb.Append(dicItems.ElementAt(i).Key);
						sb.Append(",");
					}
					if(sb.Length>1)
						sb.Remove(sb.Length - 1, 1);
					ids = sb.ToString();
				}
			}
			dic.Add("CatalogEntryIds", ids);

			string s = CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, "Marketing", "Promotion-Edit", new CommandParameters("cmdShowEditorDialog_PromotionCatalogEntrySelect", dic));
			lbSelectedEntries.Attributes.Add("onclick", s);
		}

		#region IConditionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node, ConditionElement condition)
		{
			string controlId = node["TmpKey"];
			if (controlId == null)
			{
				controlId = Guid.NewGuid().ToString();
				node["TmpKey"] = controlId;
			}
			ControlId = controlId;
			Dictionary<string, string> dicItems = Value as Dictionary<string, string>;
			if (dicItems != null && dicItems.Count>0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(dicItems.ElementAt(0).Value);
				if (dicItems.Count>1)
					sb.Append("+ (" + (dicItems.Count-1).ToString()+")");
				lbSelectedEntries.Text = sb.ToString();
			}
			else
			{
				lbSelectedEntries.Text = "Please select catalog entries";
				
			}
		}

		public object Value
		{
			get
			{
				return Session["CatalogEntryFilter_SelectedItems_" + ControlId];
			}
			set
			{
				Session["CatalogEntryFilter_SelectedItems_" + ControlId] = value;
			}
		}

		#endregion
	}
}