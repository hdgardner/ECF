using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using System.Web.Security;
using Mediachase.Commerce.Catalog.Security;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Impl;
using System.ServiceModel;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Catalog
{
    /// <summary>
    /// Catalog Context class is an entry point for all calls that are made to the catalog system.
    /// This class can be used for either remote or local usage.
    /// </summary>
    public class CatalogContext
    {
        private static volatile ICatalogSystem _Instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static ICatalogSystem Current
        {
            get
            {
                // read configuration file here and depending on it either create local or proxy object instance
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                        {
                            if (!CatalogConfiguration.Instance.Connection.UseWebServices)
                            {
                                _Instance = new CatalogContextImpl(MetaDataContext);
                            }
                            else
                            {
                                //EndpointAddress address = new EndpointAddress(CatalogConfiguration.Instance.Connection.AppDatabase);
                                //WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);
                                _Instance = new CatalogContextProxyImpl(CatalogConfiguration.Instance.Connection.ConnectionStringName);
                            }
                        }
                    }
                }

                return _Instance;
            }
        }

        private static MetaDataContext _mdContext;
        /// <summary>
        /// Gets or sets the meta data context.
        /// </summary>
        /// <value>The meta data context.</value>
        public static MetaDataContext MetaDataContext
        {
            get
            {
                if (_mdContext == null)
                    _mdContext = new MetaDataContext(CatalogConfiguration.Instance.Connection.AppDatabase);

                return _mdContext;
            }
            set
            {
                _mdContext = value;
            }
        }

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetCatalogSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = CatalogDataHelper.CreateDataCommand();
			command.CommandText = "GetCatalogSchemaVersionNumber";
			DataResult result = DataService.LoadDataSet(command);
			if (result.DataSet != null)
			{
				if (result.DataSet.Tables.Count > 0 && result.DataSet.Tables[0].Rows.Count>0)
				{
					DataRow row = result.DataSet.Tables[0].Rows[0];
					major = (int)row["Major"];
					minor = (int)row["Minor"];
					patch = (int)row["Patch"];
					installDate = (DateTime)row["InstallDate"];
				}
			}

			return retval;
		}
    }
}