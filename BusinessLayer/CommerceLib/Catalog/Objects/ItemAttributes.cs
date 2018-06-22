using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Contains collection of item attributes with their weights and images.
    /// </summary>
    [DataContract]
    public partial class ItemAttributes
    {
        /// <summary>Collection of attributes</summary>
        private ItemAttribute[] _Attribute;

        /// <summary>
        /// Gets or sets the attribute.
        /// </summary>
        /// <value>The attribute.</value>
        public ItemAttribute[] Attribute
        {
            get { return _Attribute; }
            set { _Attribute = value; }
        }

        /// <summary>Collection of images</summary>
        private Images _Images;

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>The images.</value>
        public Images Images
        {
            get { return _Images; }
            set { _Images = value; }
        }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>The images.</value>
        [Obsolete("This property is obsolete, please use Images property instead.")]
        public Image[] Image
        {
            get 
            {
                if (_Images != null)
                    return _Images.Image;

                return null;
            }
        }

        /// <summary>Collection of files</summary>
        private ItemFiles _Files;

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>The files.</value>
        public ItemFiles Files
        {
            get { return _Files; }
            set { _Files = value; }
        }

        /// <remarks/>
        private NonNegativeIntegerWithUnits _Weight;

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public NonNegativeIntegerWithUnits Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        /// <remarks/>
        private Price _ListPrice;

        /// <summary>
        /// Gets or sets the list price.
        /// </summary>
        /// <value>The list price.</value>
        public Price ListPrice
        {
            get { return _ListPrice; }
            set { _ListPrice = value; }
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Catalog.Objects.ItemAttribute"/> with the specified name.
        /// </summary>
        /// <value></value>
        public ItemAttribute this[string name]
        {
            get
            {
                if (_Attribute != null)
                    foreach (ItemAttribute attr in _Attribute)
                    {
                        if (attr.Name.ToLower().CompareTo(name.ToLower()) == 0)
                            return attr;
                    }

                return new ItemAttribute();
            }
        }

        private decimal _MinQuantity;

        /// <summary>
        /// Gets or sets the min quantity.
        /// </summary>
        /// <value>The min quantity.</value>
        public decimal MinQuantity
        {
            get { return _MinQuantity; }
            set { _MinQuantity = value; }
        }
        private decimal _MaxQuantity;

        /// <summary>
        /// Gets or sets the max quantity.
        /// </summary>
        /// <value>The max quantity.</value>
        public decimal MaxQuantity
        {
            get { return _MaxQuantity; }
            set { _MaxQuantity = value; }
        }

        private DateTime _CreatedDate;

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; }
        }

        private string _CreatedBy;

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy
        {
            get { return _CreatedBy; }
            set { _CreatedBy = value; }
        }

        private string _ModifiedBy;

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>The modified by.</value>
        public string ModifiedBy
        {
            get { return _ModifiedBy; }
            set { _ModifiedBy = value; }
        }

        private DateTime _ModifiedDate;

        /// <summary>
        /// Gets or sets the modified date.
        /// </summary>
        /// <value>The modified date.</value>
        public DateTime ModifiedDate
        {
            get { return _ModifiedDate; }
            set { _ModifiedDate = value; }
        }        
    }
}
