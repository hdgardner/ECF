using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Cms;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core.Common
{
	public partial class LanguageSelectControl : CoreBaseUserControl
	{
		// list of available cultures (need to have resources translates into specified languages)
		private string[] _cultures = new string[] { "en-US", "ru-RU" };

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindForm();
		}

		private void BindForm()
		{
			CultureInfo ci = null;
			foreach (string cultureName in _cultures)
			{
				try
				{
					ci = new CultureInfo(cultureName);
					ListItem item = new ListItem(ci.NativeName, ci.Name);
					LanguagesList.Items.Add(item);
				}
				catch { }
			}

			ManagementHelper.SelectListItem(LanguagesList, ManagementContext.Current.ConsoleUICulture.Name);

			//foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
			//{
			//    ListItem item = new ListItem(ci.NativeName, ci.Name);
			//    LanguagesList.Items.Add(item);
			//}

			//DataTable languages = Language.GetAllLanguagesDT();
			//foreach (DataRow row in languages.Rows)
			//{
			//    ListItem item = new ListItem(row["FriendlyName"].ToString(), row["LangName"].ToString());
			//    LanguagesList.Items.Add(item);
			//}
		}

		/// <summary>
		/// Handles the ServerClick event of the btnCreate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnOK_Click(object sender, EventArgs e)
		{
			bool error = false;
			try
			{
				ProcessCommand();
			}
			catch (Exception ex)
			{
				string errorMessage = ex.Message.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), "__LanguageSelectFrameError",
					String.Format("alert('{0}{1}');", "Operation failed.\\nError: ", errorMessage), true);
				error = true;
			}

			// if operation succeeded, close dialog
			if (!error)
			{
				CommandHandler.RegisterCloseOpenedFrameScript(this.Page, "ECF_ChangeLanguage_Redirect");
			}
		}

		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Request["closeFramePopup"] != null)
			{
				btnClose.OnClientClick = String.Format("javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
			}
		}

		private void ProcessCommand()
		{
			ManagementContext.Current.ConsoleUICulture = new CultureInfo(LanguagesList.SelectedValue);
		}
	}
}