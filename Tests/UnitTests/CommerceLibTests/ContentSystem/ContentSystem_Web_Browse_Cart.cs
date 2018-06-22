// Browse through Content Management

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;

    public class Public_Web_Browse_Cart : PublicWebTestBase
    {
        public Public_Web_Browse_Cart()
            : base()
        {
        }

        /// <summary>
        /// Creates the request. Use RequestIndex property to determine which request to return
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override WebTestRequest CreateRequest(int index)
        {
            WebTestRequest request = new WebTestRequest(Url + "/default.aspx");
            request.Cache = false;
            request.ParseDependentRequests = false;

            switch (index)
            {
                case 0:
                    request.Url = MakeUrl("Cart/ShoppingCart.aspx");
                    break;
                default:
                    return null;
            }

            return request;
        }

        /// <summary>
        /// Makes the URL.
        /// </summary>
        /// <param name="relativePage">The relative page.</param>
        /// <returns></returns>
        private string MakeUrl(string relativePage)
        {
            return String.Format("{0}/{1}", Url, relativePage);
        }
    }
}
