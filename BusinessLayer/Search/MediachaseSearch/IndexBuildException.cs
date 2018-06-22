﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Search
{
    /// <summary>
    /// Exception is generated when index build error occurs.
    /// </summary>
    public class IndexBuildException : SearchException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuildException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public IndexBuildException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexBuildException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public IndexBuildException(string message)
            : base(message)
        {
        }
    }
}
