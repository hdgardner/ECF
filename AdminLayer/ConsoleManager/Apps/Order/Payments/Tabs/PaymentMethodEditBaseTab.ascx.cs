using System;
using System.Collections;
using System.Collections.Generic;
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
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Manager.Order.Payments.Tabs
{
	/// <summary>
	///		Summary description for PaymentMethodEditBaseTab.
	/// </summary>
	public partial class PaymentMethodEditBaseTab : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _PaymentMethodDtoString = "PaymentMethodDto";

		private PaymentMethodDto _PaymentMethodDto = null;

		/// <summary>
		/// Gets the payment method id.
		/// </summary>
		/// <value>The payment method id.</value>
		public Guid PaymentMethodId
		{
			get
			{
				return ManagementHelper.GetGuidFromQueryString("pid");
			}
		}

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
		public string LanguageCode
		{
			get
			{
				if (Parameters["lang"] != null)
					return Parameters["lang"];
				else
					return String.Empty;
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
		/// Checks if entered payment method name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void PaymentMethodSystemNameCheck(object sender, ServerValidateEventArgs args)
		{
			// load payment method
			PaymentMethodDto dto = PaymentManager.GetPaymentMethodBySystemName(args.Value, ddlLanguage.SelectedValue, true);

			if (dto != null && dto.PaymentMethod.Count > 0)
			{
				if (dto.PaymentMethod[0].PaymentMethodId != PaymentMethodId)
				{
					args.IsValid = false;
					return;
				}
			}

			args.IsValid = true;
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			// Bind Languages
			BindLanguages();

			// bind available payment gateway classes
			BindClassNames();

			if (_PaymentMethodDto != null && _PaymentMethodDto.PaymentMethod.Count > 0)
			{
				try
				{
					PaymentMethodDto.PaymentMethodRow paymentRow = _PaymentMethodDto.PaymentMethod[0];

					this.lblPaymentMethodId.Text = paymentRow.PaymentMethodId.ToString();
					this.tbName.Text = paymentRow.Name;
					this.tbDescription.Text = paymentRow.Description;
					this.tbSystemName.Text = paymentRow.SystemKeyword;
					this.tbSortOrder.Text = paymentRow.Ordering.ToString();
					this.IsActive.IsSelected = paymentRow.IsActive;
					this.IsDefault.IsSelected = paymentRow.IsDefault;
					this.SupportsRecurring.IsSelected = paymentRow.SupportsRecurring;

					ManagementHelper.SelectListItem(ddlLanguage, paymentRow.LanguageId);
					ManagementHelper.SelectListItem(ddlClassName, paymentRow.ClassName);

					// do not allow to change system name
					this.tbSystemName.Enabled = false;

					// set initial state of dual list
					BindShippingMethodsList(paymentRow);
				}
				catch(Exception ex)
				{
					DisplayErrorMessage("Error during binding form: " + ex.Message); 
				}
			}
			else
			{
				// set default form values
				this.tbSortOrder.Text = "0";
				this.tbSystemName.Enabled = true;

				BindShippingMethodsList(null);
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
        /// Binds the class names.
        /// </summary>
		private void BindClassNames()
		{
			this.ddlClassName.Items.Clear();
			ddlClassName.Items.Add(new ListItem(RM.GetString("GENERAL_NONE"), ""));

			foreach (string cls in ReflectionHelper.GetClassesBasedOnTypeInSiteDir(typeof(IPaymentGateway)))
			{
				string className = cls.Split(new char[] { ',' })[0];
				ddlClassName.Items.Add(new ListItem(className, className + "," + cls.Split(new char[] { ',' })[1]));
			}

			ddlClassName.DataBind();
		}

        /// <summary>
        /// Binds the shipping methods list.
        /// </summary>
        /// <param name="paymentRow">The payment row.</param>
		private void BindShippingMethodsList(PaymentMethodDto.PaymentMethodRow paymentRow)
		{
			List<ShippingMethodDto.ShippingMethodRow> leftShippings = new List<ShippingMethodDto.ShippingMethodRow>();
			List<ShippingMethodDto.ShippingMethodRow> rightShippings = new List<ShippingMethodDto.ShippingMethodRow>();

			ShippingMethodDto dto = ShippingManager.GetShippingMethods(paymentRow != null ? paymentRow.LanguageId : LanguageCode, true);

			bool allToLeft = false; // if true, then add all shipping methods to the left list

			if (paymentRow != null)
			{
				PaymentMethodDto.ShippingPaymentRestrictionRow[] restrictedShippingRows = paymentRow.GetShippingPaymentRestrictionRows();
				if (restrictedShippingRows != null && restrictedShippingRows.Length > 0)
				{
					foreach (ShippingMethodDto.ShippingMethodRow shippingMethodRow in dto.ShippingMethod)
					{
						bool found = false;
						foreach (PaymentMethodDto.ShippingPaymentRestrictionRow restrictedShippingRow in restrictedShippingRows)
						{
							if (shippingMethodRow.ShippingMethodId == restrictedShippingRow.ShippingMethodId)
							{
								found = true;
								break;
							}
						}

						if (found)
							rightShippings.Add(shippingMethodRow);
						else
							leftShippings.Add(shippingMethodRow);
					}

					ShippingMethodsList.LeftDataSource = leftShippings;
					ShippingMethodsList.RightDataSource = rightShippings;
				}
				else
					// add all shipping methods to the left list
					allToLeft = true;
			}
			else
				allToLeft = true;

			if (allToLeft)
				// add all shipping methods to the left list
				ShippingMethodsList.LeftDataSource = dto.ShippingMethod;

			ShippingMethodsList.DataBind();
		}

		#region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_PaymentMethodDto = (PaymentMethodDto)context[_PaymentMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			PaymentMethodDto dto = (PaymentMethodDto)context[_PaymentMethodDtoString];
			PaymentMethodDto.PaymentMethodRow paymentRow = null;

			if (dto == null)
				// dto must be created in base payment control that holds tabs
				return;

			// create the row if it doesn't exist; or update its modified date if it exists
			if (dto.PaymentMethod.Count > 0)
			{
				paymentRow = dto.PaymentMethod[0];
			}
			else
			{
				paymentRow = dto.PaymentMethod.NewPaymentMethodRow();
				paymentRow.PaymentMethodId = Guid.NewGuid();
				paymentRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
                paymentRow.Created = DateTime.UtcNow;
				paymentRow.SystemKeyword = this.tbSystemName.Text;
			}

			// fill the row with values
            paymentRow.Modified = DateTime.UtcNow;
			paymentRow.Name = tbName.Text;
			paymentRow.ClassName = ddlClassName.SelectedValue;
			paymentRow.LanguageId = ddlLanguage.SelectedValue;
			paymentRow.Description = tbDescription.Text;
			paymentRow.IsActive = this.IsActive.IsSelected;
			paymentRow.IsDefault = this.IsDefault.IsSelected;
			paymentRow.Ordering = Int32.Parse(this.tbSortOrder.Text);
			paymentRow.SupportsRecurring = this.SupportsRecurring.IsSelected;

			// add the row to the dto
			if (paymentRow.RowState == DataRowState.Detached)
				dto.PaymentMethod.Rows.Add(paymentRow);

			// populate shipping methods restrictions

			// a). delete rows from dto that are not selected
			foreach (PaymentMethodDto.ShippingPaymentRestrictionRow rowTmp in paymentRow.GetShippingPaymentRestrictionRows())
			{
				bool found = false;
				foreach (ListItem item in ShippingMethodsList.RightItems)
				{
					if (String.Compare(item.Value, rowTmp.ShippingMethodId.ToString(), true) == 0 && rowTmp.RestrictShippingMethods)
					{
						found = true;
						break;
					}
				}

				if (!found)
					rowTmp.Delete();
			}

			// b). add selected rows to dto
			foreach (ListItem item in ShippingMethodsList.RightItems)
			{
				bool exists = false;
				foreach (PaymentMethodDto.ShippingPaymentRestrictionRow rowTmp in paymentRow.GetShippingPaymentRestrictionRows())
				{
					if (String.Compare(item.Value, rowTmp.ShippingMethodId.ToString(), true) == 0 && rowTmp.RestrictShippingMethods)
					{
						exists = true;
						break;
					}
				}

				if (!exists)
				{
					PaymentMethodDto.ShippingPaymentRestrictionRow restrictedRow = dto.ShippingPaymentRestriction.NewShippingPaymentRestrictionRow();
					restrictedRow.ShippingMethodId = new Guid(item.Value);
					restrictedRow.PaymentMethodId = paymentRow.PaymentMethodId;
					restrictedRow.RestrictShippingMethods = true;

					// add the row to the dto
					dto.ShippingPaymentRestriction.Rows.Add(restrictedRow);
				}
			}
		}
		#endregion
	}
}
