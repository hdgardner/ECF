using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Configuration;
using System.Collections.Specialized;
using System.Web.Security;

namespace Mediachase.Commerce.Profile.Providers
{
    /// <summary>
    /// Implements operations for and represents the customer profile provider. (Inherits <see cref="SqlProfileProvider"/>.)
    /// </summary>
    public class CustomerSqlProfileProvider : SqlProfileProvider
    {
        /// <summary>
        /// 
        /// </summary>
        protected string _connectionString = "";

        /// <summary>
        /// 
        /// </summary>
        protected int _commandTimeout = 30;

        private string _ApplicationName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the application for which to store and retrieve profile information.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the application for which to store and retrieve profile information. The default is the <see cref="P:System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath"/> value.
        /// </returns>
        /// <exception cref="T:System.Web.HttpException">
        /// An attempt was made to set the <see cref="P:System.Web.Profile.SqlProfileProvider.ApplicationName"/> property by a caller that does not have <see cref="F:System.Web.AspNetHostingPermissionLevel.High"/> ASP.NET hosting permission.
        /// </exception>
        /// <exception cref="T:System.Configuration.Provider.ProviderException">
        /// An attempt was made to set the <see cref="P:System.Web.Profile.SqlProfileProvider.ApplicationName"/> property to a string that is longer than 256 characters.
        /// </exception>
        public override string ApplicationName
        {
            get
            {
                if (!String.IsNullOrEmpty(_ApplicationName))
                    return _ApplicationName;

                return base.ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }

        /// <summary>
        /// Initializes the SQL Server profile provider with the property values specified in the ASP.NET application's configuration file. This method is not intended to be used directly from your code.
        /// </summary>
        /// <param name="name">The name of the <see cref="T:System.Web.Profile.SqlProfileProvider"></see> instance to initialize.</param>
        /// <param name="config">A <see cref="T:System.Collections.Specialized.NameValueCollection"></see> that contains the names and values of configuration options for the profile provider.</param>
        /// <exception cref="T:System.Exception">The applicationName attribute value exceeds 256 characters.</exception>
        /// <exception cref="T:System.ArgumentNullException">config is null.</exception>
        /// <exception cref="T:System.Web.HttpException">The connectionStringName attribute is an empty string ("") or is not specified in the application configuration file for this <see cref="T:System.Web.Profile.SqlProfileProvider"></see> instance.- or - The value of the connection string specified in the connectionStringName attribute value is empty or the specified connectionStringName value does not exist in the application configuration file for this <see cref="T:System.Web.Profile.SqlProfileProvider"></see> instance.- or - The application configuration file for this <see cref="T:System.Web.Profile.SqlProfileProvider"></see> instance contains an unrecognized attribute. </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // We must get these configuration sections first since the base 
            // class will remove them from the configuration (not sure why)
            string configurationStringName = config["connectionStringName"];
            string timeout = config["commandTimeout"];

            // Call the base class to allow all other functions to work that we
            // didn't override.
            base.Initialize(name, config);

            // Get the connection String from the config file
			ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings[configurationStringName];
            if (connStringSettings != null)
            {
                _connectionString = connStringSettings.ConnectionString;
            }

            // Get the timeout value
            if (!string.IsNullOrEmpty(timeout))
            {
                _commandTimeout = Convert.ToInt32(timeout);
            }
        }

        /// <summary>
        /// Retrieves profile property information and values from a SQL Server profile database.
        /// </summary>
        /// <param name="sc">The <see cref="T:System.Configuration.SettingsContext"></see> that contains user profile information.</param>
        /// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection"></see> containing profile information for the properties to be retrieved.</param>
        /// <returns>
        /// A <see cref="T:System.Configuration.SettingsPropertyValueCollection"></see> containing profile property information and values.
        /// </returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sc, SettingsPropertyCollection properties)
        {
            SettingsPropertyValueCollection props = base.GetPropertyValues(sc, properties);

            // Retrieve the data from the Database
            string userName = (string)sc["UserName"];
			if (!String.IsNullOrEmpty(userName))
			{
				MembershipUser user = Membership.GetUser(userName);
				if (user != null)
					PopulateAccountFromDatabase(new Guid(user.ProviderUserKey.ToString()), props);
			}

			if (props["PageSettings"] != null)
				props["PageSettings"].Property.SerializeAs = SettingsSerializeAs.Binary;

            return props;
        }

        /// <summary>
        /// Populates the account property for profile from database.
        /// </summary>
        /// <param name="customerId">The customer id.</param>
        /// <param name="props">The props.</param>
        void PopulateAccountFromDatabase(Guid customerId, SettingsPropertyValueCollection props)
        {
            Account account = ProfileContext.Current.GetAccount(customerId.ToString());
            SettingsPropertyValue propValue = props["Account"];
            if (propValue != null)
            {
                propValue.Property.IsReadOnly = true;
                propValue.Deserialized = true;
                propValue.PropertyValue = account;

                // Following two parameters will prevent the default SqlProfileProvider from saving the property into the database
                propValue.IsDirty = false;
                //propValue.UsingDefaultValue = false;
            }
        }
         
        /* Addresses are part of the account and not part of the profile
        void PopulateAddessesFromDatabase(Guid customerId, SettingsPropertyValueCollection props)
        {
            CustomerAddressCollection addresses = ProfileContext.Current.GetCustomerAddresses(customerId);
            SettingsPropertyValue propValue = props["Addresses"];
            if (propValue != null)
            {
                propValue.Property.IsReadOnly = true;
                propValue.Deserialized = true;
                propValue.PropertyValue = addresses;

                // Following two parameters will prevent the default SqlProfileProvider from saving the property into the database
                propValue.IsDirty = false;
                //propValue.UsingDefaultValue = false;
            }
        }
        * */ 

        /// <summary>
        /// Updates the SQL Server profile database with the specified property values.
        /// </summary>
        /// <param name="sc">The <see cref="T:System.Configuration.SettingsContext"></see> that contains user profile information.</param>
        /// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyValueCollection"></see> containing profile information and values for the properties to be updated.</param>
        public override void SetPropertyValues(SettingsContext sc, SettingsPropertyValueCollection properties)
        {
            SettingsPropertyValue propValue = properties["Account"];
            if (propValue != null && propValue.IsDirty)
            {
                // Sasha: do we need to save the account info here?
                // it seems to cause account creationg when user is anonymous
                // so i will remove the following 3 lines for now
                /*
                Account account = (Account)propValue.PropertyValue;
                if (account!=null)
                    account.AcceptChanges();
                 * */

                // Following two parameters will prevent the default SqlProfileProvider from saving the property into the database
                propValue.IsDirty = false;
            }

            // Remove node so we dont have to save it
            properties.Remove(propValue.Name);

            // save using default provider
            base.SetPropertyValues(sc, properties);

            // Add property back
            properties.Add(propValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerProfileProvider"/> class.
        /// </summary>
        public CustomerSqlProfileProvider()
        {
        }

    }
}
