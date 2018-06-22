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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;
using System.Globalization;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Manager.Order.Tabs
{
    public partial class TaxValueEditPopup : OrderBaseUserControl, IAdminContextControl
	{
		private const string _TaxIdString = "taxid";
		private const string _TaxDtoContextString = "TaxDto";
		private const string _TaxIdContextString = "TaxId";

		private TaxDto _Tax = null;

		int _taxValueId = 0;
		int _taxId = 0;

		/// <summary>
		/// Gets or sets the tax value id.
		/// </summary>
		/// <value>The Tax rate id.</value>
		private int TaxValueId
		{
			get
			{
				if (_taxValueId == 0)
				{
					if (!String.IsNullOrEmpty(this.DialogTrigger.Value))
						Int32.TryParse(this.DialogTrigger.Value, out _taxValueId);
				}

				return _taxValueId;
			}
			set
			{
				_taxValueId = value;
			}
		}

		private int TaxId
		{
			get
			{
				return _taxId;
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			BindForm(false);
			DialogTrigger.ValueChanged += new EventHandler(DialogTrigger_ValueChanged);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
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
            CultureInfo ci = ManagementContext.Current.ConsoleUICulture;
			TaxDto.TaxValueRow valueRow = null;

			if (TaxValueId != 0) // find existing
			{
                TaxDto.TaxValueRow[] valueRows = (TaxDto.TaxValueRow[])_Tax.TaxValue.Select(String.Format("TaxValueId={0}", TaxValueId));
				if (valueRows != null && valueRows.Length > 0)
					valueRow = valueRows[0];
			}

			// if not found, create new
			if (valueRow == null)
			{
				valueRow = _Tax.TaxValue.NewTaxValueRow();

				valueRow.TaxId = TaxId;
			}

			// update lineItem fields
			if (valueRow != null)
			{
                valueRow.TaxCategory = TaxCategoriesList.SelectedValue;
                valueRow.JurisdictionGroupId = Int32.Parse(JurisdictionGroupsList.SelectedValue);
                valueRow.Percentage = double.Parse(tbRate.Text, ci);
                valueRow.AffectiveDate = AffectiveDate.Value.ToUniversalTime();
			}

			if (valueRow.RowState == DataRowState.Detached)
				_Tax.TaxValue.AddTaxValueRow(valueRow);

            BindForm(true);

			ScriptManager.RegisterStartupScript(SaveChangesButton, typeof(TaxValueEditPopup), "DialogClose", "TaxValueDialog_CloseDialog();", true);
		}

        /// <summary>
        /// Binds the tax categories.
        /// </summary>
        private void BindTaxCategories()
        {
            CatalogTaxDto taxCategoriesDto = CatalogTaxManager.GetTaxCategories();

            var query = from taxCategory in taxCategoriesDto.TaxCategory
                        orderby taxCategory.Name
                        select taxCategory;

            foreach (CatalogTaxDto.TaxCategoryRow taxCategoryRow in query)
            {
                ListItem li = new ListItem(taxCategoryRow.Field<string>("Name"));

                if (!TaxCategoriesList.Items.Contains(li))
                TaxCategoriesList.Items.Add(li);
            }

            TaxCategoriesList.DataBind();
        }

        /// <summary>
        /// Binds the jurisdiction groups.
        /// </summary>
        private void BindJurisdictionGroups()
        {
            while (JurisdictionGroupsList.Items.Count > 1)
                JurisdictionGroupsList.Items.RemoveAt(1);

            JurisdictionDto jurisdictions = JurisdictionManager.GetJurisdictionGroups(JurisdictionManager.JurisdictionType.Tax);

            var query = from jurisdictionGroup in jurisdictions.JurisdictionGroup
                        orderby jurisdictionGroup.DisplayName
                        select jurisdictionGroup;

            foreach (JurisdictionDto.JurisdictionGroupRow groupRow in query)
            {
                string filterExpression = String.Format("JurisdictionGroupId = {0} AND TaxValueId <> {1}", groupRow.Field<Int32>("JurisdictionGroupId"), TaxValueId);
                ListItem li = new ListItem(groupRow.Field<string>("DisplayName"), groupRow.Field<Int32>("JurisdictionGroupId").ToString()); 
                
                if (groupRow.Field<Int32>("JurisdictionGroupId") != TaxValueId && _Tax.TaxValue.Select(filterExpression).Length > 0)
                    continue;

                JurisdictionGroupsList.Items.Add(li);
            }
           
            JurisdictionGroupsList.DataBind();
        }

		/// <summary>
		/// Binds the form.
		/// </summary>
		/// <param name="reset">if set to <c>true</c> [reset].</param>
		private void BindForm(bool reset)
		{
            CultureInfo ci = ManagementContext.Current.ConsoleUICulture;

            if (TaxCategoriesList.Items.Count == 1)
                BindTaxCategories();

            if(reset)
                BindJurisdictionGroups();

			TaxDto.TaxValueRow selectedRow = null;
			if (TaxValueId != 0)
				selectedRow = _Tax.TaxValue.FindByTaxValueId(TaxValueId);

			if (selectedRow != null)
			{
                if (reset)
                    SetFormFieldsValues(selectedRow.AffectiveDate, selectedRow.Percentage.ToString("#0.000", ci), selectedRow.JurisdictionGroupId, selectedRow.TaxCategory);
			}
			else if (reset)
			{
                SetFormFieldsValues(ManagementHelper.GetUserDateTimeNow(), ((double)0).ToString("#0.000", ci), -1, String.Empty);
			}
		}

		/// <summary>
		/// Sets the form fields values.
		/// </summary>
		private void SetFormFieldsValues(DateTime affectiveDate, string rate, int jurisdictionGroupId, string taxCategory)
		{
            CultureInfo ci = ManagementContext.Current.ConsoleUICulture;

            this.AffectiveDate.Value = ManagementHelper.GetUserDateTime(affectiveDate);
            this.tbRate.Text = rate;

            if(jurisdictionGroupId > 0)
                ManagementHelper.SelectListItem2(JurisdictionGroupsList, jurisdictionGroupId.ToString());
            else
                ManagementHelper.SelectListItem2(JurisdictionGroupsList, String.Empty);

            ManagementHelper.SelectListItem2(TaxCategoriesList, taxCategory);
		}

		#region IAdminContextControl Members
		/// <summary>
		/// Loads the context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void LoadContext(IDictionary context)
		{
			_Tax = (TaxDto)context[_TaxDtoContextString];
            if (_Tax != null && _Tax.Tax.Rows.Count > 0)
            {
                _taxId = _Tax.Tax[0].TaxId;
            }
            else
            {
                _taxId = -1;

                TaxDto.TaxRow taxRow = _Tax.Tax.NewTaxRow();
                taxRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
                taxRow.TaxId = _taxId;
                taxRow.TaxType = 0;
                taxRow.Name = String.Empty;
                taxRow.SortOrder = 0;

                if (taxRow.RowState == DataRowState.Detached)
                    _Tax.Tax.Rows.Add(taxRow);
            }
		}
		#endregion
	}
}