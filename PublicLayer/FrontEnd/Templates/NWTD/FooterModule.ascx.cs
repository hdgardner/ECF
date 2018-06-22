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
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Shared;

namespace Mediachase.Cms.Website.Templates.NWTD
{
    /// <summary>
    /// Footer module
    /// </summary>
    public partial class FooterModule : BaseStoreUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            DataBind();
            Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CompareProducts_js", CommerceHelper.GetAbsolutePath("/Scripts/CompareProducts.js"));
            Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CompareViewPageUrl", String.Format("CSCompareProducts.CompareViewPageUrl = \"{0}\";", CMSContext.Current.ResolveUrl("~/compare.aspx")), true);
        }
    }
}
