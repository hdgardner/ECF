using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxImporter
{
    class TaxLanguageDto
    {
        private int _TaxLanguageId;
        private string _DisplayName;
        private string _LanguageCode;
        private int _TaxId;

        public int TaxLanguageId
        {
            get
            {
                return _TaxLanguageId;
            }
            set
            {
                _TaxLanguageId = value;
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

        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
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

    }
}
