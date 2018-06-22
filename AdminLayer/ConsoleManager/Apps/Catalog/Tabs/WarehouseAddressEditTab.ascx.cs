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
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class WarehouseAddressEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _WarehouseDtoString = "WarehouseDto";
		private const string _WarehouseIdString = "WarehouseId";

		private WarehouseDto _WarehouseDto = null;

		/// <summary>
		/// Gets the warehouse id.
		/// </summary>
		/// <value>The warehouse id.</value>
		public int WarehouseId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_WarehouseIdString);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (WarehouseId > 0)
			{
				if (_WarehouseDto.Warehouse.Count > 0)
				{
                    FirstName.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "FirstName");
					LastName.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "LastName");
					Organization.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "Organization");
					Line1.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "Line1");
					Line2.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "Line2");
					City.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "City");
					State.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "State");
					CountryCode.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "CountryCode");
					CountryName.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "CountryName");
					PostalCode.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "PostalCode");
					RegionCode.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "RegionCode");
					RegionName.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "RegionName");
					DayTimePhone.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "DaytimePhoneNumber");
					EveningPhone.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "EveningPhoneNumber");
					FaxNumber.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "FaxNumber");
                    Email.Text = GetFieldValue(_WarehouseDto.Warehouse[0], "Email");

					//ManagementHelper.SelectListItem(DisplayTemplate, _CatalogEntryDto.CatalogEntry[0].TemplateName);
					//ManagementHelper.SelectListItem(MetaClassList, _CatalogEntryDto.CatalogEntry[0].MetaClassId);
				}
			}
			else
			{
			}
		}

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetFieldValue(DataRow row, string name)
        {
            if (row.IsNull(name))
                return String.Empty;

            return row[name].ToString();
        }

		#region IAdminTabControl Members

		public void SaveChanges(IDictionary context)
		{
			WarehouseDto dto = (WarehouseDto)context[_WarehouseDtoString];
			WarehouseDto.WarehouseRow row = null;

			if (dto.Warehouse == null || dto.Warehouse.Count == 0)
				row = dto.Warehouse.NewWarehouseRow();
			else
				row = dto.Warehouse[0];

			row.FirstName = FirstName.Text;
			row.LastName = LastName.Text;
			row.Organization = Organization.Text;
			row.Line1 = Line1.Text;
			row.Line2 = Line2.Text;
			row.City = City.Text;
			row.State = State.Text;
			row.CountryCode = CountryCode.Text;
			row.CountryName = CountryName.Text;
			row.PostalCode = PostalCode.Text;
			row.RegionCode = RegionCode.Text;
			row.RegionName = RegionName.Text;
			row.DaytimePhoneNumber = DayTimePhone.Text;
			row.EveningPhoneNumber = EveningPhone.Text;
			row.FaxNumber = FaxNumber.Text;
			row.Email = Email.Text;

			//row.TemplateName = DisplayTemplate.SelectedValue;
			//row.MetaClassId = Int32.Parse(MetaClassList.SelectedValue);

			if (row.RowState == DataRowState.Detached)
				dto.Warehouse.Rows.Add(row);
		}
		#endregion

		#region IAdminContextControl Members

		public void LoadContext(IDictionary context)
		{
			_WarehouseDto = (WarehouseDto)context[_WarehouseDtoString];
		}

		#endregion
	}
}