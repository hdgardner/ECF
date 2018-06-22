using System;
using System.Collections;
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
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console;

namespace Mediachase.Commerce.Manager.Core.Controls
{
	public partial class PopupPage : BasePage
	{
		/// <summary>
		/// Gets the app id.
		/// </summary>
		/// <value>The app id.</value>
		public string AppId
		{
			get
			{
				return ManagementHelper.GetAppIdFromQueryString();
			}
		}

		/// <summary>
		/// Gets the view id.
		/// </summary>
		/// <value>The view id.</value>
		public string ViewId
		{
			get
			{
				return ManagementHelper.GetViewIdFromQueryString();
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event to initialize the page.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
			// Proceed with the rest
			base.OnInit(e);

			LoadChildControls();
		}

		/// <summary>
		/// Loads the child controls.
		/// </summary>
		private void LoadChildControls()
		{
			AdminView view = ManagementContext.Current.FindView(AppId, ViewId);
			if (view != null)
			{
				try
				{
					string controlUrl = String.Format("~/Apps/{0}", view.ControlUrl);
					if (File.Exists(Server.MapPath(controlUrl)))
					{
						Control ctrl = this.LoadControl(controlUrl);
						phMain.Controls.Add(ctrl);
						HeaderText.Text = view.GetLocalizedName();
					}
					else
					{
						DisplayError(String.Format("File {0} does no exist.", Server.MapPath(controlUrl)));
					}
				}
				catch (Exception ex)
				{
					DisplayError(String.Format("Error: {0}\r\n StackTrace: {1}", ex.Message, ex.StackTrace));
				}
			}
			else
			{
				DisplayError(String.Format("View \"{0}\" in application \"{1}\" not found.", ViewId, AppId));
			}
		}

		private void DisplayError(string message)
		{
			ErrorText.Visible = true;
			ErrorText.Text = message;
		}
	}
}
