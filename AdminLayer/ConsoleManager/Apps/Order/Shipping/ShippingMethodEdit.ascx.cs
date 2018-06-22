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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Shipping
{
	/// <summary>
	///		Summary description for ShippingMethodEdit.
	/// </summary>
	public partial class ShippingMethodEdit : OrderBaseUserControl
	{
		private const string _ShippingMethodDtoEditSessionKey = "ECF.ShippingMethodDto.Edit";
		private const string _ShippingMethodIdString = "ShippingMethodId";
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

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
        /// Gets the shipping method session key.
        /// </summary>
        /// <returns></returns>
		private string GetShippingMethodSessionKey()
		{
			return _ShippingMethodDtoEditSessionKey + "_sm_" + ShippingMethodId;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
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
		private ShippingMethodDto LoadFresh()
		{
			ShippingMethodDto sm = ShippingManager.GetShippingMethod(ShippingMethodId, true);

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
			if (sm == null || sm.ShippingMethod.Count == 0)
			{
				// fill dto with all available shipping options. They will be needed for contraints to work properly when a new shipping method is created.
				sm = ShippingManager.GetShippingMethods(LanguageCode, true);

				// remove shipping methods, because we need an empty dto.
				if (sm != null)
				{
                    if (sm.ShippingMethod.Count > 0)
                    {
						while (sm.ShippingMethod.Rows.Count > 0)
							sm.ShippingMethod.Rows.RemoveAt(0);
                    }
				}

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

			// set redirect functiond for editsave control
			string langId = String.Empty;
			if (sm != null && sm.ShippingMethod != null && sm.ShippingMethod.Count > 0)
				langId = sm.ShippingMethod[0].LanguageId;
			else
				langId = LanguageCode;

			EditSaveControl.CancelClientScript = "CSOrderClient.ShippingMethodSaveRedirect('" + langId + "');";
			EditSaveControl.SavedClientScript = "CSOrderClient.ShippingMethodSaveRedirect('" + langId + "');";
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

			if (ShippingMethodId != Guid.Empty)
				sm = (ShippingMethodDto)Session[GetShippingMethodSessionKey()];

			if (sm == null && ShippingMethodId != Guid.Empty)
				sm = ShippingManager.GetShippingMethod(ShippingMethodId, true);

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
