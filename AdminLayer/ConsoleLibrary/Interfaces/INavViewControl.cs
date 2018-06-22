using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Web.Console.Interfaces
{
    public interface INavViewControl
    {
        /// <summary>
        /// Gets or sets the root node.
        /// </summary>
        /// <value>The root node.</value>
        SiteMapNode RootNode { get; set;}
    }
}
