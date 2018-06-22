namespace Mediachase.Commerce.Plugins.Payment.ECheck
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using nsoftware.IBizPay;
    using Mediachase.Commerce.Orders;

    public class ECheckGateway : AbstractPaymentGateway
    {
        Echeck _echeck = null;

        /// <summary>
        /// Processes the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override bool ProcessPayment(Payment payment, ref string message)
        {
            /*
            CheckPayment info = (CheckPayment)payment;

            _echeck = new Echeck();

            _echeck.InvoiceNumber = info.Parent.Parent.OrderGroupId.ToString();

            try
            {
                _echeck.MerchantLogin = Settings["MerchantLogin"];
                _echeck.Gateway = (EcheckGateways)Enum.Parse(typeof(EcheckGateways), Settings["Gateway"]);
                _echeck.MerchantPassword = Settings["MerchantPassword"];
            }
            catch
            {
                message = "ECheck is not configured properly";
                return false;
            }


            if (Settings["GatewayURL"].Trim() != "")
                _echeck.GatewayURL = Settings["GatewayURL"];

            _echeck.TransactionDesc = String.Format("Order Number {0}", info.TransactionId);
            _echeck.BankAccountNumber = info.BankAccountNumber;
            _echeck.BankRoutingNumber = info.BankRoutingNumber;
            if (info.BankAccountType.Length > 0)
            {
                _echeck.BankAccountType = (EcheckBankAccountTypes)Enum.Parse(typeof(EcheckBankAccountTypes), info.BankAccountType);
            }

            _echeck.BankName = info.BankName;
            _echeck.BankAccountName = info.BankAccountName;
            _echeck.CustomerLicenseNumber = info.LicenseNumber;
            _echeck.CustomerLicenseDOB = info.LicenseDOB.ToString("MM/dd/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            _echeck.CustomerLicenseState = info.LicenseState;
            _echeck.CheckNumber = info.CheckNumber;

            double transactionAmount = 0;
            transactionAmount = (double)info.Amount;
            _echeck.TransactionAmount = transactionAmount.ToString();

            Address addr = info.BillingAddress;
            _echeck.CustomerAddress = addr.Address1;
            _echeck.CustomerCity = addr.City;
            _echeck.CustomerCountry = addr.Country.Code;
            _echeck.CustomerEmail = info.Email;
            _echeck.CustomerFirstName = addr.FirstName;
            _echeck.CustomerLastName = addr.LastName;
            _echeck.CustomerPhone = addr.PhoneNumber;
            _echeck.CustomerState = addr.StateProvince;
            _echeck.CustomerZip = addr.ZipPostalCode;
            _echeck.CustomerCountry = addr.Country.Name;

            switch (_echeck.Gateway)
            {
                case EcheckGateways.gwAuthorizeNet:
                    AddSpecialField("x_Trans_Key");
                    AddConfigField("AIMHashSecret");
                    break;
            }

            try
            {
                _echeck.Authorize();

                bool approved = _echeck.TransactionApproved;
                if (!approved)
                {
                    message = "Transaction Declined: " + _echeck.ResponseText;
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new GatewayNotRespondingException(ex.Message);
            }

            //info.TextResponse = _echeck.ResponseText;
            info.AuthorizationCode = _echeck.ResponseCode;
            info.ValidationCode = _echeck.ResponseApprovalCode;
            */
            return true;
        }

        /// <summary>
        /// Adds the special field.
        /// </summary>
        /// <param name="name">The name.</param>
        private void AddSpecialField(string name)
        {
            string val = Settings[name];

            if (val.Length > 0)
                _echeck.AddSpecialField(name, val);
        }

        /// <summary>
        /// Adds the config field.
        /// </summary>
        /// <param name="name">The name.</param>
        private void AddConfigField(string name)
        {
            string val = Settings[name];

            if (val.Length > 0)
            {
                _echeck.Config(String.Format("{0}={1}", name, val));
            }
        }
    }
}
