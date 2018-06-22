using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements operations for and represents the search options.
    /// </summary>
    [DataContract]
    public abstract class SearchOptions
    {
        private int _recordsToRetrieve = 20;
        private int _startingRecord = 0;
        private bool _returnTotalCount = true;
        private string _Namespace = String.Empty;
        private StringCollection _Classes = new StringCollection();
        private bool _CacheResults = false;
        private TimeSpan _CacheTimeout = new TimeSpan(0, 0, 30);

        // Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchOptions"/> class.
        /// </summary>
        protected internal SearchOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchOptions"/> class.
        /// </summary>
        /// <param name="copy">The copy.</param>
        protected internal SearchOptions(SearchOptions copy)
        {
            this._recordsToRetrieve = copy._recordsToRetrieve;
            this._startingRecord = copy._startingRecord;
        }

        // Properties
        /// <summary>
        /// Gets or sets the classes.
        /// </summary>
        /// <value>The classes.</value>
        public StringCollection Classes
        {
            get
            {
                if (this._Classes == null)
                {
                    this._Classes = new StringCollection();
                }
                return this._Classes;
            }
            set
            {
                this._Classes.Clear();
                foreach (string val in value)
                {
                    this._Classes.Add(val);
                }
            }
        }

        /// <summary>
        /// Gets or sets the records to retrieve.
        /// </summary>
        /// <value>The records to retrieve.</value>
        public int RecordsToRetrieve
        {
            get
            {
                return this._recordsToRetrieve;
            }
            set
            {
                this._recordsToRetrieve = value;
            }
        }

        /// <summary>
        /// Gets or sets the starting record.
        /// </summary>
        /// <value>The starting record.</value>
        public int StartingRecord
        {
            get
            {
                return this._startingRecord;
            }
            set
            {
                this._startingRecord = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [return total count].
        /// </summary>
        /// <value><c>true</c> if [return total count]; otherwise, <c>false</c>.</value>
        public bool ReturnTotalCount
        {
            get
            {
                return this._returnTotalCount;
            }
            set
            {
                this._returnTotalCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace
        {
            get
            {
                return this._Namespace;
            }
            set
            {
                this._Namespace = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [cache results].
        /// </summary>
        /// <value><c>true</c> if [cache results]; otherwise, <c>false</c>.</value>
        public bool CacheResults
        {
            get
            {
                return this._CacheResults;
            }
            set
            {
                this._CacheResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the cache timeout.
        /// </summary>
        /// <value>The cache timeout.</value>
        public TimeSpan CacheTimeout
        {
            get
            {
                return this._CacheTimeout;
            }
            set
            {
                this._CacheTimeout = value;
            }
        }

        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>The cache key.</value>
        public virtual string CacheKey
        {
            get
            {
                StringBuilder key = new StringBuilder();

                key.Append("rs" + this.RecordsToRetrieve.ToString());
                key.Append("sr" + this.StartingRecord.ToString());
                key.Append("ns" + this.Namespace.ToString());
                key.Append("rc" + this.ReturnTotalCount.ToString());
                key.Append("cl" + CommerceHelper.ConvertToString(this.Classes, ","));

                return key.ToString();
            }
        }        
    }
}
