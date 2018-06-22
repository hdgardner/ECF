using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;
using System.Runtime.Serialization;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Order address.
    /// </summary>
	[Serializable]
    public class OrderAddress : OrderStorageBase
    {
        private OrderGroup _Parent;

        /// <summary>
        /// Gets the parent Order group.
        /// </summary>
        /// <value>The parent.</value>
        public OrderGroup Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAddress"/> class.
        /// </summary>
        public OrderAddress() : base(OrderContext.Current.OrderAddressMetaClass)
        {
            base["OrderGroupAddressId"] = 0;
        }

        #region Public Properties
        /// <summary>
        /// Gets the order group address id.
        /// </summary>
        /// <value>The order group address id.</value>
        public int OrderGroupAddressId
        {
            get
            {
                return base.GetInt("OrderGroupAddressId");
            }
        }

        /// <summary>
        /// Gets or sets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                return base.GetInt("OrderGroupId");
            }
            set
            {
                base["OrderGroupId"] = value;
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
        /// <value>The name of the first.</value>
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
        /// Gets or sets last name.
        /// </summary>
        /// <value>The name of the last.</value>
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
        /// Sets the parent.
        /// </summary>
        /// <param name="Parent">The parent.</param>
        public override void SetParent(object Parent)
        {
            _Parent = (OrderGroup)Parent;
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (_Parent == null)
                throw new NoNullAllowedException("Parent");

            if (_Parent.ObjectState == MetaObjectState.Added)
            {
                throw new MetaException("Must save parent object");
            }

			int oldId = 0;
			int newId = 0;
			bool deleted = true;

			if (ObjectState != MetaObjectState.Deleted)
			{
				this.OrderGroupId = _Parent.OrderGroupId;
				deleted = false;
			}

			oldId = this.OrderGroupAddressId;

			// Save record
			//using (TransactionScope scope = new TransactionScope())
			{
				base.AcceptChanges();

				if (!deleted)
				{
					newId = this.OrderGroupAddressId;

					// update shipping address in Shipments and LineItems
					if (oldId != newId)
					{
						UpdateLineItemsAddress(oldId.ToString(), newId.ToString());
						UpdateShipmentsAddress(oldId.ToString(), newId.ToString());
					}
				}
				else
				{
					// update shipments and line items associated with this address
					UpdateLineItemsAddress(oldId.ToString(), String.Empty);
					UpdateShipmentsAddress(oldId.ToString(), String.Empty);
				}
			}
        }

        /// <summary>
        /// Updates the line items address.
        /// </summary>
        /// <param name="oldAddressId">The old address id.</param>
        /// <param name="newAddressId">The new address id.</param>
		private void UpdateLineItemsAddress(string oldAddressId, string newAddressId)
		{
			if (_Parent.OrderForms.Count == 0)
				return;

			if (!String.IsNullOrEmpty(newAddressId) && oldAddressId != newAddressId)
			{
				// replace oldAddressId with newAddressId
				foreach (LineItem li in _Parent.OrderForms[0].LineItems)
					if (li.ObjectState != MetaObjectState.Deleted && String.Compare(li.ShippingAddressId, oldAddressId, true) == 0)
					{
						li.ShippingAddressId = newAddressId;
						li.AcceptChanges();
					}
			}
			else if (String.IsNullOrEmpty(newAddressId))
			{
				// set addressid to empty string (this is the case when Address is deleted)
				foreach (LineItem li in _Parent.OrderForms[0].LineItems)
					if (li.ObjectState != MetaObjectState.Deleted && String.Compare(li.ShippingAddressId, oldAddressId, true) == 0)
					{
						li.ShippingAddressId = "";
						li.AcceptChanges();
					}
			}
		}

        /// <summary>
        /// Updates the shipments address.
        /// </summary>
        /// <param name="oldAddressId">The old address id.</param>
        /// <param name="newAddressId">The new address id.</param>
		private void UpdateShipmentsAddress(string oldAddressId, string newAddressId)
		{
			if (_Parent.OrderForms.Count == 0)
				return;

			if (!String.IsNullOrEmpty(newAddressId) && oldAddressId != newAddressId)
			{
				// replace oldAddressId with newAddressId
				foreach (Shipment shipment in _Parent.OrderForms[0].Shipments)
					if (String.Compare(shipment.ShippingAddressId, oldAddressId, true) == 0)
					{
						shipment.ShippingAddressId = newAddressId;
						shipment.AcceptChanges();
						break; // break here because only 1 shipment goes to 1 address
					}
			}
			else if (String.IsNullOrEmpty(newAddressId))
			{
				// set addressid to empty string (this is the case when Address is deleted)
				foreach (Shipment shipment in _Parent.OrderForms[0].Shipments)
					if (String.Compare(shipment.ShippingAddressId, oldAddressId, true) == 0)
					{
						//shipment.ShippingAddressId = "";

						// ??? delete shipment
						shipment.Delete();
						shipment.AcceptChanges();
						break; // break here because only 1 shipment goes to 1 address
					}
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAddress"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrderAddress(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
    }
}
