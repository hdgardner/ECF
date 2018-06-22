using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Mediachase.Web.Console.Interfaces
{
    public interface IDynamicParamControl
    {
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        NameValueCollection Parameters { get; set;}
    }
}
