namespace Mediachase.Commerce.Plugins.Payment.Authorize
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Resources;
	using System.Text;
	using System.Web;
	using Mediachase.Commerce.Orders;
	using Mediachase.Commerce.Orders.Exceptions;
	using Mediachase.Commerce.Core;
	using Mediachase.Commerce.Shared;

	/// <summary>
	/// AuthorizePaymentGateway
	/// </summary>
    public class AuthorizePaymentGateway : AbstractPaymentGateway
    {
		public static readonly string _UserParameterName = "Login";
		public static readonly string _TransactionKeyParameterName = "TransactionKey";
		public static readonly string _ProcessUrlParameterName = "ProcessUrl";
		public static readonly string _PaymentOptionParameterName = "PaymentOption";
		public static readonly string _RecurringProcessUrlParameterName = "RecurringProcessUrl";
		public static readonly string _RecurringMethodParameterName = "RecurringMethod";
		public static readonly string _CancelStatusParameterName = "CancelStatus";

		public static readonly string _AuthorizeRecurringMethodParameterValue = "authorize";

        #region IPaymentGateway Members

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override bool ProcessPayment(Payment payment, ref string message)
        {
			string erroneousConfigurationMessage = "Authorize.NET payment gateway is not configured correctly. {0} is not set.";

            // cast the object first
            CreditCardPayment info = (CreditCardPayment)payment;

			if (info == null)
			{
				message = "Payment information is not specified.";
				throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
			}

			// Check if the gateway is configured correctly
            string processUrl = String.Empty;
            string user = String.Empty;
            string password = String.Empty;
			string recurringMethod = String.Empty;
			string cancelStatus = String.Empty;

			#region Getting parameters from the db
			// get user name
			if (!Settings.ContainsKey(_UserParameterName) || String.IsNullOrEmpty(Settings[_UserParameterName]))
            {
                message = String.Format(erroneousConfigurationMessage, _UserParameterName);
                throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
            }

			user = Settings[_UserParameterName];

			// get transaction key
			if (!Settings.ContainsKey(_TransactionKeyParameterName) || String.IsNullOrEmpty(Settings[_TransactionKeyParameterName]))
            {
				message = String.Format(erroneousConfigurationMessage, _TransactionKeyParameterName);
                throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
            }

			password = Settings[_TransactionKeyParameterName];
			#endregion

			bool processRegularTransaction = true;

			#region --------------- Process Transaction ---------------
			if (payment.Parent != null && (payment.Parent.Parent is PaymentPlan))
			{
				// get recurring method and determine which type of transaction to perform
				if (!Settings.ContainsKey(_RecurringMethodParameterName) || String.IsNullOrEmpty(Settings[_RecurringMethodParameterName]))
				{
					message = String.Format(erroneousConfigurationMessage, _RecurringMethodParameterName);
					throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
				}

				recurringMethod = Settings[_RecurringMethodParameterName];

				if (String.Compare(recurringMethod, _AuthorizeRecurringMethodParameterValue, StringComparison.OrdinalIgnoreCase) == 0)
					processRegularTransaction = false;
			}

			if(processRegularTransaction)
			{
				#region Get parameters for the regular transaction
				// get processing url
				if (!Settings.ContainsKey(_ProcessUrlParameterName) || String.IsNullOrEmpty(Settings[_ProcessUrlParameterName]))
				{
					message = String.Format(erroneousConfigurationMessage, _ProcessUrlParameterName);
					throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
				}

				processUrl = Settings[_ProcessUrlParameterName];
				#endregion

				#region Process regular transaction

				AuthorizeNetManager mgr = new AuthorizeNetManager(processUrl, user, password);

				TransactionData transData = new TransactionData();
				transData.card = new CreditCard();

				transData.type = Settings[_PaymentOptionParameterName] == "S" ? TransactionType.Sale : TransactionType.Authorization;

				transData.totalAmount = (double)GetSumInUSD(info.Amount, info.Parent.Parent.BillingCurrency);

				transData.card.cardNr = info.CreditCardNumber;
				transData.card.CSC = info.CreditCardSecurityCode;
				transData.card.expDate = new DateTime(info.ExpirationYear, info.ExpirationMonth, 1);

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

                if(address == null)
                    throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", "Billing address was not specified.");

				transData.card.customerAddress = new Authorize.Address();
				transData.card.customerAddress.countryCode = address.CountryCode;
				transData.card.customerAddress.zipCode = address.PostalCode;
				transData.card.customerAddress.city = address.City;
				transData.card.customerAddress.state = address.State;
				transData.card.customerAddress.street = address.Line1;
				transData.card.customerFirstName = address.FirstName;
				transData.card.customerLastName = address.LastName;
				transData.card.customerFax = address.FaxNumber;
				transData.card.customerPhone = address.DaytimePhoneNumber;
				transData.card.customerEMail = address.Email;

				ResponsePackage pkg = null;

				try
				{
					pkg = mgr.PerformCardTransaction(transData);
				}
				catch (PaymentException ex)
				{
					throw;
					//message = ex.Message;
					//return false;
				}
				catch
				{
					throw new PaymentException(PaymentException.ErrorType.ConnectionFailed, "", "Failed to connect to the payment gateway.");
				}

				//info.TextResponse = pkg.responseReasonText;
				info.AuthorizationCode = pkg.responseCode;
				info.ValidationCode = pkg.approvalCode;
				#endregion
			}
			else
			{
				#region Get parameters for the recurring transaction
				// get processing url
				if (!Settings.ContainsKey(_RecurringProcessUrlParameterName) || String.IsNullOrEmpty(Settings[_RecurringProcessUrlParameterName]))
				{
					message = String.Format(erroneousConfigurationMessage, _RecurringProcessUrlParameterName);
					throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
				}

				processUrl = Settings[_RecurringProcessUrlParameterName];

				// get cancel status
				if (!Settings.ContainsKey(_CancelStatusParameterName) || String.IsNullOrEmpty(Settings[_CancelStatusParameterName]))
				{
					message = String.Format(erroneousConfigurationMessage, _CancelStatusParameterName);
					throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", message);
				}

				cancelStatus = Settings[_CancelStatusParameterName];
				#endregion

				PaymentPlan plan = (PaymentPlan)payment.Parent.Parent;

				#region Process recurring Authorize.NET transaction

				// Payment Plan -> Recurring transaction
				AuthorizeNetRecurringManager recurringMgr = new AuthorizeNetRecurringManager(processUrl, user, password);

				ANetApiResponse response = null;
				try
				{
					if (String.Compare(plan.Status, cancelStatus, StringComparison.OrdinalIgnoreCase) == 0)
					{
						// cancel subscription
						if (!String.IsNullOrEmpty(payment.AuthorizationCode))
						{
							ARBCancelSubscriptionResponse cancelResponse = recurringMgr.CancelSubscription(PopulateCancelSubscriptionRequest(info));
							// clear the authorization code
							info.AuthorizationCode = String.Empty;
						}
						else
							throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "Payment AuthorizationCode cannot be null for the CancelSubscription operation", message);
					}
					else if (plan.CompletedCyclesCount == 0)
					{
						if (String.IsNullOrEmpty(info.AuthorizationCode))
						{
							// create subscription
							ARBCreateSubscriptionResponse createResponse = recurringMgr.CreateSubscription(PopulateCreateSubscriptionRequest(info));
							response = (ANetApiResponse)createResponse;
							info.AuthorizationCode = createResponse.subscriptionId;
						}
						else
							// update subscription
							recurringMgr.UpdateSubscription(PopulateUpdateSubscriptionRequest(info));
					}
					else
						message = "The operation is invalid.";
				}
				catch (PaymentException ex)
				{
					throw ex;
				}
				catch
				{
					throw new PaymentException(PaymentException.ErrorType.ConnectionFailed, "", "Failed to connect to the payment gateway.");
				}
				#endregion
			}
			#endregion

			return true;
        }
        #endregion

		/// <summary>
		/// Converts given sum to the sum in USD. Need to do this since Authorize.NET accepts payments only in USD.
		/// </summary>
		/// <param name="sum"></param>
		/// <param name="currentCurrency"></param>
		/// <returns></returns>
		private static decimal GetSumInUSD(decimal sum, string currentCurrency)
		{
			if (String.IsNullOrEmpty(currentCurrency))
				return sum;

			decimal usdSum = 0;

			string toCurrency = "usd";
			usdSum = CurrencyFormatter.ConvertCurrency(sum, currentCurrency, ref toCurrency);
			if (currentCurrency == toCurrency)
				throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", 
					String.Format("Couldn't convert {0} to USD. Please check that the rate for conversion is set.", currentCurrency));

			return usdSum;
		}

		#region Helper methods for recurring payment
		/// <summary>
		/// Fill in the given request with test data to create a new subscription.
		/// </summary>
		/// <param name="payment">The payment object.</param>
		private static ARBCreateSubscriptionRequest PopulateCreateSubscriptionRequest(CreditCardPayment payment)
		{
			ARBCreateSubscriptionRequest request = new ARBCreateSubscriptionRequest();

			PaymentPlan plan = (PaymentPlan)payment.Parent.Parent;

			ARBSubscriptionType sub = new ARBSubscriptionType();
			creditCardType creditCard = new creditCardType();

			sub.name = CoreConfiguration.Instance.DefaultApplicationName + " - subscription";

			creditCard.cardNumber = payment.CreditCardNumber;
			creditCard.expirationDate = String.Format("{0:d4}-{1:d2}", payment.ExpirationYear, payment.ExpirationMonth);  // required format for API is YYYY-MM
			sub.payment = new paymentType();
			sub.payment.Item = creditCard;

			// Find the address
			OrderAddress address = null;
			foreach (OrderAddress a in plan.OrderAddresses)
			{
				if (a.Name == payment.BillingAddressId)
				{
					address = a;
					break;
				}
			}

			sub.billTo = new nameAndAddressType();
			sub.billTo.firstName = address.FirstName;
			sub.billTo.lastName = address.LastName;

			// Create a subscription 

			sub.paymentSchedule = new paymentScheduleType();
			sub.paymentSchedule.startDate = plan.StartDate;
			sub.paymentSchedule.startDateSpecified = true;

			sub.paymentSchedule.totalOccurrences = Convert.ToInt16(plan.MaxCyclesCount);
			sub.paymentSchedule.totalOccurrencesSpecified = true;

			// free 1 month trial
			//sub.paymentSchedule.trialOccurrences = 1;
			//sub.paymentSchedule.trialOccurrencesSpecified = true;
			//sub.trialAmount = 0.00M;
			//sub.trialAmountSpecified = true;

			sub.amount = GetSumInUSD(payment.Amount, plan.BillingCurrency);
			sub.amountSpecified = true;

			sub.paymentSchedule.interval = GetRecurringPaymentTypeInterval(plan);

			sub.order = new orderType();
			sub.order.invoiceNumber = plan.OrderGroupId.ToString();

			sub.customer = new customerType();
			sub.customer.email = address.Email;

			request.subscription = sub;

			return request;
		}

		/// <summary>
		/// Fill in the given request with test data used to update the subscription.
		/// </summary>
		/// <param name="payment">The payment object.</param>
		private static ARBUpdateSubscriptionRequest PopulateUpdateSubscriptionRequest(CreditCardPayment payment)
		{
			ARBUpdateSubscriptionRequest request = new ARBUpdateSubscriptionRequest();

			PaymentPlan plan = (PaymentPlan)payment.Parent.Parent;

			ARBSubscriptionType sub = new ARBSubscriptionType();
			creditCardType creditCard = new creditCardType();

			request.subscriptionId = payment.AuthorizationCode;

			// update the subscription information
			creditCard.cardNumber = payment.CreditCardNumber;
			creditCard.expirationDate = String.Format("{0:d4}-{1:d2}", payment.ExpirationYear, payment.ExpirationMonth);  // required format for API is YYYY-MM
			sub.payment = new paymentType();
			sub.payment.Item = creditCard;

			// Find the address
			OrderAddress address = null;
			foreach (OrderAddress a in plan.OrderAddresses)
			{
				if (a.Name == payment.BillingAddressId)
				{
					address = a;
					break;
				}
			}

			sub.billTo = new nameAndAddressType();
			sub.billTo.firstName = address.FirstName;
			sub.billTo.lastName = address.LastName;

			sub.amount = GetSumInUSD(payment.Amount, plan.BillingCurrency);
			sub.amountSpecified = true;

			sub.order = new orderType();
			sub.order.invoiceNumber = plan.OrderGroupId.ToString();

			sub.customer = new customerType();
			sub.customer.email = address.Email;

			request.subscription = sub;

			return request;
		}

		/// <summary>
		/// Fill in the given request with test data used to cancel the subscription.
		/// </summary>
		/// <param name="payment">The payment object.</param>
		private static ARBCancelSubscriptionRequest PopulateCancelSubscriptionRequest(CreditCardPayment payment)
		{
			ARBCancelSubscriptionRequest request = new ARBCancelSubscriptionRequest();

			ARBSubscriptionType sub = new ARBSubscriptionType();
			creditCardType creditCard = new creditCardType();

			request.subscriptionId = payment.AuthorizationCode;

			return request;
		}

		/// <summary>
		/// Returns paymentScheduleTypeInterval objct based on <paramref name="plan"/> parameter values.
		/// </summary>
		/// <param name="plan">The payment plan.</param>
		/// <returns></returns>
		private static paymentScheduleTypeInterval GetRecurringPaymentTypeInterval(PaymentPlan plan)
		{
			paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();

			if (plan.CycleMode == PaymentPlanCycle.Days)
				interval.unit = ARBSubscriptionUnitEnum.days;
			else if (plan.CycleMode == PaymentPlanCycle.Months)
				interval.unit = ARBSubscriptionUnitEnum.months;
			else
				throw new PaymentException(PaymentException.ErrorType.ConfigurationError, "", "Plan CycleMode is not supported.");

			interval.length = Convert.ToInt16(plan.CycleLength);

			return interval;
		}
		#endregion
	}
}