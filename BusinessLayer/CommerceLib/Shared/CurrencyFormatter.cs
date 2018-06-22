using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using System.Linq;
using System.Data;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements operations for the currecy formatter.
    /// </summary>
    public static class CurrencyFormatter
    {
        private static volatile CultureInfo[] _Cultures = null;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the cultures.
        /// </summary>
        /// <value>The cultures.</value>
        public static CultureInfo[] Cultures
        {
            get
            {
                if (_Cultures == null)
                {
                    lock (_lockObject)
                    {
                        if (_Cultures == null)
                        {
                            _Cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                        }
                    }
                }

                return _Cultures;
            }
        }

        /// <summary>
        /// Return the three letter ISO currency code for the current thread.
        /// </summary>
        /// <returns>current currency code in String</returns>
        public static String CurrentCurrencyCode()
        {
            return new RegionInfo(Thread.CurrentThread.CurrentCulture.LCID).ISOCurrencySymbol;
        }

        /// <summary>
        /// Return the object which represents the place and language which matches the currency code which
        /// the database is able to support.  Fall back to Current Thread's culture if the currencyCode we requested doesn't 
        /// match.
        /// </summary>
        /// <param name="currencyCode">the currencycode to be matched for the culture</param>>
        /// <returns>CultureInfo object</returns>
        public static CultureInfo EffectiveCulture(string currencyCode)
        {
            if (CurrentCurrencyCode().Equals(currencyCode))
                return CultureInfo.CurrentCulture;
            else
            {
                // Find currency culture
                foreach (CultureInfo info in Cultures)
                {
                    if (new RegionInfo(info.LCID).ISOCurrencySymbol.Equals(currencyCode, StringComparison.OrdinalIgnoreCase))
                        return info;
                }
                return Thread.CurrentThread.CurrentCulture;
            }
        }


        /// <summary>
        /// Attempt to format the currency based on the browser's locale, but if that currency
        /// is not in the database, then fallback to current thread's culture.
        /// </summary>
        /// <param name="amount">the amount to be formated</param>
        /// <param name="currencyCode">currency code which will be used to find the
        /// effective culture</param>
        /// <returns>Formatted currency in String</returns>
        public static String FormatCurrency(decimal amount, string currencyCode)
        {
            return String.Format(EffectiveCulture(currencyCode), "{0:c}", amount);
        }

		/// <summary>
		/// Converts currency from currencyFrom to currencyTo. 
		/// It will try to find the direct way to convert. If none is found 
		/// it will try to find reverse way, if that is not available the base currency will 
		/// be returned.
		/// </summary>
		/// <remarks>
		///		This function will actually return the effective currency code that was applied.
		/// </remarks>
		/// <param name="amount">the amount of money to be converted</param>
		/// <param name="currencyFrom"></param>
		/// <param name="currencyTo"></param>
		/// <returns>returns converted amount</returns>
		public static decimal ConvertCurrency(decimal amount, string currencyFrom, ref string currencyTo)
		{
			if (String.IsNullOrEmpty(currencyFrom) || String.IsNullOrEmpty(currencyTo))
			{
				currencyTo = currencyFrom;
				return amount;
			}

			// Return amount if no need to convert
			if (String.Compare(currencyFrom, currencyTo, StringComparison.OrdinalIgnoreCase) == 0)
				return amount;

			CurrencyDto dtoFrom = CurrencyManager.GetCurrencyByCurrencyCode(currencyFrom);
			CurrencyDto dtoTo = CurrencyManager.GetCurrencyByCurrencyCode(currencyTo);

			decimal calculatedAmount = amount;

			//get currency ids
			int toId = 0;
			if(dtoTo.Currency.Count>0)
				toId = dtoTo.Currency[0].CurrencyId;

			// get rate for the current pair of currencies
			var rateTemp = from currencyRateTable in dtoFrom.CurrencyRate
					  orderby ((DataRow)currencyRateTable).Field<DateTime>("CurrencyRateDate") descending
					  where ((DataRow)currencyRateTable).Field<int>("ToCurrencyId") == toId
					  select currencyRateTable.AverageRate;

			double? averageRate = null;
			if (rateTemp.Count() > 0)
				averageRate = rateTemp.First();

			if (averageRate.HasValue)
				calculatedAmount = (decimal)((double)amount * averageRate.Value);
			else
				currencyTo = currencyFrom;

			return calculatedAmount;
		}
    }
}