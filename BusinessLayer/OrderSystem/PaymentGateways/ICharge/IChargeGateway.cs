namespace Mediachase.Commerce.Plugins.Payment.ICharge
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Mediachase.Commerce.Orders;
    using nsoftware.IBizPay;
    using Mediachase.Commerce.Orders.Exceptions;

    public class IChargeGateway : AbstractPaymentGateway
    {
		Icharge _icharge = null;

        /// <summary>
        /// Processes the payment. Can be used for both positive and negative transactions.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override bool ProcessPayment(Payment payment, ref string message)
		{
            CreditCardPayment info = (CreditCardPayment)payment;
			_icharge = new Icharge();
					
			_icharge.InvoiceNumber = info.Parent.Parent.OrderGroupId.ToString();

            try
            {
                _icharge.MerchantLogin = Settings["MerchantLogin"];
                _icharge.MerchantPassword = Settings["MerchantPassword"];
                _icharge.TransactionType = (IchargeTransactionTypes)Enum.Parse(typeof(IchargeTransactionTypes), Settings["TransactionType"]);
                _icharge.Gateway = (IchargeGateways)Enum.Parse(typeof(IchargeGateways), Settings["Gateway"]);
            }
            catch
            {
                message = "ICharge gateway is not configured properly";
                return false;
            }

            if (Settings["GatewayURL"].Trim() != "")
                _icharge.GatewayURL = Settings["GatewayURL"];
            			
			_icharge.CardExpMonth = info.ExpirationMonth.ToString();
            _icharge.CardExpYear = info.ExpirationYear.ToString();
			_icharge.CardNumber = info.CreditCardNumber;
            _icharge.CardCVV2Data = info.CreditCardSecurityCode;

            // Find the address
            OrderAddress address = null;
            foreach (OrderAddress a in info.Parent.Parent.OrderAddresses)
            {
                if (a.Name == info.BillingAddressId)
                {
                    address = a;
                    break;
                }
            }    

            _icharge.CustomerAddress = address.Line1;
            _icharge.CustomerCity = address.City;
            _icharge.CustomerCountry = address.CountryCode;
            _icharge.CustomerEmail = address.Email;
            _icharge.CustomerFirstName = address.FirstName;
            _icharge.CustomerLastName = address.LastName;
            _icharge.CustomerPhone = address.DaytimePhoneNumber;
            _icharge.CustomerState = address.State;
            _icharge.CustomerZip = address.PostalCode;

            double transactionAmount = (double)info.Amount;
            _icharge.TransactionAmount = transactionAmount.ToString();

			switch(_icharge.Gateway)
			{
				case IchargeGateways.gwAuthorizeNet:
                    // for testing purpose uncomment 2 lines below   
                    //_icharge.GatewayURL = "https://test.authorize.net/gateway/transact.dll";
                    //x_test_request field needs to be set directly from _icharge
                    //AddSpecialField("x_test_request", "true"); will not work since need to pass the value 
                    //for the variable
                    //_icharge.Config(String.Format("{0}={1}", "x_test_request", "true"));
                    AddSpecialField(info, "x_Trans_Key");
                    break;
				case IchargeGateways.gwPlanetPayment:
				case IchargeGateways.gwMPCS:
				case IchargeGateways.gwRTWare:
				case IchargeGateways.gwECX:
					AddSpecialField(info, "x_Trans_Key");
					AddConfigField(info, "AIMHashSecret");
					break;
				case IchargeGateways.gwViaKlix:
					AddSpecialField(info, "ssl_user_id");
					break;
				case IchargeGateways.gwBankOfAmerica:
					_icharge.AddSpecialField("ecom_payment_card_name", info.CustomerName);
					AddConfigField(info, "referer");
					break;
				case IchargeGateways.gwInnovative:
					AddSpecialField(info, "test_override_errors");
					break;
				case IchargeGateways.gwTrustCommerce:
				case IchargeGateways.gw3DSI:
					_icharge.TransactionAmount = _icharge.TransactionAmount.Replace(".", "");
					break;
				case IchargeGateways.gwPayFuse:
					AddConfigField(info, "MerchantAlias");
					_icharge.TransactionAmount = _icharge.TransactionAmount.Replace(".", "");
					break;
				case IchargeGateways.gwYourPay:
				case IchargeGateways.gwFirstData:
				case IchargeGateways.gwLinkPoint:
					_icharge.SSLCertStore = Settings["SSLCertStore"];
					_icharge.SSLCertSubject = Settings["SSLCertSubject"];
					_icharge.SSLCertEncoded = Settings["SSLCertEncoded"];					
					break;
				case IchargeGateways.gwPRIGate:
					_icharge.MerchantPassword = Settings["MerchantPassword"];
					break;
				case IchargeGateways.gwProtx:
					AddSpecialField(info, "RelatedSecurityKey");
					AddSpecialField(info, "RelatedVendorTXCode");
					AddSpecialField(info, "RelatedTXAuthNo");
					break;
				case IchargeGateways.gwOptimal:
					_icharge.MerchantPassword = Settings["MerchantPassword"];
					AddSpecialField(info, "account");
					break;
				case IchargeGateways.gwEFSNet:
					_icharge.AddSpecialField("OriginalTransactionAmount", _icharge.TransactionAmount);
					break;
				case IchargeGateways.gwCyberCash:
					AddSpecialField(info, "CustomerID");
					AddSpecialField(info, "ZoneID");
					AddSpecialField(info, "Username");
					break;
                case IchargeGateways.gwPayFlowPro:
                    // for testing purpose uncomment line below   
                    //_icharge.GatewayURL = "test-payflow.verisign.com";
                    _icharge.AddSpecialField("user", Settings["MerchantLogin"]);
                    break;
                case IchargeGateways.gwMoneris:
                    // for testing purpose uncomment line below
                    //_icharge.GatewayURL = "https://esqa.moneris.com/HPPDP/index.php";
                    _icharge.TransactionAmount = transactionAmount.ToString("##0.00");
                    break;
				default:
					break;
				
			}

			_icharge.TransactionDesc = String.Format("Order Number {0}", _icharge.TransactionId);

			try
			{
				_icharge.Authorize();

				bool approved = _icharge.TransactionApproved;
				if(!approved)
				{
					message = "Transaction Declined: " + _icharge.ResponseText;
					return false;
				}

			}
			catch(Exception ex)
			{
                throw new GatewayNotRespondingException(ex.Message);
			}

			//info.TextResponse = _icharge.ResponseText;
            info.ValidationCode = _icharge.ResponseApprovalCode;
            info.AuthorizationCode = _icharge.ResponseCode;

            return true;
		}

        /// <summary>
        /// Adds the special field.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="name">The name.</param>
        private void AddSpecialField(CreditCardPayment info, string name)
		{
			string val = Settings[name];

			if(val.Length > 0)
				_icharge.AddSpecialField(name, val);
		}

        /// <summary>
        /// Adds the config field.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="name">The name.</param>
        private void AddConfigField(CreditCardPayment info, string name)
		{
			string val = Settings[name];

			if(val.Length > 0)
			{
				_icharge.Config(String.Format("{0}={1}", name, val));
			}
        }
	}
}
