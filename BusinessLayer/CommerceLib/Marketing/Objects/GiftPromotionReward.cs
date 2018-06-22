using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// Gift promotion reward. Allows promotion engine to specify discount that requires item to be added to the cart. The add operation will be performed by
    /// the custom code either in workflow activity or front end source code.
    /// </summary>
    public class GiftPromotionReward : PromotionReward
    {
        public enum Strategy
        {
            /// <summary>
            /// Add item when it does not exist in the cart already, if it does, then simply make it 100% discounted.
            /// </summary>
            AddWhenNeeded = 0,
            /// <summary>
            /// Always adds another item even if one already exists in the cart.
            /// </summary>
            AlwaysAdd = 1,
            /// <summary>
            /// Customer must explicitly add item to the cart in order for the promotion to be applied.
            /// </summary>
            NeverAdd = 2
        }

        /// <summary>
        /// Gets or sets the add strategy.
        /// </summary>
        /// <value>The add strategy.</value>
        public Strategy AddStrategy{get;set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="GiftPromotionReward"/> class.
        /// </summary>
        /// <param name="rewardType">Type of the reward.</param>
        /// <param name="amountOff">The amount off.</param>
        /// <param name="amountType">Type of the amount.</param>
        public GiftPromotionReward(string rewardType, decimal amountOff, string amountType) : base(rewardType, amountOff, amountType)
        {
            this.AddStrategy = Strategy.AddWhenNeeded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GiftPromotionReward"/> class.
        /// </summary>
        /// <param name="rewardType">Type of the reward.</param>
        /// <param name="amountOff">The amount off.</param>
        /// <param name="amountType">Type of the amount.</param>
        /// <param name="strategy">The strategy.</param>
        public GiftPromotionReward(string rewardType, decimal amountOff, string amountType, Strategy strategy)
            : base(rewardType, amountOff, amountType)
        {
            this.AddStrategy = strategy;
        }
    }
}
