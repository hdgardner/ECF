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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.Layout;
using Mediachase.Commerce.Profile;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace Mediachase.Commerce.Manager.Core.Layout.Modules.Tabs
{
	public partial class ControlSetTab : BaseUserControl, IAdminTabControl, IAdminContextControl
	{
		#region prop: PageUid
		/// <summary>
		/// Gets the page id.
		/// </summary>
		/// <value>The page id.</value>
		private string PageUid
		{
			get
			{
				if (Request["PageUid"] == null)
					return string.Empty;

				return Request["PageUid"].ToString();
			}
		}
		#endregion

		#region prop: ColumnId
		/// <summary>
		/// Gets the column id.
		/// </summary>
		/// <value>The column id.</value>
		private string ColumnId
		{
			get
			{
				if (Request["ColumnId"] == null)
					return string.Empty;

				return Request["ColumnId"].ToString();
			}
		}
		#endregion

		#region prop: selectedUid
		/// <summary>
		/// Gets or sets the selected uid.
		/// </summary>
		/// <value>The selected uid.</value>
		private string selectedUid
		{
			get
			{
				if (ViewState["__repTemplatesSelected"] != null)
					return ViewState["__repTemplatesSelected"].ToString();

				return string.Empty;
			}
			set
			{
				ViewState["__repTemplatesSelected"] = value.ToUpperInvariant();
			}
		}
		#endregion

		#region prop: IsAdminMode
		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin mode.
		/// If true then control work for administration section
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is admin mode; otherwise, <c>false</c>.
		/// </value>
		public bool IsAdminMode
		{
			get
			{
				if (ViewState["_IsAdminMode"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_IsAdminMode"].ToString());
			}
			set
			{
				ViewState["_IsAdminMode"] = value;
			}
		}
		#endregion

		#region prop: selectedTab
		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public int selectedTab
		{
			get
			{
				if (ViewState["_selectedTab"] == null)
					return 0;

				return Convert.ToInt32(ViewState["_selectedTab"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_selectedTab"] = value;
			}
		}
		#endregion

		#region prop: ForcePostback
		/// <summary>
		/// Gets or sets a value indicating whether [force postback].
		/// </summary>
		/// <value><c>true</c> if [force postback]; otherwise, <c>false</c>.</value>
		public bool ForcePostback
		{
			get
			{
				if (ViewState["_ForcePostback"] == null)
					return false;

				return Convert.ToBoolean(ViewState["_ForcePostback"].ToString());
			}
			set
			{
				ViewState["_ForcePostback"] = value;
			}
		}
		#endregion

		#region prop: ViewMode
		/// <summary>
		/// Gets or sets the view mode.
		/// </summary>
		/// <value>The view mode.</value>
		public string ViewMode
		{
			get
			{
				if (ViewState["_ViewMode"] == null)
					return "0";

				return ViewState["_ViewMode"].ToString();
			}
			set
			{
				ViewState["_ViewMode"] = value;
			}
		}
		#endregion

		#region prop: IsAdmin

		/// <summary>
		/// Gets or sets a value indicating whether this instance is admin.
		/// </summary>
		/// <value><c>true</c> if this instance is admin; otherwise, <c>false</c>.</value>
		public bool IsAdmin
		{
			get
			{
				if (ViewState["_IsAdmin"] == null)
					return true;

				return Convert.ToBoolean(ViewState["_IsAdmin"].ToString(), CultureInfo.InvariantCulture);
			}
			set
			{
				ViewState["_IsAdmin"] = value;
			}
		}
		#endregion

		private CpInfo _currentColumn = null;
		public CpInfo CurrentColumn
		{
			get
			{
				if (_currentColumn == null)
				{
					// get this string from the database
					string userSettings = GetUserPageSettings();

					// deserialize user settings
					System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
					List<CpInfo> list = null;

					try
					{
						list = jss.Deserialize<List<CpInfo>>(userSettings);
					}
					catch
					{
						// something's wrong with user settings, reset it
						//userSettings = null;
					}

					// find requested column
					foreach (CpInfo info in list)
					{
						if (String.Compare(info.Id, this.ColumnId, true) == 0)
						{
							_currentColumn = info;
							break;
						}
					}
				}

				return _currentColumn;
			}
		}

		private string GetUserPageSettings()
		{
			string userSettings = String.Empty;
			CustomerProfile profile = ProfileContext.Current.Profile;
			if (profile != null && profile.PageSettings != null)
				userSettings = profile.PageSettings.GetSettingString(this.PageUid);

			if (String.IsNullOrEmpty(userSettings))
				userSettings = Mediachase.Commerce.Manager.Dashboard.Home._DefaultControls;

			return userSettings;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindGrid();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			grdMain.ItemCreated += new DataGridItemEventHandler(grdMain_ItemCreated);
		}

		void grdMain_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				CpInfo column = CurrentColumn;
				DataGridItem item = e.Item;

				if (column != null && column.Items.Count > 0)
				{
					CheckBox chb = item.FindControl("cbId") as CheckBox;
					if (chb != null)
					{
						DataRowView row = item.DataItem as DataRowView;
						if (row != null)
						{
							foreach (CpInfoItem infoItem in column.Items)
							{
								if (String.Compare(infoItem.Id, (string)row["Id"], true) == 0)
								{
									chb.Checked = true;
									break;
								}
							}
						}
					}
				}
			}
		}

		private void BindGrid()
		{
			grdMain.DataSource = GenerateSource();
			grdMain.DataBind();
		}

		/// <summary>
		/// Generates the source.
		/// </summary>
		/// <returns></returns>
		private DataTable GenerateSource()
		{
			DataTable dt = new DataTable();

			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));

			foreach (DynamicControlInfo dci in DynamicControlFactory.GetControlInfos())
			{
			    DataRow row = dt.NewRow();

			    row["Id"] = dci.Uid;
				row["Title"] = UtilHelper.GetResFileString(dci.Title);
				row["Description"] = UtilHelper.GetResFileString(dci.Description);

			    dt.Rows.Add(row);
			}

			return dt;
		}

		private string[] GetCheckedItems()
		{
			StringBuilder retVal = new StringBuilder();

			foreach (DataGridItem item in grdMain.Items)
			{
				CheckBox cb = (CheckBox)item.FindControl("cbId");

				if (cb != null && cb.Checked)
				{
					retVal.AppendFormat("{0};", item.Cells[0].Text);
				}
			}

			return retVal.ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
		}

		#region IAdminTabControl Members
		public void SaveChanges(IDictionary context)
		{
			if (context == null)
			{
				// update settings
				try
				{
					string currentLayout = string.Empty;

					// 1. Get layout string for selected items
					string[] ids = GetCheckedItems();
					List<CpInfo> userList = null;

					string userSettings = GetUserPageSettings();
					// deserialize current user settings
					System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();

					try
					{
						userList = jss.Deserialize<List<CpInfo>>(userSettings);
					}
					catch
					{
						// something's wrong with user settings
					}

					foreach (CpInfo cpItem in userList)
					{
						if (cpItem.Id == this.ColumnId)
						{
							// clear all existing items
							cpItem.Items.Clear();

							// add selected blocks
							foreach (string id in ids)
							{
								CpInfoItem newItem = new CpInfoItem();
								newItem.Collapsed = "false";
								newItem.Id = id;
								newItem.InstanceUid = Guid.NewGuid().ToString("N");
								cpItem.Items.Add(newItem);
							}
						}
					}

					currentLayout = jss.Serialize(userList);

					// 2. Save changes
					ProfileContext.Current.Profile.PageSettings.SetSettingString(this.PageUid, currentLayout);
					ProfileContext.Current.Profile.Save();
				}
				catch (Exception ex)
				{
					// TODO: handle exception
				}
			}
			else if (context[Mediachase.Ibn.Web.UI.Layout.Modules.AddFramePopup._SaveOptionKey] != null &&
				String.Compare((string)context[Mediachase.Ibn.Web.UI.Layout.Modules.AddFramePopup._SaveOptionKey],
						Mediachase.Ibn.Web.UI.Layout.Modules.AddFramePopup._SaveDefaultKey, true) == 0)
			{
				// reset to default settings
				try
				{
					ProfileContext.Current.Profile.PageSettings.SetSettingString(this.PageUid, Mediachase.Commerce.Manager.Dashboard.Home._DefaultControls);
					ProfileContext.Current.Profile.Save();
				}
				catch (Exception ex)
				{
					// TODO: handle exception
				}
			}
		}
		#endregion

		#region IAdminContextControl Members
		public void LoadContext(IDictionary context)
		{
		}
		#endregion
	}
}