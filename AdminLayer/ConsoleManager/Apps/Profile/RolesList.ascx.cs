using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Profile
{
    public partial class RolesList : ProfileBaseUserControl
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				LoadDataAndDataBind(String.Empty);
        }

		/// <summary>
		/// Loads the data and data bind.
		/// </summary>
		/// <param name="sortExpression">The sort expression.</param>
		private void LoadDataAndDataBind(string sortExpression)
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("Name", typeof(string)));

			string[] roles = Roles.GetAllRoles();
			if (roles != null && roles.Length > 0)
			{
				foreach (string role in roles)
				{
					DataRow row = table.NewRow();
					row[0] = role;
					table.Rows.Add(row);
				}

				DataView view = table.DefaultView;
				view.Sort = sortExpression;
				MyListView.DataSource = view;
			}

			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("Name");
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
			if (IsPostBack && (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0))
			{
				LoadDataAndDataBind(MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}