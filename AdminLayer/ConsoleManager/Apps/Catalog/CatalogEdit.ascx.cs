using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class CatalogEdit : CatalogBaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                DataBind();
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

            CatalogDto catalog = null;

            if (Parameters["CatalogId"] != null)
            {
                catalog = CatalogContext.Current.GetCatalogDto(Int32.Parse(Parameters["CatalogId"].ToString()));
            }
            else
            {
                catalog = new CatalogDto();
            }

            IDictionary context = new ListDictionary();

            context.Add("Catalog", catalog);

            ViewControl.SaveChanges(context);

            int catalogId = catalog.Catalog[0].CatalogId;

            if (catalog.HasChanges())
                CatalogContext.Current.SaveCatalog(catalog);
        }
    }
}