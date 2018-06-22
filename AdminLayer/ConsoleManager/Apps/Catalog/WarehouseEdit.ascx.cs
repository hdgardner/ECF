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
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Catalog
{
	public partial class WarehouseEdit : CatalogBaseUserControl
	{
		private const string _WarehouseDtoEditSessionKey = "ECF.WarehouseDto.Edit";
		private const string _WarehouseIdString = "WarehouseId";
		private const string _WarehouseDtoString = "WarehouseDto";

		private WarehouseDto _Warehouse = null;

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
            if (WarehouseId > 0)
                SecurityManager.CheckRolePermission("catalog:admin:warehouses:mng:edit");
            else
                SecurityManager.CheckRolePermission("catalog:admin:warehouses:mng:create");

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
		private WarehouseDto LoadFresh()
		{
			WarehouseDto warehouse = WarehouseManager.GetWarehouseByWarehouseId(WarehouseId);

			// persist in session
			Session[_WarehouseDtoEditSessionKey] = warehouse;

			return warehouse;
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			if (WarehouseId > 0)
			{
				WarehouseDto warehouse = null;
				if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
					warehouse = LoadFresh();
				else // load from session
				{
					warehouse = (WarehouseDto)Session[_WarehouseDtoEditSessionKey];

					if (warehouse == null)
						warehouse = LoadFresh();
				}

				// Put a dictionary key that can be used by other tabs
				IDictionary dic = new ListDictionary();
				dic.Add(_WarehouseDtoString, warehouse);

				// Call tabs load context
				ViewControl.LoadContext(dic);

				_Warehouse = warehouse;
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

			WarehouseDto warehouse = (WarehouseDto)Session[_WarehouseDtoEditSessionKey];

			if (WarehouseId > 0 && warehouse == null)
				warehouse = WarehouseManager.GetWarehouseByWarehouseId(WarehouseId);
			else if (WarehouseId == 0)
				warehouse = new WarehouseDto();

			IDictionary context = new ListDictionary();
			context.Add(_WarehouseDtoString, warehouse);

			ViewControl.SaveChanges(context);

			if (warehouse.HasChanges())
				WarehouseManager.SaveWarehouse(warehouse);

			// we don't need to store Dto in session any more
			Session.Remove(_WarehouseDtoEditSessionKey);
		}
	}
}