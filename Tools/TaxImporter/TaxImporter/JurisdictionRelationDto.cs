using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class JurisdictionRelationDto
    {
        private int _JurisdictionId;
        private int _JurisdictionGroupId;

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
    }
}
