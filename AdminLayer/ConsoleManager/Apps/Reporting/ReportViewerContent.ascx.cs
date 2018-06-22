using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Apps.Reporting
{
    public partial class ReportViewerContent : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindReport();
        }

        /// <summary>
        /// Binds the report.
        /// </summary>
        private void BindReport()
        {
            string reportPath = Request.QueryString["_r"];

            if (!String.IsNullOrEmpty(reportPath))
                MyReportViewer.LocalReport.ReportPath = reportPath;
        }
    }
}