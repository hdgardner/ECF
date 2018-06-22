using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Implements operations for and represents the catalog association.
    /// </summary>
    [DataContract]
    public partial class Association
    {
        string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        string _Description;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        EntryAssociations _EntryAssociations;

        /// <summary>
        /// Gets or sets the entry associations.
        /// </summary>
        /// <value>The entry associations.</value>
        public EntryAssociations EntryAssociations
        {
            get { return _EntryAssociations; }
            set { _EntryAssociations = value; }
        }
    }
}