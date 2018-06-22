using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Mediachase.Commerce.Shared;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Core.Log
{
    /// <summary>
    /// Represents the profile search parameters. (Inherits <see cref="SearchParameters"/>.)
    /// </summary>
	[DataContract]
    public sealed class ApplicationLogParameters : SearchParameters
    {
		private string _source = String.Empty;
        private string _operation = String.Empty;
        private string _objectType = String.Empty;
        private DateTime _created = DateTime.MinValue;

		/// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLogParameters"/> class.
		/// </summary>
        public ApplicationLogParameters()
		{
		}

		/// <summary>
        /// Gets or sets the source key.
		/// </summary>
		/// <value>The source key.</value>
		public String SourceKey
		{
			get 
			{
                return this._source; 
			}
            set
            {
                this._source = value;
            }
		}

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>The operation.</value>
        public String Operation
        {
            get
            {
                return this._operation;
            }
            set
            {
                this._operation = value;
            }
        }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        /// <value>Type of the object.</value>
        public String ObjectType
        {
            get
            {
                return this._objectType;
            }
            set
            {
                this._objectType = value;
            }
        }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>The date created.</value>
        public DateTime Created
        {
            get
            {
                return this._created;
            }
            set
            {
                this._created = value;
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
				key.Append("s" + this.SourceKey);
                key.Append("o" + this.Operation);
                key.Append("t" + this.ObjectType);
                key.Append("c" + this.Created.ToLongDateString());

				return base.CacheKey + key.ToString();
			}
		}
    }
}
