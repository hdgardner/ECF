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
using System.Collections.Generic;
using System.Data;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Orders.Managers;
using System.Threading;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
	public partial class CalculateTaxActivity: Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(CalculateTaxActivity));

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
                return (OrderGroup)(base.GetValue(CalculateTaxActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(CalculateTaxActivity.OrderGroupProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateTaxActivity"/> class.
        /// </summary>
        public CalculateTaxActivity()
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

                // Calculate sale tax
                this.CalculateSaleTaxes();

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
        /// Calculates the sale taxes.
        /// </summary>
        private void CalculateSaleTaxes()
        {
            // Get the property, since it is expensive process, make sure to get it once
            OrderGroup order = OrderGroup;
            
            foreach (OrderForm form in order.OrderForms)
            {
                decimal totalTaxes = 0;
                foreach (Shipment shipment in form.Shipments)
                {
                    List<LineItem> items = Shipment.GetShipmentLineItems(shipment);

                    // Calculate sales and shipping taxes per items
                    foreach (LineItem item in items)
                    {
                        // Try getting an address
                        OrderAddress address = GetAddressByName(form, shipment.ShippingAddressId);
                        if (address != null) // no taxes if there is no address
                        {
                            // Try getting an entry
                            CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(item.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                            if (entryDto.CatalogEntry.Count > 0) // no entry, no tax category, no tax
                            {
                                CatalogEntryDto.VariationRow[] variationRows = entryDto.CatalogEntry[0].GetVariationRows();
                                if (variationRows.Length > 0)
                                {
                                    string taxCategory = CatalogTaxManager.GetTaxCategoryNameById(variationRows[0].TaxCategoryId);
                                    TaxValue[] taxes = OrderContext.Current.GetTaxes(Guid.Empty, taxCategory, Thread.CurrentThread.CurrentCulture.Name, address.CountryCode, address.State, address.PostalCode, address.RegionCode, String.Empty, address.City);

                                    if (taxes.Length > 0)
                                    {
                                        foreach (TaxValue tax in taxes)
                                        {
                                            if(tax.TaxType == TaxType.SalesTax)
                                                totalTaxes += item.ExtendedPrice * ((decimal)tax.Percentage / 100);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                form.TaxTotal = totalTaxes;
            }         
        }

        /// <summary>
        /// Gets the name of the address by name.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private OrderAddress GetAddressByName(OrderForm form, string name)
        {
            foreach (OrderAddress address in form.Parent.OrderAddresses)
            {
                if (address.Name.Equals(name))
                    return address;
            }

            return null;
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

        private void ValidateOrderProperties(ValidationErrorCollection validationErrors)
        {
            // Validate the To property
            if (this.OrderGroup == null)
            {
                ValidationError validationError = ValidationError.GetNotSetValidationError(CalculateTaxActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }

	}
}
