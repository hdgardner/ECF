using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Marketing.Dto;
using System.Data;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// Promotion Item is responsible for the item in the marketing system. Results are cached.
    /// </summary>
    public class PromotionItem
    {
        private MarketingHelper _Helper;
        private int _RowIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItem"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="rowIndex">Index of the row.</param>
        internal PromotionItem(MarketingHelper helper, int rowIndex)
        {
            _Helper = helper;
            _RowIndex = rowIndex;
        }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        /// <value>The index of the row.</value>
        internal int RowIndex
        {
            get
            {
                return this._RowIndex;
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return _Helper.Promotions.Promotion.Columns.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value></value>
        public object this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this._Helper.Promotions.Promotion[RowIndex][index];
            }
        }

        /// <summary>
        /// Gets the data row.
        /// </summary>
        /// <value>The data row.</value>
        public PromotionDto.PromotionRow DataRow
        {
            get
            {
                return this._Helper.Promotions.Promotion[RowIndex];
            }
        }

        /// <summary>
        /// Gets the campaign.
        /// </summary>
        /// <value>The campaign.</value>
        public CampaignDto.CampaignRow Campaign
        {
            get
            {
                return _Helper.Campaigns.Campaign.FindByCampaignId(DataRow.CampaignId);
            }
        }

        /// <summary>
        /// Returns total number of times current promotion has been used.
        /// </summary>
        /// <value>The total used count.</value>
        public int TotalUsedCount
        {
            get
            {
                System.Data.DataRow[] rows = _Helper.Usage.Select(String.Format("PromotionId = {0}", DataRow.PromotionId));
                if(rows != null && rows.Length > 0)
                    return (int)rows[0]["TotalUsed"];

                return 0;
            }
        }

        /// <summary>
        /// Returns total number of times current promotion has been used by current customer. The results are cached.
        /// </summary>
        /// <value>The customer used count.</value>
        public int GetCustomerUsageCount(Guid customerId)
        {
            DataTable usage = null;

            // Load customer specific 
            string cacheKey = MarketingCache.CreateCacheKey("MarketingHelper-customer", customerId.ToString());
            object cachedObject = MarketingCache.Get(cacheKey);

            if (cachedObject != null)
                usage = (DataTable)cachedObject;
            else
            {
                usage = PromotionManager.GetPromotionUsageStatistics(customerId);
                MarketingCache.Insert(cacheKey, usage, MarketingConfiguration.Instance.CacheConfig.PromotionCollectionTimeout);
            }

            System.Data.DataRow[] rows = usage.Select(String.Format("PromotionId = {0}", DataRow.PromotionId));
            if (rows != null && rows.Length > 0)
                return (int)rows[0]["TotalUsed"];

            return 0;
        }

        List<ExpressionDto.ExpressionRow> _Expressions;
        /// <summary>
        /// Gets the expressions.
        /// </summary>
        /// <value>The expressions.</value>
        public List<ExpressionDto.ExpressionRow> Expressions
        {
            get
            {
                if (_Expressions != null)
                    return _Expressions;

                List<ExpressionDto.ExpressionRow> expressions = new List<ExpressionDto.ExpressionRow>();
                PromotionDto.PromotionConditionRow[] conditions = this._Helper.Promotions.Promotion[RowIndex].GetPromotionConditionRows();
                if(conditions != null)
                {
                    foreach (PromotionDto.PromotionConditionRow condition in conditions)
                    {
                        expressions.Add(this._Helper.Expressions.Expression.FindByExpressionId(condition.ExpressionId));
                    }
                }

                _Expressions = expressions;

                return _Expressions;
            }
        }

        List<ExpressionDto.ExpressionRow> _PolicyExpressions;
        /// <summary>
        /// Gets the policy expressions.
        /// </summary>
        /// <value>The policy expressions.</value>
        public List<ExpressionDto.ExpressionRow> PolicyExpressions
        {
            get
            {
                if (_PolicyExpressions != null)
                    return _PolicyExpressions;

                List<ExpressionDto.ExpressionRow> expressions = new List<ExpressionDto.ExpressionRow>();
                List<PolicyDto.PolicyRow> policies = this.Policies;
                if (policies != null && policies.Count > 0)
                {
                    foreach (PolicyDto.PolicyRow policy in policies)
                    {
                        expressions.Add(this._Helper.Expressions.Expression.FindByExpressionId(policy.ExpressionId));
                    }
                }

                _PolicyExpressions = expressions;

                return _PolicyExpressions;
            }
        }

        List<PolicyDto.PolicyRow> _Policies;
        /// <summary>
        /// Gets the policies.
        /// </summary>
        /// <value>The policies.</value>
        public List<PolicyDto.PolicyRow> Policies
        {
            get
            {
                if (_Policies != null)
                    return _Policies;

                List<PolicyDto.PolicyRow> policies = new List<PolicyDto.PolicyRow>();

                string promotionGroup = this.DataRow.PromotionGroup;
                int promotionId = this.DataRow.PromotionId;

                List<int> policyArray = new List<int>();

                // Get policies that apply to promotion
                PromotionDto.PromotionPolicyRow[] promotionPolicyRows = this.DataRow.GetPromotionPolicyRows();
                if(promotionPolicyRows != null && promotionPolicyRows.Length > 0)
                {
                    foreach(PromotionDto.PromotionPolicyRow row in promotionPolicyRows)
                    {
                        if(!policyArray.Contains(row.PolicyId))
                            policyArray.Add(row.PolicyId);
                    }
                }

                // Get Policies that apply to promotion group
                PolicyDto.GroupPolicyRow[] groupPolicyRows = (PolicyDto.GroupPolicyRow[])this._Helper.Policies.GroupPolicy.Select(String.Format("GroupName = '{0}'", promotionGroup));
                if(groupPolicyRows != null && groupPolicyRows.Length > 0)
                {
                    foreach(PolicyDto.GroupPolicyRow row in groupPolicyRows)
                    {
                        if(!policyArray.Contains(row.PolicyId))
                            policyArray.Add(row.PolicyId);
                    }
                }

                // Get Policies that apply globally
                PolicyDto.PolicyRow[] globalPolicyRows = (PolicyDto.PolicyRow[])this._Helper.Policies.Policy.Select(String.Format("IsLocal = 'True'"));
                if(globalPolicyRows != null && globalPolicyRows.Length > 0)
                {
                    foreach(PolicyDto.PolicyRow row in globalPolicyRows)
                    {
                        if(!policyArray.Contains(row.PolicyId))
                            policyArray.Add(row.PolicyId);
                    }
                }

                if (policyArray != null && policyArray.Count > 0)
                {
                    foreach (int id in policyArray)
                    {
                        policies.Add(this._Helper.Policies.Policy.FindByPolicyId(id));
                    }
                }

                _Policies = policies;

                return _Policies;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value></value>
        public object this[string name]
        {
            get
            {
                return _Helper.Promotions.Promotion[RowIndex][name];
            }
        } 
    }
}
