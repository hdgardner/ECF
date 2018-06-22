using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis;
using System.Collections.Specialized;
using Mediachase.Commerce.Shared;

namespace Mediachase.Search.Extensions
{
    public class CatalogEntrySearchCriteria : AbstractSearchCriteria
    {
        /// <summary>
        /// Gets the default sort order.
        /// </summary>
        /// <value>The default sort order.</value>
        public static SearchSort DefaultSortOrder { get { return new SearchSort("_sortorder", false);} }

        /// <summary>
        /// Gets or sets the sort.
        /// </summary>
        /// <value>The sort.</value>
        public override SearchSort Sort
        {
            get
            {
                /*
                if (base.Sort == null)
                {
                    base.Sort = new SearchSort("_sortorder", false);
                }
                 * */
                return base.Sort;
            }
            set
            {
                base.Sort = value;
            }
        }

        private bool _IsFuzzySearch = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fuzzy search.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is fuzzy search; otherwise, <c>false</c>.
        /// </value>
        public bool IsFuzzySearch
        {
            get { return _IsFuzzySearch; }
            set { ChangeState(); _IsFuzzySearch = value; }
        }

        private float _FuzzyMinSimilarity = FuzzyQuery.defaultMinSimilarity;

        /// <summary>
        /// Gets or sets the fuzzy min similarity.
        /// </summary>
        /// <value>The fuzzy min similarity.</value>
        public float FuzzyMinSimilarity
        {
            get { return _FuzzyMinSimilarity; }
            set { ChangeState(); _FuzzyMinSimilarity = value; }
        }

        private StringCollection _CatalogNames = new StringCollection();

        /// <summary>
        /// Gets or sets the catalog names.
        /// </summary>
        /// <value>The catalog names.</value>
        public virtual StringCollection CatalogNames
        {
            get { return _CatalogNames; }
            set { ChangeState(); _CatalogNames = value; }
        }

        private StringCollection _CatalogNodes = new StringCollection();

        /// <summary>
        /// Gets or sets the catalog node codes.
        /// </summary>
        /// <value>The catalog node code.</value>
        public virtual StringCollection CatalogNodes
        {
            get { return _CatalogNodes; }
            set { ChangeState(); _CatalogNodes = value; }
        }

        private string _SearchPhrase = String.Empty;

        /// <summary>
        /// Gets or sets the search phrase.
        /// </summary>
        /// <value>The search phrase.</value>
        public virtual string SearchPhrase
        {
            get { return _SearchPhrase; }
            set { ChangeState(); _SearchPhrase = value; }
        }

        private StringCollection _SearchIndex = new StringCollection();

        /// <summary>
        /// Gets or sets the indexes of the search.
        /// </summary>
        /// <value>The index of the search.</value>
        public virtual StringCollection SearchIndex
        {
            get { return _SearchIndex; }
            set { ChangeState(); _SearchIndex = value; }
        }

        private StringCollection _ClassType = new StringCollection();

        /// <summary>
        /// Gets or sets the class types.
        /// </summary>
        /// <value>The class types.</value>
        public virtual StringCollection ClassTypes
        {
            get { return _ClassType; }
            set { ChangeState(); _ClassType = value; }
        }

        private DateTime _StartDate = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { ChangeState(); _StartDate = value; }
        }
        
        private DateTime _EndDate = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { ChangeState(); _EndDate = value; }
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public override string Scope
        {
            get { return SearchScope.CatalogEntry; }
        }

		public bool IsISBN {
			get {
				//string keyWords = this.SearchPhrase;
				//check to see if the KeyWords matches an ISBN Pattern
				if (System.Text.RegularExpressions.Regex.IsMatch(this.SearchPhrase, @"^[0-9-]+[a-z-0-9]?$")) { //any string of all hyphens or numbers and possibly an alpha character at the end
					return true;
				}
				return false;
			}
		}

        Query _Query = null;
        /// <summary>
        /// Gets the build query.
        /// </summary>
        /// <value>The build query.</value>
        public override Query Query
        {
            get
            {
                if (_Query != null && !IsModified)
                    return _Query;

                BooleanQuery query = (BooleanQuery)base.Query;

                AddQuery("_catalog", query, CatalogNames);
                AddQuery("_node", query, CatalogNodes);

                // Add search
                if (!String.IsNullOrEmpty(SearchPhrase)){
					
					//Node: this line is not part of the original Source code. 
					string searchPhrase = this.IsISBN ?  SearchPhrase.Replace("-", string.Empty)  : SearchPhrase;
                   
					if (IsFuzzySearch)
                    {
                        StandardAnalyzer analyzer = new StandardAnalyzer();
                        TokenStream source = analyzer.TokenStream("_content", new System.IO.StringReader(SearchPhrase));

                        System.Collections.ArrayList v = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
                        Lucene.Net.Analysis.Token t;

                        while (true)
                        {
                            try
                            {
                                t = source.Next();
                            }
                            catch (System.IO.IOException e)
                            {
                                t = null;
                            }
                            if (t == null)
                                break;
                            v.Add(t);
                        }
                        try
                        {
                            source.Close();
                        }
                        catch (System.IO.IOException e)
                        {
                            // ignore
                        }

                        if (v.Count > 0)
                        {
                            // no phrase query:
                            BooleanQuery q = new BooleanQuery(true);
                            for (int i = 0; i < v.Count; i++)
                            {
                                t = (Lucene.Net.Analysis.Token)v[i];
                                FuzzyQuery currentQuery = new FuzzyQuery(new Term("_content", t.TermText()));
                                q.Add(currentQuery, BooleanClause.Occur.MUST);
                            }
                            query.Add(q, BooleanClause.Occur.MUST);
                        }

                        /*
                        QueryParser parser = new QueryParser("_content", new StandardAnalyzer());
                        Query searchQuery = parser.GetFuzzyQuery("_content", SearchPhrase, FuzzyMinSimilarity);
                        //FuzzyQuery searchQuery = new FuzzyQuery(new Term("_content", SearchPhrase), FuzzyMinSimilarity);
                        query.Add(searchQuery, BooleanClause.Occur.MUST);
                         * */
                    }
                    else
                    {
                        QueryParser parser = new QueryParser("_content", new StandardAnalyzer());
						parser.SetAllowLeadingWildcard(true); //NOTE: this is a change to the source code. This line was not originally here.
                        parser.SetDefaultOperator(QueryParser.Operator.AND);
						// Query searchQuery = parser.Parse(SearchPhrase);  //NOTE: this is a change to the source code this line was originally uncommented
						if (this.IsISBN) searchPhrase = "*"+ searchPhrase; //NOTE: This is an addition to the original source code. If it's an ISBN, add a wildcard to the beginning
						WildcardQuery searchQuery = new WildcardQuery(new Term("_content", searchPhrase + "*" ));  //NOTE: this is a change to the source code. This line was originally commented out
                        query.Add(searchQuery, BooleanClause.Occur.MUST);
                    }
                }

                AddQuery("_metaclass", query, SearchIndex);
                AddQuery("_classtype", query, ClassTypes);

                // Add date filter                
                //ConstantScoreRangeQuery datesFilterStart = new ConstantScoreRangeQuery("StartDate", null, StartDate.ToString("s"), false, true);
                //query.Add(datesFilterStart, BooleanClause.Occur.MUST);
                //ConstantScoreRangeQuery datesFilterEnd = new ConstantScoreRangeQuery("EndDate", EndDate.ToString("s"), null, true, false);
                //query.Add(datesFilterEnd, BooleanClause.Occur.MUST);

                _Query = query;

                return _Query;
            }
        }

        /// <summary>
        /// Adds the query.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="query">The query.</param>
        /// <param name="filter">The filter.</param>
        private void AddQuery(string fieldName, BooleanQuery query, StringCollection filter)
        {
            if (filter.Count > 0)
            {
                if (filter.Count == 1)
                {
                    if (!String.IsNullOrEmpty(filter[0]))
                    {
                        TermQuery nodeQuery = new TermQuery(new Term(fieldName, filter[0]));
                        query.Add(nodeQuery, BooleanClause.Occur.MUST);
                    }
                }
                else
                {
                    BooleanQuery booleanQuery = new BooleanQuery();
                    bool containsFilter = false;
                    foreach (string index in filter)
                    {
                        if (!String.IsNullOrEmpty(index))
                        {
                            TermQuery nodeQuery = new TermQuery(new Term(fieldName, index));
                            booleanQuery.Add(nodeQuery, BooleanClause.Occur.SHOULD);
                            containsFilter = true;
                        }
                    }
                    if (containsFilter)
                        query.Add(booleanQuery, BooleanClause.Occur.MUST);
                }
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public override string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                key.Append("index:" + CommerceHelper.ConvertToString(this.SearchIndex, ","));
                key.Append("phrase:" + SearchPhrase);
                key.Append("startdate:" + StartDate.ToShortDateString());
                key.Append("enddate:" + EndDate.ToShortDateString());
                key.Append("node:" + CommerceHelper.ConvertToString(this.CatalogNodes, ","));
                key.Append("ctlg:" + CommerceHelper.ConvertToString(this.CatalogNames, ","));
                key.Append("ct:" + CommerceHelper.ConvertToString(this.ClassTypes, ","));

                return base.CacheKey + key.ToString();
            }
        }
    }
}
