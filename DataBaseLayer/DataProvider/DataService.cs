using System;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Web.Configuration;

namespace Mediachase.Data.Provider
{
    public class DataService
    {
        private static DataProvider _provider = null;
        private static DataProviderCollection _providers = null;
        private static object _lock = new object();

        /// <summary>
        /// Gets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public DataProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public DataProviderCollection Providers
        {
            get { return _providers; }
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataTable LoadTable(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.LoadTable(command);
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataResult LoadDataSet(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.LoadDataSet(command);
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataResult LoadReader(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.LoadReader(command);
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataTable LoadTableSchema(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.LoadTableSchema(command);
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataResult ExecuteNonExec(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.ExecuteNonExec(command);
        }

        /// <summary>
        /// Loads the table.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataResult ExecuteScalar(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();


            // Delegate to the provider
            return _provider.ExecuteScalar(command);
        }

        /// <summary>
        /// Runs the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        public static void Run(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            _provider.Run(command);
        }

        /// <summary>
        /// Runs the return integer.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static int RunReturnInteger(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.RunReturnInteger(command);
        }

        /// <summary>
        /// Saves the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static DataResult Save(DataCommand command)
        {
            // Make sure a provider is loaded
            LoadProviders();

            // Delegate to the provider
            return _provider.Save(command);
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
                        // Get a reference to the <imageService> section
                        DataProviderSection section = (DataProviderSection)ConfigurationManager.GetSection("FrameworkProviders/dataService");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new DataProviderCollection();

                        if (section.Providers == null)
                            throw new ProviderException("Failed to initialize porvider section");

                        try
                        {
                            ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(DataProvider));
                        }
                        catch (Exception ex)
                        {
                            throw new ProviderException(String.Format("Failed to load the DataProvider. Exception: {0}", ex.Message));
                        }

                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException
                                ("Unable to load default DataProvider");
                    }
                }
            }
        }
    }


}
