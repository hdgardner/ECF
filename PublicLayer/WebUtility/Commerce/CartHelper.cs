using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Engine;
using System.Collections.Specialized;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Commerce.Catalog.Objects;
using System.Globalization;
using System.Web;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;

namespace Mediachase.Cms.WebUtility.Commerce
{
    /// <summary>
    /// Cart helper class used to simplify cart operations.
    /// The cart is automatically cached in the current Http Context.
    /// </summary>
    public class CartHelper
    {
        /// <summary>
        /// Default name for the cart.
        /// </summary>
        public static string WishListName = "WishList";

        #region Data Members
        private Cart _Cart;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CartHelper"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CartHelper(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Guid userId = ProfileContext.Current.UserId;
            if (userId == Guid.Empty)
            {
                throw new InvalidOperationException("Invalid user id");
            }

            // Cache cart in the current context
            string cartKey = String.Format("Cart-{0}-{1}", name, userId.ToString());

            Cart cart = CMSContext.Current.Context.Items[cartKey] as Cart;

            if (cart == null)
            {
                cart = OrderContext.Current.GetCart(name, userId);

                if (String.IsNullOrEmpty(cart.CustomerName) || cart.CustomerName.Equals(ProfileContext.Anonymous, StringComparison.OrdinalIgnoreCase))
                    cart.CustomerName = ProfileContext.Current.CustomerName;

                CMSContext.Current.Context.Items[cartKey] = cart;
            }
            
            this._Cart = cart;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartHelper"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="userId">The user id.</param>
        public CartHelper(string name, Guid userId)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            // Cache cart in the current context
            string cartKey = String.Format("Cart-{0}-{1}", name, userId.ToString());

            Cart cart = CMSContext.Current.Context.Items[cartKey] as Cart;

            if (cart == null)
            {
                cart = OrderContext.Current.GetCart(name, userId);

                if (String.IsNullOrEmpty(cart.CustomerName) || cart.CustomerName.Equals(ProfileContext.Anonymous, StringComparison.OrdinalIgnoreCase))
                    cart.CustomerName = ProfileContext.Current.CustomerName;

                CMSContext.Current.Context.Items[cartKey] = cart;
            }

            this._Cart = cart;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartHelper"/> class.
        /// </summary>
        /// <param name="cart">The cart.</param>
        public CartHelper(Cart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException("cart");
            }
            this._Cart = cart;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the cart.
        /// </summary>
        /// <value>The cart.</value>
        public virtual Cart Cart
        {
            get
            {
                return this._Cart;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public virtual bool IsEmpty
        {
            get
            {
                foreach (OrderForm orderForm in this.Cart.OrderForms)
                {
                    if (orderForm.LineItems.Count > 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the line items.
        /// </summary>
        /// <value>The line items.</value>
        public virtual IEnumerable<LineItem> LineItems
        {
            get
            {
                foreach (OrderForm orderForm in this.Cart.OrderForms)
                {
                    foreach (LineItem lineItem in orderForm.LineItems)
                    {
                        yield return lineItem;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the primary address.
        /// </summary>
        /// <value>The primary address.</value>
        public virtual OrderAddress PrimaryAddress
        {
            get
            {
                if (String.IsNullOrEmpty(OrderForm.BillingAddressId))
                    return null;

                foreach (OrderAddress address in Cart.OrderAddresses)
                {
                    if (address.Name.Equals(OrderForm.BillingAddressId, StringComparison.OrdinalIgnoreCase))
                        return address;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the order form.
        /// </summary>
        /// <value>The order form.</value>
        public virtual OrderForm OrderForm
        {
            get
            {
                return GetOrderForm();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is address required.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is address required; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsAddressRequired
        {
            get
            {
                foreach (LineItem lineItem in this.LineItems)
                {
                    if (String.IsNullOrEmpty(lineItem.ShippingAddressId))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Finds the address by id.
        /// </summary>
        /// <param name="addressId">The address id.</param>
        /// <returns></returns>
        public virtual OrderAddress FindAddressById(string addressId)
        {
            if (String.IsNullOrEmpty(addressId))
                return null;

            foreach (OrderAddress address in Cart.OrderAddresses)
            {
                if (address.OrderGroupAddressId == Int32.Parse(addressId))
                    return address;
            }

            return null;
        }

        /// <summary>
        /// Finds the name of the address by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual OrderAddress FindAddressByName(string name)
        {
			if (name == null)
				return null;

            foreach (OrderAddress address in Cart.OrderAddresses)
            {
                if (address.Name.ToUpper() == name.ToUpper())
                    return address;
            }

            return null;
        }

        /// <summary>
        /// Gets the total number of items in the basket.
        /// </summary>
        /// <returns>the total number of items in the basket.</returns>
        public virtual decimal GetTotalItemCount()
        {
            decimal totalItemCount = 0;
            foreach (OrderForm orderForm in this.Cart.OrderForms)
            {
                foreach (LineItem lineItem in orderForm.LineItems)
                {
                    totalItemCount += lineItem.Quantity;
                }
            }
            return totalItemCount;
        }


        /// <summary>
        /// Gets the default OrderForm.
        /// </summary>
        /// <returns>the default OrderForm</returns>
        public virtual OrderForm GetOrderForm()
        {
            return this.GetOrderForm(null);
        }

        /// <summary>
        /// Gets the named OrderForm.
        /// </summary>
        /// <param name="orderFormName">The name of the OrderForm object to retrieve.</param>
        /// <returns>The named OrderForm.</returns>
        public virtual OrderForm GetOrderForm(string orderFormName)
        {
            if (String.IsNullOrEmpty(orderFormName))
            {
                orderFormName = Cart.DefaultName;
            }

            OrderForm orderForm = this.Cart.OrderForms[orderFormName];
            if (orderForm == null)
            {
                orderForm = new OrderForm();
                orderForm.Name = orderFormName;
                this.Cart.OrderForms.Add(orderForm);
            }
            return orderForm;
        }

        /// <summary>
        /// Deletes the current basket instance from the database.
        /// </summary>
        public virtual void Delete()
        {
            // Remove any reservations
            // Load existing usage Dto for the current order
            PromotionUsageDto usageDto = PromotionManager.GetPromotionUsageDto(0, Guid.Empty, this.Cart.OrderGroupId);

            // Clear all old items first
            if (usageDto.PromotionUsage.Count > 0)
            {
                foreach (PromotionUsageDto.PromotionUsageRow row in usageDto.PromotionUsage)
                {
                    row.Delete();
                }
            }

            // Save the promotion usage
            PromotionManager.SavePromotionUsage(usageDto);

            // Delete the cart
            this.Cart.Delete();
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public virtual void AddEntry(Entry entry)
        {
            AddEntry(entry, 1);
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        public virtual void AddEntry(Entry entry, decimal quantity)
        {
            Guid guid = ProfileContext.Current.UserId; // this should persist in the cookie
            this.Cart.BillingCurrency = CMSContext.Current.CurrencyCode;

            OrderForm orderForm = null;
            if (this.Cart.OrderForms.Count == 0) // create a new one
            {
                orderForm = new OrderForm();
                orderForm.Name = this.Cart.Name;
                this.Cart.OrderForms.Add(orderForm);
            }
            else // use first one
            {
                orderForm = this.Cart.OrderForms[0];
            }

            // Add line items
            LineItem lineItem = CreateLineItem(entry, quantity);

            // check if items already exist
            bool found = false;
            foreach (LineItem item in orderForm.LineItems)
            {
                if (item.CatalogEntryId == lineItem.CatalogEntryId)
                {
                    item.Quantity = lineItem.Quantity;
                    item.ExtendedPrice = lineItem.ListPrice;
                    found = true;
                    break;
                }
            }

            if (!found)
                orderForm.LineItems.Add(CreateLineItem(entry, quantity));

            // Save cart
            this.Cart.AcceptChanges();
        }

        /// <summary>
        /// Creates the line item.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        private LineItem CreateLineItem(Entry entry, decimal quantity)
        {
            LineItem lineItem = new LineItem();

            // If entry has a parent, add parents name
            if (entry.ParentEntry != null)
            {
                lineItem.DisplayName = String.Format("{0}: {1}", entry.ParentEntry.Name, entry.Name);
                lineItem.ParentCatalogEntryId = entry.ParentEntry.ID;
            }
            else
            {
                lineItem.DisplayName = entry.Name;
                lineItem.ParentCatalogEntryId = String.Empty;
            }

            lineItem.CatalogEntryId = entry.ID;
            Price price = StoreHelper.GetSalePrice(entry, quantity);
            lineItem.ListPrice = entry.ItemAttributes.ListPrice.Amount;
            lineItem.PlacedPrice = price.Amount;
            lineItem.ExtendedPrice = price.Amount;
            lineItem.MaxQuantity = entry.ItemAttributes.MaxQuantity;
            lineItem.MinQuantity = entry.ItemAttributes.MinQuantity;
            lineItem.Quantity = quantity;
            return lineItem;
        }

        /// <summary>
        /// Runs the workflow and generates the error message for all the warnings.
        /// </summary>
        /// <param name="name">The name.</param>
        public virtual void RunWorkflow(string name)
        {
            WorkflowResults results = this.Cart.RunWorkflow(name);
            object warnings = results.OutputParameters["Warnings"];
            if (warnings != null)
            {
                StringDictionary warningsList = warnings as StringDictionary;
                if (warningsList != null)
                {
                    foreach (string warning in warningsList.Values)
                    {
                        ErrorManager.GenerateError(warning);
                    }
                }
            }       
        }

        /// <summary>
        /// Resets this instance. Will clean up line items, remove payments and delete addresses.
        /// The cart needs to be saved in order for changes to be persisted.
        /// </summary>
        public virtual void Reset()
        {
            // Reset shopping cart
            foreach (LineItem lineItem in this.LineItems)
            {
                lineItem.ShippingAddressId = String.Empty;
                lineItem.ShippingMethodId = Guid.Empty;
                lineItem.ShippingMethodName = null;
            }

            foreach (OrderForm orderForm in Cart.OrderForms)
            {
                foreach (Payment orderPayment in orderForm.Payments)
                    orderPayment.Delete();

                foreach (Shipment shipment in orderForm.Shipments)
                    shipment.Delete();
            }

            foreach (OrderAddress orderAddress in Cart.OrderAddresses)
                orderAddress.Delete();

            Cart.AddressId = String.Empty;
        }
        #endregion
    }
}
