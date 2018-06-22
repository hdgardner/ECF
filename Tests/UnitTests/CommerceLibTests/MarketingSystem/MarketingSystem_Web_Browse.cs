// Web browse test for marketing system

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;


    public class MarketingSystem_Web_Browse : AdminWebTestBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingSystem_Web_Browse"/> class.
        /// </summary>
        public MarketingSystem_Web_Browse() : base()
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
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Campaign-List", false, false);
                    break;
                case 1:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Campaign-Edit", false, false);
                    request.QueryStringParameters.Add("campaignid", "1", false, false);
                    break;
                case 2:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Promotion-List", false, false); 
                    break;
                case 3:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Promotion-Edit", false, false);
                    request.QueryStringParameters.Add("promotionid", "1", false, false);
                    break;
                case 4:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Segment-List", false, false);
                    break;
                    /*
                case 5:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Segment-Edit", false, false);
                    request.QueryStringParameters.Add("segmentid", "1", false, false);
                    break;
                     * */
                case 5:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Policy-List", false, false);
                    break;
                case 6:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Policy-List", false, false);
                    request.QueryStringParameters.Add("group", "order", false, false);
                    break;
                case 7:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Expression-List", false, false);
                    request.QueryStringParameters.Add("group", "promotion", false, false);
                    break;
                case 8:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Expression-Edit", false, false);
                    request.QueryStringParameters.Add("expressionid", "1", false, false);
                    break;
                case 9:
                    request.QueryStringParameters.Add("_a", "Marketing", false, false);
                    request.QueryStringParameters.Add("_v", "Expression-List", false, false);
                    request.QueryStringParameters.Add("group", "segment", false, false); 
                    break;
                default:
                    return null;
            }

            return request;
        }
    }
}
