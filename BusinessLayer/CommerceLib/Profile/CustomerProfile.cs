using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Runtime.Serialization;
using System.Configuration;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Implements operations for the customer profile. (Inherits <see cref="ProfileBase"/>.)
    /// </summary>
    public partial class CustomerProfile : ProfileBase
    {
		public static readonly string PageSettingsBaseKey = "PageSettings_";
        public static readonly string TemplateSettingsBaseKey = "TemplateSettings_";
		public static readonly string GridSettingsBaseKey = "GridSettings_";
        public static readonly string GridPageSizeBaseKey = "GridPageSize_";

        /// <summary>
        /// Gets or sets the preferred shipping address.
        /// </summary>
        /// <value>The preferred shipping address.</value>
        [SettingsAllowAnonymous(false)]
        //[ProfileProvider("EmployeeInfoProvider")]
        public string PreferredShippingAddress
        {
            get { return base["PreferredShippingAddress"].ToString(); }
            set { base["PreferredShippingAddress"] = value; }
        }

        /// <summary>
        /// Gets or sets the preferred billing address.
        /// </summary>
        /// <value>The preferred billing address.</value>
        [SettingsAllowAnonymous(false)]
        //[ProfileProvider("EmployeeInfoProvider")]
        public string PreferredBillingAddress
        {
            get { return base["PreferredBillingAddress"].ToString(); }
            set { base["PreferredBillingAddress"] = value; }
        }

        /*
        /// <summary>
        /// Gets or sets the last catalog page URL.
        /// </summary>
        /// <value>The last catalog page URL.</value>
        [SettingsAllowAnonymous(true)]
        public string LastCatalogPageUrl
        {
            get { return base["LastCatalogPageUrl"].ToString(); }
            set { base["LastCatalogPageUrl"] = value; }
        }
         * */

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [SettingsAllowAnonymous(true)]
        public string FirstName
        {
            get { return base["FirstName"].ToString(); }
            set { base["FirstName"] = value; }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [SettingsAllowAnonymous(true)]
        public string LastName
        {
            get { return base["LastName"].ToString(); }
            set { base["LastName"] = value; }
        }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        [SettingsAllowAnonymous(true)]
        public string FullName
        {
            get { return base["FullName"].ToString(); }
            set { base["FullName"] = value; }
        }

        /// <summary>
        /// Gets or sets the last visited.
        /// </summary>
        /// <value>The last visited.</value>
        [SettingsAllowAnonymous(true)]
        public DateTime LastVisited
        {
            get { return (DateTime)base["LastVisited"]; }
            set { base["LastVisited"] = value; }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [SettingsAllowAnonymous(true)]
        public string Email
        {
            get { return base["Email"].ToString(); }
            set { base["Email"] = value; }
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        /// <value>The account.</value>
        [SettingsAllowAnonymous(false)]
        public Account Account
        {
            get 
			{
				return (Account)base["Account"];
			}
            set { base["Account"] = value; }
        }

		/// <summary>
		/// Gets or sets the preferred CM UI Language.
		/// </summary>
		/// <value>The preferred language.</value>
		[SettingsAllowAnonymous(true)]
		public string ConsoleUILanguage
		{
			get { return base["ConsoleUILanguage"].ToString(); }
			set { base["ConsoleUILanguage"] = value; }
		}

		/// <summary>
		/// Gets or sets the preferred CM UI Language.
		/// </summary>
		/// <value>The preferred language.</value>
		[SettingsAllowAnonymous(false)]
		//[ConfigurationProperty("serializeAs", DefaultValue = "Binary")]
		public CMPageSettings PageSettings
		{
			get { return (CMPageSettings)base["PageSettings"]; }
			set { base["PageSettings"] = value; }
		}

        /// <summary>
        /// Updates the profile data source with changed profile property values.
        /// </summary>
        public override void Save()
        {
            base.Save();
        }
    }

	[Serializable]
	public class CMPageSettings : ISerializable
	{
		private Hashtable _pageSettings = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Initializes a new instance of the <see cref="CMPageSettings"/> class.
        /// </summary>
		public CMPageSettings()
		{
		}

        /// <summary>
        /// Gets the setting int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public int GetSettingInt(string key, int defaultValue)
		{
			int retVal = defaultValue;
			try
			{
				if (_pageSettings[CustomerProfile.PageSettingsBaseKey + key] != null)
					retVal = Convert.ToInt32(_pageSettings[CustomerProfile.PageSettingsBaseKey + key]);
			}
			catch { }
			return retVal;
		}

        /// <summary>
        /// Sets the setting int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="settings">The settings.</param>
		public void SetSettingInt(string key, int settings)
		{
			_pageSettings[CustomerProfile.PageSettingsBaseKey + key] = settings;
		}

        /// <summary>
        /// Gets the setting string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public string GetSettingString(string key)
		{
			return GetSettingString(key, null);
		}

        /// <summary>
        /// Gets the setting string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
		public string GetSettingString(string key, string defaultValue)
		{
			string retVal = String.Empty;
			try
			{
				retVal = _pageSettings[CustomerProfile.PageSettingsBaseKey + key].ToString();
			}
			catch { }
			return retVal;
		}

        /// <summary>
        /// Sets the setting string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="settings">The settings.</param>
		public void SetSettingString(string key, string settings)
		{
			_pageSettings[CustomerProfile.PageSettingsBaseKey + key] = settings;
		}

        /// <summary>
        /// Makes the grid page size key.
        /// </summary>
        /// <param name="viewId">The view id.</param>
        /// <returns></returns>
		public static string MakeGridPageSizeKey(string viewId)
		{
			return CustomerProfile.GridPageSizeBaseKey + viewId;
		}

		/// <summary>
		/// Makes the grid settings key.
		/// </summary>
		/// <param name="appId">The app id.</param>
		/// <param name="viewId">The view id.</param>
		/// <returns></returns>
		public static string MakeGridSettingsKey(string appId, string viewId)
		{
			return String.Concat(CustomerProfile.GridSettingsBaseKey, appId, "_", viewId);
		}

		#region ISerializable Members
        /// <summary>
		/// Initializes a new instance of the <see cref="CMPageSettings"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
		protected CMPageSettings(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            foreach(SerializationEntry entry in info)
                _pageSettings[entry.Name] = entry.Value;
        }

        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            foreach (string key in _pageSettings.Keys)
                info.AddValue(key, _pageSettings[key]);
        }
        #endregion
	}
}