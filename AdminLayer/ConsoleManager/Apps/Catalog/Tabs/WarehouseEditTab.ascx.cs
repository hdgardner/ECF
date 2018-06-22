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
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class WarehouseEditTab : CatalogBaseUserControl, IAdminTabControl, IAdminContextControl
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

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
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
					this.Name.Text = _WarehouseDto.Warehouse[0].Name;
					this.CodeText.Text = _WarehouseDto.Warehouse[0].Code;
					this.SortOrder.Text = _WarehouseDto.Warehouse[0].SortOrder.ToString();
					this.IsActive.IsSelected = _WarehouseDto.Warehouse[0].IsActive;
					this.IsPrimary.IsSelected = _WarehouseDto.Warehouse[0].IsPrimary;
				}
			}
			else
			{
				this.SortOrder.Text = "0";
			}
		}

		#region IAdminTabControl Members

		public void SaveChanges(IDictionary context)
		{
			WarehouseDto dto = (WarehouseDto)context[_WarehouseDtoString];
			WarehouseDto.WarehouseRow row = null;

			if (dto.Warehouse.Count > 0)
			{
				row = dto.Warehouse[0];
                row.Modified = DateTime.UtcNow;
			}
			else
			{
				row = dto.Warehouse.NewWarehouseRow();
                row.Created = DateTime.UtcNow;
                row.Modified = DateTime.UtcNow;
				row.CreatorId = Page.User.Identity.Name;
				row.IsPrimary = IsPrimary.IsSelected;
			}

			row.ModifierId = Page.User.Identity.Name;
			row.Name = Name.Text;
			row.Code = CodeText.Text;
			row.IsActive = this.IsActive.IsSelected;
			row.SortOrder = Int32.Parse(this.SortOrder.Text);
			row.ApplicationId = CatalogConfiguration.Instance.ApplicationId;

			if (row.RowState == DataRowState.Detached)
				dto.Warehouse.Rows.Add(row);
		}

		#endregion

        #region Custom Validators
        /// <summary>
        /// Checks if entered warehouse name is unique.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void WarehouseNameCheck(object sender, ServerValidateEventArgs args)
        {
            // check if warehouse with specified name is exsist
            int warehouseId = WarehouseManager.GetWarehouseIdByName(args.Value);
            if (warehouseId > 0 && warehouseId != WarehouseId)
            {
                args.IsValid = false;
                return;
            }
            
            args.IsValid = true;
        }

        /// <summary>
        /// Checks if entered varehouse code is unique.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void WarehouseCodeCheck(object sender, ServerValidateEventArgs args)
        {
            // check if warehouse with specified code is exsist
            int warehouseId = WarehouseManager.GetWarehouseIdByCode(args.Value);
            if (warehouseId > 0 && warehouseId != WarehouseId)
            {
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
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