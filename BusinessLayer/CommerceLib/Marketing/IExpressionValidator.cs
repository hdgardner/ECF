using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Expression validator interface. Every expression validator must implement this interface.
    /// </summary>
    public interface IExpressionValidator
    {
        /// <summary>
        /// Evals the specified expression against the context passed in the dictionary.
        /// </summary>
        /// <param name="key">The key. Must be a unique key identifying the current expression. It might be used for caching purpose by the engine.</param>
        /// <param name="expr">The expression that needs to be evaluated.</param>
        /// <param name="context">The context, which consists of object that will be accessible during expression evaluation.</param>
        /// <returns></returns>
        ValidationResult Eval(string key, string expr, IDictionary<string, object> context);
    }
}
