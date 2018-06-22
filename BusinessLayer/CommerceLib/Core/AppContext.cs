using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;
using System.Web.Security;
using System.ServiceModel;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using System.Reflection;
using System.Data;
using Mediachase.Commerce.Core.Data;
using System.Web;

namespace Mediachase.Commerce.Core
{
    /// <summary>
    /// Implements methods for the application context.
    /// </summary>
    public class AppContext
    {
        private readonly static string ContextConstantName = "Mediachase.AppContext";
        private static volatile AppContext _Instance;
        private static readonly object _lockObject = new object();
        Guid _ApplicationId = Guid.Empty;
        string _ApplicationName = String.Empty;

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static AppContext Current
        {
            get
            {
                HttpContext httpContext = HttpContext.Current;

                if (httpContext != null)
                {
                    if (httpContext.Items[ContextConstantName] == null)
                    {
                        lock (_lockObject)
                        {
                            if (httpContext.Items[ContextConstantName] == null)
                            {
                                AppContext appContext = new AppContext();
                                if (!httpContext.Items.Contains(ContextConstantName))
                                    httpContext.Items.Add(ContextConstantName, appContext);
                                else
                                    httpContext.Items[ContextConstantName] = appContext;
                            }
                        }
                    }

                    return (AppContext)httpContext.Items[ContextConstantName];
                }
                else // Use local thread if we are not in web environment
                {
                    if (_Instance == null)
                    {
                        lock (_lockObject)
                        {
                            if (_Instance == null)
                            {
                                _Instance = new AppContext();
                            }
                        }
                    }
                    return _Instance;
                }                
            }
        }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        /// <value>The application id.</value>
        public Guid ApplicationId
        {
            get
            {
                if (_ApplicationId == Guid.Empty && !String.IsNullOrEmpty(CoreConfiguration.Instance.DefaultApplicationName))
                {
                    AppDto dto = AppContext.Current.GetApplicationDto(CoreConfiguration.Instance.DefaultApplicationName);

                    if (dto != null && dto.Application.Count != 0 && dto.Application[0].IsActive)
                    {
                        _ApplicationId = dto.Application[0].ApplicationId;
                        _ApplicationName = dto.Application[0].Name;
                    }
                }

                if (_ApplicationId == Guid.Empty)
                    throw new ApplicationException(String.Format("ApplicationId is empty. Please either set the application id value explicitly or modify the default application name which is now set to \"{0}\".", CoreConfiguration.Instance.DefaultApplicationName));


                return _ApplicationId;
            }
            set
            {
                _ApplicationId = value;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get
            {
                if (String.IsNullOrEmpty(_ApplicationName) && this._ApplicationId != Guid.Empty)
                {
                    AppDto dto = AppContext.Current.GetApplicationDto(this._ApplicationId);

                    if (dto != null && dto.Application.Count != 0 && dto.Application[0].IsActive)
                    {
                        _ApplicationName = dto.Application[0].Name;
                    }
                }
                else if (String.IsNullOrEmpty(_ApplicationName))
                {
                    Guid appId = this.ApplicationId;
                }

                return _ApplicationName;
            }
            set
            {
                _ApplicationName = value; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        AppContext()
        {
        }

        #region Public Methods
        /// <summary>
        /// Gets the applications dto.
        /// </summary>
        /// <returns></returns>
        public AppDto GetApplicationDto()
        {
            return AppManager.GetApplicationDto();
        }

        /// <summary>
        /// Gets the application dto.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <returns></returns>
        public AppDto GetApplicationDto(Guid appId)
        {
            return AppManager.GetApplicationDto(appId);
        }

        /// <summary>
        /// Gets the application dto.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public AppDto GetApplicationDto(string name)
        {
            return AppManager.GetApplicationDto(name);
        }

        /// <summary>
        /// Saves the application.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public void SaveApp(AppDto dto)
        {
            AppManager.SaveApplication(dto);
        }


        /// <summary>
        /// Returns product name.
        /// </summary>
        /// <returns></returns>
        public static string GetProductName()
        {
            object[] obj = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (obj.Length > 0)
            {
                AssemblyProductAttribute ata = obj[0] as AssemblyProductAttribute;
                return ata.Product;
            }
            return "Error retrieving the product name";
        }

        /// <summary>
        /// Returns product version.
        /// </summary>
        /// <returns></returns>
        public static string GetProductVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

		/// <summary>
		/// Returns 0 if no patches were installed.
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <param name="installDate"></param>
		/// <returns></returns>
		public static int GetApplicationSystemVersion(out int major, out int minor, out int patch, out DateTime installDate)
		{
			int retval = 0;

			major = 0;
			minor = 0;
			patch = 0;
			installDate = DateTime.MinValue;

			DataCommand command = CoreDataHelper.CreateDataCommand();
			command.CommandText = "GetApplicationSchemaVersionNumber";
			DataResult result = DataService.LoadDataSet(command);
			if (result.DataSet != null)
			{
				if (result.DataSet.Tables.Count > 0 && result.DataSet.Tables[0].Rows.Count > 0)
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
        #endregion
    }
}