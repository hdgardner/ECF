using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.WebTesting;
using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
using System.Configuration;
using System.Reflection;
using Mediachase.Ibn.Data.Meta;

namespace UnitTests.Common
{
    /// <summary>
    /// Base class for admin web tests
    /// </summary>
    public abstract class PublicWebTestBase : WebTestBase
    {
        private string _Url = String.Empty;

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
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
        /// Initializes a new instance of the <see cref="PublicWebTestBase"/> class.
        /// </summary>
        public PublicWebTestBase() : base()
        {
            _Url = "http://ecf50.dev.mediachase.local/frontend";//GetConfigSetting("public-url");
            this.PreAuthenticate = false;
        }

        /// <summary>
        /// Creates the default request.
        /// </summary>
        /// <returns></returns>
        private WebTestRequest CreateDefaultRequest()
        {
            WebTestRequest request = new WebTestRequest(Url);
            request.ExpectedResponseUrl = Url + "/Login.aspx?ReturnUrl=%2fadmin%2fDefault.aspx" + "";
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
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

            /*
            if ((this.Context.ValidationLevel >= ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }
             * */

            // First, login to commerce manager
            //yield return CreateDefaultRequest();
            //yield return CreateLoginRequest("admin", "store", "mediachase");

            // Now do specific tests
            WebTestRequest request = null;

            // Continue until no request is returned by the method
            while ((request = CreateRequest(RequestIndex)) != null)
            {
                // Increment request count
                _RequestIndex++;

                // Return the request created
                yield return request;
                request = null;
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
