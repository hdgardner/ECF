using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;

namespace Mediachase.Web.Console.Common
{
    public class ReflectionHelper
    {
        #region Reflection Helpers
        protected static ArrayList _TypeList = new ArrayList();
        /// <summary>
        /// Loads the types.
        /// </summary>
        private static void LoadTypes()
        {
            lock (_TypeList)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type[] types = null;
                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        WrapAndThrowTypeLoadException(assembly, ex);
                    }

                    foreach (Type loadedType in types)
                    {
                        _TypeList.Add(loadedType);
                    }
                }
            }
        }

        /// <summary>
        /// Wraps the and throw type load exception.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="ex">The ex.</param>
        private static void WrapAndThrowTypeLoadException(Assembly assembly,ReflectionTypeLoadException ex)
        {
            if (ex.LoaderExceptions.Length > 0)
            {
                throw new ApplicationException(String.Format("Failed to load {0}, {1}.", assembly.FullName, ex.Message), ex.LoaderExceptions[0]);
                /*
                Exception e = ex;
                for (int index = 0; index <= ex.LoaderExceptions.Length - 1; index++)
                {
                    if (ex.LoaderExceptions[index] == null)
                        break;

                    

                    e..InnerException = ex.LoaderExceptions[index];
                    e = e.InnerException;
                }
                 * */
            }

            throw ex;
        }

        /// <summary>
        /// Gets the subclasses.
        /// </summary>
        /// <param name="baseClass">The base class.</param>
        /// <returns></returns>
        public static string[] GetSubclasses(string baseClass)
        {
            ArrayList subclassList = new ArrayList();

            lock (_TypeList)
            {
                // Only load types once
                if (_TypeList.Count == 0)
                {
                    LoadTypes();
                }

                try
                {
                    foreach (Type pluginType in _TypeList)
                    {
                        if (pluginType != null && pluginType.GetInterface(baseClass, true) != null)
                        {
                            subclassList.Add(pluginType.AssemblyQualifiedName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _TypeList = new ArrayList();
                    // Added following line to suppress warning about
                    //  ex not being used
                    ex.ToString();
                    throw;
                }
            }

            return (string[])subclassList.ToArray(typeof(string));
        }

		/// <summary>
		/// Gets the classes that are inherited from the specified class.
		/// </summary>
		/// <param name="baseClass">The base class.</param>
		/// <returns></returns>
		public static Type[] GetInheritedClasses(Type baseType)
		{
			List<Type> classesList = new List<Type>();

			lock (_TypeList)
			{
				// Only load types once
				if (_TypeList.Count == 0)
				{
					LoadTypes();
				}

				try
				{
					foreach (Type type in _TypeList)
					{
						if (type != null && type.IsClass && type.IsSubclassOf(baseType))
							classesList.Add(type);
					}
				}
				catch (Exception ex)
				{
					_TypeList = new ArrayList();
					// Added following line to suppress warning about
					//  ex not being used
					ex.ToString();
					throw;
				}
			}

			return classesList.ToArray();
		}

        /// <summary>
        /// Searches for interfaces in the SitePhysicalPath location
        /// </summary>
        /// <param name="assemblyType">Type of the assembly.</param>
        /// <returns></returns>
		public static string[] GetClassesBasedOnTypeInSiteDir(System.Type assemblyType)
		{
			return GetClassesBasedOnTypeInSiteDir(assemblyType, System.Web.HttpContext.Current.Server.MapPath("~"));
		}

        /// <summary>
        /// Finds all classes that are based on certain type in site home directory
        /// </summary>
        /// <param name="assemblyType">Type of the assembly.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
		public static string[] GetClassesBasedOnTypeInSiteDir(System.Type assemblyType, string path)
		{
			ArrayList arrClasses = new ArrayList();

			if (Directory.Exists(path))
			{
				LocalLoader loader = new LocalLoader(path);
				string[] f = Directory.GetFiles(String.Concat(path, @"\bin"), "*.dll");

				for (int i = 0; i < f.Length; i++)
				{

					try
					{
						//skip McLicenseVerify.dll
						if ((new System.IO.FileInfo(f[i])).Name.StartsWith("McLicenseVerify"))
							continue;

						loader.LoadAssembly(f[i]);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex.ToString());
					}
				}

				string[] classes = loader.GetSubclasses(assemblyType.ToString());
				loader.Unload();
				return classes;
			}

			return (string[])arrClasses.ToArray(typeof(string));
		}


        /// <summary>
        /// Returns the Type object by FullName
        /// </summary>
        /// <param name="typeName">The plugin Type to look up</param>
        /// <returns>The Type object for the specified type name; null if not found</returns>
        private static Type GetTypeByName(string typeName)
        {
            foreach (Type pluginType in _TypeList)
            {
                if (pluginType.FullName == typeName)
                {
                    return pluginType;
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// This property will bind property specified to the object passed.
        /// For property names the expression with "-" seperator can be used similar to
        /// how asp.net control properties can be specified.
        /// 
        /// For example Look-ImageUrl will correspond to the Property ImageUrl for Look object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void BindProperty(object source, string propName, object value)
        {
            PropertyInfo property = source.GetType().GetProperty(propName);
            if (property != null)
            {
                property.SetValue(source, ChangeType(value, property.PropertyType), null);
            }
            else if (propName.Contains("-"))
            {
                //string newkey = key.Replace('-', '.');
                string[] keys = propName.Split(new char[] { '-' });
                object obj = source;

                for (int index = 0; index < keys.Length; index++)
                {
                    string newkey = keys[index];
                    if (property == null)
                    {
                        property = source.GetType().GetProperty(newkey);

                        if (property == null)
                            return;

                        obj = property.GetValue(obj, null);
                    }
                    else
                    {
                        if (index < keys.Length - 1)
                            obj = property.GetValue(obj, null);

                        property = property.PropertyType.GetProperty(newkey);
                    }

                    if (property == null)
                        return;
                }

                if (property != null && obj != null)
                {
                    property.SetValue(obj, ChangeType(value, property.PropertyType), null);
                }
            }
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <returns></returns>
        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsEnum)
                return Enum.Parse(conversionType, value.ToString(), true);
            else if (conversionType == typeof(System.Web.UI.WebControls.Unit))
                return new System.Web.UI.WebControls.Unit(value.ToString());
            else if (conversionType == typeof(System.Type))
                return Type.GetType(value.ToString());
            else if (conversionType == typeof(System.String[]))
                return value.ToString().Split(new char[] { ','});
            else
                return Convert.ChangeType(value, conversionType);
        }
    }
}
