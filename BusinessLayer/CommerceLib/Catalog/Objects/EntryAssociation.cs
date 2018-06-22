using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains the data representing the catalog entry association.
    /// </summary>
    [DataContract]
    public partial struct EntryAssociation
    {
        int _SortOrder;
        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                _SortOrder = value;
            }
        }

        string _AssociationType;

        /// <summary>
        /// Gets or sets the type of the association.
        /// </summary>
        /// <value>The type of the association.</value>
        public string AssociationType
        {
            get { return _AssociationType; }
            set { _AssociationType = value; }
        }
        string _AssociationDesc;

        /// <summary>
        /// Gets or sets the association desc.
        /// </summary>
        /// <value>The association desc.</value>
        public string AssociationDesc
        {
            get { return _AssociationDesc; }
            set { _AssociationDesc = value; }
        }
        Entry _Entry;

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        /// <value>The entry.</value>
        public Entry Entry
        {
            get { return _Entry; }
            set { _Entry = value; }
        }
    }
}
