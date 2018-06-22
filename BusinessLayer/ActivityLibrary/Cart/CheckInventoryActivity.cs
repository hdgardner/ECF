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

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
    public partial class CheckInventoryActivity : Activity
    {
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(CheckInventoryActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(CheckInventoryActivity));

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
                return (OrderGroup)(base.GetValue(CheckInventoryActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(CheckInventoryActivity.OrderGroupProperty, value);
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
                return (StringDictionary)(base.GetValue(CheckInventoryActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(CheckInventoryActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckInventoryActivity"/> class.
        /// </summary>
        public CheckInventoryActivity()
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
                this.ValidateItems();

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
        /// Validates the items.
        /// </summary>
        private void ValidateItems()
        {
            foreach (OrderForm form in OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    if (lineItem.CatalogEntryId != "0" && !String.IsNullOrEmpty(lineItem.CatalogEntryId) && !lineItem.CatalogEntryId.StartsWith("@")) // ignore custom entries
                    {
                        if (lineItem.InventoryStatus != InventoryStatus.Enabled)
                            continue;

                        // Check Inventory
                        // item exists with appropriate quantity
                        if (lineItem.InStockQuantity >= lineItem.Quantity)
                        {
                            continue;
                        }
                        else if (lineItem.InStockQuantity > 0) // there still exist items in stock
                        {
                            // check if we can backorder some items
                            if (lineItem.AllowBackordersAndPreorders)
                            {
                                if (lineItem.InStockQuantity + lineItem.BackorderQuantity >= lineItem.Quantity)
                                {
                                    continue;
                                }
                                else
                                {
                                    lineItem.Quantity = lineItem.InStockQuantity + lineItem.BackorderQuantity;
                                    Warnings.Add("LineItemQtyChanged-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" quantity has been changed, some items might be backordered.", lineItem.DisplayName));
                                    continue;
                                }
                            }
                            else
                            {
                                lineItem.Quantity = lineItem.InStockQuantity;
                                Warnings.Add("LineItemQtyChanged-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" quantity has been changed.", lineItem.DisplayName));
                                continue;
                            }
                        }
                        else if (lineItem.InStockQuantity == 0)
                        {
                            if (lineItem.AllowBackordersAndPreorders && lineItem.PreorderQuantity > 0)
                            {
                                if (lineItem.PreorderQuantity >= lineItem.Quantity)
                                    continue;
                                else
                                {
                                    lineItem.Quantity = lineItem.PreorderQuantity;
                                    Warnings.Add("LineItemQtyChanged-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" quantity has been changed.", lineItem.DisplayName));
                                    continue;
                                }
                            }
                        }


                        // Remove item if it reached this stage
                        Warnings.Add("LineItemRemoved-" + lineItem.Id.ToString(), String.Format("Item \"{0}\" has been removed from the cart because it is no longer available.", lineItem.DisplayName));

                        // Delete item
                        lineItem.Delete();
                    }
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(CheckInventoryActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
    }
}
