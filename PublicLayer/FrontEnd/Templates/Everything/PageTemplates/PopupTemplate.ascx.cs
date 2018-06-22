using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;

namespace Mediachase.Cms.Website.Templates.Everything.PageTemplates
{
    public partial class PopupTemplate : BaseStoreUserControl, IPublicTemplate
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.EnsureID();
            if (!IsPostBack)
                DataBind();
        }

        #region IPublicTemplate Members

        /// <summary>
        /// Gets the control places.
        /// </summary>
        /// <value>The control places.</value>
        public string ControlPlaces
        {
            get
            {

                return "MainContentArea";
            }
        }

        #endregion

    }
}