using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// External Asset
    /// </summary>
    [DataContract]
    public class ItemAsset
    {
        private string _AssetKey;

        /// <summary>
        /// Gets or sets the asset key.
        /// </summary>
        /// <value>The asset key.</value>
        public string AssetKey
        {
            get { return _AssetKey; }
            set { _AssetKey = value; }
        }

        private string _AssetType;

        /// <summary>
        /// Gets or sets the type of the asset.
        /// </summary>
        /// <value>The type of the asset.</value>
        public string AssetType
        {
            get { return _AssetType; }
            set { _AssetType = value; }
        }

        private string _GroupName;

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get { return _GroupName; }
            set { _GroupName = value; }
        }

        private int _SortOrder;

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder
        {
            get { return _SortOrder; }
            set { _SortOrder = value; }
        }
    }
}
