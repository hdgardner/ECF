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
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Cms;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.WebUtility.Commerce;
using Mediachase.Cms.Util;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
    /// <summary>
    /// Collects the payment information.
    /// </summary>
    public partial class CheckoutPaymentModule : BaseStoreUserControl
    {
        #region Private Variables
        private PaymentMethodDto _PaymentOptions = null;
        private OrderAddress _BillingAddress = null;
        #endregion

        private CartHelper _CartHelper = null;
        /// <summary>
        /// Gets or sets the cart helper.
        /// </summary>
        /// <value>The cart helper.</value>
        public CartHelper CartHelper { get { return _CartHelper; } set { _CartHelper = value; } }

        /// <summary>
        /// Gets or sets the email text.
        /// </summary>
        /// <value>The email text.</value>
        public string EmailText
        {
            get
            {
                return OrderEmail.Text;
            }
            set
            {
                OrderEmail.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        /// <value>The billing address.</value>
        public OrderAddress BillingAddress
        {
            get
            {
                OrderAddress addr = null;
                if (rbBillingAddress.Checked)
                    addr = AddressViewModule.AddressInfo;
                else if(rbBillingNewAddress.Checked)
                {
                    addr = AddressNewModule1.AddressInfo;
                }

                if (addr != null)
                    addr.Email = OrderEmail.Text;

                return addr;
            }
            set
            {
                _BillingAddress = value;
            }
        }

        /// <summary>
        /// Gets the billing address.
        /// </summary>
        /// <returns></returns>
        private OrderAddress GetBillingAddress()
        {
            return BillingAddress;
        }

        /// <summary>
        /// Gets the payment method.
        /// </summary>
        /// <value>The payment method.</value>
        public PaymentMethodDto.PaymentMethodRow PaymentMethod
        {
            get
            {
                string key = String.Empty;
                foreach (DataListItem item in PaymentOptionList.Items)
                {
                    if (((GlobalRadioButton)item.FindControl("PaymentOption")).Checked)
                    {
                        key = PaymentOptionList.DataKeys[item.ItemIndex].ToString();
                    }
                }

                if (_PaymentOptions != null)
                    foreach (PaymentMethodDto.PaymentMethodRow listItem in _PaymentOptions.PaymentMethod)
                    {
                        if (listItem.SystemKeyword == key)
                        {
                            ViewState["PaymentMethod"] = listItem.PaymentMethodId;
                            return listItem;
                        }
                    }

                /*
                if (ViewState["PaymentMethod"] != null)
                    return (PaymentMethodDto.PaymentMethodRow)ViewState["PaymentMethod"];
                 * */

                return null;
            }
        }

        /// <summary>
        /// Gets the payment info.
        /// </summary>
        /// <value>The payment info.</value>
        public Payment PaymentInfo
        {
            get
            {
                PaymentMethodDto.PaymentMethodRow paymentRow = PaymentMethod;
                if (paymentRow != null)
                {
                    IPaymentOption po = FindPaymentControl(PaymentMethod.SystemKeyword);
                    if (po != null)
                        return po.PreProcess(CartHelper.OrderForm);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the payment option control.
        /// </summary>
        /// <value>The payment option control.</value>
        public IPaymentOption PaymentOptionControl
        {
            get
            {
                return FindPaymentControl(PaymentMethod.SystemKeyword);
            }
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns></returns>
        public bool ValidateData()
        {
            bool isValid = false;
            PaymentMethodDto.PaymentMethodRow paymentRow = PaymentMethod;
            if (paymentRow != null)
            {
                IPaymentOption po = FindPaymentControl(paymentRow.SystemKeyword);
                if (po != null)
                    isValid = po.ValidateData();
            }

            return isValid & RadioButtonsPaymentCustomValidator.IsValid;
        }

        /// <summary>
        /// Finds the payment control.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        private IPaymentOption FindPaymentControl(string keyword)
        {
            string key = String.Empty;
            foreach (DataListItem item in PaymentOptionList.Items)
            {
                PlaceHolder paymentOptionHolder = item.FindControl("PaymentOptionHolder") as PlaceHolder;
                if (paymentOptionHolder != null)
                {
                    IPaymentOption po = paymentOptionHolder.FindControl(PaymentMethod.SystemKeyword) as IPaymentOption;
                    if (po != null)
                        return po;
                }
            }

            return null;
        }

        #region Bind Methods
        /// <summary>
        /// Binds the billing address.
        /// </summary>
        private void BindBillingAddress()
        {
            CustomerProfile ci = ProfileContext.Current.Profile;

            if (ci == null || ci.Account == null || ci.Account.Addresses == null || ci.Account.Addresses.Count == 0)
            {
                tblAddresses.Visible = false;
                OrderAddress address = CartHelper.FindAddressByName(CartHelper.OrderForm.BillingAddressId);
                if (address != null)
                {
                    AddressNewModule1.AddressInfo = address;
                }
                else
                {
                    if (CartHelper.Cart.OrderAddresses.Count > 0)
                    {
                        AddressNewModule1.AddressInfo = CartHelper.Cart.OrderAddresses[0];
                    }
                }

                rbBillingNewAddress.Checked = true;
                return;
            }

            //bool bSearch = CurrentOrderInfo.BillingAddress!=null;
            //bool bFound = false;
            AddressesList.Items.Clear();

            if (ci.Account.Addresses.Count > 0)
            {
                AddressesList.DataSource = ci.Account.Addresses;
                AddressesList.DataBind();

                AddressViewModule.AddressInfo = StoreHelper.ConvertToOrderAddress(ci.Account.Addresses[0]);
                AddressViewModule.DataBind();

                CommonHelper.SelectListItem(AddressesList, Profile.PreferredBillingAddress);

                if(!rbBillingNewAddress.Checked)
                    rbBillingAddress.Checked = true;
            }
            else
            {

            }

            /*
            foreach (AddressInfo info in ci.Addresses)
            {
                string strAddress = MakeAddressString(info);
                AddressesList.Items.Add(new ListItem(strAddress, info.AddressId));
                if(bSearch && (info.AddressId == CurrentOrderInfo.BillingAddress.AddressId))
                    bFound = true;
            }
             * */

            /*
            if (!bFound)
            {
                if (CurrentOrderInfo.BillingAddress != null)
                {
                    AddressNewModule1.AddressInfo = CurrentOrderInfo.BillingAddress;
                }
                else
                {
                    // bind shipping address
                    if (CurrentOrderInfo.Shipments != null && CurrentOrderInfo.Shipments.Length > 0 && CurrentOrderInfo.Shipments[0].Details.DeliveryAddress != null)
                    {
                        // need to set AddressId to 0 to avoid replacing corresponding address' fields if new address' fields will be changed
                        AddressInfo ai = CurrentOrderInfo.Shipments[0].Details.DeliveryAddress;
                        ai.AddressId = "0";
                        AddressNewModule1.AddressInfo = ai;
                    }
                }

                rbBillingNewAddress.Checked = true;

                // Bind view address
                if (ci != null && ci.Addresses != null && ci.Addresses.Length != 0)
                {
                    AddressViewModule.AddressInfo = ci.Addresses[0];
                    AddressViewModule.DataBind();
                }
            }
            else
            {
                if (BillingAddress != null && !String.IsNullOrEmpty(BillingAddress.AddressId))
                {
                    CommonHelper.SelectListItem(AddressesList, BillingAddress.AddressId);
                    AddressViewModule.AddressInfo = BillingAddress;
                    AddressViewModule.DataBind();
                }
                else
                {
                    CommonHelper.SelectListItem(AddressesList, AddressesList.Items[0].Value);
                    AddressViewModule.AddressInfo = ci.Addresses[0];
                    AddressViewModule.DataBind();
                }
                rbBillingAddress.Checked = true;
            }
             * */
        }

        /*
        /// <summary>
        /// Makes the address string.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns></returns>
        private string MakeAddressString(AddressInfo info)
        {
            if (info == null)
                return String.Empty;
            string str = String.Format("{0} {1} {2}, {3}, {4}, {5}, {6}, {7}",
                    info.FirstName, info.MiddleName, info.LastName, info.Address1, info.Address2,
                    info.City, info.PostalCode, info.Country);
            return str.Replace(", ,", ", ");
        }
         * */

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (PaymentOptionList.Items.Count > 0)
            {
                DataListItem item = PaymentOptionList.Items[0];
                GlobalRadioButton ctrl = ((GlobalRadioButton)item.FindControl("PaymentOption"));
                ctrl.Checked = true;
            }

            base.Render(writer);
        }


        /// <summary>
        /// Binds the payment options.
        /// </summary>
        private void BindPaymentOptions()
        {
            //if (!this.IsPostBack)
            {
                if (_PaymentOptions == null)
					_PaymentOptions = PaymentManager.GetPaymentMethods(CMSContext.Current.LanguageName);

                PaymentOptionList.DataSource = _PaymentOptions;
                PaymentOptionList.DataBind();
            }
        }

        /// <summary>
        /// Handles the ItemDataBound event of the PaymentOptionList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.</param>
        protected void PaymentOptionList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem == null)
                    return;

                PaymentMethodDto.PaymentMethodRow listItem = ((PaymentMethodDto.PaymentMethodRow)((DataRowView)e.Item.DataItem).Row);

                // Check the item if it has been already selected
                if (ViewState["PaymentMethod"] != null)
                {
                    Guid selectedPayment = new Guid(ViewState["PaymentMethod"].ToString());
                    if (listItem.PaymentMethodId == selectedPayment)
                    {
                        GlobalRadioButton radioButton = (GlobalRadioButton)e.Item.FindControl("PaymentOption");
                        radioButton.Checked = true;
                    }
                }

                // Retrieve the Label control in the current DataListItem.
                PlaceHolder optionPane = (PlaceHolder)e.Item.FindControl("PaymentOptionHolder");
                System.Web.UI.Control paymentCtrl = StoreHelper.LoadPaymentPlugin(this, listItem.SystemKeyword);
                paymentCtrl.ID = listItem.SystemKeyword;
                paymentCtrl.EnableViewState = true;
                optionPane.Controls.Add(paymentCtrl);
                //TestPaymentOptionHolder.Controls.Add(paymentCtrl);
            }
        }

        /// <summary>
        /// Handles the ItemBound event of the PaymentOptionList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.</param>
        protected void PaymentOptionList_ItemCreated(Object sender, DataListItemEventArgs e)
        {
        }
        #endregion
       

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            BindPaymentOptions();
            base.OnInit(e);
        }

        /// <summary>
        /// Prepares the step.
        /// </summary>
        public void PrepareStep()
        {
            if (String.IsNullOrEmpty(OrderEmail.Text))
            {
                if (CartHelper.PrimaryAddress != null && !String.IsNullOrEmpty(CartHelper.PrimaryAddress.Email))
                    OrderEmail.Text = CartHelper.PrimaryAddress.Email;
                else
                    OrderEmail.Text = ProfileContext.Current.User != null ? ProfileContext.Current.User.Email : String.Empty;  //Profile.Email;
            }

            BindBillingAddress();

            AddressNewModule1.Validate = rbBillingNewAddress.Checked;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the AddressesList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void AddressesList_SelectedIndexChanged(Object sender, EventArgs e)
        {
			CustomerProfile ci = ProfileContext.Current.Profile;

			int selectedAddress = Int32.Parse(AddressesList.SelectedValue);

			foreach (CustomerAddress addr in ci.Account.Addresses)
			{
				if (addr.AddressId == selectedAddress)
				{
					AddressViewModule.AddressInfo = StoreHelper.ConvertToOrderAddress(addr);
					AddressViewModule.DataBind();

					//upAddresses.Update();
					break;
				}
			}
        }

        /// <summary>
        /// Handles the Validate event of the RadioButtons control.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="args">The <see cref="T:System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void RadioButtons_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = rbBillingAddress.Checked || rbBillingNewAddress.Checked;
        }
    }
}
