using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Config;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using System.Text;



namespace Mediachase.Commerce.Manager.Apps.Profile
{
	public partial class AccountEntrySearch : System.Web.UI.UserControl
	{
		private static readonly string _ProfileNamespace = "Mediachase.Commerce.Profile";

		int _MaximumRows = 20;
		int _StartRowIndex = 0;

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("PrincipalId");
				StringBuilder script = new StringBuilder("this.disabled = true;\r\n");
				script.AppendFormat("__doPostBack('{0}', '');", btnSearch.UniqueID);
				btnSearch.OnClientClick = script.ToString();
				DataBind();
			}
			if (!IsPostBack)
				BindOrganizationsList();

			if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
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
			MyListView.DataSourceID = AccountsDataSource.ID;
            
			if (ddlOrganizations.SelectedValue != "[all]")
				AccountsDataSource.Parameters.OrganizationNames.Add(ddlOrganizations.SelectedValue);

			StringBuilder sqlWhere = new StringBuilder();
			StringBuilder sqlMetaWhere = new StringBuilder();
			if (!String.IsNullOrEmpty(tbName.Text.Trim()))
			{
				sqlWhere.AppendFormat(" Principal.Name LIKE '%{0}%'", tbName.Text.Trim().Replace("'", "''"));
			}
			if (ddlOrganizations.SelectedValue != "[all]")
			{
				sqlMetaWhere.AppendFormat("OrganizationId={0}", ddlOrganizations.SelectedValue);
			}
			/*if (sb.Length > 0 && !String.IsNullOrEmpty(tbUserName.Text.Trim()))
			{
				sb.Append(" OR ");
			}
			if (!String.IsNullOrEmpty(tbUserName.Text.Trim()))
			{
				sb.AppendFormat("UserName LIKE '%{0}%'", tbUserName.Text.Trim());
			}*/
			/*if (sqlWhere.Length > 0 && !String.IsNullOrEmpty(tbAddress.Text.Trim()))
			{
				sqlWhere.Append(" AND ");
			}*/
			if (sqlWhere.Length > 0 && !String.IsNullOrEmpty(tbAddress.Text.Trim()))
			{
				sqlWhere.Append(" AND ");
			}
			if (!String.IsNullOrEmpty(tbAddress.Text.Trim()))
			{
				sqlWhere.AppendFormat("(CustomerAddress.Name LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'","''"));
				sqlWhere.AppendFormat(" CustomerAddress.FirstName LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.LastName LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.Organization LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.Line1 LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.Line2 LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.City LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.State LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.CountryCode LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.PostalCode LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.RegionCode LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.RegionName LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.DaytimePhoneNumber LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.EveningPhoneNumber LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.FaxNumber LIKE '%{0}%' OR", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.AppendFormat(" CustomerAddress.Email LIKE '%{0}%'", tbAddress.Text.Trim().Replace("'", "''"));
				sqlWhere.Append(" )");
			}
			AccountsDataSource.Parameters.SqlWhereClause = sqlWhere.ToString();
			AccountsDataSource.Parameters.SqlMetaWhereClause = sqlMetaWhere.ToString();

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
			btnSearch.Click += new EventHandler(btnSearch_Click);

			base.OnInit(e);
		}

		void btnSearch_Click(object sender, EventArgs e)
		{
			_StartRowIndex = 0;
			InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
			MyListView.ResetPageNumber();
			DataBind();
			MyListView.MainUpdatePanel.Update();

			btnSearch.Enabled = true;
			upSearchButton.Update();
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
					InitDataSource(_StartRowIndex, _MaximumRows, true, sortExpression);
				}
		}

		/// <summary>
		/// Handles the PagePropertiesChanging event of the CurrentListView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
			_MaximumRows = e.MaximumRows;
			_StartRowIndex = e.StartRowIndex;
		}

		/// <summary>
		/// Handles the PagePropertiesChanged event of the CurrentListView control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
		{
			InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
		}

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			/*if (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
			{
				// reset start index
				_StartRowIndex = 0;

				InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
				DataBind();
				MyListView.MainUpdatePanel.Update();
			}*/
		}

		void BindOrganizationsList()
		{

			int total = 0;

			ProfileSearchParameters pars = new ProfileSearchParameters();
			ProfileSearchOptions options = new ProfileSearchOptions();
			//pars.FreeTextSearchPhrase = sFilter;
			options.Namespace = "Mediachase.Commerce.Profile";
			options.Classes.Add("Organization");

			Organization[] orgs = ProfileContext.Current.FindOrganizations(pars, options, out total);
			ddlOrganizations.Items.Clear();
			ddlOrganizations.Items.Add(new ListItem("[all]", "[all]"));
			if (orgs != null)
			{ 
				foreach(Organization org in orgs)
				{
					ddlOrganizations.Items.Add(new ListItem(org.Name, org.Id.ToString()));
				}
			}
			
		}
	}
}