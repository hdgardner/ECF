using System;
using System.Collections;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Asset.Tabs
{
    public partial class NewFolderTab : BaseUserControl, IAdminTabControl, IAdminContextControl
    {
        #region prop: ParentId
        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>The parent id.</value>
        private int ParentId
        {
            get
            {
                if (Request["Id"] == null)
                    return -1;

                return int.Parse(Request["Id"], CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region FolderId
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int FolderId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("folderid");
            }
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //set the textbox value to display the name of the folder and hide the create button
            if (!Page.IsPostBack)
            {
                if (FolderId != 0)
                {
                    //check permissions first
                    SecurityManager.CheckRolePermission("asset:mng:edit");

                    btnCreate.Visible = false;
                    BindData();
                }
                else
                {
                    //check permissions first
                    SecurityManager.CheckRolePermission("asset:mng:create");
                }
            }
        }

        /// <summary>
        /// Binds the data. Get the folder business object and add the folder name to the textbox
        /// </summary>
        private void BindData()
        {
            BusinessObject _bindObject = MetaObjectActivator.CreateInstance<BusinessObject>("Folder", FolderId);
            if (_bindObject != null)
            {
                FolderName.Text = _bindObject["Name"].ToString();
            }
        }

        /// <summary>
        /// Handles the ServerClick event of the btnCreate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnCreate_ServerClick(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                Folder folder = new Folder();
                folder.Name = FolderName.Text;
                folder.Save();
                Mediachase.Ibn.Data.Services.TreeManager.AppendNode(Folder.GetAssignedMetaClass(), this.ParentId, true, folder);
            }
            catch (Exception){}

            CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
            //Response.Redirect(this.Request.RawUrl);
        }

        /// <summary>
        /// Checks if entered folder name is unique.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void FolderNameCheck(object sender, ServerValidateEventArgs args)
        {
            // load node by name
            Mediachase.Ibn.Data.Services.TreeNode[] nodes = TreeManager.GetChildNodes(Folder.GetAssignedMetaClass(), ParentId);
            foreach (Mediachase.Ibn.Data.Services.TreeNode node in nodes)
            {
                if (node.Title.Equals(args.Value))
                {
                    args.IsValid = false;
                    return;
                }
            }

            args.IsValid = true;
        }
        
        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes made to the folder name
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            if (FolderId > 0)
            {
                BusinessObject _bindObject = MetaObjectActivator.CreateInstance<BusinessObject>("Folder", FolderId);
                if (_bindObject != null)
                {
                    _bindObject["Name"] = FolderName.Text;
                    //ProcessCollection(this.Page.Controls, (BusinessObject)_bindObject);
                    ((BusinessObject)_bindObject).Save();
                }
            }
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