using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
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
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Content.Folders
{
	public partial class FolderMoveControl : BaseUserControl
	{
        /// <summary>
        /// Gets the site id.
        /// </summary>
        /// <value>The site id.</value>
		public Guid SiteId
		{
			get
			{
				return new Guid(Request.QueryString["SiteId"].ToString());
			}
		}

        /// <summary>
        /// Gets the folder id.
        /// </summary>
        /// <value>The folder id.</value>
		public int FolderId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("FolderId");
			}
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			btnOK.Click += new EventHandler(btnOK_Click);

			base.OnInit(e);
		}

        /// <summary>
        /// Handles the Click event of the btnOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnOK_Click(object sender, EventArgs e)
		{
			// close dialog
			CommandParameters cp = new CommandParameters("MoveFolderDialogCommand");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic["folder"] = ddlFolders.SelectedValue;
			cp.CommandArguments = dic;
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                SecurityManager.CheckRolePermission("content:site:nav:mng:edit");
                
                BindFolders();
				DataBind();
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

        /// <summary>
        /// Binds the folders.
        /// </summary>
		private void BindFolders()
		{
			//get all folders
			DataTable folders = FileTreeItem.LoadAllFoldersDT(SiteId);
			Hashtable avalibleFolders = new Hashtable();

			foreach (DataRow folder in folders.Rows)
			{
				if ((int)folder["PageId"] != FolderId)
					avalibleFolders.Add(folder["PageId"], folder["Outline"]);
			}

			//avalibleFolders.Add("", "[select folder]");
			ddlFolders.DataSource = avalibleFolders;
			ddlFolders.DataTextField = "Value";
			ddlFolders.DataValueField = "Key";
		}
	}
}