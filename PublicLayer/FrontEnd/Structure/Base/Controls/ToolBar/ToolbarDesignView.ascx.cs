using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.Managers;
using Mediachase.Cms;
using Mediachase.Cms.Pages;

namespace Mediachase.Cms.Controls
{
    public partial class ToolbarDesignView : System.Web.UI.UserControl
    {
        #region property: VersionId
        /// <summary>
        /// Gets the version id.
        /// </summary>
        /// <value>The version id.</value>
        public int VersionId
        {
            get { return CMSContext.Current.VersionId; }
        }
        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Visible) return;

            string val = ((HtmlInputHidden)this.Page.FindControl("ToolBoxVisible")).Value;
            if (!String.IsNullOrEmpty(val))
            {
                TopToolbar.Items[0].Checked = Boolean.Parse(val.ToString());
            }
            else
            {
                HttpCookie cookie = Request.Cookies["ToolBoxVisibleCookie"];
                if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
                {
                    TopToolbar.Items[0].Checked = Boolean.Parse(cookie.Value);
                }
            }
        }
   }
}