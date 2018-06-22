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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Profile;
using System.Collections.Specialized;
using Mediachase.Commerce.Catalog.Managers;
using System.Web.Security;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    /// <summary>
    /// This activity records the usage of the promotions so this information can be used to inforce various customer and application based limits.
    /// </summary>
	public partial class RecordPromotionUsageActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(RecordPromotionUsageActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(RecordPromotionUsageActivity));
        public static DependencyProperty UsageStatusProperty = DependencyProperty.Register("UsageStatus", typeof(PromotionUsageStatus), typeof(RecordPromotionUsageActivity));

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
                return (OrderGroup)(base.GetValue(RecordPromotionUsageActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(RecordPromotionUsageActivity.OrderGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the usage status.
        /// </summary>
        /// <value>The usage status.</value>
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        public PromotionUsageStatus UsageStatus
        {
            get
            {
                return (PromotionUsageStatus)(base.GetValue(RecordPromotionUsageActivity.UsageStatusProperty));
            }
            set
            {
                base.SetValue(RecordPromotionUsageActivity.UsageStatusProperty, value);
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
                return (StringDictionary)(base.GetValue(RecordPromotionUsageActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(RecordPromotionUsageActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordPromotionUsageActivity"/> class.
        /// </summary>
        public RecordPromotionUsageActivity()
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

                // Calculate order discounts
                this.RecordPromotions();

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
        /// Records the promotions.
        /// 
        /// Step 1: Load the existing usage that is related to the current order (if any).
        /// Step 2: Record/update the usage of lineitem, order and shipment level discounts.
        /// 
        /// The CustomerId can be taken from the Current Order.CustomerId.
        /// </summary>
        private void RecordPromotions()
        {
            List<Discount> discounts = new List<Discount>();

            OrderGroup group = this.OrderGroup;

            // if the order has been just added, skip recording the discounts
            if (group.ObjectState == Mediachase.MetaDataPlus.MetaObjectState.Added)
                return;

            PromotionUsageStatus status = this.UsageStatus;

            foreach (OrderForm form in group.OrderForms)
            {
                // Add order level discounts
                foreach (Discount discount in form.Discounts)
                {
                    discounts.Add(discount);
                }

                // Add lineitem discounts
                foreach (LineItem item in form.LineItems)
                {
                    foreach (Discount discount in item.Discounts)
                    {
                        discounts.Add(discount);
                    }
                }

                // Add shipping discounts
                foreach (Shipment shipment in form.Shipments)
                {
                    foreach (Discount discount in shipment.Discounts)
                    {
                        discounts.Add(discount);
                    }
                }
            }

            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, group.OrderGroupId);

            // Clear all old items first
            if (usageDto.PromotionUsage.Count > 0)
            {
                foreach (PromotionUsageDto.PromotionUsageRow row in usageDto.PromotionUsage)
                {
                    row.Delete();
                }
            }

            // Now process the discounts
            foreach (Discount discount in discounts)
            {
                // we only record real discounts that exist in our database
                if (discount.DiscountId <= 0)
                    continue;

                PromotionUsageDto.PromotionUsageRow row = usageDto.PromotionUsage.NewPromotionUsageRow();
                row.CustomerId = group.CustomerId;
                row.LastUpdated = DateTime.UtcNow;
                row.OrderGroupId = group.OrderGroupId;
                row.PromotionId = discount.DiscountId;
                row.Status = status.GetHashCode();
                row.Version = 1; // for now version is always 1

                usageDto.PromotionUsage.AddPromotionUsageRow(row);
            }

            // Save the promotion usage
            PromotionManager.SavePromotionUsage(usageDto);
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(ValidateLineItemsActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
	}
}
