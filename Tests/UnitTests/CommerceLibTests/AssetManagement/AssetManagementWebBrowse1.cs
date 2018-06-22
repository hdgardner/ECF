namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using UnitTests.AssetManagement;
    using UnitTests.Common;
    using Mediachase.Ibn.Library;
    using Mediachase.Ibn.Data.Services;
    using Mediachase.Ibn.Data.Sql.Management;

    public class AssetManagementWebBrowse1 : AssetManagementWebTestBase
    {

        public AssetManagementWebBrowse1() : base()
        {
        }

        private string getFolderID()
        {
            return String.Empty;
        }

        private string getFolderElementID()
        {
            return String.Empty;
        }

        /// <summary>
        /// Creates the request. Use RequestIndex property to determine which request to return
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override WebTestRequest CreateRequest(int index)
        {
            WebTestRequest request;
            ExtractHiddenFields extractionRule;
            FormPostHttpBody requestBody;

            switch (index)
            {
                case 0:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", "1", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 1:
                    request = new WebTestRequest("http://localhost/admin/Apps/Core/Controls/DialogPage.aspx");
                    request.ThinkTime = 10;
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-NewFolder", false, false);
                    request.QueryStringParameters.Add("Id", "1", false, false);
                    request.QueryStringParameters.Add("refreshName", "McCommandHandlerRefreshWindow", false, false);
                    request.QueryStringParameters.Add("closeFramePopup", "McCommandHandler_ClosePopup", false, false);
                    request.QueryStringParameters.Add("dependRefreshCommand", "", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "2";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 2:
                    request = new WebTestRequest("http://localhost/admin/Apps/Core/Controls/DialogPage.aspx");
                    request.Method = "POST";
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-NewFolder", false, false);
                    request.QueryStringParameters.Add("Id", "1", false, false);
                    request.QueryStringParameters.Add("refreshName", "McCommandHandlerRefreshWindow", false, false);
                    request.QueryStringParameters.Add("closeFramePopup", "McCommandHandler_ClosePopup", false, false);
                    request.QueryStringParameters.Add("dependRefreshCommand", "", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN2.__EVENTTARGET"].ToString());
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN2.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN2.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("ctl19$FolderName", "Web Test");
                    requestBody.FormPostParameters.Add("ctl19$btnCreate", "Create");
                    request.Body = requestBody;
                    return request;

                case 3:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", "1", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "cmPanel|cmContent");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", "cmContent");
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ddPaging", "20");
                    requestBody.FormPostParameters.Add("__ASYNCPOST", "true");
                    string parameters = this.Context["$HIDDEN1._params"].ToString();
                    request.Body = requestBody;
                    return request;

                case 4:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");

                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    return request;

                case 5:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx"); 
                    
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "FileItem-Edit", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 6:
                    request = new WebTestRequest("http://localhost/admin/Apps/Core/Controls/Uploader/uploadiframe.aspx");
                    request.ThinkTime = 30;
                    request.QueryStringParameters.Add("SessionUid", "a9101499-1804-45e7-9a4e-5002b657224c", false, false);
                    request.QueryStringParameters.Add("TempFileStorageProvider", "McLocalDiskTempFileStorageProvider", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "0";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 7:
                    request = new WebTestRequest("http://localhost/admin/Apps/Core/Controls/Uploader/uploadiframe.aspx");
                    request.Method = "POST";
                    request.QueryStringParameters.Add("SessionUid", "a9101499-1804-45e7-9a4e-5002b657224c", false, false);
                    request.QueryStringParameters.Add("TempFileStorageProvider", "McLocalDiskTempFileStorageProvider", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN0.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("__MEDIACHASE_FORM_UNIQUEID", this.Context["$HIDDEN0.__MEDIACHASE_FORM_UNIQUEID"].ToString());
                    requestBody.FormPostParameters.Add(new FileUploadParameter("McFileUp", "Winter.jpg", "image/pjpeg"));
                    request.Body = requestBody;
                    return request;

                case 8:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "FileItem-Edit", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ctl00|ctl21$ViewControl$Default" +
                            "TabContainer$ctl00$ctl00$btnUpload");
                    requestBody.FormPostParameters.Add("ctl21_ViewControl_DefaultTabContainer_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true]}");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("__MEDIACHASE_FORM_UNIQUEID2", this.Context["$HIDDEN1.__MEDIACHASE_FORM_UNIQUEID2"].ToString());
                    requestBody.FormPostParameters.Add("__ASYNCPOST", "true");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$btnUpload", "");
                    request.Body = requestBody;
                    return request;

                case 9:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "FileItem-Edit", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    request.QueryStringParameters.Add("objectid", getFolderElementID(), false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 10:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.Method = "POST";
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "FileItem-Edit", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    request.QueryStringParameters.Add("objectid", getFolderElementID(), false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                    requestBody.FormPostParameters.Add("ctl21_ViewControl_DefaultTabContainer_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true]}");
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ucView$fRenderer$fRenderer_tLay" +
                            "out$ctl00$smartTL_68307933ebb341849c54b29cd5d7c8a8$ctl00$Cntrl_362748bbeaa34daa8" +
                            "158b4ea64e29953$txtValue", "Winter.jpg");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ucView$fRenderer$fRenderer_tLay" +
                            "out$ctl00$smartTL_68307933ebb341849c54b29cd5d7c8a8$ctl05$Cntrl_600a066db6b04fa5b" +
                            "62f2cc684cc7912$txtValue", "winter picture");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ucView$fRenderer$fRenderer_tLay" +
                            "out$ctl00$smartTL_68307933ebb341849c54b29cd5d7c8a8$ctl02$Cntrl_c811240a4a41403d9" +
                            "2d6f6dd3a665255$txtValue", "galore");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ucView$fRenderer$fRenderer_tLay" +
                            "out$ctl00$smartTL_68307933ebb341849c54b29cd5d7c8a8$ctl03$Cntrl_1202ebe595b748239" +
                            "c1f083045be92fd$txtValue", "800");
                    requestBody.FormPostParameters.Add("ctl21$ViewControl$DefaultTabContainer$ctl00$ctl00$ucView$fRenderer$fRenderer_tLay" +
                            "out$ctl00$smartTL_68307933ebb341849c54b29cd5d7c8a8$ctl04$Cntrl_15bd5192401d40a5a" +
                            "ee889b8e89f58e3$txtValue", "600");
                    requestBody.FormPostParameters.Add("ctl21$EditSaveControl$SaveChangesButton", "  OK  ");
                    request.Body = requestBody;
                    return request;

                case 11:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 12:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", getFolderID(), false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "cmPanel|cmContent");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", "cmContent");
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", "{\"CommandName\":\"cmdAssetDelete\",\"CommandArguments\":{\"GridId\":\"MainListView\",\"Grid" +
                            "ClientId\":\"ctl21_MyListView_MainListView_lvTable\",\"permissions\":\"asset:mng:delet" +
                            "e\"}}");
                    requestBody.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ddPaging", "20");
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ctrl1$1_cb", "on");
                    requestBody.FormPostParameters.Add("__ASYNCPOST", "true");
                    request.Body = requestBody;
                    return request;

                case 13:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", "1", false, false);
                    extractionRule = new ExtractHiddenFields();
                    extractionRule.Required = true;
                    extractionRule.HtmlDecode = true;
                    extractionRule.ContextParameterName = "1";
                    request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule.Extract);
                    return request;

                case 14:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.Method = "POST";
                    request.Headers.Add(new WebTestRequestHeader("x-microsoftajax", "Delta=true"));
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", "1", false, false);
                    requestBody = new FormPostHttpBody();
                    requestBody.FormPostParameters.Add("ScriptManager1", "cmPanel|cmContent");
                    requestBody.FormPostParameters.Add("__EVENTTARGET", "cmContent");
                    requestBody.FormPostParameters.Add("__EVENTARGUMENT", "{\"CommandName\":\"cmdAssetDelete\",\"CommandArguments\":{\"GridId\":\"MainListView\",\"Grid" +
                            "ClientId\":\"ctl21_MyListView_MainListView_lvTable\",\"permissions\":\"asset:mng:delet" +
                            "e\"}}");
                    requestBody.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
                    requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                    requestBody.FormPostParameters.Add("_action", this.Context["$HIDDEN1._action"].ToString());
                    requestBody.FormPostParameters.Add("_params", this.Context["$HIDDEN1._params"].ToString());
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ddPaging", "20");
                    requestBody.FormPostParameters.Add("ctl21$MyListView$MainListView$ctrl7$7_cb", "on");
                    requestBody.FormPostParameters.Add("__ASYNCPOST", "true");
                    request.Body = requestBody;
                    return request;

                case 15:
                    request = new WebTestRequest("http://localhost/admin/Apps/Shell/Pages/ContentFrame.aspx");
                    request.QueryStringParameters.Add("_a", "Asset", false, false);
                    request.QueryStringParameters.Add("_v", "Asset-List", false, false);
                    request.QueryStringParameters.Add("id", "1", false, false);
                    return request;

                default:
                    return null;
            }

        }
    }
}
