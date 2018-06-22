using System;
using System.Configuration.Provider;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Mediachase.Commerce.Engine.Template
{
    /// <summary>
    /// Implements operations for the template provider. (Inherits <see cref="ProviderBase"/>.)
    /// </summary>
    public abstract class TemplateProvider : ProviderBase
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public abstract string ApplicationName { get; set; }

        /// <summary>
        /// Processes the specified template with a context provided.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The processed template with context embedded.
        /// </returns>
        public abstract string Process(string template, CultureInfo culture, IDictionary context);
    }
}
