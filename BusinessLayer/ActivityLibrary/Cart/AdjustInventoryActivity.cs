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
    public partial class AdjustInventoryActivity : Activity
    {
        public static DependencyProperty OrderGroupProperty = DependencyProperty.Register("OrderGroup", typeof(OrderGroup), typeof(AdjustInventoryActivity));
        public static DependencyProperty WarningsProperty = DependencyProperty.Register("Warnings", typeof(StringDictionary), typeof(AdjustInventoryActivity));

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
                return (OrderGroup)(base.GetValue(AdjustInventoryActivity.OrderGroupProperty));
            }
            set
            {
                base.SetValue(AdjustInventoryActivity.OrderGroupProperty, value);
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
                return (StringDictionary)(base.GetValue(AdjustInventoryActivity.WarningsProperty));
            }
            set
            {
                base.SetValue(AdjustInventoryActivity.WarningsProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdjustInventoryActivity"/> class.
        /// </summary>
        public AdjustInventoryActivity()
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

                this.AdjustItems();

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
        /// Adjusts the items.
        /// </summary>
        private void AdjustItems()
        {
            /*
            foreach (OrderForm form in OrderGroup.OrderForms)
            {
                foreach (LineItem lineItem in form.LineItems)
                {
                    if (lineItem.CatalogEntryId != "0" && !String.IsNullOrEmpty(lineItem.CatalogEntryId) && !lineItem.CatalogEntryId.StartsWith("@")) // ignore custom entries
                    {
                        if (lineItem.InventoryStatus != InventoryStatus.Enabled)
                            continue;

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
             * */
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
                ValidationError validationError = ValidationError.GetNotSetValidationError(AdjustInventoryActivity.OrderGroupProperty.Name);
                validationErrors.Add(validationError);
            }
        }
    }
}
