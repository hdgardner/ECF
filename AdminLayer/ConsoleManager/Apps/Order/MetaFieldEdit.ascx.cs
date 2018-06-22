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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Manager.Order
{
    public partial class MetaFieldEdit : OrderBaseUserControl
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {
            LoadDataAndDataBind();  
            base.DataBind();            
        }

        /// <summary>
        /// Loads the data and data bind.
        /// </summary>
        private void LoadDataAndDataBind()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            MetaFieldEditControl1.MDContext = OrderContext.MetaDataContext;
            MetaFieldEditControl1.SaveControl.SaveChanges += new EventHandler<Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs>(SaveControl_SaveChanges);
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