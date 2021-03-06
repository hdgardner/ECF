using System;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Exceptions
{
    /// <summary>
    /// Implements operations for the catalog exception class. (Inherits <see cref="System.Exception"/>.)
    /// </summary>
	[Serializable]
    public class CatalogException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogException"/> class.
        /// </summary>
        public CatalogException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CatalogException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public CatalogException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected CatalogException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
    }
}
