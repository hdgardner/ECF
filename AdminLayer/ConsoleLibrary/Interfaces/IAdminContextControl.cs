using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Web.Console.Interfaces
{
    public interface IAdminContextControl
    {
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        void LoadContext(IDictionary context);
    }
}
