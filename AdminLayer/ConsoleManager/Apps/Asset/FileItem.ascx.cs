using System;
using System.Collections;
using System.Collections.Specialized;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Asset
{
    public partial class FileItem : BaseUserControl
    {
        /// <summary>
        /// Gets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int ObjectId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString("objectid");
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();

            if (!this.IsPostBack)
                DataBind();
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (ObjectId > 0)
            {
                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();

                // Call tabs load context
                ViewControl.LoadContext(dic);
            }
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

            // Put a dictionary key that can be used by other tabs
            IDictionary dic = new ListDictionary();

            ViewControl.SaveChanges(dic);
        }
     }
}