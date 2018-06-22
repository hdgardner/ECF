using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Profile;

namespace Mediachase.Commerce.Manager.Profile
{
    public partial class PrincipalEdit : ProfileBaseUserControl
    {
        /// <summary>
        /// Gets the principal id.
        /// </summary>
        /// <value>The principal id.</value>
        public Guid PrincipalId
        {
            get
            {
                return new Guid(Request.QueryString["id"]);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!String.IsNullOrEmpty(Request.QueryString["mode"]) && String.Compare(Request.QueryString["mode"].ToString(), "myprofile", true) == 0)
			{
				return;
			}

            ////first check permissions
            //if (String.IsNullOrEmpty(ProfileContext.Current.UserId))
            //{
            //    //if permissions not present, deny
            //    if (!ProfileContext.Current.CheckPermission("profile:acc:mng:create"))
            //        throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            //}
            //else
            //{
            //    //if permissions not present, deny
            //    if (!ProfileContext.Current.CheckPermission("profile:acc:mng:edit"))
            //        throw new UnauthorizedAccessException("Current user does not have enough rights to access the requested operation.");
            //}
            
            if (String.Compare(Request.QueryString["_v"], "Account-View", true) == 0)
			{
				EditSaveControl.CancelClientScript = "CSManagementClient.ChangeView('Profile','Account-List');";
				EditSaveControl.SavedClientScript = "CSManagementClient.ChangeView('Profile', 'Account-List');";
			}
			else
			{
				EditSaveControl.CancelClientScript = "CSManagementClient.ChangeView('Profile','Organization-List');";
				EditSaveControl.SavedClientScript = "CSManagementClient.ChangeView('Profile', 'Organization-List');";
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
            ViewControl.ViewId = Request.QueryString["_v"];
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void EditSaveControl_SaveChanges(object sender, Mediachase.Commerce.Manager.Core.SaveControl.SaveEventArgs e)
        {
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

            try
            {
                ViewControl.SaveChanges(null);
            }
            catch (MembershipCreateUserException ex)
            {
                e.RunScript = false;
                DisplayErrorMessage(ex.Message);
            }
        }
    }
}