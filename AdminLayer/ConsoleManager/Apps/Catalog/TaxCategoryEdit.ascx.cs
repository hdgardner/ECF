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
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Commerce.Manager.Catalog
{
	public partial class TaxCategoryEdit : CatalogBaseUserControl
	{
		CatalogTaxDto _taxDto = null;

		/// <summary>
		/// Gets the TaxCategory id.
		/// </summary>
		/// <value>The TaxCategory id.</value>
		public int TaxCategoryId
		{
			get
			{
				return ManagementHelper.GetIntFromQueryString("tcId");
			}
		}

		/// <summary>
		/// Gets the TaxCategoryDto.
		/// </summary>
		public CatalogTaxDto CurrentDto
		{
			get
			{
				if (_taxDto == null)
					_taxDto = CatalogTaxManager.GetTaxCategoryByTaxCategoryId(TaxCategoryId);

				return _taxDto;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                if (TaxCategoryId > 0)
                    SecurityManager.CheckRolePermission("catalog:admin:taxcategories:mng:edit");
                else
                    SecurityManager.CheckRolePermission("catalog:admin:taxcategories:mng:create");

                BindForm();
            }
		}

		private void BindForm()
		{
			if (CurrentDto != null && CurrentDto.TaxCategory.Count > 0)
				TaxCategoryName.Text = CurrentDto.TaxCategory[0].Name;
			else
				TaxCategoryName.Text = String.Empty;
		}

        /// <summary>
        /// Checks if entered tax category name is unique.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        public void TaxCategoryNameCheck(object sender, ServerValidateEventArgs args)
        {
            // load tax category by name
            CatalogTaxDto dto = CatalogTaxManager.GetTaxCategoryByName(args.Value);

            // check if tax category with specified name is loaded
            if (dto != null && dto.TaxCategory.Count > 0 &&
                dto.TaxCategory[0].TaxCategoryId != TaxCategoryId)
            {
                args.IsValid = false;
                return;
            }

            args.IsValid = true;
        }

		/// <summary>
		/// Handles the ServerClick event of the btnCreate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void btnOK_Click(object sender, EventArgs e)
		{
            // Validate form
            if (!this.Page.IsValid)
            {
                return;
            }

            bool error = false;
			try
			{
				ProcessCommand();
			}
			catch (Exception ex)
			{
				string errorMessage = ex.Message.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), "__TaxCategoryEditFrameError",
					String.Format("alert('{0}{1}');", "Operation failed.\\nError: ", errorMessage), true);
				error = true;
			}

			// if operation succeeded, close dialog
			if (!error)
				CommandHandler.RegisterCloseOpenedFrameScript(this.Page, String.Empty);
		}

		/// <summary>
		/// Handles the PreRender event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Request["closeFramePopup"] != null)
			{
				btnClose.OnClientClick = String.Format("javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
			}
		}

		private void ProcessCommand()
		{
			if (CurrentDto != null && CurrentDto.TaxCategory.Count > 0)
			{
				// update tax category
				CurrentDto.TaxCategory[0].Name = TaxCategoryName.Text;
				CatalogTaxManager.SaveTaxCategory(CurrentDto);
			}
			else
			{
				// create new tax category
				CatalogTaxManager.CreateTaxCategory(TaxCategoryName.Text, false);
			}
		}
	}
}