using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    /// <summary>
    /// This activity will look into LineItems defined and will split items based on the address and shipping method id specified 
    /// into different shipments.
    /// </summary>
    public partial class ShipmentSplitActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(ShipmentSplitActivity));

        private const string EventsCategory = "Handlers";

        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public OrderGroup OrderGroup
        {
            get
            {
                return (OrderGroup)(base.GetValue(ShipmentSplitActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(ShipmentSplitActivity.OrderGroupProperty, value);
            }
        }

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            try
            {
                // Validate the properties at runtime
                this.ValidateRuntime();

                // Process Shipment now
                this.SplitShipments();

                // Retun the closed status indicating that this activity is complete.
                return ActivityExecutionStatus.Closed;
            }
            catch
            {
                // An unhandled exception occured.  Throw it back to the WorkflowRuntime.
                throw;
            }
        }

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

        private void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(ShipmentSplitActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }


        /// <summary>
        /// Splits the shipments according to address and shipping method specified.
        /// </summary>
        private void SplitShipments()
        {
            // TODO: 
            foreach (OrderForm form in OrderGroup.OrderForms)
            {
                SplitForm(form);
            }
        }

        private void SplitForm(OrderForm form)
        {
            int index = 0;
            foreach (LineItem item in form.LineItems)
            {
                Shipment itemShipment = null;
                string key = index.ToString();

                // Find appropriate shipment for item
                foreach (Shipment shipment in form.Shipments)
                {
                    if ((shipment.ShippingMethodId == item.ShippingMethodId) && (String.Compare(shipment.ShippingAddressId, item.ShippingAddressId) == 0))
                    {
                        // we found out match, exit
                        itemShipment = shipment;
                        //break;
                    }
                    else
                    {
                        // if shipment contains current LineItem, remove it from the shipment
                        if (shipment.LineItemIndexes.Contains(key))
                            shipment.LineItemIndexes.Remove(key);
                    }
                }

                // did we find any shipment?
                if (itemShipment == null)
                {
                    itemShipment = form.Shipments.AddNew();
                    itemShipment.ShippingAddressId = item.ShippingAddressId;
                    itemShipment.ShippingMethodId = item.ShippingMethodId;
                    itemShipment.ShippingMethodName = item.ShippingMethodName;
                }

                // Add item to the shipment
                //if (item.LineItemId == 0)
                //    throw new ArgumentNullException("LineItemId = 0");

                if (itemShipment.LineItemIndexes.Contains(key))
                    itemShipment.LineItemIndexes.Remove(key);

                //if (!itemShipment.LineItemIds.Contains(key))
                itemShipment.LineItemIndexes.Add(key);

                index++;
            }
        }

		public ShipmentSplitActivity()
		{
			InitializeComponent();
		}
	}
}
