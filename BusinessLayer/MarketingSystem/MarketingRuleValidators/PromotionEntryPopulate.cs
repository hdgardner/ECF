using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog.Objects;

namespace Mediachase.Commerce.Marketing.Validators
{
    public class PromotionEntryPopulate : IPromotionEntryPopulate
    {
        #region IPromotionEntryPopulate Members

        /// <summary>
        /// Populates the specified promotion entry with attribute values from the val object. Automatically adds all the meta fields.
        /// The objects supported are LineItem and Entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="val">The val. Can be LineItem or Entry.</param>
        public void Populate(ref PromotionEntry entry, object val)
        {
            if (!(val is LineItem) && !(val is Entry))
                throw new InvalidCastException("This interface only support types of LineItem or Entry");

            if (val is LineItem)
            {
                LineItem lineItem = val as LineItem;
                PopulateLineItem(ref entry, lineItem);
            }
            else
            {
                Entry catEntry = val as Entry;
                PopulateCatalogEntry(ref entry, catEntry);
            }
        }
        #endregion

        /// <summary>
        /// Populates the catalog entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="catEntry">The cat entry.</param>
        private void PopulateCatalogEntry(ref PromotionEntry entry, Entry catEntry)
        {
            entry.Quantity = 1;
            entry.Owner = catEntry;

            // Save line item id, so it is easier to distinguish to which item discount is applied
            entry["Id"] = catEntry.ID;

            if (catEntry.ItemAttributes != null)
            {
                entry["MinQuantity"] = catEntry.ItemAttributes.MinQuantity;
                entry["MaxQuantity"] = catEntry.ItemAttributes.MaxQuantity;
                entry["ExtendedPrice"] = catEntry.ItemAttributes.ListPrice;

                if (catEntry.ItemAttributes.Attribute != null)
                {
                    // Now populate all the custom attributes
                    foreach (ItemAttribute attr in catEntry.ItemAttributes.Attribute)
                    {
                        if (attr.Value != null && attr.Value.Length > 0)
                            entry[attr.Name] = attr.Value[0];
                    }
                }
            }

            if (catEntry.Inventory != null)
            {
                entry["AllowBackordersAndPreorders"] = catEntry.Inventory.AllowBackorder && catEntry.Inventory.AllowPreorder;
                entry["InStockQuantity"] = catEntry.Inventory.InStockQuantity;
                entry["PreorderQuantity"] = catEntry.Inventory.PreorderQuantity;
                entry["BackorderQuantity"] = catEntry.Inventory.BackorderQuantity;
                entry["InventoryStatus"] = catEntry.Inventory.InventoryStatus;
            }
        }

        /// <summary>
        /// Populates the line item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="lineItem">The line item.</param>
        private void PopulateLineItem(ref PromotionEntry entry, LineItem lineItem)
        {
            entry.Quantity = lineItem.Quantity;
            entry.Owner = lineItem;

            // Save line item id, so it is easier to distinguish to which item discount is applied
            entry["LineItemId"] = lineItem.LineItemId;
            entry["ShippingAddressId"] = lineItem.ShippingAddressId;

            entry["MinQuantity"] = lineItem.MinQuantity;
            entry["MaxQuantity"] = lineItem.MaxQuantity;
            entry["LineItemDiscountAmount"] = lineItem.LineItemDiscountAmount;
            entry["OrderLevelDiscountAmount"] = lineItem.OrderLevelDiscountAmount;
            entry["ShippingMethodName"] = lineItem.ShippingMethodName ?? string.Empty;
            entry["ExtendedPrice"] = lineItem.ExtendedPrice;
            entry["Description"] = lineItem.Description ?? string.Empty;
            entry["Status"] = lineItem.Status ?? string.Empty;
            entry["DisplayName"] = lineItem.DisplayName ?? string.Empty;
            entry["AllowBackordersAndPreorders"] = lineItem.AllowBackordersAndPreorders;
            entry["InStockQuantity"] = lineItem.InStockQuantity;
            entry["PreorderQuantity"] = lineItem.PreorderQuantity;
            entry["BackorderQuantity"] = lineItem.BackorderQuantity;
            entry["InventoryStatus"] = lineItem.InventoryStatus;

            // Now populate all the custom meta fields
            foreach (MetaField field in lineItem.MetaClass.MetaFields)
            {
                entry[field.Name] = lineItem[field];
            }
        }
    }
}
