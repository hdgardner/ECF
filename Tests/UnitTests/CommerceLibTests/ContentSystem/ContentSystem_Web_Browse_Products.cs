// Browse through Content Management

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;

    public class Public_Web_Browse_Products : PublicWebTestBase
    {
        public Public_Web_Browse_Products()
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
                    request.Url = MakeUrl("Canon-PowerShot-Pro-Series-S5-IS-80MP-Digital-Camera-with-12x-Optical-Image-Stabilized-Zoom.aspx");
                    break;
                case 1:
                    request.Url = MakeUrl("Canon-PowerShot-Pro-Series-S3-IS-6MP-with-12x-Image-Stabilized-Zoom.aspx");
                    break;
                case 2:
                    request.Url = MakeUrl("Canon-TC-DC58B-Tele-Converter-Lens-for-S5-IS-S3-IS-S2-IS-Digital-Camera.aspx");
                    break;
                case 3:
                    request.Url = MakeUrl("Lexar-Media-128-MB-Secure-Digital-Card.aspx");
                    break;
                case 4:
                    request.Url = MakeUrl("Canon-PSC-85-Deluxe-Soft-Case-for-Canon-Powershot-A650IS-and-A720IS.aspx");
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
