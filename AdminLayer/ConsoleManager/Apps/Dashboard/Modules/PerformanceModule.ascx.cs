using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Microsoft.Reporting.WebForms;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Storage;
using Mediachase.Web.Console.Reporting.DataSources;

namespace Mediachase.Commerce.Manager.Apps.Dashboard.Modules
{
    public partial class PerformanceModule : BaseUserControl
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
            DataCommand cmd = OrderDataHelper.CreateTranDataCommand();
            cmd.CommandText = String.Format("select sum(F.SubTotal) GrossSales, sum(F.SubTotal - F.DiscountAmount) NetSales, G.BillingCurrency from OrderForm F inner join OrderFormEx FEX ON FEX.ObjectId = F.OrderFormId inner join OrderGroup G on G.OrderGroupId = F.OrderGroupId where G.Status = (select [Name] from OrderStatus OS where OrderStatusId = (select max(OrderStatusId) from OrderStatus) and OS.ApplicationId = '{0}') and G.ApplicationId = '{0}' and FEX.Created between '{1}' and '{2}' GROUP BY G.BillingCurrency;", OrderConfiguration.Instance.ApplicationId, new DateTime(DateTime.UtcNow.Year, 1, 1).ToString("s"), DateTime.UtcNow.ToString("s"));
            cmd.CommandType = CommandType.Text;
            cmd.Parameters = new DataParameters();
            cmd.TableMapping = DataHelper.MapTables("SalesTable");
            cmd.DataSet = new PerfDataSet();
            DataResult results = DataService.LoadDataSet(cmd);

            cmd.CommandText = String.Format("select sum(F.SubTotal) GrossSales, sum(F.SubTotal - F.DiscountAmount) NetSales, G.BillingCurrency, month(FEX.Created) [Month] from OrderForm F inner join OrderFormEx FEX ON FEX.ObjectId = F.OrderFormId inner join OrderGroup G on G.OrderGroupId = F.OrderGroupId where G.Status = (select [Name] from OrderStatus OS where OrderStatusId = (select max(OrderStatusId) from OrderStatus) and OS.ApplicationId = '{0}') and G.ApplicationId = '{0}' and FEX.Created between '{1}' and '{2}' GROUP BY G.BillingCurrency, month(FEX.Created);", OrderConfiguration.Instance.ApplicationId, new DateTime(DateTime.UtcNow.Year, 1, 1).ToString("s"), DateTime.UtcNow.ToString("s"));
            cmd.CommandType = CommandType.Text;
            cmd.Parameters = new DataParameters();
            cmd.TableMapping = DataHelper.MapTables("SalesMonthlyTable");
            cmd.DataSet = new PerfDataSet();
            DataResult results2 = DataService.LoadDataSet(cmd);

            ReportDataSource source = new ReportDataSource();
            source.Name = "SalesTable";
            source.Value = ((PerfDataSet)results.DataSet).SalesTable;

            ReportDataSource source2 = new ReportDataSource();
            source2.Name = "SalesMonthlyTable";
            source2.Value = ((PerfDataSet)results2.DataSet).SalesMonthlyTable;
            
            PerfReportViewer.LocalReport.DataSources.Add(source);
            PerfReportViewer.LocalReport.DataSources.Add(source2);
            PerfReportViewer.DataBind();
        }
    }
}