using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using System.Configuration;
using System.Reflection;
using Mediachase.Ibn.Data.Meta;
using UnitTests.Common;

namespace UnitTests.AssetManagement
{
    /// <summary>
    /// Base class for admin web tests
    /// </summary>
    public abstract class AssetManagementWebTestBase : WebTestBase
    {
        private string _CommerceManagerUrl = String.Empty;

        /// <summary>
        /// Gets or sets the commerce manager URL.
        /// </summary>
        /// <value>The commerce manager URL.</value>
        public string CommerceManagerUrl
        {
            get { return _CommerceManagerUrl; }
            set { _CommerceManagerUrl = value; }
        }

        private int _RequestIndex = 0;

        /// <summary>
        /// Gets or sets the index of the request.
        /// </summary>
        /// <value>The index of the request.</value>
        public int RequestIndex
        {
            get { return _RequestIndex; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminWebTestBase"/> class.
        /// </summary>
        public AssetManagementWebTestBase()
            : base()
        {
            _CommerceManagerUrl = "http://localhost/admin";//GetConfigSetting("commercemanager-url");
        }

        /// <summary>
        /// Creates the default request.
        /// </summary>
        /// <returns></returns>
        private WebTestRequest CreateDefaultRequest()
        {
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl);
            request.ExpectedResponseUrl = CommerceManagerUrl + "/Login.aspx?ReturnUrl=%2fadmin%2fDefault.aspx" + "";
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            return request;
        }

        /// <summary>
        /// Creates the login request.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        private WebTestRequest CreateLoginRequest(string username, string password, string application)
        {
            WebTestRequest request = new WebTestRequest(CommerceManagerUrl + "/Login.aspx");
            request.ExpectedResponseUrl = CommerceManagerUrl + "/Login.aspx?ReturnUrl=%2fadmin%2fDefault.aspx" + "";

            // "/Login.aspx"
            request.Method = "POST";
            request.ExpectedResponseUrl = CommerceManagerUrl + "/Apps/Shell/Pages/default.aspx";
            request.QueryStringParameters.Add("ReturnUrl", "%2fadmin%2fDefault.aspx", false, false);
            FormPostHttpBody requestBody = new FormPostHttpBody();
            requestBody.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            requestBody.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            requestBody.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            requestBody.FormPostParameters.Add("LoginCtrl$UserName", username);
            requestBody.FormPostParameters.Add("LoginCtrl$Password", password);
            requestBody.FormPostParameters.Add("LoginCtrl$Application", application);
            requestBody.FormPostParameters.Add("LoginCtrl$LoginButton", "Log In");
            request.Body = requestBody;
            return request;
        }

        /// <summary>
        /// When overridden in a derived class, returns an <see cref="T:System.Collections.Generic.IEnumerator`1"/> interface supporting a simple iteration over a generic collection of <see cref="T:Microsoft.VisualStudio.TestTools.WebTesting.WebTestRequest"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> that contains <see cref="T:Microsoft.VisualStudio.TestTools.WebTesting.WebTestRequest"/> objects.
        /// </returns>
        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }

            // First, login to commerce manager
            yield return CreateDefaultRequest();
            yield return CreateLoginRequest("admin", "store", "eCommerceFramework");

            // Now do specific tests
            WebTestRequest request = null;

            // Continue until no request is returned by the method
            while ((request = CreateRequest(RequestIndex)) != null)
            {
                // Increment request count
                _RequestIndex++;

                // Return the request created
                yield return request;
            }

            // null
            request = null;
        }

        /// <summary>
        /// Creates the request. Use RequestIndex property to determine which request to return
        /// </summary>
        /// <returns></returns>
        public abstract WebTestRequest CreateRequest(int index);
    }
}
