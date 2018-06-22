using System;
using Mediachase.Commerce.Profile;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Profile
{
    public partial class MetaClassList : ProfileBaseUserControl
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
            DataBind();
        }

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Init(object sender, EventArgs e)
        {
            MetaClassEditControl1.MDContext = ProfileContext.MetaDataContext;
        }
    }
}