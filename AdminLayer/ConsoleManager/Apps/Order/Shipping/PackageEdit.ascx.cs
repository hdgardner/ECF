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
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Shipping
{
	public partial class PackageEdit : OrderBaseUserControl
	{
		private const string _ShippingMethodDtoEditSessionKey = "ECF.ShippingMethodDto.Edit";
		private const string _PackageIdString = "PackageId";
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		/// <summary>
		/// Gets the PackageId.
		/// </summary>
		/// <value>The PackageId.</value>
		public int PackageId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_PackageIdString);
			}
		}

		/// <summary>
		/// Gets the package session key.
		/// </summary>
		/// <returns></returns>
		private string GetPackageSessionKey()
		{
			return _ShippingMethodDtoEditSessionKey + "_package_" + PackageId;
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
            //Check permissions based on whether the page is loading for edit or creating a new package
            if (PackageId > 0)
                SecurityManager.CheckRolePermission("order:admin:shipping:packages:mng:edit");
            else
                SecurityManager.CheckRolePermission("order:admin:shipping:packages:mng:create");
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
		private ShippingMethodDto LoadFresh()
		{
			ShippingMethodDto sm = ShippingManager.GetShippingPackage(PackageId);

			// persist in session
			Session[GetPackageSessionKey()] = sm;

			return sm;
		}

		/// <summary>
		/// Creates the empty dto.
		/// </summary>
		/// <param name="sm">The sm.</param>
		/// <param name="persistInSession">if set to <c>true</c> [persist in session].</param>
		private void CreateEmptyDto(ref ShippingMethodDto sm, bool persistInSession)
		{
			if (sm == null || sm.Package.Count == 0)
			{
				sm = new ShippingMethodDto();
				if (persistInSession)
					Session[GetPackageSessionKey()] = sm;
			}
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			ShippingMethodDto sm = null;

			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
			{
				sm = LoadFresh();

				CreateEmptyDto(ref sm, true);
			}
			else // load from session
			{
				sm = (ShippingMethodDto)Session[GetPackageSessionKey()];

				if (sm == null)
					sm = LoadFresh();
			}

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_ShippingMethodDtoString, sm);

			// Call tabs load context
			ViewControl.LoadContext(dic);
		}

		/// <summary>
		/// Handles the SaveChanges event of the EditSaveControl control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
		{
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			ShippingMethodDto sm = null;

			if (PackageId != 0)
				sm = (ShippingMethodDto)Session[GetPackageSessionKey()];

			if (sm == null && PackageId != 0)
				sm = ShippingManager.GetShippingPackage(PackageId);

			CreateEmptyDto(ref sm, true);

			IDictionary dic = new ListDictionary();
			dic.Add(_ShippingMethodDtoString, sm);

			ViewControl.SaveChanges(dic);

			// save changes
			if (sm.HasChanges())
				ShippingManager.SavePackage(sm);

			ViewControl.CommitChanges(dic);

			// we don't need to store Dto in session any more
			Session.Remove(GetPackageSessionKey());
		}
	}
}