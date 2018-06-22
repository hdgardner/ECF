using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Web.Console.Interfaces
{
    public interface ISiteMapProvider
    {
        /// <summary>
        /// Adds the new node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="parentNode">The parent node.</param>
        void AddNewNode(SiteMapNode node, SiteMapNode parentNode);
    }
}
