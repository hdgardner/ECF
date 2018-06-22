using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    public class SearchSort
    {
        private string _FieldName;

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        private bool _isDescending = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is descending.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is descending; otherwise, <c>false</c>.
        /// </value>
        public bool IsDescending
        {
            get { return _isDescending; }
            set { _isDescending = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSort"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="isDescending">if set to <c>true</c> [is descending].</param>
        public SearchSort(string fieldName, bool isDescending)
        {
            _FieldName = fieldName;
            _isDescending = isDescending;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSort"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        public SearchSort(string fieldName)
        {
            _FieldName = fieldName;
        }
    }
}
