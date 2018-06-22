using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// Contains results of promotion engine execution
    /// </summary>
    public class PromotionResult
    {
        List<PromotionItemRecord> records = new List<PromotionItemRecord>();

        /// <summary>
        /// Gets or sets the promotion records.
        /// </summary>
        /// <value>The promotion records.</value>
        public List<PromotionItemRecord> PromotionRecords
        {
            get { return records; }
            set { records = value; }
        }

        decimal _RunningTotal = 0;
        /// <summary>
        /// Gets or sets the running total.
        /// </summary>
        /// <value>The running total.</value>
        public decimal RunningTotal
        {
            get
            {
                return _RunningTotal;
            }
            set
            {
                _RunningTotal = value;
            }
        }

         /// <summary>
        /// Gets the discount amount.
        /// </summary>
        /// <param name="totalAmount">The total amount.</param>
        /// <returns></returns>
        public decimal GetDiscountAmount(decimal totalAmount)
        {
            decimal discountAmount = 0;
            foreach (PromotionItemRecord record in PromotionRecords)
            {
                discountAmount += record.GetDiscountAmount(totalAmount);
            }

            return discountAmount;
        }

        /// <summary>
        /// Determines whether [contains] [the specified promotion id].
        /// </summary>
        /// <param name="promotionId">The promotion id.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified promotion id]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int promotionId)
        {
            foreach (PromotionItemRecord record in PromotionRecords)
            {
                if (record.Status == PromotionItemRecordStatus.Commited && record.PromotionItem.DataRow.PromotionId == promotionId)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the count for a particular promotion.
        /// </summary>
        /// <param name="promotionId">The promotion id.</param>
        /// <returns></returns>
        public int GetCount(int promotionId)
        {
            int count = 0;
            foreach (PromotionItemRecord record in PromotionRecords)
            {
                if (record.Status == PromotionItemRecordStatus.Commited && record.PromotionItem.DataRow.PromotionId == promotionId)
                    count++;
            }

            return count;
        }
    }
}
