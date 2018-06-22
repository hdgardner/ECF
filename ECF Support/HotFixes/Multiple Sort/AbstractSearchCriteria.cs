using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Lucene.Net.Search;
using Lucene.Net.Index;

namespace Mediachase.Search.Extensions
{
    public abstract class AbstractSearchCriteria : ISearchCriteria
    {
        #region ISearchCriteria Members
        private bool _IsModified = true;

        /// <summary>
        /// Gets a value indicating whether this instance is modified.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is modified; otherwise, <c>false</c>.
        /// </value>
        protected bool IsModified
        {
            get { return _IsModified; }
            set { _IsModified = value; }
        }

        List<SearchFilter> _Filters = new List<SearchFilter>();
        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public virtual SearchFilter[] Filters
        {
            get { return _Filters.ToArray(); }
        }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public virtual void Add(SearchFilter filter)
        {
            _Filters.Add(filter);
        }

        SearchSort _Sort = null;
        public virtual SearchSort Sort
        {
            get { return _Sort; }
            set { _Sort = value; }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public abstract string Scope
        { 
            get;
        }

        private string _Locale = String.Empty;
        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        public virtual string Locale
        {
            get
            {
                if (String.IsNullOrEmpty(_Locale))
                    _Locale = Thread.CurrentThread.CurrentCulture.Name;

                return _Locale;
            }
            set
            {
                _Locale = value;
            }
        }

        string _KeyField = String.Empty;
        /// <summary>
        /// Gets the key field.
        /// </summary>
        /// <value>The key field.</value>
        public string KeyField
        {
            get
            {
                if (String.IsNullOrEmpty(_KeyField))
                    return "_id";

                return _KeyField;
            }
        }

        Dictionary<string, ISearchFilterValue> _ActiveFilters = new Dictionary<string, ISearchFilterValue>();

        /// <summary>
        /// Gets the active filter values.
        /// </summary>
        /// <value>The active filter values.</value>
        public virtual ISearchFilterValue[] ActiveFilterValues
        {
            get { return _ActiveFilters.Values.ToArray<ISearchFilterValue>(); }
        }

        /// <summary>
        /// Gets the active filter fields.
        /// </summary>
        /// <value>The active filter fields.</value>
        public virtual string[] ActiveFilterFields
        {
            get { return _ActiveFilters.Keys.ToArray<string>(); }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public virtual string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                key.Append("loc:" + Locale);

                if (Sort != null)
                    key.Append("sort:" + Sort.ToString());

                key.Append("scope:" + this.Scope);
                key.Append("keyfield:" + this.KeyField);

                // Add active fields
                foreach (string field in ActiveFilterFields)
                {
                    key.Append("af:" + field + "|" + _ActiveFilters[field].key);
                }

                return key.ToString();
            }
        }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value">The value.</param>
        public void Add(string field, ISearchFilterValue value)
        {
            _ActiveFilters.Add(field, value);
        }

        Query _Query = null;
        /// <summary>
        /// Gets the build query.
        /// </summary>
        /// <value>The build query.</value>
        public virtual Query Query
        {
            get
            {
                if (_Query != null && !IsModified)
                    return _Query;

                TermQuery languageQuery = new TermQuery(new Term("_lang", Locale.ToLower()));
                BooleanQuery query = new BooleanQuery();
                query.Add(languageQuery, BooleanClause.Occur.MUST);

                if (_ActiveFilters.Count != 0)
                {
                    foreach (string field in this.ActiveFilterFields)
                    {
                        ISearchFilterValue value = _ActiveFilters[field];

                        Query filterQuery = SearchCommon.CreateQuery(field, value);
                        if (filterQuery != null)
                            query.Add(filterQuery, BooleanClause.Occur.MUST);
                    }
                }

                _Query = query;

                IsModified = false;
                return _Query;
            }
        }
        #endregion

        /// <summary>
        /// Changes the state.
        /// </summary>
        protected void ChangeState()
        {
            _IsModified = true;
        }
    }
}
