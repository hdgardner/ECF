namespace Mediachase.Web.Console.Navigation
{
    using System;
    using System.Globalization;
    using System.Web.Hosting;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Data;
    using System.Configuration.Provider;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal static class SecUtility
    {

        internal const int Infinite = Int32.MaxValue;
        /// <summary>
        /// Gets the default name of the app.
        /// </summary>
        /// <returns></returns>
        internal static string GetDefaultAppName()
        {
            try
            {
                string appName = HostingEnvironment.ApplicationVirtualPath;
                if (String.IsNullOrEmpty(appName))
                {

                    appName = System.Diagnostics.Process.GetCurrentProcess().
                                     MainModule.ModuleName;

                    int indexOfDot = appName.IndexOf('.');
                    if (indexOfDot != -1)
                    {
                        appName = appName.Remove(indexOfDot);
                    }
                }

                if (String.IsNullOrEmpty(appName))
                {
                    return "/";
                }
                else
                {
                    return appName;
                }
            }
            catch
            {
                return "/";
            }
        }

        // We don't trim the param before checking with password parameters
        /// <summary>
        /// Validates the password parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns></returns>
        internal static bool ValidatePasswordParameter(ref string param, int maxSize)
        {
            if (param == null)
            {
                return false;
            }

            if (param.Length < 1)
            {
                return false;
            }

            if (maxSize > 0 && (param.Length > maxSize))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="checkForNull">if set to <c>true</c> [check for null].</param>
        /// <param name="checkIfEmpty">if set to <c>true</c> [check if empty].</param>
        /// <param name="checkForCommas">if set to <c>true</c> [check for commas].</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <returns></returns>
        internal static bool ValidateParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize)
        {
            if (param == null)
            {
                return !checkForNull;
            }

            param = param.Trim();
            if ((checkIfEmpty && param.Length < 1) ||
                 (maxSize > 0 && param.Length > maxSize) ||
                 (checkForCommas && param.Contains(",")))
            {
                return false;
            }

            return true;
        }

        // We don't trim the param before checking with password parameters
        /// <summary>
        /// Checks the password parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="paramName">Name of the param.</param>
        internal static void CheckPasswordParameter(ref string param, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (param.Length < 1)
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_be_empty, paramName), paramName);
            }

            if (maxSize > 0 && param.Length > maxSize)
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_too_long, paramName, maxSize.ToString(CultureInfo.InvariantCulture)), paramName);
            }
        }

        /// <summary>
        /// Checks the parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="checkForNull">if set to <c>true</c> [check for null].</param>
        /// <param name="checkIfEmpty">if set to <c>true</c> [check if empty].</param>
        /// <param name="checkForCommas">if set to <c>true</c> [check for commas].</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="paramName">Name of the param.</param>
        internal static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                if (checkForNull)
                {
                    throw new ArgumentNullException(paramName);
                }

                return;
            }

            param = param.Trim();
            if (checkIfEmpty && param.Length < 1)
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_be_empty, paramName), paramName);
            }

            if (maxSize > 0 && param.Length > maxSize)
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_too_long, paramName, maxSize.ToString(CultureInfo.InvariantCulture)), paramName);
            }

            if (checkForCommas && param.Contains(","))
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_can_not_contain_comma, paramName), paramName);
            }
        }

        /// <summary>
        /// Checks the array parameter.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="checkForNull">if set to <c>true</c> [check for null].</param>
        /// <param name="checkIfEmpty">if set to <c>true</c> [check if empty].</param>
        /// <param name="checkForCommas">if set to <c>true</c> [check for commas].</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="paramName">Name of the param.</param>
        internal static void CheckArrayParameter(ref string[] param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName);
            }

            if (param.Length < 1)
            {
                throw new ArgumentException(SR.GetString(SR.Parameter_array_empty, paramName), paramName);
            }

            Hashtable values = new Hashtable(param.Length);
            for (int i = param.Length - 1; i >= 0; i--)
            {
                SecUtility.CheckParameter(ref param[i], checkForNull, checkIfEmpty, checkForCommas, maxSize,
                    paramName + "[ " + i.ToString(CultureInfo.InvariantCulture) + " ]");
                if (values.Contains(param[i]))
                {
                    throw new ArgumentException(SR.GetString(SR.Parameter_duplicate_array_element, paramName), paramName);
                }
                else
                {
                    values.Add(param[i], param[i]);
                }
            }
        }

        /// <summary>
        /// Gets the boolean value.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        internal static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
        {
            string sValue = config[valueName];
            if (sValue == null)
            {
                return defaultValue;
            }

            bool result;
            if (bool.TryParse(sValue, out result))
            {
                return result;
            }
            else
            {
                throw new ProviderException(SR.GetString(SR.Value_must_be_boolean, valueName));
            }
        }

        /// <summary>
        /// Gets the int value.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="valueName">Name of the value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="zeroAllowed">if set to <c>true</c> [zero allowed].</param>
        /// <param name="maxValueAllowed">The max value allowed.</param>
        /// <returns></returns>
        internal static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
        {
            string sValue = config[valueName];

            if (sValue == null)
            {
                return defaultValue;
            }

            int iValue;
            if (!Int32.TryParse(sValue, out iValue))
            {
                if (zeroAllowed)
                {
                    throw new ProviderException(SR.GetString(SR.Value_must_be_non_negative_integer, valueName));
                }

                throw new ProviderException(SR.GetString(SR.Value_must_be_positive_integer, valueName));
            }

            if (zeroAllowed && iValue < 0)
            {
                throw new ProviderException(SR.GetString(SR.Value_must_be_non_negative_integer, valueName));
            }

            if (!zeroAllowed && iValue <= 0)
            {
                throw new ProviderException(SR.GetString(SR.Value_must_be_positive_integer, valueName));
            }

            if (maxValueAllowed > 0 && iValue > maxValueAllowed)
            {
                throw new ProviderException(SR.GetString(SR.Value_too_big, valueName, maxValueAllowed.ToString(CultureInfo.InvariantCulture)));
            }

            return iValue;
        }

        /// <summary>
        /// Determines whether [is directory separator char] [the specified ch].
        /// </summary>
        /// <param name="ch">The ch.</param>
        /// <returns>
        /// 	<c>true</c> if [is directory separator char] [the specified ch]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDirectorySeparatorChar(char ch)
        {
            return (ch == '\\' || ch == '/');
        }

        /// <summary>
        /// Determines whether [is absolute physical path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is absolute physical path] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsAbsolutePhysicalPath(string path)
        {
            if (path == null || path.Length < 3)
                return false;

            // e.g c:\foo
            if (path[1] == ':' && IsDirectorySeparatorChar(path[2]))
                return true;

            // e.g \\server\share\foo or //server/share/foo
            return IsUncSharePath(path);
        }

        /// <summary>
        /// Determines whether [is unc share path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is unc share path] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsUncSharePath(string path)
        {
            // e.g \\server\share\foo or //server/share/foo
			if (!String.IsNullOrEmpty(path) && path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]))
				return true;
			
            return false;
		}

		#region 2delete
		/*
		internal static void CheckSchemaVersion(ProviderBase provider, SqlConnection connection, string[] features, string version, ref int schemaVersionCheck)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (features == null)
            {
                throw new ArgumentNullException("features");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (schemaVersionCheck == -1)
            {
                throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
            }
            else if (schemaVersionCheck == 0)
            {
                lock (provider)
                {
                    if (schemaVersionCheck == -1)
                    {
                        throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
                    }
                    else if (schemaVersionCheck == 0)
                    {
                        int iStatus = 0;
                        SqlCommand cmd = null;
                        SqlParameter p = null;

                        foreach (string feature in features)
                        {
                            cmd = new SqlCommand("dbo.aspnet_CheckSchemaVersion", connection);

                            cmd.CommandType = CommandType.StoredProcedure;

                            p = new SqlParameter("@Feature", feature);
                            cmd.Parameters.Add(p);

                            p = new SqlParameter("@CompatibleSchemaVersion", version);
                            cmd.Parameters.Add(p);

                            p = new SqlParameter("@ReturnValue", SqlDbType.Int);
                            p.Direction = ParameterDirection.ReturnValue;
                            cmd.Parameters.Add(p);

                            cmd.ExecuteNonQuery();

                            iStatus = ((p.Value != null) ? ((int)p.Value) : -1);
                            if (iStatus != 0)
                            {
                                schemaVersionCheck = -1;

                                throw new ProviderException(SR.GetString(SR.Provider_Schema_Version_Not_Match, provider.ToString(), version));
                            }
                        }

                        schemaVersionCheck = 1;
                    }
                }
            }
		}
		*/
		#endregion

        /// <summary>
        /// Gets the and remove boolean attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="val">if set to <c>true</c> [val].</param>
        /// <returns></returns>
		internal static XmlNode GetAndRemoveBooleanAttribute(XmlNode node, string attrib, ref bool val)
        {
            return GetAndRemoveBooleanAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        // input.Xml cursor must be at a true/false XML attribute
        /// <summary>
        /// Gets the and remove boolean attribute internal.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <param name="val">if set to <c>true</c> [val].</param>
        /// <returns></returns>
        private static XmlNode GetAndRemoveBooleanAttributeInternal(XmlNode node, string attrib, bool fRequired, ref bool val)
        {
            XmlNode a = GetAndRemoveAttribute(node, attrib, fRequired);
            if (a != null)
            {
                if (a.Value == "true")
                {
                    val = true;
                }
                else if (a.Value == "false")
                {
                    val = false;
                }
                else
                {
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Invalid_boolean_attribute, a.Name),
                                    a);
                }
            }

            return a;
        }

        /// <summary>
        /// Gets the and remove attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <returns></returns>
        private static XmlNode GetAndRemoveAttribute(XmlNode node, string attrib, bool fRequired)
        {
            XmlNode a = node.Attributes.RemoveNamedItem(attrib);

            // If the attribute is required and was not present, throw
            if (fRequired && a == null)
            {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Missing_required_attribute, attrib, node.Name),
                    node);
            }

            return a;
        }

        /// <summary>
        /// Gets the and remove non empty string attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        internal static XmlNode GetAndRemoveNonEmptyStringAttribute(XmlNode node, string attrib, ref string val)
        {
            return GetAndRemoveNonEmptyStringAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        /// <summary>
        /// Gets the and remove non empty string attribute internal.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private static XmlNode GetAndRemoveNonEmptyStringAttributeInternal(XmlNode node, string attrib, bool fRequired, ref string val)
        {
            XmlNode a = GetAndRemoveStringAttributeInternal(node, attrib, fRequired, ref val);
            if (a != null && val.Length == 0)
            {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Empty_attribute, attrib),
                    a);
            }

            return a;
        }

        /// <summary>
        /// Gets the and remove string attribute internal.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="fRequired">if set to <c>true</c> [f required].</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private static XmlNode GetAndRemoveStringAttributeInternal(XmlNode node, string attrib, bool fRequired, ref string val)
        {
            XmlNode a = GetAndRemoveAttribute(node, attrib, fRequired);
            if (a != null)
            {
                val = a.Value;
            }

            return a;
        }

        /// <summary>
        /// Checks for unrecognized attributes.
        /// </summary>
        /// <param name="node">The node.</param>
        internal static void CheckForUnrecognizedAttributes(XmlNode node)
        {
            if (node.Attributes.Count != 0)
            {
                throw new ConfigurationErrorsException(
                                SR.GetString(SR.Config_base_unrecognized_attribute, node.Attributes[0].Name),
                                node.Attributes[0]);
            }
        }

        /// <summary>
        /// Checks for non comment child nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        internal static void CheckForNonCommentChildNodes(XmlNode node)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Comment)
                {
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Config_base_no_child_nodes),
                                    childNode);
                }
            }
        }

        /// <summary>
        /// Gets the and remove string attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        internal static XmlNode GetAndRemoveStringAttribute(XmlNode node, string attrib, ref string val)
        {
            return GetAndRemoveStringAttributeInternal(node, attrib, false /*fRequired*/, ref val);
        }

        /// <summary>
        /// Checks the forbidden attribute.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrib">The attrib.</param>
        internal static void CheckForbiddenAttribute(XmlNode node, string attrib)
        {
            XmlAttribute attr = node.Attributes[attrib];
            if (attr != null)
            {
                throw new ConfigurationErrorsException(
                                SR.GetString(SR.Config_base_unrecognized_attribute, attrib),
                                attr);
            }
        }

        // Returns whether the virtual path is relative.  Note that this returns true for
        // app relative paths (e.g. "~/sub/foo.aspx")
        /// <summary>
        /// Determines whether [is relative URL] [the specified virtual path].
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>
        /// 	<c>true</c> if [is relative URL] [the specified virtual path]; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsRelativeUrl(string virtualPath)
        {
            // If it has a protocol, it's not relative
            if (virtualPath.IndexOf(":", StringComparison.Ordinal) != -1)
                return false;

            return !IsRooted(virtualPath);
        }

        /// <summary>
        /// Determines whether the specified basepath is rooted.
        /// </summary>
        /// <param name="basepath">The basepath.</param>
        /// <returns>
        /// 	<c>true</c> if the specified basepath is rooted; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsRooted(String basepath)
        {
            return (String.IsNullOrEmpty(basepath) || basepath[0] == '/' || basepath[0] == '\\');
        }

        /// <summary>
        /// Gets the and remove string attribute.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="val">The val.</param>
        internal static void GetAndRemoveStringAttribute(NameValueCollection config, string attrib, string providerName, ref string val)
        {
            val = config.Get(attrib);
            config.Remove(attrib);
        }

        /// <summary>
        /// Checks the unrecognized attributes.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="providerName">Name of the provider.</param>
        internal static void CheckUnrecognizedAttributes(NameValueCollection config, string providerName)
        {
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ConfigurationErrorsException(
                                    SR.GetString(SR.Unexpected_provider_attribute, attribUnrecognized, providerName));
            }
        }

        /// <summary>
        /// Gets the string from bool.
        /// </summary>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        /// <returns></returns>
        internal static string GetStringFromBool(bool flag)
        {
            return flag ? "true" : "false";
        }
        /// <summary>
        /// Gets the and remove positive or infinite attribute.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="val">The val.</param>
        internal static void GetAndRemovePositiveOrInfiniteAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            GetPositiveOrInfiniteAttribute(config, attrib, providerName, ref val);
            config.Remove(attrib);
        }

        /// <summary>
        /// Gets the positive or infinite attribute.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="val">The val.</param>
        internal static void GetPositiveOrInfiniteAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            string s = config.Get(attrib);
            int t;

            if (s == null)
            {
                return;
            }

            if (s == "Infinite")
            {
                t = Infinite;
            }
            else
            {
                try
                {
                    t = Convert.ToInt32(s, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is FormatException || e is OverflowException)
                    {
                        throw new ConfigurationErrorsException(
                            SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));
                    }
                    else
                    {
                        throw;
                    }

                }

                if (t < 0)
                {
                    throw new ConfigurationErrorsException(
                        SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));

                }
            }

            val = t;
        }

        /// <summary>
        /// Gets the and remove positive attribute.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="val">The val.</param>
        internal static void GetAndRemovePositiveAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            GetPositiveAttribute(config, attrib, providerName, ref val);
            config.Remove(attrib);
        }

        /// <summary>
        /// Gets the positive attribute.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="attrib">The attrib.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="val">The val.</param>
        internal static void GetPositiveAttribute(NameValueCollection config, string attrib, string providerName, ref int val)
        {
            string s = config.Get(attrib);
            int t;

            if (s == null)
            {
                return;
            }

            try
            {
                t = Convert.ToInt32(s, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is FormatException || e is OverflowException)
                {
                    throw new ConfigurationErrorsException(
                        SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));
                }
                else
                {
                    throw;
                }

            }

            if (t < 0)
            {
                throw new ConfigurationErrorsException(
                    SR.GetString(SR.Invalid_provider_positive_attributes, attrib, providerName));

            }

            val = t;
        }
    }
}
