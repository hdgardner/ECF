using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains information about file item.
    /// </summary>
    [DataContract]
    public class ItemFile
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _ContentType;

        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        /// <remarks/>
        private string[] _Value;

        public string[] Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("fileData")]
        public byte[] _FileContents;

        public byte[] FileContents
        {
            get { return _FileContents; }
            set { _FileContents = value; }
        }
    }
}
