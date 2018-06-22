using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
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
using Common.Logging;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    /// <summary>
    /// This activity is responsible for calculating the shipping prices for shipments defined for order group.
    /// It calls the appropriate interface defined by the shipping option table and passes the method id and shipment object.
    /// </summary>
	public partial class ProcessShipmentsActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(ProcessShipmentsActivity));
        public static DependencyProperty ProcessingShipmentEvent = DependencyProperty.Register("ProcessingShipment", typeof(EventHandler), typeof(ProcessShipmentsActivity));
        public static DependencyProperty ProcessedShipmentEvent = DependencyProperty.Register("ProcessedShipment", typeof(EventHandler), typeof(ProcessShipmentsActivity));

        private const string EventsCategory = "Handlers";
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
                return (OrderGroup)(base.GetValue(ProcessShipmentsActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(ProcessShipmentsActivity.OrderGroupProperty, value);
            }
        }

        #region Public Events


        /// <summary>
        /// Occurs when [processing shipment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessingShipment event is raised before a Shipment is processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessingShipment
        {
            add
            {
                base.AddHandler(ProcessShipmentsActivity.ProcessingShipmentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessShipmentsActivity.ProcessingShipmentEvent, value);
            }
        }


        /// <summary>
        /// Occurs when [processed shipment].
        /// </summary>
        [ValidationOption(ValidationOption.Optional)]
        [Description("The ProcessedShipment event is raised after the Shipment has been processed.")]
        [Category(EventsCategory)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public event EventHandler ProcessedShipment
        {
            add
            {
                base.AddHandler(ProcessShipmentsActivity.ProcessedShipmentEvent, value);
            }
            remove
            {
                base.RemoveHandler(ProcessShipmentsActivity.ProcessedShipmentEvent, value);
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessShipmentsActivity"/> class.
        /// </summary>
        public ProcessShipmentsActivity()
		{
            Logger = LogManager.GetLogger(GetType());
			InitializeComponent();
		}

        /// <summary>
        /// Called by the workflow runtime to execute an activity.
        /// </summary>
        /// <param name="executionContext">The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionContext"/> to associate with this <see cref="T:System.Workflow.ComponentModel.Activity"/> and execution.</param>
        /// <returns>
        /// The <see cref="T:System.Workflow.ComponentModel.ActivityExecutionStatus"/> of the run task, which determines whether the activity remains in the executing state, or transitions to the closed state.
        /// </returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Raise the ProcessingShipmentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessShipmentsActivity.ProcessingShipmentEvent, this, EventArgs.Empty);

                // Validate the properties at runtime
                this.ValidateRuntime();

                // Process Shipment now
                this.ProcessShipments();

                // Raise the ProcessedShipmentEvent event to the parent workflow or activity
                base.RaiseEvent(ProcessShipmentsActivity.ProcessedShipmentEvent, this, EventArgs.Empty);

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch
            {
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(ProcessShipmentsActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }

        /// <summary>
        /// Processes the shipments.
        /// </summary>
        private void ProcessShipments()
        {
            ShippingMethodDto methods = ShippingManager.GetShippingMethods(Thread.CurrentThread.CurrentUICulture.Name);

            OrderGroup order = OrderGroup;

            // request rates, make sure we request rates not bound to selected delivery method
            foreach (OrderForm form in order.OrderForms)
            {
                foreach(Shipment shipment in form.Shipments)
                {
                    ShippingMethodDto.ShippingMethodRow row = methods.ShippingMethod.FindByShippingMethodId(shipment.ShippingMethodId);

                    // If shipping method is not found, set it to 0 and continue
                    if (row == null)
                    {
                        Logger.Info(String.Format("Total shipment is 0 so skip shipment calculations."));
                        shipment.ShipmentTotal = 0;
                        continue;
                    }

                    // Check if package contains shippable items, if it does not use the default shipping method instead of the one specified
                    Logger.Debug(String.Format("Getting the type \"{0}\".", row.ShippingOptionRow.ClassName));
                    Type type = Type.GetType(row.ShippingOptionRow.ClassName);
                    if (type == null)
                    {
                        throw new TypeInitializationException(row.ShippingOptionRow.ClassName, null);
                    }
                    string message = String.Empty;
                    Logger.Debug(String.Format("Creating instance of \"{0}\".", type.Name));
                    IShippingGateway provider = (IShippingGateway)Activator.CreateInstance(type);

					List<LineItem> items = Shipment.GetShipmentLineItems(shipment);

                    Logger.Debug(String.Format("Calculating the rates."));
                    ShippingRate rate = provider.GetRate(row.ShippingMethodId, items.ToArray(), ref message);
                    if (rate != null)
                    {
                        Logger.Debug(String.Format("Rates calculated."));
                        shipment.ShipmentTotal = rate.Price;
                    }
                    else
                        Logger.Debug(String.Format("No rates has been found."));
                }
            }
        }
	}
}
