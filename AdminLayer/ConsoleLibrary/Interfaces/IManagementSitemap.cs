using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Web.Console.Interfaces
{
    /// <summary>
    /// Site map interface used to create navigation structure in the management console.
    /// The Initialize method will be called once and will allow one to create menu choices.
    /// </summary>
    public interface IManagementSitemap
    {
        /// <summary>
        /// Use this method to create menu items in the Management Console.
        /// <remarks>
        /// The following example will add Catalog Management as a root menu choice on the left.
        /// </remarks>
        /// 	<example>
        /// SiteNavigationNode rootNode = new SiteNavigationNode("Catalog_Home", "Catalog Management", String.Empty);
        /// rootNode.App = "Catalog";
        /// context.Navigation.Home.Children.Add(rootNode);
        /// </example>
        /// </summary>
        /// <param name="context">The context.</param>
        void Initialize(ManagementContext context);
    }
}
