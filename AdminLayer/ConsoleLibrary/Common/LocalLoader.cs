using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Mediachase.Web.Console.Common
{
	/// <summary>
	/// Local loader class
	/// </summary>
	public class LocalLoader : MarshalByRefObject
	{
		AppDomain appDomain;
		RemoteLoader remoteLoader;

        /// <summary>
        /// Creates the local loader class
        /// </summary>
        /// <param name="pluginDirectory">The plugin directory</param>
		public LocalLoader(string pluginDirectory)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationName = String.Format("Plugins-{0}", Guid.NewGuid().ToString());
			setup.ApplicationBase = pluginDirectory;
			setup.PrivateBinPath = "bin";
			//setup.PrivateBinPathProbe = "bin";
			//setup.CachePath = AppDomain.CurrentDomain;//Path.Combine(pluginDirectory, "Plugins/cache" + Path.DirectorySeparatorChar);
			//setup.ShadowCopyFiles = "true";
			//setup.ShadowCopyDirectories = pluginDirectory;

			appDomain = AppDomain.CreateDomain("Plugins", null, setup);
			
			remoteLoader = (RemoteLoader)appDomain.CreateInstanceAndUnwrap(
				Assembly.GetExecutingAssembly().FullName,
				"Mediachase.Web.Console.Common.RemoteLoader");
		}

        /// <summary>
        /// Loads the specified assembly
        /// </summary>
        /// <param name="filename">The filename of the assembly to load</param>
		public void LoadAssembly(string filename)
		{
			remoteLoader.LoadAssembly(filename);
		}

        /// <summary>
        /// Unloads the plugins
        /// </summary>
		public void Unload()
		{
			AppDomain.Unload(appDomain);
			appDomain = null;
		}

        /// <summary>
        /// The list of loaded plugin assemblies
        /// </summary>
        /// <value>The assemblies.</value>
		public string[] Assemblies
		{
			get
			{
				return remoteLoader.GetAssemblies();
			}
		}

        /// <summary>
        /// The list of loaded plugin types
        /// </summary>
        /// <value>The types.</value>
		public string[] Types
		{
			get
			{
				return remoteLoader.GetTypes();
			}
		}

        /// <summary>
        /// Retrieves the type objects for all subclasses of the given type within the loaded plugins.
        /// </summary>
        /// <param name="baseClass">The base class</param>
        /// <returns>All subclases</returns>
		public string[] GetSubclasses(string baseClass)
		{
			return remoteLoader.GetSubclasses(baseClass);
		}

		/// <summary>
		/// Determines if this loader manages the specified type
		/// </summary>
		/// <param name="typeName">The type to check if this PluginManager handles</param>
		/// <returns>True if this PluginManager handles the type</returns>
		public bool ManagesType(string typeName)
		{
			return remoteLoader.ManagesType(typeName);
		}

        /// <summary>
        /// Returns the value of a static property
        /// </summary>
        /// <param name="typeName">The type to retrieve the static property value from</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <returns>The value of the static property</returns>
		public object GetStaticPropertyValue(string typeName, string propertyName)
		{
			return remoteLoader.GetStaticPropertyValue(typeName, propertyName);
		}

        /// <summary>
        /// Returns the result of a static method call
        /// </summary>
        /// <param name="typeName">The type to call the static method on</param>
        /// <param name="methodName">The name of the method to call</param>
        /// <param name="methodParams">The parameters to pass to the method</param>
        /// <returns>The return value of the method</returns>
		public object CallStaticMethod(string typeName, string methodName, object[] methodParams)
		{
			return remoteLoader.CallStaticMethod(typeName, methodName, methodParams);
		}

        /// <summary>
        /// Returns a proxy to an instance of the specified plugin type
        /// </summary>
        /// <param name="typeName">The name of the type to create an instance of</param>
        /// <param name="bindingFlags">The binding flags for the constructor</param>
        /// <param name="constructorParams">The parameters to pass to the constructor</param>
        /// <returns>The constructed object</returns>
		public MarshalByRefObject CreateInstance(string typeName, BindingFlags bindingFlags,
			object[] constructorParams)
		{
			return remoteLoader.CreateInstance(typeName, bindingFlags, constructorParams);
		}
	}
}
