using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains collection of <see cref="Entry"/> elements and paging information.
    /// </summary>
    [DataContract]
    public partial struct Entries
    {
        private int _TotalResults;

        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        /// <value>The total results.</value>
        public int TotalResults
        {
            get { return _TotalResults; }
            set { _TotalResults = value; }
        }

        private int _TotalPages;

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages
        {
            get { return _TotalPages; }
            set { _TotalPages = value; }
        }

        /// <summary>Collection of items.</summary>
        private Entry[] _Entry;

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public Entry[] Entry
        {
            get { return _Entry; }
            set { _Entry = value; }
        }


        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Catalog.Objects.Entry"/> with the specified id.
        /// </summary>
        /// <value></value>
        public Entry this[string id]
        {
            get
            {
                if (_Entry != null)
                    foreach (Entry entry in _Entry)
                    {
                        if (entry.ID.ToLower().CompareTo(id.ToLower()) == 0)
                            return entry;
                    }

                return null;
            }
        }

		/// <summary>
		/// Gets the <see cref="Mediachase.Commerce.Catalog.Objects.Entry"/> under the specified index.
		/// </summary>
		/// <value></value>
		public Entry this[int index]
		{
			get
			{
				if (_Entry != null && _Entry.Length > index)
					return _Entry.GetValue(index) as Entry;

				return null;
			}
		}
    }
}
