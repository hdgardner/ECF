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
    using Mediachase.Commerce.Orders;
    using Mediachase.Commerce.Orders.Dto;

	/// <summary>
	///	Used to display address without ability to edit values.
	/// </summary>
    public partial class CustomerAddressEditControl : BaseStoreUserControl
	{
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
			DataBind();
		}

        /// <summary>
        /// Saves the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveAddress(Object sender, EventArgs e)
		{
			AddressEditModule1.SaveChanges();
			
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
			btnSave.Text = RM.GetString("ACCOUNT_ADDRESS_EDIT_SAVE");
			btnCancel.Text = RM.GetString("ACCOUNT_ADDRESS_EDIT_CANCEL");
			hlAddNewAddress.Text = RM.GetString("ACCOUNT_ADDRESS_EDIT_VIEW_ADDRESSES");
		}
	}
}