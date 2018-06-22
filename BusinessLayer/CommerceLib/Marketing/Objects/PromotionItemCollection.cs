using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Marketing.Dto;
using System.Collections;
using System.Data;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Commerce.Marketing.Objects
{
    /// <summary>
    /// Implements operations for and represents the promotion item collection.
    /// </summary>
    public class PromotionItemCollection : ICollection<PromotionItem>
    {
        private MarketingHelper _Helper;
        internal readonly int[] _ValidRowIndexes;
        private PromotionItem[] _ValidPromotionItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItemCollection"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        internal PromotionItemCollection(MarketingHelper helper)
        {
            _Helper = helper;

            this._ValidRowIndexes = new int[helper.Promotions.Promotion.Count];
            for (int i = 0; i < this._Helper.Promotions.Promotion.Count; i++)
            {
                this._ValidRowIndexes[i] = i;
            }
            this._ValidPromotionItems = new PromotionItem[this._ValidRowIndexes.Length];

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItemCollection"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="validRowIndexes">The valid row indexes.</param>
        internal PromotionItemCollection(PromotionItemCollection source, int[] validRowIndexes)
        {
            this._Helper = source._Helper;
            this._ValidRowIndexes = validRowIndexes;
            this._ValidPromotionItems = new PromotionItem[this._ValidRowIndexes.Length];
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Marketing.Objects.PromotionItem"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PromotionItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.Count))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (_ValidPromotionItems[index] == null)
                {
                    _ValidPromotionItems[index] = new PromotionItem(this._Helper, this._ValidRowIndexes[index]);
                }
                return this._ValidPromotionItems[index];
            }
        }

        #region ICollection<PromotionItem> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        void ICollection<PromotionItem>.Add(PromotionItem item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(PromotionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            //if (this._Helper == item..ClWrapper)
            {
                for (int i = 0; i < this._ValidRowIndexes.Length; i++)
                {
                    if (this._ValidRowIndexes[i] == item.RowIndex)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="T:System.ArgumentNullException">array is null.</exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        public void CopyTo(PromotionItem[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get
            {
                return this._ValidRowIndexes.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove(PromotionItem item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<PromotionItem> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<PromotionItem> GetEnumerator()
        {
            return new PromotionItemCollectionEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new PromotionItemCollectionEnumerator(this);
        }

        #endregion
    }

    /// <summary>
    /// Implements operations for and represents the enumerator for the promotion item collection.
    /// </summary>
    public sealed class PromotionItemCollectionEnumerator : IEnumerator<PromotionItem>, IDisposable, IEnumerator
    {
        // Fields
        private PromotionItemCollection collection;
        private int index;

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionItemCollectionEnumerator"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        internal PromotionItemCollectionEnumerator(PromotionItemCollection collection)
        {
            this.index = -1;
            this.collection = collection;
            ((IEnumerator)this).Reset();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        bool IEnumerator.MoveNext()
        {
            this.index++;
            if (this.index >= this.collection.Count)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        void IEnumerator.Reset()
        {
            this.index = -1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }

        // Properties
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        public PromotionItem Current { get { return this.Current; } }
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        PromotionItem IEnumerator<PromotionItem>.Current
        {
            get
            {
                if ((this.index < 0) || (this.index >= this.collection.Count))
                {
                    throw new InvalidOperationException("EnumeratorPositionBad");
                }
                return this.collection[this.index];
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>The element in the collection at the current position of the enumerator.</returns>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
    }

    /// <summary>
    /// Internal Marketing Helper class that holds the cache of all marketing data needed during runtime.
    /// </summary>
    internal class MarketingHelper
    {
        private CampaignDto _Campaigns;

        /// <summary>
        /// Gets or sets the campaigns.
        /// </summary>
        /// <value>The campaigns.</value>
        public CampaignDto Campaigns
        {
            get { return _Campaigns; }
            set { _Campaigns = value; }
        }
        private ExpressionDto _Expressions;

        /// <summary>
        /// Gets or sets the expressions.
        /// </summary>
        /// <value>The expressions.</value>
        public ExpressionDto Expressions
        {
            get { return _Expressions; }
            set { _Expressions = value; }
        }
        private PolicyDto _Policies;

        /// <summary>
        /// Gets or sets the policies.
        /// </summary>
        /// <value>The policies.</value>
        public PolicyDto Policies
        {
            get { return _Policies; }
            set { _Policies = value; }
        }
        private PromotionDto _Promotions;

        /// <summary>
        /// Gets or sets the promotions.
        /// </summary>
        /// <value>The promotions.</value>
        public PromotionDto Promotions
        {
            get { return _Promotions; }
            set { _Promotions = value; }
        }
        private SegmentDto _Segments;

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <value>The segments.</value>
        public SegmentDto Segments
        {
            get { return _Segments; }
            set { _Segments = value; }
        }

        DataTable _Usage = null;

        /// <summary>
        /// Gets or sets the total usage. The property implement lazy loading and only loaded when information is requested. 
        /// </summary>
        /// <remarks>
        /// The property is cached as part of the overall MarketingHelper caching, so no additional caching is needed.
        /// </remarks>
        /// <value>The usage.</value>
        public DataTable Usage
        {
            get
            {
                if (_Usage == null)
                {
                    // Load usage statistics
                    _Usage = PromotionManager.GetPromotionUsageStatistics();               
                }

                return _Usage;
            }
            set
            {
                _Usage = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingHelper"/> class.
        /// </summary>
        /// <param name="campaigns">The campaigns.</param>
        /// <param name="expressions">The expressions.</param>
        /// <param name="policies">The policies.</param>
        /// <param name="promotions">The promotions.</param>
        /// <param name="segments">The segments.</param>
        public MarketingHelper(CampaignDto campaigns, ExpressionDto expressions, PolicyDto policies, PromotionDto promotions, SegmentDto segments)
        {
            _Campaigns = campaigns;
            _Expressions = expressions;
            _Policies = policies;
            _Promotions = promotions;
            _Segments = segments;
        }
    }
}
