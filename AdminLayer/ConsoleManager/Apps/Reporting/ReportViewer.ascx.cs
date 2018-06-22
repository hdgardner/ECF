using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Apps.Reporting
{
    public partial class ReportViewer : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadFilter();
        }

        /// <summary>
        /// Loads the filter.
        /// </summary>
        private void LoadFilter()
        {
            string filterPath = Request.QueryString["_f"];

            if (!String.IsNullOrEmpty(filterPath))
            {
                ReportFilter.Controls.Clear();
                Control ctrl = this.LoadControl(String.Format("Filters/{0}.ascx", filterPath));
                ReportFilter.Controls.Add(ctrl);
            }            
        }
    }
}