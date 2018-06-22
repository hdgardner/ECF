using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Profile;
using Mediachase.Cms.Pages;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
	public class VersionApproveHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;

				string gridId = cp.CommandArguments["GridId"];
				string[] items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);

				if (items != null)
				{
					int error = 0;
					string errorMessage = String.Empty;
					try
					{
                        ProcessApproveCommand(items);

						ManagementHelper.SetBindGridFlag(gridId);
					}
					catch (Exception ex)
					{
						error++;
						errorMessage = ex.Message;
					}

					if (error > 0)
					{
						errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
						ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
							String.Format("alert('{0}{1}');", "Failed to delete item(s). Error: ", errorMessage), true);
					}
				}
				else
					return;
			}
		}
		#endregion

		void ProcessApproveCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int versionId = Int32.Parse(keys[0]);

                    using (IDataReader reader = PageVersion.GetVersionById(versionId))
                    {
                        if (reader.Read())
                        {
                            int templateId = (int)reader["TemplateId"];
                            int statusId = (int)reader["StatusId"];
                            int stateId = (int)reader["StateId"];
                            int langId = (int)reader["LangId"];

                            int newStatus = WorkflowAccess.GetNextStatusId(statusId);

                            if (newStatus > 0)
                                PageVersion.UpdatePageVersion(versionId, templateId, langId, statusId, newStatus, ProfileContext.Current.UserId, 1, "");
                                //PageVersion.UpdatePageVersion(CMSContext.Current.VersionId, templateId, langId, statusId, WorkflowAccess.GetMaxStatus(Roles.GetRolesForUser(), WorkflowStatus.GetLast(statusId)), (Guid)Membership.GetUser(Page.User.Identity.Name).ProviderUserKey, 1, string.Empty);

                            // if we publish version
                            if (newStatus == WorkflowStatus.GetLast(statusId))
                            {
                                //find old publishd and put to archive
                                using (IDataReader reader2 = PageVersion.GetVersionByStatusId((int)reader["PageId"], newStatus))
                                {
                                    while (reader2.Read())
                                    {
                                        if ((int)reader2["LangId"] == langId)
                                        {
                                            if (versionId != (int)reader2["VersionId"])
                                                PageVersion.UpdatePageVersion((int)reader2["VersionId"], (int)reader2["TemplateId"], (int)reader2["LangId"], (int)reader2["StatusId"], WorkflowStatus.GetArcStatus((int)reader2["StatusId"]), ProfileContext.Current.UserId, 2, "sent to archieve");
                                        }

                                    }
                                    reader2.Close();
                                }
                            }
                        }
                        reader.Close();
                    }
				}
			}
		}
	}
}