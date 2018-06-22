namespace Mediachase.Cms.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using Mediachase.Cms.WebUtility;
    using System.Web.UI;
    using System.ComponentModel;
    using Mediachase.Commerce.Profile;

	/// <summary>
	///	Used to display address without ability to edit values.
	/// </summary>
    public partial class CustomerAddressNewControl : BaseStoreUserControl
	{
        /// <summary>
        /// The shopping cart mode is when customer selecting address for newly added items to the shopping cart.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shopping cart mode; otherwise, <c>false</c>.
        /// </value>
		public bool IsShoppingCartMode
		{
			get
			{
				return false; //return ClientHelper.GetBooleanValue(Request.QueryString["cartmode"], false);
			}
		}

        /// <summary>
        /// Gets the return URL.
        /// </summary>
        /// <value>The return URL.</value>
		public string ReturnUrl
		{
			get
			{
				return (string)Request.QueryString["returnurl"];
			}
		}

        /// <summary>
        /// Sets the address info.
        /// </summary>
        /// <value>The address info.</value>
		public CustomerAddress AddressInfo
		{
			set
			{
				if (value != null)
					AddressEditModule1.AddressInfo = value;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			ApplyLocalization();
			if (!IsPostBack)
				BindData();
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
			ViewAllAddressesRow.Visible = !Profile.IsAnonymous; //ClientContext.Context.IsUserAuthenticated;
			DataBind();
		}

        /// <summary>
        /// Saves the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveAddress(Object sender, EventArgs e)
		{
			int id = AddressEditModule1.SaveChanges();
			int a1 = 0;
			Account account = Profile.Account;
			if (!Profile.IsAnonymous && account != null)
			{
				a1 = account.Addresses.Add(AddressEditModule1.AddressInfo);
				//Profile.Save();
			}

			if (this.IsShoppingCartMode)
			{
				/*if (ClientContext.Context.Customer != null)
					ClientCustomer.AddCustomerInfo(null, new AddressInfo[] { AddressViewModule1.AddressInfo }, null);*/
				//ClientContext.Context.UpdateCartShipping(AddressViewModule1.AddressInfo);
			}
			/*else
				AddressViewModule1.SaveChanges();*/

			if (String.IsNullOrEmpty(ReturnUrl))
				Response.Redirect(ResolveUrl("~/Profile/secure/AccountAddress.aspx"));
			else
				Response.Redirect(ReturnUrl);
		}

        /// <summary>
        /// Cancels the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void CancelAddress(Object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(ReturnUrl))
				Response.Redirect(ResolveUrl("~/Profile/secure/AccountAddress.aspx"));
			else
				Response.Redirect(ReturnUrl);
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		protected void ApplyLocalization()
		{
			hlAddNewAddress.Text = RM.GetString("ACCOUNT_ADDRESS_NEW_VIEW_ADDRESSES");
			btnSave.Text = RM.GetString("ACCOUNT_ADDRESS_NEW_SAVE");
			btnCancel.Text = RM.GetString("ACCOUNT_ADDRESS_NEW_CANCEL");
		}
	}
}