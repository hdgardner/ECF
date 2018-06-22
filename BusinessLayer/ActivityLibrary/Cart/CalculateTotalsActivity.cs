using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
	public partial class CalculateTotalsActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(CalculateTotalsActivity));

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
                return (OrderGroup)(base.GetValue(CalculateTotalsActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(CalculateTotalsActivity.OrderGroupProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateTotalsActivity"/> class.
        /// </summary>
        public CalculateTotalsActivity()
		{
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
                // Validate the properties at runtime
                this.ValidateRuntime();

                // Calculate order totals
                this.CalculateTotals();

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
        /// Calculates the totals.
        /// </summary>
        private void CalculateTotals()
        {
            decimal subTotal = 0m;
            //decimal discountTotal = 0m;
            decimal shippingTotal = 0m;
            decimal handlingTotal = 0m;
            decimal taxTotal = 0m;
            decimal total = 0m;

            // Get the property, since it is expensive process, make sure to get it once
            OrderGroup order = OrderGroup;

            // Calculate totals for OrderForms
            foreach (OrderForm form in order.OrderForms)
            {
                // Calculate totals for order form
                CalculateTotalsOrderForms(form);

                subTotal += form.SubTotal;
                //discountTotal += form.DiscountAmount;
                shippingTotal += form.ShippingTotal;
                handlingTotal += form.HandlingTotal;
                taxTotal += form.TaxTotal;
                total += form.Total;
            }

            // calculate OrderGroup totals
            order.SubTotal = subTotal;
            order.ShippingTotal = shippingTotal;
            order.TaxTotal = taxTotal;
            order.Total = total;
            order.HandlingTotal = handlingTotal;
        }

        /// <summary>
        /// Calculates the totals order forms.
        /// </summary>
        /// <param name="form">The form.</param>
        private void CalculateTotalsOrderForms(OrderForm form)
        {
            decimal subTotal = 0m;
            decimal discountTotal = 0m;
            decimal shippingDiscountTotal = 0m;
            decimal shippingTotal = 0m;

            foreach (LineItem item in form.LineItems)
            {
                item.ExtendedPrice = item.ListPrice * item.Quantity - item.LineItemDiscountAmount - item.OrderLevelDiscountAmount;
                subTotal += item.ListPrice * item.Quantity - item.LineItemDiscountAmount;
                //discountTotal += item.LineItemDiscountAmount;
                discountTotal += item.OrderLevelDiscountAmount;
            }

            foreach (Shipment shipment in form.Shipments)
            {
                shippingTotal += shipment.ShipmentTotal;
                shippingDiscountTotal += shipment.ShippingDiscountAmount;
            }

            discountTotal += shippingDiscountTotal;

            form.ShippingTotal = shippingTotal;
            form.DiscountAmount = discountTotal;
            form.SubTotal = subTotal;

            form.Total = subTotal + shippingTotal + form.TaxTotal - discountTotal;
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(CalculateTotalsActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
	}
}
