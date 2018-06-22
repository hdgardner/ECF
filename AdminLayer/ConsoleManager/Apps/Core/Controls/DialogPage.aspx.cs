using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core.Controls
{
	public partial class DialogPage : BasePage, IAdminContextControl
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

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
		public Unit Width
		{
			get
			{
				return this.Width;
			}
			set
			{
				this.Width = value;
			}
		}

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
		public Unit Height
		{
			get
			{
				return this.Height;
			}
			set
			{
				this.Height = value;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindScripts();
			if (!IsPostBack)
				EnsureChildControls();
		}

        /// <summary>
        /// Binds the scripts.
        /// </summary>
		private void BindScripts()
		{
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtBase", ResolveClientUrl("~/Scripts/ext/ext-base.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ExtAll", ResolveClientUrl("~/Scripts/ext/ext-all.js"));
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ManagementClientProxy", ResolveClientUrl("~/Scripts/ManagementClientProxy.js"));

			foreach (ModuleConfig module in ManagementContext.Current.Configs)
			{
				string url = String.Format("~/Apps/{0}/Scripts", module.Name);
				string path = Server.MapPath(url);
				if (Directory.Exists(path))
				{
					foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
					{
						this.ClientScript.RegisterClientScriptInclude(file.Name, this.ResolveClientUrl(String.Format("{0}/{1}", url, file.Name)));
					}
				}
			}
		}

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
		protected override void CreateChildControls()
		{
			if (!this.ChildControlsCreated)
			{
				CreateControls();
				base.CreateChildControls();
				this.ChildControlsCreated = true;
			}
		}

        /// <summary>
        /// Creates the controls.
        /// </summary>
		private void CreateControls()
		{
			AdminView view = ManagementContext.Current.FindView(this.AppId, this.ViewId);

			if (view == null)
				return;

			Panel panel = this.contentPanel;
			panel.Controls.Clear();

			// Load custom control
			string controlUrl = String.Format("~/Apps/{0}", view.ControlUrl);
			if (File.Exists(Server.MapPath(controlUrl)))
			{
				Control ctrl = this.LoadControl(controlUrl);
				panel.Controls.Add(ctrl);
			}
			else
			{
				throw new FileNotFoundException(Server.MapPath(controlUrl));
			}
		}

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			EnsureChildControls();

			// Call save method on all the forms loaded
			foreach (Control ctrl in contentPanel.Controls)
			{
				// Make sure control is of type that has save changes method
				if (ctrl is IAdminContextControl)
				{
					// Persist changes
					((IAdminContextControl)ctrl).LoadContext(context);
				}
			}
		}
	}
}
