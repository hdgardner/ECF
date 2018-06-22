using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Net;
using System.Collections;
using System.Globalization;
using Mediachase.Commerce.Orders.Exceptions;

namespace Mediachase.Commerce.Plugins.Payment.Authorize
{
	#region Helper Classes
	/*public enum CardType
	{
		Visa,
		MasterCard,
		AmericanExpress,
		DinersClub,
		Discover,
		JCB
	}*/

	public enum TransactionType
	{
		Authorization,		
		Sale
	}

	public class Address
	{
		public string street;
		public string city;
		public string state;
		public string zipCode;
		public string countryCode;

        /// <summary>
        /// Makes the address string.
        /// </summary>
        /// <returns></returns>
		public string MakeAddressString()
		{
			string result = "";

			if(street!=null)
				result += String.Format("x_address={0}&", street);
			if(city!=null)
				result += String.Format("x_city={0}&", city);
			if(state!=null)
				result += String.Format("x_state={0}&", state);
			if(countryCode!=null)
				result += String.Format("x_country={0}&", countryCode);
			if(zipCode!=null)
				result += String.Format("x_zip={0}&", zipCode);
			
			return result;
		}
	}

	public class CreditCard
	{
		//public CardType cardType;
		public string cardNr;
		public string CSC;
		public DateTime expDate;		
		public string customerFirstName;
		public string customerLastName;
		public string customerEMail;
		public string customerPhone;
		public string customerFax;
		public Address customerAddress;

        /// <summary>
        /// Makes the customer information string.
        /// </summary>
        /// <returns></returns>
		public string MakeCustomerInformationString()
		{
			string result = "";

			if(customerFirstName!=null)
				result += String.Format("x_first_name={0}&", customerFirstName);
			if(customerLastName!=null)
				result += String.Format("x_last_name={0}&", customerLastName);
			if(customerEMail!=null)
				result += String.Format("x_email={0}&", customerEMail);
			if(customerPhone!=null)
				result += String.Format("x_phone={0}&", customerPhone);
			if(customerFax!=null)
				result += String.Format("x_fax={0}&", customerFax);
			if(customerAddress!=null)
				result += customerAddress.MakeAddressString();

			return result;
		}
	}

	public class TransactionData
	{
		public TransactionType type = TransactionType.Authorization;
		public CreditCard card;
		public double totalAmount;

        /// <summary>
        /// Gets the type of the transaction.
        /// </summary>
        /// <returns></returns>
		public string GetTransactionType()
		{
			switch(type)
			{
				case TransactionType.Authorization:
					return "AUTH_ONLY";
				case TransactionType.Sale:
					return "AUTH_CAPTURE";
				default:
					return "";
			}
		}
	}
	
	// for verion 3.1
	public class ResponsePackage
	{		
		public bool IsError = false;		
		public string responseCode;
		public string responseSubcode;
		public string responseReasonCode;
		public string responseReasonText;
		public string approvalCode;
		public string AVSResult;
		public string transactionID;
		public string MD5Hash;
		public string CSCResponse;
		//public string cardholderAuthenticationResponse;
	}

	class RequestPackage
	{
		private string _serverURL;
		private string _user;
		private string _password;

		/// <summary>
		/// Initializes a new instance of the <see cref="RequestPackage"/> class.
		/// </summary>
		/// <param name="serverURL">The server URL.</param>
		/// <param name="user">The user.</param>
		/// <param name="password">The password.</param>
		public RequestPackage(string serverURL, string user, string password)
		{
			_serverURL = serverURL;
			_user = user;
			_password = password;
		}

		/// <summary>
		/// Processes the specified trans data.
		/// </summary>
		/// <param name="transData">The trans data.</param>
		/// <returns></returns>
		public ResponsePackage Process(TransactionData transData)
		{
			if (transData.card == null)
				throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", "Credit Card not defined");

			string requestXML = "";

			ResponsePackage response = new ResponsePackage();

			NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
			nfi.NumberDecimalSeparator = ".";

			requestXML = String.Format("x_version=3.1&x_delim_data=True&" +
				"x_login={0}&" +
				"x_password={1}&" +
				"x_amount={2}&" +
				"x_card_num={3}&" +
				"x_exp_date={4}&" +
				"x_card_code={5}&",
				_user, _password, transData.totalAmount.ToString("####.00", nfi), transData.card.cardNr, transData.card.expDate.ToString("MMyyyy"), transData.card.CSC);

			requestXML += transData.card.MakeCustomerInformationString();

			requestXML += String.Format(/*"x_test_request=TRUE&"+*/
				"x_method=CC&" +
				"x_type={0}",
				transData.GetTransactionType());

			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] data = encoding.GetBytes(requestXML);

			HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(_serverURL);
			myRequest.Method = "POST";
			myRequest.ContentType = "application/x-www-form-urlencoded";
			myRequest.ContentLength = data.Length;
			Stream newStream = myRequest.GetRequestStream();
			newStream.Write(data, 0, data.Length);
			newStream.Close();

			WebResponse myResponse = myRequest.GetResponse();
			String rawResponseString = String.Empty;
			using (StreamReader sr = new StreamReader(myResponse.GetResponseStream()))
			{
				rawResponseString = sr.ReadToEnd();
				response = ParseResponse(rawResponseString);
				sr.Close();
			}

			return response;
		}

		/// <summary>
		/// Parses the response.
		/// </summary>
		/// <param name="responseString">The response string.</param>
		/// <returns></returns>
		public ResponsePackage ParseResponse(string responseString)
		{
			ResponsePackage response = new ResponsePackage();
			string[] arr = responseString.Split(',');
			if (arr[0] != "1")
				response.IsError = true;

			response.responseCode = GetResponseCodeDescription(arr[0]);
			response.responseSubcode = arr[1];
			response.responseReasonCode = arr[2];
			response.responseReasonText = arr[3];
			response.approvalCode = arr[4];
			response.AVSResult = GetAVSResultCodeDescription(arr[5]);
			response.transactionID = arr[6];
			response.MD5Hash = arr[37];
			response.CSCResponse = GetCSCResponseCodeDescription(arr[38]);
			//response.cardholderAuthenticationResponse = GetCardholderAuthenticationResponseCodeDescription(arr[39]);

			return response;
		}

		/// <summary>
		/// Gets the response code description.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns></returns>
		public string GetResponseCodeDescription(string code)
		{
			switch (code)
			{
				case "1":
					return "Approved";
				case "2":
					return "Declined";
				case "3":
					return "Error";
				default:
					return "";
			}
		}

		/// <summary>
		/// Gets the AVS result code description.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns></returns>
		public string GetAVSResultCodeDescription(string code)
		{
			switch (code)
			{
				case "A":
					return "Address (Street) metches, ZIP does not";
				case "B":
					return "Address information not provided for AVS check";
				case "E":
					return "AVS error";
				case "G":
					return "Non-U.S. Card Issuing Bank";
				case "N":
					return "No Match on Address (Street) or ZIP";
				case "P":
					return "AVS not applicable for this transaction";
				case "R":
					return "Retry - System unavailable or timed out";
				case "S":
					return "Service not supported by issuer";
				case "U":
					return "Address information is unavailale";
				case "W":
					return "9 digit ZIP matches, Address (Street) does not";
				case "X":
					return "Address (Street) and 9 digit ZIP match";
				case "Y":
					return "Address (Street) and 5 digit ZIP match";
				case "Z":
					return "5 digit ZIP matches, Address (Street) does not";
				default:
					return "";
			}
		}

		/// <summary>
		/// Gets the CSC response code description.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns></returns>
		public string GetCSCResponseCodeDescription(string code)
		{
			switch (code)
			{
				case "M":
					return "Match";
				case "N":
					return "No Match";
				case "P":
					return "Not Processed";
				case "S":
					return "Should have been present";
				case "U":
					return "Issuer unable to process request";
				default:
					return "";
			}
		}

		/// <summary>
		/// Gets the cardholder authentication response code description.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns></returns>
		public string GetCardholderAuthenticationResponseCodeDescription(string code)
		{
			switch (code)
			{
				case "0":
					return "CAVV not validated because erroneous datawas submitted";
				case "1":
					return "CAVV failed validation";
				case "2":
					return "CAVV passed validation";
				case "3":
					return "CAVV validation could not be performed; issuer attempt incomplete";
				case "4":
					return "CAVV validation could not be performed; issuer system error";
				case "7":
					return "CAVV attempt - failed validation - issuer available(U.S.-issued card/non-U.S. acquirer)";
				case "8":
					return "CAVV attempt - passed validation - issuer available(U.S.-issued card/non-U.S. acquirer)";
				case "9":
					return "CAVV attempt - failed validation - issuer unavailable(U.S.-issued card/non-U.S. acquirer)";
				case "A":
					return "CAVV attempt - passed validation - issuer available(U.S.-issued card/non-U.S. acquirer)";
				case "B":
					return "CAVV passed validation, information only, no liability shift";
				default:
					return "CAVV not validated";
			}
		}
	}
	#endregion

	public class AuthorizeNetManager
	{
		private string _serverURL;
		private string _user;
		private string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeNetManager"/> class.
        /// </summary>
        /// <param name="serverURL">The server URL.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
		public AuthorizeNetManager(string serverURL, string user, string password)
		{
			_serverURL = serverURL;
			_user = user;
			_password = password;
		}

        /// <summary>
        /// Performs the card transaction.
        /// </summary>
        /// <param name="transData">The trans data.</param>
        /// <returns></returns>
		public ResponsePackage PerformCardTransaction(TransactionData transData)
		{
			RequestPackage request = new RequestPackage(_serverURL, _user, _password);
			ResponsePackage result = request.Process(transData);

            if (result.IsError)
            {                
                PaymentException exception = new PaymentException(PaymentException.ErrorType.ProviderError, result.responseCode, result.responseReasonText);

                // Add additional parameters about the error, which might help end user
                exception.ResponseMessages.Add("CSCResponse", result.CSCResponse);
                exception.ResponseMessages.Add("ReasonCode", result.responseReasonCode);
                exception.ResponseMessages.Add("Subcode", result.responseSubcode);
                exception.ResponseMessages.Add("AVSResult", result.AVSResult);

                // now throw the exception
                throw exception;
            }

			return result;
		}
	}
}