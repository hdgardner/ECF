using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class JurisdictionGroupDto
    {
        private int _JurisdictionGroupId;
        private Guid _ApplicationId;
        private string _DisplayName;
        private int _JurisdictionType;
        private string _Code;

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

        public Guid ApplicationId
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
