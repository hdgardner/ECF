using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.Config;
using System.Collections.Generic;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Controls;
using Mediachase.Cms;
using System.Globalization;
using Mediachase.Commerce.Core.Managers;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class CatalogEntrySearch : CatalogBaseUserControl
    {
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
				MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("CatalogEntryId", "ClassTypeId");

				LoadDataAndDataBind();

                // Dont search on first load
                if (IsPostBack)
                {
                    InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                }

                DataBind();
			}

            if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                DataBind();
            }
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
			BindLanguagesList();
			ManagementHelper.SelectListItem(ListLanguages, CommonSettingsManager.GetDefaultLanguage());

			BindCatalogsList();

			StringBuilder script = new StringBuilder("this.disabled = true;\r\n");
			script.AppendFormat("__doPostBack('{0}', '');", btnSearch.UniqueID);
			btnSearch.OnClientClick = script.ToString();
        }

		/// <summary>
		/// Binds the languages.
		/// </summary>
		private void BindLanguagesList()
		{
			ListLanguages.DataValueField = "LangId";
			ListLanguages.DataTextField = "LangName";

			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
				ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
				ListLanguages.Items.Add(item);
			}
		}

        /// <summary>
        /// Binds the catalogs list.
        /// </summary>
		private void BindCatalogsList()
		{
			CatalogDto dto = CatalogContext.Current.GetCatalogDto();

            ListCatalogs.Items.Add(new ListItem("all catalogs", "[all]"));
            foreach (CatalogDto.CatalogRow row in dto.Catalog.Rows)
            {
                ListItem item = new ListItem(row.Name, row.Name);
                ListCatalogs.Items.Add(item);
            }
            ListCatalogs.Items[0].Selected = true;
			ListCatalogs.DataBind();
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
			MyListView.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);

			Page.LoadComplete += new EventHandler(Page_LoadComplete);

            btnSearch.Click += new EventHandler(btnSearch_Click);

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			//if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			//{
			//    object objArgs = Request.Form["__EVENTARGUMENT"];
			//    if (objArgs != null)
			//    {
			//        Dictionary<string, object> cmd = new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(objArgs.ToString()) as Dictionary<string, object>;
			//        if (cmd != null && cmd.Count > 1)
			//        {
			//            object cmdName = cmd[_CommandName];
			//            if (String.Compare((string)cmdName, _MoveCopyDialogCommand, true) == 0)
			//            {
			//                // reset start index
			//                _StartRowIndex = 0;

			//                // process move/copy command
			//                Dictionary<string, object> args = cmd[_CommandArguments] as Dictionary<string, object>;
			//                if (args != null)
			//                {
			//                    //ProcessMoveCopyCommand(args);
			//                    ManagementHelper.SetBindGridFlag(MyListView.CurrentListView.ID);
			//                }
			//            }
			//        }
			//    }
			//}

            if (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
            {
                // reset start index
                _StartRowIndex = 0;

                InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
                DataBind();
                MyListView.MainUpdatePanel.Update();
            }
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
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
		private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount, string orderByClause)
		{
            MyListView.DataSourceID = CatalogSearchDataSource1.ID;

			// fill in search parameters
			CatalogSearchDataSource1.Options.Namespace = String.Empty;

			// language filter
			CatalogSearchDataSource1.Parameters.Language = ListLanguages.SelectedValue;

			// catalog filter
            if (ListCatalogs.SelectedValue == "[all]")
            {
                foreach (ListItem item in ListCatalogs.Items)
                {
                    if (item.Value == "[all]")
                        continue;

                    CatalogSearchDataSource1.Parameters.CatalogNames.Add(item.Value);
                }
            }
            else if (ListCatalogs.SelectedIndex >= 0)// selected catalogs
			{
				for (int iTmp = 0; iTmp < ListCatalogs.Items.Count; iTmp++)
				{
					ListItem li = ListCatalogs.Items[iTmp];
					if (li.Selected)
						CatalogSearchDataSource1.Parameters.CatalogNames.Add(li.Value);
				}
			}

            if (!String.IsNullOrEmpty(UniqueId.Text))
            {
                int id = 0;
                StringBuilder sqlWhereClause = new StringBuilder("");
                if(Int32.TryParse(UniqueId.Text, out id))
                    sqlWhereClause.AppendFormat("CatalogEntryId = {0}", id);
                else
                    sqlWhereClause.AppendFormat("Code LIKE '%{0}%'", UniqueId.Text);

                CatalogSearchDataSource1.Parameters.SqlWhereClause = sqlWhereClause.ToString();
            }

			CatalogSearchDataSource1.Parameters.OrderByClause = orderByClause;
            CatalogSearchDataSource1.Parameters.FreeTextSearchPhrase = tbKeywords.Text;

			CatalogSearchDataSource1.Options.RecordsToRetrieve = recordsCount;
			CatalogSearchDataSource1.Options.StartingRecord = startRowIndex;
			CatalogSearchDataSource1.Options.ReturnTotalCount = true;
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
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
    }
}