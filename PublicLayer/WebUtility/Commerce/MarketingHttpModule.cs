using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Orders;
using System.Web.Security;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.WebUtility.Commerce
{
    public class MarketingHttpModule : IHttpModule
    {
        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute +=new EventHandler(context_PreRequestHandlerExecute);
        }

        /// <summary>
        /// Handles the PreRequestHandlerExecute event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            /*
            //MembershipUser user = ProfileContext.Current.User;
            if (MarketingContext.Current.MarketingProfileContext.ContainsKey(MarketingContext.ContextConstants.ShoppingCart))
                MarketingContext.Current.MarketingProfileContext.Add(MarketingContext.ContextConstants.ShoppingCart, OrderContext.Current.GetCart(Cart.DefaultName, ProfileContext.Current.UserId));
            else
                MarketingContext.Current.MarketingProfileContext[MarketingContext.ContextConstants.ShoppingCart] = OrderContext.Current.GetCart(Cart.DefaultName, ProfileContext.Current.UserId);
             * */
        }
        #endregion
    }
}
