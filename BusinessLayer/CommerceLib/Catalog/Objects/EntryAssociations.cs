using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains the data representing the collection of catalog entry associations (see <see cref="EntryAssociation"/>).
    /// </summary>
    [DataContract]
    public partial class EntryAssociations
    {
        EntryAssociation[] _Association;

        /// <summary>
        /// Gets or sets the association.
        /// </summary>
        /// <value>The association.</value>
        public EntryAssociation[] Association
        {
            get { return _Association; }
            set { _Association = value; }
        }
    }
}
