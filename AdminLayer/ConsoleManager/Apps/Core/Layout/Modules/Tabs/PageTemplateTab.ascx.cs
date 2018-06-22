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
using Mediachase.Ibn.Web.UI.Layout;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Core.Layout.Modules.Tabs
{
	public partial class PageTemplateTab : BaseUserControl, IAdminTabControl, IAdminContextControl
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

		#region prop: SelectedUid
		/// <summary>
		/// Gets or sets the selected uid.
		/// </summary>
		/// <value>The selected uid.</value>
		private string SelectedUid
		{
			get
			{
				if (ViewState["__repTemplatesSelected"] != null)
					return ViewState["__repTemplatesSelected"].ToString();

				return String.Empty;
			}
			set
			{
				ViewState["__repTemplatesSelected"] = value.ToUpperInvariant();
			}
		}
		#endregion

		private string GetUserTemplateSettings()
		{
			string userSettings = String.Empty;
			CustomerProfile profile = ProfileContext.Current.Profile;
			if (profile != null && profile.PageSettings != null)
				userSettings = profile.PageSettings.GetSettingString(MakeSettingsKey());

			if (String.IsNullOrEmpty(userSettings))
				userSettings = Mediachase.Commerce.Manager.Dashboard.Home._DefaultTemplate;

			return userSettings;
		}

		private string MakeSettingsKey()
		{
			return CustomerProfile.TemplateSettingsBaseKey + this.PageUid;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindTemplates();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			repTemplates.ItemCommand += new RepeaterCommandEventHandler(repTemplates_ItemCommand);
		}

		void repTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			Button btn = (Button)e.Item.FindControl("btnCommand");

			if (btn != null)
			{
				this.SelectedUid = btn.CommandArgument.ToString();
				BindTemplates();
			}
		}

		private void BindTemplates()
		{
			if (String.IsNullOrEmpty(this.SelectedUid))
				this.SelectedUid = GetUserTemplateSettings();

			repTemplates.DataSource = GenerateTemplatesDataSource();
			repTemplates.DataBind();

			foreach (RepeaterItem item in repTemplates.Items)
			{
			    Button btn = (Button)item.FindControl("btnCommand");
				HtmlGenericControl div = (HtmlGenericControl)item.FindControl("mainItemDiv");

			    if (btn != null && div != null)
			    {
					if (String.Compare(btn.CommandArgument, this.SelectedUid, StringComparison.InvariantCultureIgnoreCase) == 0)
						div.Attributes.Add("class", "customizeWSTemplateItemSelected");

			        div.Attributes.Add("onclick", this.Page.ClientScript.GetPostBackEventReference(btn, btn.CommandArgument));
			    }
			}
		}

		private DataTable GenerateTemplatesDataSource()
		{
			DataTable dt = new DataTable();

			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Description", typeof(string)));
			dt.Columns.Add(new DataColumn("ImageUrl", typeof(string)));

			foreach (WorkspaceTemplateInfo wti in WorkspaceTemplateFactory.GetTemplateInfos())
			{
				DataRow row = dt.NewRow();
				row["Id"] = wti.Uid;
				row["Title"] = UtilHelper.GetResFileString(wti.Title);
				row["Description"] = UtilHelper.GetResFileString(wti.Description);
				row["ImageUrl"] = CommerceHelper.GetAbsolutePath(wti.ImageUrl);

				dt.Rows.Add(row);
			}

			return dt;
		}

		#region IAdminTabControl Members
		public void SaveChanges(IDictionary context)
		{
			if (context == null)
			{
				// update settings
				try
				{
					// save changes
					ProfileContext.Current.Profile.PageSettings.SetSettingString(MakeSettingsKey(), this.SelectedUid);
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
					ProfileContext.Current.Profile.PageSettings.SetSettingString(MakeSettingsKey(), Mediachase.Commerce.Manager.Dashboard.Home._DefaultTemplate);
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