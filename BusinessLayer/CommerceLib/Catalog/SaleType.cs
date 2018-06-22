using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog
{
    /// <summary>
    /// Represents the catalog sale types.
    /// </summary>
	public static class SaleType
	{
        /// <summary>
        /// The TypeKey enumeration defines the sale type keys.
        /// </summary>
		[Flags]
		public enum TypeKey
		{
            /// <summary>
            /// Represents all customers.
            /// </summary>
			AllCustomers,
            /// <summary>
            /// Represents the customer.
            /// </summary>
			Customer,
            /// <summary>
            /// Represents the customer price group.
            /// </summary>
			CustomerPriceGroup
		}

        /// <summary>
        /// Gets the key by value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetKey(int value)
        {
            foreach (SalePriceTypeDefinition element in CatalogConfiguration.Instance.SalePriceTypes)
            {
                if (element.Value == value)
                    return element.Key;
            }

            return String.Empty;
        }
        /*

        /// <summary>
        /// Create a new sale types dictionary.
        /// </summary>
		public static Dictionary<int, string> SaleTypes = new Dictionary<int, string>();

        /// <summary>
        /// Initializes the <see cref="SaleType"/> class.
        /// </summary>
		static SaleType()
		{
			SaleTypes.Add(TypeKey.AllCustomers.GetHashCode(), "All Customers");
			SaleTypes.Add(TypeKey.Customer.GetHashCode(), "Customer");
			SaleTypes.Add(TypeKey.CustomerPriceGroup.GetHashCode(), "Customer Price Group");
		}

        /// <summary>
        /// Gets the type of the sale.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static KeyValuePair<int, string> GetSaleType(Type key)
		{
			if (SaleTypes.ContainsKey(key.GetHashCode()))
				return new KeyValuePair<int, string>(key.GetHashCode(), SaleTypes[key.GetHashCode()]);
			else
				return new KeyValuePair<int, string>(0, String.Empty);
		}

        /// <summary>
        /// Returns value under specified TypeKey in SaleTypes collection
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static string GetSaleTypeValue(TypeKey key)
		{
			if (SaleTypes.ContainsKey(key.GetHashCode()))
				return SaleTypes[key.GetHashCode()];
			else
				return null;
		}
         * */
	}
}
