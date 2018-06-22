using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;
using System.Collections;
using SpellChecker.Net.Search.Spell;

namespace Mediachase.Search
{
    public class SearchResults
    {
        private Hits _Hits;
        private Query _BaseQuery;
        private IndexReader _IndexReader;
        private ISearchCriteria _SearchCriteria;

        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount
        {
            get
            {
                if (_Hits != null)
                    return _Hits.Length();
                else
                    return 0;
            }
        }

        FacetGroup[] _FacetGroups;

        /// <summary>
        /// Gets the facet groups.
        /// </summary>
        /// <value>The facet groups.</value>
        public FacetGroup[] FacetGroups
        {
            get
            {
                if (_FacetGroups == null)
                {
                    List<FacetGroup> groups = new List<FacetGroup>();
                    IndexReader reader = _IndexReader;
                    QueryFilter baseQueryFilter = new QueryFilter(_BaseQuery);
                    BitArray baseBitArray = baseQueryFilter.Bits(reader);

                    foreach (SearchFilter filter in _SearchCriteria.Filters)
                    {
                        FacetGroup group = new FacetGroup();
                        group.Name = SearchCommon.GetDescription(_SearchCriteria.Locale, filter.Descriptions);
                        group.FieldName = filter.field;

                        int groupCount = 0;

                        groupCount += GetFilterCount(reader, baseBitArray, group, filter.field, filter.Values.SimpleValue);
                        groupCount += GetFilterCount(reader, baseBitArray, group, filter.field, filter.Values.RangeValue);
                        groupCount += GetFilterCount(reader, baseBitArray, group, filter.field, filter.Values.PriceRangeValue);

                        // Add only if items exist under
                        if (groupCount > 0)
                        {
                            groups.Add(group);
                        }
                    }

                    _FacetGroups = groups.ToArray();
                }

                return _FacetGroups;
            }
        }


        /// <summary>
        /// Gets the int results.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="recordsToRetrieve">The records to retrieve.</param>
        /// <returns></returns>
        public int[] GetIntResults(int startIndex, int recordsToRetrieve)
        {
            if (_Hits == null)
                return null;

            List<int> entries = new List<int>();

            int totalCount = _Hits.Length();

            if (recordsToRetrieve > totalCount)
                recordsToRetrieve = totalCount;

            int duplicates = 0;
            for (int index = startIndex; index < startIndex + recordsToRetrieve + duplicates; index++)
            {
                if (index >= totalCount)
                    break;

                int id = Int32.Parse(_Hits.Doc(index).Get("_id"));

                // If we find a duplicate value, skip it and go to the next one
                if (entries.Contains(id))
                {
                    duplicates++;
                    continue;
                }

                entries.Add(id);
            }

            return entries.ToArray();

        }

        /// <summary>
        /// Gets the similar words.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public string[] GetSimilarWords(string fieldName, string word)
        {
            SpellChecker.Net.Search.Spell.SpellChecker spell = new SpellChecker.Net.Search.Spell.SpellChecker(_IndexReader.Directory());
            spell.IndexDictionary(new LuceneDictionary(_IndexReader, fieldName));
            return spell.SuggestSimilar(word, 2);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResults"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="criteria">The criteria.</param>
        public SearchResults(IndexReader reader, Hits hits, ISearchCriteria criteria)
        {
            _IndexReader = reader;
            _Hits = hits;
            _BaseQuery = criteria.Query;
            _SearchCriteria = criteria;
        }

        #region Private Helper Functions

        /// <summary>
        /// Gets the filter count.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="baseArray">The base array.</param>
        /// <param name="facetGroup">The facet group.</param>
        /// <param name="filterField">The filter field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private int GetFilterCount(IndexReader reader, BitArray baseArray, FacetGroup facetGroup, string filterField, SimpleValue[] values)
        {
            if (values == null)
                return 0;

            int count = 0;

            foreach (SimpleValue value in values)
            {
                QueryFilter queryFilter = new QueryFilter(SearchCommon.CreateQuery(filterField, value));
                BitArray filterArray = queryFilter.Bits(reader);
                int newCount = GetFacetHitCount(baseArray, filterArray);

                if (newCount == 0)
                    continue;

                Facet newFacet = new Facet();
                newFacet.Name = SearchCommon.GetDescription(_SearchCriteria.Locale, value.Descriptions);
                newFacet.Count = newCount;
                newFacet.Key = value.key;
                newFacet.Group = facetGroup;

                facetGroup.Facets.Add(newFacet);

                count += newCount;
            }

            return count;
        }

        /// <summary>
        /// Gets the filter count.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="baseArray">The base array.</param>
        /// <param name="facetGroup">The facet group.</param>
        /// <param name="filterField">The filter field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private int GetFilterCount(IndexReader reader, BitArray baseArray, FacetGroup facetGroup, string filterField, RangeValue[] values)
        {
            if (values == null)
                return 0;

            int count = 0;

            foreach (RangeValue value in values)
            {
                QueryFilter queryFilter = new QueryFilter(SearchCommon.CreateQuery(filterField, value));
                BitArray filterArray = queryFilter.Bits(reader);
                int newCount = GetFacetHitCount(baseArray, filterArray);

                if (newCount == 0)
                    continue;

                Facet newFacet = new Facet();
                newFacet.Name = SearchCommon.GetDescription(_SearchCriteria.Locale, value.Descriptions);
                newFacet.Count = newCount;
                newFacet.Key = value.key;
                newFacet.Group = facetGroup;

                facetGroup.Facets.Add(newFacet);

                count += newCount;
            }

            return count;
        }

        /// <summary>
        /// Gets the filter count.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="baseArray">The base array.</param>
        /// <param name="facetGroup">The facet group.</param>
        /// <param name="filterField">The filter field.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private int GetFilterCount(IndexReader reader, BitArray baseArray, FacetGroup facetGroup, string filterField, PriceRangeValue[] values)
        {
            if (values == null)
                return 0;

            int count = 0;

            foreach (PriceRangeValue value in values)
            {
                QueryFilter queryFilter = new QueryFilter(SearchCommon.CreateQuery(filterField, value));
                BitArray filterArray = queryFilter.Bits(reader);
                int newCount = GetFacetHitCount(baseArray, filterArray);

                if (newCount == 0)
                    continue;

                Facet newFacet = new Facet();
                newFacet.Name = SearchCommon.GetDescription(_SearchCriteria.Locale, value.Descriptions);
                newFacet.Count = newCount;
                newFacet.Key = value.key;
                newFacet.Group = facetGroup;

                facetGroup.Facets.Add(newFacet);

                count += newCount;
            }

            return count;
        }

        /// <summary>
        /// Gets the facet hit count.
        /// </summary>
        /// <param name="baseBitSet">The base bit set.</param>
        /// <param name="filterBitSet">The filter bit set.</param>
        /// <returns></returns>
        private int GetFacetHitCount(BitArray baseBitSet, BitArray filterBitSet)
        {
            filterBitSet.And(baseBitSet);
            int cardinality = 0;
            for (int i = 0; i < filterBitSet.Count; i++)
            {
                if (filterBitSet.Get(i)) cardinality++;
            }
            return cardinality;
        }
        #endregion
    }
}
