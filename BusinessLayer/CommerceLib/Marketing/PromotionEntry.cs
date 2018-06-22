using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Implements operations for the promotion entry. (Inherits <see cref="ICloneable"/>.)
    /// </summary>
    public class PromotionEntry : ICloneable
    {
        private Hashtable _Storage = new Hashtable();

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        public Hashtable Storage
        {
            get { return _Storage; }
        }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public decimal Quantity
        {
            get { return (decimal)Storage["Quantity"]; }
            set { Storage["Quantity"] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get
            {
                object val = this.Storage[key];
                if (val != null)
                    return val;

                return String.Empty;
            }
            set
            {
                this.Storage[key] = value;
            }
        }

		/// <summary>
		/// Gets or sets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public object Owner
		{
			get { return Storage["PromotionEntryOwner"]; }
			set { Storage["PromotionEntryOwner"] = value; }
		}

        /// <summary>
        /// Gets or sets the cost per entry.
        /// </summary>
        /// <value>The cost per entry.</value>
        public decimal CostPerEntry
        {
            get { return (decimal)Storage["CostPerEntry"]; }
            set { Storage["CostPerEntry"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the catalog.
        /// </summary>
        /// <value>The name of the catalog.</value>
        public string CatalogName
        {
            get { return (string)Storage["CatalogName"]; }
            set { Storage["CatalogName"] = value; }
        }
        /// <summary>
        /// Gets or sets the catalog node code.
        /// </summary>
        /// <value>The catalog node code.</value>
        public string CatalogNodeCode
        {
            get { return (string)Storage["CatalogNodeCode"]; }
            set { Storage["CatalogNodeCode"] = value; }
        }

        /// <summary>
        /// Gets or sets the catalog entry code.
        /// </summary>
        /// <value>The catalog entry code.</value>
        public string CatalogEntryCode
        {
            get { return (string)Storage["CatalogEntryCode"]; }
            set { Storage["CatalogEntryCode"] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionEntry"/> class.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="catalogNodeCode">The catalog node code.</param>
        /// <param name="catalogEntryCode">The catalog entry code.</param>
        /// <param name="costPerEntry">The cost per entry.</param>
        public PromotionEntry(string catalogName, string catalogNodeCode, string catalogEntryCode, decimal costPerEntry)
        {
            this.CatalogName = CatalogName;
            this.CatalogNodeCode = catalogNodeCode;
            this.CatalogEntryCode = catalogEntryCode;
            this.CostPerEntry = costPerEntry;
            this.Quantity = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionEntry"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PromotionEntry(Hashtable context)
        {
            _Storage = context;
        }

        #region ICloneable Members
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            PromotionEntry entry = new PromotionEntry((Hashtable)this.Storage.Clone());
            return entry;
        }
        #endregion
    }
}
