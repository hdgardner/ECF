using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class TaxDto
    {
        private int _TaxId;
        private int _TaxType;
        private string _Name;
        private int _SortOrder;
        private System.Guid _ApplicationId;

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

        public int TaxType
        {
            get
            {
                return _TaxType;
            }
            set
            {
                _TaxType = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public int SortOrder
        {
            get
            {
                return _SortOrder;
            }
            set
            {
                _SortOrder = value;
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
    }
}
