using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Shared;
using System.Threading;
using Mediachase.Commerce.Orders.Exceptions;
using Common.Logging;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    /// <summary>
    /// This activity handles processing different types of payments. It will call the appropriate 
    /// payment handler configured in the database and raise exceptions if something goes wrong.
    /// It also deals with removing sensitive data for credit card types of payments depending on the 
    /// configuration settings.
    /// </summary>
	public partial class ProcessPaymentActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(ProcessPaymentActivity));
        public static DependencyProperty ProcessingPaymentEvent = DependencyProperty.Register("ProcessingPayment", typeof(EventHandler), typeof(ProcessPaymentActivity));
        public static DependencyProperty ProcessedPaymentEvent = DependencyProperty.Register("ProcessedPayment", typeof(EventHandler), typeof(ProcessPaymentActivity));

        private const string EventsCategory = "Handlers";

        // Define private constants for the Validation Errors 
        private const int TotalPaymentMismatch = 1;

        private readonly ILog Logger;

        /// <summary>
        /// Gets or sets the order group.
        /// </summary>
        /// <value>The order group.</value>
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public OrderGroup OrderGroup
        {
            get
            {
                return (OrderGroup)(base.GetValue(ProcessPaymentActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(ProcessPaymentActivity.OrderGroupProperty, value);
            }
        }

        #region Public Events


        /// <summary>
        /// Occurs when [processing payment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessingPayment event is raised before a payment is processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessingPayment
        {
            add
            {
                base.AddHandler(ProcessPaymentActivity.ProcessingPaymentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessPaymentActivity.ProcessingPaymentEvent, value);
            }
        }


        /// <summary>
        /// Occurs when [processed payment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessedPayment event is raised after the payment has been processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessedPayment
        {
            add
            {
                base.AddHandler(ProcessPaymentActivity.ProcessedPaymentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessPaymentActivity.ProcessedPaymentEvent, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessPaymentActivity"/> class.
        /// </summary>
        public ProcessPaymentActivity()
		{
            Logger = LogManager.GetLogger(GetType());
			InitializeComponent();

		}

        /// <summary>
        /// Executes the specified execution context.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Raise the ProcessingPaymentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessPaymentActivity.ProcessingPaymentEvent, this, EventArgs.Empty);

                // Validate the properties at runtime
                this.ValidateRuntime();

                // Process payment now
                this.ProcessPayment();

                // Raise the ProcessedPaymentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessPaymentActivity.ProcessedPaymentEvent, this, EventArgs.Empty);

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch(Exception ex)
            {
                Logger.Error("Failed to process payment.", ex);
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
                throw;
            }
        }

        /// <summary>
        /// Validates the runtime.
        /// </summary>
        /// <returns></returns>
        private bool ValidateRuntime()
        {
            // Create a new collection for storing the validation errors
            ValidationErrorCollection validationErrors = new ValidationErrorCollection();

            // Validate the Order Properties
            this.ValidateOrderProperties(validationErrors);

            // Raise an exception if we have ValidationErrors
            if (validationErrors.HasErrors)
            {
                string validationErrorsMessage = String.Empty;

                foreach (ValidationError error in validationErrors)
                {
                    validationErrorsMessage +=
                        string.Format("Validation Error:  Number {0} - '{1}' \n",
                        error.ErrorNumber, error.ErrorText);
                }

                // Throw a new exception with the validation errors.
                throw new WorkflowValidationFailedException(validationErrorsMessage, validationErrors);
            }


            // If we made it this far, then the data must be valid. 
            return true;
        }

        /// <summary>
        /// Validates the order properties.
        /// </summary>
        /// <param name="validationErrors">The validation errors.</param>
        private void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(ProcessPaymentActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }

            // Cycle through all Order Forms and check total, it should be equal to total of all payments
            decimal paymentTotal = 0;
            foreach (OrderForm orderForm in OrderGroup.OrderForms)
            {
                foreach (Payment payment in orderForm.Payments)
                {
                    paymentTotal += payment.Amount;
                }
            }

            if (paymentTotal != OrderGroup.Total)
            {
                Logger.Error(String.Format("Payment Total Price does not match order total price."));
                ValidationError validationError = new ValidationError("Payment Total Price does not match order total price",
                                    TotalPaymentMismatch, false);
                validationErrors.Add(validationError);
            }
        }

        /// <summary>
        /// Processes the payment.
        /// </summary>
        private void ProcessPayment()
        {
            // If total is 0, we do not need to proceed
            if (OrderGroup.Total == 0)
                return;

            // Start Charging!
            PaymentMethodDto methods = PaymentManager.GetPaymentMethods(Thread.CurrentThread.CurrentCulture.Name);
            foreach (OrderForm orderForm in OrderGroup.OrderForms)
            {
                foreach (Payment payment in orderForm.Payments)
                {
                    PaymentMethodDto.PaymentMethodRow[] rows = (PaymentMethodDto.PaymentMethodRow[])methods.PaymentMethod.Select(String.Format("PaymentMethodId = '{0}'", payment.PaymentMethodId));

                    // If we couldn't find payment method specified, generate an error
                    if (rows == null || rows.Length == 0)
                    {
                        throw new MissingMethodException(String.Format("Specified payment method \"{0}\" has not been defined.", payment.PaymentMethodId));
                    }

                    Logger.Debug(String.Format("Getting the type \"{0}\".", rows[0].ClassName));
                    Type type = Type.GetType(rows[0].ClassName);
                    if (type == null)
                        throw new TypeLoadException(String.Format("Specified payment method class \"{0}\" can not be created.", rows[0].ClassName));

                    Logger.Debug(String.Format("Creating instance of \"{0}\".", type.Name));
                    IPaymentGateway provider = (IPaymentGateway)Activator.CreateInstance(type);

                    provider.Settings = CreateSettings(rows[0]);

                    string message = "";
                    Logger.Debug(String.Format("Processing the payment."));
                    if (!provider.ProcessPayment(payment, ref message))
                    {
                        throw new PaymentException(PaymentException.ErrorType.ProviderError, "", String.Format(message));
                    }
                    Logger.Debug(String.Format("Payment processed."));
                    PostProcessPayment(payment);

                    // TODO: add message to transaction log
                }
            }
        }

        /// <summary>
        /// Pres the process payment. Unencrypts the data if needed.
        /// </summary>
        private void PostProcessPayment(Payment payment)
        {
            // We only care about credit cards here, all other payment types should be encrypted by default
            if (payment.GetType() == typeof(CreditCardPayment))
            {
                // for partial type, remove everything but last 4 digits
                if (OrderConfiguration.Instance.SensitiveDataMode == SensitiveDataPersistance.Partial)
                {
                    string ccNumber = ((CreditCardPayment)payment).CreditCardNumber;
                    if (!String.IsNullOrEmpty(ccNumber) && ccNumber.Length > 4)
                    {
                        ccNumber = ccNumber.Substring(ccNumber.Length - 4);
                        ((CreditCardPayment)payment).CreditCardNumber = ccNumber;
                    }
                }
                else if (OrderConfiguration.Instance.SensitiveDataMode == SensitiveDataPersistance.DoNotPersist)
                {
                    ((CreditCardPayment)payment).CreditCardNumber = String.Empty; 
                }

                // Always remove pin
                ((CreditCardPayment)payment).CreditCardSecurityCode = String.Empty;
            }
        }

        /// <summary>
        /// Creates the settings.
        /// </summary>
        /// <param name="methodRow">The method row.</param>
        /// <returns></returns>
        private Dictionary<string, string> CreateSettings(PaymentMethodDto.PaymentMethodRow methodRow)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            PaymentMethodDto.PaymentMethodParameterRow[] rows = methodRow.GetPaymentMethodParameterRows();
            foreach (PaymentMethodDto.PaymentMethodParameterRow row in rows)
            {
                settings.Add(row.Parameter, row.Value);
            }

            return settings;
        }

	}
}
