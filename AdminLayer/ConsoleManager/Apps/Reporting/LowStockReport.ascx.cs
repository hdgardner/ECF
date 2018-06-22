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
    public partial class LowStockReport : System.Web.UI.UserControl
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
                BindReport();
            }
        }

        /// <summary>
        /// Binds the report.
        /// </summary>
        private void BindReport()
        {
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("SELECT E.[Name], I.* from [Inventory] I INNER JOIN [CatalogEntry] E ON E.Code = I.SkuId INNER JOIN Catalog C ON C.CatalogId = E.CatalogId WHERE (I.InstockQuantity - I.ReservedQuantity) < 20 AND I.InventoryStatus <> 0 AND C.ApplicationId = '{0}';", OrderConfiguration.Instance.ApplicationId);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters = new DataParameters();
            cmd.TableMapping = DataHelper.MapTables("EntryStock");
            cmd.DataSet = new ProductDataSet();
            DataResult results = DataService.LoadDataSet(cmd);

            ReportDataSource source = new ReportDataSource();
            source.Name = "EntryStock";
            source.Value = ((ProductDataSet)results.DataSet).EntryStock;

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
    }
}