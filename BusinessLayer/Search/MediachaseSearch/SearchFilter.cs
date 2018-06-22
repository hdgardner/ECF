using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    public interface ISearchFilterValue
    {
        /// <summary>
        /// Gets or sets the descriptions.
        /// </summary>
        /// <value>The descriptions.</value>
        Descriptions Descriptions { get; set; }
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        string key { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SearchFilter
    {

        private Descriptions descriptionsField;

        private SearchFilterValues valuesField;

        private string fieldField;

        /// <remarks/>
        public Descriptions Descriptions
        {
            get
            {
                return this.descriptionsField;
            }
            set
            {
                this.descriptionsField = value;
            }
        }

        /// <remarks/>
        public SearchFilterValues Values
        {
            get
            {
                return this.valuesField;
            }
            set
            {
                this.valuesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string field
        {
            get
            {
                return this.fieldField;
            }
            set
            {
                this.fieldField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Descriptions
    {

        private Description[] descriptionField;

        private string defaultLocaleField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Description")]
        public Description[] Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string defaultLocale
        {
            get
            {
                return this.defaultLocaleField;
            }
            set
            {
                this.defaultLocaleField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class Description
    {

        private string localeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SimpleValue : ISearchFilterValue
    {

        private Descriptions descriptionsField;

        private string keyField;

        private string valueField;

        private bool numericField;

        private string localeField;

        public SimpleValue()
        {
            this.numericField = false;
        }

        /// <remarks/>
        public Descriptions Descriptions
        {
            get
            {
                return this.descriptionsField;
            }
            set
            {
                this.descriptionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool numeric
        {
            get
            {
                return this.numericField;
            }
            set
            {
                this.numericField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PriceRangeValue : ISearchFilterValue
    {

        private Descriptions descriptionsField;

        private string keyField;

        private string localeField;

        private string lowerboundField;

        private string upperboundField;

        private bool lowerboundincludedField;

        private bool upperboundincludedField;

        private string currencyField;

        public PriceRangeValue()
        {
            this.lowerboundincludedField = false;
            this.upperboundincludedField = true;
        }

        /// <remarks/>
        public Descriptions Descriptions
        {
            get
            {
                return this.descriptionsField;
            }
            set
            {
                this.descriptionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string lowerbound
        {
            get
            {
                return this.lowerboundField;
            }
            set
            {
                this.lowerboundField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string upperbound
        {
            get
            {
                return this.upperboundField;
            }
            set
            {
                this.upperboundField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lowerboundincluded
        {
            get
            {
                return this.lowerboundincludedField;
            }
            set
            {
                this.lowerboundincludedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool upperboundincluded
        {
            get
            {
                return this.upperboundincludedField;
            }
            set
            {
                this.upperboundincludedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string currency
        {
            get
            {
                return this.currencyField;
            }
            set
            {
                this.currencyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RangeValue : ISearchFilterValue
    {

        private Descriptions descriptionsField;

        private string keyField;

        private string localeField;

        private string lowerboundField;

        private string upperboundField;

        private bool lowerboundincludedField;

        private bool upperboundincludedField;

        private bool numericField;

        public RangeValue()
        {
            this.lowerboundincludedField = false;
            this.upperboundincludedField = true;
            this.numericField = false;
        }

        /// <remarks/>
        public Descriptions Descriptions
        {
            get
            {
                return this.descriptionsField;
            }
            set
            {
                this.descriptionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key
        {
            get
            {
                return this.keyField;
            }
            set
            {
                this.keyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string locale
        {
            get
            {
                return this.localeField;
            }
            set
            {
                this.localeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string lowerbound
        {
            get
            {
                return this.lowerboundField;
            }
            set
            {
                this.lowerboundField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string upperbound
        {
            get
            {
                return this.upperboundField;
            }
            set
            {
                this.upperboundField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool lowerboundincluded
        {
            get
            {
                return this.lowerboundincludedField;
            }
            set
            {
                this.lowerboundincludedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool upperboundincluded
        {
            get
            {
                return this.upperboundincludedField;
            }
            set
            {
                this.upperboundincludedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool numeric
        {
            get
            {
                return this.numericField;
            }
            set
            {
                this.numericField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class SearchFilterValues
    {

        private RangeValue[] rangeValueField;

        private PriceRangeValue[] priceRangeValueField;

        private SimpleValue[] simpleValueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RangeValue")]
        public RangeValue[] RangeValue
        {
            get
            {
                return this.rangeValueField;
            }
            set
            {
                this.rangeValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PriceRangeValue")]
        public PriceRangeValue[] PriceRangeValue
        {
            get
            {
                return this.priceRangeValueField;
            }
            set
            {
                this.priceRangeValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SimpleValue")]
        public SimpleValue[] SimpleValue
        {
            get
            {
                return this.simpleValueField;
            }
            set
            {
                this.simpleValueField = value;
            }
        }
    }

}
