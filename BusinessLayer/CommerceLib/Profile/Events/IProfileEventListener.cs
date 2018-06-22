using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Profile.Events
{
    /// <summary>
    /// Provides the methods necessary for the principal event listener.
    /// </summary>
    public interface IProfileEventListener
    {
        #region Principal Events
        void PrincipalCreated(object source, PrincipalEventArgs args);
        void PrincipalUpdated(object source, PrincipalEventArgs args);
        void PrincipalDeleted(object source, PrincipalEventArgs args);
        #endregion

        #region Address Events
        void AddressCreated(object source, AddressEventArgs args);
        void AddressUpdated(object source, AddressEventArgs args);
        void AddressDeleted(object source, AddressEventArgs args);
        #endregion
    }
}