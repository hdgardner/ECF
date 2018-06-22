using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Search;

namespace Mediachase.Cms.WebUtility.Search
{
    public class SelectedFilter
    {
        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _RemoveUrl;


        /// <summary>
        /// Gets or sets the remove URL.
        /// </summary>
        /// <value>The remove URL.</value>
        public string RemoveUrl
        {
            get { return _RemoveUrl; }
            set { _RemoveUrl = value; }
        }
        private string _ValueName;

        /// <summary>
        /// Gets or sets the name of the value.
        /// </summary>
        /// <value>The name of the value.</value>
        public string ValueName
        {
            get { return _ValueName; }
            set { _ValueName = value; }
        }

        private SearchFilter _Filter;

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public SearchFilter Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }
        private SimpleValue _SimpleValue;

        /// <summary>
        /// Gets or sets the simple value.
        /// </summary>
        /// <value>The simple value.</value>
        public SimpleValue SimpleValue
        {
            get { return _SimpleValue; }
            set { _SimpleValue = value; }
        }
        private RangeValue _RangeValue;

        /// <summary>
        /// Gets or sets the range value.
        /// </summary>
        /// <value>The range value.</value>
        public RangeValue RangeValue
        {
            get { return _RangeValue; }
            set { _RangeValue = value; }
        }
        private PriceRangeValue _PriceRangeValue;

        /// <summary>
        /// Gets or sets the price range value.
        /// </summary>
        /// <value>The price range value.</value>
        public PriceRangeValue PriceRangeValue
        {
            get { return _PriceRangeValue; }
            set { _PriceRangeValue = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedFilter"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="value">The value.</param>
        public SelectedFilter(SearchFilter filter, SimpleValue value)
        {
            _Filter = filter;
            _SimpleValue = value;
            _Name = SearchCommon.GetDescription(CMSContext.Current.LanguageName, filter.Descriptions);
            _ValueName = SearchCommon.GetDescription(CMSContext.Current.LanguageName, value.Descriptions);
            _RemoveUrl = SearchFilterHelper.GetQueryStringNavigateUrl(filter.field, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedFilter"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="value">The value.</param>
        public SelectedFilter(SearchFilter filter, RangeValue value)
        {
            _Filter = filter;
            _RangeValue = value;
            _Name = SearchCommon.GetDescription(CMSContext.Current.LanguageName, filter.Descriptions);
            _ValueName = SearchCommon.GetDescription(CMSContext.Current.LanguageName, value.Descriptions);
            _RemoveUrl = SearchFilterHelper.GetQueryStringNavigateUrl(filter.field, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedFilter"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="value">The value.</param>
        public SelectedFilter(SearchFilter filter, PriceRangeValue value)
        {
            _Filter = filter;
            _PriceRangeValue = value;
            _Name = SearchCommon.GetDescription(CMSContext.Current.LanguageName, filter.Descriptions);
            _ValueName = SearchCommon.GetDescription(CMSContext.Current.LanguageName, value.Descriptions);
            _RemoveUrl = SearchFilterHelper.GetQueryStringNavigateUrl(filter.field, true);
        }
    }

}
