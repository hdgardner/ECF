using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using Mediachase.Commerce.Orders;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class AddressEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		private const string _OrderContextObjectString = "OrderGroup";
		private const string _PostBackArgumentString = "addressChanged";

        OrderGroup _order = null;
		OrderAddress _selectedAddress = null;

        int _AddressId = 0;
        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        /// <value>The address id.</value>
        public int AddressId
        {
            get
            {
                if (_AddressId == 0)
                {
                    if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
                    {
                        _AddressId = Int32.Parse(this.DialogTrigger.Value);
                    }
                }

                return _AddressId;
            }
            set
            {
                _AddressId = value;
            }
        }

		private OrderAddress SelectedAddress
		{
			get
			{
				if (_selectedAddress == null)
				{
					if (_order != null && AddressId != 0)
					{
						OrderAddressCollection addresses = _order.OrderAddresses;
						foreach (OrderAddress addr in addresses)
						{
							if (addr.OrderGroupAddressId == AddressId)
								_selectedAddress = addr;
						}
					}
				}

				return _selectedAddress;
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			bool reset = false;
			if (Request.Form["__EVENTTARGET"] == DialogTrigger.UniqueID)
				reset = true;
			BindForm(reset);
			//BindMetaForm();
            DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
        }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			Page.LoadComplete += new EventHandler(Page_LoadComplete);
			base.OnInit(e);

			MetaDataTab.MDContext = OrderContext.MetaDataContext;
		}

		/// <summary>
		/// Handles the LoadComplete event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Page_LoadComplete(object sender, EventArgs e)
		{
			if (String.Compare(Request.Form["__EVENTTARGET"], SaveChangesButton.UniqueID, StringComparison.Ordinal) == 0)
				SaveChangesButton_Click(SaveChangesButton, null);
		}

        /// <summary>
        /// Handles the ValueChanged event of the DialogTrigger control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DialogTrigger_ValueChanged(object sender, EventArgs e)
        {
			BindForm(true);
        }

        /// <summary>
        /// Handles the Click event of the SaveChangesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void SaveChangesButton_Click(object sender, EventArgs e)
        {
			if (String.Compare(Request.Form["__EVENTARGUMENT"], _PostBackArgumentString) == 0)
			{
				if (_order != null)
				{
					OrderAddress address = null;

					if (AddressId != 0) // find existing
					{
						foreach (OrderAddress a in _order.OrderAddresses)
						{
							if (a.OrderGroupAddressId == AddressId)
							{
								address = a;
								break;
							}
						}
					}

					if (address == null)
						address = _order.OrderAddresses.AddNew();

					address.Name = Name.Text;
					address.FirstName = FirstName.Text;
					address.LastName = LastName.Text;
					address.Organization = Organization.Text;
					address.Line1 = Line1.Text;
					address.Line2 = Line2.Text;
					address.City = City.Text;
					address.State = State.Text;
					address.CountryCode = CountryCode.Text;
					address.CountryName = CountryName.Text;
					address.PostalCode = PostalCode.Text;
					address.RegionCode = RegionCode.Text;
					address.RegionName = RegionName.Text;
					address.DaytimePhoneNumber = DayTimePhone.Text;
					address.EveningPhoneNumber = EveningPhone.Text;
					address.FaxNumber = FaxNumber.Text;
					address.Email = Email.Text;

					// Put a dictionary key that can be used by other tabs
					IDictionary dic = new ListDictionary();
					dic.Add(MetaDataTab._MetaObjectContextKey + MetaDataTab.Languages[0], address);
					MetaDataTab.SaveChanges(dic);

					ScriptManager.RegisterStartupScript(MetaDataTab, typeof(AddressEditTab), "DialogClose", "Address_CloseDialog();", true);
				}
			}
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        private void BindForm(bool reset)
        {
            if (SelectedAddress != null)
            {
                if (reset)
                {
                    Name.Text = SelectedAddress.Name;
                    FirstName.Text = SelectedAddress.FirstName;
                    LastName.Text = SelectedAddress.LastName;
                    Organization.Text = SelectedAddress.Organization;

                    Line1.Text = SelectedAddress.Line1;
                    Line2.Text = SelectedAddress.Line2;
                    City.Text = SelectedAddress.City;
                    State.Text = SelectedAddress.State;
                    CountryCode.Text = SelectedAddress.CountryCode;
                    CountryName.Text = SelectedAddress.CountryName;
                    PostalCode.Text = SelectedAddress.PostalCode;
                    RegionCode.Text = SelectedAddress.RegionCode;
                    RegionName.Text = SelectedAddress.RegionName;
                    DayTimePhone.Text = SelectedAddress.DaytimePhoneNumber;
                    EveningPhone.Text = SelectedAddress.EveningPhoneNumber;
                    FaxNumber.Text = SelectedAddress.FaxNumber;
                    Email.Text = SelectedAddress.Email;
                }
                //ManagementHelper.SelectListItem(PaymentType, selectedPayment.);
                MetaDataTab.ObjectId = SelectedAddress.OrderGroupAddressId;
            }
            else if (reset)
            {
            }

            // Bind Meta classes
			BindMetaForm();
        }

		private void BindMetaForm()
		{
			// Bind Meta Form
			MetaDataTab.MetaClassId = OrderContext.Current.OrderAddressMetaClass.Id;

			Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();
			metaObjects.Add(MetaDataTab.Languages[0], SelectedAddress);

			IDictionary dic = new ListDictionary();
			dic.Add("MetaObjectsContext", metaObjects);

			MetaDataTab.LoadContext(dic);

			MetaDataTab.DataBind();
		}

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            //OrderGroup order = (OrderGroup)context[_OrderContextObjectString];
        }
        #endregion

        #region IAdminContextControl Members

        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            _order = (OrderGroup)context[_OrderContextObjectString];
        }
        #endregion
    }
}