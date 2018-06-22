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
using Mediachase.Cms;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using System.Threading;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Content.Folders
{
    public partial class CommandList : BaseUserControl
    {
        /// <summary>
        /// Gets the page id.
        /// </summary>
        /// <value>The page id.</value>
        public int PageId
        {
            get
            {
				return ManagementHelper.GetIntFromQueryString("PageId");
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
            {
                SecurityManager.CheckRolePermission("content:site:nav:mng:edit");

                LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
            }
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
			if (IsPostBack &&
				(ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0))
			{
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}

		/// <summary>
		/// Loads the data and data bind.
		/// </summary>
		private void LoadDataAndDataBind(string sortExpression)
		{
			DataTable dt = NavigationManager.GetByPageId(PageId);

			dt.Columns.Add(new DataColumn("Name", typeof(string)));

			foreach (DataRow row in dt.Rows)
			{
				row["Name"] = NavigationManager.GetItemNameById((int)row["ItemId"]);
			}

			DataView view = dt.DefaultView;
			view.Sort = sortExpression;

			MyListView.DataSource = view;

			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("Id", "UrlUID");
			MyListView.DataBind();
		}
    }
}