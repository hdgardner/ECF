using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Cms;
using Mediachase.Cms.WebUtility.Controls;

public partial class Controls_Catalog_CompareView : BaseTemplateUserControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Page_Load(object sender, System.EventArgs e)
    {

    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        this.TemplateType = "compare";
        base.OnInit(e);
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CMSContext.Current.IsDesignMode)
            divCatalogNodeViewer.InnerHtml = "Products Compare Control";
    }
}