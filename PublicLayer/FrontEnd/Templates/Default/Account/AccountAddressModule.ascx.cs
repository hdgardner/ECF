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

namespace Mediachase.Cms.Website.Templates.Default.Modules
{
	/// <summary>
	///	Used to display address without ability to edit values.
	/// </summary>
	public partial class AccountAddressModule : BaseStoreUserControl
	{
		private int _RowIndex = 0;

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
			_RowIndex = 0;
			Account account = Profile.Account;
			if (account != null)
			{
				AddressList.DataSource = account.Addresses;
				//AddressList.DataBind();
			}

            DataBind();
		}

        /// <summary>
        /// Gets the index of the current row.
        /// </summary>
        /// <returns></returns>
		public int GetCurrentRowIndex()
		{
			return _RowIndex;
		}

        /// <summary>
        /// Handles the ItemCreated event of the AddressList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataListItemEventArgs"/> instance containing the event data.</param>
		protected void AddressList_ItemCreated(object sender, DataListItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				this._RowIndex++;
				Button btn = e.Item.FindControl("btnDeleteAddress") as Button;
				if (btn != null)
				    btn.OnClientClick = "return confirm('Are you sure you want to delete the address?');";
			}
		}

        /// <summary>
        /// Edits the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void EditAddress(Object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			int addrId = 0;
			try
			{
				int index = (btn.Parent as DataListItem).ItemIndex;
				addrId = Int32.Parse(AddressList.DataKeys[index].ToString());
			}
			catch { }
			//Response.Redirect(ClientHelper.FormatUrl("addressedit", addrId, String.Empty));
			Response.Redirect(ResolveUrl(String.Format("~/Profile/secure/AccountAddressEdit.aspx?addrid={0}", addrId)));
		}

        /// <summary>
        /// Deletes the address.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void DeleteAddress(Object sender, EventArgs e)
		{
			Button btn = (Button)sender;
			int index = (btn.Parent as DataListItem).ItemIndex;

			Account account = Profile.Account;
			if (account != null)
			{
				CustomerAddress ca = account.FindCustomerAddress((int)AddressList.DataKeys[index]);
				if (ca != null)
				{
					ca.Delete();
					ca.AcceptChanges();
				}
			}

			BindData();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		protected void ApplyLocalization()
		{
			hlAddNewAddress.Text = RM.GetString("ACCOUNT_ADDRESS_NEW");
			foreach (DataListItem li in AddressList.Items)
			{
				Button btn = (Button)li.FindControl("btnEditAddress");
				if (btn != null)
				{
					btn.Text = RM.GetString("ACCOUNT_ADDRESS_EDIT");
				}
				btn = (Button)li.FindControl("btnDeleteAddress");
				if (btn != null)
				{
					btn.Text = RM.GetString("ACCOUNT_ADDRESS_DELETE");
				}
			}
		}

	}
}