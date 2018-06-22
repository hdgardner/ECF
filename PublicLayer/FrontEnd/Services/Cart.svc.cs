using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Orders;
using System.Collections.Generic;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Catalog.Managers;
using System.Text.RegularExpressions;

namespace Mediachase.Cms.Website.Services {
	
	/// <summary>
	/// A service for interacting with customer carts.
	/// </summary>
	[ServiceContract(Namespace = "NWTD")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class Cart {

		/// <summary>
		/// Updates the name of the cart
		/// </summary>
		/// <param name="cartName">The name of the cart to update</param>
		/// <param name="newName">The new name for the cart</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
		public CartResponse Update(string cartName, string newName) {
			
			Guid userId = ProfileContext.Current.UserId;
			
			var response = new CartResponse();
			Mediachase.Commerce.Orders.Cart test = Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(userId, newName);
            if (!CartNameIsValid(newName)) {
                response.Message = "Invalid Wish List name! The Wish List name may only contain letters, numbers, spaces, and underscores";
				response.Status = CartResponseStatus.ERROR;
            }
            else if (CartNameIsDuplicate(userId,newName)) {
				//uh oh, there's already a cart by this name!
				response.Status = CartResponseStatus.ERROR;
				response.Message = string.Format("A Wish List with the name {0} already exists", newName);
			} else {
				CartHelper helper = new CartHelper(cartName, userId);
				helper.Cart.Name = newName;
                if (helper.Cart.OrderForms[cartName] != null) {
                    OrderForm childForm = helper.Cart.OrderForms[cartName];
                    childForm.Name = newName;
                    childForm.AcceptChanges();
                }
				helper.Cart.AcceptChanges();
				if (NWTD.Profile.ActiveCart.Equals(cartName)) NWTD.Profile.ActiveCart = newName; //if it's the active cart, we need to update that information in state
				response.Message = "Successfully updated the Wish List.";
			}
			return response;
			
		}

		/// <summary>
		/// Deletes a cart for the current user
		/// </summary>
		/// <param name="cartName">The name of the cart to be deleted</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
		public CartResponse Delete(string cartName) {
			
			Guid userId = ProfileContext.Current.UserId;
			
			CartHelper helper = new CartHelper(cartName, userId);
			var response = new CartResponse();

			helper.Cart.Delete();
			helper.Cart.AcceptChanges();

			return response;

		}

		/// <summary>
		/// Creates a cart for the current user
		/// </summary>
		/// <param name="cartName">the name of the cart to be created</param>
        /// <param name="activate">whether the new cart should be active</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
		public CartResponse Create(string cartName,bool activate) {
			var response = new CartResponse();
			Guid userId = ProfileContext.Current.UserId;
			if(!CartNameIsValid(cartName)){
                response.Message = "Invalid Wish List name! The Wish List name may only contain letters, numbers, spaces, and underscores";
				response.Status = CartResponseStatus.ERROR;
			}
            else if(CartNameIsDuplicate(userId,cartName)){
                response.Message = "Invalid Wish List name! There is already a Wish List with this name";
				response.Status = CartResponseStatus.ERROR;
            }
			else{
				NWTD.Orders.Cart.CreateCart(ProfileContext.Current.Profile.Account, cartName);
                if (activate) {
                    NWTD.Profile.ActiveCart = cartName;
                }
				response.Message = "New Wish List Successfully Created";
			}
			return response;
		}

		/// <summary>
		/// Copies a cart for the current user
		/// </summary>
		/// <param name="cartName">The name of the cart to copy</param>
		/// <param name="newName">The name for the new cart</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
        public CartResponse Copy(string cartName, string newName, bool activate){
			
			var response = new CartResponse(); //the response for the operation
			
			if (string.IsNullOrEmpty(newName)) {
				newName = string.Format("Copy of {0}", cartName);
			}
			
			Guid userId = ProfileContext.Current.UserId;

			//make sure there isn't already a cart with the same name
			if (!CartNameIsValid(newName)) {
                response.Message = "Invalid Wish List name! The Wish List name may only contain letters, numbers, spaces, and underscores";
				response.Status = CartResponseStatus.ERROR;
			} 
            else if(CartNameIsDuplicate(userId, newName)){
                response.Message = "Invalid Wish List name! There is already a Wish List with this name";
				response.Status = CartResponseStatus.ERROR;
            }
            else {
				CartHelper originalCartHelper = new CartHelper(cartName, userId);
				
				//create the new cart
				Mediachase.Commerce.Orders.Cart cartToAdd = NWTD.Orders.Cart.CreateCart(ProfileContext.Current.Profile.Account, newName);

				//now, we'll need a CartHelper to start adding the lines
				CartHelper newCartHelper = new CartHelper(cartToAdd);
				
				//now add all the same line items to the new cart, copying any relevant metadata (gratis and quantity)
				foreach (LineItem lineItem in originalCartHelper.LineItems) {
					//get the entry
					Entry entry = CatalogContext.Current.GetCatalogEntry(lineItem.CatalogEntryId, new Mediachase.Commerce.Catalog.Managers.CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
					if (entry != null) newCartHelper.AddEntry(entry, lineItem.Quantity);
					
					//get the item we just added and set its gratis
					LineItem addedItem = newCartHelper.LineItems.Single(li => li.CatalogEntryId == entry.ID);
					addedItem["Gratis"] = lineItem["Gratis"];
					//addedItem.ShippingAddressId = lineItem.ShippingAddressId;
					newCartHelper.RunWorkflow("CartValidate");
					cartToAdd.AcceptChanges();
				}

				//save the changes
				cartToAdd.AcceptChanges();

                if (activate){
                    NWTD.Profile.ActiveCart = cartToAdd.Name;
                    response.Message = "Wish List succesfully copied and made active";
                }
                else
                    response.Message = "Wish List succesfully copied";
			}
			return response;
		}

		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		public CartResponse Select(string cartName) {
			CartResponse response = new CartResponse();
			if (Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(ProfileContext.Current.UserId, cartName) != null) {
				NWTD.Profile.ActiveCart = cartName;
				global::NWTD.Orders.Cart.Reminder = false;
				response.Status = CartResponseStatus.OK;
                response.Message = string.Format("{0} is now the active Wish List.", cartName);
			}
			else {
				response.Status = CartResponseStatus.ERROR;
                response.Message = string.Format("There is no Wish List named {0} for this user.", cartName);
			}
			return response;
		}

		/// <summary>
		/// Adds a catalog item to a cart for the current user
		/// </summary>
		/// <param name="item">The item to add</param>
		/// <param name="cartname">The name of the cart to which the item shall be added</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
		public CartResponse AddItem(CartItem item, string cartname) {

			NWTD.Profile.EnsureCustomerCart();
			
			CartResponse response = new CartResponse();
			if (string.IsNullOrEmpty(cartname)) cartname = global::NWTD.Profile.ActiveCart;
			CartHelper ch = new CartHelper(cartname);
			Entry itemToAdd = CatalogContext.Current.GetCatalogEntry(item.Code, new Mediachase.Commerce.Catalog.Managers.CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
			
			if (itemToAdd == null) {
				response.Status = CartResponseStatus.ERROR;
				response.Message = String.Format("The item with ISBN number {0} does not exist.",item.Code);
				return response;
			}

			//if this item is already in the cart, get it's quantity, then add it to the entered quantity
			decimal addedQuantity = item.Quantity;
			LineItem existingItem = ch.LineItems.SingleOrDefault(li => li.CatalogEntryId == itemToAdd.ID);
			
			//this is from back when we used let people select quantities when adding to the cart
			//if (existingItem != null) item.Quantity += existingItem.Quantity;

			if (existingItem != null) {
                response.Message = string.Format("{0} is already in your Wish List and cannot be added a second time.  If you need more, please return to your Wish List and increase the quantity of the existing item.", item.Code);
				response.Status = CartResponseStatus.WARNING;
				return response;
			}

			try {
				ch.AddEntry(itemToAdd, item.Quantity);
                response.Message = String.Format("Added {0} item(s) with ISBN number {1} to the Wish List ({2}).", addedQuantity.ToString(), item.Code, cartname);
				var addedItem = new CartItem();
				addedItem.Code = item.Code;
				addedItem.Quantity = item.Quantity;
				response.ItemsAdded.Add(addedItem);
                
                //too bad this doesn't work...
                //ch.Cart.Modified = DateTime.Now;
                //ch.Cart.AcceptChanges();

				return response;
			} catch (Exception ex) {
				response.Status = CartResponseStatus.ERROR;
				response.Message = ex.Message;
				return response;
			}
		}

		/// <summary>
		/// Adds multiple items to the cart
		/// </summary>
		/// <param name="items">A list of items to add</param>
		/// <param name="cartname">The name of the cart to which the items shall be added</param>
		/// <returns></returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)] //the entire parameter must be serialized to a string (no individual raw POST values)
		public CartResponse AddItems(List<CartItem> items, string cartname) {
			
			CartResponse response = new CartResponse();
			List<string> messages = new List<string>();

			foreach (CartItem item in items) {
				CartResponse addResponse = this.AddItem(item, cartname);
				if (addResponse.Status == CartResponseStatus.ERROR || addResponse.Status == CartResponseStatus.WARNING) {
					messages.Add(string.Format("The item with ISBN number {0} could not be added to your Wish List. {1} <br />", item.Code, addResponse.Message));
					response.Status = CartResponseStatus.WARNING;
				} else {
					response.ItemsAdded.Add(item);
					messages.Add(response.Message);
				}
			}

			response.Message = string.Join(Environment.NewLine, messages.ToArray());

			return response; 
		}

        /// <summary>
        /// Makes sure the supplied cart name only contains letters, numbers, spaces, and underscores
        /// </summary>
        /// <param name="CartName"></param>
        /// <returns></returns>
		public static bool CartNameIsValid(string CartName) {
           return  Regex.IsMatch(CartName, @"^[a-zA-Z0-9_ ]+$");
		}

        /// <summary>
        /// Finds out if the name is available for a new cart
        /// </summary>
        /// <param name="UserId">The ID of the user for whom the cart would be created</param>
        /// <param name="CartName">The name of the cart to check</param>
        /// <returns>True if the name is availalbe, False if it is not.</returns>
        public static bool CartNameIsDuplicate(Guid UserId, string CartName) {
            Mediachase.Commerce.Orders.Cart test = Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(UserId, CartName);
            if (test != null) {
                //uh oh, there's already a cart by this name!
                return true;
            }
            return false;
        }

		/// <summary>
		/// A simlple class that represents the item to be added to the cart
		/// </summary>
		public class CartItem {
			/// <summary>
			/// The internal code of the item
			/// </summary>
			public string Code;
			/// <summary>
			/// The number of items
			/// </summary>
			public decimal Quantity = 0;
		}


		public enum CartResponseStatus { OK, ERROR, WARNING }

		/// <summary>
		/// Provides information about operations taking place in the Cart service
		/// </summary>
		[DataContract]
		public class CartResponse {
			
			public CartResponse() : this(CartResponseStatus.OK) { }

			public CartResponse(CartResponseStatus status) { this.Status = status; this.ItemsAdded = new List<CartItem>(); }

			[DataMember]
			public CartResponseStatus Status;
			[DataMember]
			public string Message = string.Empty;
			[DataMember]
			public List<CartItem> ItemsAdded;
		}

	}


}
