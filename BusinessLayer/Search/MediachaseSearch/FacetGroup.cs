using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    public class FacetGroup
    {
        private string _FieldName;

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
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

        private List<Facet> _Facets = new List<Facet>();

        /// <summary>
        /// Gets or sets the facets.
        /// </summary>
        /// <value>The facets.</value>
        public List<Facet> Facets
        {
            get { return _Facets; }
            set { _Facets = value; }
        }
    }
}
