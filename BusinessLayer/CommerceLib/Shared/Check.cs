using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements the shared checking methods.
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// Arguments the is not null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="name">The name.</param>
        [DebuggerStepThrough]
        public static void ArgumentIsNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is true.
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        /// <param name="message">The message.</param>
        [DebuggerStepThrough]
        public static void IsTrue(bool argument, string message)
        {
            if (argument == false)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Determines whether [is not true] [the specified argument].
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        /// <param name="message">The message.</param>
        [DebuggerStepThrough]
        public static void IsNotTrue(bool argument, string message)
        {
            if (argument == true)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Determines whether [is not null] [the specified argument].
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="message">The message.</param>
        [DebuggerStepThrough]
        public static void IsNotNull(object argument, string message)
        {
            if (argument == null)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
