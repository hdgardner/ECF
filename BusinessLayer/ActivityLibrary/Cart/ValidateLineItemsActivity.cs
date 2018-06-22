using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.Runtime;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Workflow.Activities.Cart
{
	public partial class ValidateLineItemsActivity : Activity
	{
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(ValidateLineItemsActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(ValidateLineItemsActivity));

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
                return (OrderGroup)(base.GetValue(ValidateLineItemsActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(ValidateLineItemsActivity.OrderGroupProperty, value);
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
                return (StringDictionary)(base.GetValue(ValidateLineItemsActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(ValidateLineItemsActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateLineItemsActivity"/> class.
        /// </summary>
        public ValidateLineItemsActivity()
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
			CatalogRelationDto relationDto = null;
			CatalogDto catalogDto = null;
			int oldCatalogId = 0;

            foreach (OrderForm form in OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    if (lineItem.CatalogEntryId != "0" && !String.IsNullOrEmpty(lineItem.CatalogEntryId) && !lineItem.CatalogEntryId.StartsWith("@")) // ignore custom entries
                    {
                        CatalogEntryDto entryDto = CatalogContext.Current.GetCatalogEntryDto(lineItem.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

						if (entryDto.CatalogEntry.Count > 0)
						{
							CatalogEntryDto.CatalogEntryRow entryRow = entryDto.CatalogEntry[0];
                            if (entryRow.IsActive && entryRow.StartDate < FrameworkContext.Current.CurrentDateTime && entryRow.EndDate > FrameworkContext.Current.CurrentDateTime)
							{
								if (oldCatalogId != entryRow.CatalogId)
								{
									// load these Dtos only if we haven't loaded them before
									catalogDto = CatalogContext.Current.GetCatalogDto(entryRow.CatalogId);
									relationDto = CatalogContext.Current.GetCatalogRelationDto(entryRow.CatalogId, 0, 0, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.CatalogEntry));
									oldCatalogId = entryRow.CatalogId;
								}

								// check if catalog is visible
                                if (catalogDto.Catalog.Count > 0 && catalogDto.Catalog[0].IsActive && catalogDto.Catalog[0].StartDate < FrameworkContext.Current.CurrentDateTime && catalogDto.Catalog[0].EndDate > FrameworkContext.Current.CurrentDateTime)
                                {
                                    // populate item
                                    lineItem.Catalog = catalogDto.Catalog[0].Name;

                                    // get parent entry
                                    if (relationDto.CatalogEntryRelation.Count > 0)
                                    {
                                        CatalogRelationDto.CatalogEntryRelationRow[] entryRelationRows = (CatalogRelationDto.CatalogEntryRelationRow[])relationDto.CatalogEntryRelation.Select(String.Format("ChildEntryId={0}", entryRow.CatalogEntryId));
                                        if (entryRelationRows.Length > 0)
                                        {
                                            CatalogEntryDto parentEntryDto = CatalogContext.Current.GetCatalogEntryDto(entryRelationRows[0].ParentEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                            if (parentEntryDto.CatalogEntry.Count > 0)
                                                lineItem.ParentCatalogEntryId = parentEntryDto.CatalogEntry[0].Code;
                                        }
                                    }

                                    // Inventory info
                                    CatalogEntryDto.InventoryRow invRow = entryRow.InventoryRow;
                                    if (invRow != null)
                                    {
                                        lineItem.AllowBackordersAndPreorders = invRow.AllowBackorder | invRow.AllowPreorder;
                                        lineItem.BackorderQuantity = invRow.BackorderQuantity;
                                        lineItem.InStockQuantity = invRow.InStockQuantity;
                                        lineItem.InventoryStatus = invRow.InventoryStatus;
                                        lineItem.PreorderQuantity = invRow.PreorderQuantity;
                                    }

                                    CatalogEntryDto.VariationRow[] varRows = entryRow.GetVariationRows();

                                    if (varRows.Length > 0)
                                    {
                                        CatalogEntryDto.VariationRow varRow = varRows[0];

                                        lineItem.MaxQuantity = varRow.MaxQuantity;
                                        lineItem.MinQuantity = varRow.MinQuantity;

                                        Account account = ProfileContext.Current.GetAccount(lineItem.Parent.Parent.CustomerId);
                                        decimal? newPrice = GetItemPrice(entryRow, lineItem, account);
                                        
                                        if (newPrice == null)
                                            newPrice = varRow.ListPrice;

                                        // Check the price changes if any
                                        if (lineItem.ListPrice != (decimal)newPrice)
                                        {
                                            Warnings.Add("LineItemPriceChange-" + form.LineItems.IndexOf(lineItem).ToString(), String.Format("Price for \"{0}\" has been changed from {1:c} to {2:c}.", lineItem.DisplayName, lineItem.ListPrice, newPrice));
                                            lineItem.ListPrice = (decimal)newPrice;
											//lineItem.PlacedPrice = lineItem.ListPrice;
                                        }
                                    }                                    

                                    continue;
                                }
                                else
                                {
                                    // Go to remove
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
        /// Gets the item price.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        private decimal? GetItemPrice(CatalogEntryDto.CatalogEntryRow entry, LineItem lineItem, Account account)
        {
            decimal? price = null;

            CatalogEntryDto.SalePriceRow[] priceRows = entry.GetSalePriceRows();

            string currencyCode = lineItem.Parent.Parent.BillingCurrency;

            // Determine price by using tiers
            if (priceRows.Length > 0)
            {
                foreach (CatalogEntryDto.SalePriceRow priceRow in priceRows)
                {
                    if (!currencyCode.Equals(priceRow.Currency))
                        continue;

                    // Check inventory first
                    if (priceRow.MinQuantity > lineItem.Quantity)
                        continue; // didn't meet min quantity requirements

                    // Check dates
                    if (priceRow.StartDate > FrameworkContext.Current.CurrentDateTime || priceRow.EndDate < FrameworkContext.Current.CurrentDateTime)
                        continue; // falls outside of acceptable range

                    if ((SaleType.TypeKey)priceRow.SaleType == SaleType.TypeKey.AllCustomers) // no need to check, applies to everyone
                    {
                        if (price == null || price > priceRow.UnitPrice)
                            price = priceRow.UnitPrice;
                    }
                    else if ((SaleType.TypeKey)priceRow.SaleType == SaleType.TypeKey.Customer) // check if it applies to a customer
                    {
                        if (account == null)
                            continue;

                        MembershipUser user = Membership.GetUser((object)account.ProviderKey);

                        if (user == null)
                            continue;

                        // Check sale code
                        if (user.UserName != priceRow.SaleCode)
                            continue; // didn't match

                        if (price == null || price > priceRow.UnitPrice)
                            price = priceRow.UnitPrice;
                    }
                    else if ((SaleType.TypeKey)priceRow.SaleType == SaleType.TypeKey.CustomerPriceGroup) // check if it applies to a customer
                    {
                        if (account == null)
                            continue;

                        // Check sale code
                        if (account.CustomerGroup != priceRow.SaleCode)
                            continue; // didn't match

                        if (price == null || price > priceRow.UnitPrice)
                            price = priceRow.UnitPrice;
                    }
                    else //NEW CODE
                    {
                        if (priceRow.SaleType.ToString() != string.Empty)
                        {
                            string salePriceTypeKey = string.Empty;

                            //get the sale type name
                            foreach (SalePriceTypeDefinition element in CatalogConfiguration.Instance.SalePriceTypes)
                            {
                                if (element.Value == priceRow.SaleType)
                                {
                                    salePriceTypeKey = element.Key;
                                    break;
                                }
                            }

                            if (HttpContext.Current.Session[salePriceTypeKey] != null)
                            {
                                // Check sale code
                                if ((string)HttpContext.Current.Session[salePriceTypeKey] != priceRow.SaleCode)
                                    continue; // didn't match

                                if (price == null || price > priceRow.UnitPrice)
                                    price = priceRow.UnitPrice;
                            }
                        }
                    } //END NEW CODE
                }
            }

            return price;
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
