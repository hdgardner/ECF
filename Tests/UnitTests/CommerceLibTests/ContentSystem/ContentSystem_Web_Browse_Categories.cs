// Browse through Content Management

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;

    public class Public_Web_Browse_Categories : PublicWebTestBase
    {
        public Public_Web_Browse_Categories()
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


            /* memory lead debugging
            switch (index)
            {
                case 0:
                    request.Url = MakeUrl("memory.aspx");
                    break;
                default:
                    return null;
            }
             * */

            switch (index)
            {
                case 0:
                    request.Url = MakeUrl("Digital-Cameras.aspx");
                    break;
                case 1:
                    request.Url = MakeUrl("Mobile-Phones.aspx");
                    break;
                case 2:
                    request.Url = MakeUrl("Mobile-Phones.aspx");
                    break;
                case 3:
                    request.Url = MakeUrl("Phone-Accessories.aspx");
                    break;
                case 4:
                    request.Url = MakeUrl("DailySpecials.aspx");
                    break;
                case 5:
                    request.Url = MakeUrl("Camera-Accessories.aspx");
                    break;
                case 6:
                    request.Url = MakeUrl("Digital-Cameras.aspx?Brand=canon");
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
