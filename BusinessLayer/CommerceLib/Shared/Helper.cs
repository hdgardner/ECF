using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements helper methods for the CommerceLib project.
    /// </summary>
    public static class CommerceHelper
    {
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="sep">The sep.</param>
        /// <returns></returns>
        public static string ConvertToString(StringCollection col, string sep)
        {
            if (col == null || col.Count == 0)
                return String.Empty;

            string returnString = String.Empty;
            foreach (string item in col)
            {
                if (String.IsNullOrEmpty(returnString))
                    returnString = item;
                else
                    returnString = returnString + sep + item;
            }

            return returnString;
        }

        /// <summary>
        /// Cleans the URL field.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string CleanUrlField(string input)
        {
            // step 1: remove leading spaces
            input = input.Trim();

            // step 2: replace one or more decimals with a dash
            input = Regex.Replace(input, @"\.+", "");

            // step 3: replace all consecutive spaces with one dash
            input = Regex.Replace(input, @"\s+", "-");

            // step 4: replace all nonalphanumeric characters with " "
            input = Regex.Replace(input, @"[^\w\-]", "");

            // step 5: replace all consecutive dashes with one dash
            input = Regex.Replace(input, @"\-+", "-");

            return input;
        }

		#region GetAbsolutePath

        /// <summary>
        /// Calls <see cref="GetAbsolutePath(string, bool)"/> with includePort = false.
        /// </summary>
        /// <param name="xsPath">The xs path.</param>
        /// <returns></returns>
		public static string GetAbsolutePath(string xsPath)
		{
			return GetAbsolutePath(xsPath, false);
		}

        /// <summary>
        /// Returns absolute path for the specified relative url.
        /// </summary>
        /// <param name="xsPath">The xs path.</param>
        /// <param name="includePort">If true, server port value will be included in the resulting Url; otherwise it will be included only if it was specified in Request.</param>
        /// <returns></returns>
		public static string GetAbsolutePath(string xsPath, bool includePort)
		{
			if (!String.IsNullOrEmpty(xsPath) && xsPath.StartsWith("~"))
				xsPath = xsPath.Substring(1);

			StringBuilder builder = new StringBuilder();
			builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");
			builder.Append(HttpContext.Current.Request.Url.Host);

			if (includePort)
				builder.Append(":" + HttpContext.Current.Request.Url.Port);
			else
			{
                /* removed because it causes performance issues, what issues? */
				if (Regex.IsMatch(HttpContext.Current.Request.Url.Authority, ":[\\d]+"))
					builder.Append(":" + HttpContext.Current.Request.Url.Port);
			}

			builder.Append(HttpContext.Current.Request.ApplicationPath);
			if (!HttpContext.Current.Request.ApplicationPath.EndsWith("/"))
				builder.Append("/");
			if (!String.IsNullOrEmpty(xsPath))
			{
				if (xsPath[0] == '/')
					xsPath = xsPath.Substring(1, xsPath.Length - 1);
				builder.Append(xsPath);
			}
			return builder.ToString();
		}
		#endregion

		#region GetAbsoluteThemedPath
        /// <summary>
        /// Gets the absolute themed path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns>Absolute path.</returns>
		public static string GetAbsoluteThemedPath(string virtualPath, string themeName)
		{
			return GetAbsolutePath("/App_Themes/" + themeName + virtualPath);
		}
		#endregion

		#region Add/Remove Context methods
        /// <summary>
        /// Adds specified object to the current HttpContext.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
		public static void SetContextParameter(string name, object value)
		{
			HttpContext.Current.Items[name] = value;
		}

        /// <summary>
        /// Returns an object with the given name from the current HttpContext.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public static object GetContextParameter(string name)
		{
			return HttpContext.Current.Items[name];
		}

        /// <summary>
        /// Returns an object with the given name from the current HttpContext.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public static object GetContextParameter(string name, object defaultValue)
		{
			object obj = HttpContext.Current.Items[name];
			return obj != null ? obj : defaultValue;
		}

        /// <summary>
        /// Returns an object with the given name from the current HttpContext.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetContextParameter<T>(string name, T defaultValue)
        {
            object obj = HttpContext.Current.Items[name];
            return obj != null ? (T)obj : defaultValue;
        }

        /// <summary>
        /// Removes the context parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void RemoveContextParameter(string key)
        {
            if (HttpContext.Current.Items.Contains(key))
                HttpContext.Current.Items.Remove(key);
        }
		#endregion


        /// <summary>
        /// Translates size to string. For instance 1.2 KB instead of  1200, or 200 bytes instead of 200.
        /// </summary>
        /// <param name="size">The size in bytes.</param>
        /// <returns>String representation of the size converted to bytes, KB or MB.</returns>
        public static string ByteSizeToStr(long size)
        {
            string sReturn;

            // TODO: localize strings
            if (size < 512)
                sReturn = String.Format("{0} bytes", size);
            else if (size < 1000 * 1024)
                sReturn = String.Format("{0,0:f} KB", size / 1024.0);
            else
                sReturn = String.Format("{0,0:f} MB", size / (1024.0 * 1000));

            return sReturn;
        }

        /// <summary>
        /// Determines whether [has write access] [the specified index dir].
        /// </summary>
        /// <param name="indexDir">The index dir.</param>
        /// <returns>
        /// 	<c>true</c> if [has write access] [the specified index dir]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasWriteAccess(DirectoryInfo indexDir)
        {
            string tempFileName = Path.Combine(indexDir.FullName, Guid.NewGuid().ToString());
            //Yuck! but it is the simplest way
            try
            {
                File.CreateText(tempFileName).Close();
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            try
            {
                File.Delete(tempFileName);
            }
            catch (UnauthorizedAccessException)
            {
                //we may have permissions to create but not delete, ignoring
            }
            return true;
        }
    }
}
