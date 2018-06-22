using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Navigation;
using Mediachase.Web.Console.Config;
using Mediachase.Web.Console.Common;

namespace Mediachase.Web.Console
{
    public class ManagementContext
    {
		private List<ModuleConfig> _Configs = new List<ModuleConfig>();
		private bool _ConfigsLoaded = false; // will be set to true if all modules were loaded

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>The current.</value>
        public static ManagementContext Current
        {
            get
            {
                object context = HttpContext.Current.Items["ecf-ManagementContext"];
                if (context != null)
                    return (ManagementContext)context;

                ManagementContext ctx = new ManagementContext();
                HttpContext.Current.Items.Add("ecf-ManagementContext", ctx);

                return ctx;
            }
        }

        /// <summary>
        /// Gets or sets the console language.
        /// </summary>
        /// <value>The console language.</value>
        public CultureInfo ConsoleUICulture
        {
            get
			{
				CultureInfo ci = Thread.CurrentThread.CurrentUICulture;

				bool found = false;
				string lang = ProfileContext.Current.Profile != null ? ProfileContext.Current.Profile.ConsoleUILanguage : null;
				
				if (!String.IsNullOrEmpty(lang))
				{
					try
					{
						ci = CultureInfo.CreateSpecificCulture(lang);
						found = true;
					}
					catch (ArgumentException)
					{
					}
				}

				if (!found)
				{
					// init language
					if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.UserLanguages.Length > 0)
					        ci = CultureInfo.CreateSpecificCulture(HttpContext.Current.Request.UserLanguages[0]);
				}

				return ci;
			}
            set
			{
				if (value.IsNeutralCulture)
					throw new ApplicationException("Only specific culture may be assigned.");

				Thread.CurrentThread.CurrentCulture = value;
				Thread.CurrentThread.CurrentUICulture = value;

				// save propery to the profile
				if (ProfileContext.Current.Profile != null && value != null)
				{
					ProfileContext.Current.Profile.ConsoleUILanguage = value.Name;
					ProfileContext.Current.Profile.Save();
				}
			}
        }

        NameValueCollection _Parameters = null;
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public NameValueCollection Parameters
        {
            get
            {
                if (_Parameters == null)
                    return HttpContext.Current.Request.QueryString;

                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        /// <summary>
        /// Returns the list of activated modules.
        /// </summary>
        /// <value>The modules.</value>
		public ModuleConfig[] Configs
		{
			get
			{
				lock (_Configs)
				{
					// Load all the custom modules
					if (!_ConfigsLoaded)
					{
						_Configs = ConfigManager.Current.InitializeConfigs();

						if (_Configs != null)
							_ConfigsLoaded = true;
					}
				}

				return _Configs.ToArray();
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementContext"/> class.
        /// </summary>
        ManagementContext()
        {
        }

        /// <summary>
        /// Finds the name of the module by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public ModuleConfig FindModuleByName(string name)
		{
			foreach (ModuleConfig module in this.Configs)
				if (String.Compare(module.Name, name, true) == 0)
					return module;

			return null;
		}

        /// <summary>
        /// Finds the view.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="viewId">The view id.</param>
        /// <returns></returns>
		public AdminView FindView(string appId, string viewId)
		{
			foreach (ModuleConfig module in this.Configs)
			{
				if (String.Compare(module.Name, appId, false) == 0)
				{
					foreach (AdminView view in ConfigManager.Current.GetConfig(module.Name).Views)
					{
						if (String.Compare(view.ViewId, viewId, true) == 0)
							return view;
					}
					break;
				}
			}

			return null;
		}
    }
}
