using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Mediachase.Cms.Controls
{
    /// <summary>
    /// Summary description for AssemblyHelper.
    /// </summary>
    public static class AssemblyUtil
    {
        /// <summary>
        /// Splits the type and assembly.
        /// </summary>
        /// <param name="typeString">The type string.</param>
        /// <returns></returns>
        private static string[] SplitTypeAndAssembly(string typeString)
        {
            List<string> retVal = new List<string>();

            int num = typeString.LastIndexOf(',');
            if (num < 0)
            {
                retVal.Add(typeString);
            }
            else
            {
                int num2 = typeString.LastIndexOf(']');
                if (num2 > num)
                {
                    retVal.AddRange(typeString.Split(','));
                }

                int index = typeString.IndexOf(',', num2 + 1);

                retVal.Add(typeString.Substring(0, index));
                retVal.Add(typeString.Substring(index + 1));
            }

            return retVal.ToArray();

        }

        /// <summary>
        /// Gets the assemblies path.
        /// </summary>
        /// <returns></returns>
        public static string GetAssembliesPath()
        {
            Assembly asm = null;
            asm = Assembly.GetExecutingAssembly();

            if (asm.GlobalAssemblyCache)
                asm = Assembly.GetCallingAssembly();

            string result = asm.CodeBase.Substring(0, asm.CodeBase.LastIndexOf("/") + 1);
            string prefix = "///";
            return result.Substring(result.IndexOf(prefix) + prefix.Length);
        }


        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns></returns>
        public static object LoadObject(string type, Type interfaceType)
        {
            if (interfaceType == null)
                throw new ArgumentNullException("interfaceType");

            return LoadObject(type, interfaceType.FullName);
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object LoadObject(string type)
        {
            return LoadObject(type, string.Empty);
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <returns></returns>
        public static object LoadObject(string type, string interfaceName)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (interfaceName == null)
                throw new ArgumentNullException("interfaceName");

            string[] typeInformation = SplitTypeAndAssembly(type);

            if (typeInformation.Length < 2)
                throw new ArgumentException("Unable to parse type name.  Use 'type,assembly'");

            string assemblyString = typeInformation[1].Trim();
            assemblyString = assemblyString.Replace("Mediachase.Task", "Mediachase.IbnNext");
            if (assemblyString == "testTask")
                assemblyString = "Mediachase.IbnNext.Web";

            Assembly asm = Assembly.Load(assemblyString);
            if (asm == null)
                throw new ArgumentException("Unable to load assembly " + assemblyString);

            string typeString = typeInformation[0].Trim();
            typeString = typeString.Replace("Mediachase.Task", "Mediachase.IbnNext");

            Type networkServerType = asm.GetType(typeString);
            if (networkServerType == null)
                throw new ArgumentException("Unable to load type " + typeString);

            if (string.IsNullOrEmpty(interfaceName))
            {
                Type ifaceType = networkServerType.GetInterface(interfaceName);
                if (!networkServerType.IsClass || ifaceType == null)
                    throw new ArgumentException(typeInformation[0] + " must be a valid class implenting " + interfaceName);
            }

            return Activator.CreateInstance(networkServerType);
        }

        /// <summary>
        /// Loads the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static System.Type LoadType(string type)
        {
            string[] typeInformation = SplitTypeAndAssembly(type);

            if (typeInformation.Length < 2)
                throw new ArgumentException("Unable to parse type name.  Use format 'type, assembly'");

            string assemblyString = typeInformation[1].Trim();
            assemblyString = assemblyString.Replace("Mediachase.Task", "Mediachase.IbnNext");
            if (typeInformation[0].Trim().StartsWith("Mediachase.IbnNext.TimeTracking"))
                assemblyString = "Mediachase.IbnNext.TimeTracking";

            Assembly asm = Assembly.Load(assemblyString);
            if (asm == null)
                throw new ArgumentException("Unable to load assembly " + assemblyString);

            string typeString = typeInformation[0].Trim();
            typeString = typeString.Replace("Mediachase.Task", "Mediachase.IbnNext");

            Type realType = asm.GetType(typeString);
            if (realType == null)
                throw new ArgumentException("Unable to load type " + typeString);

            return realType;
        }

        /// <summary>
        /// Loads the recursive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns></returns>
        public static object[] LoadRecursive(string path, Type interfaceType)
        {
            ArrayList channelList = new ArrayList();

            ArrayList files = new ArrayList();

            files.AddRange(Directory.GetFiles(path, "*.exe"));
            files.AddRange(Directory.GetFiles(path, "*.dll"));

            foreach (string asmName in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(Path.Combine(path, asmName));

                    if (asm != null)
                    {
                        Type[] types = asm.GetTypes();

                        foreach (Type type in types)
                        {
                            if (type.IsSubclassOf(interfaceType))
                            {
                                channelList.Add(AssemblyUtil.LoadObject(string.Format("{0}, {1}", asm.FullName, type.FullName), interfaceType));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }

            return channelList.ToArray();
        }

        /// <summary>
        /// Loads the recursive.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <returns></returns>
        public static object[] LoadRecursive(string path, Type interfaceType, bool deep)
        {
            ArrayList channelList = new ArrayList();

            channelList.AddRange(LoadRecursive(path, interfaceType));

            if (deep)
            {
                foreach (string DirItem in Directory.GetDirectories(path))
                {
                    channelList.AddRange(LoadRecursive(Path.Combine(path, DirItem), interfaceType));
                }
            }

            return channelList.ToArray();
        }

        /// <summary>
        /// Gets the value from string.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object GetValueFromString(Type fieldType, string value)
        {
            if (fieldType == null)
                throw new ArgumentNullException("fieldType");

            if (fieldType == typeof(string))
            {
                return value;
            }
            else if (fieldType.IsEnum)
            {
                return Enum.Parse(fieldType, value);
            }
            else if (fieldType == typeof(Byte))
            {
                return Byte.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(Int16))
            {
                return Int16.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(Int32))
            {
                return Int32.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(Int64))
            {
                return Int64.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(UInt16))
            {
                return Int16.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(UInt32))
            {
                return Int32.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(UInt64))
            {
                return Int64.Parse(value, NumberStyles.Any);
            }
            else if (fieldType == typeof(DateTime))
            {
                return DateTime.Parse(value);
            }
            else if (fieldType == typeof(Boolean))
            {
                return Boolean.Parse(value);
            }
            else if (fieldType == typeof(Double))
            {
                try
                {
                    return Double.Parse(value, NumberStyles.Any);
                }
                catch
                {
                    return Double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
            }
            else if (fieldType == typeof(Decimal))
            {
                try
                {
                    return Decimal.Parse(value, NumberStyles.Any);
                }
                catch
                {
                    return Decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
            }
            else if (fieldType == typeof(Single))
            {
                try
                {
                    return Single.Parse(value, NumberStyles.Any);
                }
                catch
                {
                    return Single.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
                }
            }
            else if (fieldType == typeof(Guid))
            {
                return new Guid(value);
            }
            else if (fieldType == typeof(string[]))
            {
                return new string[] { value };
            }
            else if (fieldType == typeof(object[]))
            {
                return new object[] { value };
            }

            throw new NotSupportedException(string.Format("Can not convert string to '{0}' type.", fieldType.FullName));
        }

        /// <summary>
        /// Gets the type string.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string GetTypeString(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }
    }
}
