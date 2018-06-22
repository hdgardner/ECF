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

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
    public partial class CatalogPermissionsTab : BaseUserControl, IAdminTabControl
    {
        /// <summary>
        /// Gets the catalog id.
        /// </summary>
        /// <value>The catalog id.</value>
        public int CatalogId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("CatalogId");
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Binds the form.
        /// </summary>
        private void BindForm()
        {
            if (CatalogId > 0)
            {
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
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
            CatalogDto dto = (CatalogDto)context["Catalog"];

        }
        #endregion
    }
}