using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Profile;

namespace NWTD.Orders {
	
	/// <summary>
	/// NWTD has a lot of things it does with carts that go beyond ECF's default functionality.
	/// A Utility Class that contains static methods and constants for workign with NWTD-specific funtionality such as calculating cart totals and and creating mutliple user carts.
	/// </summary>
	public class Cart {

		public const string BILLING_ADDRESS_NAME = "billing";
		public const string SHIPPING_ADDRESS_NAME = "shipping";
		public const string CART_REMINDER_KEY = "cartreminder";
		
		public enum CART_STATUS { SUBMITTED, OPEN };

		/// <summary>
		/// Returns whether the current customer shoudl receive prompt to choose an active cart. 
		/// This wil be based on whether the user has established a new session
		/// </summary>
		public static bool Reminder {
			get {

				//return true;
				//TODO:uncomment this if its decided to not show the select cart modal if there's only one cart...
				// if (CurrentCustomerCarts.Count < 2) return false;

				if (System.Web.HttpContext.Current.Session[CART_REMINDER_KEY] == null) return true;

				return (bool)System.Web.HttpContext.Current.Session[CART_REMINDER_KEY]; 
			}
			set { System.Web.HttpContext.Current.Session[CART_REMINDER_KEY] = value; }
		}

		/// <summary>
		/// Returns a list of active Carts the current customer has created.
		/// </summary>
		public static List<Mediachase.Commerce.Orders.Cart> CurrentCustomerCarts {
			get {
				return GetUserCarts(ProfileContext.Current.UserId, true);
			}
		}

		/// <summary>
		/// Returns a list of Carts associated with a user.
		/// </summary>
		/// <param name="UserID">The ID of the user</param>
		/// <param name="ActiveOnly">Whether to get only active carts</param>
		/// <returns></returns>
		public static List<Mediachase.Commerce.Orders.Cart> GetUserCarts(Guid UserID, bool ActiveOnly) {

			if (!ActiveOnly) {
				return Mediachase.Commerce.Orders.Cart.LoadByCustomer(UserID)
					.ToArray()
					.ToList<Mediachase.Commerce.Orders.Cart>();
			}

			return Mediachase.Commerce.Orders.Cart.LoadByCustomer(UserID)
				.Cast<Mediachase.Commerce.Orders.Cart>()
				.Where(c => c.Status.Equals(global::NWTD.Orders.Cart.CART_STATUS.OPEN.ToString()))
				.ToList<Mediachase.Commerce.Orders.Cart>();

		}

		/// <summary>
		/// Calculates the total of a line item, including sales and gratis
		/// </summary>
		/// <param name="lineItem">The line item to total</param>
		/// <returns>The total in decimal form</returns>
		public static decimal LineItemTotal(LineItem lineItem) {
			if (lineItem == null) return 0m;
			decimal gratis = (lineItem["Gratis"] != null) ? (decimal)lineItem["Gratis"] : 0m;
			string currencyCode = lineItem.Parent.Parent.BillingCurrency;
			decimal pricePerItem =lineItem.Quantity > 0 ?  lineItem.ExtendedPrice / lineItem.Quantity : 0;
			return pricePerItem * (lineItem.Quantity - gratis);

		}

		/// <summary>
		/// Calculates the subtotal of a cart, including and gratis
		/// </summary>
		/// <param name="cart">The cart for which the total is being calculated</param>
		/// <returns></returns>
		public static decimal CartTotal(Mediachase.Commerce.Orders.Cart cart) {
			return CartTotal(cart, false, false);
		}

		/// <summary>
		/// Calculates the total of a cart, including and gratis
		/// </summary>
		/// <param name="cart">The cart for which the total is being calculated</param>
		/// <param name="IncludeTax">Whether to include tax in the caculation</param>
		/// <param name="IncludeShipping">Whether to include shipping in the calculation</param>
		/// <returns></returns>
		public static decimal CartTotal(Mediachase.Commerce.Orders.Cart cart, bool IncludeTax, bool IncludeShipping) {
			decimal total = 0m;
			CartHelper helper = new CartHelper(cart);
			foreach (LineItem lineItem in helper.LineItems) {
				total += Cart.LineItemTotal(lineItem);
			}

			if (IncludeShipping) total += CartShippingCharge(cart);
			if (IncludeTax) total += (CartTax(cart));

			return total;
		}

		/// <summary>
		/// Calculates the estimated tax of the cart, based on the first line item found that has a shipping address
		/// </summary>
		/// <param name="cart">The cart for which the tax is being estimated</param>
		/// <returns></returns>
		public static decimal CartTax(Mediachase.Commerce.Orders.Cart cart) {
			CartHelper helper = new CartHelper(cart);
			OrderAddress shippingAddress = FindCartShippingAddress(cart);
			decimal taxRate = 0m;	
			bool isFreightTaxable = true;
			Account account = Mediachase.Commerce.Profile.Account.LoadByPrincipalId(helper.Cart.CustomerId);

			if (account.Organization.GetBool("IsTaxExempt")) { 
				return 0m;
			}

			//error check just in case the meta fields don't exist
			try {
				decimal.TryParse(shippingAddress["TaxRate"].ToString(), out taxRate);
				bool.TryParse(shippingAddress["IsFreightTaxable"].ToString(), out isFreightTaxable);
			} catch (Exception ex) {
				return 0m;
			}

			return CartTotal(cart, false, isFreightTaxable) * (taxRate/100);
		}

		/// <summary>
		/// Calculate the shipping charge based on the cart's owner's organization
		/// </summary>
		/// <param name="cart">The cart being examined</param>
		/// <returns></returns>
		public static decimal CartShippingCharge(Mediachase.Commerce.Orders.Cart cart) {
			Account account = Mediachase.Commerce.Profile.Account.LoadByPrincipalId(cart.CustomerId);

			decimal shipRate = 0m;
			decimal shipmentMinimumCharge = 0m;
			decimal shipmentFlatRate = 0m;
			if (account.Organization != null) {
				//error check just in case the meta fields don't exist
				try {
					decimal.TryParse(account.Organization["ShipRate"].ToString(), out shipRate);
					decimal.TryParse(account.Organization["ShipMinCharge"].ToString(), out shipmentMinimumCharge);
					decimal.TryParse(account.Organization["ShipFlatCharge"].ToString(), out shipmentFlatRate);
				} catch (Exception ex) {
					return 0m;
				}
			}

			if (shipmentFlatRate > 0) return shipmentFlatRate * CartTotal(cart);
			
			decimal total = 0m;

			total = shipRate * CartTotal(cart);
			if (total < shipmentMinimumCharge) total = shipmentMinimumCharge;

			return total;
		}

		/// <summary>
		/// Searches a cart's line items for a shipping address. Returns the first address found.
		/// </summary>
		/// <param name="cart">The cart being examined</param>
		/// <returns></returns>
		public static OrderAddress FindCartShippingAddress(Mediachase.Commerce.Orders.Cart cart) { 
			CartHelper helper = new CartHelper(cart);

			foreach (LineItem lineItem in helper.LineItems) {
				if (!String.IsNullOrEmpty(lineItem.ShippingAddressId)) {
					OrderAddress address = helper.FindAddressByName(lineItem.ShippingAddressId);
					if (address != null) return address;
				}
			}

			return null;
		}

		/// <summary>
		/// Creates a new cart for the supplied customer with the supplied name. 
		/// This method does some additional things beyond ECF's cart creation code, 
		/// so it's very importand that NWTD carts are created this way.
		/// </summary>
		/// <param name="Account"></param>
		/// <param name="CartName"></param>
		/// <returns></returns>
		public static Mediachase.Commerce.Orders.Cart CreateCart(Account Account, String CartName) {
			if (Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(Account.PrincipalId, CartName) != null) throw new Exception("A cart for this customer with this name already exists");
			//create the cart
			Mediachase.Commerce.Orders.Cart cartToAdd = Mediachase.Commerce.Orders.OrderContext.Current.GetCart(CartName, Account.PrincipalId);
			cartToAdd.CustomerName = Mediachase.Commerce.Profile.Account.LoadByPrincipalId(Account.PrincipalId).Name;
			cartToAdd.OrderForms.Add(new Mediachase.Commerce.Orders.OrderForm() { Name = CartName }); //We need to give it a name. So we can call the GetOrderForm mehod on the ECF's CartHelper class and actually return somethiung
			cartToAdd.Status = NWTD.Orders.Cart.CART_STATUS.OPEN.ToString();
			cartToAdd.AcceptChanges();

			return cartToAdd;
		}

        /// <summary>
        /// Generates an Order number from the cart following NWTDs custom conventions
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        public static string GenerateOrderNumber(Mediachase.Commerce.Orders.Cart cart)  {
            
            //string num = new Random().Next(100, 999).ToString();
            //return String.Format("{0}{1}{2}", NWTD.Profile.CustomerDepository.ToString(), cart.OrderGroupId, num);
        
        //Replaced above two lines with the following logic for shorter WebConfirmation Number (Heath Gardner 08/19/13)
            //Get current customer's depository (e.g. NWTD or MSSD)
            string custDepo = NWTD.Profile.CustomerDepository.ToString();
           
            //Convert customer's depository into two character string (e.g. NW, MS, or ZZ for unknowns)
            if (custDepo == "NWTD")
                custDepo = "NW";
            else if (custDepo == "MSSD")
                custDepo = "MS";
            else
                custDepo = "ZZ";
            
            //Build and return the WebConfirmation Number
            return String.Format("{0}{1}", custDepo, cart.OrderGroupId);
            
        }

		/// <summary>
		/// Assigns totals to a cart.
		/// </summary>
		/// <param name="Cart"></param>
		public static void AssignTotals(ref Mediachase.Commerce.Orders.Cart Cart){
			Cart.SubTotal = CartTotal(Cart, false, false);
			Cart.ShippingTotal = CartShippingCharge(Cart);
			Cart.TaxTotal = CartTax(Cart);
			Cart.Total = Cart.SubTotal + Cart.ShippingTotal + Cart.TaxTotal;
		}

		/// <summary>
		/// Finds out whether a cart can be edited (submitted carts can't).
		/// </summary>
		/// <param name="Cart">The cart to investigate</param>
		/// <returns></returns>
		public static bool CartCanBeEdited(Mediachase.Commerce.Orders.Cart Cart) {
			return Cart.Status.ToString().Equals(NWTD.Orders.Cart.CART_STATUS.OPEN.ToString());
		}

	}
}
