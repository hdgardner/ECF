using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;

namespace Mediachase.Search
{
    public interface ISearchCriteria
    {
        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        SearchFilter[] Filters { get; }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        void Add(SearchFilter filter);

        /// <summary>
        /// Gets the active filter values.
        /// </summary>
        /// <value>The active filter values.</value>
        ISearchFilterValue[] ActiveFilterValues { get; }

        /// <summary>
        /// Gets the active filter fields.
        /// </summary>
        /// <value>The active filter fields.</value>
        string[] ActiveFilterFields { get; }

        /// <summary>
        /// Adds the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="value">The value.</param>
        void Add(string field, ISearchFilterValue value);

        /// <summary>
        /// Gets the sorts.
        /// </summary>
        /// <value>The sorts.</value>
        SearchSort Sort { get; set; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        string Scope { get; }

        /// <summary>
        /// Gets the key field.
        /// </summary>
        /// <value>The key field.</value>
        string KeyField { get; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        string Locale { get; set; }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        string CacheKey { get;  }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>The query.</value>
        Query Query {get;}
    }
}