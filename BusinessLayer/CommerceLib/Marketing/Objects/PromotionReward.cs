using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// Represents the promotion reward types.
    /// </summary>
    public class PromotionRewardType
    {
        /// <summary>
        /// Public string literal for the reward type for the whole order.
        /// </summary>
        public const string WholeOrder = "WholeOrder";
        /// <summary>
        /// Public string literal for the reward type for all affected entries.
        /// </summary>
        public const string AllAffectedEntries = "AllAffectedEntries";
        /// <summary>
        /// Public string literal for the reward type for each affected entry.
        /// </summary>
        public const string EachAffectedEntry = "EachAffectedEntry";
    }

    /// <summary>
    /// Represents the promotion reward amount type.
    /// </summary>
    public class PromotionRewardAmountType
    {
        /// <summary>
        /// Public string literal for the percentage.
        /// </summary>
        public const string Percentage = "Percentage";
        /// <summary>
        /// Public string literal for the value.
        /// </summary>
        public const string Value = "Value";
    }

    /// <summary>
    /// Implements operations for the promotion reward.
    /// </summary>
    public class PromotionReward
    {
        private string _RewardType = String.Empty;

        /// <summary>
        /// Gets or sets the type of the Reward. The types can be:
        /// - WholeOrder
        /// will adjust the total for the whole order
        /// - AllAffectedEntries
        /// will split adjustment between all the entries, so if discount was $50 and there are 5 entries, each one will
        /// be discount $10
        /// - EachAffectedEntry
        /// will apply full discount to each entry, so if discount was $50 and there are 5 entries the total discount will be 
        /// $250
        /// </summary>
        /// <value>The type of the Reward.</value>
        public string RewardType
        {
            get { return _RewardType; }
            set { _RewardType = value; }
        }
        private decimal _AmountOff = 0;

        /// <summary>
        /// Gets or sets the amount off.
        /// </summary>
        /// <value>The amount off.</value>
        public decimal AmountOff
        {
            get { return _AmountOff; }
            set { _AmountOff = value; }
        }

        private string _AmountType = String.Empty;

        /// <summary>
        /// Gets or sets the type of the amount. Can be either Percentage or Value.
        /// </summary>
        /// <value>The type of the amount.</value>
        public string AmountType
        {
            get { return _AmountType; }
            set { _AmountType = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionReward"/> class.
        /// </summary>
        /// <param name="rewardType">Type of the reward.</param>
        /// <param name="amountOff">The amount off.</param>
        /// <param name="amountType">Type of the amount.</param>
        public PromotionReward(string rewardType, decimal amountOff, string amountType)
        {
            _RewardType = rewardType;
            _AmountOff = amountOff;
            _AmountType = amountType;
        }
    }
}
