using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus;

namespace Mediachase.Commerce.Storage
{
    /// <summary>
    /// Storage object interface.
    /// </summary>
    public interface IStorageObject
    {
        /// <summary>
        /// Gets the state of the object.
        /// </summary>
        /// <value>The state of the object.</value>
        MetaObjectState ObjectState {get;}
        /// <summary>
        /// Accepts the changes.
        /// </summary>
        void AcceptChanges();
    }
}
