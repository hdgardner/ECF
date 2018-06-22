// Browse through Content Management

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;

    public class ContentSystem_Web_Browse : AdminWebTestBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSystem_Web_Browse"/> class.
        /// </summary>
        public ContentSystem_Web_Browse()
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
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl + "/Apps/Shell/Pages/ContentFrame.aspx");
            switch (index)
            {
                case 0:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Approve-List", false, false);
                    request.QueryStringParameters.Add("", "approve", false, false);
                    break;
                case 1:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Site-List", false, false);
                    break;
                case 2:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Site-Edit", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 3:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Menu-List", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false); 
                    break;
                case 4:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Menu-Edit", false, false);
                    request.QueryStringParameters.Add("menuid", "3", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 5:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "MenuItem-List", false, false);
                    request.QueryStringParameters.Add("menuitemid", "9", false, false);
                    request.QueryStringParameters.Add("langid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 6:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "MenuItem-Edit", false, false);
                    request.QueryStringParameters.Add("menuitemid", "10", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    request.QueryStringParameters.Add("langid", "1", false, false);
                    break;
                case 7:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 8:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "14", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 9:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 10:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Page-Edit", false, false);
                    request.QueryStringParameters.Add("PageId", "18", false, false);
                    request.QueryStringParameters.Add("FolderId", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 11:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 12:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "18", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 13:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Page-Edit", false, false);
                    request.QueryStringParameters.Add("PageId", "30", false, false);
                    request.QueryStringParameters.Add("FolderId", "18", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 14:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "18", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 15:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "30", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 16:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Page-Edit", false, false);
                    request.QueryStringParameters.Add("PageId", "19", false, false);
                    request.QueryStringParameters.Add("FolderId", "30", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 17:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "Folder-List", false, false);
                    request.QueryStringParameters.Add("folderid", "30", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 18:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "MenuItem-List", false, false);
                    request.QueryStringParameters.Add("menuitemid", "10", false, false);
                    request.QueryStringParameters.Add("langid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    break;
                case 19:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "MenuItem-Edit", false, false);
                    request.QueryStringParameters.Add("menuitemid", "17", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false);
                    request.QueryStringParameters.Add("langid", "1", false, false);
                    break;
                case 20:
                    request.QueryStringParameters.Add("_a", "Content", false, false);
                    request.QueryStringParameters.Add("_v", "MenuItem-List", false, false);
                    request.QueryStringParameters.Add("menuitemid", "11", false, false);
                    request.QueryStringParameters.Add("langid", "1", false, false);
                    request.QueryStringParameters.Add("siteid", "1ab08b1a-c480-47b5-a98e-3d50b433dcb5", false, false); 
                    break;
                default:
                    return null;
            }

            return request;
        }
    }
}
