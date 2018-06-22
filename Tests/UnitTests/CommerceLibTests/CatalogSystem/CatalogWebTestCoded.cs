namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.Common;
    using Mediachase.Commerce.Catalog.Dto;
    using Mediachase.Commerce.Catalog;


    /// <summary>
    /// Coded web test to create, edit, and delete a catalog.
    /// </summary>
    public class CatalogWebTestCoded : AdminWebTestBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogWebTestCoded"/> class.
        /// </summary>
        public CatalogWebTestCoded() : base() { }

        /// <summary>
        /// Finds the catalog id.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <returns></returns>
        int findCatalogId(string catalogName)
        {
            CatalogDto dto =  CatalogContext.Current.GetCatalogDto();
            CatalogDto.CatalogRow row = null;

            int index = -1;
            
            try
            {
                foreach (CatalogDto.CatalogRow node in dto.Catalog)
                {
                    if (node.Name.Equals(catalogName))
                    {
                        index = row.CatalogId;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.Write(e.Message);
            }
            
            return index;
        }

        /// <summary>
        /// Creates the request. Use RequestIndex property to determine which request to return
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override WebTestRequest CreateRequest(int index)
        {
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl + "/Apps/Shell/Pages/Contentframe.aspx");
            FormPostHttpBody requestBody;
            ExtractHiddenFields extractionRule;
            int catalogId;

            switch (index)
            {
                case 0:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    break;

                case 1:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Edit", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    break;


                case 2:
                    request.Method = "POST";
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Edit", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ctl21_ViewControl_DefaultTabContainer_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true]}");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogName", "WebTestCatalog");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogNameRequiredE_ClientStat" +
                            "e", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogNameRequiredE_C" +
                            "lientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$Date", "3/9/2009");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$vceDate1_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$vceDate1" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate2_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate2" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate3_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate3" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$Date", "3/9/2010");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$vceDate1_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$vceDate1_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate2_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate2_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate3_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate3_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultCurrency", "usd");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguage", "en-us");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguageRequiredE_Client" +
                            "State", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguageRequire" +
                            "dE_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$BaseWeight", "lbs");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$SortOrder", "0");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$IsCatalogActive$MySelectList", "False");
                    requestBody.FormPostParameters.Add("ctl21$EditSaveControl$SaveChangesButton", "OK");
                    request.Body = requestBody;
                    break;


                case 3:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    break;

                case 4:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Edit", false, false);
                    catalogId = findCatalogId("WebTestCatalog");
                    request.QueryStringParameters.Add("catalogid", catalogId.ToString(), false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    break;

                case 5:
                    request.Method = "POST";
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Edit", false, false);
                    catalogId = findCatalogId("WebTestCatalog");
                    request.QueryStringParameters.Add("catalogid", catalogId.ToString(), false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ctl21_ViewControl_DefaultTabContainer_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true]}");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogName", "WebTest");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogNameRequiredE_ClientStat" +
                            "e", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$CatalogNameRequiredE_C" +
                            "lientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$Date", "3/9/2009");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$vceDate1_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$vceDate1" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate2_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate2" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate3_ClientSt" +
                            "ate", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$AvailableFrom$cveDate3" +
                            "_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$Date", "3/12/2010");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$vceDate1_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$vceDate1_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate2_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate2_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate3_ClientState", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ExpiresOn$cveDate3_Cli" +
                            "entState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultCurrency", "usd");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguage", "en-us");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguageRequiredE_Client" +
                            "State", this.Context["$HIDDEN1.ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$DefaultLanguageRequire" +
                            "dE_ClientState"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$BaseWeight", "lbs");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$OtherLanguagesList", "");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$SortOrder", "0");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$IsCatalogActive$MySelectList", "False");
                    requestBody.FormPostParameters.Add("ctl21$EditSaveControl$SaveChangesButton", "OK");
                    request.Body = requestBody;
                    break;

                case 6:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    break;

                case 7:
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "cmPanel|cmContent");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", "cmContent");
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", "{\"CommandName\":\"cmdCatalogDeleteCatalog\",\"CommandArguments\":{\"GridId\":\"MainListVi" +
                            "ew\",\"GridClientId\":\"ctl21_MyListView_MainListView_lvTable\",\"permissions\":\"catalo" +
                            "g:ctlg:mng:delete\"}}");
                    requestBody.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ddPaging", "20");
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ctrl3$3_cb", "on");
                    requestBody.FormPostParameters.Add("__ASYNCPOST", "true");
                    request.Body = requestBody;
                    break;

                case 8:
                    request.QueryStringParameters.Add("_a", "Catalog", false, false);
                    request.QueryStringParameters.Add("_v", "Catalog-List", false, false);
                    break;

                default:
                    return null;

            }
            return request;
        }
    }
}