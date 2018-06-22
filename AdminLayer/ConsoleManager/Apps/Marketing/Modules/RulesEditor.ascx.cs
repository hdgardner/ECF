using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Controls;
using System.Data;

namespace Mediachase.Commerce.Manager.Apps.Marketing.Modules
{
    public partial class RulesEditor : BaseUserControl, IAdminContextControl
    {
		private const string _SegmentDtoString = "SegmentDto";
		private const string _ExpressionDtoString = "ExpressionDto";
		private const string _RuleEditCommandString = "cmdRuleEdit";
		private const string _FilterExpression = "FilterExpression";

		SegmentDto _segment = null;
		ExpressionDto _expression = null;

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public ExpressionDto DataSource
        {
            set
            {
                this.RulesList.DataSource = value;
            }
            get
            {
                return (ExpressionDto)this.RulesList.DataSource;
            }
        }


		private List<FilterExpressionNodeCollection> ExpressionFilters
		{
			get
			{
				return HttpContext.Current.Session[_FilterExpression] as List<FilterExpressionNodeCollection>;
			}
			set
			{
				HttpContext.Current.Session[_FilterExpression] = value;
			}
		}
		
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CommandManager cm = CommandManager.GetCurrent(this.Page);
            cm.AddCommand("", "Marketing", "RulesEditor", "cmdShowRulesEditorDialog");
			cm.AddCommand("", "Marketing", "RulesEditor", "cmdShowRulesEditorDialogEdit");
			cm.AddCommand("", "Marketing", "RulesEditor", _RuleEditCommandString);

			if (!IsPostBack)
			{
				ExpressionFilters = LoadSerializedFilters();
			}
			ExpressionsPanel.Update();

            /*
            if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
                LoadDataAndDataBind(String.Empty);
             * */
        }

        /*
        private void LoadDataAndDataBind(string sortExpression)
        {
            DataView view = DataSource.Expression.DefaultView;
            view.Sort = sortExpression;
            MyListView.DataSource = view;
            MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("ExpressionId");
            MyListView.DataBind();
        }
         * */

        /// <summary>
        /// Handles the ItemCommand event of the RulesList control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
		protected void RulesList_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (String.Compare(e.CommandName, "DeleteExpression", StringComparison.OrdinalIgnoreCase) == 0)
			{
				//Notify RuleEditDialog for deletetion expression

				// delete expression
				int exprId = 0;
				if (Int32.TryParse(e.CommandArgument.ToString(), out exprId))
				{
					if (_expression != null)
					{
						ExpressionDto.ExpressionRow expressionRow = _expression.Expression.FindByExpressionId(exprId);
						if (expressionRow != null)
						{
							expressionRow.Delete();
						}
					}
				}

				//Delete corresponding FilterExpressionNodeCollection from list serialized collection
				ExpressionFilters.RemoveAt(e.Item.ItemIndex);
			}
			RulesList.DataSource = _expression;
			RulesList.DataBind();
			ExpressionsPanel.Update();
		}

        /// <summary>
        /// Handles the ItemDataBound event of the RulesList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void RulesList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
                ImageButton linkButton = e.Item.FindControl("EditControl") as ImageButton;
				if (linkButton != null)
				{
					linkButton.OnClientClick = GetJSCommandDialog((int)DataBinder.Eval(e.Item.DataItem, "ExpressionId"),
																  e.Item.ItemIndex);
				}

                LinkButton linkControl = e.Item.FindControl("LinkControl") as LinkButton;
				if (linkControl != null)
				{
					linkControl.OnClientClick = GetJSCommandDialog((int)DataBinder.Eval(e.Item.DataItem, "ExpressionId"),
																	e.Item.ItemIndex);
				}


			}
        }

		/// <summary>
		/// Saves the changes.
		/// </summary>
		public void SaveChanges()
		{
			//Serialize FilterExpression in segment table
			if (_segment != null && _segment.Segment.Count > 0)
			{
				SegmentDto.SegmentRow row = _segment.Segment[0];
				row.ExpressionFilter = SerializeFilterExpressions(ExpressionFilters);
			}
		}

		private List<FilterExpressionNodeCollection> LoadSerializedFilters()
		{
			List<FilterExpressionNodeCollection> retVal = null;

			//try first deserialize
			if (_segment != null && _segment.Segment.Count != 0)
			{
				SegmentDto.SegmentRow row = _segment.Segment[0];
				if (!row.IsExpressionFilterNull())
				{
					retVal = DeseralizeFilterExpressions(row.ExpressionFilter);
				}
			}
			if (retVal == null)
			{
				retVal = new List<FilterExpressionNodeCollection>();
			}

			return retVal;
		}

		/// <summary>
		/// Gets the JS command dialog.
		/// </summary>
		/// <param name="expressionId">The expression id.</param>
		/// <returns></returns>
		protected string GetJSCommandDialog(int expressionId)
		{
			return GetJSCommandDialog(expressionId, -1);
		}

		/// <summary>
		/// Gets the JS command dialog.
		/// </summary>
		/// <param name="expressionId">The expression id.</param>
		/// <param name="expressionIdex">The expression idex.</param>
		/// <returns></returns>
		protected string GetJSCommandDialog(int expressionId, int expressionIdex)
		{
			return GetJSCommandDialog(expressionId, expressionIdex,  "cmdShowRulesEditorDialogEdit");
		}

        /// <summary>
        /// Gets the JS command dialog.
        /// </summary>
        /// <param name="expressionId">The expression id.</param>
        /// <returns></returns>
        protected string GetJSCommandDialog(int expressionId, int expressionIndex, string commandName)
        {
			CommandParameters cp = new CommandParameters(commandName);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["ExpressionId"] = expressionId.ToString();
			dic["ExpressionIndex"] = expressionIndex.ToString();
            cp.CommandArguments = dic;
			string cmd = CommandManager.GetCommandString(commandName, dic);
            return String.Format("{0};return false;", cmd);
        }

		/// <summary>
		/// Deseralizes the settings.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private static List<FilterExpressionNodeCollection> DeseralizeFilterExpressions(byte[] rawExpressionFilter)
		{
			List<FilterExpressionNodeCollection> retVal = null;

			if (rawExpressionFilter != null)
			{
				System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream(rawExpressionFilter))
				{
					retVal = formatter.Deserialize(stream) as List<FilterExpressionNodeCollection>;
				}
			}
			return retVal;
		}

		/// <summary>
		/// Serializes the settings binary.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <returns></returns>
		private static byte[] SerializeFilterExpressions(List<FilterExpressionNodeCollection> expressionFilters)
		{
			if (expressionFilters == null)
			{
				throw new ArgumentNullException("expressionFilters");
			}

			byte[] retVal = null;
			System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				formatter.Serialize(stream, expressionFilters);
				retVal = stream.ToArray();
			}
			return retVal;
		}

		#region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(System.Collections.IDictionary context)
		{
			_segment = (SegmentDto)context[_SegmentDtoString];
			_expression = (ExpressionDto)context[_ExpressionDtoString];
		}
		#endregion
	}
}