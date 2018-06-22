using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Controls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Order
{
	public partial class Jurisdictions : OrderBaseUserControl
	{
		private const string _BaseSortKey = "SortExpression_";

		/// <summary>
		/// Gets the JurisdictionType.
		/// </summary>
		/// <value>The JurisdictionType.</value>
		public JurisdictionManager.JurisdictionType JurisdictionType
		{
			get
			{
				JurisdictionManager.JurisdictionType jType = JurisdictionManager.JurisdictionType.Shipping;

				string type = ManagementHelper.GetStringValue(Request.QueryString["type"], String.Empty);
				if (!String.IsNullOrEmpty(type))
				{
					try
					{
						jType = (JurisdictionManager.JurisdictionType)Enum.Parse(typeof(JurisdictionManager.JurisdictionType), type);
					}
					catch { }
				}

				return jType;
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
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			MyListView.CurrentListView.Sorting += new EventHandler<ListViewSortEventArgs>(CurrentListView_Sorting);
			MyListView.CurrentListView.PagePropertiesChanged += new EventHandler(CurrentListView_PagePropertiesChanged);
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
			{
				// find the column which is to be sorted
				if (column.AllowSorting && String.Compare(column.GetSortExpression(), e.SortExpression, true) == 0)
				{
					// update DataSource parameters
					string sortExpression = e.SortExpression + " " + (e.SortDirection == SortDirection.Descending ? "DESC" : "ASC");
					LoadDataAndDataBind(sortExpression);
					ProfileContext.Current.Profile.PageSettings.SetSettingString(SortExpressionSettingKey, sortExpression);
					ProfileContext.Current.Profile.Save();
					break;
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
				LoadDataAndDataBind(ProfileContext.Current.Profile.PageSettings.GetSettingString(SortExpressionSettingKey));
				MyListView.MainUpdatePanel.Update();
			}
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
		/// Loads the data and data bind.
		/// </summary>
		/// <param name="sortExpression">The sort expression.</param>
		private void LoadDataAndDataBind(string sortExpression)
		{
			JurisdictionDto dto = JurisdictionManager.GetJurisdictions(JurisdictionType);
			if (dto.Jurisdiction != null)
			{
				DataView view = dto.Jurisdiction.DefaultView;
				view.Sort = sortExpression;
				MyListView.DataSource = view;
			}

			MyListView.CurrentListView.SetSortProperties(sortExpression);
			MyListView.CurrentListView.PrimaryKeyId = EcfListView.MakePrimaryKeyIdString("JurisdictionId", "JurisdictionType");
			MyListView.DataBind();
		}
	}
}