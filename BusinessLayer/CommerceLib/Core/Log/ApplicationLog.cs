using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using System.Data;
using Mediachase.Commerce.Shared;
using Mediachase.Commerce.Core.Data;

namespace Mediachase.Commerce.Core.Log
{
    /// <summary>
    /// Implements operations for the profile search.
    /// </summary>
    public class ApplicationLog
    {
        private ApplicationLogOptions _applicationLogOptions = null;
        private ApplicationLogParameters _applicationLogParameters = null;

        /// <summary>
        /// Gets or sets the search options.
        /// </summary>
        /// <value>The search options.</value>
        public ApplicationLogOptions LogOptions
        {
            get
            {
                return this._applicationLogOptions;
            }
            set
            {
                this._applicationLogOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        public ApplicationLogParameters LogParameters
        {
            get 
            { 
                return _applicationLogParameters; 
            }
            set 
            { 
                _applicationLogParameters = value; 
            }
        }

    }
}
