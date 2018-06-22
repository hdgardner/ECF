using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Storage;


namespace NWTD {


	public enum UserLevel { A , B, ANONYMOUS };
	public enum Depository {NONE = 0, NWTD = 1, MSSD = 2}

	/// <summary>
	/// A utility class for dealing with custom NWTD customer-based functionality. 
	/// This class required certain NWTD-specific metat data to be configured in ECF
	/// </summary>
	public class Profile {

		#region Static Properties
		/// <summary>
		/// The session key used to store a business partner price group
		/// </summary>
		public const string PRICING_STATE_GROUP = "BusinessPartnerPriceGroup";

		/// <summary>
		/// The session key used to store a businessparter ID for pricing purposes
		/// </summary>
		public const string PRICING_BP_GROUP = "BusinessPartnerID";

		/// <summary>
		/// Retrieves the NWTD User level of the currently logged in user
		/// </summary>
		public static UserLevel CurrentUserLevel {
			get {
				System.Web.Security.MembershipUser user = System.Web.Security.Membership.GetUser(ProfileContext.Current.UserName);
				if (user == null) return UserLevel.ANONYMOUS;

				if(System.Web.Security.Roles.IsUserInRole(user.UserName, "Level A")) return UserLevel.A;
				return UserLevel.B; 
			}
		}

		/// <summary>
		/// The Business Partner of which the the current user is a member
		/// </summary>
		public static Organization BusinessPartner {
			get{
				return Mediachase.Commerce.Profile.ProfileContext.Current.Profile.Account.Organization;
			}
		}

		/// <summary>
		/// The state in which the current users's business parter belongs to
		/// </summary>
		public static string BusinessPartnerState {
			get {
				if (BusinessPartner != null && BusinessPartner["BusinessPartnerState"] != null) {
					return BusinessPartner["BusinessPartnerState"].ToString();
				}
				return null;
			}
		}

		/// <summary>
		/// The NWTD Identifier for the current user's business partner
		/// </summary>
		public static string BusinessPartnerID {
			get {
				if (BusinessPartner != null && BusinessPartner["BusinessPartnerID"] != null) {
					return BusinessPartner["BusinessPartnerID"].ToString();
				}
				return null;
			}
		
		}

		/// <summary>
		/// The name of the current user's active cart
		/// </summary>
		public static string ActiveCart {
			get {

				if (ProfileContext.Current.Profile.Account["ActiveCart"] == null) {
					ProfileContext.Current.Profile.Account["ActiveCart"] = Mediachase.Commerce.Orders.Cart.DefaultName;
					ProfileContext.Current.Profile.Account.AcceptChanges();
				}
				return ProfileContext.Current.Profile.Account["ActiveCart"].ToString();
			}
			set {
				ProfileContext.Current.Profile.Account["ActiveCart"] = value;
				ProfileContext.Current.Profile.Account.AcceptChanges();
			}
		}

		/// <summary>
		/// The Depository associated with the currently logged in customer
		/// </summary>
		public static Depository CustomerDepository {
			get {
				return GetDepositoryByState(BusinessPartnerState);
			}
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Gets the Depository associated with a State
		/// </summary>
		/// <param name="State">The two-letter abbreviated name of the sate</param>
		/// <returns>NWTD.Profile.Depository</returns>
		public static Depository GetDepositoryByState(string State) {
			string state = State.ToLower();
			if (state == "ut" || state == "nv")
				return Depository.MSSD;

			if (state == "wa" || state == "or" || state == "ak")
				return Depository.NWTD;

			return Depository.NONE;
		
		}

		/// <summary>
		/// Gets the depository associated with an Account (based on that Account's business partner)
		/// </summary>
		/// <param name="Account">The account for which depsitory you want to know</param>
		/// <returns>NWTD.Profile.Depository</returns>
		public static Depository GetCustomerDepository(Account Account) {
			string state = GetUserState(Account);
			return GetDepositoryByState(state);
		}

		/// <summary>
		/// Sets the acctive cart for a supplied customer
		/// </summary>
		/// <param name="Account"></param>
		/// <param name="Cart"></param>
		public static void AssignActiveCart(Account Account, Mediachase.Commerce.Orders.Cart Cart) {
			Account["ActiveCart"] = Cart.Name;
			Account.AcceptChanges();
		}

		/// <summary>
		/// Makes sure that the current customer has an active cart
		/// </summary>
		public static void EnsureCustomerCart() {
			EnsureCustomerCart(ProfileContext.Current.Profile.Account);
		}

		/// <summary>
		/// Makes sure that the supplied customer has an active cart.
		/// </summary>
		/// <param name="Account"></param>
		public static void EnsureCustomerCart(Account Account) {
			Mediachase.Commerce.Orders.Cart activeCart = null;

			//first, check to see if the "activecart" session key matches up with a real cart, and if it is, we're golden
			if (Account["ActiveCart"] != null) {
				activeCart = Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(Account.PrincipalId, Account["ActiveCart"].ToString());
				if (activeCart != null && !NWTD.Orders.Cart.CartCanBeEdited(activeCart)) activeCart = null;//nope, this cart is not editible, so we need to move along
				
			}
			//next, we need to see if the customer has any carts at all, create one
			MetaStorageCollectionBase<Mediachase.Commerce.Orders.Cart> carts = Mediachase.Commerce.Orders.Cart.LoadByCustomer(Account.PrincipalId);

			//if we still don't have an active cart, we'll loop throught the customer's carts and assign the first valid one we find
			if (activeCart == null) {
				foreach (Mediachase.Commerce.Orders.Cart cart in carts) {
					if (activeCart == null && cart.Status.ToString().Equals(NWTD.Orders.Cart.CART_STATUS.OPEN.ToString())) {
						activeCart = cart;
					}
				}
			}

			//finally, if there is still no active cart, create the damn thing
			if (activeCart == null ) {
				//first, we have to make sure we come up with a unique name.
				string baseName = Mediachase.Commerce.Orders.Cart.DefaultName;
				string cartName = baseName;
				int iterations = 1;
				while (Mediachase.Commerce.Orders.Cart.LoadByCustomerAndName(Account.PrincipalId, cartName) != null) {
					cartName = string.Format("{0}-{1}", baseName, iterations.ToString());
					iterations++;
				}
				activeCart = NWTD.Orders.Cart.CreateCart(Account, cartName);

			}


			
			AssignActiveCart(Account, activeCart);		
		}

		/// <summary>
		/// Makes sure that the supplied customer has an active cart.
		/// </summary>
		/// <param name="UserId"></param>
		public static void EnsureCustomerCart(Guid UserId) {
			Account account = Mediachase.Commerce.Profile.Account.LoadByPrincipalId(UserId);
			EnsureCustomerCart(account);
		}

		/// <summary>
		/// Gets the US state that the customer is part of. 
		/// Requires that the user is associated with an Orgnaization have a metafield called "BusinessPartnerState" that is set.
		/// Otherwise you'll get an empty string
		/// </summary>
		/// <param name="UserAccount"></param>
		/// <returns>A string representation of the user's US state. Empty if the customer has none.</returns>
		public static String GetUserState(Account UserAccount) {
			string state = string.Empty;
			if (UserAccount.Organization != null) {
				if(UserAccount.Organization["BusinessPartnerState"] != null) state = UserAccount.Organization["BusinessPartnerState"].ToString();
			}
			return state;
		}

		/// <summary>
		/// Gets the business partner ID associated with a customer. This is actually the Organization associed with the user's meta field value of the "BusinessPartnerID" metafield.
		/// </summary>
		/// <param name="UserAccount"></param>
		/// <returns>The business partner ID. If none exists, returns an empty string.</returns>
		public static String GetUserBusinessPartnerID(Account UserAccount) {
			string id = string.Empty;
			if (UserAccount.Organization != null) {
				if (UserAccount.Organization["BusinessPartnerID"] != null) id = UserAccount.Organization["BusinessPartnerID"].ToString();
			}
			return id;
		}
		
		/// <summary>
		/// NWTD has customer-based price groups that are stored as metadata against a customer's Orgnaization
		/// </summary>
		/// <param name="UserAccount"></param>
		/// <returns></returns>
		public static string GetUserBusinessPartnerPriceGroup(Account UserAccount) {
			string state = string.Empty;
			if (UserAccount.Organization != null) {
				if (UserAccount.Organization["BusinessPartnerPriceGroup"] != null) state = UserAccount.Organization["BusinessPartnerPriceGroup"].ToString();
			}
			return state;
		
		}

		/// <summary>
		/// Sets the current user's price group into the session. ECF uses this information to do price calucations
		/// </summary>
		public static void SetSaleInformation() {
			if(ProfileContext.Current != null)
				SetSaleInformation(ProfileContext.Current.UserId);
		}

		/// <summary>
		/// Sets the current user's price group into the session. ECF uses this information to do price calucations
		/// </summary>
		/// <param name="UserId"></param>
		public static void SetSaleInformation(Guid UserId) {
			Account account = Mediachase.Commerce.Profile.Account.LoadByPrincipalId(UserId);
			if(account != null)
				SetSaleInformation(account);
		}

		//TODO: make the body of this function use the static properties defined in this class
		/// <summary>
		/// Sets the current user's price group into the session. ECF uses this information to do price calucations
		/// </summary>
		/// <param name="Account"></param>
		public static void SetSaleInformation(Account Account) {
			if (Account.Organization != null) {
				System.Web.HttpContext.Current.Session[PRICING_STATE_GROUP] = NWTD.Profile.GetUserBusinessPartnerPriceGroup(Account);
				System.Web.HttpContext.Current.Session[PRICING_BP_GROUP] = NWTD.Profile.GetUserBusinessPartnerID(Account);
			}
		}
		
		#endregion
	}

}
