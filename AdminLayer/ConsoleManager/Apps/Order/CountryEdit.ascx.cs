using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order
{
	public partial class CountryEdit : OrderBaseUserControl
	{
		private const string _CountryDtoEditSessionKey = "ECF.CountryDto.Edit";
		private const string _CountryIdString = "countryid";
		private const string _CountryDtoString = "CountryDto";

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
		protected void Page_Load(object sender, EventArgs e)
		{
			LoadContext();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
		}

		/// <summary>
		/// Loads the fresh.
		/// </summary>
		/// <returns></returns>
		private CountryDto LoadFresh()
		{
			CountryDto country = CountryManager.GetCountry(CountryId, true);

			if (country == null)
				country = new CountryDto();

			if (country.Country.Count == 0)
				country.EnforceConstraints = false;

			// persist in session
			Session[_CountryDtoEditSessionKey] = country;

			return country;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			//if (CountryId > 0)
			{
				CountryDto country = null;
				if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
					country = LoadFresh();
				else // load from session
				{
					country = (CountryDto)Session[_CountryDtoEditSessionKey];

					if (country == null)
						country = LoadFresh();
				}

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_CountryDtoString, country);

				// Call tabs load context
				ViewControl.LoadContext(dic);
			}
		}

		/// <summary>
		/// Handles the SaveChanges event of the EditSaveControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
		{
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			try
			{
				CountryDto country = (CountryDto)Session[_CountryDtoEditSessionKey];

				if (CountryId > 0 && country == null)
					country = CountryManager.GetCountry(CountryId, true);
				else if (CountryId == 0)
					country = new CountryDto();

				country.EnforceConstraints = false;

				IDictionary context = new ListDictionary();
				context.Add(_CountryDtoString, country);

				ViewControl.SaveChanges(context);

				if (country.HasChanges())
				{
					// if new country has just been created, update countryid for stateProvince rows
					if (country.Country.Rows.Count > 0 && country.Country[0].RowState== DataRowState.Added)
					{
						int cId = country.Country[0].CountryId;
						foreach (CountryDto.StateProvinceRow row in country.StateProvince.Rows)
							row.CountryId = cId;
					}

					country.EnforceConstraints = true;

					// commit changes to the db
					CountryManager.SaveCountry(country);
				}
			}
			catch (Exception ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
			finally
			{
				// we don't need to store Dto in session any more
				Session.Remove(_CountryDtoEditSessionKey);
			}
		}
	}
}