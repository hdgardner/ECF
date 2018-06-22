using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using Mediachase.Web.Console.Config;
using System.Xml;
using System.Web;
using System.IO;

namespace Mediachase.Web.Console
{
	internal class ConfigManager
	{
		internal const string BaseAppsPath = "~/Apps/";

		private object _syncObject = new object();
		private static ConfigManager _Instance = null;

		/// <summary>
		/// Gets the ConfigManager.
		/// </summary>
		public static ConfigManager Current
		{
			get
			{
				if (_Instance == null)
					_Instance = new ConfigManager();

				return _Instance;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigManager"/> class.
        /// </summary>
		ConfigManager()
		{
		}

		private Dictionary<string, bool> _IsInitialized = new Dictionary<string, bool>();
		private Dictionary<string, CacheDependency> _Dependencies = new Dictionary<string, CacheDependency>();
		private Dictionary<string, ModuleConfig> _Configs = new Dictionary<string, ModuleConfig>();

        /// <summary>
        /// Initializes the configs.
        /// </summary>
        /// <returns></returns>
		public List<ModuleConfig> InitializeConfigs()
		{
			// Load configuration if exists here
			if (HttpContext.Current != null)
			{
				string dirPath = HttpContext.Current.Server.MapPath(BaseAppsPath);
				if (Directory.Exists(dirPath))
					foreach (string dir in Directory.GetDirectories(dirPath))
						InitializeConfig(Path.GetFileName(dir));
			}

			return _Configs.Values.ToList<ModuleConfig>();
		}

        /// <summary>
        /// Returns config for the specified App.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		internal ModuleConfig GetConfig(string name)
		{
			if (HasConfigChanged(name))
				InitializeConfig(name);

			if (!_IsInitialized.ContainsKey(name) || !_IsInitialized[name])
				throw new Exception("Configuration section has not been initialized.");

			return _Configs[name];
		}

        /// <summary>
        /// Returns config for the specified App.
        /// </summary>
        /// <param name="name">The name.</param>
		private void InitializeConfig(string name)
		{
			if(HttpContext.Current!=null)
			{
				string filePath = HttpContext.Current.Server.MapPath(String.Concat(BaseAppsPath, name, "/", name, ".config"));

				if (File.Exists(filePath))
				{
					lock (_syncObject)
					{
						// get ModuleConfig object for the current fileName
						ModuleConfig config = InternalGetConfig(filePath, name);

						// if config loaded, add it to the collection
						if (config != null)
						{
							_Configs[name] = config;
							if (!_IsInitialized.ContainsKey(name))
								_IsInitialized.Add(name, true);
							else
								_IsInitialized[name] = true;
						}
					}
				}
			}
		}

        /// <summary>
        /// Initializes module's properties
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		private ModuleConfig InternalGetConfig(string filePath, string name)
		{
			if (_IsInitialized.ContainsKey(name) && !HasConfigChanged(name))
				return _Configs[name];

			_Dependencies[name] = new CacheDependency(filePath, DateTime.Now);
			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);
			XmlNode section = doc.SelectSingleNode("Configuration");

            if (section == null)
                return null;

			ModuleConfig config = new ModuleConfig(name);

			XmlNode node = section.SelectSingleNode("Views");
			if (node == null)
				return null;

			config.Views = ViewManager.CreateViewCollection(config, node);

			XmlNode settingsNode = section.SelectSingleNode("Settings");
			if (settingsNode != null)
				config.Settings = SettingManager.CreateSettingsCollection(settingsNode);

            // Create ACL
            XmlNode aclNode = section.SelectSingleNode("Acl");
            if (aclNode != null)
            {
                config.Acl = new Acl();
                config.Acl.Groups = AclManager.CreateAclCollection(config, aclNode);
            }

			return config;
		}

        /// <summary>
        /// Determines whether [has config changed] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// 	<c>true</c> if [has config changed] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
		private bool HasConfigChanged(string name)
		{
			if (_Dependencies[name] == null || _Dependencies[name].HasChanged)
				return true;

			return false;
		}
	}
}
