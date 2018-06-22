// Cleaned up code for web test for CatalogSystem

namespace UnitTests.CatalogSystem
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using System.Configuration;
    using UnitTests.Common;

    /// <summary>
    /// Browses the catalog system
    /// </summary>
    public class CatalogSystem_Web_Browse : AdminWebTestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogSystem_Web_Browse"/> class.
        /// </summary>
        public CatalogSystem_Web_Browse() : base()
        {
        }

        /// <summary>
        /// Creates the request. Use RequestIndex property to determine which request to return
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override WebTestRequest CreateRequest(int index)
        {
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl + "/Apps/Shell/Pages/ContentFrame.aspx");

            switch (index)
            {
                case 0:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    break;

                case 1:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Edit", false, false);
                    request.QueryStringParameters.Add("catalogid", "1", false, false);
                    break;

                case 2:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    break;

                case 3:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Node-List", false, false);
                    request.QueryStringParameters.Add("catalogid", "1", false, false);
                    request.QueryStringParameters.Add("catalognodeid", "", false, false);
                    break;

                case 4:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Node-Edit", false, false);
                    request.QueryStringParameters.Add("catalogid", "1", false, false);
                    request.QueryStringParameters.Add("catalognodeid", "", false, false);
                    request.QueryStringParameters.Add("nodeid", "8", false, false);
                    break;

                case 5:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Node-List", false, false);
                    request.QueryStringParameters.Add("catalogid", "1", false, false);
                    request.QueryStringParameters.Add("catalognodeid", "", false, false);
                    break;

                case 6:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Node-List", false, false);
                    request.QueryStringParameters.Add("catalogid", "1", false, false);
                    request.QueryStringParameters.Add("catalognodeid", "8", false, false);
                    break;

                default:
                    return null;
            }

            return request;
        }
    }
}