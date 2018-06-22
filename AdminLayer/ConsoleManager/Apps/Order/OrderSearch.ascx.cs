using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Web.Console.Config;
using Mediachase.Commerce.Orders.DataSources;
using Resources;

namespace Mediachase.Commerce.Manager.Order
{
    public partial class OrderSearchList : OrderBaseUserControl
    {
		private const string _ShoppingCartClass = "ShoppingCart";
		private const string _PaymentPlanClass = "PaymentPlan";

		int _MaximumRows = 20;
		int _StartRowIndex = 0;

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
					MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("OrderGroupId", "CustomerId");

				LoadDataAndDataBind();

				if (IsPostBack)
					InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);

				DataBind();
			}
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
		private void LoadDataAndDataBind()
		{
			OrderStatusDto statusDto = OrderStatusManager.GetOrderStatus();
			OrderStatus.DataValueField = "Name";
			OrderStatus.DataSource = statusDto.OrderStatus;
			OrderStatus.DataBind();
			OrderStatus.Items.Insert(0, new ListItem("any", ""));
            
			DataRange.Items.Clear();
			DataRange.Items.Add(new ListItem(SharedStrings.All, ""));
			DataRange.Items.Add(new ListItem(SharedStrings.Today, "today"));
			DataRange.Items.Add(new ListItem(SharedStrings.This_Week, "thisweek"));
			DataRange.Items.Add(new ListItem(SharedStrings.This_Month, "thismonth"));

			StringBuilder script = new StringBuilder("this.disabled = true;\r\n");
			script.AppendFormat("__doPostBack('{0}', '');", btnSearch.UniqueID);
			btnSearch.OnClientClick = script.ToString();
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
					// reset start index
					_StartRowIndex = 0;

					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					InitDataSource(_StartRowIndex, _MaximumRows, true, sortExpression);
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
			//if (IsPostBack && ManagementHelper.GetBindGridFlag(MyListView.CurrentListView.ID))
			//{
			//    // reset start index
			//    _StartRowIndex = 0;

			//    InitDataSource(_StartRowIndex, _MaximumRows, true, MyListView.CurrentListView.SortExpression);
			//    DataBind();
			//    MyListView.MainUpdatePanel.Update();
			//}
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
        /// Inits the data source.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="recordsCount">The records count.</param>
        /// <param name="returnTotalCount">if set to <c>true</c> [return total count].</param>
        /// <param name="orderByClause">The order by clause.</param>
		private void InitDataSource(int startRowIndex, int recordsCount, bool returnTotalCount, string orderByClause)
		{
			MyListView.DataSourceID = OrderListDataSource.ID;

			DateTime nowDate = DateTime.UtcNow;
			DateTime startDate = DateTime.UtcNow;
            DateTime endDate = DateTime.UtcNow;
			bool applyDateFilter = false;

			String filterType = DataRange.SelectedValue;

			if (String.Compare(filterType, "thisweek", true) == 0)
			{
				startDate = nowDate.AddDays(-nowDate.DayOfWeek.GetHashCode());
				endDate = nowDate;
				applyDateFilter = true;
			}
			else if (String.Compare(filterType, "thismonth", true) == 0)
			{
				startDate = nowDate.AddDays(-nowDate.Day);
				endDate = nowDate;
				applyDateFilter = true;
			}
			else if (String.Compare(filterType, "today", true) == 0)
			{
                startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
				endDate = nowDate;
				applyDateFilter = true;
			}
			else
			{
				applyDateFilter = false;
			}

			if (applyDateFilter)
                OrderListDataSource.Parameters.SqlMetaWhereClause = String.Format("META.Modified between '{0}' and '{1}'", startDate.ToString("s"), endDate.ToString("s"));

			StringBuilder sqlWhereClause = new StringBuilder("(1=1)");

			int orderId = 0;
			if (int.TryParse(OrderNumber.Text, out orderId) && orderId > 0)
				sqlWhereClause.AppendFormat(" AND (OrderGroupId = {0})", orderId);
			
			string status = OrderStatus.SelectedValue;
			if (!String.IsNullOrEmpty(status))
				sqlWhereClause.AppendFormat(" AND (Status = '{0}')", status);

			if (String.IsNullOrEmpty(orderByClause))
				orderByClause = String.Format("OrderGroupId DESC");

			//search by CustomerName
			if (!String.IsNullOrEmpty(CustomerKeyword.Text))
			{
				StringBuilder customerWhereClause = new StringBuilder();
				foreach (string keyword in CustomerKeyword.Text.Split(' '))
				{
					if (customerWhereClause.Length > 0)
						customerWhereClause.Append(" OR ");
					customerWhereClause.AppendFormat(" CustomerName LIKE '%{0}%'", keyword.Replace("'", "''"));
				}
				sqlWhereClause.AppendFormat(" AND ({0})", customerWhereClause);
			}

			OrderListDataSource.Parameters.SqlWhereClause = sqlWhereClause.ToString();

			OrderSearchOptions options = new OrderSearchOptions();
			OrderListDataSource.Options.Namespace = "Mediachase.Commerce.Orders";

			string classType = ClassType.SelectedValue;
			OrderListDataSource.Options.Classes.Add(classType);

			if (String.Compare(classType, _ShoppingCartClass, true) == 0)
				MyListView.DataMember = OrderDataSource.OrderDataSourceView.CartsViewName;
			else if (String.Compare(classType, _PaymentPlanClass, true) == 0)
				MyListView.DataMember = OrderDataSource.OrderDataSourceView.PaymentPlansViewName;
			else
				MyListView.DataMember = OrderDataSource.OrderDataSourceView.PurchaseOrdersViewName;

			OrderListDataSource.Options.RecordsToRetrieve = recordsCount;
			OrderListDataSource.Options.StartingRecord = startRowIndex;
			OrderListDataSource.Parameters.OrderByClause = orderByClause;
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