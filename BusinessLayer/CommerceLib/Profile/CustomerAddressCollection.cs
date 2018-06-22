using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Implements operations for and represents the customer address collection.
    /// </summary>
    public class CustomerAddressCollection : MetaStorageCollectionBase<CustomerAddress>
    {
        private Principal _Parent;

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Principal Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerAddressCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CustomerAddressCollection(Principal parent)
        {
            _Parent = parent;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Add(CustomerAddress value)
        {
            value.SetParent(_Parent);
            return base.Add(value);
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="principal">The principal.</param>
        internal void SetParent(Principal principal)
        {
            foreach (CustomerAddress address in this)
            {
                address.SetParent(principal);
            }
        }
    }
}
