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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Manager.Order
{
    public partial class MetaClassEdit : OrderBaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                LoadDataAndDataBind(); 
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
            base.DataBind();
        }

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			MetaClassEditControl1.MDContext = OrderContext.MetaDataContext;
            MetaClassEditControl1.SaveControl.SaveChanges += new EventHandler<Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs>(SaveControl_SaveChanges);
		}

        /// <summary>
        /// Handles the SaveChanges event of the SaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs"/> instance containing the event data.</param>
        void SaveControl_SaveChanges(object sender, Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs e)
        {
            // Clear meta cache on save
            OrderContext.Current.ClearMetaCache();
        }
    }
}