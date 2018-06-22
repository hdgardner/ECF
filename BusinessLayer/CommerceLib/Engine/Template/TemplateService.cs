using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Collections;
using System.Configuration;
using System.Globalization;

namespace Mediachase.Commerce.Engine.Template
{
    /// <summary>
    /// Implements operations for and represents the template service.
    /// </summary>
    public class TemplateService
    {
        private static TemplateProvider _provider = null;
        private static TemplateProviderCollection _providers = null;
        private static object _lock = new object();

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public TemplateProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public TemplateProviderCollection Providers
        {
            get { return _providers; }
        }

        /// <summary>
        /// Processes the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string Process(string template, CultureInfo culture, IDictionary context)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.Process(template, culture, context);
        }

        /// <summary>
        /// Loads the providers.
        /// </summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the section
                        TemplateProviderSection section = (TemplateProviderSection)ConfigurationManager.GetSection("FrameworkProviders/templateService");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new TemplateProviderCollection();

                        if (section.Providers == null)
                            throw new ProviderException("Failed to initialize porvider section");

                        try
                        {
                            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(TemplateProvider));
                        }
                        catch (Exception ex)
                        {
                            throw new ProviderException(String.Format("Failed to load the TemplateProvider. Exception: {0}", ex.Message));
                        }

                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException
                                ("Unable to load default TemplateProvider");
                    }
                }
            }
        }
    }


}
