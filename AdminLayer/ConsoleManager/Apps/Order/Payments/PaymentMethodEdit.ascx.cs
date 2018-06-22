using System;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Payments
{
	/// <summary>
	///	Summary description for PaymentMethodEdit.
	/// </summary>
	public partial class PaymentMethodEdit : OrderBaseUserControl
	{
		private const string _PaymentMethodDtoEditSessionKey = "ECF.PaymentMethodDto.Edit";
		private const string _PaymentMethodIdString = "PaymentMethodId";
		private const string _PaymentMethodDtoString = "PaymentMethodDto";

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
        /// Gets the payment method session key.
        /// </summary>
        /// <returns></returns>
		private string GetPaymentMethodSessionKey()
		{
			return _PaymentMethodDtoEditSessionKey + "_" + PaymentMethodId;
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
            //Check permissions based on whether the page is loading for edit or creating a new package
            if (PaymentMethodId != Guid.Empty)
                SecurityManager.CheckRolePermission("order:admin:payments:mng:edit");
            else
                SecurityManager.CheckRolePermission("order:admin:payments:mng:create");
            
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
		private PaymentMethodDto LoadFresh()
		{
			PaymentMethodDto pm = PaymentManager.GetPaymentMethod(PaymentMethodId, true);

			// persist in session
			Session[GetPaymentMethodSessionKey()] = pm;

			return pm;
		}

        /// <summary>
        /// Creates the empty dto.
        /// </summary>
        /// <param name="pm">The pm.</param>
        /// <param name="persistInSession">if set to <c>true</c> [persist in session].</param>
		private void CreateEmptyDto(ref PaymentMethodDto pm, bool persistInSession)
		{
			if (pm == null || pm.PaymentMethod.Count == 0)
			{
				pm = new PaymentMethodDto();
				if (persistInSession)
					Session[GetPaymentMethodSessionKey()] = pm;
			}
		}

        /// <summary>
        /// Loads the context.
        /// </summary>
		private void LoadContext()
		{
			PaymentMethodDto pm = null;
			
			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
			{
				pm = LoadFresh();

				CreateEmptyDto(ref pm, true);
			}
			else // load from session
			{
				pm = (PaymentMethodDto)Session[GetPaymentMethodSessionKey()];

				if (pm == null)
					pm = LoadFresh();
			}

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_PaymentMethodDtoString, pm);

			// Call tabs load context
			ViewControl.LoadContext(dic);

			// set redirect functiond for editsave control
			string langId = String.Empty;
			if (pm != null && pm.PaymentMethod != null && pm.PaymentMethod.Count > 0)
				langId = pm.PaymentMethod[0].LanguageId;
			else
				langId = LanguageCode;

			EditSaveControl.CancelClientScript = "CSOrderClient.PaymentMethodSaveRedirect('" + langId + "');";
			EditSaveControl.SavedClientScript = "CSOrderClient.PaymentMethodSaveRedirect('" + langId + "');";
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

			PaymentMethodDto pm = null;

			if (PaymentMethodId != Guid.Empty)
				pm = (PaymentMethodDto)Session[_PaymentMethodDtoEditSessionKey];

			if (pm == null && PaymentMethodId != Guid.Empty)
				pm = PaymentManager.GetPaymentMethod(PaymentMethodId, true);

			CreateEmptyDto(ref pm, true);

			IDictionary dic = new ListDictionary();
			dic.Add(_PaymentMethodDtoString, pm);

			ViewControl.SaveChanges(dic);

			// save changes
			if (pm.HasChanges())
				PaymentManager.SavePayment(pm);

			ViewControl.CommitChanges(dic);

			// we don't need to store Dto in session any more
			Session.Remove(GetPaymentMethodSessionKey());
		}
	}
}