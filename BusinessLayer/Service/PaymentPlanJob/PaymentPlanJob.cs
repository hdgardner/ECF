using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Core.Data;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Commerce.Orders;
using Quartz;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Core.Dto;

namespace Mediachase.Commerce.Services
{
    public class PaymentPlanJob : IStatefulJob
    {
        private readonly ILog _Log = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentPlanJob"/> class.
        /// </summary>
        public PaymentPlanJob()
        {
            _Log = LogManager.GetLogger(GetType());
        }

        #region IJob Members

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(JobExecutionContext context)
        {
            string applicationName = context.JobDetail.Group;

            if (String.IsNullOrEmpty(applicationName) || applicationName == "all") // index all applications
            {
                AppDto dto = AppContext.Current.GetApplicationDto();

                foreach (AppDto.ApplicationRow row in dto.Application)
                {
                    ProcessApplication(row.Name);
                }
            }
            else
            {
                ProcessApplication(applicationName);
            }  
        }
        #endregion

        /// <summary>
        /// Processes the application.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        void ProcessApplication(string applicationName)
        {
            _Log.Info(String.Format("Processing Payment Plans for \"{0}\" application.", applicationName));

            try
            {
                int totalRecords = -1;
                int startRowIndex = 0;
                OrderSearchOptions options = new OrderSearchOptions();
                OrderSearchParameters parameters = new OrderSearchParameters();
                options.Namespace = "Mediachase.Commerce.Orders";
                options.Classes.Add("PaymentPlan");
                options.RecordsToRetrieve = 50;
                parameters.OrderByClause = String.Format("OrderGroupId DESC");

                do
                {
                    options.StartingRecord = startRowIndex;
                    PaymentPlan[] orders = OrderContext.Current.FindPaymentPlans(parameters, options, out totalRecords);

                    if (orders != null)
                    {
                        foreach (PaymentPlan plan in orders)
                        {
                            // Charge the plan
                            ProcessPlan(plan);
                        }
                    }

                    startRowIndex += options.RecordsToRetrieve;
                }
                while (totalRecords > 0);

                _Log.Info("Successfully completed.");
            }
            catch (Exception ex)
            {
                _Log.Error("Processing Payment Plans failed.", ex);
            }
        }

        /// <summary>
        /// Processes the plan.
        /// </summary>
        /// <param name="plan">The plan.</param>
        private void ProcessPlan(PaymentPlan plan)
        {
            if (plan.IsDue)
            {
                // Execute workflow
                plan.RunWorkflow("CartCheckout");

                // Save as a purchase order
                PurchaseOrder po = plan.SaveAsPurchaseOrder();

                // Update last transaction date
                plan.LastTransactionDate = DateTime.UtcNow;

                // Encrease cycle count
                plan.CompletedCyclesCount++;

                // Save changes
                plan.AcceptChanges();

                // Allow for a log to debug
                _Log.Info(String.Format("Processed Payment Plan and created Purchase Order {0}", po.TrackingNumber));
            }            
        }
    }
}
