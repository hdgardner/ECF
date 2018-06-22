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

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
	public class VersionRejectHandler : ICommand
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
                        ProcessRejectCommand(items);

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

		void ProcessRejectCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int versionId = Int32.Parse(keys[0]);

					// delete selected version
                    int templateId = -1;
                    int statusId = -1;
                    using (IDataReader reader = PageVersion.GetVersionById(versionId))
                    {
                        if (reader.Read())
                        {
                            if (reader["TemplateId"] != DBNull.Value)
                            {
                                templateId = (int)reader["TemplateId"];
                            }
                            if (reader["StatusId"] != DBNull.Value)
                            {
                                statusId = (int)reader["StatusId"];
                            }
                        }
                        reader.Close();
                    }

                    int newStatus = -1;
                    newStatus = WorkflowAccess.GetPrevStatusId(statusId);
                    if (newStatus > 0)
                    {
                        PageVersion.UpdatePageVersion(versionId, templateId, CMSContext.Current.LanguageId,
                            statusId, newStatus, ProfileContext.Current.UserId, 2, "");
                    }
				}
			}
		}
	}
}