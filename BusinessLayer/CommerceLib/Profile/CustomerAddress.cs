using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Customer address.
    /// </summary>
    public class CustomerAddress : ProfileStorageBase
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        /// <value>The address id.</value>
        public int AddressId
        {
            get
            {
                return base.GetInt("AddressId");
            }
			set
			{
				base["AddressId"] = value;
			}
        }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        /// <value>The application id.</value>
        public Guid ApplicationId
        {
            get
            {
                return GetGuid("ApplicationId");
            }
            set
            {
                this["ApplicationId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the principal id.
        /// </summary>
        /// <value>The principal id.</value>
        public Guid PrincipalId
        {
            get
            {
                return base.GetGuid("PrincipalId");
            }
            set
            {
                base["PrincipalId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return base.GetString("Name");
            }
            set
            {
                base["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName
        {
            get
            {
                return base.GetString("FirstName");
            }
            set
            {
                base["FirstName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName
        {
            get
            {
                return base.GetString("LastName");
            }
            set
            {
                base["LastName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        public string Organization
        {
            get
            {
                return base.GetString("Organization");
            }
            set
            {
                base["Organization"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the line1.
        /// </summary>
        /// <value>The line1.</value>
        public string Line1
        {
            get
            {
                return base.GetString("Line1");
            }
            set
            {
                base["Line1"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the line2.
        /// </summary>
        /// <value>The line2.</value>
        public string Line2
        {
            get
            {
                return base.GetString("Line2");
            }
            set
            {
                base["Line2"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        public string City
        {
            get
            {
                return base.GetString("City");
            }
            set
            {
                base["City"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State
        {
            get
            {
                return base.GetString("State");
            }
            set
            {
                base["State"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>The country code.</value>
        public string CountryCode
        {
            get
            {
                return base.GetString("CountryCode");
            }
            set
            {
                base["CountryCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        /// <value>The name of the country.</value>
        public string CountryName
        {
            get
            {
                return base.GetString("CountryName");
            }
            set
            {
                base["CountryName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>The postal code.</value>
        public string PostalCode
        {
            get
            {
                return base.GetString("PostalCode");
            }
            set
            {
                base["PostalCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the region code.
        /// </summary>
        /// <value>The region code.</value>
        public string RegionCode
        {
            get
            {
                return base.GetString("RegionCode");
            }
            set
            {
                base["RegionCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the region.
        /// </summary>
        /// <value>The name of the region.</value>
        public string RegionName
        {
            get
            {
                return base.GetString("RegionName");
            }
            set
            {
                base["RegionName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the daytime phone number.
        /// </summary>
        /// <value>The daytime phone number.</value>
        public string DaytimePhoneNumber
        {
            get
            {
                return base.GetString("DaytimePhoneNumber");
            }
            set
            {
                base["DaytimePhoneNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the evening phone number.
        /// </summary>
        /// <value>The evening phone number.</value>
        public string EveningPhoneNumber
        {
            get
            {
                return base.GetString("EveningPhoneNumber");
            }
            set
            {
                base["EveningPhoneNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fax number.
        /// </summary>
        /// <value>The fax number.</value>
        public string FaxNumber
        {
            get
            {
                return base.GetString("FaxNumber");
            }
            set
            {
                base["FaxNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get
            {
                return base.GetString("Email");
            }
            set
            {
                base["Email"] = value;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddress"/> class.
        /// </summary>
        public CustomerAddress() : base(ProfileContext.Current.CustomerAddressMetaClass)
        {
            this["AddressId"] = 0;
            ApplicationId = Guid.Empty;
            PrincipalId = Guid.Empty;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public override void SetParent(object parent)
        {
            Principal principal = (Principal)parent;
            this.PrincipalId = principal.PrincipalId;
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (ApplicationId == Guid.Empty)
                ApplicationId = ProfileConfiguration.Instance.ApplicationId;

            if (PrincipalId == Guid.Empty)
                PrincipalId = ProfileContext.Current.UserId;

            base.AcceptChanges();
        }
    }
}
