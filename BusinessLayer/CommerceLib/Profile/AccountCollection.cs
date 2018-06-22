using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Profile
{
    /// <summary>
    /// Represents the collection of customer accounts.
    /// </summary>
    public class AccountCollection : MetaStorageCollectionBase<Account>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountCollection"/> class.
        /// </summary>
        public AccountCollection()
        {
        }
    }
}
