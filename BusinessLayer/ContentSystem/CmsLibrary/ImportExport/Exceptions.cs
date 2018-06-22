using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Cms.ImportExport
{
    public class SiteAlreadyExistsException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SiteAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
