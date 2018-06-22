using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Mediachase.Commerce.Profile.Providers
{
    /// <summary>
    /// Extended Sql Membership provider that allows dynamically changing the current application name instead of using static.
    /// </summary>
    public class CustomerSqlMembershipProvider : SqlMembershipProvider
    {
        private string _ApplicationName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the application to store and retrieve membership information for.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the application to store and retrieve membership information for. The default is the <see cref="P:System.Web.HttpRequest.ApplicationPath"/> property value for the current <see cref="P:System.Web.HttpContext.Request"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// An attempt was made to set the <see cref="P:System.Web.Security.SqlMembershipProvider.ApplicationName"/> property to an empty string or null.
        /// </exception>
        /// <exception cref="T:System.Configuration.Provider.ProviderException">
        /// An attempt was made to set the <see cref="P:System.Web.Security.SqlMembershipProvider.ApplicationName"/> property to a string that is longer than 256 characters.
        /// </exception>
        public override string ApplicationName
        {
            get
            {
                if(!String.IsNullOrEmpty(_ApplicationName))
                    return _ApplicationName;

                return base.ApplicationName;
            }
            set
            {
                _ApplicationName = value;
            }
        }
    }
}