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
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class ExpressionEdit : MarketingBaseUserControl
    {
		private const string _ExpressionDtoEditSessionKey = "ECF.ExpressionDto.Edit";
		private const string _ExpressionIdString = "ExpressionId";
		private const string _ExpressionDtoString = "ExpressionDto";

		private ExpressionDto _Expression = null;

        /// <summary>
        /// Gets the expression id.
        /// </summary>
        /// <value>The expression id.</value>
        public int ExpressionId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString(_ExpressionIdString);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();

			// need to pass "group"=Expression Category in QueryString
			// if expression is new, this parameter will be taken from the current QS; otherwise, get it from the expression being edited
			if (ExpressionId > 0 && _Expression != null && _Expression.Expression != null && _Expression.Expression.Count > 0)
			{
				EditSaveControl.SavedClientScript = String.Format("CSManagementClient.ChangeView('Marketing','Expression-List', 'group={0}');", _Expression.Expression[0].Category);
				EditSaveControl.CancelClientScript = String.Format("CSManagementClient.ChangeView('Marketing','Expression-List', 'group={0}');", _Expression.Expression[0].Category);
			}
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
        }

        /// <summary>
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private ExpressionDto LoadFresh()
        {
			ExpressionDto expression = ExpressionManager.GetExpressionDto(ExpressionId);

            // persist in session
            Session[_ExpressionDtoEditSessionKey] = expression;

            return expression;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (ExpressionId > 0)
            {
				ExpressionDto expression = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    expression = LoadFresh();
                }
                else // load from session
                {
					expression = (ExpressionDto)Session[_ExpressionDtoEditSessionKey];

                    if (expression == null)
                    {
                        expression = LoadFresh();
                    }
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
				dic.Add(_ExpressionDtoString, expression);

                // Call tabs load context
                ViewControl.LoadContext(dic);

				_Expression = expression;
            }
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
        {
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			ExpressionDto expression = (ExpressionDto)Session[_ExpressionDtoEditSessionKey]; //null;

			if (ExpressionId > 0 && expression == null)
				expression = ExpressionManager.GetExpressionDto(ExpressionId); //Int32.Parse(Parameters["ExpressionId"].ToString()));
			else if (ExpressionId == 0)
				expression = new ExpressionDto();

            IDictionary context = new ListDictionary();
			context.Add(_ExpressionDtoString, expression);

            ViewControl.SaveChanges(context);

            if (expression.HasChanges())
                ExpressionManager.SaveExpression(expression);

			// we don't need to store Dto in session any more
			Session.Remove(_ExpressionDtoEditSessionKey);
        }
    }
}