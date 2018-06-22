using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Shipping
{
	/// <summary>
	///		Summary description for ShippingOptionEdit.
	/// </summary>
	public partial class ShippingOptionEdit : OrderBaseUserControl
	{
		private const string _ShippingMethodDtoEditSessionKey = "ECF.ShippingMethodDto.Edit";
		private const string _ShippingOptionIdString = "ShippingOptionId";
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

        /// <summary>
        /// Gets the shipping option id.
        /// </summary>
        /// <value>The shipping option id.</value>
		public Guid ShippingOptionId
		{
			get
			{
				return ManagementHelper.GetGuidFromQueryString("soid");
			}
		}

        /// <summary>
        /// Gets the shipping method session key.
        /// </summary>
        /// <returns></returns>
		private string GetShippingMethodSessionKey()
		{
			return _ShippingMethodDtoEditSessionKey + "_so_" + ShippingOptionId;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            //Check permissions based on whether the page is loading for edit or creating a new package
            if (ShippingOptionId != Guid.Empty)
                SecurityManager.CheckRolePermission("order:admin:shipping:providers:mng:edit");
            else
                SecurityManager.CheckRolePermission("order:admin:shipping:providers:mng:create");
            
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
			ShippingMethodDto sm = ShippingManager.GetShippingOption(ShippingOptionId, true);

			// persist in session
			Session[GetShippingMethodSessionKey()] = sm;

			return sm;
		}

        /// <summary>
        /// Creates the empty dto.
        /// </summary>
        /// <param name="sm">The sm.</param>
        /// <param name="persistInSession">if set to <c>true</c> [persist in session].</param>
		private void CreateEmptyDto(ref ShippingMethodDto sm, bool persistInSession)
		{
			if (sm == null || sm.ShippingOption.Count == 0)
			{
				sm = new ShippingMethodDto();
				if (persistInSession)
					Session[GetShippingMethodSessionKey()] = sm;
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
				sm = (ShippingMethodDto)Session[GetShippingMethodSessionKey()];

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

			if (ShippingOptionId != Guid.Empty)
				sm = (ShippingMethodDto)Session[GetShippingMethodSessionKey()];

			if (sm == null && ShippingOptionId != Guid.Empty)
				sm = ShippingManager.GetShippingOption(ShippingOptionId, true);

			CreateEmptyDto(ref sm, true);

			IDictionary dic = new ListDictionary();
			dic.Add(_ShippingMethodDtoString, sm);

			ViewControl.SaveChanges(dic);

			// save changes
			if (sm.HasChanges())
				ShippingManager.SaveShipping(sm);

			ViewControl.CommitChanges(dic);

			// we don't need to store Dto in session any more
			Session.Remove(GetShippingMethodSessionKey());
		}
	}
}
