using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Xml.XPath;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.XmlTools;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Web.Console.Common
{
    public class ManagementHelper
    {
        private readonly static string NeedToBindGridKey = "NeedToBindGrid";
        //private readonly static string NeedToDataBindKey = "NeedToDataBind";

        /// <summary>
        /// Gets the console resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetConsoleResource(string name)
        {
            string str = "";
            try
            {
                str = HttpContext.GetGlobalResourceObject("ConsoleResources", name, ManagementContext.Current.ConsoleUICulture).ToString();
            }
            catch
            {
                try
                {
                    object resource = HttpContext.GetGlobalResourceObject("ConsoleResources", name);
                    if (resource == null)
                    {
                        throw new ApplicationException(String.Format("Resource for key {0} was not found.", name));
                    }

                    str = resource.ToString();
                }
                catch (System.Resources.MissingManifestResourceException)
                {
                    throw new ApplicationException(String.Format("Resource for key {0} was not found.", name));
                }
            }
            return str;
        }

        /// <summary>
        /// Selects item in the drop down list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">The val.</param>
        public static void SelectListItem(DropDownList list, object val)
        {
            if (list.Items.Count == 0)
                return;

            ListItem li = list.SelectedItem;
            if (li != null)
                li.Selected = false;

            // select another item
            if (val != null)
            {
                li = list.Items.FindByValue(val.ToString());
                if (li != null)
                    li.Selected = true;
            }
        }

        /// <summary>
        /// Selects item in the drop down list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">The val.</param>
        public static void SelectListItem2(DropDownList list, object val)
        {
            if (list.Items.Count == 0)
                return;

            for (int i = 0; i < list.Items.Count; i++)
            {
                if (String.Compare(list.Items[i].Value, val.ToString(), true) == 0)
                {
                    list.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Selects item in the radio list
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">The val.</param>
        public static void SelectRadioListItem(RadioButtonList list, object val)
        {
            ListItem li = list.SelectedItem;
            if (li != null)
                li.Selected = false;

            // select another item
            if (val != null)
            {
                li = list.Items.FindByValue(val.ToString());
                if (li != null)
                    li.Selected = true;
            }
        }

        /// <summary>
        /// Selects item in the listbox
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">The val.</param>
        /// <param name="clearSelection">if set to <c>true</c> [clear selection].</param>
        public static void SelectListItem(ListBox list, object val, bool clearSelection)
        {
            if (list.Items.Count == 0)
                return;

            if (clearSelection)
            {
                foreach (ListItem item in list.Items)
                {
                    if (item.Selected)
                        item.Selected = false;
                }
            }

            // select another item
            if (val != null)
            {
                ListItem li = list.Items.FindByValue(val.ToString());
                if (li != null)
                    li.Selected = true;
            }
        }

        /// <summary>
        /// The same as SelectListItem, but ignores items' values case. Used for string values only.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">The val.</param>
		public static void SelectListItemIgnoreCase(DropDownList list, string val)
		{
			if (list.Items.Count == 0)
				return;

			// deselect currently selected item
			ListItem liSelected = list.SelectedItem;
			if (liSelected != null)
				liSelected.Selected = false;

			// select another item
			if (val != null)
			{
				foreach (ListItem li in list.Items)
					if (String.Compare(li.Value, val, true) == 0)
					{
						// if item is found, break the cycle
						li.Selected = true;
						break;
					}
			}
		}

        /// <summary>
        /// The same as SelectListItem, but ignores items' values case. Used for string values only.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="val">String value.</param>
        /// <param name="clearSelection">if set to <c>true</c> [clear selection].</param>
		public static void SelectListItemIgnoreCase(ListBox list, string val, bool clearSelection)
		{
			if (list.Items.Count == 0)
				return;

			if (clearSelection)
			{
				foreach (ListItem item in list.Items)
				{
					if (item.Selected)
						item.Selected = false;
				}
			}

			// select another item
			if (val != null)
			{
				foreach(ListItem li in list.Items)
					if (String.Compare(li.Value, val, true) == 0)
					{
						li.Selected = true;

						// stop the cycle if list has single selection mode.
						if(list.SelectionMode != ListSelectionMode.Multiple)
							break;
					}
			}
		}

        /// <summary>
        /// Translates reader to the datatable
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static System.Data.DataTable ConvertToTable(System.Data.IDataReader reader)
        {
            System.Data.DataTable _table = reader.GetSchemaTable();
            System.Data.DataTable _dt = new System.Data.DataTable();
            System.Data.DataColumn _dc;
            System.Data.DataRow _row;
            System.Collections.ArrayList _al = new System.Collections.ArrayList();

            for (int i = 0; i < _table.Rows.Count; i++)
            {

                _dc = new System.Data.DataColumn();

                if (!_dt.Columns.Contains(_table.Rows[i]["ColumnName"].ToString()))
                {

                    _dc.ColumnName = _table.Rows[i]["ColumnName"].ToString();
                    _dc.Unique = Convert.ToBoolean(_table.Rows[i]["IsUnique"]);
                    _dc.AllowDBNull = Convert.ToBoolean(_table.Rows[i]["AllowDBNull"]);
                    _dc.DataType = (System.Type)_table.Rows[i]["DataType"];
                    _dc.ReadOnly = Convert.ToBoolean(_table.Rows[i]["IsReadOnly"]);
                    _al.Add(_dc.ColumnName);
                    _dt.Columns.Add(_dc);

                }

            }

            while (reader.Read())
            {

                _row = _dt.NewRow();

                for (int i = 0; i < _al.Count; i++)
                {

                    _row[((System.String)_al[i])] = reader[(System.String)_al[i]];

                }

                _dt.Rows.Add(_row);

            }

            return _dt;

        }

        /// <summary>
        /// Gets the int from query string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static int GetIntFromQueryString(string name)
        {
			return GetIntFromQueryString(name, 0);
        }


		/// <summary>
		/// Gets the int from query string.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static int GetIntFromQueryString(string name, int defaultValue)
		{
			string queryParam = HttpContext.Current.Request.QueryString[name];
			if (String.IsNullOrEmpty(queryParam))
			{
				queryParam = HttpContext.Current.Request.Form[name];
			}
		
			int retVal;
			if(!Int32.TryParse(queryParam, out retVal))
			{
				retVal = defaultValue;
			}

			return retVal;
		}

        /// <summary>
        /// Gets the GUID from query string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public static Guid GetGuidFromQueryString(string name)
		{
			Guid retval = Guid.Empty;

			string o = HttpContext.Current.Request.QueryString[name];

			if (String.IsNullOrEmpty(o))
				o = HttpContext.Current.Request.Form[name];

			if (!String.IsNullOrEmpty(o))
			{
				try
				{
					retval = new Guid(o);
				}
				catch { }
			}
			return retval;
		}

        /// <summary>
        /// Gets the value from query string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public static string GetValueFromQueryString(string name, string defaultValue)
		{
			string val = HttpContext.Current.Request.QueryString[name];

            if (String.IsNullOrEmpty(val))
                val = HttpContext.Current.Request.Form[name];

			return !String.IsNullOrEmpty(val) ? val : defaultValue;
		}

        /// <summary>
        /// Gets the app id from query string.
        /// </summary>
        /// <returns></returns>
		public static string GetAppIdFromQueryString()
		{
			return HttpContext.Current.Request.QueryString["_a"];
		}

        /// <summary>
        /// Gets the view id from query string.
        /// </summary>
        /// <returns></returns>
		public static string GetViewIdFromQueryString()
		{
			return HttpContext.Current.Request.QueryString["_v"];
		}

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <param name="userGuid">The user GUID.</param>
        /// <returns></returns>
        public static string GetUserName(Guid userGuid)
        {
            MembershipUser user = Membership.GetUser(userGuid);
            if (user == null)
                return userGuid.ToString();
            else
                return user.UserName;
        }

        /// <summary>
        /// Gets the boolean value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        public static bool GetBooleanValue(object obj, bool defaultValue)
        {
            if (obj == null)
                return defaultValue;

            bool val = defaultValue;
            if (Boolean.TryParse(obj.ToString(), out val))
            {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string GetStringValue(object obj, string defaultValue)
        {
            if (obj == null)
                return defaultValue;

            return obj.ToString();
        }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int GetIntegerValue(object obj, int defaultValue)
        {
            if (obj == null)
                return defaultValue;

            int val = defaultValue;
            if (Int32.TryParse(obj.ToString(), out val))
            {
                return val;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the flag icon.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
		public static string GetFlagIcon(CultureInfo culture)
		{
			string baseFlagsPath = "~/App_Themes/Default/Images/Flags/";

			if (culture != null && !String.IsNullOrEmpty(culture.Name) && culture.Name.Length >= 3)
			{
				string extension = culture.Name.Substring(3);

				string path = String.Format("{0}{1}.gif", baseFlagsPath, extension);

				if (File.Exists(HttpContext.Current.Server.MapPath(path)))
					return path;

				extension = culture.TwoLetterISOLanguageName;

				path = String.Format("{0}{1}.gif", baseFlagsPath, extension);
				if (File.Exists(HttpContext.Current.Server.MapPath(path)))
					return path;
			}

			return String.Concat(baseFlagsPath, "-.gif");
		}

        /// <summary>
        /// Collect all the controls of type 'T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
		public static List<T> CollectControls<T>(Control parent) where T : Control
		{
			List<T> list = new List<T>();
			LoopControls(parent, list);
			return list;
		}

        /// <summary>
        /// Loop recursively and collect the controls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentContol">The parent contol.</param>
        /// <param name="list">The list.</param>
		private static void LoopControls<T>(Control parentContol, List<T> list) where T : Control
		{
			foreach (Control ctrl in parentContol.Controls)
			{
				if (ctrl is T)
					list.Add(ctrl as T);
				else
				{
					if (ctrl.Controls.Count > 0)
						LoopControls(ctrl, list);
				}
			}
		}

        /// <summary>
        /// Searches for the control with specified id and type in the given controls collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll">The coll.</param>
        /// <param name="controlId">The control id.</param>
        /// <returns></returns>
		public static T GetControlFromCollection<T>(ControlCollection coll, string controlId) where T: Control
		{
			T retVal = null;
			if (coll != null && coll.Count > 0 && !String.IsNullOrEmpty(controlId))
			{
				foreach (Control ctrl in coll)
				{
					if ((ctrl is T) && String.Compare(ctrl.ID, controlId, false) == 0)
					{
						retVal = (T)ctrl;
						break;
					}
					else
					{
						retVal = GetControlFromCollection<T>(ctrl.Controls, controlId);
						if (retVal != null)
							break;

					}
				}
			}
			return retVal;
		}

        /// <summary>
        /// Returns the parent control of specified type for the given control
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="control">The control.</param>
        /// <returns></returns>
		public static T GetParentControl<T>(Control control) where T : Control
		{
			T retVal = null;
			if (control != null)
			{
				Control parent  = control.Parent;
				while (parent != null)
				{
					if (parent is T)
					{
						retVal = (T)parent;
						break;
					}
					parent = parent.Parent;
				}
			}
			return retVal;
		}

        #region RequireBindGrid
        /// <summary>
        /// Sets the bind grid flag.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        public static void SetBindGridFlag(string gridId)
        {
            string key = String.Concat(NeedToBindGridKey, "_", gridId);
            CommerceHelper.SetContextParameter(key, true);
        }

        /// <summary>
        /// Sets the bind grid flag.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetBindGridFlag(string gridId, bool value)
        {
            string key = String.Concat(NeedToBindGridKey, "_", gridId);
            if (value)
                CommerceHelper.SetContextParameter(key, value);
            else // remove from context
                CommerceHelper.RemoveContextParameter(key);
        }

        /// <summary>
        /// Gets the bind grid flag.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <returns></returns>
        public static bool GetBindGridFlag(string gridId)
        {
            string key = String.Concat(NeedToBindGridKey, "_", gridId);
            return CommerceHelper.GetContextParameter<bool>(key, false);
        }
        #endregion

        /// <summary>
        /// Gets the entry URL.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <param name="classTypeId">The class type id.</param>
        /// <returns></returns>
        public static string GetEntryUrl(int entryId, string classTypeId)
        {
            string url = "~/Apps/Shell/Pages/ContentFrame.aspx?_a=Catalog&_v={0}&catalogentryid="+entryId.ToString();

            if (classTypeId.Equals(EntryType.Product, StringComparison.OrdinalIgnoreCase))
                url = String.Format(url, "Product-Edit");
            else if (classTypeId.Equals(EntryType.Variation, StringComparison.OrdinalIgnoreCase))
                url = String.Format(url, "Variation-Edit");
            else if (classTypeId.Equals(EntryType.Package, StringComparison.OrdinalIgnoreCase))
                url = String.Format(url, "Package-Edit");
            else if (classTypeId.Equals(EntryType.Bundle, StringComparison.OrdinalIgnoreCase))
                url = String.Format(url, "Bundle-Edit");

            return url;
        }

        /// <summary>
        /// Gets the import export folder path.
        /// </summary>
        /// <param name="appFolder">The app folder.</param>
        /// <returns></returns>
        public static string GetImportExportFolderPath(string appFolder)
        {
            string baseExportFolder = ConfigurationManager.AppSettings["ECF.ImportExportFolder"];
            if (String.IsNullOrEmpty(baseExportFolder))
                baseExportFolder = "~/ImportExportRepository/";

            if (!baseExportFolder.EndsWith("/"))
                baseExportFolder += "/";

            string dirVirtual = baseExportFolder + AppContext.Current.ApplicationName + "/" + appFolder + "/";

            if (HttpContext.Current != null)
            {
                string dirAbsolute = HttpContext.Current.Server.MapPath(dirVirtual);

                if (!Directory.Exists(dirAbsolute))
                {
                    Directory.CreateDirectory(dirAbsolute);
                }
            }

            return dirVirtual;
        }

		/// <summary>
		/// Registers commands from xml for the specified app and view. 
		/// This is needed because when there are no items in a grid CommandManager doesn't register commands that are used by grid items.
		/// </summary>
		/// <param name="appId"></param>
		/// <param name="viewId"></param>
		/// <returns></returns>
		public static void RegisterCommandsForView(Page page, string appId, string viewId)
		{
			Selector selector = new Selector("", appId, viewId);

			IXPathNavigable navigable = Mediachase.Ibn.XmlTools.XmlBuilder.GetXml(StructureType.MetaView, selector);
			string path = String.Format(CultureInfo.InvariantCulture, "{0}/Commands", StructureType.MetaView.ToString());
			XPathNavigator commands = navigable.CreateNavigator().SelectSingleNode(path);

			string commandName = String.Empty;
			foreach (XPathNavigator commandItem in commands.SelectChildren(string.Empty, string.Empty))
			{
				if (String.Compare(commandItem.Name, "command", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					commandName = commandItem.GetAttribute("id", string.Empty);
					CommandManager.GetCurrent(page).AddCommand("", appId, viewId, commandName);
				}
			}
		}

        /// <summary>
        /// Returns formatted string back
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime date)
        {
            string formatted = "";
            DateTime userUtcNow = GetUserDateTimeNow();
			DateTime userDate = GetUserDateTime(date);

            // make Today and Yesterday bold
			if ((userDate.DayOfYear == userUtcNow.DayOfYear) && (userDate.Year == userUtcNow.Year))
            {
                formatted = "<b>" + UtilHelper.GetResFileString("{SharedStrings:Today}") + "</b>, ";
				formatted += userDate.ToString("t");
            }
			else if ((userDate.DayOfYear == (userUtcNow.DayOfYear - 1)) && (userDate.Year == userUtcNow.Year))
            {
                formatted = "<b>" + UtilHelper.GetResFileString("{SharedStrings:Yesterday}") + "</b>, ";
				formatted += userDate.ToString("t");
            }
            else
            {
				formatted = userDate.ToString();
            }

            return formatted;
        }

        public static string FormatAgoDateTime(DateTime date)
        {
			DateTime userUtcNow = GetUserDateTime(DateTime.UtcNow);
			DateTime userDate = GetUserDateTime(date);

            // make Today and Yesterday bold
			if ((userDate.DayOfYear == userUtcNow.DayOfYear) && (userDate.Year == userUtcNow.Year))
            {
				int minutes = (userUtcNow - userDate).Minutes;
				int hours = (userUtcNow - userDate).Hours;
				if (userDate > userUtcNow.AddHours(-1))
                {
                    if (minutes > 2)
                        return UtilHelper.GetResFileString("{SharedStrings:Minute_Ago}");

                    return String.Format(UtilHelper.GetResFileString("{SharedStrings:Minutes_Ago}"), minutes);
                }
                else
                {
                    if (hours < 2)
                    {
                        return String.Format(UtilHelper.GetResFileString("{SharedStrings:Hour_Ago}"), hours, minutes);
                    }

                    return String.Format(UtilHelper.GetResFileString("{SharedStrings:Hours_Ago}"), hours, minutes);
                }
            }
			else if ((userDate.DayOfYear == (userUtcNow.DayOfYear - 1)) && (userDate.Year == userUtcNow.Year))
            {
				int days = (userUtcNow - userDate).Days;
				int hours = (userUtcNow - userDate).Hours;
                return String.Format(UtilHelper.GetResFileString("{SharedStrings:Day_Ago}"), days, hours);
            }
            else
            {
				int days = (userUtcNow - userDate).Days;

                if (days < 5)
                {
					int hours = (userUtcNow - userDate).Hours;
                    return String.Format(UtilHelper.GetResFileString("{SharedStrings:Days_Ago}"), days, hours);
                }

                return String.Empty;
            }
        }

		/// <summary>
		/// Returns datetime value converted based on current user's settings.
		/// </summary>
		/// <param name="dt">DateTime in UTC.</param>
		/// <returns></returns>
		public static DateTime GetUserDateTime(DateTime dt)
		{
			// TODO: need to store time zone settings for each user and display datetime based on these settings.
			return dt.ToLocalTime();
		}

		/// <summary>
		/// Returns current datetime value converted based on current user's settings.
		/// </summary>
		/// <param name="dt">DateTime in UTC.</param>
		/// <returns></returns>
		public static DateTime GetUserDateTimeNow()
		{
			return GetUserDateTime(DateTime.UtcNow);
		}

		/// <summary>
		/// Includes browser-specific css files.
		/// </summary>
		/// <param name="page"></param>
		public static void RegisterBrowserStyles(Page page)
		{
			string browser = HttpContext.Current.Request.Browser.Browser;

			bool pathExists = false;

			// check if folder for the browser exists
			string url = String.Format("~/App_Themes/{0}", browser);
			string path = HttpContext.Current.Server.MapPath(url);
			if (!Directory.Exists(path))
			{
				// check folder for the specific browser version
				browser = HttpContext.Current.Request.Browser.Type;
				url = String.Format("~/App_Themes/{0}", browser);
				path = HttpContext.Current.Server.MapPath(url);
				pathExists = Directory.Exists(path);
			}
			else
				pathExists = true;
			
			if(pathExists)
			{
				foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
					page.ClientScript.RegisterClientScriptBlock(page.GetType(), browser + file.Name,
						String.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", page.ResolveClientUrl(String.Format("{0}/{1}", url, file.Name))));
			}
		}

		/// <summary>
		/// Includes Ext JS css files.
		/// </summary>
		/// <param name="page"></param>
		public static void RegisterExtJsStyles(Page page)
		{
			if (page != null)
				page.ClientScript.RegisterClientScriptBlock(page.GetType(), "extCss",
						String.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", page.ResolveClientUrl("~/App_Themes/Default/css/ext-all.css")));
		}
    }
}