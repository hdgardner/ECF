using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Implements operations for and represents the validation result.
    /// </summary>
    public class ValidationResult
    {
        private bool _IsValid;
        private string _ValidationError;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                _IsValid = value;
            }
        }

        /// <summary>
        /// Gets or sets the validation error.
        /// </summary>
        /// <value>The validation error.</value>
        public string ValidationError
        {
            get
            {
                return _ValidationError;
            }
            set
            {
                _ValidationError = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="error">The error.</param>
        public ValidationResult(bool isValid, string error)
        {
            _IsValid = isValid;
            _ValidationError = error;
        }
    }
}
