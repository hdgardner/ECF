using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Profile;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;

namespace Mediachase.Commerce.Manager.Content.Template
{
    public partial class TemplateList : BaseUserControl
    {
		private const string _BaseSortKey = "SortExpression_";
		
        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
				return ManagementHelper.GetValueFromQueryString("lang", String.Empty);
            }
        }

		private string SortExpressionSettingKey
		{
			get
			{
				return _BaseSortKey + ManagementHelper.GetViewIdFromQueryString();
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
				string sortExpression = ProfileContext.Current.Profile.PageSettings.GetSettingString(SortExpressionSettingKey);
				LoadDataAndDataBind(sortExpression);
			}
		}

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
		private void LoadDataAndDataBind(string sortExpression)
        {
            TemplateDto templates = DictionaryManager.GetTemplateDto(0);
            DataView templateView = templates.main_Templates.DefaultView;
			templateView.Sort = sortExpression;
            templateView.RowFilter = String.Format("LanguageCode LIKE '{0}'", LanguageCode);

            MyListView.DataSource = templateView;

			MyListView.CurrentListView.SetSortProperties(sortExpression);
			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("TemplateId");
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
			LoadDataAndDataBind(MyListView.CurrentListView.SortExpression); //ProfileContext.Current.Profile.PageSettings.GetSettingString(SortExpressionSettingKey));
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
					ProfileContext.Current.Profile.PageSettings.SetSettingString(SortExpressionSettingKey, sortExpression);
					ProfileContext.Current.Profile.Save();
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
			if (ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID) || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
			{
				LoadDataAndDataBind(ProfileContext.Current.Profile.PageSettings.GetSettingString(SortExpressionSettingKey)); //MyListView.CurrentListView.SortExpression);
				MyListView.MainUpdatePanel.Update();
			}
		}
    }
}