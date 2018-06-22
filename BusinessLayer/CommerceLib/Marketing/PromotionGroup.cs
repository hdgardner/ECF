using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Presents the marketing promotion group.
    /// </summary>
    public static class PromotionGroup
    {
        /// <summary>
        /// The PromotionGroupKey enumeration defines the promotion group keys.
        /// </summary>
		[Flags]
		public enum PromotionGroupKey
		{
            /// <summary>
            /// Represents the order promotion group key.
            /// </summary>
			Order,
            /// <summary>
            /// Represents the entry promotion group key.
            /// </summary>
			Entry,
            /// <summary>
            /// Represents the shipping promotion group key.
            /// </summary>
			Shipping
		}

        /// <summary>
        /// Create a new Groups dictionary.
        /// </summary>
		public static Dictionary<string, string> Groups = new Dictionary<string,string>();

        /// <summary>
        /// Initializes the <see cref="PromotionGroup"/> class.
        /// </summary>
		static PromotionGroup()
		{
			Groups.Add("order", "Order");
			Groups.Add("entry", "Catalog Entry");
			Groups.Add("shipping", "Shipping");
		}

        /// <summary>
        /// Gets the promotion group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static KeyValuePair<string, string> GetPromotionGroup(PromotionGroupKey key)
		{
			if (Groups.ContainsKey(key.ToString().ToLower()))
				return new KeyValuePair<string, string>(key.ToString().ToLower(), Groups[key.ToString().ToLower()]);
			else
				return new KeyValuePair<string, string>(String.Empty, String.Empty);
		}

        /// <summary>
        /// Returns value under specified PromotionGroupKey in Groups collection
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static string GetPromotionGroupValue(PromotionGroupKey key)
		{
			if (Groups.ContainsKey(key.ToString().ToLower()))
				return Groups[key.ToString().ToLower()];
			else
				return null;
		}
    }
}
