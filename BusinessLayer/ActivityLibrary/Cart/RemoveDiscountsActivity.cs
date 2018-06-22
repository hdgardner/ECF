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
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Commerce.Marketing.Dto;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
	public partial class RemoveDiscountsActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(RemoveDiscountsActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(RemoveDiscountsActivity));

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
                return (OrderGroup)(base.GetValue(RemoveDiscountsActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(RemoveDiscountsActivity.OrderGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public StringDictionary Warnings
        {
            get
            {
                return (StringDictionary)(base.GetValue(RemoveDiscountsActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(RemoveDiscountsActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveDiscountsActivity"/> class.
        /// </summary>
        public RemoveDiscountsActivity()
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

                // Remove discounts
                this.RemoveDiscounts();

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
        /// Removes the discounts.
        /// </summary>
        private void RemoveDiscounts()
        {
            // Process line item discounts first
            foreach (OrderForm form in this.OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    // First remove items
                    foreach (LineItemDiscount discount in lineItem.Discounts)
                    {
                        if (!discount.DiscountName.StartsWith("@") /*&& discount.DiscountId == -1*/) // ignore custom entries
                            discount.Delete();
                    }
                }
            }

            foreach (OrderForm form in this.OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    lineItem.OrderLevelDiscountAmount = 0;
                    lineItem.LineItemDiscountAmount = 0;
                    lineItem.ExtendedPrice = lineItem.ListPrice * lineItem.Quantity;
                }

                foreach (OrderFormDiscount discount in form.Discounts)
                {
                    if (!discount.DiscountName.StartsWith("@") /*&& discount.DiscountId == -1*/) // ignore custom entries
                        discount.Delete();
                }
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(RemoveDiscountsActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
	}
}
