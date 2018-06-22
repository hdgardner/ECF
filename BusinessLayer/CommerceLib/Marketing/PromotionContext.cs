using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Marketing.Dto;
using System.Data;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Contains content of current promotion excution runtime.
    /// </summary>
    public class PromotionContext
    {
        #region Public Properties

        /// <summary>
        /// Contains information of current list of used promotions
        /// </summary>
        PromotionUsageDto _PromotionUsageDto = null;

        PromotionItem _Promotion = null;
        /// <summary>
        /// Gets the promotion currently being evaluated.
        /// </summary>
        /// <value>The promotion.</value>
        public PromotionItem CurrentPromotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                _Promotion = value;
            }
        }

        PromotionResult _Result = new PromotionResult();

        /// <summary>
        /// Gets the promotion result which contains current results.
        /// </summary>
        /// <value>The promotion result.</value>
        public PromotionResult PromotionResult
        {
            get { return _Result; }
        }

        string _TargetGroup = String.Empty;
        /// <summary>
        /// Gets or sets the target group.
        /// </summary>
        /// <value>The target group.</value>
        public string TargetGroup
        {
            get
            {
                return _TargetGroup;
            }
            set
            {
                _TargetGroup = value;
            }
        }

        IDictionary<string, object> _Context;
        /// <summary>
        /// Gets the context holder.
        /// </summary>
        /// <value>The context.</value>
        public IDictionary<string, object> Context
        {
            get
            {
                return _Context;
            }
        }

        PromotionEntriesSet _SourceEntriesSet;

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public PromotionEntriesSet SourceEntriesSet
        {
            get { return _SourceEntriesSet; }
            set { _SourceEntriesSet = value; }

        }

        PromotionEntriesSet _TargetEntriesSet;

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public PromotionEntriesSet TargetEntriesSet
        {
            get { return _TargetEntriesSet; }
            set { _TargetEntriesSet = value; }
        }

        private List<string> _Coupons;

        /// <summary>
        /// Gets or sets the coupons.
        /// </summary>
        /// <value>The coupons.</value>
        public List<string> Coupons
        {
            get { return _Coupons; }
            set { _Coupons = value; }
        }

        private List<int> _Segments;

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <value>The segments.</value>
        public List<int> Segments
        {
            get { return _Segments; }
            set { _Segments = value; }
        }

        private List<string> _ExclusiveGroups;

        /// <summary>
        /// Keeps track of groups that have exclusive discount already
        /// </summary>
        /// <value>The exclusive groups.</value>
        public List<string> ExclusiveGroups
        {
            get { return _ExclusiveGroups; }
            set { _ExclusiveGroups = value; }
        }

        private Guid _CustomerId = Guid.Empty;

        /// <summary>
        /// Gets or sets the customer id.
        /// </summary>
        /// <value>The customer id.</value>
        public Guid CustomerId
        {
            get { return _CustomerId; }
            set { _CustomerId = value; }
        }

        /// <summary>
        /// Gets the reserved count for the current promotion.
        /// </summary>
        /// <value>The reserved count.</value>
        public int ReservedCount
        {
            get
            {
                int count = 0;

                if (_PromotionUsageDto != null && this.CurrentPromotion != null)
                {
                    DataRow[] rows = _PromotionUsageDto.PromotionUsage.Select(String.Format("PromotionId = {0}", this.CurrentPromotion.DataRow.PromotionId));

                    if (rows.Length > 0)
                        return rows.Length;
                }

                return count;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionContext"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// 
        /// <param name="sourceEntries">The source entries.</param>
        /// <param name="targetEntries">The target entries.</param>
        public PromotionContext(IDictionary<string, object> context, PromotionEntriesSet sourceEntries, PromotionEntriesSet targetEntries)
        {
            _Context = context;
            _SourceEntriesSet = sourceEntries;
            _TargetEntriesSet = targetEntries;
            _ExclusiveGroups = new List<string>();

            // Retrieve all the coupons customer entered
            if (context != null && context.ContainsKey(MarketingContext.ContextConstants.Coupons))
                Coupons = (List<string>)context[MarketingContext.ContextConstants.Coupons];
            else
                Coupons = new List<string>();

            // Retrieve customer segments, it should be initialized beforehand
            if (context != null && context.ContainsKey(MarketingContext.ContextConstants.CustomerSegments))
                Segments = (List<int>)context[MarketingContext.ContextConstants.CustomerSegments];
            else
                Segments = new List<int>();


            // Retrieve customer id, it should be initialized beforehand
            if (context != null && context.ContainsKey(MarketingContext.ContextConstants.CustomerId))
                CustomerId = (Guid)context[MarketingContext.ContextConstants.CustomerId];
            else
                CustomerId = Guid.Empty;

            // Retrieve promotion usage, it should be initialized beforehand
            if (context != null && context.ContainsKey(MarketingContext.ContextConstants.PromotionUsage))
                _PromotionUsageDto= (PromotionUsageDto)context[MarketingContext.ContextConstants.PromotionUsage];
            else
                _PromotionUsageDto = null;
        }



        /// <summary>
        /// Adds the promotion item record.
        /// </summary>
        /// <param name="record">The record.</param>
        public void AddPromotionItemRecord(PromotionItemRecord record)
        {
            PromotionResult.PromotionRecords.Add(record);
        }

        /// <summary>
        /// Commits the records.
        /// </summary>
        public void CommitRecords()
        {
            foreach (PromotionItemRecord record in this.PromotionResult.PromotionRecords)
            {
                if (record.Status == PromotionItemRecordStatus.Executing)
                {
                    record.Status = PromotionItemRecordStatus.Commited;

                    // Change the running total, so we can use that to calculate total amount
                    PromotionResult.RunningTotal -= record.GetDiscountAmount(PromotionResult.RunningTotal);
                }
            }
        }

        /// <summary>
        /// Rejects the records.
        /// </summary>
        public void RejectRecords()
        {
            foreach (PromotionItemRecord record in this.PromotionResult.PromotionRecords)
            {
                if (record.Status == PromotionItemRecordStatus.Executing)
                    record.Status = PromotionItemRecordStatus.Invalid;
            }
        }
    }
}
