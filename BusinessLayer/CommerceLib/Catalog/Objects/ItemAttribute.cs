using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains information about item's attribute.
    /// </summary>
    [DataContract]
    public partial class ItemAttribute
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _FriendlyName;

        public string FriendlyName
        {
            get { return _FriendlyName; }
            set { _FriendlyName = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _Type;

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        /// <remarks/>
        private string[] _Value;

        public string[] Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return (Value == null || Value.Length == 0) ? String.Empty : Value[0];
        }             
    }
}
