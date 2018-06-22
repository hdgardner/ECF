using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using System.Web.Security;

namespace Mediachase.Cms.WebUtility.Commerce
{
    public class PromotionHelper
    {
        PromotionContext _Context = null;

        /// <summary>
        /// Gets or sets the promotion context.
        /// </summary>
        /// <value>The promotion context.</value>
        public PromotionContext PromotionContext
        {
            get { return _Context; }
            set { _Context = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionHelper"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public PromotionHelper(PromotionContext context)
        {
            InitPromotionContext();
            _Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionHelper"/> class.
        /// </summary>
        public PromotionHelper()
        {
            InitPromotionContext();
            _Context = new PromotionContext(MarketingContext.Current.MarketingProfileContext, null, null);
        }

        /// <summary>
        /// Evals the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public void Eval(PromotionFilter filter)
        {
            MarketingContext.Current.EvaluatePromotions(true, this.PromotionContext, filter);
        }

        /// <summary>
        /// Inits the promotion context.
        /// </summary>
        public void InitPromotionContext()
        {
            // We assume if there is already shopping cart context defined for the current http request, that we do not need to reinitialize it again
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(MarketingContext.ContextConstants.ShoppingCart))
            {
                SetContext(MarketingContext.ContextConstants.ShoppingCart, OrderContext.Current.GetCart(Cart.DefaultName, ProfileContext.Current.UserId));
                //SetContext(MarketingContext.ContextConstants.CustomerProfile, ProfileContext.Current.Profile);

                // Set customer segment context
                MembershipUser user = ProfileContext.Current.User;
                if (user != null)
                {
                    Account account = ProfileContext.Current.Profile.Account;
                    if (account != null)
                    {
                        SetContext(MarketingContext.ContextConstants.CustomerAccount, account);

                        Guid accountId = account.PrincipalId;
                        Guid organizationId = Guid.Empty;
                        if (account.Organization != null)
                        {
                            organizationId = account.Organization.PrincipalId;
                        }

                        SetContext(MarketingContext.ContextConstants.CustomerSegments, MarketingContext.Current.GetCustomerSegments(accountId, organizationId));
                    }
                }

                // Set customer promotion history context
                SetContext(MarketingContext.ContextConstants.CustomerId, ProfileContext.Current.UserId);
            }
        }         

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="val">The val.</param>
        private void SetContext(string key, object val)
        {
            if (!MarketingContext.Current.MarketingProfileContext.ContainsKey(key))
                MarketingContext.Current.MarketingProfileContext.Add(key, val);
            else
                MarketingContext.Current.MarketingProfileContext[key] = val;
        }
    }
}
