/*
 * D I S C L A I M E R          
 * 
 * WARNING: ANY USE BY YOU OF THE SAMPLE CODE PROVIDED IS AT YOUR OWN RISK.
 * 
 * Authorize.Net provides this code "as is" without warranty of any kind, either
 * express or implied, including but not limited to the implied warranties of 
 * merchantability and/or fitness for a particular purpose.   
 * Authorize.Net owns and retains all right, title and interest in and to the 
 * Automated Recurring Billing intellectual property.
 */

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mediachase.Commerce.Orders.Exceptions;

namespace Mediachase.Commerce.Plugins.Payment.Authorize
{
	/// <summary>
	/// AuthorizeNet manager for recurring payments.
	/// </summary>
	public class AuthorizeNetRecurringManager
    {
		// This is the URL of the API server.
		// You must give this a valid value to successfully execute this sample code.
		private string _serverURL;

		// This is the user's Payment Gateway account login name used for transaction processing.
		// You must give this a valid value to successfully execute this sample code.
		private string _user;

		// This is the TransactionKey associated with that account.
		// You must give this a valid value to successfully execute this sample code.
		private string _password;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverURL"></param>
		/// <param name="user"></param>
		/// <param name="password"></param>
		public AuthorizeNetRecurringManager(string serverURL, string user, string password)
		{
			_serverURL = serverURL; // @"https://apitest.authorize.net/xml/v1/request.api";
			_user = user;
			_password = password;
		}
		
        /// <summary>
        /// Create a new ARB Subscription.
        /// </summary>
		public ARBCreateSubscriptionResponse CreateSubscription(ARBCreateSubscriptionRequest createSubscriptionRequest)
        {
            // Populate the authentication information
			PopulateMerchantAuthentication((ANetApiRequest)createSubscriptionRequest);
			ValidateCreateSubscribtionParameters(createSubscriptionRequest);

            // The response type will normally be ARBCreateSubscriptionResponse.
            // However, in the case of an error such as an XML parsing error, the response
            //   type will be ErrorResponse.
            object response = ProcessRequest(createSubscriptionRequest);

			// process response. If there are any errors, exception will be thrown.
			CheckResponseForErrors(response);

			return (ARBCreateSubscriptionResponse)response;
        }

        /// <summary>
        /// Update an existing ARB subscription using the subscription ID returned by the create.
        /// </summary>
		public ARBUpdateSubscriptionResponse UpdateSubscription(ARBUpdateSubscriptionRequest updateSubscriptionRequest)
        {
           	// Populate the authentication information
			PopulateMerchantAuthentication((ANetApiRequest)updateSubscriptionRequest);
			ValidateUpdateSubscribtionParameters(updateSubscriptionRequest);

            // The response type will normally be ARBUpdateSubscriptionResponse.
            // However, in the case of an error such as an XML parsing error, the response
            //   type will be ErrorResponse.

			object response = ProcessRequest(updateSubscriptionRequest);

			// process response. If there are any errors, exception will be thrown.
			CheckResponseForErrors(response);

			return (ARBUpdateSubscriptionResponse)response;
        }

        /// <summary>
        /// Cancel an existing ARB subscription using the subscription ID returned by the create.
        /// </summary>
		public ARBCancelSubscriptionResponse CancelSubscription(ARBCancelSubscriptionRequest cancelSubscriptionRequest)
        {
            // Populate the authentication information
			PopulateMerchantAuthentication((ANetApiRequest)cancelSubscriptionRequest);

            // The response type will normally be ARBCancelSubscriptionRequest.
            // However, in the case of an error such as an XML parsing error, the response
            //   type will be ErrorResponse.

			object response = ProcessRequest(cancelSubscriptionRequest);

			// process response. If there are any errors, exception will be thrown.
			CheckResponseForErrors(response);

			return (ARBCancelSubscriptionResponse)response;
		}

		#region Private methods
		/// <summary>
        /// Send the request to the API server and load the response into an XML document.
        /// An XmlSerializer is used to form the XML used in the request to the API server. 
        /// The response from the server is also XML. An XmlReader is used to process the
        /// response stream from the API server so that it can be loaded into an XmlDocument.
        /// </summary>
		/// <param name="apiRequest"></param>
        /// <returns>
        /// Object containing the response received from the API server.
        /// </returns>
        private object ProcessRequest(object apiRequest)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_serverURL);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";
            webRequest.KeepAlive = true;

            // Serialize the request
			XmlSerializer serializer = new XmlSerializer(apiRequest.GetType());
            XmlWriter writer = new XmlTextWriter(webRequest.GetRequestStream(), Encoding.UTF8);
            serializer.Serialize(writer, apiRequest);
            writer.Close();

            // Get the response
            WebResponse webResponse = webRequest.GetResponse();

            // Load the response from the API server into an XmlDocument.
			String rawResponseString = String.Empty;
			object response = null;
			using (XmlReader xmlReader = XmlReader.Create(webResponse.GetResponseStream()))
			{
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(xmlReader);
				response = ProcessXmlResponse(xmldoc);
				xmlReader.Close();
			}

			return response;
        }

        /// <summary>
        /// Deserialize the given XML document into the correct object type using the root
        /// node to determine the type of output object.
        /// 
        /// For any given API request the response can be one of two types:
        ///    ErorrResponse or [methodname]Response. 
        /// For example, the ARBCreateSubscriptionRequest would normally result in a response of
        /// ARBCreateSubscriptionResponse. This is also the name of the root node of the response.
        /// This name can be used to deserialize the response into local objects. 
        /// </summary>
        /// <param name="xmldoc">
        /// This is the XML document to process. It holds the response from the API server.
        /// </param>
        /// <param name="apiResponse">
        /// This will hold the deserialized object of the appropriate type.
        /// </param>
        /// <returns>
        /// True if successful, false if not.
        /// </returns>
		private object ProcessXmlResponse(XmlDocument xmldoc)
        {
			object apiResponse = null;
            XmlSerializer serializer;

            try
            {
                // Use the root node to determine the type of response object to create
                switch (xmldoc.DocumentElement.Name)
                {
                    case "ARBCreateSubscriptionResponse":
                        serializer = new XmlSerializer(typeof(ARBCreateSubscriptionResponse));
                        apiResponse = (ARBCreateSubscriptionResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;

                    case "ARBUpdateSubscriptionResponse":
                        serializer = new XmlSerializer(typeof(ARBUpdateSubscriptionResponse));
                        apiResponse = (ARBUpdateSubscriptionResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;

                    case "ARBCancelSubscriptionResponse":
                        serializer = new XmlSerializer(typeof(ARBCancelSubscriptionResponse));
                        apiResponse = (ARBCancelSubscriptionResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;

                    case "ErrorResponse":
                        serializer = new XmlSerializer(typeof(ANetApiResponse));
                        apiResponse = (ANetApiResponse)serializer.Deserialize(new StringReader(xmldoc.DocumentElement.OuterXml));
                        break;

                    default:
                        break;
                }
            }
            catch
            {
            }

            return apiResponse;
        }

        /// <summary>
        /// Determine the type of the response object and process accordingly.
        /// Since this is just sample code the only processing being done here is to write a few
        /// bits of information to the console window.
        /// </summary>
        /// <param name="response"></param>
        private void CheckResponseForErrors(object response)
        {
            // Every response is based on ANetApiResponse so you can always do this sort of type casting.
            ANetApiResponse baseResponse = (ANetApiResponse)response;

            // If the result code is "Ok" then the request was successfully processed.
            if (baseResponse.messages.resultCode == messageTypeEnum.Error)
            {
                // Write error messages to console window
				string exceptionMessage = String.Join("\r\n", baseResponse.messages.message.Select(a => (String.Format("[{0}] {1}", a.code, a.text))).ToArray());
				//Array.ForEach(baseResponse.messages.message, a => exceptionMessage += (String.Format("[{0}] {1}\r\n", a.code, a.text)));

                throw new PaymentException(PaymentException.ErrorType.ProviderError, baseResponse.messages.message.Length > 0 ? baseResponse.messages.message[0].code : String.Empty, exceptionMessage);
            }
        }

		/// <summary>
		/// Fill in the merchant authentication. This data is required for all API methods.
		/// </summary>
		/// <param name="request"></param>
		private void PopulateMerchantAuthentication(ANetApiRequest request)
		{
			request.merchantAuthentication = new merchantAuthenticationType();
			request.merchantAuthentication.name = _user;
			request.merchantAuthentication.transactionKey = _password;
			request.refId = Guid.NewGuid().ToString("N").Substring(0, 20); // MaxLength for refId is 20
		}

		/// <summary>
		/// Checks the length of strings in the request and corrects it when needed.
		/// </summary>
		/// <param name="request"></param>
		private void ValidateCreateSubscribtionParameters(ARBCreateSubscriptionRequest request)
		{
			if (request.subscription != null)
			{
				if(!String.IsNullOrEmpty(request.subscription.name) && request.subscription.name.Length > 50)
					request.subscription.name = request.subscription.name.Substring(0, 50);

				if (request.subscription.customer != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.customer.id) && request.subscription.customer.id.Length > 20)
						request.subscription.customer.id = request.subscription.customer.id.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.customer.email) && request.subscription.customer.email.Length > 255)
						request.subscription.customer.email = request.subscription.customer.email.Substring(0, 255);

					if (!String.IsNullOrEmpty(request.subscription.customer.phoneNumber) && request.subscription.customer.phoneNumber.Length > 25)
						request.subscription.customer.phoneNumber = request.subscription.customer.phoneNumber.Substring(0, 25);

					if (!String.IsNullOrEmpty(request.subscription.customer.faxNumber) && request.subscription.customer.faxNumber.Length > 25)
						request.subscription.customer.faxNumber = request.subscription.customer.faxNumber.Substring(0, 25);
				}

				if (request.subscription.order != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.order.invoiceNumber) && request.subscription.order.invoiceNumber.Length > 20)
						request.subscription.order.invoiceNumber = request.subscription.order.invoiceNumber.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.order.description) && request.subscription.order.description.Length > 255)
						request.subscription.order.description = request.subscription.order.description.Substring(0, 255);
				}

				if (request.subscription.billTo != null)
				{
					if (request.subscription.billTo.firstName.Length > 50)
						request.subscription.billTo.firstName = request.subscription.billTo.firstName.Substring(0, 50);

					if (request.subscription.billTo.lastName.Length > 50)
						request.subscription.billTo.lastName = request.subscription.billTo.lastName.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.billTo.company) && request.subscription.billTo.company.Length > 50)
						request.subscription.billTo.company = request.subscription.billTo.company.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.billTo.address) && request.subscription.billTo.address.Length > 60)
						request.subscription.billTo.address = request.subscription.billTo.address.Substring(0, 60);

					if (!String.IsNullOrEmpty(request.subscription.billTo.city) && request.subscription.billTo.city.Length > 40)
						request.subscription.billTo.city = request.subscription.billTo.city.Substring(0, 40);

					if (!String.IsNullOrEmpty(request.subscription.billTo.state) && request.subscription.billTo.state.Length > 2)
						request.subscription.billTo.state = request.subscription.billTo.state.Substring(0, 2);

					if (!String.IsNullOrEmpty(request.subscription.billTo.zip) && request.subscription.billTo.zip.Length > 20)
						request.subscription.billTo.zip = request.subscription.billTo.zip.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.billTo.country) && request.subscription.billTo.country.Length > 60)
						request.subscription.billTo.country = request.subscription.billTo.country.Substring(0, 60);
				}

				if (request.subscription.shipTo != null)
				{
					if (request.subscription.shipTo.firstName.Length > 50)
						request.subscription.shipTo.firstName = request.subscription.shipTo.firstName.Substring(0, 50);

					if (request.subscription.shipTo.lastName.Length > 50)
						request.subscription.shipTo.lastName = request.subscription.shipTo.lastName.Substring(0, 50);

					if (request.subscription.shipTo.company.Length > 50)
						request.subscription.shipTo.company = request.subscription.shipTo.company.Substring(0, 50);

					if (request.subscription.shipTo.address.Length > 60)
						request.subscription.shipTo.address = request.subscription.shipTo.address.Substring(0, 60);

					if (request.subscription.shipTo.city.Length > 40)
						request.subscription.shipTo.city = request.subscription.shipTo.city.Substring(0, 40);

					if (request.subscription.shipTo.state.Length > 40)
						request.subscription.shipTo.state = request.subscription.shipTo.state.Substring(0, 40);

					if (request.subscription.shipTo.zip.Length > 20)
						request.subscription.shipTo.zip = request.subscription.shipTo.zip.Substring(0, 20);

					if (request.subscription.shipTo.country.Length > 60)
						request.subscription.shipTo.country = request.subscription.shipTo.country.Substring(0, 60);
				}
			}
		}

		/// <summary>
		/// Checks the length of strings in the request and corrects it when needed.
		/// </summary>
		/// <param name="request"></param>
		private void ValidateUpdateSubscribtionParameters(ARBUpdateSubscriptionRequest request)
		{
			if (request.subscription != null)
			{
				if (!String.IsNullOrEmpty(request.subscription.name) && request.subscription.name.Length > 50)
					request.subscription.name = request.subscription.name.Substring(0, 50);

				if (request.subscription.customer != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.customer.id) && request.subscription.customer.id.Length > 20)
						request.subscription.customer.id = request.subscription.customer.id.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.customer.email) && request.subscription.customer.email.Length > 255)
						request.subscription.customer.email = request.subscription.customer.email.Substring(0, 255);

					if (!String.IsNullOrEmpty(request.subscription.customer.phoneNumber) && request.subscription.customer.phoneNumber.Length > 25)
						request.subscription.customer.phoneNumber = request.subscription.customer.phoneNumber.Substring(0, 25);

					if (!String.IsNullOrEmpty(request.subscription.customer.faxNumber) && request.subscription.customer.faxNumber.Length > 25)
						request.subscription.customer.faxNumber = request.subscription.customer.faxNumber.Substring(0, 25);
				}

				if (request.subscription.order != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.order.invoiceNumber) && request.subscription.order.invoiceNumber.Length > 20)
						request.subscription.order.invoiceNumber = request.subscription.order.invoiceNumber.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.order.description) && request.subscription.order.description.Length > 255)
						request.subscription.order.description = request.subscription.order.description.Substring(0, 255);
				}

				if (request.subscription.billTo != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.billTo.firstName) && request.subscription.billTo.firstName.Length > 50)
						request.subscription.billTo.firstName = request.subscription.billTo.firstName.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.billTo.lastName) && request.subscription.billTo.lastName.Length > 50)
						request.subscription.billTo.lastName = request.subscription.billTo.lastName.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.billTo.company) && request.subscription.billTo.company.Length > 50)
						request.subscription.billTo.company = request.subscription.billTo.company.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.billTo.address) && request.subscription.billTo.address.Length > 60)
						request.subscription.billTo.address = request.subscription.billTo.address.Substring(0, 60);

					if (!String.IsNullOrEmpty(request.subscription.billTo.city) && request.subscription.billTo.city.Length > 40)
						request.subscription.billTo.city = request.subscription.billTo.city.Substring(0, 40);

					if (!String.IsNullOrEmpty(request.subscription.billTo.state) && request.subscription.billTo.state.Length > 2)
						request.subscription.billTo.state = request.subscription.billTo.state.Substring(0, 2);

					if (!String.IsNullOrEmpty(request.subscription.billTo.zip) && request.subscription.billTo.zip.Length > 20)
						request.subscription.billTo.zip = request.subscription.billTo.zip.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.billTo.country) && request.subscription.billTo.country.Length > 60)
						request.subscription.billTo.country = request.subscription.billTo.country.Substring(0, 60);
				}

				if (request.subscription.shipTo != null)
				{
					if (!String.IsNullOrEmpty(request.subscription.shipTo.firstName) && request.subscription.shipTo.firstName.Length > 50)
						request.subscription.shipTo.firstName = request.subscription.shipTo.firstName.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.lastName) && request.subscription.shipTo.lastName.Length > 50)
						request.subscription.shipTo.lastName = request.subscription.shipTo.lastName.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.company) && request.subscription.shipTo.company.Length > 50)
						request.subscription.shipTo.company = request.subscription.shipTo.company.Substring(0, 50);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.address) && request.subscription.shipTo.address.Length > 60)
						request.subscription.shipTo.address = request.subscription.shipTo.address.Substring(0, 60);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.city) && request.subscription.shipTo.city.Length > 40)
						request.subscription.shipTo.city = request.subscription.shipTo.city.Substring(0, 40);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.state) && request.subscription.shipTo.state.Length > 40)
						request.subscription.shipTo.state = request.subscription.shipTo.state.Substring(0, 40);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.zip) && request.subscription.shipTo.zip.Length > 20)
						request.subscription.shipTo.zip = request.subscription.shipTo.zip.Substring(0, 20);

					if (!String.IsNullOrEmpty(request.subscription.shipTo.country) && request.subscription.shipTo.country.Length > 60)
						request.subscription.shipTo.country = request.subscription.shipTo.country.Substring(0, 60);
				}
			}
		}
		#endregion
	}
}
