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
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Core.StoreLogs
{
	public partial class SystemLog : BaseUserControl
    {
        protected int GetMaximumRows()
        {
			return EcfListView.GetSavedPageSize(this.Page, MyListView1.ViewId, EcfListView.DefaultPageSize);
        }

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
                MyListView1.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("LogId");

                LoadDataAndDataBind();

                InitDataSource(_StartRowIndex, GetMaximumRows(), true);
                DataBind();
            }

            if (String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
            {
                InitDataSource(_StartRowIndex, GetMaximumRows(), true);
                DataBind();
            }
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
		private void LoadDataAndDataBind()
        {
            
        }

		/// <summary>
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
            MyListView1.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
            MyListView1.CurrentListView.PagePropertiesChanging += new EventHandler<PagePropertiesChangingEventArgs>(CurrentListView_PagePropertiesChanging);

			Page.LoadComplete += new EventHandler(Page_LoadComplete);
            btnSearch.Click += new EventHandler(btnSearch_Click);
			base.OnInit(e);
		}

        /// <summary>
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
        private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount)
        {
            MyListView1.DataSourceID = ApplicationLogDataSource1.ID;

            // fill in search parameters
            ApplicationLogDataSource1.Options.Namespace = String.Empty;

            ApplicationLogDataSource1.Parameters.SourceKey = "system";
            ApplicationLogDataSource1.Parameters.Operation = Operation.Text;
            ApplicationLogDataSource1.Parameters.ObjectType = ObjectType.Text;

            DateTime dt = DateTime.MinValue;
            if (DateTime.TryParse(Created.Text, out dt))
            {
                ApplicationLogDataSource1.Parameters.Created = dt;
            }

            ApplicationLogDataSource1.Options.RecordsToRetrieve = recordsCount;
            ApplicationLogDataSource1.Options.StartingRecord = startRowIndex;
            ApplicationLogDataSource1.Options.ReturnTotalCount = true;
        }

        /// <summary>
        /// Handles the PagePropertiesChanging event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.PagePropertiesChangingEventArgs"/> instance containing the event data.</param>
		void CurrentListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
            //_MaximumRows = e.MaximumRows;
            _StartRowIndex = e.StartRowIndex;
		}

        /// <summary>
        /// Handles the PagePropertiesChanged event of the CurrentListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            InitDataSource(_StartRowIndex, GetMaximumRows(), true);
            DataBind();
        }

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
            if (ManagementHelper.GetBindGridFlag(MyListView1.CurrentListView.ID))
            {
                // reset start index
                _StartRowIndex = 0;

                InitDataSource(_StartRowIndex, GetMaximumRows(), true);
                DataBind();
                MyListView1.MainUpdatePanel.Update();
            }
		}

        /// <summary>
        /// Handles the Click event of the btnSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void btnSearch_Click(object sender, EventArgs e)
        {
            _StartRowIndex = 0;
            InitDataSource(_StartRowIndex, GetMaximumRows(), true);
            MyListView1.ResetPageNumber();
            DataBind();
            MyListView1.MainUpdatePanel.Update();

            btnSearch.Enabled = true;
            upSearchButton.Update();
        }
    }
}