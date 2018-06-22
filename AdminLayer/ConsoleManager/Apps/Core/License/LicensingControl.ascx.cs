using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Licensing;
using Mediachase.FileUploader.Web;
using System.Text;
using System.IO;

namespace Mediachase.Commerce.Manager.Core
{
    public partial class LicensingControl : BaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControls();
            }
        }

        /// <summary>
        /// Binds the controls.
        /// </summary>
        private void BindControls()
        {
            lblHostID.Text = String.Format("Host ID: {0}", CommerceLicensing.GetHostUid());
            CommerceLicenseInfo[] licenseInfo = CommerceLicensing.GetLicenseInfo(); // do not use cache here
            dlLicenseList.DataSource = licenseInfo;
            dlLicenseList.DataBind();
        }

        /// <summary>
        /// Handles the ServerClick event of the btnUpload control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnUpload_ServerClick(object sender, EventArgs e)
        {
            Encoding encoding = Encoding.UTF8;

            McHttpPostedFile file = mcHtmlInputFile.PostedFile;
            if (file != null)
            {
                byte[] data = new Byte[file.ContentLength];
                file.InputStream.Read(data, 0, file.ContentLength);
                CommerceLicensing.SetLicense(encoding.GetString(data));
                BindControls();
            }
        }
        /// <summary>
        /// Handles the Click event of the DeleteLicense control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void DeleteLicense_Click(object sender, ImageClickEventArgs e)
        {
            CommerceLicensing.RemoveLicense(((ImageButton)sender).CommandArgument);
            BindControls();
        }

        /// <summary>
        /// Handles the Click event of the btnDownloadLicense control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnDownloadLicense_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbLicenseKey.Text))
            {
                string errorMessage = String.Empty;
                
                CommerceLicensing.InstallLicense(tbLicenseKey.Text, out errorMessage);
                
                if(!String.IsNullOrEmpty(errorMessage))
                    this.DisplayErrorMessage(errorMessage);

                tbLicenseKey.Text = String.Empty;
            }
            BindControls();
        }
}
}