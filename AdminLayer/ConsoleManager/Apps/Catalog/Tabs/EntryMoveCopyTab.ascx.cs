using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntryMoveCopyTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
    {
        /// <summary>
        /// Gets the tree data source.
        /// </summary>
        /// <value>The tree data source.</value>
		protected string TreeDataSource
		{
			get
			{
				return Page.ResolveClientUrl("~/Apps/Catalog/Tree/MoveCopyDialogTreeSource.aspx");
			}
		}

        /// <summary>
        /// Gets the catalog id.
        /// </summary>
        /// <value>The catalog id.</value>
        public int CatalogId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("CatalogId");
            }
        }

        /// <summary>
        /// Gets the catalog node id.
        /// </summary>
        /// <value>The catalog node id.</value>
        public int CatalogNodeId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("CatalogNodeId");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			btnOK.Click += new EventHandler(btnOK_Click);

			ManagementHelper.RegisterExtJsStyles(this.Page);

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
			CommandParameters cp = new CommandParameters("MoveCopyDialogCommand");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic["cmd"] = MoveCopyOption.SelectedValue;
			dic["folder"] = targetFolder.Value;
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

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
        {
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
        }

        #endregion
    }
}