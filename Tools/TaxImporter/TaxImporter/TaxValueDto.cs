using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class TaxValueDto
    {
        private int _TaxValueId;
        private float _Percentage;
        private int _TaxId;
        private string _TaxCategory;
        private int _JurisdictionGroupId;
        private System.Guid _SiteId;
        private DateTime _AffectiveDate;

        public int TaxValueId
        {
            get
            {
                return _TaxValueId;
            }
            set
            {
                _TaxValueId = value;
            }
        }

        public float Percentage
        {
            get
            {
                return _Percentage;
            }
            set
            {
                _Percentage = value;
            }
        }

        public int TaxId
        {
            get
            {
                return _TaxId;
            }
            set
            {
                _TaxId = value;
            }
        }

        public string TaxCategory
        {
            get
            {
                return _TaxCategory;
            }
            set
            {
                _TaxCategory = value;
            }
        }

        public int JurisdictionGroupId
        {
            get
            {
                return _JurisdictionGroupId;
            }
            set
            {
                _JurisdictionGroupId = value;
            }
        }

        public System.Guid SiteId
        {
            get
            {
                return _SiteId;
            }
            set
            {
                _SiteId = value;
            }
        }

        public DateTime AffectiveDate
        {
            get
            {
                return _AffectiveDate;
            }
            set
            {
                _AffectiveDate = value;
            }
        }
    }
}
