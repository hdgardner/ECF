namespace Mediachase.eCF.PublicStore.SharedModules
{
	using System;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.Cms.Util;
    using Mediachase.Cms.WebUtility;
	using Mediachase.Commerce.Profile;
	using Mediachase.Commerce.Orders.Dto;
	using Mediachase.Commerce.Orders;
	using Mediachase.Commerce.Core;
	using System.Web.Security;
	using Mediachase.Commerce.Orders.Managers;
	using System.Collections.Generic;

	/// <summary>
	/// Shared user control that can used to display and provide ability to modify the address. It also includes
    /// validation controls.
	/// </summary>
	public partial class CustomerAddressEditModule : BaseStoreUserControl
	{
		private Guid _PrincipalId;

        /// <summary>
        /// Gets the principal id.
        /// </summary>
        /// <value>The principal id.</value>
		protected Guid PrincipalId
		{
			get
			{
				if (_PrincipalId == Guid.Empty)
				{
					Account _account = Profile.Account;
					if (_account != null)
						_PrincipalId = _account.PrincipalId;
				}

				return _PrincipalId;
			}
		}

        /// <summary>
        /// Sets a value indicating whether [enable client script].
        /// </summary>
        /// <value><c>true</c> if [enable client script]; otherwise, <c>false</c>.</value>
        public bool EnableClientScript
        {
            set
            {
                //EmailRequired.EnableClientScript = value;
                //EmailValid.EnableClientScript = value;
                AddressValidator.EnableClientScript = value;
                NameValidator.EnableClientScript = value;
                LastNameValidator.EnableClientScript = value;
                CityValidator.EnableClientScript = value;
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
                //EmailRequired.Enabled = value;
                //EmailValid.Enabled = value;
				AddressValidator.Enabled = value;
				NameValidator.Enabled = value;
				LastNameValidator.Enabled = value;
				CityValidator.Enabled = value;
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
        public CustomerAddress AddressInfo
        {
            get
            {
				Account account = Profile.Account;
				CustomerAddress addr = null;
				if (Profile.Account == null || (addr = Profile.Account.FindCustomerAddress(AddressId)) == null)
					addr = new CustomerAddress();

				////int id = 0;
				////Int32.TryParse(tbAddressID.Text, out id);
				////addr.AddressId = id;

				//addr.AddressId = !String.IsNullOrEmpty(tbAddressID.Text) ? Int32.Parse(tbAddressID.Text) : 0;

				if (addr.ObjectState == Mediachase.MetaDataPlus.MetaObjectState.Added)
				{
					addr.AddressId = !String.IsNullOrEmpty(tbAddressID.Text) ? Int32.Parse(tbAddressID.Text) : 0;
					//addr.Name = Profile.UserName;
					addr.PrincipalId = PrincipalId;
					addr.ApplicationId = AppContext.Current.ApplicationId;
					MembershipUser user = ProfileContext.Current.User;
					if (user != null)
					{
						addr.Name = FirstName.Text + " " + LastName.Text; //user.UserName;
						addr.Email = user.Email;
					}
				}

				//addr.PrincipalId = PrincipalId; //ProfileContext.Current.UserId;
				//addr.ApplicationId = AppContext.Current.ApplicationId;
				//addr.Email = ProfileContext.Current.User != null ? ProfileContext.Current.User.Email : null;
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
                    tbAddressID.Text = value.AddressId.ToString();
					BindCountries();
					CommonHelper.SelectListItem(Country, value.CountryCode);
					BindRegions();
					CommonHelper.SelectListItem(State, value.State);
					//Email.Text = value.Email;
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
			{
                BindCountries();
				BindRegions();
				BindAddress();
			}
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

		private CustomerAddress GetCurrentAddress()
		{
			CustomerAddress address = null;

			Account account = Profile.Account;

			if (account != null && AddressId > 0)
				address = account.FindCustomerAddress(AddressId);

			return address;
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		public void BindCountries()
		{
            if (Countries != null)
            {
                Country.DataSource = Countries;
                Country.DataBind();
            }

            //BindRegions();
		}

        /// <summary>
        /// Binds the address.
        /// </summary>
		public void BindAddress()
		{
			CustomerAddress address = GetCurrentAddress();

			if (address != null)
			{
				this.AddressInfo = address;
				//CommonHelper.SelectListItem(Country, address.CountryCode);
				//if (!StateTxt.Visible)
				//    CommonHelper.SelectListItem(State, address.State);
				//else
				//    StateTxt.Text = address.State;
			}
		}

        /// <summary>
        /// Binds the regions.
        /// </summary>
		private void BindRegions()
		{
			State.Items.Clear();

			// Bind regions
            if (Countries != null)
            {
				List<CountryDto.CountryRow> rows = new List<CountryDto.CountryRow>(
					from tmpRow in Countries.Country.AsEnumerable() 
					where tmpRow.Code == Country.SelectedValue
					select tmpRow);
				//CountryDto.CountryRow[] rows = (CountryDto.CountryRow[])Countries.Country.Select(String.Format("Code = {0}", Country.SelectedValue));

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

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
		public int SaveChanges()
		{
			int retVal = 0;
			if (Profile.Account != null)
			{
				// save address
				CustomerAddress address = this.AddressInfo;
				address.AcceptChanges();
				retVal = address.AddressId;

				tbAddressID.Text = retVal.ToString();
			}

			return retVal;
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the Country control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void Country_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindRegions();

			CustomerAddress address = GetCurrentAddress();
			if (address != null)
				CommonHelper.SelectListItem(State, address.State);
		}
	}
}