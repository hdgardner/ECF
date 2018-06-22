using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Lucene.Net.Search;
using System.Collections.Specialized;
using Mediachase.Search;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Core;
using Lucene.Net.Documents;
using System.Web.Caching;
using Mediachase.Search.Extensions;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Cms.WebUtility.Search
{
    public class SearchFilterHelper
    {
        #region Local Fields
        private static object _LockObject = new object();
        private static volatile SearchConfig _SearchConfig = null;
        private SearchConfig _DynamicSearchConfig = null;
        private SearchManager _SearchManager = null;
        private static volatile Dictionary<string, CacheDependency> _Dependencies = new Dictionary<string, CacheDependency>();
        #endregion

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>The manager.</value>
        private SearchManager Manager
        {
            get
            {
                if (_SearchManager == null)
                    _SearchManager = new SearchManager(AppContext.Current.ApplicationName);

                return _SearchManager;
            }
        }

        #region Public Properties
        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static SearchFilterHelper Current
        {
            get
            {
                SearchFilterHelper helper = null;
                if (HttpContext.Current.Items["Mediachase.SearchFilterHelper"] == null)
                {
                    helper = new SearchFilterHelper();
                    HttpContext.Current.Items["Mediachase.SearchFilterHelper"] = helper;
                }
                else
                {
                    helper = (SearchFilterHelper)HttpContext.Current.Items["Mediachase.SearchFilterHelper"];
                }

                return helper;
            }
        }

        private SearchResults _Results;

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>The results.</value>
        public SearchResults Results
        {
            get { return _Results; }
        }

        /// <summary>
        /// Gets the search config.
        /// </summary>
        /// <value>The search config.</value>
        /// <returns></returns>
        public SearchConfig SearchConfig
        {
            get
            {
                bool changed = HasConfigChanged("BaseConfig");
                if (_SearchConfig == null || changed)
                {
                    lock (_LockObject)
                    {
                        changed = HasConfigChanged("BaseConfig");
                        if (_SearchConfig == null || changed)
                        {
                            string path = HttpContext.Current.Server.MapPath("~/App_Data/searchFilters.config");
                            TextReader reader = new StreamReader(path);
                            XmlSerializer serializer = new XmlSerializer(typeof(SearchConfig));
                            _SearchConfig = (SearchConfig)serializer.Deserialize(reader);
                            reader.Close();

                            _Dependencies["BaseConfig"] = new CacheDependency(path, DateTime.Now);
                        }
                    }
                }

                if(_DynamicSearchConfig != null)
                {
                    return _DynamicSearchConfig;
                }
                else
                {
                    _DynamicSearchConfig = new SearchConfig();
                }

                // Add dynamic fields here like categories
                string code = HttpContext.Current.Request["nc"];
                if (!String.IsNullOrEmpty(code))
                {
                    CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
                    foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
                    {
                        CatalogNodes nodes = CatalogContext.Current.GetCatalogNodes(catalog.Name, code);
                        if (nodes.CatalogNode != null && nodes.CatalogNode.Length > 0)
                        {
                            List<SearchFilter> filters = new List<SearchFilter>();
                            foreach (CatalogNode nodeRow in nodes.CatalogNode)
                            {
                                SearchFilter configFilter = new SearchFilter();
                                configFilter.field = "_node";

                                configFilter.Descriptions = new Descriptions();
                                configFilter.Descriptions.defaultLocale = "en-us";

                                Description desc = new Description();
                                desc.locale = "en-us";
                                desc.Value = new StoreResourceManager().GetString("CATEGORY_SHOP_LABEL");
                                configFilter.Descriptions.Description = new Description[] { desc };

                                SimpleValue val = new SimpleValue();
                                val.key = nodeRow.ID;
                                val.value = nodeRow.ID;
                                val.Descriptions = new Descriptions();
                                val.Descriptions.defaultLocale = "en-us";
                                Description desc2 = new Description();
                                desc2.locale = "en-us";
                                desc2.Value = StoreHelper.GetNodeDisplayName(nodeRow);
                                val.Descriptions.Description = new Description[] { desc2 };

                                configFilter.Values = new SearchFilterValues();
                                configFilter.Values.SimpleValue = new SimpleValue[] { val };
                                filters.Add(configFilter);
                            }
                            _DynamicSearchConfig.SearchFilters = filters.ToArray();
                        }
                    }
                }

                List<SearchFilter> existingFilters = null;
                if(_DynamicSearchConfig.SearchFilters!=null)
                    existingFilters = new List<SearchFilter>(_DynamicSearchConfig.SearchFilters);
                else
                    existingFilters = new List<SearchFilter>();

                foreach (SearchFilter filter in _SearchConfig.SearchFilters)
                {
                    existingFilters.Add(filter);
                }

                _DynamicSearchConfig.SearchFilters = existingFilters.ToArray();

                return _DynamicSearchConfig;
            }
        }
        #endregion

        /// <summary>
        /// Determines whether [has config changed].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [has config changed]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasConfigChanged(string name)
        {
            lock (_LockObject)
            {
                if (!_Dependencies.ContainsKey(name))
                    return true;

                if (_Dependencies[name] == null || _Dependencies[name].HasChanged)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchFilterHelper"/> class.
        /// </summary>
        private SearchFilterHelper()
        {
            Manager.Analyzer = new WhitespaceAnalyzer();
        }

        #region Search Methods
        /// <summary>
        /// Gets the facets.
        /// </summary>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        public virtual FacetGroup[] GetFacets(bool cacheResults, TimeSpan cacheTimeout)
        {
            NameValueCollection querystring = HttpContext.Current.Request.QueryString;

            string cacheKey = String.Empty;
            CatalogEntrySearchCriteria criteria = CreateSearchCriteria(querystring["search"], null);

            if (cacheResults)
            {
            // Only cache results if specified
                cacheKey = CatalogCache.CreateCacheKey("search-catalog-facets", criteria.CacheKey);

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                {
                    return (FacetGroup[])cachedObject;
                }
            }

            FacetGroup[] groups = null;

            if(_Results != null)
                groups = _Results.FacetGroups;
            else
                groups = SearchEntries(criteria).FacetGroups;

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, groups, cacheTimeout);
            }
            return groups;
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <returns></returns>
        public virtual SearchResults SearchEntries()
        {
            NameValueCollection querystring = HttpContext.Current.Request.QueryString;
            return SearchEntries(querystring["search"], null);
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="recordsToRetrieve">The records to retrieve.</param>
        /// <param name="count">The count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        public virtual Entries SearchEntries(string keywords, SearchSort sort, int startIndex, int recordsToRetrieve, out int count, CatalogEntryResponseGroup responseGroup, bool cacheResults, TimeSpan cacheTimeout)
        {
            CatalogEntrySearchCriteria criteria = CreateSearchCriteria(keywords, sort);
            return SearchEntries(criteria, startIndex, recordsToRetrieve, out count, responseGroup, cacheResults, cacheTimeout);
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="recordsToRetrieve">The records to retrieve.</param>
        /// <param name="count">The count.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="cacheResults">if set to <c>true</c> [cache results].</param>
        /// <param name="cacheTimeout">The cache timeout.</param>
        /// <returns></returns>
        public virtual Entries SearchEntries(CatalogEntrySearchCriteria criteria, int startIndex, int recordsToRetrieve, out int count, CatalogEntryResponseGroup responseGroup, bool cacheResults, TimeSpan cacheTimeout)
        {
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = String.Empty;

            // Only cache results if specified
            if (cacheResults)
            {
                cacheKey = CatalogCache.CreateCacheKey("seach-catalog-entries", responseGroup.CacheKey, criteria.CacheKey, "start:"+startIndex.ToString(), "end:"+recordsToRetrieve.ToString());

                // check cache first
                object cachedObject = CatalogCache.Get(cacheKey);

                if (cachedObject != null)
                {
                    Entries cachedEntries = (Entries)cachedObject;
                    count = cachedEntries.TotalResults;
                    return cachedEntries;
                }
            }

            // Perform Lucene search
            SearchResults results = SearchEntries(criteria);
            count = results.TotalCount;
            
            // Get IDs we need
            int[] resultIndexes = results.GetIntResults(startIndex, recordsToRetrieve + 5); // we add padding here to accomodate entries that might have been deleted since last indexing
            
            // Retrieve actual entry objects, with no caching
            Entries entries = CatalogContext.Current.GetCatalogEntries(resultIndexes, false, new TimeSpan(), responseGroup);
            entries.TotalResults = count;

            if (!String.IsNullOrEmpty(cacheKey)) // cache results
            {
                // Insert to the cache collection
                CatalogCache.Insert(cacheKey, entries, cacheTimeout);
            }

            return entries;
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public virtual SearchResults SearchEntries(string keywords, SearchSort sort)
        {
            CatalogEntrySearchCriteria criteria = CreateSearchCriteria(keywords, sort);
            return SearchEntries(criteria);
        }

        /// <summary>
        /// Searches the entries.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public virtual SearchResults SearchEntries(CatalogEntrySearchCriteria criteria)
        {
            try
            {
				if (criteria.IsISBN) Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(Int32.MaxValue);
                _Results = Manager.Search(criteria);
            }
            catch (SystemException)
            {
                if (HttpContext.Current.IsDebuggingEnabled)
                    throw;
            }

            // Perform fuzzy search if nothing has been found AND the search phrase is NOT a big number (e.g. ISBN)
			if (_Results.TotalCount == 0 && !criteria.IsISBN) {
				Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(1024);
				criteria.IsFuzzySearch = true;
				criteria.FuzzyMinSimilarity = 0.7f;
				_Results = Manager.Search(criteria);
			}
			Lucene.Net.Search.BooleanQuery.SetMaxClauseCount(1024);
            return _Results;
        }

        /// <summary>
        /// Creates the search criteria.
        /// </summary>
        /// <param name="keywords">The keywords.</param>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public CatalogEntrySearchCriteria CreateSearchCriteria(string keywords, SearchSort sort)
        {
            NameValueCollection querystring = HttpContext.Current.Request.QueryString;
            string currentNodeCode = querystring["nc"];
            CatalogEntrySearchCriteria criteria = new CatalogEntrySearchCriteria();

            if (!String.IsNullOrEmpty(currentNodeCode))
            {
                criteria.CatalogNodes.Add(currentNodeCode);

                // Include all the sub nodes in our search results
                CatalogDto catalogs = CatalogContext.Current.GetCatalogDto(CMSContext.Current.SiteId);
                foreach (CatalogDto.CatalogRow catalog in catalogs.Catalog)
                {
                    CatalogNodes nodes = CatalogContext.Current.GetCatalogNodes(catalog.Name, querystring["nc"]);
                    if (nodes.CatalogNode != null && nodes.CatalogNode.Length > 0)
                    {
                        foreach (CatalogNode nodeRow in nodes.CatalogNode)
                        {
                            criteria.CatalogNodes.Add(nodeRow.ID);
                        }
                    }
                }
            }

            criteria.SearchPhrase = keywords;
            criteria.SearchIndex.Add(querystring["filter"]);
            criteria.Sort = sort;

            // Add all filters
            foreach (SearchFilter filter in SearchConfig.SearchFilters)
            {
                // Check if we already filtering
                if (querystring.Get(filter.field) != null)
                    continue;

                criteria.Add(filter);
            }

            // Get selected filters
            SelectedFilter[] filters = SelectedFilters;
            if (filters.Length != 0)
            {
                foreach (SelectedFilter filter in filters)
                {
                    if (filter.PriceRangeValue != null)
                        criteria.Add(filter.Filter.field, filter.PriceRangeValue);
                    if (filter.RangeValue != null)
                        criteria.Add(filter.Filter.field, filter.RangeValue);
                    if (filter.SimpleValue != null)
                        criteria.Add(filter.Filter.field, filter.SimpleValue);
                }
            }

            return criteria;
        }
        #endregion

        #region QueryString Methods
        /// <summary>
        /// Gets the clean URL without filters.
        /// </summary>
        /// <returns></returns>
        public string GetCleanUrl()
        {
            string url = CMSContext.Current.CurrentUrl;

            foreach (SelectedFilter filter in this.SelectedFilters)
            {
                url = GetQueryStringNavigateUrl(url, filter.Filter.field, String.Empty, true);
            }

            return url;
        }

        /// <summary>
        /// Gets the query string navigate URL.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetQueryStringNavigateUrl(string field, string key)
        {
            return GetQueryStringNavigateUrl(field, key, false);
        }

        /// <summary>
        /// Gets the query string navigate URL.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string GetQueryStringNavigateUrl(string field, bool remove)
        {
            return GetQueryStringNavigateUrl(field, String.Empty, remove);
        }

        /// <summary>
        /// Gets the query string navigate URL.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="key">The key.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string GetQueryStringNavigateUrl(string field, string key, bool remove)
        {
            return GetQueryStringNavigateUrl(CMSContext.Current.CurrentUrl, field, key, remove);
        }

        /// <summary>
        /// Gets the query string navigate URL.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="key">The key.</param>
        /// <param name="remove">if set to <c>true</c> [remove].</param>
        /// <returns></returns>
        public static string GetQueryStringNavigateUrl(string url, string field, string key, bool remove)
        {
            string queryField = field;

            StringBuilder builder = new StringBuilder();

            string path = String.Empty;
            if (url.Contains("?"))
            {
                builder.Append(url.Substring(0, url.IndexOf("?")));
                builder.Append("?");

                NameValueCollection query = new NameValueCollection();
                string queryString = url.Substring(url.IndexOf("?") + 1);
                string[] queryParamsArray = queryString.Split(new char[] { '&' });
                foreach (string queryParam in queryParamsArray)
                {
                    string[] queryParamArray = queryParam.Split(new char[] { '=' });
                    if (!queryParamArray[0].Equals(queryField, StringComparison.OrdinalIgnoreCase))
                    {
                        builder.Append(queryParamArray[0]);
                        builder.Append("=");
                        if (queryParamArray.Length > 1)
                            builder.Append(queryParamArray[1]);
                        builder.Append("&");
                    }
                }
            }
            else
            {
                builder.Append(url);
                builder.Append("?");
            }

            if (!remove)
            {
                builder.Append(queryField);
                builder.Append("=");
                builder.Append(key);
            }
            else
            {
                builder.Remove(builder.Length-1, 1);
            }


            return builder.ToString();
        }

        SelectedFilter[] _SelectedFilters = null;
        /// <summary>
        /// Gets the selected filters.
        /// </summary>
        /// <value>The selected filters.</value>
        public virtual SelectedFilter[] SelectedFilters
        {
            get
            {
                if (_SelectedFilters != null)
                    return _SelectedFilters;

                NameValueCollection querystring = HttpContext.Current.Request.QueryString;
                List<SelectedFilter> filters = new List<SelectedFilter>();

                // Cycle through query string to see if we have any filters specified
                foreach (string key in querystring.AllKeys)
                {
                    bool foundTerm = false;
                    SearchConfig config = SearchConfig;
                    foreach (SearchFilter filter in config.SearchFilters)
                    {
                        // we found a filter!
                        if (filter.field.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            // Now find the value
                            string val = querystring[key];

                            // Simple values
                            if (filter.Values.SimpleValue != null)
                            {
                                foreach (SimpleValue value in filter.Values.SimpleValue)
                                {
                                    if (value.key.Equals(val, StringComparison.OrdinalIgnoreCase))
                                    {
                                        filters.Add(new SelectedFilter(filter, value));
                                        foundTerm = true;
                                        break;
                                    }
                                }
                            }

                            // Range values
                            if (filter.Values.RangeValue != null)
                            {
                                foreach (RangeValue value in filter.Values.RangeValue)
                                {
                                    if (value.key.Equals(val, StringComparison.OrdinalIgnoreCase))
                                    {
                                        filters.Add(new SelectedFilter(filter, value));
                                        foundTerm = true;
                                        break;
                                    }
                                }
                            }

                            // Price Range values
                            if (filter.Values.PriceRangeValue != null)
                            {
                                foreach (PriceRangeValue value in filter.Values.PriceRangeValue)
                                {
                                    if (value.key.Equals(val, StringComparison.OrdinalIgnoreCase))
                                    {
                                        filters.Add(new SelectedFilter(filter, value));
                                        foundTerm = true;
                                        break;
                                    }
                                }
                            }

                        }

                        if (foundTerm)
                            break;
                    }
                }

                _SelectedFilters = filters.ToArray();
                return _SelectedFilters;
            }
        }
        #endregion
    }
}
