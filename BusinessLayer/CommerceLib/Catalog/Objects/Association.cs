using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains relation information about the entry that is a related entry.
    /// </summary>
    [DataContract]
    public partial class RelationInfo
    {
        int _SortOrder;

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder
        {
            get { return _SortOrder; }
            set { _SortOrder = value; }
        }

        decimal _Quantity;

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public decimal Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        string _RelationType;

        /// <summary>
        /// Gets or sets the type of the relation.
        /// </summary>
        /// <value>The type of the relation.</value>
        public string RelationType
        {
            get { return _RelationType; }
            set { _RelationType = value; }
        }

        string _GroupName;

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; }
        }
    }
}