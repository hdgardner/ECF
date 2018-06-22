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
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class ExpressionList : MarketingBaseUserControl
    {
        /// <summary>
        /// Gets the expression category.
        /// </summary>
        /// <value>The expression category.</value>
		public string ExpressionCategory
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["group"], String.Empty);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
		}

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
		private void LoadDataAndDataBind(string sortExpression)
		{
			ExpressionDto dto = ExpressionManager.GetExpressionDto(ExpressionCategory);
			if (dto.Expression != null)
			{
				DataView view = dto.Expression.DefaultView;
				view.Sort = sortExpression;
				MyListView.DataSource = view;
			}

			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("ExpressionId");
			MyListView.DataBind();
		}

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);
			base.OnInit(e);
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
		}

        /// <summary>
        /// Handles the Sorting event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.ListViewSortEventArgs"/> instance containing the event data.</param>
		void CurrentListView_Sorting(object sender, ListViewSortEventArgs e)
		{
			AdminView view = MyListView.CurrentListView.GetAdminView();
			foreach (ViewColumn column in view.Columns)
			{
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					LoadDataAndDataBind(sortExpression);
				}
			}
		}

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if ((IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID)) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}