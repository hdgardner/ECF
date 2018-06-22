using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
	public partial class CountryEditTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _CountryIdString = "countryid";
		private const string _CountryDtoString = "CountryDto";

		private CountryDto _Country = null;

		/// <summary>
		/// Gets the Country id.
		/// </summary>
		/// <value>The Country id.</value>
		public int CountryId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_CountryIdString);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (CountryId > 0)
			{
				if (_Country.Country.Count > 0)
				{
					this.tbCountryName.Text = _Country.Country[0].Name;
					this.CodeText.Text = _Country.Country[0].Code;
					this.SortOrder.Text = _Country.Country[0].Ordering.ToString();
					this.IsVisible.IsSelected = _Country.Country[0].Visible;
				}
				else
				{
					DisplayErrorMessage(String.Format("Country with id={0} not found.", CountryId));
					return;
				}
			}
			else
			{
				this.SortOrder.Text = "0";
			}
		}

		/// <summary>
		/// Checks if entered country code is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void CountryCodeCheck(object sender, ServerValidateEventArgs args)
		{
			// load country by code
			CountryDto dto = CountryManager.GetCountry(args.Value, true);

			// check if country with specified code is loaded
			if (dto != null && dto.Country.Count > 0 &&
				dto.Country[0].CountryId != CountryId &&
				String.Compare(dto.Country[0].Code, args.Value, StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				args.IsValid = false;
				return;
			}

			args.IsValid = true;
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Country = (CountryDto)context[_CountryDtoString];
		}
		#endregion

		#region IAdminTabControl Members
		/// <summary>
		/// Saves the changes.
		/// </summary>
		/// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			CountryDto dto = (CountryDto)context[_CountryDtoString];

			CountryDto.CountryRow countryRow = null;

			if (dto == null)
				// dto must be created in base control that holds tabs
				return;

			if (dto.Country.Count > 0)
				countryRow = dto.Country[0];
			else
			{
				countryRow = dto.Country.NewCountryRow();
				countryRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
			}

			countryRow.Name = tbCountryName.Text;
			countryRow.Code = CodeText.Text;
			countryRow.Ordering = Int32.Parse(SortOrder.Text);
			countryRow.Visible = IsVisible.IsSelected;

			if (countryRow.RowState == DataRowState.Detached)
				dto.Country.Rows.Add(countryRow);
		}
		#endregion
	}
}