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

namespace Mediachase.Commerce.Manager.Order
{
	public partial class TaxEdit : OrderBaseUserControl
	{
		private const string _TaxDtoEditSessionKey = "ECF.TaxDto.Edit";
		private const string _TaxIdString = "taxid";
		private const string _TaxDtoString = "TaxDto";

		private TaxDto _TaxDto = null;

		/// <summary>
		/// Gets the tax id.
		/// </summary>
		/// <value>The tax id.</value>
		public int TaxId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString(_TaxIdString);
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
		protected void Page_Load(object sender, EventArgs e)
		{
            //first check for permissions for create or edit permissions. If TaxId > 0, its an edit
            if (TaxId > 0)
                SecurityManager.CheckRolePermission("order:admin:taxes:mng:edit");
            else
                SecurityManager.CheckRolePermission("order:admin:taxes:mng:create");
            
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
		private TaxDto LoadFresh()
		{
			TaxDto tax = TaxManager.GetTax(TaxId);

			// persist in session
			Session[_TaxDtoEditSessionKey] = tax;

			return tax;
		}

		/// <summary>
		/// Creates the empty dto.
		/// </summary>
		/// <param name="tax">The pm.</param>
		/// <param name="persistInSession">if set to <c>true</c> [persist in session].</param>
		private void CreateEmptyDto(ref TaxDto tax, bool persistInSession)
		{
			if (tax == null || tax.Tax.Count == 0)
			{
				tax = new TaxDto();
				if (persistInSession)
					Session[_TaxDtoEditSessionKey] = tax;
			}
		}

		/// <summary>
		/// Loads the context.
		/// </summary>
		private void LoadContext()
		{
			TaxDto tax = null;

			if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
			{
				tax = LoadFresh();

				CreateEmptyDto(ref tax, true);
			}
			else // load from session
			{
				tax = (TaxDto)Session[_TaxDtoEditSessionKey];

				if (tax == null)
					tax = LoadFresh();
			}

			// Put a dictionary key that can be used by other tabs
			IDictionary dic = new ListDictionary();
			dic.Add(_TaxDtoString, tax);

			// Call tabs load context
			ViewControl.LoadContext(dic);

			// set redirect functiond for editsave control
			string langId = String.Empty;
			//if (tax != null && tax.TaxLanguage != null && tax.TaxLanguage.Count > 0)
			//    langId = tax.TaxLanguage[0].LanguageId;
			//else
			//	langId = LanguageCode;

			EditSaveControl.CancelClientScript = "CSOrderClient.TaxSaveRedirect();";
			EditSaveControl.SavedClientScript = "CSOrderClient.TaxSaveRedirect();";
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

			TaxDto tax = null;

			if (TaxId > 0)
				tax = (TaxDto)Session[_TaxDtoEditSessionKey];

			if (tax == null && TaxId > 0)
				tax = TaxManager.GetTax(TaxId);

			CreateEmptyDto(ref tax, true);

			IDictionary dic = new ListDictionary();
			dic.Add(_TaxDtoString, tax);

			ViewControl.SaveChanges(dic);

			// save changes
			if (tax.HasChanges())
				TaxManager.SaveTax(tax);

			ViewControl.CommitChanges(dic);

			// we don't need to store Dto in session any more
			Session.Remove(_TaxDtoEditSessionKey);
		}
	}
}