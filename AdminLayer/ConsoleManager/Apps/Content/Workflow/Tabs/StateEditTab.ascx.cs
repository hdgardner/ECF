using System;
using System.Collections;
using System.Data;
using System.Web.Security;
using System.Web.UI.WebControls;
using Mediachase.Data.Provider;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Interfaces;
using mc = Mediachase.Cms;

namespace Mediachase.Commerce.Manager.Apps.Content.Workflow.Tabs
{
	public partial class StateEditTab : BaseUserControl, IAdminTabControl
	{
        /// <summary>
        /// Gets the status id.
        /// </summary>
        /// <value>The status id.</value>
		public int StatusId
		{
			get
			{
				return ManagementHelper.GetIntegerValue(Request.QueryString["StatusId"], 0);
			}
		}

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
			rvWeightRange.MinimumValue = "0";
			rvWeightRange.MaximumValue = Int32.MaxValue.ToString();

			if (!IsPostBack)
				BindForm();
		}

        /// <summary>
        /// Binds the form.
        /// </summary>
		private void BindForm()
		{
			if (StatusId > 0)
			{
				using (IDataReader reader = mc.WorkflowStatus.LoadById(StatusId))
				{
					if (reader.Read())
					{
						Name.Text = reader["FriendlyName"].ToString();
						tbWeight.Text = reader["Weight"].ToString();
					}
                    reader.Close();
				}
			}

			BindRoles();
		}

        /// <summary>
        /// Binds the roles.
        /// </summary>
		private void BindRoles()
		{
			RolesList.DataSource = Roles.GetAllRoles();
			RolesList.DataBind();

			if (StatusId > 0)
			{
				using (IDataReader reader = mc.WorkflowAccess.LoadByStatusId(StatusId))
				{
					while (reader.Read())
					{
						ListItem li = RolesList.Items.FindByValue((string)reader["RoleId"]);
						if (li != null)
							li.Selected = true;
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
			if (StatusId > 0)
			{
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						mc.WorkflowStatus.UpdateStatus(StatusId, WorkflowId, Int32.Parse(tbWeight.Text), Name.Text);

						// update roles
						foreach (ListItem item in RolesList.Items)
						{
							// load role
							int statusAccessId = 0;
							using (IDataReader reader = mc.WorkflowAccess.LoadByRoleIdStatusIdNotEveryone(item.Value, StatusId))
							{
								if (reader.Read())
									statusAccessId = (int)reader["StatusAccessId"];

                                reader.Close();
							}

							// if role exists and is not selected, remove it
							if (!item.Selected && statusAccessId > 0)
								mc.WorkflowAccess.DeleteAccess(statusAccessId);
							else
								// role is not in the db, add it
								if (item.Selected && statusAccessId == 0)
									mc.WorkflowAccess.AddAccess(StatusId, item.Value);
						}

						scope.Complete();
					}
					catch (Exception ex)
					{
						DisplayErrorMessage(ex.Message);
						return;
					}
				}
			}
			else
			{
				// add a new state
				using (TransactionScope scope = new TransactionScope(IsolationLevel.ReadUncommitted))
				{
					try
					{
						int statusId = mc.WorkflowStatus.AddStatus(WorkflowId, Int32.Parse(tbWeight.Text), Name.Text);

						// add roles
						foreach (ListItem item in RolesList.Items)
							// if role exists and is not selected, remove it
							if (item.Selected)
								mc.WorkflowAccess.AddAccess(statusId, item.Value);

						scope.Complete();
					}
					catch (Exception ex)
					{
						DisplayErrorMessage(ex.Message);
						return;
					}
				}
			}
		}
		#endregion
	}
}