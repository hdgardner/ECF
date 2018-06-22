using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mediachase.Commerce.Orders.Exceptions
{
    /// <summary>
    /// Exception occurs when application fails to contact the gateway.
    /// </summary>
    public class GatewayNotRespondingException : OrderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayNotRespondingException"/> class.
        /// </summary>
        public GatewayNotRespondingException()
            : base("failed to connect to the payment gateway")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayNotRespondingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GatewayNotRespondingException(string message)
            : base(message)
        {
        }
    }
}
