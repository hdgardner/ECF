using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents non-negative integer with units of measurement.
    /// </summary>
    public partial struct NonNegativeIntegerWithUnits
    {
        /// <summary>Units of measurement</summary>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _Units;

        public string Units
        {
            get { return _Units; }
            set { _Units = value; }
        }

        /// <summary>Integer value</summary>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "nonNegativeInteger")]
        public string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }
}
