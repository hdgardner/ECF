using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Orders;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using Mediachase.Web.Console.Reporting.DataSources;
using Microsoft.Reporting.WebForms;
using System.Data;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Apps.Reporting
{
    public partial class CustomerOrderTotalsReport : System.Web.UI.UserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                StartDate.Value = DateTime.Now.AddMonths(-1);
                EndDate.Value = DateTime.Now;
                BindReport();
            }
        }

        /// <summary>
        /// Binds the report.
        /// </summary>
        private void BindReport()
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = "[ecf_reporting_CustomerOrderTotals]";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("interval", GroupBy.SelectedValue, DataParameterType.VarChar));
            cmd.Parameters.Add(new DataParameter("startdate", StartDate.Value.ToUniversalTime(), DataParameterType.DateTime));
            cmd.Parameters.Add(new DataParameter("enddate", EndDate.Value.ToUniversalTime(), DataParameterType.DateTime));
            cmd.TableMapping = DataHelper.MapTables("CustomerOrderTotalsReport");
            cmd.DataSet = new CustomersDataSet();
            DataResult results = DataService.LoadDataSet(cmd);

            ReportDataSource source = new ReportDataSource();
            source.Name = "CustomerOrderTotalsReport";
            source.Value = ((CustomersDataSet)results.DataSet).CustomerOrderTotalsReport;

            MyReportViewer.LocalReport.DataSources.Clear();
            MyReportViewer.LocalReport.DataSources.Add(source);
            MyReportViewer.DataBind();
        }

        /// <summary>
        /// Handles the BookmarkNavigation event of the MyReportViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Reporting.WebForms.BookmarkNavigationEventArgs"/> instance containing the event data.</param>
        protected void MyReportViewer_BookmarkNavigation(object sender, BookmarkNavigationEventArgs e)
        {
            CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntryDto(e.BookmarkId);

            if (dto.CatalogEntry.Count > 0)
            {
                e.Cancel = true;
                Response.Redirect(ManagementHelper.GetEntryUrl(dto.CatalogEntry[0].CatalogEntryId, dto.CatalogEntry[0].ClassTypeId));
            }
            
            e.Cancel = true;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindReport();
        }
    }
}