using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using System.Globalization;

namespace Mediachase.Search
{
    public class SearchCommon
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="descs">The descs.</param>
        /// <returns></returns>
        public static string GetDescription(string locale, Descriptions descs)
        {
            string defaultLocale = descs.defaultLocale;
            string defaultValue = String.Empty;
            string currentLocale = locale;
            foreach (Description desc in descs.Description)
            {
                if (desc.locale.Equals(currentLocale, StringComparison.OrdinalIgnoreCase))
                    return desc.Value;

                if (desc.locale.Equals(defaultLocale, StringComparison.OrdinalIgnoreCase))
                    defaultValue = desc.Value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Query CreateQuery(string field, ISearchFilterValue value)
        {
            if (typeof(RangeValue) == value.GetType())
            {
                return CreateQuery(field, (RangeValue)value);
            }
            else if (typeof(PriceRangeValue) == value.GetType())
            {
                return CreateQuery(field, (PriceRangeValue)value);
            }
            else if (typeof(SimpleValue) == value.GetType())
            {
                return CreateQuery(field, (SimpleValue)value);
            }
            return null;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Query CreateQuery(string field, RangeValue value)
        {
            Query query = new ConstantScoreRangeQuery(field, value.numeric ? ConvertToSearchable(value.lowerbound) : value.lowerbound, value.numeric ? ConvertToSearchable(value.upperbound) : value.upperbound, value.lowerboundincluded, value.upperboundincluded);
            return query;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Query CreateQuery(string field, PriceRangeValue value)
        {
            RangeFilter filter = null;
            string upper = value.upperbound;
            if (String.IsNullOrEmpty(upper))
            {
                upper = NumberTools.MAX_STRING_VALUE;
            }
            else
            {
                upper = ConvertDecimalToSearchable(value.upperbound);
            }

            filter = new RangeFilter(String.Format("{0}{1}", field, value.currency), ConvertDecimalToSearchable(value.lowerbound), upper, value.lowerboundincluded, value.upperboundincluded);
            Query query = new ConstantScoreQuery(filter);// new ConstantScoreRangeQuery(String.Format("{0}{1}", field, value.currency), ConvertToSearchable(value.lowerbound), upper, value.lowerboundincluded, value.upperboundincluded);
            return query;
        }

        public static string ConvertDecimalToSearchable(string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            string valString = String.Empty;
            valString = ((decimal)Decimal.Parse(input, CultureInfo.InvariantCulture)).ToString("####.0000", CultureInfo.InvariantCulture);
            valString = NumberTools.LongToString(long.Parse(valString.Replace(".", ""), CultureInfo.InvariantCulture));

            return valString;
        }

        /// <summary>
        /// Converts to searchable.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ConvertToSearchable(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            //string valString = Convert.ToDouble(value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            return NumberTools.LongToString(long.Parse(value.Replace(".", ""), CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Query CreateQuery(string field, SimpleValue value)
        {
            Query query = new TermQuery(new Term(field, value.numeric ? ConvertToSearchable(value.value) : value.value));
            return query;
        }
    }
}
