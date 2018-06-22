using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Mediachase.Commerce.Shared;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Profile.Search
{
    /// <summary>
    /// Represents the profile search parameters. (Inherits <see cref="SearchParameters"/>.)
    /// </summary>
	[DataContract]
    public sealed class ProfileSearchParameters : SearchParameters
    {
		private StringCollection _organizationNamesCollection;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProfileSearchParameters"/> class.
		/// </summary>
		public ProfileSearchParameters()
		{
			this._organizationNamesCollection = new StringCollection();
		}

		/// <summary>
		/// Gets the organization names.
		/// </summary>
		/// <value>The organization names.</value>
		public StringCollection OrganizationNames
		{
			get 
			{ 
				return this._organizationNamesCollection; 
			}
		}

		/// <summary>
		/// Gets the cache key. Used to generate hash that will be used to store data in memory if needed.
		/// </summary>
		/// <value>The cache key.</value>
		public override string CacheKey
		{
			get
			{
				StringBuilder key = new StringBuilder();
				key.Append("as" + CommerceHelper.ConvertToString(this.OrganizationNames, ","));

				return base.CacheKey + key.ToString();
			}
		}
    }
}
