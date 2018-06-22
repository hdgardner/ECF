using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// The PromotionItemRecordStatus enumeration defines the promotion item record status.
    /// </summary>
    public enum PromotionItemRecordStatus
    {
        /// <summary>
        /// Represents the executing status.
        /// </summary>
        Executing,
        /// <summary>
        /// Represents the committed status.
        /// </summary>
        Commited,
        /// <summary>
        /// Represents the invalid status.
        /// </summary>
        Invalid
    }

    /// <summary>
    /// Contains result about particular promotion applied once.
    /// </summary>
    public class PromotionItemRecord
    {
        private PromotionItemRecordStatus _Status = PromotionItemRecordStatus.Executing;

        /// <summary>
        /// Gets or sets the status of this promotion record.
        /// </summary>
        /// <value>The status.</value>
        public PromotionItemRecordStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        private PromotionEntriesSet _TargetEntriesSet;

        /// <summary>
        /// Gets or sets the target entries set.
        /// </summary>
        /// <value>The target entries set.</value>
        public PromotionEntriesSet TargetEntriesSet
        {
            get { return _TargetEntriesSet; }
            set { _TargetEntriesSet = value; }
        }

        private PromotionEntriesSet _AffectedEntriesSet;

        /// <summary>
        /// Gets or sets the affected entries set.
        /// </summary>
        /// <value>The affected entries set.</value>
        public PromotionEntriesSet AffectedEntriesSet
        {
            get { return _AffectedEntriesSet; }
            set { _AffectedEntriesSet = value; }
        }

        private PromotionReward _Reward;

        /// <summary>
        /// Gets or sets the promotion reward.
        /// </summary>
        /// <value>The promotion reward.</value>
        public PromotionReward PromotionReward
        {
            get { return _Reward; }
            set { _Reward = value; }
        }

        private PromotionItem _PromotionItem;

        /// <summary>
        /// Gets or sets the promotion item.
        /// </summary>
        /// <value>The promotion item.</value>
        public PromotionItem PromotionItem
        {
            get { return _PromotionItem; }
            set { _PromotionItem = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItemRecord"/> class.
        /// </summary>
        /// <param name="targetEntries">The target entries.</param>
        /// <param name="affectedEntries">The affected entries.</param>
        /// <param name="reward">The reward.</param>
        public PromotionItemRecord(PromotionEntriesSet targetEntries, PromotionEntriesSet affectedEntries, PromotionReward reward)
        {
            _TargetEntriesSet = targetEntries;
            _AffectedEntriesSet = affectedEntries;
            _Reward = reward;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItemRecord"/> class.
        /// </summary>
        public PromotionItemRecord()
        {
        }

        /// <summary>
        /// Gets the discount amount.
        /// </summary>
        /// <param name="totalAmount">The total amount.</param>
        /// <returns></returns>
        public decimal GetDiscountAmount(decimal totalAmount)
        {
            decimal discountAmount = 0;
            if (PromotionReward.RewardType == PromotionRewardType.AllAffectedEntries)
            {
                if (PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = AffectedEntriesSet.TotalCost * PromotionReward.AmountOff / 100;
                }
                else // need to split discount between all items
                {
                    discountAmount = PromotionReward.AmountOff;
                }
            }
            else if (PromotionReward.RewardType == PromotionRewardType.EachAffectedEntry)
            {
                if (PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    discountAmount = AffectedEntriesSet.TotalCost * PromotionReward.AmountOff / 100;
                }
                else 
                {
                    discountAmount = AffectedEntriesSet.TotalQuantity * PromotionReward.AmountOff;
                }
            }
            else if (PromotionReward.RewardType == PromotionRewardType.WholeOrder)
            {
                decimal percentageOffTotal = 0;
                if (PromotionReward.AmountType == PromotionRewardAmountType.Percentage)
                {
                    // calculate percentage adjusted by the running amount, so it will be a little less if running amount is less than total
                    percentageOffTotal = (PromotionReward.AmountOff / 100) * (totalAmount / AffectedEntriesSet.TotalCost);
                    //percentageOffTotal = PromotionReward.AmountOff / 100;
                    discountAmount = totalAmount * PromotionReward.AmountOff / 100;
                }
                else
                {
                    percentageOffTotal = PromotionReward.AmountOff / totalAmount;
                    discountAmount = PromotionReward.AmountOff;
                }
            }

            return discountAmount;
        }
    }
}
