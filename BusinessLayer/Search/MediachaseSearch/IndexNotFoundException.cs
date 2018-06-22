using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    /// <summary>
    /// Exception is generated when no existing indexes are found.
    /// </summary>
    public class IndexNotFoundException : SearchException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexNotFoundException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public IndexNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public IndexNotFoundException(string message)
            : base(message)
        {
        }
    }
}
