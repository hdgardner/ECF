using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Marketing.ExpressionFunctions
{
	public partial class Sum : System.Web.UI.UserControl, IFunctionValue
	{
// 		#region prop: CurrentNode
// 		/// <summary>
// 		/// Gets or sets the current node.
// 		/// </summary>
// 		/// <value>The current node.</value>
// 		private FilterExpressionNode CurrentNode
// 		{
// 			get
// 			{
// 				if (ViewState["NodeElement"] == null)
// 					return null;
// 				return (FilterExpressionNode)ViewState["NodeElement"];
// 			}
// 			set { ViewState["NodeElement"] = value; }
// 		} 
// 		#endregion
// 
// 		#region prop: expressionPlace
// 
// 		/// <summary>
// 		/// Gets or sets the expression place.
// 		/// </summary>
// 		/// <value>The expression place.</value>
// 		private string expressionPlace
// 		{
// 			get
// 			{
// 				if (ViewState["expressionPlace"] == null)
// 					return null;
// 				return ViewState["expressionPlace"].ToString();
// 			}
// 			set { ViewState["expressionPlace"] = value; }
// 		}
// 		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			FieldsList.SelectedIndexChanged += new EventHandler(FieldsList_SelectedIndexChanged);
			if (Value.Count > 0)
			{
				foreach (ListItem item in FieldsList.Items)
				{
					item.Selected = false;
					if (item.Value == ((FilterExpressionNode)Value.First()).Key)
					{
						item.Selected = true;
					}
				}
			}
			else
			{
				if (FieldsList.DataSource != null && ((FilterExpressionNodeCollection)FieldsList.DataSource).Count != 0)
				{
					Value.Add(((FilterExpressionNodeCollection)FieldsList.DataSource)[FieldsList.SelectedIndex]);
				}
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the FieldsList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void FieldsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			Value.Clear();
			//this.CurrentNode.Condition = ((FilterExpressionFunctionParams)this.Parent).ControlContainer.Provider.GetElementConditions(this.expressionPlace, this.CurrentNode)[0];
			Value.Add(((FilterExpressionNodeCollection)FieldsList.DataSource)[FieldsList.SelectedIndex]);
			//Value.Add(this.CurrentNode);
		} 

		#region IFunctionValue Members

		public void BindData(string expressionPlace, string expressionKey, FilterExpressionNode node)
		{
			string[] parts = expressionPlace.Split(':');
			//Change orig place
			if (parts.Length > 1)
			{
				parts[1] = "Sum";
			}
			else
			{
				parts = new string[] { parts[0], "Sum"};
			}
			FieldsList.DataSource = ((FilterExpressionFunctionParams)this.Parent).ControlContainer.Provider.GetNewElements(String.Join(":", parts), node);
			FieldsList.Visible = ((FilterExpressionNodeCollection)FieldsList.DataSource).Count != 0;
			FieldsList.DataTextField = "Name";
			FieldsList.DataValueField = "Key";
			FieldsList.DataBind();
			//this.CurrentNode = node;
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