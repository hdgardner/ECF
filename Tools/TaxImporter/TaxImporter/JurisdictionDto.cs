using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class JurisdictionDto
    {
        private int _JurisdictionId;
        private string _DisplayName;
        private string _StateProvinceCode;
        private string _CountryCode;
        private int _JurisdictionType;
        private string _ZipPostalCodeStart;
        private string _ZipPostalCodeEnd;
        private string _City;
        private string _District;
        private string _County;
        private string _GeoCode;
        private System.Guid _ApplicationId;
        private string _Code;

        public int JurisdictionId
        {
            get
            {
                return _JurisdictionId;
            }
            set
            {
                _JurisdictionId = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                _DisplayName = value;
            }
        }

        public string StateProvinceCode
        {
            get
            {
                return _StateProvinceCode;
            }
            set
            {
                _StateProvinceCode = value;
            }
        }

        public string CountryCode
        {
            get
            {
                return _CountryCode;
            }
            set
            {
                _CountryCode = value;
            }
        }

        public int JurisdictionType
        {
            get
            {
                return _JurisdictionType;
            }
            set
            {
                _JurisdictionType = value;
            }
        }

        public string ZipPostalCodeStart
        {
            get
            {
                return _ZipPostalCodeStart;
            }
            set
            {
                _ZipPostalCodeStart = value;
            }
        }

        public string ZipPostalCodeEnd
        {
            get
            {
                return _ZipPostalCodeEnd;
            }
            set
            {
                _ZipPostalCodeEnd = value;
            }
        }

        public string City
        {
            get
            {
                return _City;
            }
            set
            {
                _City = value;
            }
        }

        public string District
        {
            get
            {
                return _District;
            }
            set
            {
                _District = value;
            }
        }

        public string County
        {
            get
            {
                return _County;
            }
            set
            {
                _County = value;
            }
        }

        public string GeoCode
        {
            get
            {
                return _GeoCode;
            }
            set
            {
                _GeoCode = value;
            }
        }

        public System.Guid ApplicationId
        {
            get
            {
                return _ApplicationId;
            }
            set
            {
                _ApplicationId = value;
            }
        }

        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
            }
        }

    }
}
