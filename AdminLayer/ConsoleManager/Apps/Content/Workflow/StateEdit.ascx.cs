using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Manager.Core;

namespace Mediachase.Commerce.Manager.Apps.Content.Workflow
{
	public partial class StateEdit : BaseUserControl
	{
        /// <summary>
        /// Gets the workflow id.
        /// </summary>
        /// <value>The workflow id.</value>
		public int WorkflowId
		{
			get
			{
				return Mediachase.Web.Console.Common.ManagementHelper.GetIntegerValue(Request.QueryString["WorkflowId"], 0);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			EditSaveControl.CancelClientScript = String.Format("CSManagementClient.ChangeView('Content','State-List', 'WorkflowId={0}');", WorkflowId);
			EditSaveControl.SavedClientScript = String.Format("CSManagementClient.ChangeView('Content','State-List', 'WorkflowId={0}');", WorkflowId);
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

			try
			{
				ViewControl.SaveChanges(null);
			}
			catch (Exception ex)
			{
				e.RunScript = false;
				DisplayErrorMessage(ex.Message);
			}
		}
	}
}