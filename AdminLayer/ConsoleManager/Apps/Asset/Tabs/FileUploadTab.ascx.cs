using System;
using System.Collections;
using System.Globalization;
using Mediachase.FileUploader.Web;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Asset.Tabs
{
    public partial class FileUploadTab : BaseUserControl, IAdminTabControl, IAdminContextControl
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

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {		
        }

        /// <summary>
        /// Handles the ServerClick event of the btnUpload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnUpload_ServerClick(object sender, EventArgs e)
        {
            if (createBlob() == -1)
                throw new Exception("Error during creating file");

            CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
        }

        #region btnUpload_Click
        /// <summary>
        /// Handles the Click event of the btnUpload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        void btnUpload_Click(object sender, EventArgs e)
        {
            if (createBlob() == -1)
                throw new Exception("Error during creating file");

            Response.Redirect(this.Request.RawUrl);
        }
        #endregion

        #region createBlob
        /// <summary>
        /// Creates the BLOB.
        /// </summary>
        /// <returns></returns>
        private int? createBlob()
        {
            try
            {
                foreach (McHttpPostedFile file in mcHtmlInputFile.PostedFiles)
                {
                    FolderElement.Create(this.ParentId, file.FileName, file.InputStream); //.Save();
                }

                return 0;

            }
            catch (Exception)
            {
                return -1;
            }
        }
        #endregion

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