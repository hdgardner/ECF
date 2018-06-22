using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using ComponentArt.Web.UI;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class EntryBundleItemsEditTab : CatalogBaseUserControl, IAdminTabControl
    {

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            EditTab.RelationTypeId = EntryRelationType.BundleEntry;
        }


        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            EditTab.ServiceMethod = "GetEntryList";
            base.DataBind();
            BindForm();
        }


        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            EditTab.RelationTypeId = EntryRelationType.BundleEntry;

            // Save changes
            EditTab.SaveChanges(context);
        }
        #endregion
    }
}