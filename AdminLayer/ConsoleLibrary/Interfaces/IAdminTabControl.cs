using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Interfaces
{
    public interface IAdminTabControl : IDynamicParamControl
    {
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        void SaveChanges(IDictionary context);
    }
}
