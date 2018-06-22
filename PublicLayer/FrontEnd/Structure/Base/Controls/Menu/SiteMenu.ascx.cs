using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility.Controls;
using Mediachase.Cms.Web.UI.Controls;

namespace Mediachase.Cms.Website.Structure.Base.Controls.Menu
{
    public partial class SiteMenu : BaseTemplateUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //BaseStaticWrapper wrapper = new BaseStaticWrapper();

            //08046F65-1D6C-4f5e-B409-90DD9789B2DE
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            this.TemplateType = "menu";
            CmsPlaceHolder.RegisterStaticControl(this, this, "08046F65-1D6C-4f5e-B409-90DD9789B2DE");
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
                ControlViewer.InnerHtml = "Menu Control";
        }
    }
}