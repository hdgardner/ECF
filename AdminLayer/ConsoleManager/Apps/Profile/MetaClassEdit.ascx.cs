using System;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Profile
{
    public partial class MetaClassEdit : ProfileBaseUserControl
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
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            MetaFieldEditControl1.MDContext = ProfileContext.MetaDataContext;
        }
    }
}