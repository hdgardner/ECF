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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Config;

namespace Mediachase.Commerce.Manager.Profile
{
    public partial class AccountList : ProfileBaseUserControl
    {
		private static readonly string _ProfileNamespace = "Mediachase.Commerce.Profile";

		int _StartRowIndex = 0;

		protected int GetMaximumRows()
		{
			return EcfListView.GetSavedPageSize(this.Page, MyListView.ViewId, EcfListView.DefaultPageSize);
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				if (!IsPostBack)
				{
					MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("ProviderUserKey");
				}

				InitDataSource(_StartRowIndex, GetMaximumRows(), true, String.Empty);
				DataBind();
			}
        }

        /// <summary>
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
		private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount, string orderByClause)
		{
			AccountsDataSource.Options.RecordsToRetrieve = recordsCount;
			AccountsDataSource.Options.StartingRecord = startRowIndex;
			AccountsDataSource.Options.ReturnTotalCount = true;
			AccountsDataSource.Options.Namespace = _ProfileNamespace;
			AccountsDataSource.Options.Classes.Add("Account");
			AccountsDataSource.Parameters.OrderByClause = orderByClause;
		}

        /// <summary>
        /// Raises the <see cref="E:Init"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
			Page.LoadComplete += new EventHandler(Page_LoadComplete);

            base.OnInit(e);
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
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// reset start index
					_StartRowIndex = 0;

					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					InitDataSource(_StartRowIndex, GetMaximumRows(), true, sortExpression);
				}
		}

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
			_StartRowIndex = e.StartRowIndex;
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView.CurrentListView.SortExpression);
		}

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (/*IsPostBack &&*/ ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
			{
				// reset start index
				_StartRowIndex = 0;

				InitDataSource(_StartRowIndex, GetMaximumRows(), true, MyListView.CurrentListView.SortExpression);
				DataBind();
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}