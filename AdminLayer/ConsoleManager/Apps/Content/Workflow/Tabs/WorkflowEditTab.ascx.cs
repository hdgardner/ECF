using System;
using System.Collections;
using System.Data;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using mc = Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Apps.Content.Workflow.Tabs
{
	public partial class WorkflowEditTab : BaseUserControl, IAdminTabControl
	{
        /// <summary>
        /// Gets the workflow id.
        /// </summary>
        /// <value>The workflow id.</value>
		public int WorkflowId
		{
			get
			{
				return ManagementHelper.GetIntegerValue(Request.QueryString["WorkflowId"], 0);
			}
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindForm();
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			if (WorkflowId > 0)
			{
				using (IDataReader reader = mc.Workflow.LoadById(WorkflowId))
				{
					if (reader.Read())
					{
						Name.Text = reader["FriendlyName"].ToString();
						IsDefault.IsSelected = (bool)reader["IsDefault"];
					}
                    reader.Close();
				}
			}
		}

		#region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
		public void SaveChanges(IDictionary context)
		{
			if (WorkflowId > 0)
			{
				// update workflow
				try
				{
					mc.Workflow.UpdateWorkflow(WorkflowId, Name.Text, IsDefault.IsSelected);
				}
				catch (Exception ex)
				{
					DisplayErrorMessage(ex.Message);
					return;
				}
			}
			else
			{
				// add a new workflow
				try
				{
					mc.Workflow.AddWorkflow(Name.Text, IsDefault.IsSelected);
				}
				catch (Exception ex)
				{
					DisplayErrorMessage(ex.Message);
					return;
				}
			}
		}
		#endregion
	}
}