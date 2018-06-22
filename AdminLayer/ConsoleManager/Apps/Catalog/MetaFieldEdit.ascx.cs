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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Shared;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Catalog
{
    public partial class MetaFieldEdit : CatalogBaseUserControl
    {

        #region ObjectId
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int ObjectId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("id");
            }
        }
        #endregion
        
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //check permissions first
                if (ObjectId > 0)
                    SecurityManager.CheckRolePermission("catalog:admin:meta:fld:mng:edit");
                else
                    SecurityManager.CheckRolePermission("catalog:admin:meta:fld:mng:create");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            MetaFieldEditControl1.MDContext = CatalogContext.MetaDataContext;
        }
    }
}