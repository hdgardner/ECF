using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Cms;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Manager.Core.Controls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	/// <summary>
	///		Summary description for ShippingMethodEditBaseTab.
	/// </summary>
	public partial class ShippingMethodEditBaseTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;

		/// <summary>
		/// Gets the shipping method id.
		/// </summary>
		/// <value>The shipping method id.</value>
		public Guid ShippingMethodId
		{
			get
			{
				return ManagementHelper.GetGuidFromQueryString("smid");
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
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			ddlShippingOption.SelectedIndexChanged += new EventHandler(ddlShippingOption_SelectedIndexChanged);

			base.OnInit(e);
		}

		/// <summary>
		/// Checks if entered shipping method name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void ShippingMethodNameCheck(object sender, ServerValidateEventArgs args)
		{
			// load shipping method
			ShippingMethodDto dto = ShippingManager.GetShippingMethods(ddlLanguage.SelectedValue, true);

			if (dto != null && dto.ShippingMethod.Count > 0)
			{
				ShippingMethodDto.ShippingMethodRow[] shippingRows = (ShippingMethodDto.ShippingMethodRow[])dto.ShippingMethod.Select(String.Format("Name='{0}' and ShippingMethodId<>'{1}'", args.Value, ShippingMethodId));
				if (shippingRows != null && shippingRows.Length > 0)
				{
					args.IsValid = false;
					return;
				}
			}

			args.IsValid = true;
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlShippingOption control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ddlShippingOption_SelectedIndexChanged(object sender, EventArgs e)
		{
			Guid selectedId = Guid.Empty;
			try
			{
				selectedId = new Guid(ddlShippingOption.SelectedValue);
			}
			catch { }
			BindShippingOptionParameters(selectedId);
			ShippingOptionParametersContentPanel.Update();
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			// Bind Languages
			BindLanguages();

			string language = ddlLanguage.SelectedValue;

			// bind available Shipping gateway classes
			BindShippingOptions(language);

			// Bind Currencies
			BindCurrency();

			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingMethod.Count > 0)
			{
				try
				{
					ShippingMethodDto.ShippingMethodRow shippingRow = _ShippingMethodDto.ShippingMethod[0];

					this.lblShippingMethodId.Text = shippingRow.ShippingMethodId.ToString();
					this.tbName.Text = shippingRow.Name;
					this.tbFriendlyName.Text = shippingRow.DisplayName;
					try
					{
						this.tbDescription.Text = shippingRow.Description;
					}
					catch 
					{
						this.tbDescription.Text = "";
					}
					this.tbBasePrice.Text = shippingRow.BasePrice.ToString("#.00", System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat);
					
					try
					{
						this.tbSortOrder.Text = shippingRow.Ordering.ToString();
					}
					catch
					{
						this.tbSortOrder.Text = "0";
					}

					this.IsActive.IsSelected = shippingRow.IsActive;
					this.IsDefault.IsSelected = shippingRow.IsDefault;

					ManagementHelper.SelectListItem(ddlShippingOption, shippingRow.ShippingOptionId);
					ManagementHelper.SelectListItemIgnoreCase(ddlLanguage, shippingRow.LanguageId);
					ManagementHelper.SelectListItemIgnoreCase(ddlCurrency, shippingRow.Currency);

					// do not allow to change system name
					this.tbName.Enabled = false;
				}
				catch(Exception ex)
				{
					DisplayErrorMessage("Error during binding form: " + ex.Message); 
				}
			}
			else
			{
				// set default form values
				this.tbName.Enabled = true;
				this.tbSortOrder.Text = "0";
			}
		}

        /// <summary>
        /// Binds the languages.
        /// </summary>
		private void BindLanguages()
		{
			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
				ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
				ddlLanguage.Items.Add(item);
			}
		}

        /// <summary>
        /// Binds the currency.
        /// </summary>
		private void BindCurrency()
		{
			CurrencyDto currencies = CatalogContext.Current.GetCurrencyDto();
			foreach (CurrencyDto.CurrencyRow row in currencies.Currency)
			{
				ListItem item = new ListItem(row.Name, row.CurrencyCode);
				ddlCurrency.Items.Add(item);
			}
		}

        /// <summary>
        /// Binds the shipping options.
        /// </summary>
        /// <param name="language">The language.</param>
		private void BindShippingOptions(string language)
		{
			this.ddlShippingOption.Items.Clear();

			ShippingMethodDto sm = ShippingManager.GetShippingMethods(language, true);

			ddlShippingOption.DataSource = sm.ShippingOption;
			ddlShippingOption.DataBind();
		}

        /// <summary>
        /// Binds the shipping option parameters.
        /// </summary>
        /// <param name="shippingOptionId">The shipping option id.</param>
		private void BindShippingOptionParameters(Guid shippingOptionId)
		{
			phShippingOptionPatameters.Controls.Clear();
			if (shippingOptionId != Guid.Empty)
			{
				// TODO: 
				// Load dynamic configuration form
				//System.Web.UI.Control ctrl = null;
				//if (System.IO.File.Exists(Server.MapPath(string.Concat("~/Plugins/ShippingGateways/", option.SystemKeyword, "/ConfigureVariableShipping.ascx"))))
				//{
				//    ctrl = base.LoadControl(string.Concat("~/Plugins/ShippingGateways/", option.SystemKeyword, "/ConfigureVariableShipping.ascx"));
				//}
				//else { }

				//if (ctrl != null)
				//{
				//    ctrl.ID = option.SystemKeyword;
				//    ((IDynamicAdminControl)ctrl).ObjectId = this.MethodId;
				//    this.ShippingOptionPane.Controls.Add(ctrl);
				//}
			}
		}

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_ShippingMethodDto = (ShippingMethodDto)context[_ShippingMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			ShippingMethodDto dto = (ShippingMethodDto)context[_ShippingMethodDtoString];
			ShippingMethodDto.ShippingMethodRow row = null;

			if (dto == null)
				// dto must be created in base Shipping control that holds tabs
				return;

			// create the row if it doesn't exist; or update its modified date if it exists
			if (dto.ShippingMethod.Count > 0)
			{
				row = dto.ShippingMethod[0];
			}
			else
			{
				row = dto.ShippingMethod.NewShippingMethodRow();
				row.ApplicationId = OrderConfiguration.Instance.ApplicationId;
				row.ShippingMethodId = Guid.NewGuid();
                row.Created = DateTime.UtcNow;
				row.Name = this.tbName.Text;
			}

			// fill the row with values
            row.Modified = DateTime.UtcNow;
			row.DisplayName = tbFriendlyName.Text;
			row.Description = tbDescription.Text;
			Guid shippingOption = Guid.Empty;
			try
			{
				shippingOption = new Guid(ddlShippingOption.SelectedValue);
			}
			catch { }

            //add ShippingOption row in dataset for ForeignKeyConstraint requirement.
            if (row["ShippingOptionId"] != DBNull.Value && !shippingOption.Equals(row.ShippingOptionId))
            {
                string shippingOptionFilter = String.Format("ShippingOptionId = '{0}'", shippingOption);
                if (dto.ShippingOption.Select(shippingOptionFilter).Length == 0)
                {
                    string language = ddlLanguage.SelectedValue;
                    ShippingMethodDto sm = ShippingManager.GetShippingMethods(language, true);
                    ShippingMethodDto.ShippingOptionRow[] shippingOptionRows = (ShippingMethodDto.ShippingOptionRow[])sm.ShippingOption.Select(shippingOptionFilter);
                    if (shippingOptionRows != null && shippingOptionRows.Length > 0)
                    {
                        ShippingMethodDto.ShippingOptionRow newShippingOptionRow = dto.ShippingOption.NewShippingOptionRow();
                        newShippingOptionRow.ItemArray = shippingOptionRows[0].ItemArray;
                        dto.ShippingOption.AddShippingOptionRow(newShippingOptionRow);
                        dto.ShippingOption[dto.ShippingOption.Count - 1].AcceptChanges();
                    }
                }
            }

            row.ShippingOptionId = shippingOption;
            row.BasePrice = decimal.Parse(tbBasePrice.Text);
			row.Currency = ddlCurrency.SelectedValue;
			row.LanguageId = ddlLanguage.SelectedValue;
			row.IsActive = IsActive.IsSelected;
			row.IsDefault = IsDefault.IsSelected;
			row.Ordering = Int32.Parse(tbSortOrder.Text);

			// add the row to the dto
			if (row.RowState == DataRowState.Detached)
				dto.ShippingMethod.Rows.Add(row);
		}
		#endregion
	}
}
