using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Commerce.Profile;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CheckoutAddressModule : BaseStoreUserControl
    {
        private CartHelper _CartHelper = null;
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }

        /// <summary>
        /// Gets the shipping address.
        /// </summary>
        /// <value>The shipping address.</value>
        public OrderAddress ShippingAddress
        {
            get
            {
                EnsureChildControls();
                if (rbShipToNewAddress.Checked)
                {
                    return AddressNewModule1.AddressInfo;
                }
                else
                {
                    foreach (DataListItem dli in AddressList.Items)
                    {
                        GlobalRadioButton btn = dli.FindControl("rbShipToAddress") as GlobalRadioButton;
                        if (btn.Checked)
                            return (dli.FindControl("AddressViewModule1") as Mediachase.eCF.PublicStore.SharedModules.AddressViewModule).AddressInfo;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Validates the current form on post back. Called by the wizard class.
        /// </summary>
        /// <returns></returns>
        public bool ValidateForm()
        {
            // Turn on address validation if new address is created
            if (rbShipToNewAddress.Checked)
                AddressNewModule1.Validate = true;
            else
                AddressNewModule1.Validate = false;
            
            this.Page.Validate();

            return Page.IsValid;

            //return RadioButtonsCustomValidator.IsValid;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            AddressNewModule1.Validate = false;
			hlAddNewAddress.NavigateUrl = ResolveUrl("~/Profile/secure/AccountAddressNew.aspx?returnurl=" + HttpUtility.UrlEncode(ResolveUrl("~/Cart/ShoppingCart.aspx")));
            //if (rbShipToNewAddress.Checked)
            //    AddressNewModule1.Validate = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            BindData();
            base.OnInit(e);
        }

        /// <summary>
        /// Applies the localization.
        /// </summary>
		protected void ApplyLocalization()
		{
			
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            // No support for saving addresses for anonymous user
            if (Profile.IsAnonymous)
            {
                rbShipToNewAddress.Checked = true;
                trShippingAddresses.Visible = false;
                return;
            }

            CustomerAddressCollection addresses = null;
            if (Profile.Account != null)
                addresses = Profile.Account.Addresses;

            if (addresses != null && addresses.Count > 0)
            {
                AddressList.DataSource = addresses;
                AddressList.DataBind();
            }
            else
                trShippingAddresses.Visible = false;

            rbShipToNewAddress.Checked = true;
        }

        /// <summary>
        /// Edits the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void EditAddress(Object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = (btn.Parent as DataListItem).ItemIndex;
            DataList dList = btn.Parent.Parent as DataList;
			int addressId = Int32.Parse((dList.DataKeys[index]).ToString());
			Response.Redirect(ResolveUrl(String.Format("~/Profile/secure/AccountAddressEdit.aspx?addrId={0}&returnurl={1}",
				addressId, HttpUtility.UrlEncode(ResolveUrl("~/Cart/ShoppingCart.aspx")))));
            //Response.Redirect(ClientHelper.FormatUrl("addressedit", Int32.Parse((dList.DataKeys[index]).ToString()), this.Request.Path));
        }

        /// <summary>
        /// Handles the Validate event of the RadioButtons control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void RadioButtons_Validate(object source, ServerValidateEventArgs args)
        {
            bool bChecked = false;
            foreach (DataListItem dli in AddressList.Items)
            {
                GlobalRadioButton rb = dli.FindControl("rbShipToAddress") as GlobalRadioButton;
                if (rb != null && rb.Checked)
                {
                    bChecked = true;
                    break;
                }
            }

            args.IsValid = bChecked || rbShipToNewAddress.Checked;
        }
    }
}