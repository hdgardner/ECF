using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;
using Mediachase.Licensing;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager
{
    public partial class Licensing : BasePage
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LicenseManager.Validate(typeof(Mediachase.MetaDataPlus.MetaObject), null);
                NoLicensePanel.Visible = false;
                LicensedPanel.Visible = true;
            }
            catch (LicenseException ex)
            {
                NoLicensePanel.Visible = true;
                LicensedPanel.Visible = false;
                if (String.IsNullOrEmpty(Request.QueryString["m"]))
                    ExceptionLabel.Text = ex.Message;
            }            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.LoadComplete += new EventHandler(Licensing_LoadComplete);
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the LoadComplete event of the Licensing control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Licensing_LoadComplete(object sender, EventArgs e)
        {
            if (this.IsPostBack && ErrorModule1.Messages.Count == 0)
                Response.Redirect("~/Licensing.aspx");
        }
    }
}
