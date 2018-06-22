using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Profile.Search;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Commerce.Manager.Apps.Dashboard.Modules
{
    public partial class SummaryModule : BaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindStatistics();
        }

        /// <summary>
        /// Binds the statistics.
        /// </summary>
        private void BindStatistics()
        {
            // calculate number of entries
            int recordsCount = 0;
            CatalogContext.Current.FindItemsDto(new CatalogSearchParameters(), new CatalogSearchOptions(), ref recordsCount);
            EntriesCount.InnerHtml = recordsCount > 0 ? recordsCount.ToString() : "0";

            // calculate number of nodes
            recordsCount = 0;
            CatalogContext.Current.FindNodesDto(new CatalogSearchParameters(), new CatalogSearchOptions(), ref recordsCount);
			NodesCount.InnerHtml = recordsCount > 0 ? recordsCount.ToString() : "0";

            // Calculate number of orders
            OrderSearchOptions orderoptions = new OrderSearchOptions();
            orderoptions.Namespace = "Mediachase.Commerce.Orders";
            orderoptions.Classes.Add("PurchaseOrder");
            OrderContext.Current.FindPurchaseOrders(new OrderSearchParameters(), orderoptions, out recordsCount);
			OrdersCount.InnerHtml = recordsCount > 0 ? recordsCount.ToString() : "0";

            // Calculate number of customers
            ProfileSearchOptions profileOptions = new ProfileSearchOptions();
            profileOptions.Namespace = "Mediachase.Commerce.Profile";
            profileOptions.Classes.Add("Account");
            ProfileContext.Current.FindAccounts(new ProfileSearchParameters(), profileOptions, out recordsCount);
			CustomerCount.InnerHtml = recordsCount > 0 ? recordsCount.ToString() : "0";

            // Calculate number of promotions
            recordsCount = PromotionManager.GetPromotionDto().Promotion.Count;
			PromotionCount.InnerHtml = recordsCount > 0 ? recordsCount.ToString() : "0";
        }
    }
}