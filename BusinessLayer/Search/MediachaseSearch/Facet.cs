using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    public class Facet
    {
        private FacetGroup _Group;

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public FacetGroup Group
        {
            get { return _Group; }
            set { _Group = value; }
        }

        private string _Name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        private int _Count;

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        private string _Key;

        /// <summary>
        /// Gets or sets the Key.
        /// </summary>
        /// <value>The URL.</value>
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
    }
}
