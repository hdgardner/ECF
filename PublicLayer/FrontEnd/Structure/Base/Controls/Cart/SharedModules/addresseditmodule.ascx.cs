namespace Mediachase.eCF.PublicStore.SharedModules
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using Mediachase.Cms.WebUtility;
    using Mediachase.Commerce.Orders;
    using Mediachase.Commerce.Orders.Dto;
    using System.ComponentModel;
    using Mediachase.Commerce.Profile;
    using Mediachase.Commerce.Orders.Managers;

    /// <summary>
    /// Shared user control that can used to display and provide ability to modify the address. It also includes
    /// validation controls.
    /// </summary>
    public partial class AddressEditModule : BaseStoreUserControl
    {
        /// <summary>
        /// Sets a value indicating whether [enable client script].
        /// </summary>
        /// <value><c>true</c> if [enable client script]; otherwise, <c>false</c>.</value>
        public bool EnableClientScript
        {
            set
            {
                EmailRequired.EnableClientScript = value;
                EmailValid.EnableClientScript = value;
                AddressValidator.EnableClientScript = value;
                NameValidator.EnableClientScript = value;
                LastNameValidator.EnableClientScript = value;
                CityValidator.EnableClientScript = value;
                StateValidator.EnableClientScript = value;
                ZipValidator.EnableClientScript = value;
            }
        }

        /// <summary>
        /// Sets a value indicating whether this <see cref="T:AddressEditModule"/> is validate.
        /// </summary>
        /// <value><c>true</c> if validate; otherwise, <c>false</c>.</value>
        public bool Validate
        {
            set
            {
                EmailRequired.Enabled = value;
                EmailValid.Enabled = value;
                AddressValidator.Enabled = value;
                NameValidator.Enabled = value;
                LastNameValidator.Enabled = value;
                CityValidator.Enabled = value;

                //only validate the state if the dropdown is NOT being used
                if (value)
                {
                    if (State.Visible)
                        StateValidator.Enabled = false;
                    else
                        StateValidator.Enabled = true;
                }
                else
                    StateValidator.Enabled = value;
                ZipValidator.Enabled = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [check valid].
        /// </summary>
        /// <value><c>true</c> if [check valid]; otherwise, <c>false</c>.</value>
        public bool CheckValid
        {
            get
            {
                return this.Page.IsValid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show email].
        /// </summary>
        /// <value><c>true</c> if [show email]; otherwise, <c>false</c>.</value>
        public bool ShowEmail
        {
            get
            {
                return EmailTr.Visible;
            }
            set
            {
                EmailTr.Visible = value;
            }
        }

        /// <summary>
        /// Gets the address id.
        /// </summary>
        /// <value>The address id.</value>
        public int AddressId
        {
            get
            {
                try
                {
                    return Int32.Parse(Request.QueryString["addrId"]);
                }
                catch { return 0; }
            }
        }

        /// <summary>
        /// Gets or sets the address info.
        /// </summary>
        /// <value>The address info.</value>
        [Bindable(true)]
        public OrderAddress AddressInfo
        {
            get
            {
                OrderAddress addr = new OrderAddress();

                //addr.Id = tbAddressID.Text;
                addr.Email = Email.Text;
                addr.EveningPhoneNumber = EveningPhone.Text;
                addr.Organization = Company.Text;
                addr.Line1 = Address1.Text;
                addr.Line2 = Address2.Text;
                addr.City = City.Text;
                addr.FirstName = FirstName.Text;
                addr.LastName = LastName.Text;
				if (Country.SelectedItem != null)
				{
					addr.CountryName = Country.SelectedItem.Text;
					addr.CountryCode = Country.SelectedValue;
				}

				if (StateTxt.Visible || State.SelectedIndex == -1) // a little hack since visible is not stored in the viewstate
				{
					addr.State = StateTxt.Text;
					addr.RegionName = StateTxt.Text;
				}
				else
				{
					addr.State = State.SelectedValue;
					addr.RegionName = State.SelectedItem.Text;
				}

                addr.DaytimePhoneNumber = Phone.Text;
                addr.FaxNumber = Fax.Text;
                addr.PostalCode = Zip.Text;

                return addr;
            }
            set
            {
                if (value != null)
                {
                    tbAddressID.Text = value.OrderGroupAddressId.ToString();
                    Email.Text = value.Email;
                    EveningPhone.Text = value.EveningPhoneNumber;
                    Company.Text = value.Organization;
                    Address1.Text = value.Line1;
                    Address2.Text = value.Line2;
                    City.Text = value.City;
                    FirstName.Text = value.FirstName;
                    //MiddleName.Text = value.MiddleName;
                    LastName.Text = value.LastName;
                    StateTxt.Text = value.State;
                    Phone.Text = value.DaytimePhoneNumber;
                    Fax.Text = value.FaxNumber;
                    Zip.Text = value.PostalCode;
                    //CommonHelper.SelectListItem(Country, value.CountryName);
                    BindRegions();
                    //CommonHelper.SelectListItem(State, value.State);
                }
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Country.Items.Count == 0)
                BindCountries();
        }

        private CountryDto _Countries = null;
        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <value>The countries.</value>
        private CountryDto Countries
        {
            get
            {
                if (_Countries == null)
                    _Countries = CountryManager.GetCountries();

                return _Countries;
            }
        }

        /// <summary>
        /// Binds the countries.
        /// </summary>
        public void BindCountries()
        {
            if (Countries != null)
            {
                Country.DataSource = Countries;
                Country.DataBind();
            }

            OrderAddress address = null;
            //if (AddressId > 0)
            //   address = ClientHelper.GetAddress(AddressId);

            //if (address != null)
            //    CommonHelper.SelectListItem(Country, address.Country);

            BindRegions();

            if (address != null)
                this.AddressInfo = address;
        }

        /// <summary>
        /// Binds the regions.
        /// </summary>
        private void BindRegions()
        {
            State.Items.Clear();

            // Bind regions
            if (Countries != null && Countries.Country != null && Countries.Country.Count > 0)
            {
                CountryDto.CountryRow[] rows = (CountryDto.CountryRow[])Countries.Country.Select(String.Format("Code = '{0}'", Country.SelectedValue));
                if (rows.Length > 0)
                {
                    DataRow[] regionRows = rows[0].GetStateProvinceRows();

                    if (regionRows.Length > 0)
                    {
                        State.DataSource = regionRows;
                        State.DataBind();

                        State.Visible = true;
                        StateTxt.Visible = false;
                        StateValidator.Enabled = false;

                    }
                    else
                    {
                        State.Visible = false;
                        StateTxt.Visible = true;
                        StateValidator.Enabled = true;

                    }
                }
            }
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        /*public int SaveChanges()
        {
            AddressInfo address = this.AddressInfo;
            CustomerContentAddResponseItem newItem = ClientCustomer.AddCustomerInfo(null, new AddressInfo[] { address }, null);
            if (newItem != null && newItem.newAddressIDs != null && newItem.newAddressIDs.Length > 0)
            {
                //ClientContext.Context.Cart = null; // set cart to null to refresh it
                tbAddressID.Text = newItem.newAddressIDs[0].ToString();
                return newItem.newAddressIDs[0];
            }
            else
                return 0;
        }*/

        /// <summary>
        /// Handles the SelectedIndexChanged event of the Country control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected void Country_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            BindRegions();
        }
    }
}
