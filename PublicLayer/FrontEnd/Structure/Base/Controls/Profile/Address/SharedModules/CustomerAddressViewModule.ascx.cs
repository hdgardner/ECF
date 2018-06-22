namespace Mediachase.eCF.PublicStore.SharedModules
{
	using System;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using Mediachase.Cms.WebUtility;
	using Mediachase.Commerce.Profile;

	/// <summary>
	///	Used to display address without ability to edit values.
	/// </summary>
	public partial class CustomerAddressViewModule : BaseStoreUserControl
	{
        /// <summary>
        /// Gets or sets the address info.
        /// </summary>
        /// <value>The address info.</value>
        [Bindable(true)]
        public CustomerAddress AddressInfo
        {
            set
            {
                BindData(value);                
            }
            get
            {
                CustomerAddress ai = new CustomerAddress();
                //ai.Id = tbAddressID.Text;
                ai.Email = Email.Text;
                ai.Organization = Company.Text;
                ai.Line1 = Address1.Text;
                ai.Line2 = Address2.Text;
                ai.City = City.Text;
                ai.FirstName = FirstName.Text;
                //ai.mi = MiddleName.Text;
                ai.LastName = LastName.Text;
                ai.CountryName = Country.Text;
                ai.State = State.Text;
                ai.DaytimePhoneNumber = Phone.Text;
                ai.EveningPhoneNumber = EveningPhone.Text;
                ai.FaxNumber = Fax.Text;
                ai.PostalCode = Zip.Text;
                return ai;
            }
        }

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        /// <value>The address id.</value>
        public int AddressId
        {
			get
			{
				return Int32.Parse(tbAddressID.Text);
			}
			//set
			//{
			//    /*
			//    CustomerInfo ci = ClientCustomer.GetCustomerInfo();
			//    if (ci.Addresses != null && ci.Addresses.Length > 0)
			//    {
			//        foreach (AddressInfo info in ci.Addresses)
			//            if (info.AddressId == value.ToString())
			//            {
			//                this.AddressInfo = info;
			//                break;
			//            }
			//    }
			//     * */
			//}
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="addr">The addr.</param>
        public void BindData(CustomerAddress addr)
        {
            if (addr == null)
                return;

            tbAddressID.Text = addr.AddressId.ToString();

            if (!String.IsNullOrEmpty(addr.Organization))
                Company.Text = addr.Organization;

            Address1.Text = addr.Line1;

            if (!String.IsNullOrEmpty(addr.Line2))
                Address2.Text = addr.Line2;

            City.Text = addr.City;
            State.Text = addr.RegionName;
            Email.Text = addr.Email;
            FirstName.Text = addr.FirstName;
            LastName.Text = addr.LastName;

            Phone.Text = addr.DaytimePhoneNumber;
            EveningPhone.Text = addr.EveningPhoneNumber;
            if (!String.IsNullOrEmpty(addr.FaxNumber))
            {
                if (addr.FaxNumber.Contains("(fax)"))
                    Fax.Text = addr.FaxNumber;
                else
                    Fax.Text = addr.FaxNumber + " (fax)";
            }
             
            Zip.Text = addr.PostalCode;
            Country.Text = addr.CountryName;
        }
	}
}