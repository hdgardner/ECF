using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Dashboard
{
	public partial class Home : BasePage
	{
		internal readonly static string _DefaultTemplate = "1FDECA13-4F00-4282-8688-745A2AFBF6F6";

		internal readonly static string _DefaultControls =
			"[{\"items\":[{\"id\":\"54F95808-867C-4250-BA0F-CA79380AA4A1\",\"collapsed\":false,\"instanceUid\":\"FB09B70B_CC52_465a_8564_2CC447071DC7\"}," +
				"{\"id\":\"F1946D0E-D000-4a0b-A995-AAB83A8209CE\",\"collapsed\":false, \"instanceUid\":\"3476AE4B_8238_4418_BB8E_B391DC178370\"}," +
				"{\"id\":\"7F9C4EE7-F996-40b1-810C-91CF987F5108\",\"collapsed\":false, \"instanceUid\":\"CF6C3C4D_BD6F_41f8_854A_91D4BC9E9483\"}]," +
				"\"id\":\"Column1\"}," +
			"{\"items\":[{\"id\":\"E49AE893-154A-4c34-BCEC-4D091A8C8415\",\"collapsed\":false, \"instanceUid\":\"E5930722_8D94_4576_9C3C_4F0CA94BA243\"}," +
				"{\"id\":\"2CB461C8-91D6-44bb-979D-E2CFFC864DAA\",\"collapsed\":false, \"instanceUid\":\"F1478908_92B6_4fd1_ABFE_BA10C61F0D84\"}]," +
				"\"id\":\"Column2\"}]";

		private readonly static string _dashboardPageUid = "dashboardPageMain";

		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		/// <summary>
		/// Gets or sets the page uid.
		/// </summary>
		/// <value>The page uid.</value>
		public string PageUid
		{
			get
			{
				if (ViewState["_PageUid"] != null)
					return ViewState["_PageUid"].ToString();

				return _dashboardPageUid;
			}
			set
			{
				ViewState["_PageUid"] = value;
			}
		}

		private string GetUserTemplateSettings()
		{
			string userSettings = String.Empty;
			CustomerProfile profile = ProfileContext.Current.Profile;
			if (profile != null && profile.PageSettings != null)
				userSettings = profile.PageSettings.GetSettingString(MakeSettingsKey());

			if (String.IsNullOrEmpty(userSettings))
				userSettings = _DefaultTemplate;

			return userSettings;
		}

		private string MakeSettingsKey()
		{
			return CustomerProfile.TemplateSettingsBaseKey + this.PageUid;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			Control divContainer = CommandManager.GetCurrent(this.Page).Parent.FindControl(CommandManager.GetCurrent(this.Page).ContainerId);
			if (divContainer != null)
				cpManagerExtender.ContainerId = divContainer.ClientID;

			CommandParameters cp = new CommandParameters("cmdCoreLayoutPropertyPage");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("controlUid", "%controlUid%");
			cp.CommandArguments = dic;

			cpManagerExtender.PropertyPageCommand = CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, "Core", "LayoutManage", cp);
			cpManagerExtender.DeleteMessage = "Are you sure you want to remove the block?";

			BindScripts();
		}

        /// <summary>
        /// Binds the scripts.
        /// </summary>
		private void BindScripts()
		{
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtBase", ResolveClientUrl("~/Scripts/ext/ext-base.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtAll", ResolveClientUrl("~/Scripts/ext/ext-all.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ManagementClientProxy", ResolveClientUrl("~/Scripts/ManagementClientProxy.js"));
			
		    string url = "~/Apps/Dashboard/Scripts";
		    string path = Server.MapPath(url);
		    if (Directory.Exists(path))
		    {
		        foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
		        {
		            this.ClientScript.RegisterClientScriptInclude(file.Name, this.ResolveClientUrl(String.Format("{0}/{1}", url, file.Name)));
		        }
		    }

			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "ExtBlankInit", "Ext.BLANK_IMAGE_URL = 'images/s.gif';", true);

			// set page title
			string scriptKey = "CSManagementClient.SetPageTitle('" + UtilHelper.GetResFileString("{DashboardStrings:Dashboard_Home}") + "');";
			Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "Dashboard_PageTitleKey", scriptKey, true);

			// register portlet scripts
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Apps/Core/Layout/Scripts/Portal.js"));
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Apps/Core/Layout/Scripts/PortalColumn.js"));
			//ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Apps/Core/Layout/Scripts/Portlet.js"));
			ScriptManager.GetCurrent(this.Page).ScriptMode = ScriptMode.Release;

			ScriptManager.GetCurrent(this.Page).EnablePageMethods = true;
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);

			cpManager.DataSource = GetUserTemplateSettings();
			cpManager.PageUid = this.PageUid;
			cpManagerExtender.PageUid = this.PageUid;
			cpManager.DataBind();
		}
	}
}
