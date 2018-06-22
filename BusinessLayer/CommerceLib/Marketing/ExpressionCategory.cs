using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Represents the marketing expression categories.
    /// </summary>
    public static class ExpressionCategory
    {
        /// <summary>
        /// The CategoryKey enumeration defines the expression category keys.
        /// </summary>
		[Flags]
		public enum CategoryKey
		{
            /// <summary>
            /// Represents the promotion key.
            /// </summary>
			Promotion,
            /// <summary>
            /// Represents the segment key.
            /// </summary>
			Segment,
            /// <summary>
            /// Represents the policy key.
            /// </summary>
			Policy
		}

        /// <summary>
        /// Create a new categories dictionary.
        /// </summary>
		public static Dictionary<string, string> Categories = new Dictionary<string,string>();

        /// <summary>
        /// Initializes the <see cref="ExpressionCategory"/> class.
        /// </summary>
		static ExpressionCategory()
		{
			Categories.Add("promotion", "Promotion");
			Categories.Add("segment", "Segment");
			Categories.Add("policy", "Policy");
		}

        /// <summary>
        /// Gets the expression category.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static KeyValuePair<string, string> GetExpressionCategory(CategoryKey key)
		{
			if (Categories.ContainsKey(key.ToString().ToLower()))
				return new KeyValuePair<string, string>(key.ToString().ToLower(), Categories[key.ToString().ToLower()]);
			else
				return new KeyValuePair<string, string>(String.Empty, String.Empty);
		}

        /// <summary>
        /// Returns value under specified CategoryKey in Categories collection
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static string GetExpressionCategoryValue(CategoryKey key)
		{
			if (Categories.ContainsKey(key.ToString().ToLower()))
				return Categories[key.ToString().ToLower()];
			else
				return null;
		}

		/*
		public const string Promotion = "promotion";
        public const string Segment = "segment";
        public const string Policy = "policy";*/
    }
}