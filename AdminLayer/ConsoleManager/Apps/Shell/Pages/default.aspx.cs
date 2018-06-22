using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Commerce.Core;
using Mediachase.Ibn.Web.UI.WebControls;
using System.IO;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Apps.Shell.Pages
{
	public partial class _default : BasePage
	{
		protected readonly string _DefaultAddId = "Dashboard";
		protected readonly string _DefaultView = "Home";

		protected string defaultLink = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Apps/Shell/Pages/Empty.html");

		protected string _DefaultViewArray = "[]";

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            //set application name to page title
			Literal1.Text = String.Format("{0} - {1}", AppContext.Current.ApplicationName, UtilHelper.GetResFileString("{SharedStrings:Mediachase_Commerce_Manager_5_0}"));

			Response.Cache.SetNoStore();
			GetDefaultLink();
			RegisterScripts();

			ManagementHelper.RegisterBrowserStyles(this);
		}

		#region GetDefaultLink
        /// <summary>
        /// Gets the default link.
        /// </summary>
		private void GetDefaultLink()
		{
			// get page url
			string s = System.Configuration.ConfigurationManager.AppSettings["ShellFirstPageUrl"];
			if (s.StartsWith("http"))
			{
				defaultLink = s;
				return;
			}
			else if (s.StartsWith("~"))
			{
				s = s.Substring(1);
				s = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(s);
				defaultLink = s;
			}
			else
				defaultLink = Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath(s);

			// append queryString
			StringBuilder sb = new StringBuilder(defaultLink);
			string qs = String.Format("_a={0}&_v={1}", _DefaultAddId, _DefaultView); // default QueryString

			if (Request.QueryString.Count == 0)
			{
                /*
				if (Profile != null && Profile.NavigationState != null &&
					!String.IsNullOrEmpty(Profile.NavigationState.SelectedMenuId) &&
					!String.IsNullOrEmpty(Profile.NavigationState.SelectedContentNameId))
					qs = "_a=" + Profile.NavigationState.SelectedMenuId + "&_v=" + Profile.NavigationState.SelectedContentNameId +
						"&" + Profile.NavigationState.QueryString;
                 * */
			}
			else
				qs = Request.QueryString.ToString();

			sb.AppendFormat("?{0}", qs);

			// return page url with queryString
			defaultLink = sb.ToString();
		}
		#endregion

        /// <summary>
        /// Registers the scripts.
        /// </summary>
		private void RegisterScripts()
		{
			//scripts
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtBase", ResolveClientUrl("~/Scripts/ext/ext-base.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtAll", ResolveClientUrl("~/Scripts/ext/ext-all.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "MainLayout", ResolveClientUrl("~/Scripts/Shell/mainLayout.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "MainHistory", ResolveClientUrl("~/Scripts/Shell/mainHistory.js"));

			Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "Sitemap", String.Format("CSManagementClient.MenuCount = {0};", 
				ManagementContext.Current.Configs.Length), true);
			Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "InitializeClient",
				String.Format("CSManagementClient.Views = {0};  CSManagementClient.CurrentView = {1};", GetRegisteredViewsArray(), _DefaultViewArray), true);
		}

        /// <summary>
        /// Gets the registered views array.
        /// </summary>
        /// <returns></returns>
		private string GetRegisteredViewsArray()
		{
			StringBuilder pageViewDataString = new StringBuilder();
			pageViewDataString.Append("[");
			bool isFirstNode = true;
			foreach (ModuleConfig module in ManagementContext.Current.Configs)
			{
				foreach (AdminView view in module.Views)
				{
					string[] viewDataArray = new String[5];
					viewDataArray[0] = String.Format("'{0}'", module.Name);
					viewDataArray[1] = String.Format("'{0}'", view.ViewId);
					viewDataArray[2] = String.Format("'{0}'", view.GetLocalizedName());
					viewDataArray[3] = view.IsNameDynamic.ToString().ToLowerInvariant();

					// Bind transitions
					StringBuilder tranDataString = new StringBuilder();
					bool isFirstTranNode = true;
					foreach (ViewTransition tran in view.Transitions)
					{
						string[] tranDataArray = new String[2];
						tranDataArray[0] = String.Format("'{0}'", tran.Name);
						tranDataArray[1] = String.Format("'{0}'", tran.ViewId);

						if (!isFirstTranNode)
							tranDataString.Append(",");
						else
							isFirstTranNode = false;

						tranDataString.Append("[");
						tranDataString.Append(String.Join(",", tranDataArray));
						tranDataString.Append("]");
					}

					if (tranDataString.Length > 0)
						viewDataArray[4] = String.Format("[{0}]", tranDataString);

					if (!isFirstNode)
						pageViewDataString.Append(",");
					else
						isFirstNode = false;

					pageViewDataString.Append("[");
					pageViewDataString.Append(String.Join(",", viewDataArray));
					pageViewDataString.Append("]");

					if (String.Compare(module.Name, _DefaultAddId, StringComparison.InvariantCultureIgnoreCase) == 0 &&
						String.Compare(view.ViewId, _DefaultView, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						// initialize current view array
						_DefaultViewArray = String.Format("[{0}]", String.Join(",", viewDataArray));
					}
				}
			}

			pageViewDataString.Append("]");

			return pageViewDataString.ToString();
		}
	}
}
